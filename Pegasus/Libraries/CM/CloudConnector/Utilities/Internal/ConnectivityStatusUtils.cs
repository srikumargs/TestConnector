using System;
using System.IO;
using System.Net;
using Sage.Connector.Common;
using Sage.Connector.Logging;
using Sage.ServiceModel;

namespace Sage.Connector.Utilities.Internal
{
    internal sealed class ConnectivityStatusUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public static Boolean BasicTcpConnectivityTest(Uri uri, Int32 timeout)
        {
            Boolean result = false;

            try
            {
                TcpHelper.TestConnect(uri, timeout);
                result = true;
            }
            catch (Exception ex)
            {
                using (var lm = new LogManager())
                {
                    lm.WriteError(null, ex.ExceptionAsString());
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static MicrosoftNCSITestResult MicrosoftNCSITest()
        {
            MicrosoftNCSITestResult result = MicrosoftNCSITestResult.None;
            try
            {
                // Appendix K: Network Connectivity Status Indicator and Resulting Internet Communication in Windows Vista
                // http://technet.microsoft.com/en-us/library/cc766017(v=ws.10).aspx

                // setup NCSI usage defaults in case not present (e.g., XP)
                String activeDnsProbeContent = "131.107.255.255";
                String activeDnsProbeHost = "dns.msftncsi.com";
                String activeWebProbeContent = "Microsoft NCSI";
                String activeWebProbeHost = "www.msftncsi.com";
                String activeWebProbePath = "ncsi.txt";

                // Consider whether we want to respect the current values set in the Registry
                // (and possibly tweaked by some paranoid user) for the OS-level network 
                // connectivity status indicator
                //using (var subKey = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Services\NlaSvc\Parameters\Internet", false))
                //{
                //    // now override defaults with OS-supplied values
                //    activeDnsProbeContent = (String)subKey.GetValue("ActiveDnsProbeContent", activeDnsProbeContent);
                //    activeDnsProbeHost = (String)subKey.GetValue("ActiveDnsProbeHost", activeWebProbeHost);
                //    activeWebProbeContent = (String)subKey.GetValue("ActiveWebProbeContent", activeWebProbeContent);
                //    activeWebProbeHost = (String)subKey.GetValue("ActiveWebProbeHost", activeWebProbeHost);
                //    activeWebProbePath = (String)subKey.GetValue("ActiveWebProbePath", activeWebProbePath);
                //}

                Uri ncsiUri = new Uri(String.Format("http://{0}/{1}", activeWebProbeHost, activeWebProbePath));

                if (BasicTcpConnectivityTest(ncsiUri, ConnectorRegistryUtils.InternetConnectivityStatusTcpConnectTimeout))
                {
                    result = MicrosoftNCSITestResult.Step1_TcpConnectionTested;

                    var ipHostEntry = Dns.GetHostEntry(activeDnsProbeHost);
                    result = MicrosoftNCSITestResult.Step2_DnsGetHostEntry;

                    if (ipHostEntry.AddressList != null &&
                        ipHostEntry.AddressList.Length > 0 &&
                        ipHostEntry.AddressList[0].ToString() == IPAddress.Parse(activeDnsProbeContent).ToString() &&
                        ipHostEntry.HostName == activeDnsProbeHost)
                    {
                        result = MicrosoftNCSITestResult.Step3_DnsGetHostEntryValueCompare;

                        var request = WebRequest.Create(ncsiUri);
                        result = MicrosoftNCSITestResult.Step4_WebRequestCreated;

                        using (var response = request.GetResponse())
                        {
                            result = MicrosoftNCSITestResult.Step5_ResponseReceived;

                            Stream stream = null;
                            try
                            {
                                stream = response.GetResponseStream();
                                result = MicrosoftNCSITestResult.Step6_ResponseStreamReceived;

                                using (var reader = new StreamReader(stream))
                                {
                                    result = MicrosoftNCSITestResult.Step7_StreamReaderCreated;

                                    stream = null;
                                    String responseFromServer = reader.ReadToEnd();
                                    result = MicrosoftNCSITestResult.Step8_ResponseReadToEnd;

                                    if (responseFromServer == activeWebProbeContent)
                                    {
                                        result = MicrosoftNCSITestResult.Success;
                                    }
                                }
                            }
                            finally
                            {
                                if (stream != null)
                                {
                                    stream.Dispose();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                using (var lm = new LogManager())
                {
                    lm.WriteError(null, ex.ExceptionAsString());
                }
            }

            return result;
        }
    }
}
