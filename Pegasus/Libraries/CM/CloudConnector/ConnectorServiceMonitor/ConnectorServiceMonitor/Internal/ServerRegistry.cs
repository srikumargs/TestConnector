using System;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Microsoft.Win32;
using Sage.CRE.HostingFramework.Proxy;
using Sage.Diagnostics;
using Sage.Net;
using ConnectorServiceMonitor.ViewModel;
using System.Reflection;

namespace ConnectorServiceMonitor.Internal
{
    internal sealed class ServerRegistry : IServerRegistry
    {
        public ServerRegistry(ServerRegistrationParams registrationParams)
        { _registrationParams = registrationParams; }

        #region IServerRegistry Members

        public bool IsConfigured
        { get { return DataStore.Create(_registrationParams.RegistrySubKeyPath).IsConfigured; } }

        public ServerConnectionTestResult TestCandidateServer(String hostAndPort)
        { return DetermineServerConnectionState(hostAndPort, String.Empty); }

        public RegistrationResponse RegisterServer(String hostAndPort)
        {
            DataStore dataStore = DataStore.Create(_registrationParams.RegistrySubKeyPath);

            String[] splitHostName = hostAndPort.Split(':');

            hostAndPort = splitHostName[0];
            Int32 portNumber = Convert.ToInt32(splitHostName[1]);

            String currentHostAndPort = dataStore.HostAndPort;

            // determine the fallback ip address; this may be a duplicate of the this.Host, if that
            // happens to be an ip address already
            IPHostEntry ipHostEntry = null;
            IPAddress fallbackIPAddress = null;
            if (!IPAddress.TryParse(hostAndPort, out fallbackIPAddress))
            {
                try
                {
                    ipHostEntry = Dns.GetHostEntry(hostAndPort);
                    fallbackIPAddress = GetIPv4AddressFromIPHostEntry(ipHostEntry);
                }
                catch (SocketException)
                {
                    InfoTrace.WriteLine(this, "Unable to resolve hostName '{0}' to IP address ... try again", hostAndPort);
                    // this situation can occur when network connectivity is first re-established
                }
            }
            else
            {
                InfoTrace.WriteLine(this, "hostName '{0}' is an IP address ... skipping Dns.GetHostEntry()", hostAndPort);
            }

            try
            {
                dataStore.FallbackIPAddress = fallbackIPAddress.ToString();
            }
            catch (SocketException ex)
            {
                VerboseTrace.WriteLine(this, "Received unexpected exception during Dns.GetHostByName:  {0}", ex.Message);
                // if we got here, then we simply cannot set the fallback address now ... should be a recoverable thing the next
                // time we do a DetermineServerConnectionState
            }


            dataStore.IsConfigured = true;
            dataStore.HostAndPort = String.Format("{0}:{1}", hostAndPort, portNumber);

            RegistrationResponse result = RegistrationResponse.Succeded;
            result |= (hostAndPort != currentHostAndPort) ? RegistrationResponse.ServerChangeDetected : 0;
            return result;
        }

        public String HostAndPort
        { get { return DataStore.Create(_registrationParams.RegistrySubKeyPath).HostAndPort; } }

        public String Host
        {
            get
            {
                String result = "<Unknown>";

                String serverAndPortNumber = HostAndPort;
                if (!String.IsNullOrEmpty(serverAndPortNumber))
                {
                    if (serverAndPortNumber.Contains(':'))
                    {
                        result = serverAndPortNumber.Substring(0, serverAndPortNumber.LastIndexOf(':'));
                    }
                }

                return result;
            }
        }

        public Int32 CatalogServicePort
        {
            get
            {
                Int32 result = -1;

                String serverAndPortNumber = HostAndPort;
                if (!String.IsNullOrEmpty(serverAndPortNumber))
                {
                    if (serverAndPortNumber.Contains(':'))
                    {
                        result = Convert.ToInt32(serverAndPortNumber.Substring(serverAndPortNumber.LastIndexOf(':') + 1, serverAndPortNumber.Length - serverAndPortNumber.LastIndexOf(':') - 1));
                    }
                }

                return result;
            }
        }

        public Int32 DefaultCatalogServicePort
        { get { return _registrationParams.DefaultCatalogServicePort; } }

        public HelpHandler HelpHandler
        { get { return _registrationParams.HelpHandler; } }

        public Boolean RunOnLogin
        {
            get { return DataStore.Create(_registrationParams.RegistrySubKeyPath).RunOnLogin; }
            set { DataStore.Create(_registrationParams.RegistrySubKeyPath).RunOnLogin = value; }
        }

        public Int32 AutoRefreshInterval
        {
            get { return DataStore.Create(_registrationParams.RegistrySubKeyPath).AutoRefreshInterval; }
            set { DataStore.Create(_registrationParams.RegistrySubKeyPath).AutoRefreshInterval = value; }
        }

        public Int32 RequestsShowing
        {
            get { return DataStore.Create(_registrationParams.RegistrySubKeyPath).RequestsShowing; }
            set { DataStore.Create(_registrationParams.RegistrySubKeyPath).RequestsShowing = value; }
        }

        #endregion

        #region Private methods
        private ServerConnectionTestResult DetermineServerConnectionState(String serverName, String fallbackIpAddress)
        {
            String[] splitServerName = serverName.Split(':');

            serverName = splitServerName[0];
            Int32 portNumber = Convert.ToInt32(splitServerName[1]);

            ServerConnectionTestResult result = null;

            ServerNameUsageRecommendation usageRecommendation = ServerNameUsageRecommendation.UseServerName;
            ServerConnectionState connectionState = ServerConnectionState.None;
            String serverNameResult = string.Empty;
            String serviceAddress = string.Empty;

            try
            {
                if (NetUtils.ServerNameIsLocalMachine(serverName))
                {
                    serverNameResult = IPAddress.Loopback.ToString();
                    InfoTrace.WriteLine(this, "serverName {0} is the local machine; overriding to just use loopback address {1}.", serverName, serverNameResult);

                    // we are running on what looks to be the server-machine
                    serviceAddress = GetMonitorServiceAddress(serverNameResult, portNumber);
                    if (!String.IsNullOrEmpty(serviceAddress))
                    {
                        InfoTrace.WriteLine(this, "Successfully found service address '{0}' on {1}.", serviceAddress, serverNameResult);
                        connectionState = ServerConnectionState.Connected;
                    }
                    else
                    {
                        InfoTrace.WriteLine(this, "Did not receive success response from GetServiceAddress {0}.", serverNameResult);
                        connectionState = ServerConnectionState.ServiceAddressNotFound;
                    }
                }
                else
                {
                    // we are running on a non-server machine ... we need to test connectivity
                    if (NetworkInterface.GetIsNetworkAvailable())
                    {
                        InfoTrace.WriteLine(this, "Network is currently available.");

                        // Test reachability of server machine by sending an ICMP.  This doesn't contact the HostingFramework ... it just
                        // tests whether the machine is up and running
                        IPHostEntry ipHostEntry = null;
                        IPAddress ipAddress = null;
                        if (!IPAddress.TryParse(serverName, out ipAddress))
                        {
                            try
                            {
                                ipHostEntry = Dns.GetHostEntry(serverName);
                            }
                            catch (SocketException)
                            {
                                InfoTrace.WriteLine(this, "Unable to resolve server '{0}' to IP address ... try again", serverName);
                                // this situation can occur when network connectivity is first re-established
                            }
                        }
                        else
                        {
                            InfoTrace.WriteLine(this, "serverName '{0}' is an IP address ... skipping Dns.GetHostEntry()", serverName);
                        }

                        String resolvedServerName = serverNameResult = serverName;
                        if (ipHostEntry == null)
                        {
                            if (String.IsNullOrEmpty(fallbackIpAddress))
                            {
                                if (ipAddress != null)
                                {
                                    InfoTrace.WriteLine(this, "serverName '{0}' is an IP address;  recommend use server name", serverName);
                                    usageRecommendation = ServerNameUsageRecommendation.UseServerName;
                                }
                                else
                                {
                                    InfoTrace.WriteLine(this, "serverName '{0}' could not be resolved, is not an IP address, no fallback IP address provided;  unable to connect to server;  recommend use server name (for next attempt)", serverName);
                                    usageRecommendation = ServerNameUsageRecommendation.UseServerName;
                                    resolvedServerName = String.Empty;
                                }
                            }
                            else
                            {
                                InfoTrace.WriteLine(this, "serverName '{0}' could not be resolved, but fallback IP address '{1}' provided;  recommending use fallback IP address", serverName, fallbackIpAddress);
                                usageRecommendation = ServerNameUsageRecommendation.UseFallbackIPAddress;
                                resolvedServerName = fallbackIpAddress;
                            }
                        }
                        else
                        {
                            IPAddress ipV4Address = GetIPv4AddressFromIPHostEntry(ipHostEntry);
                            if (ipV4Address.ToString() != fallbackIpAddress)
                            {
                                InfoTrace.WriteLine(this, "fallback IP address '{0}' is different from resolved address '{1}';  recommending use server name;  update fallback IP address", fallbackIpAddress, ipV4Address.ToString());
                                DataStore.Create(_registrationParams.RegistrySubKeyPath).FallbackIPAddress = ipV4Address.ToString();
                                usageRecommendation = ServerNameUsageRecommendation.UseServerName;
                                resolvedServerName = ipV4Address.ToString();
                            }
                            else
                            {
                                InfoTrace.WriteLine(this, "serverName '{0}' successfully resolved to the same value as the provisioned fallback IP address '{1}';  recommending use server name", serverName, fallbackIpAddress);
                                usageRecommendation = ServerNameUsageRecommendation.UseServerName;
                                resolvedServerName = ipV4Address.ToString();
                            }
                        }


                        // We now know what address we are going to try ... attempt to communicate with the server.
                        //  - ICMP ping
                        //  - CatalogService lookup of MonitorService address

                        if (String.IsNullOrEmpty(resolvedServerName))
                        {
                            InfoTrace.WriteLine(this, "Unable to resolve server '{0}' to IP address;  no fallback IP address available", serverName);
                            connectionState = ServerConnectionState.CouldNotResolveServer;
                        }
                        else
                        {
                            using (Ping icmpPing = new Ping())
                            {
                                PingReply icmpPingReply = icmpPing.Send(resolvedServerName, _icmpPingTimeout);
                                if (icmpPingReply.Status == IPStatus.Success)
                                {
                                    InfoTrace.WriteLine(this, "Received success response to ICMP ping from {0}.", resolvedServerName);

                                    // The machine is up and running, now attempt to contact the server-mode ServiceHost on that machine.

                                    serviceAddress = GetMonitorServiceAddress(resolvedServerName, portNumber);
                                    if (!String.IsNullOrEmpty(serviceAddress))
                                    {
                                        InfoTrace.WriteLine(this, "Successfully found service address '{0}' on {1}.", serviceAddress, resolvedServerName);
                                        connectionState = ServerConnectionState.Connected;
                                    }
                                    else
                                    {
                                        InfoTrace.WriteLine(this, "Did not receive success response from GetServiceAddress {0}.", resolvedServerName);
                                        connectionState = ServerConnectionState.ServiceAddressNotFound;
                                    }
                                }
                                else
                                {
                                    InfoTrace.WriteLine(this, "Did not receive success response to ICMP ping from {0}.", resolvedServerName);
                                    connectionState = ServerConnectionState.ServerNotResponding;
                                }
                            }
                        }
                    }
                    else
                    {
                        InfoTrace.WriteLine(this, "Network is currently not available.");
                        connectionState = ServerConnectionState.NetworkUnavailable;
                    }
                }
            }
            finally
            {
                result = new ServerConnectionTestResult(connectionState, usageRecommendation, String.Format("{0}:{1}", serverNameResult, portNumber), serviceAddress);
            }

            return result;
        }

        private IPAddress GetIPv4AddressFromIPHostEntry(IPHostEntry ipHostEntry)
        {
            IPAddress result = null;

            foreach (IPAddress ipAddress in ipHostEntry.AddressList)
            {
                InfoTrace.WriteLine(this, "ipHostEntry.HostName '{0}' has IP address '{1}' ({2})", ipHostEntry.HostName, ipAddress.ToString(), ipAddress.AddressFamily);
                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    result = ipAddress;
                }
            }

            return result;
        }

        private String GetMonitorServiceAddress(String serverName, Int32 portNumber)
        {
            string result = string.Empty;
            try
            {
                using(var proxy = CatalogServiceProxyFactory.CreateFromCatalog(serverName, portNumber))
                {
                    var serviceInfo = proxy.GetServiceInfoByName("MonitorService");
                    result = serviceInfo.Uris.First().ToString();
                }
            }
            catch (Exception ex)
            {
                // Since the purpose of this method is to essentially ask a question, we eat all exceptions here in order to provide a simple
                // yes/no answer.  Known exceptions:
                //
                // - if the server-mode service host is not running, then we could get an exception
                InfoTrace.WriteLine(this, ex.Message);
            }

            return result;
        }

        #endregion

        private class DataStore
        {
            public static DataStore Create(String registrySubKeyPath)
            { return new DataStore(registrySubKeyPath); }

            private DataStore(String registrySubKeyPath)
            {
                _registrySubKeyPath = registrySubKeyPath;
            }

            public Boolean IsConfigured
            {
                get { return ReadBoolean(_registrySubKeyPath, IS_CONFIGURED, false); }
                set { WriteString(_registrySubKeyPath, IS_CONFIGURED, Convert.ToString(value)); }
            }

            public String HostAndPort
            {
                get { return ReadString(_registrySubKeyPath, HOST_AND_PORT, "<Unknown>"); }
                set { WriteString(_registrySubKeyPath, HOST_AND_PORT, value); }
            }

            public String FallbackIPAddress
            {
                get { return ReadString(_registrySubKeyPath, FALLBACK_IPADDRESS, String.Empty); }
                set { WriteString(_registrySubKeyPath, FALLBACK_IPADDRESS, value); }
            }

            public Int32 AutoRefreshInterval
            {
                get { return ReadInt32(_registrySubKeyPath, "AutoRefreshInterval", 0); }
                set { WriteInt32(_registrySubKeyPath, "AutoRefreshInterval", value); }
            }

            public Int32 RequestsShowing
            {
                get { return ReadInt32(_registrySubKeyPath, "RequestsShowing", 1); }
                set { WriteInt32(_registrySubKeyPath, "RequestsShowing", value); }
            }

            public Boolean RunOnLogin
            {
                get
                {
                    // call the property setter to explicity cause Windows registry to be populated
                    return Link.Exists(Environment.SpecialFolder.Startup, Common.MonitorBriefProductName);
                }
                set
                {
                    // populate Windows registry to get the indicated behavior
                    var myLocation = Assembly.GetEntryAssembly().Location;
                    Link.Update(Environment.SpecialFolder.Startup, myLocation, Common.MonitorBriefProductName, value);
                }
            }

            private static Boolean ReadBoolean(String registrySubKeyPath, String valueName, Boolean defaultValue)
            {
                Boolean result = defaultValue;

                RegistryKey key = Registry.CurrentUser.OpenSubKey(registrySubKeyPath);
                if (key != null && key.GetValue(valueName) != null)
                {
                    result = Boolean.Parse((String)key.GetValue(valueName));
                }
                else
                {
                    key = Registry.CurrentUser.CreateSubKey(registrySubKeyPath);
                    PopulateDefaults(key);
                    key.SetValue(valueName, defaultValue);
                }

                return result;
            }

            private static String ReadString(String registrySubKeyPath, String valueName, String defaultValue)
            {
                String result = defaultValue;

                RegistryKey key = Registry.CurrentUser.OpenSubKey(registrySubKeyPath);
                if (key != null && key.GetValue(valueName) != null)
                {
                    result = (String)key.GetValue(valueName);
                }
                else
                {
                    key = Registry.CurrentUser.CreateSubKey(registrySubKeyPath);
                    PopulateDefaults(key);
                    key.SetValue(valueName, defaultValue);
                }

                return result;
            }

            private static Int32 ReadInt32(String registrySubKeyPath, String valueName, Int32 defaultValue)
            {
                Int32 result = defaultValue;

                RegistryKey key = Registry.CurrentUser.OpenSubKey(registrySubKeyPath);
                if (key != null && key.GetValue(valueName) != null)
                {
                    result = (Int32)key.GetValue(valueName);
                }
                else
                {
                    key = Registry.CurrentUser.CreateSubKey(registrySubKeyPath);
                    PopulateDefaults(key);
                    key.SetValue(valueName, defaultValue);
                }

                return result;
            }

            private static void PopulateDefaults(RegistryKey key)
            {
                var myLocation = Assembly.GetEntryAssembly().Location;
                Link.Update(Environment.SpecialFolder.Startup, myLocation, Common.MonitorBriefProductName, true);
                key.SetValue(IS_CONFIGURED, Convert.ToString(false));
                key.SetValue(HOST_AND_PORT, "<unknown>");
                key.SetValue(FALLBACK_IPADDRESS, String.Empty);
                key.SetValue("AutoRefreshInterval", 0);
                key.SetValue("RequestsShowing", 1);
            }

            private static void WriteString(String registrySubKeyPath, String valueName, String value)
            { Registry.CurrentUser.CreateSubKey(registrySubKeyPath).SetValue(valueName, value); }

            private static void WriteInt32(String registrySubKeyPath, String valueName, Int32 value)
            { Registry.CurrentUser.CreateSubKey(registrySubKeyPath).SetValue(valueName, value); }

            private String _registrySubKeyPath;
        }

        private const String IS_CONFIGURED = "IsConfigured";
        private const String HOST_AND_PORT = "HostAndPort";
        private const String FALLBACK_IPADDRESS = "ServerFallbackIPAddress";


        //_icmpPingTimeout is the timeout value to use (in milliseconds) for the ICMP ping
        private readonly int _icmpPingTimeout = 5000;                                                          // = 0; (automatically initialized by runtime)
        private readonly ServerRegistrationParams _registrationParams;
    }
}
