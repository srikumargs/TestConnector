using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Security.Principal;

// This source file resides in the "LinkedSource" source code folder in order to enable
// multiple assemblies to share the implementation without requiring the class to be exposed as a
// public type of any shared assembly.
//
// Requires:
//  - %SAGE_SANDBOX%\Libraries\Sandbox\Tools\LinkedSource\FolderManager.cs
//  - %SAGE_SANDBOX%\Libraries\CRE\HostingFramework\LinkedSource\LibraryConstants.cs
using Sage.CM.HostingFramework.LinkedSource;

namespace Sage.CRE.HostingFramework.LinkedSource
{
    internal static class ServiceConstantsFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostingFrameworkServiceExeFilePath"></param>
        /// <returns></returns>
        public static ServiceConstants GetConstants(String hostingFrameworkServiceExeFilePath)
        {
            ServiceConstants result;

            String hostingFrameworkServiceAssemblyName = Path.GetFileNameWithoutExtension(hostingFrameworkServiceExeFilePath);
            lock (_lockObject)
            {
                if (!_serviceConstantsByAssemblyLocation.TryGetValue(hostingFrameworkServiceExeFilePath, out result))
                {
                    result = new ServiceConstants(hostingFrameworkServiceExeFilePath);
                    _serviceConstantsByAssemblyLocation.Add(hostingFrameworkServiceExeFilePath, result);
                }
            }

            return result;
        }

        private static Dictionary<String, ServiceConstants> _serviceConstantsByAssemblyLocation = new Dictionary<String, ServiceConstants>();
        public static Object _lockObject = new Object();
    }

    /// <summary>
    /// Internal string constants 
    /// </summary>
    internal class ServiceConstants
    {
        private static Boolean IsUserAdmin()
        {
            WindowsIdentity id = WindowsIdentity.GetCurrent();
            WindowsPrincipal p = new WindowsPrincipal(id);
            return p.IsInRole(WindowsBuiltInRole.Administrator);
        }

        internal ServiceConstants(String hostingFrameworkServiceExeFilePath)
        {
            String hostingFrameworkServiceFileName = Path.GetFileNameWithoutExtension(hostingFrameworkServiceExeFilePath);
            _exeFilePath = hostingFrameworkServiceExeFilePath;
            _fileName = Path.GetFileName(_exeFilePath);
            String fileNameWithoutExtension = Path.GetFileNameWithoutExtension(_fileName);
            _instanceConfigFilePath = Path.Combine(Path.GetDirectoryName(_exeFilePath), String.Format("{0}-InstanceConfig.xml", fileNameWithoutExtension));
            _instanceConfigDefaultsFilePath = Path.Combine(Path.GetDirectoryName(_exeFilePath), String.Format("{0}-InstanceConfigDefaults.xml", fileNameWithoutExtension));

            // We want these values to come from a file that is not installed, so the the end-user can change them
            // without Windows Installer thinking that the file needs repair ... and without the end-user customizations
            // getting stomped on during an upgrade.  Only the DefaultInstanceConfig.xml should be installed.  The
            // InstanceConfig.xml should be copied on the fly if it doesn't exist ... but only if the user is an Admin.
            // If they are not an Admin, then just attempt to use the defaults file instead ... under the assumption
            // that the InstanceConfig.xml will get copied later (under an Admin context).
            if (!File.Exists(_instanceConfigFilePath))
            {
                if (IsUserAdmin())
                {
                    File.Copy(_instanceConfigDefaultsFilePath, _instanceConfigFilePath);
                }
                else
                {
                    _instanceConfigFilePath = _instanceConfigDefaultsFilePath;
                }
            }

            if (!File.Exists(_instanceConfigFilePath))
            {
                throw new FileNotFoundException("Unable to find Hosting Framework instance configuration file.", _instanceConfigFilePath);
            }

            XDocument doc = XDocument.Load(_instanceConfigFilePath);

            Int32 version = Convert.ToInt32(doc.Descendants("InstanceConfig").Single().Attribute("Version").Value, CultureInfo.InvariantCulture);
            if (version != CURRENT_CONFIG_FILE_VERSION)
            {
                UpgradeConfigFile(doc, _instanceConfigFilePath, _instanceConfigDefaultsFilePath);
            }

            String baseName = ReadValue(doc, "BaseName");
            _version = ReadValue(doc, "Version");
            _name = String.Format("{0}.{1}", baseName, _version);

            _displayName = ReadValue(doc, "DisplayName");
            _description = ReadValue(doc, "Description");
            _notifyHostReadyTimeout = Convert.ToInt32(ReadValue(doc, "NotifyHostReadyTimeout"), CultureInfo.InvariantCulture);
            _serviceProcessRunningMutexName = String.Format(CultureInfo.InvariantCulture, @"Global\__{0}_RUNNING", _name);
            _serviceReadyMutexName = String.Format(CultureInfo.InvariantCulture, @"Global\__{0}_READY", _name);
            _serviceStartErrorResultFilename = String.Format(CultureInfo.InvariantCulture, @"{0}_STARTUP_ERROR", _name);

            // Service account user and password
            _serviceAccountUser = ReadValueOrDefault(doc, "User");
            _serviceAccountPassword = ReadValueOrDefault(doc, "Password");

            // Other service settings
            _serviceStartType = ReadValueOrDefault(doc, "StartType");
            _serviceFirstFailureAction = ReadValueOrDefault(doc, "FirstFailure");
            _serviceSecondFailureAction = ReadValueOrDefault(doc, "SecondFailure");
            _serviceSubsequentFailureAction = ReadValueOrDefault(doc, "SubsequentFailure");
            _serviceDelayedStart = ReadValueOrDefault(doc, "DelayedStart");

            foreach (XElement x in doc.Descendants("BaseAddress"))
            {
                String id = x.Attribute("Id").Value;
                _baseAddressTemplates.Add(id, ReadValue(x, "AddressTemplate"));
                String startingPort = ReadValue(x, "StartingPort");
                _baseAddressTemplatePorts.Add(id, startingPort);

                // FIXME:  this just picks up the first non-null BaseAddress starting port, is this ok?
                if (!String.IsNullOrEmpty(startingPort))
                {
                    _catalogServicePortNumber = Convert.ToInt32(startingPort, CultureInfo.InvariantCulture);
                }
            }

            String removeUsersPermissionToAppDataAsString = ReadValueOrDefault(doc, "RemoveUsersPermissionToAppData");
            if (!String.IsNullOrEmpty(removeUsersPermissionToAppDataAsString))
            {
                _removeUsersPermissionToAppData = Boolean.Parse(removeUsersPermissionToAppDataAsString);
            }

            String blockApplicationIncomingConnectionsAsString = ReadValueOrDefault(doc, "BlockApplicationIncomingConnections");
            if (!String.IsNullOrEmpty(blockApplicationIncomingConnectionsAsString))
            {
                _blockApplicationIncomingConnections = Boolean.Parse(blockApplicationIncomingConnectionsAsString);
            }

            _instanceApplicationDataFolder = Path.Combine(LibraryConstants.VersionIndependent.LibraryApplicationDataFolder(Sage.Sandbox.Tools.LinkedSource.FolderManager.SageCommonApplicationDataLocation), Name);
        }

        #region Public properties
        /// <summary>
        /// The service name
        /// </summary>
        public String Name
        { get { return _name; } }

        /// <summary>
        /// The service version
        /// </summary>
        public String Version
        { get { return _version; } }

        /// <summary>
        /// The display name of the service
        /// </summary>
        public String DisplayName
        { get { return _displayName; } }

        /// <summary>
        /// The file name (i.e., without the path) of the service EXE
        /// </summary>
        public String FileName
        { get { return _fileName; } }

        /// <summary>
        /// The name of the service process running mutex
        /// </summary>
        public String ServiceProcessRunningMutexName
        { get { return _serviceProcessRunningMutexName; } }

        /// <summary>
        /// The name of the service-ready mutex
        /// </summary>
        public String ServiceReadyMutexName
        { get { return _serviceReadyMutexName; } }

        /// <summary>
        /// File name for service error file
        /// </summary>
        public String ServiceStartErrorResultFilename
        { get { return _serviceStartErrorResultFilename; } }

        /// <summary>
        /// The full path to the service EXE
        /// </summary>
        public String ExeFilePath
        { get { return _exeFilePath; } }

        /// <summary>
        /// The description text for the service in the SCM
        /// </summary>
        public String Description
        { get { return _description; } }

        /// <summary>
        /// The timeout value to use (in milliseconds) when attempting to notify clients that the host is ready 
        /// </summary>
        public Int32 NotifyHostReadyTimeout
        { get { return _notifyHostReadyTimeout; } }

        public Dictionary<String, String> BaseAddressTemplates
        { get { return _baseAddressTemplates; } }

        public Dictionary<String, String> BaseAddressTemplatePorts
        { get { return _baseAddressTemplatePorts; } }

        /// <summary>
        /// The path to the instance-specific Application Data folder.
        /// </summary>
        public String InstanceApplicationDataFolder
        { get { return _instanceApplicationDataFolder; } }

        /// <summary>
        /// The path to the instance-specific configuration file.
        /// </summary>
        /// <remarks>
        /// This file should NOT be installed.  It is copied at run-time (if it doesn't exist) from the default file.
        /// </remarks>
        public String InstanceConfigFilePath
        { get { return _instanceConfigFilePath; } }

        /// <summary>
        /// The path to the instance-specific default configuration file.
        /// </summary>
        /// <remarks>
        /// This file should be installed.
        /// </remarks>
        public String InstanceConfigDefaultsFilePath
        { get { return _instanceConfigDefaultsFilePath; } }

        /// <summary>
        /// The port number of the Catalog Service, this port is the only "well-known" port.  All others must be discovered through the catalog service.
        /// </summary>
        public Int32 CatalogServicePortNumber
        { get { return _catalogServicePortNumber; } }

        /// <summary>
        /// Whether the Users group should have all AppData permission revoked
        /// </summary>
        public Boolean RemoveUsersPermissionToAppData
        { get { return _removeUsersPermissionToAppData; } }

        /// <summary>
        /// Whether the Windows Firewall should be configured to block all (non-local) incoming connections
        /// </summary>
        public Boolean BlockApplicationIncomingConnections
        { get { return _blockApplicationIncomingConnections; } }

        /// <summary>
        /// User to install and run the service as
        /// Can be a stock account such as Network Service
        /// </summary>
        public String ServiceAccountUser
        { get { return _serviceAccountUser; } }

        /// <summary>
        /// Password for the provided service account user
        /// Not applicable for stock accounts such as Network Service
        /// </summary>
        public String ServiceAccountPassword
        { get { return _serviceAccountPassword; } }

        /// <summary>
        /// The start type to use for the service
        /// </summary>
        public String ServiceStartType
        { get { return _serviceStartType; } }

        /// <summary>
        /// The action to take on first failue of the service
        /// </summary>
        public String ServiceFirstFailureAction
        { get { return _serviceFirstFailureAction; } }

        /// <summary>
        /// The action to take on second failure of the service
        /// </summary>
        public String ServiceSecondFailureAction
        { get { return _serviceSecondFailureAction; } }

        /// <summary>
        /// The action to take on all subsequent failures of the service
        /// After the first and second
        /// </summary>
        public String ServiceSubsequentFailureAction
        { get { return _serviceSubsequentFailureAction; } }

        /// <summary>
        /// Whether or not to delay an auto start service
        /// </summary>
        public String ServiceDelayedStart
        { get { return _serviceDelayedStart; } }

        #endregion

        private String ReadValue(XDocument doc, String valueName)
        { return ReadValue(doc.Root, valueName); }

        private String ReadValue(XElement doc, String valueName)
        { return doc.Descendants(valueName).Single().Value; }

        private String ReadValueOrDefault(XDocument doc, String valueName)
        { return ReadValueOrDefault(doc.Root, valueName); }

        private String ReadValueOrDefault(XElement doc, String valueName)
        {
            String result = String.Empty;

            var element = doc.Descendants(valueName).SingleOrDefault();
            if (element != null)
            {
                result = element.Value;
            }

            return result;
        }

        private static void UpgradeConfigFile(XDocument oldConfigFile, String instanceConfigDefaultsFilePath, String instanceConfigFilePath)
        {
            // TODO: implement upgrade when config file schema changes; read oldConfigFile
            //XDocument doc = new XDocument();
            //doc.Add(new XElement("InstanceConfig", new XAttribute("Version", CURRENT_CONFIG_FILE_VERSION),
            //    new XElement("BaseName", "Sage.CRE.HostingFramework.Service"),
            //    new XElement("Version", "3.2"),
            //    new XElement("DisplayName", "Sage CRE Hosting Framework"),
            //    new XElement("Description", "Provides a generic framework for hosting Sage CRE service-oriented components."),
            //    new XElement("NotifyHostReadyTimeout", "10000"),
            //    new XElement("BaseAddresses",
            //        new XAttribute("Id", "tcp"),
            //        new XElement("AddressTemplate", "net.tcp://localhost:{0}"),
            //             new XElement("StartingPort", "48620"))));
            //doc.Save(instanceConfigFilePath);
        }

        private const Int32 CURRENT_CONFIG_FILE_VERSION = 1;

        #region Private fields
        private readonly String _name;
        private readonly String _version;
        private readonly String _displayName;
        private readonly String _fileName;
        private readonly String _serviceProcessRunningMutexName;
        private readonly String _serviceReadyMutexName;
        private readonly String _exeFilePath;
        private readonly String _description;
        private readonly Int32 _notifyHostReadyTimeout;
        private readonly Dictionary<String, String> _baseAddressTemplates = new Dictionary<String, String>();
        private readonly Dictionary<String, String> _baseAddressTemplatePorts = new Dictionary<String, String>();
        private readonly String _instanceApplicationDataFolder;
        private readonly String _instanceConfigFilePath;
        private readonly String _instanceConfigDefaultsFilePath;
        private readonly Int32 _catalogServicePortNumber;
        private readonly Boolean _removeUsersPermissionToAppData;
        private readonly Boolean _blockApplicationIncomingConnections;
        private readonly String _serviceAccountUser;
        private readonly String _serviceAccountPassword;
        private readonly String _serviceStartType;
        private readonly String _serviceFirstFailureAction;
        private readonly String _serviceSecondFailureAction;
        private readonly String _serviceSubsequentFailureAction;
        private readonly String _serviceStartErrorResultFilename;
        private readonly String _serviceDelayedStart;

        #endregion
    }
}
