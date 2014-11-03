using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Sage.Connector.Common;
using Sage.Connector.ConnectorServiceCommon;
using Sage.Connector.Cloud.Integration.Interfaces;
using Sage.Connector.Cloud.Integration.Interfaces.DataContracts;
using Sage.Connector.Cloud.Integration.Interfaces.Requests;
using Sage.Connector.Cloud.Integration.Interfaces.Responses;
using Sage.CRE.Cloud.WebService.Connector;
using Sage.CRE.Cloud.WebService.Connector.Extensibility;
using CloudContracts = Sage.Connector.Cloud.Integration.Interfaces.DataContracts;

namespace Sage.Connector.SageCloudService
{
    /// <summary>
    /// Implementation of the sage cloud messaging WCF service
    /// Notes on attributes:
    /// ServiceBehavior - Per call instance context is the default.  In this mode, a service object exists
    ///     only while a call is in progress, so every client request gets a new dedicated instance.  If any
    ///     state is introduced to this service, it will have to be correctly managed so that the state from
    ///     one call persists to a future call.
    /// AspNetCompatibilityRequirements - allows for persistent out-of-process storage for session state
    ///     when in an environment where subsequent requests can be processed by different hosts or processes.
    ///     Without this, WCF will just store all its session state in memory.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, AddressFilterMode = AddressFilterMode.Any, ConcurrencyMode = ConcurrencyMode.Multiple)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [GetHttpRequestHeaderBehavior]
    public class MockCloudService : IGatewayService, IAdminService, IRequestService, IResponseService, IUploadSessionService, IDownloadSessionService, ICREMessageServiceInjection
    {
        static MockCloudService()
        {
            SetupTenants();
        }

        /// <summary>
        /// Return the Connector premise identification key
        /// </summary>
        /// <param name="connectorId"></param>
        /// <returns></returns>
        public static string GetConnectorPremiseKey(Guid connectorId)
        {
            return connectorId.ToString("N");
        }

        private static void SetupTenants()
        {
            string configXmlPath;
            try
            {
                FileInfo assembly = new FileInfo(System.Reflection.Assembly.GetExecutingAssembly().Location);
                configXmlPath = assembly.DirectoryName;
            }
            catch (Exception)
            {
                configXmlPath = "";
            }

            ConfigurationTenants configTenantCollection = ConfigurationTenantUtils.DeserializeTenants(configXmlPath);
            if (null != configTenantCollection)
            {
                foreach (ConfigurationTenant tenant in configTenantCollection.Tenants)
                {
                    _tenants.Add(tenant.TenantId.ToLower(), new TenantContainer(
                        tenant,
                        GetDefaultConfigParams(),
                        GetDefaultGatewayServiceInfo())
                        );
                }
            }
            else
            {
                String[] tenantIds = new String[]
                {
                    "3813FCCF-4946-43E8-AC72-0C00D2DF9F6F",
                    "7B6791D5-8658-44E5-86BC-8181735D0BF7",
                    "5DC75A87-4688-4D45-B4FF-91D43DB98072",
                    "db94139b-38b5-495f-86b6-680f02908094",
    #region 50 additional tenants
                    "00000000-0000-0000-0000-000000000001",
                    "00000000-0000-0000-0000-000000000002",
                    "00000000-0000-0000-0000-000000000003",
                    "00000000-0000-0000-0000-000000000004",
                    "00000000-0000-0000-0000-000000000005",
                    "00000000-0000-0000-0000-000000000006",
                    "00000000-0000-0000-0000-000000000007",
                    "00000000-0000-0000-0000-000000000008",
                    "00000000-0000-0000-0000-000000000009",
                    "00000000-0000-0000-0000-000000000010",
                    "00000000-0000-0000-0000-000000000011",
                    "00000000-0000-0000-0000-000000000012",
                    "00000000-0000-0000-0000-000000000013",
                    "00000000-0000-0000-0000-000000000014",
                    "00000000-0000-0000-0000-000000000015",
                    "00000000-0000-0000-0000-000000000016",
                    "00000000-0000-0000-0000-000000000017",
                    "00000000-0000-0000-0000-000000000018",
                    "00000000-0000-0000-0000-000000000019",
                    "00000000-0000-0000-0000-000000000020",
                    "00000000-0000-0000-0000-000000000021",
                    "00000000-0000-0000-0000-000000000022",
                    "00000000-0000-0000-0000-000000000023",
                    "00000000-0000-0000-0000-000000000024",
                    "00000000-0000-0000-0000-000000000025",
                    "00000000-0000-0000-0000-000000000026",
                    "00000000-0000-0000-0000-000000000027",
                    "00000000-0000-0000-0000-000000000028",
                    "00000000-0000-0000-0000-000000000029",
                    "00000000-0000-0000-0000-000000000030",
                    "00000000-0000-0000-0000-000000000031",
                    "00000000-0000-0000-0000-000000000032",
                    "00000000-0000-0000-0000-000000000033",
                    "00000000-0000-0000-0000-000000000034",
                    "00000000-0000-0000-0000-000000000035",
                    "00000000-0000-0000-0000-000000000036",
                    "00000000-0000-0000-0000-000000000037",
                    "00000000-0000-0000-0000-000000000038",
                    "00000000-0000-0000-0000-000000000039",
                    "00000000-0000-0000-0000-000000000040",
                    "00000000-0000-0000-0000-000000000041",
                    "00000000-0000-0000-0000-000000000042",
                    "00000000-0000-0000-0000-000000000043",
                    "00000000-0000-0000-0000-000000000044",
                    "00000000-0000-0000-0000-000000000045",
                    "00000000-0000-0000-0000-000000000046",
                    "00000000-0000-0000-0000-000000000047",
                    "00000000-0000-0000-0000-000000000048",
                    "00000000-0000-0000-0000-000000000049",
                    "00000000-0000-0000-0000-000000000050"
    #endregion
                };

                foreach (var tenantId in tenantIds)
                {
                    _tenants.Add(tenantId.ToLower(), new TenantContainer(
                        tenantId.ToLower(),
                        "Test",
                        GetDefaultConfigParams(),
                        GetDefaultGatewayServiceInfo()));
                }
            }
        }

        /// <summary>
        /// Used to reset tenant container in box and out box contents to nothing
        /// </summary>
        public static void ResetMutableTenantState()
        {
            foreach (var tenant in _tenants)
            {
                tenant.Value.ClearInboxOutbox();
                tenant.Value.ConfigParams = GetDefaultConfigParams();
                tenant.Value.GatewayServiceInfo = GetDefaultGatewayServiceInfo();
            }
        }

        /// <summary>
        /// Used to pass an example UpgradeInfo
        /// </summary>
        /// <returns></returns>
        public static UpgradeInfo GetUpgradeInfoForRequiredUpdate()
        {
            UpgradeView view = VersionInformation;
            Uri link;
            if (!Uri.TryCreate(view.UpgradeRequiredLink, UriKind.Absolute, out link))
                link = new Uri("http://dictionary.reference.com/browse/require");
            return new UpgradeInfo(view.MinProductVersion, view.UpgradeRequiredDate, view.MinInterfaceVersion, view.UpgradeRequiredDescription, link);
        }

        /// <summary>
        /// Used to pass an example UpgradeInfo
        /// </summary>
        /// <returns></returns>
        public static UpgradeInfo GetUpgradeInfoForAvailableUpdate()
        {
            UpgradeView view = VersionInformation;
            Uri link;
            if (!Uri.TryCreate(view.UpgradeAvailableLink, UriKind.Absolute, out link))
                link = new Uri("http://dictionary.reference.com/browse/available");
            return new UpgradeInfo(view.CurrentProductVersion, view.UpgradeAvailableDate, view.CurrentInterfaceVersion, view.UpgradeAvailableDescription, link);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Int32 TenantCount()
        { return _tenants.Count(); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public static string RetrievePremiseKey(string tenantId)
        { return _tenants[tenantId].PremiseKey; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public static Sage.Connector.Cloud.Integration.Interfaces.WebAPI.Configuration RetrieveConfigParams(string tenantId)
        { return _tenants[tenantId].ConfigParams; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public static Sage.Connector.Cloud.Integration.Interfaces.WebAPI.Configuration RetrieveConfigParamsCustomUpdate(string tenantId)
        { return _tenants[tenantId].ConfigParamsCustomUpdate; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public static ServiceInfo RetrieveGatewayServiceInfo(string tenantId)
        { return _tenants[tenantId].GatewayServiceInfo; }

        /// <summary>
        /// 
        /// </summary>
        public static IEnumerable<String> TenantIds
        {
            get { return _tenants.Keys; }
        }

        /// <summary>
        /// 
        /// </summary>
        private static UpgradeView VersionInformation
        {
            get
            {
                if (_versionInformation == null)
                {
                    _versionInformation = new UpgradeView("1.0", "1.0", "1.0", "1.0", "", "", "", "", DateTime.Now, DateTime.Now);
                }
                return _versionInformation;
            }
            set
            {
                _versionInformation = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static IEnumerable<String> TenantPremiseKeys
        {
            get { return _tenants.Values.Select(x => x.PremiseKey); }
        }

        /// <summary>
        /// Tenants Test Time
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public static string AutomationEndpointAddress(string tenantId)
        { return _tenants[tenantId].TenantEndpointAddress; }

        /// <summary>
        /// Tenants Test Time
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public static double AutomationTestTime(string tenantId)
        { return _tenants[tenantId].TestTime; }

        /// <summary>
        /// Tenants request delay
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public static double AutomationRequestDelay(string tenantId)
        { return _tenants[tenantId].RequestDelay; }

        /// <summary>
        /// Tenants Initial requests
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public static ConfigurationRequest[] AutomationInitialRequests(string tenantId)
        { return _tenants[tenantId].InitialRequests; }

        /// <summary>
        /// Tenants Continued requests
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public static ConfigurationRequest[] AutomationContinuedRequests(string tenantId)
        { return _tenants[tenantId].ContinuedRequests; }

        /// <summary>
        /// Request for ConfigParams
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public static Sage.Connector.Cloud.Integration.Interfaces.WebAPI.Configuration UpdateConfigurationRequest(string tenantId)
        {
            return _tenants[tenantId].ConfigParams;
        }

        /// <summary>
        /// Returns whether the tenant has been disabled or not
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public static bool? TenantDisabled(string tenantId)
        {
            return _tenants[tenantId].TenantDisabled;
        }

        /// <summary>
        /// Deletes the tenant
        /// </summary>
        /// <param name="tenantId"></param>
        public static void DeleteTenant(string tenantId)
        {
            _tenants[tenantId].TenantDisabled = null;
        }

        /// <summary>
        /// Temporarily Disables or Re-enables the tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="disable"></param>
        public static void DisableTenant(string tenantId, bool disable)
        {
            _tenants[tenantId].TenantDisabled = disable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static UpgradeView RetrieveVersionInformation()
        {
            return VersionInformation;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="view"></param>
        public static void SetVersionInformation(UpgradeView view)
        {
            VersionInformation = view;
        }

        /// <summary>
        /// A request is pending for Updating the tenants configuration
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public static bool IsUpdateConfigurationRequestPending(string tenantId)
        {
            return _tenants[tenantId].ConfigParamUpdatePending;
        }

        internal static String GetPremiseKeyForTenant(Guid tenantId)
        {
            String result = String.Empty;
            if (_tenants.ContainsKey(tenantId.ToString()))
            {
                result = _tenants[tenantId.ToString()].PremiseKey;
            }

            return result;
        }

        private static Sage.Connector.Cloud.Integration.Interfaces.WebAPI.Configuration GetDefaultConfigParams()
        {
            return new Cloud.Integration.Interfaces.WebAPI.Configuration()
            {
                ConfigurationBaseUri = new Uri(MockCloudServiceHost.WebAPIAddress),
                ConfigurationResourcePath = "api/configuration",
                RequestBaseUri = new Uri(MockCloudServiceHost.WebAPIAddress),
                RequestResourcePath = "api/messages/requests",
                ResponseBaseUri = new Uri(MockCloudServiceHost.WebAPIAddress),
                ResponseResourcePath = "api/messages/responses",
                RequestUploadResourcePath = "api/messages/requests/startuploadrequest",
                ResponseUploadResourcePath = "api/messages/responses/enduploadrequest",
                NotificationResourceUri = new Uri(MockCloudServiceHost.NotificationAddress),
                MinimumConnectorProductVersion = "1.0.0.0",
                UpgradeConnectorProductVersion = "3.0.0.0",
                UpgradeConnectorPublicationDate = DateTime.UtcNow.Date,
                UpgradeConnectorDescription = "Improved performance.",
                UpgradeConnectorLinkUri = new Uri(@"http://www.sage.com/connector_download"),
                SiteAddressBaseUri = new Uri(MockCloudServiceHost.SiteAddress),
                TenantPublicUri = new Uri(MockCloudServiceHost.SiteAddress), // to be augmented with tenant specifics
                TenantName = "TenantName", // to be augmented with tenant specifics
                MaxBlobSize = 16384000,
                LargeResponseSizeThreshold = 10000000,
                SuggestedMaxConnectorUptimeDuration = new TimeSpan(7, 0, 0, 0),
                MinCommunicationFailureRetryInterval = TimeSpan.FromSeconds(1),
                MaxCommunicationFailureRetryInterval = TimeSpan.FromSeconds(60)
            };
        }

        private static ServiceInfo GetDefaultGatewayServiceInfo()
        {
            ServiceInfo gatewayServiceInfo = new ServiceInfo(
                "Gateway",
                new Uri(String.Format("http://{0}:8002/MockCloudService.svc/Gateway", Environment.MachineName)),
                GetSeviceContractNamespace(typeof(IGatewayService)));

            return gatewayServiceInfo;
        }


        internal static TenantContainer GetTenantContainer(string tenantId)
        {
            TenantContainer tenant;
            if (!_tenants.TryGetValue(tenantId, out tenant))
            {
                throw new PremiseCommunicationException("Incorrect TenantId!");
            }
            return tenant;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static Boolean StaticExternalAddToPremiseMessage(string tenantId, Request message)
        {
            TenantContainer tenantContainer = GetTenantContainer(tenantId);
            bool result = tenantContainer.ExternalAddToPremiseMessage(message);

            return result;
        }

        /// <summary>
        /// The number of responses for the tenant CURRENTLY in the inbox.
        /// These messages are cleared out periodically to decrease the memory footprint.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public static Tuple<Int32, Int32> StaticExternalPeekCurrentMessageCounts(string tenantId)
        {
            TenantContainer tenantContainer = GetTenantContainer(tenantId);
            Tuple<Int32, Int32> result = tenantContainer.ExternalPeekCurrentMessageCounts();

            return result;
        }

        /// <summary>
        /// The total number of responses received for the tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public static Tuple<Int32, Int32, Int32> StaticExternalPeekTotalMessageCounts(string tenantId)
        {
            TenantContainer tenantContainer = GetTenantContainer(tenantId);
            Tuple<Int32, Int32, Int32> result = tenantContainer.ExternalPeekTotalMessageCounts();

            return result;
        }

        /// <summary>
        /// Static external retrieval of  output messages
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public static IEnumerable<Request> StaticExternalPeekOutboxMessage(string tenantId)
        {
            TenantContainer tenantContainer = GetTenantContainer(tenantId);
            IEnumerable<Request> result = tenantContainer.ExternalPeekOutboxMessage();

            return result;
        }

        /// <summary>
        /// Static external retrieval of input of messages
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public static IEnumerable<Response> StaticExternalPeekInboxMessage(string tenantId)
        {
            TenantContainer tenantContainer = GetTenantContainer(tenantId);
            IEnumerable<Response> result = tenantContainer.ExternalPeekInboxMessage();
            return result;
        }

        private static readonly Dictionary<String, TenantContainer> _tenants = new Dictionary<String, TenantContainer>();
        private static UpgradeView _versionInformation;

        #region Private members

        private static UpgradeInfo CheckForPremiseUpgradeAvailable(PremiseAgent agent)
        {
            UpgradeInfo retVal = null;

            if (VersionInformation.CurrentInterfaceVersion.CompareTo(agent.InterfaceVersion) > 0 ||
                VersionInformation.CurrentProductVersion.CompareTo(agent.ConnectorProductVersion) > 0)
            {
                retVal = GetUpgradeInfoForAvailableUpdate();
            }
            return retVal;
        }

        private static String GetSeviceContractNamespace(Type serviceContractType)
        { return (serviceContractType.GetCustomAttributes(typeof(ServiceContractAttribute), false).Cast<ServiceContractAttribute>().Single().Namespace); }

        #endregion

        #region IGatewayService Members

        /// <summary>
        /// Gets the service infos for all non gateway services
        /// </summary>
        /// <returns></returns>
        public SiteServiceInfo GetSiteServiceInfo()
        {
            SiteServiceInfo result = null;

            Guid tenantId;
            PremiseAgent premiseAgent;
            ServiceUtils.GetHttpHeaderData(out tenantId, out premiseAgent);
            TenantContainer tenantContainer = GetTenantContainer(tenantId.ToString());
            UpgradeInfo upgradeInfo = null;

            //Check to see if upgrade is available
            upgradeInfo = CheckForPremiseUpgradeAvailable(premiseAgent);

            try
            {
                Uri incomingMessageHeaderTo = OperationContext.Current.IncomingMessageHeaders.To;
                String localAddressOriginalString = incomingMessageHeaderTo.OriginalString;
                Uri baseUri = new Uri(localAddressOriginalString.Substring(0, incomingMessageHeaderTo.OriginalString.Length - incomingMessageHeaderTo.PathAndQuery.Length));
                Uri gatewayUri = new Uri(baseUri, new Uri("MockCloudService.svc/Gateway", UriKind.Relative));
                Uri adminUri = new Uri(baseUri, new Uri("MockCloudService.svc/Admin", UriKind.Relative));
                Uri requestUri = new Uri(baseUri, new Uri("MockCloudService.svc/Request", UriKind.Relative));
                Uri responseUri = new Uri(baseUri, new Uri("MockCloudService.svc/Response", UriKind.Relative));
                Uri uploadUri = new Uri(baseUri, new Uri("MockCloudService.svc/UploadSession", UriKind.Relative));
                Uri downloadUri = new Uri(baseUri, new Uri("MockCloudService.svc/DownloadSession", UriKind.Relative));
                Uri notificationUri = new Uri(MockCloudServiceHost.NotificationAddress);
                Uri webAPIUri = new Uri(MockCloudServiceHost.WebAPIAddress);

                result = new SiteServiceInfo(
                    premiseAgent.InterfaceVersion,
                    premiseAgent.ConnectorProductVersion,
                    upgradeInfo,
                    new Uri(MockCloudServiceHost.SiteAddress),
                    new ServiceInfo[]
                    {
                        new ServiceInfo(ServiceConstants.V1_GATEWAY_SERVICE_INFO_NAME, gatewayUri, GetSeviceContractNamespace(typeof(IGatewayService))),
                        new ServiceInfo(ServiceConstants.V1_ADMIN_SERVICE_INFO_NAME, adminUri, GetSeviceContractNamespace(typeof(IAdminService))),
                        new ServiceInfo(ServiceConstants.V1_REQUEST_SERVICE_INFO_NAME, requestUri, GetSeviceContractNamespace(typeof(IRequestService))),
                        new ServiceInfo(ServiceConstants.V1_RESPONSE_SERVICE_INFO_NAME, responseUri, GetSeviceContractNamespace(typeof(IResponseService))),
                        new ServiceInfo(ServiceConstants.V1_UPLOAD_SESSION_SERVICE_INFO_NAME, uploadUri, GetSeviceContractNamespace(typeof(IUploadSessionService))),
                        new ServiceInfo(ServiceConstants.V1_DOWNLOAD_SESSION_SERVICE_INFO_NAME, downloadUri, GetSeviceContractNamespace(typeof(IDownloadSessionService))),
                        new ServiceInfo(ServiceConstants.V2_REQUEST_WAITING_NOTIFICATION_SERVICE_INFO_NAME, notificationUri, ServiceConstants.V1_SERVICE_NAMESPACE),
                        new ServiceInfo(ServiceConstants.V2_API_SERVICE_INFO_NAME, webAPIUri, ServiceConstants.V1_SERVICE_NAMESPACE), 
                    });
            }
            catch (Exception ex)
            {
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(this, ex.ExceptionAsString());
                }
            }
            return result;
        }

        #endregion

        #region IAdminService
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ConfigParams GetConfigParams()
        {
            Guid tenantId;
            PremiseAgent premiseAgent;
            ServiceUtils.GetHttpHeaderData(out tenantId, out premiseAgent);
            TenantContainer tenantContainer = GetTenantContainer(tenantId.ToString());

            ConfigParams result = null;
            try
            {
                result = null;
            }
            catch (Exception ex)
            {
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(this, ex.ExceptionAsString());
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public CloudContracts.TenantInfo GetTenantInfo()
        {
            CloudContracts.TenantInfo result = null;

            try
            {
                Guid tenantId;
                PremiseAgent premiseAgent;
                ServiceUtils.GetHttpHeaderData(out tenantId, out premiseAgent);
                TenantContainer tenantContainer = GetTenantContainer(tenantId.ToString());

                result = new CloudContracts.TenantInfo(new Uri(tenantContainer.Uri), tenantContainer.Name);
            }
            catch (Exception ex)
            {
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(this, ex.ExceptionAsString());
                }
            }

            return result;
        }
        #endregion

        #region IRequestService
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Request[] GetRequests()
        {
            Request[] result = null;

            Guid tenantId;
            PremiseAgent premiseAgent;
            ServiceUtils.GetHttpHeaderData(out tenantId, out premiseAgent);
            TenantContainer tenantContainer = GetTenantContainer(tenantId.ToString());

            try
            {
                result = tenantContainer.GetRequests();
            }
            catch (Exception ex)
            {
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(this, ex.ExceptionAsString());
                }
            }

            return result;
        }
        #endregion

        #region IResponseService
        /// <summary>
        /// 
        /// </summary>
        /// <param name="responses"></param>
        public void PutResponses(Response[] responses)
        {
            Guid tenantId;
            PremiseAgent premiseAgent;
            ServiceUtils.GetHttpHeaderData(out tenantId, out premiseAgent);
            TenantContainer tenantContainer = GetTenantContainer(tenantId.ToString());

            try
            {
                tenantContainer.PutResponses(responses);
            }
            catch (Exception ex)
            {
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(this, ex.ExceptionAsString());
                }
            }
        }
        #endregion

        #region IUploadSessionService
        /// <summary>
        /// 
        /// </summary>
        /// <param name="premiseDocumentId"></param>
        /// <param name="purposeDescription"></param>
        /// <param name="expectedSizeInBytes"></param>
        /// <returns></returns>
        public CloudContracts.UploadSessionInfo CreateUploadSession(String premiseDocumentId, String purposeDescription, Int32 expectedSizeInBytes)
        {
            Guid tenantId;
            PremiseAgent premiseAgent;
            ServiceUtils.GetHttpHeaderData(out tenantId, out premiseAgent);
            //TenantContainer tenantContainer = GetTenantContainer(tenantId.ToString());

            // Return a dummy upload session info
            // Since the mock cloud is not connected to azure storage
            return Utils.MockUploadSessionInfo;
        }
        #endregion

        #region IDownloadSessionService

        /// <summary>
        /// Returns a dummy download session info (not supported by mock)
        /// </summary>
        /// <param name="cloudDocumentId"></param>
        /// <returns></returns>
        public DownloadSessionInfo CreateDownloadSession(string cloudDocumentId)
        {
            return Utils.MockDownloadSessionInfo;
        }

        #endregion

        #region ICREMessageServiceInjection Members

        /// <summary>
        /// External retrieval of  output messages
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public IEnumerable<Request> ExternalPeekOutboxMessage(string tenantId)
        {
            TenantContainer tenantContainer = GetTenantContainer(tenantId);
            IEnumerable<Request> result = tenantContainer.ExternalPeekOutboxMessage();

            return result;
        }

        /// <summary>
        /// External retrieval of input o messages
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public IEnumerable<Response> ExternalPeekInboxMessage(string tenantId)
        {
            TenantContainer tenantContainer = GetTenantContainer(tenantId);
            IEnumerable<Response> result = tenantContainer.ExternalPeekInboxMessage();

            return result;
        }

        /// <summary>
        /// External injection of a cloud-to-premise message (no premise-tenant segregation)
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public bool ExternalAddToPremiseMessage(string tenantId, Request message)
        {
            TenantContainer tenantContainer = GetTenantContainer(tenantId);
            bool result = tenantContainer.ExternalAddToPremiseMessage(message);

            return result;
        }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class PremiseCommunicationException : Exception
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        public PremiseCommunicationException(String msg)
            : base(msg)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="innerException"></param>
        public PremiseCommunicationException(String msg, Exception innerException)
            : base(msg, innerException)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="context"></param>
        protected PremiseCommunicationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
