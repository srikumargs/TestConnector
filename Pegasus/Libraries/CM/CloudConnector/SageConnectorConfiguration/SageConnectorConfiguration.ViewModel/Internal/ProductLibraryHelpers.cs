using System;
using System.Collections;
using System.Linq;
using Microsoft.Win32;
using System.Reflection;
using System.Collections.Generic;
using System.Diagnostics;
using SageConnectorConfiguration.Model;

namespace SageConnectorConfiguration.ViewModel
{
    /// <summary>
    /// Validation Responses with Failure Explanation
    /// </summary>
    public class ValidationResponse
    {
        /// <summary>
        /// Whether the validation was successful
        /// </summary>
        public Boolean Success { get; set; }
        /// <summary>
        /// The error to display to the user
        /// </summary>
        public String UserFacingError { get; set; }
        /// <summary>
        /// The internal diagnostic error
        /// </summary>
        public String InternalError { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ProductLibraryHelpers
    {
        #region Private Run-Time Assembly Management
        private static Assembly _dataAssembly;
        private static Assembly _commonAssembly; 
        private static Assembly _utilitiesAssembly;

        private static Assembly DataAssembly
        {
            get
            {
                if (null == _dataAssembly)
                {
                    _dataAssembly = LoadConnectorAssembly("Sage.Connector.Data.DLL");
                }
                return _dataAssembly;
            }
        }

        private static Assembly CommonAssembly
        {
            get 
            {
                if (null == _commonAssembly)
                {
                    _commonAssembly = LoadConnectorAssembly("Sage.Connector.Common.DLL");
                }
                return _commonAssembly;
            }
        }

        private static Assembly UtilitiesAssembly
        {
            get
            {
                if (null == _utilitiesAssembly)
                {
                    _utilitiesAssembly = LoadConnectorAssembly("Sage.Connector.Utilities.DLL");
                }
                return _utilitiesAssembly;
            }
        }

        private static string BaseConnectorPath
        {
            get
            {
                using (var scaKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Sage\SageConnector\SCA", false))
                {
                    if (null != scaKey)
                        return scaKey.GetValue("InstallPath") as String;
                }

                return System.IO.Directory.GetCurrentDirectory();
            }
        }

        private static Assembly LoadConnectorAssembly(string assemblyFileName)
        {
            string connectorAssemblyFullPath = System.IO.Path.Combine(BaseConnectorPath, assemblyFileName);
            return Assembly.LoadFrom(connectorAssemblyFullPath);
        }

        private static Type ConfigurationHelpersType
        {
            get
            {
                return UtilitiesAssembly.GetType("Sage.Connector.Utilities.ConfigurationHelpers", true);
            }
        }
        #endregion

        #region Plugin Management

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<String> InstalledConnectorPluginIds()
        {
            MethodInfo existMethod = ConfigurationHelpersType.GetMethod("InstalledConnectorPluginIds", BindingFlags.Public | BindingFlags.Static, null, new Type[] {}, null);
            var retVal = existMethod.Invoke(null, new object[] {}) as IEnumerable<String>;
            return retVal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pluginID"></param>
        /// <returns></returns>
        public static String PluginProductName(string pluginID)
        {
            MethodInfo existMethod = ConfigurationHelpersType.GetMethod("PluginProductName", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string) }, null);
            var retVal = existMethod.Invoke(null, new object[] { pluginID });
            return retVal.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pluginID"></param>
        /// <returns></returns>
        public static String PluginHelpBaseURL(string pluginID)
        {
            MethodInfo existMethod = ConfigurationHelpersType.GetMethod("PluginHelpBaseURL", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string) }, null);
            var retVal = existMethod.Invoke(null, new object[] { pluginID });
            return retVal.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pluginID"></param>
        /// <returns></returns>
        public static String PluginAdministratorTerm(string pluginID)
        {
            MethodInfo existMethod = ConfigurationHelpersType.GetMethod("PluginAdministratorTerm", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string) }, null);
            var retVal = existMethod.Invoke(null, new object[] { pluginID });
            return retVal.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pluginID"></param>
        /// <returns></returns>
        public static String PluginAdministratorAccountNamePrefill(string pluginID)
        {
            MethodInfo existMethod = ConfigurationHelpersType.GetMethod("PluginAdministratorAccountNamePrefill", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string) }, null);
            var retVal = existMethod.Invoke(null, new object[] { pluginID });
            return retVal.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pluginID"></param>
        /// <returns></returns>
        public static Boolean PluginAdministratorAccountNameReadOnly(string pluginID)
        {
            MethodInfo existMethod = ConfigurationHelpersType.GetMethod("PluginAdministratorAccountNameReadOnly", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string) }, null);
            var retVal = existMethod.Invoke(null, new object[] { pluginID });
            return Convert.ToBoolean(retVal);
        }

        #endregion

        #region Detection Management

        private static Uri ComponentIdentificationSpecifications
        {
            get
            {
                return new Uri(@"http://cdn.updates.timberline.com/SoftwareAlerts/Sage.CRE.ComponentIdentification/DetectProductInstructions.xml");
            }
        }

        private static string ComponentIdentificationSpecificationsStorageLocation
        {
            get
            {
                var currentDirectory = System.IO.Directory.GetCurrentDirectory();
                return System.IO.Path.Combine(currentDirectory, "DetectProductInstructions.xml");
            }
        }

        private static Sage.CRE.ComponentIdentification.Detector LoadDetector()
        {
            if (System.IO.File.Exists(ComponentIdentificationSpecificationsStorageLocation))
                return
                    Sage.CRE.ComponentIdentification.Detector.LoadFromXml(
                        ComponentIdentificationSpecificationsStorageLocation, true);

            return Sage.CRE.ComponentIdentification.Detector.LoadFromWeb(ComponentIdentificationSpecifications,
                                                                         ComponentIdentificationSpecificationsStorageLocation);
        }

        private static Boolean ComponentProductInstalled(string productId, out Version productVersion)
        {
            var detector = LoadDetector();
            var detectedProducts = detector.DetectProducts();
            var detectedProduct = detectedProducts.FirstOrDefault(p => p.ProductId == productId);
            productVersion = null;

            // Registry detection require version verification
            if (null != detectedProduct)
            {
                var versionProperty = detectedProduct.Properties.FirstOrDefault(p => p.Key == "Version");
                if (versionProperty.Key == "Version")
                {
                    if (Version.TryParse(versionProperty.Value, out productVersion))
                        return true;
                    return !versionProperty.Value.Contains("registrypath");
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="packageVersionString"></param>
        /// <returns></returns>
        public static Boolean InstallRequired(string packageVersionString)
        {
            Version currentProductVersion = null;
            if (ComponentProductInstalled("SCAConnector", out currentProductVersion))
            {
                Version packageVersion = null;
                if (Version.TryParse(packageVersionString, out packageVersion))
                {
                    return (packageVersion > currentProductVersion);
                }

                // Unable to determine the current package version, bypass install
                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Boolean BackOfficeInstalled()
        {
            Version productVersion = null;
            return ComponentProductInstalled("Sage100Cont", out productVersion) || ComponentProductInstalled("Sage300CRE", out productVersion);
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static Boolean InstallService(string user, string password)
        {
            MethodInfo installServiceMethod = ConfigurationHelpersType.GetMethod("InstallService", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string), typeof(string) }, null);
            var retVal = installServiceMethod.Invoke(null, new object[] { user, password });
            Boolean bRetVal = Convert.ToBoolean(retVal);

            // Configure Monitor for local user / local machine access
            if (bRetVal)
            {
                ProcessStartInfo psi = new ProcessStartInfo(LocalMonitorExecutablePath);
                psi.Arguments = "CONFIGUREFORLOCAL";
                Process myProcess = Process.Start(psi);
                myProcess.WaitForExit();
            }

            return bRetVal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionKey"></param>
        /// <param name="tenantId"></param>
        /// <param name="premiseKey"></param>
        /// <param name="siteAddress"></param>
        public static void DecodeConnectionKey(String connectionKey, out String tenantId, out String premiseKey, out String siteAddress)
        {
            MethodInfo decodeMethod = ConfigurationHelpersType.GetMethod("DecodeConnectionKey", BindingFlags.Public | BindingFlags.Static, null,
                new Type[] { typeof(String), typeof(String).MakeByRefType(), typeof(String).MakeByRefType(), typeof(String).MakeByRefType() }, null);
            var inputParams = new object[] { connectionKey, null, null, null };
            decodeMethod.Invoke(null, inputParams);
            tenantId = inputParams[1].ToString();
            premiseKey = inputParams[2].ToString();
            siteAddress = inputParams[3].ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="backOfficeUserName"></param>
        /// <param name="backOfficeUserPassword"></param>
        /// <param name="backOfficeConnectionName"></param>
        /// <param name="backOfficeConnectionDisplayable"></param>
        /// <param name="backOfficeConnectionInformation"></param>
        /// <param name="cloudName"></param>
        /// <param name="cloudURL"></param>
        /// <param name="connectionKey"></param>
        /// <param name="connectorPluginId"></param>
        /// <returns></returns>
        public static bool SaveNewTenantConfiguration(
            string backOfficeUserName,
            string backOfficeUserPassword,
            string backOfficeConnectionName,
            string backOfficeConnectionDisplayable,
            string backOfficeConnectionInformation,
            string cloudName,
            string cloudURL,
            string connectionKey,
            string connectorPluginId)
        {
            string tenantId, premiseKey, siteAddress;
            DecodeConnectionKey(connectionKey, out tenantId, out premiseKey, out siteAddress);

            Type pcrFactoryType = DataAssembly.GetType("Sage.Connector.Data.PremiseConfigurationRecordFactory", true);
            Type loggerType = CommonAssembly.GetType("Sage.Connector.Common.ILogging", true);
            MethodInfo createMethod = pcrFactoryType.GetMethod("Create", BindingFlags.Public | BindingFlags.Static, null, new Type[] { loggerType }, new ParameterModifier[0]);
            var pcrFactory = createMethod.Invoke(null, new object[] { null });

            MethodInfo createPCRMethod = pcrFactoryType.GetMethod("CreateNewEntry",
                BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.DeclaredOnly,
                null, new Type[] { }, new ParameterModifier[0]);
            var pcr = createPCRMethod.Invoke(pcrFactory, null);

            pcr.GetType().InvokeMember("BackOfficeUserName", BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty, null, pcr, new String[] { backOfficeUserName });
            pcr.GetType().InvokeMember("BackOfficeUserPassword", BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty, null, pcr, new String[] { backOfficeUserPassword });
            pcr.GetType().InvokeMember("BackOfficeCompanyName", BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty, null, pcr, new String[] { backOfficeConnectionName });
            pcr.GetType().InvokeMember("BackOfficeConnectionInformationDisplayable", BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty, null, pcr, new String[] { backOfficeConnectionDisplayable });
            pcr.GetType().InvokeMember("BackOfficeConnectionInformation", BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty, null, pcr, new String[] { backOfficeConnectionInformation });
            pcr.GetType().InvokeMember("CloudTenantId", BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty, null, pcr, new String[] { tenantId });
            pcr.GetType().InvokeMember("CloudCompanyName", BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty, null, pcr, new String[] { cloudName });
            pcr.GetType().InvokeMember("CloudCompanyUrl", BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty, null, pcr, new String[] { cloudURL });
            pcr.GetType().InvokeMember("CloudPremiseKey", BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty, null, pcr, new String[] { premiseKey });
            pcr.GetType().InvokeMember("SiteAddress", BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty, null, pcr, new String[] { siteAddress });
            pcr.GetType().InvokeMember("ConnectorPluginId", BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty, null, pcr, new String[] { connectorPluginId });

            Type pcrType = DataAssembly.GetType("Sage.Connector.Data.PremiseConfigurationRecord", true);
            MethodInfo saveNewTenantMethod = ConfigurationHelpersType.GetMethod("SaveNewTenantConfiguration", BindingFlags.Public | BindingFlags.Static, null, new Type[] { pcrType }, null);
            var retVal = saveNewTenantMethod.Invoke(null, new object[] { pcr });

            return Convert.ToBoolean(retVal);
        }

        /// <summary>
        /// 
        /// </summary>
        public static String LocalMonitorExecutablePath
        {
            get { return System.IO.Path.Combine(BaseConnectorPath, @"Monitor\Tray\ConnectorServiceMonitor.exe"); }
        }

        #region Connection Validations
        /// <summary>
        /// 
        /// </summary>
        /// <param name="backOfficeUserName"></param>
        /// <param name="backOfficeUserPassword"></param>
        /// <param name="pluginId"></param>
        /// <returns></returns>
        public static IEnumerable<BackOfficeConnection> RetrieveBackOfficeConnections(
             String pluginId,
            String backOfficeUserName,
            String backOfficeUserPassword
           )
        {
            List<BackOfficeConnection> retList = new List<BackOfficeConnection>();

            MethodInfo existMethod = ConfigurationHelpersType.GetMethod("RetrieveBackOfficeConnections", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string), typeof(string), typeof(string) }, null);
            var retVal = existMethod.Invoke(null, new object[] {pluginId, backOfficeUserName, backOfficeUserPassword});
            var connections = retVal as IEnumerable;
            if (null != connections)
                foreach (var connection in connections)
                {
                    var connectionInformation = connection.GetType().InvokeMember("ConnectionInformation", BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty, null, connection, new String[] {  });
                    var displayConnectionInformation = connection.GetType().InvokeMember("DisplayableConnectionInformation", BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty, null, connection, new String[] {  });
                    var connectionName = connection.GetType().InvokeMember("Name", BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty, null, connection, new String[] {  });
                    retList.Add(new BackOfficeConnection()
                                    {
                                        ConnectionInformation = connectionInformation.ToString(),
                                        DisplayName = displayConnectionInformation.ToString(),
                                        Name = connectionName.ToString()
                                    });
                }

            return retList;
        }

        private static ValidationResponse TranslateValidationResponse(object retVal)
        {
            bool bSuccess = Convert.ToBoolean(retVal.GetType().InvokeMember("Success",
                                                                           BindingFlags.Instance | BindingFlags.Public |
                                                                           BindingFlags.GetProperty, null,
                                                                           retVal, null));
            string sInternalError = Convert.ToString(retVal.GetType().InvokeMember("InternalError",
                                                                                   BindingFlags.Instance |
                                                                                   BindingFlags.Public |
                                                                                   BindingFlags.GetProperty, null,
                                                                                   retVal, null));
            string sUserFacingError = Convert.ToString(retVal.GetType().InvokeMember("UserFacingError",
                                                                                   BindingFlags.Instance |
                                                                                   BindingFlags.Public |
                                                                                   BindingFlags.GetProperty, null,
                                                                                   retVal, null));
            return new ValidationResponse()
                       {
                           Success = bSuccess,
                           InternalError = sInternalError,
                           UserFacingError = sUserFacingError
                       };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="backOfficeUserName"></param>
        /// <param name="backOfficeUserPassword"></param>
        /// <param name="pluginId"></param>
        /// <returns></returns>
        public static ValidationResponse ValidateBackOfficeAdministrator(
             String pluginId,
            String backOfficeUserName,
            String backOfficeUserPassword
           )
        {
            MethodInfo existMethod = ConfigurationHelpersType.GetMethod("ValidateBackOfficeAdministrator", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string), typeof(string), typeof(string) }, null);
            var retVal = existMethod.Invoke(null, new object[] {pluginId,  backOfficeUserName, backOfficeUserPassword, });
            return TranslateValidationResponse(retVal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="backOfficeUserName"></param>
        /// <param name="backOfficeUserPassword"></param>
        /// <param name="backOfficeConnectionInformation"></param>
        /// <param name="pluginId"></param>
        /// <returns></returns>
        public static ValidationResponse ValidateBackOfficeConnection(
            String pluginId,
            String backOfficeUserName,
            String backOfficeUserPassword,
            String backOfficeConnectionInformation
            )
        {
            MethodInfo existMethod = ConfigurationHelpersType.GetMethod("ValidateBackOfficeConnection", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string), typeof(string), typeof(string), typeof(string) }, null);
            var retVal = existMethod.Invoke(null, new object[] {pluginId, backOfficeUserName, backOfficeUserPassword, backOfficeConnectionInformation});
            return TranslateValidationResponse(retVal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionKey"></param>
        /// <returns></returns>
        public static ValidationResponse ValidateTenantConnection(string connectionKey)
        {
            String tenantId, premiseKey, siteAddress;
            DecodeConnectionKey(connectionKey, out tenantId, out premiseKey, out siteAddress);
        
            MethodInfo existMethod = ConfigurationHelpersType.GetMethod("ValidateTenantConnection", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string), typeof(string), typeof(string) }, null);
            var retVal = existMethod.Invoke(null, new object[] { siteAddress, tenantId, premiseKey });
            return TranslateValidationResponse(retVal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionKey"></param>
        /// <returns></returns>
        public static bool TenantConnectionExists(string connectionKey)
        {
            MethodInfo existMethod = ConfigurationHelpersType.GetMethod("TenantConnectionExists", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string) }, null);
            var retVal = existMethod.Invoke(null, new object[] { connectionKey });
            return Convert.ToBoolean(retVal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public static bool TenantExists(string tenantId)
        {
            MethodInfo existMethod = ConfigurationHelpersType.GetMethod("TenantExists", BindingFlags.Public | BindingFlags.Static, null, new Type[] { typeof(string) }, null);
            var retVal = existMethod.Invoke(null, new object[] { tenantId });
            return Convert.ToBoolean(retVal);
        }
        #endregion 
    }
}
