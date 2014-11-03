using System;
using System.Collections.Generic;
using System.IO;
using System.Net.NetworkInformation;
using Newtonsoft.Json.Linq;
using Sage.Authorisation;
using Sage.Authorisation.Client;
using Sage.Connector.AutoUpdate;
using Sage.Connector.Cloud.Integration.Interfaces.WebAPI;
using Sage.Connector.Common;
using Sage.Connector.Data;
using Sage.Connector.DispatchService.Proxy;
using Sage.Connector.Logging;
using Sage.Connector.MessagingService.Interfaces;
using Sage.Connector.MessagingService.Proxy;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.Connector.StateService.Proxy;
using Sage.Connector.Utilities;
using HostingFramework = Sage.CRE.HostingFramework;


//Notes:
//This class was originally located in Libraries\CRE\CloudConnector\Utilities\ConfigurationHelpers.cs
//This class was originally created by Cathay in support of the "SageConnectorConfiguration" tool.
//That use is deprecated.

namespace Sage.Connector.Management
{
    /// <summary>
    /// Encapsulate functionality for starting and initializing connected services
    /// </summary>
    public static class ConfigurationHelpers
    {
        #region Private Helpers

        private static Boolean CopyFileFromBaseline(String fileName, Boolean allowOverwrite)
        {
            var dataFolder = ConnectorServiceUtils.InstanceApplicationDataFolder;
            var baselineDataFolder = Path.Combine(ConnectorServiceUtils.InstanceApplicationDataFolder, "Baseline");
            bool fileExists = File.Exists(Path.Combine(dataFolder, fileName));

            if (!fileExists || allowOverwrite)
            {
                File.Copy(Path.Combine(baselineDataFolder, fileName), Path.Combine(dataFolder, fileName));
            }

            return true;
        }

        private static void CopyBaselineDataFolder()
        {

            var baselineDataFolder = Path.Combine(ConnectorServiceUtils.InstanceApplicationDataFolder, "Baseline");
            foreach (var file in Directory.GetFiles(baselineDataFolder, "*.*"))
            {
                if (!CopyFileFromBaseline(Path.GetFileName(file), false))
                    return;
            }
        }

        #endregion

        /// <summary>
        /// Install and initialize and start connector services
        /// </summary>
        /// <returns></returns>
        public static Boolean InstallAndStartServices()
        {
            //DO we need to separate the services?
            return InstallService("localsystem", "");
        }

        
        private static bool UninstallConnectorService()
        {
            bool retval = true;
            using (var logger = new SimpleTraceLogger())
            {
                bool isServiceRegisterd = ConnectorServiceUtils.IsServiceRegistered(logger);
                if (isServiceRegisterd)
                {
                    StopConnectorService();
                    if (!ConnectorServiceUtils.UninstallService(ConnectorRegistryUtils.ConnectorServiceInstallTimeout))
                        retval = false;
                }
            }
            return retval;
        }


        private static bool StopConnectorService()
        {
            bool retval = true;
            using (var logger = new SimpleTraceLogger())
            {
                bool isServiceRegisterd = ConnectorServiceUtils.IsServiceRegistered(logger);
                if (isServiceRegisterd)
                {
                    if (ConnectorServiceUtils.IsServiceRunning(logger))
                        if (!ConnectorServiceUtils.StopService(ConnectorRegistryUtils.ConnectorServiceInstallTimeout))
                            ConnectorServiceUtils.KillService(logger);
                }
            }
            return retval;
        }

        /// <summary>
        /// Install and initialize connector services
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="password">The password.</param>
        /// <returns></returns>
        private static Boolean InstallService(string user, string password)
        {
            if (null == password)
                password = string.Empty;
            var securePassword = password.ToSecureString();

            using (var logger = new SimpleTraceLogger())
            {
                bool isServiceRegisterd = ConnectorServiceUtils.IsServiceRegistered(logger);

                if (!isServiceRegisterd)
                {
                    //setup baseline data. only want to do this on initial setup or we lose connection info.
                    CopyBaselineDataFolder();

                    if (HostingFramework.Interfaces.ExitCode.Success != ConnectorServiceUtils.InstallService(
                       ConnectorRegistryUtils.ConnectorServiceInstallTimeout,
                       user,
                       securePassword,
                       true))
                        return false;
                }
               
                //start the connector service
                if (!ConnectorServiceUtils.IsServiceRunning(logger))
                    if (ConnectorServiceUtils.IsServiceEnabled())
                        if (HostingFramework.Interfaces.ExitCode.Success != ConnectorServiceUtils.StartService(
                            ConnectorRegistryUtils.ConnectorServiceStartTimeout,
                            ConnectorRegistryUtils.ConnectorServiceStartRetries))
                            return false;

                //install the monitor service
                if (!ConnectorMonitorServiceUtils.IsServiceRegistered(logger))
                    if (HostingFramework.Interfaces.ExitCode.Success != ConnectorMonitorServiceUtils.InstallService(
                        ConnectorRegistryUtils.MonitorServiceInstallTimeout,
                        StockAccountUtils.GetHostingFrameworkParamForStockAccountType(ConnectorRegistryUtils.MonitorServiceAccount),
                        null,
                        true))
                        return false;

                //start the monitor service
                if (!ConnectorMonitorServiceUtils.IsServiceRunning(logger))
                    if (ConnectorServiceUtils.IsServiceEnabled())
                        if (HostingFramework.Interfaces.ExitCode.Success != ConnectorMonitorServiceUtils.StartService(
                            ConnectorRegistryUtils.MonitorServiceStartTimeout,
                            ConnectorRegistryUtils.MonitorServiceStartRetries))
                            return false;
                
                //wait for the connector service to be ready.
                if (!ConnectorServiceUtils.IsServiceReady())
                    ConnectorServiceUtils.WaitForServiceReady(
                        ConnectorRegistryUtils.ConnectorServiceWaitForReadyTimeout,
                        ConnectorRegistryUtils.HostingFxWaitForNotReadySleepInterval);
                
            }

            return true;
        }

        /// <summary>
        /// Determines whether the monitor service is registered.
        /// </summary>
        /// <returns></returns>
        public static bool IsMonitorServiceRegistered()
        {
            bool retval;
            using (var logger = new SimpleTraceLogger())
            {
                retval = ConnectorMonitorServiceUtils.IsServiceRegistered(logger);
            }
            return retval;
        }

        /// <summary>
        /// Return the current connector identifier
        /// </summary>
        /// <returns></returns>
        public static Guid GetConnectorId()
        {
            return ConnectorRegistryUtils.ConnectorInstanceGuid;
        }

        /// <summary>
        /// Save a new or existing tenant configuration
        /// </summary>
        /// <param name="pcr">The PCR.</param>
        /// <returns></returns>
        public static bool SaveTenantConfiguration(PremiseConfigurationRecord pcr)
        {
            if (ConfigurationSettingFactory.RetrieveConfiguration(pcr.CloudTenantId) != null)
                return ConfigurationSettingFactory.UpdateConfiguration(pcr);

            return ConfigurationSettingFactory.SaveNewTenant(pcr);
        }


        /// <summary>
        /// Gets the new tenant configuration.
        /// </summary>
        /// <returns></returns>
        public static PremiseConfigurationRecord CreateNewTenantConfiguration()
        {
            return ConfigurationSettingFactory.CreateNewTenant();
        }

        /// <summary>
        /// Gets the new tenant configuration.
        /// </summary>
        /// <returns></returns>
        public static PremiseConfigurationRecord RetrieveTenantConfiguration(string tenantId)
        {
            return ConfigurationSettingFactory.RetrieveConfiguration(tenantId);
        }

        /// <summary>
        /// Gets the new tenant configuration.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<PremiseConfigurationRecord> GetAllTenantConfigurations()
        {
            return ConfigurationSettingFactory.RetrieveAllConfigurations();
        }

        /// <summary>
        /// Delete the tenant.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <remarks>
        /// Will return false if given a non existent tenantId.
        /// </remarks>
        public static bool DeleteTenant(string tenantId)
        {
            return ConfigurationSettingFactory.DeleteTenant(tenantId);
        }

        #region Validation

        /// <summary>
        /// Couple simple validation success with
        /// failure information
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
        /// Verifies for a given tenant identified whether the tenant already exists
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <returns></returns>
        public static bool TenantExists(string tenantId)
        {
            var pcr = ConfigurationSettingFactory.RetrieveConfiguration(tenantId);
            return (null != pcr && pcr.CloudTenantId == tenantId);
        }

        /// <summary>
        /// Validates back office connection
        /// </summary>
        /// <param name="backOfficeId">The back office identifier.</param>
        /// <param name="companyConnectionCredentials">The company connection credentials.</param>
        /// <returns></returns>
        public static ValidationResponse ValidateBackOfficeConnection(
            String backOfficeId,
            IDictionary<string, string> companyConnectionCredentials
            )
        {
            using (var proxy = BackOfficeValidationServiceProxyFactory.CreateFromCatalog(
                "localhost",
                ConnectorServiceUtils.CatalogServicePortNumber))
            {
                var response = proxy.ValidateBackOfficeConnection(
                    backOfficeId,
                    companyConnectionCredentials
                    );
                return new ValidationResponse()
                {
                    Success = response.BackOfficeConnectivityStatus == BackOfficeConnectivityStatus.Normal,
                    UserFacingError = (response.UserFacingMessages.Length > 0)
                        ? response.UserFacingMessages[0]
                        : string.Empty,
                    InternalError = (response.RawErrorMessage.Length > 0)
                        ? response.RawErrorMessage[0]
                        : string.Empty
                };
            }
        }

        /// <summary>
        /// Validates valid tenant information and connectivity
        /// </summary>
        /// <param name="siteAddress">The site address.</param>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="premiseId">The premise identifier.</param>
        /// <param name="wireClaim">The serialized wire claim</param>
        /// <returns></returns>
        public static ValidationResponse ValidateTenantConnection(
            String siteAddress,
            String tenantId,
            String premiseId,
            String wireClaim)
        {
            //NOTE: The translation here to user facing messages happens in 2 other places in the code as well
            //once sageConnector and again in the monitor. Perhaps consolidate to state service where TenantConnectivityStatus is defined?

            using (var proxy = TenantValidationServiceProxyFactory.CreateFromCatalog(
                "localhost",
                ConnectorServiceUtils.CatalogServicePortNumber))
            {
                var response = proxy.ValidateTenantConnection(
                    siteAddress,
                    tenantId,
                    premiseId,
                    wireClaim);
                string sUserFacingError = string.Empty;
                switch (response.TenantConnectivityStatus)
                {
                    case TenantConnectivityStatus.CloudUnavailable:
                        sUserFacingError = "Website is unavailable.";
                        break;
                    case TenantConnectivityStatus.GatewayServiceUnavailable:
                        sUserFacingError = "Website gateway is unavailable.";
                        break;
                    case TenantConnectivityStatus.CommunicationFailure:
                        sUserFacingError = "Communication failure.";
                        break;
                    case TenantConnectivityStatus.LocalNetworkUnavailable:
                        sUserFacingError = "Local network is unavailable.";
                        break;
                    case TenantConnectivityStatus.InternetConnectionUnavailable:
                        sUserFacingError = "Internet connection is unavailable.";
                        break;
                    case TenantConnectivityStatus.TenantDisabled:
                        sUserFacingError = "Tenant has been disabled.";
                        break;
                    case TenantConnectivityStatus.Reconfigure:
                        sUserFacingError = "Connection is reconfiguring.";
                        break;
                    case TenantConnectivityStatus.InvalidConnectionInformation:
                        sUserFacingError = "Invalid connection information.";
                        break;
                    case TenantConnectivityStatus.IncompatibleClient:
                        sUserFacingError = "Client interface is incompatible.";
                        break;
                }

                return new ValidationResponse()
                {
                    Success = response.TenantConnectivityStatus == TenantConnectivityStatus.Normal,
                    UserFacingError = sUserFacingError
                };
            }
        }

        #endregion


        /// <summary>
        /// Gets the default cloud deployment URI.
        /// </summary>
        /// <returns></returns>
        public static Uri GetDefaultCloudSiteUri()
        {
            SiteGroupManager sgm = CreateSiteGroupManagerWithMockOverwrite();

            SiteGroup site = sgm.FindDefaultGroup();
            return site.CloudSiteUri;
        }

        

        /// <summary>
        /// Finds the site group for URI.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns></returns>
        public static SiteGroup FindSiteGroupForUri(Uri uri)
        {
            SiteGroupManager sgm = CreateSiteGroupManagerWithMockOverwrite();

            SiteGroup site = sgm.FindCloudSiteUri(uri);
            return site;
        }

        /// <summary>
        /// Gets the site groups.
        /// </summary>
        /// <returns></returns>
        public static IList<SiteGroup> GetSiteGroups()
        {
            SiteGroupManager sgm = CreateSiteGroupManagerWithMockOverwrite();

            IList<SiteGroup> sites = sgm.SiteGroups();
            return sites;
        }

        private static SiteGroupManager CreateSiteGroupManagerWithMockOverwrite()
        {
            SiteGroupManager sgm = new SiteGroupManager();
            sgm.ReadConfig();
            sgm.OverWriteServiceAndPlatformUriIfEmpty("MockCloud", GetMockCloudSiteDefaultUri());
            return sgm;
        }


        private static Uri GetMockCloudSiteDefaultUri()
        {
            Uri baseWebApp = new Uri(String.Format("http://{0}:8004/", Environment.MachineName));
            return baseWebApp;
        }

        /// <summary>
        /// Gets the tenant list.
        /// </summary>
        /// <param name="sageIdToken">The sage identifier token.</param>
        /// <param name="siteAddress">The site address.</param>
        /// <returns></returns>
        public static IEnumerable<UserManagementTenant> GetTenantList(string sageIdToken, string siteAddress)
        {

            //http://ashplatform.sagedatacloud.com/sdata/api/Master/1.0/-/Tenants

            List<UserManagementTenant> list = new List<UserManagementTenant>();
            Cloud.Integration.Interfaces.WebAPI.TenantInfo[] tenantList = null;
            try
            {

                using (
                    var proxy = MessagingServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                {
                    tenantList = proxy.CloudTenantList(new Uri(siteAddress), Guid.Empty.ToString(), sageIdToken);
                }

            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    //lm.WriteError(this, ex.ExceptionAsString());
                    lm.WriteError(null, ex.ExceptionAsString());
                }
                //TODO Should this be a value with success of false?
                tenantList = null;
            }

            if (null != tenantList)
            {
                foreach (var t in tenantList)
                {

                    var umt = new UserManagementTenant()
                    {
                        TenantGuid = t.TenantGuid,
                        RegisteredCompanyId = t.RegisteredCompany,
                        RegisteredConnectorId = t.RegisteredConnectorId,
                        TenantName = t.TenantName
                    };

                    list.Add(umt);
                }
            }

            return list;
        }

        /// <summary>
        /// Obtain an OAuth token from Sage ID
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public static string ObtainAuthorizationToken(String clientId, String scope)
        {
            try
            {
                using (OAuthClient auth = new OAuthClient(clientId, ""))
                {
                    try
                    {
                        auth.SuppressInteractive = false;

                        AuthorisationInfo startInfo = new AuthorisationInfo
                        {
                            Scope = scope,
                            ResponseType = "code",
                            State = "Authorizing",
                            ResetDuration = false,
                            DeviceName = "iOS"
                        };

                        IAuthorisationResult result = auth.Authorise(startInfo, null);

                        return result.AccessToken;
                    }
                    catch (Exception ex)
                    {
                        using (var lm = new LogManager())
                        {
                            lm.WriteError(null, ex.ExceptionAsString());
                        }
                    }
                    finally
                    {
                        auth.CleanUp();
                    }
                }
            }catch(Exception ex)
            {
                using (var lm = new LogManager())
                {
                    lm.WriteError(null, ex.ExceptionAsString());
                }
            }
            return String.Empty;
        }

        /// <summary>
        /// Clean up retrieved access token
        /// </summary>
        /// <param name="clientId"></param>
        public static void SignOut(String clientId)
        {
            using (OAuthClient auth = new OAuthClient(clientId, ""))
            {
                try
                {
                    auth.CleanUp();
                }
                catch (Exception ex)
                {
                    using (var lm = new LogManager())
                    {
                        lm.WriteError(null, ex.ExceptionAsString());
                    }
                }
            }
        }


        /// <summary>
        /// Registers the connection.
        /// </summary>
        /// <param name="siteAddress">The site address.</param>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="backOfficeCompanyId">The back office company identifier.</param>
        /// <param name="authenticationToken">The authentication token required for registration</param>
        /// <returns></returns>
        public static RegistrationResult RegisterConnection(Uri siteAddress, string tenantId, string backOfficeCompanyId, string authenticationToken)
        {
            RegistrationResult retval = null;
            try
            {
                TenantRegistrationWithErrorInfo tenantRegistration = null;
                using (
                    var proxy = MessagingServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                {
                    tenantRegistration = proxy.CloudTenantRegistration(siteAddress, tenantId, backOfficeCompanyId, authenticationToken);
                }
                retval = new RegistrationResult(tenantRegistration);
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(null, ex.ExceptionAsString());
                }
                retval = new RegistrationResult(null);
            }

            return retval;
        }


        /// <summary>
        /// Clear the connector registration
        /// </summary>
        /// <param name="siteAddress">The site address.</param>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="authenticationToken">The authentication token required for registration</param>
        /// <returns></returns>
        public static RegistrationResult ClearConnectorRegistration(Uri siteAddress, string tenantId, string authenticationToken)
        {
            RegistrationResult retval = null;
            try
            {
                TenantRegistrationWithErrorInfo tenantRegistration = null;
                using (
                    var proxy = MessagingServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                {
                    tenantRegistration = proxy.ClearConnectorTenantRegistration(siteAddress, tenantId, authenticationToken);
                }
                retval = new RegistrationResult(tenantRegistration);
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(null, ex.ExceptionAsString());
                }
                retval = new RegistrationResult(null);
            }

            return retval;
        }

        


        /// <summary>
        /// Downloads the back office plugin.
        /// </summary>
        /// <param name="backOfficeId">The back office identifier.</param>
        /// <param name="autoUpdateProductId">The automatic update product identifier.</param>
        /// <param name="autoUpdateVersion">The automatic update version.</param>
        /// <param name="autoUpdateComponentBaseName">Name of the automatic update component base.</param>
        public static void DownloadBackOfficePlugin(string backOfficeId, string autoUpdateProductId, string autoUpdateVersion,
            string autoUpdateComponentBaseName)
        {
            string packageId = AutoUpdateManager.GetBackOfficePluginPackageId(backOfficeId);
            Uri autoUpdateUri = AutoUpdateManager.GetAutoUpdateAddress(packageId);

            using (var proxy = AutoUpdateServiceProxyFactory.CreateFromCatalog(
                "localhost",
                ConnectorServiceUtils.CatalogServicePortNumber))
            {
                proxy.DownloadBackOfficePlugin(backOfficeId, autoUpdateUri, autoUpdateProductId, autoUpdateVersion,
                    autoUpdateComponentBaseName);
            }
        }

        /// <summary>
        /// Checks for automatic updates.
        /// </summary>
        public static void CheckForAutoUpdates()
        {

            using (var proxy = AutoUpdateServiceProxyFactory.CreateFromCatalog(
                "localhost",
                ConnectorServiceUtils.CatalogServicePortNumber))
            {
                proxy.CheckForUpdates();
            }
        }

    }


    /// <summary>
    /// 
    /// </summary>
    public class SiteGroup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SiteGroup" /> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cloudSiteUri">The cloud site URI.</param>
        /// <param name="clientId">Connnector client identifier</param>
        /// <param name="scope">Scope of authentication</param>
        /// <param name="isDefault">if set to <c>true</c> [is default].</param>
        /// <param name="dataCloudWebPageUri">URI for Data Cloud Web Page</param>
        /// <param name="dataCloudWebPageDisplayName">Display Name for the Data cloud web Page</param>
        public SiteGroup(string id, Uri cloudSiteUri, string clientId, string scope, bool isDefault,
            Uri dataCloudWebPageUri, string dataCloudWebPageDisplayName)
        {
            Id = id;
            CloudSiteUri = cloudSiteUri;
            ConnectorServiceUri = cloudSiteUri;
            ClientId = clientId;
            Scope = scope;
            IsDefault = isDefault;
            DataCloudWebPageUri = dataCloudWebPageUri;
            DataCloudWebPageDisplayName = dataCloudWebPageDisplayName;
        }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id { get; internal set; }

        /// <summary>
        /// Gets the cloud site URI.
        /// </summary>
        /// <value>
        /// The cloud site URI.
        /// </value>
        public Uri CloudSiteUri { get;  internal set; }

        /// <summary>
        /// Gets the connector service URI.
        /// </summary>
        /// <value>
        /// The connector service URI.
        /// </value>
        public Uri ConnectorServiceUri { get; internal set; }

        /// <summary>
        /// Gets the Data cloud Webpage URI.
        /// </summary>
        /// <value>
        /// The Data cloud webpage  URI.
        /// </value>
        public Uri DataCloudWebPageUri { get; internal set; }

        /// <summary>
        /// Gets Display name for Data Cloud
        /// </summary>
        public String DataCloudWebPageDisplayName { get; internal set; }

        /// <summary>
        /// Gets the connnector client identifier
        /// </summary>
        public String ClientId { get; internal set; }

        /// <summary>
        /// Gets the authentication scope
        /// </summary>
        public String Scope { get; internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is default.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is default; otherwise, <c>false</c>.
        /// </value>
        public bool IsDefault { get; internal set; }
    }
}
