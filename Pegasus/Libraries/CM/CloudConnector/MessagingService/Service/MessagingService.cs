using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.ServiceModel;
using Newtonsoft.Json.Linq;
using Sage.Connector.Cloud.Integration.Interfaces.WebAPI;
using Sage.Connector.Common;
using Sage.Connector.ConnectorServiceCommon;
using Sage.Connector.Logging;
using Sage.Connector.MessagingService.ClientProxies;
using Sage.Connector.MessagingService.Interfaces;
using Sage.Connector.MessagingService.Internal;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.Connector.Utilities;
using Sage.Connector.Cloud.Integration.Interfaces;
using Sage.Connector.Cloud.Integration.Interfaces.Faults;
using Sage.Connector.Cloud.Integration.Interfaces.MessageInspectors;
using Sage.CRE.HostingFramework.Interfaces;
using CloudContracts = Sage.Connector.Cloud.Integration.Interfaces.DataContracts;

namespace Sage.Connector.MessagingService
{
    /// <summary>
    /// Handles all communication with cloud. This includes polling the cloud for requests and delivery of responses from
    /// backoffice.  Requests are retrieved from the cloud and placed in the input queue. Responses are retrieved from the
    /// output queue and sent to the cloud.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, ConfigurationName = "MessagingService")]
    [SingletonServiceControl(StartMethod = "Startup", StopMethod = "Shutdown")]
    public sealed class MessagingService : IMessagingService, IDisposable
    {
        /// <summary>
        /// If a SYSTEM environment variable SAGE_CONNECTOR_MESSAGING_SERVICE_BREAK is set to 1,, then 
        /// fire a debugger breakpoint.  This allows us to decide to break into the HostingFramework
        /// startup code without changing any code.
        /// 
        /// Can use the RUNME-ClearMessagingServiceBreak and RUNME-SetMessagingServiceBreak shortcuts
        /// to facilitate changing this variable.
        /// </summary>
        [Conditional("DEBUG")]
        private static void ConditionalDebugBreak()
        {
            String doBreak = Environment.GetEnvironmentVariable("SAGE_CONNECTOR_MESSAGING_SERVICE_BREAK", EnvironmentVariableTarget.Machine);
            if (!String.IsNullOrEmpty(doBreak) && doBreak == "1")
            {
                Debugger.Break();
            }
        }

        /// <summary>
        /// Initializes a new instance of the MessagingService class
        /// </summary>
        public MessagingService()
        {
            ConditionalDebugBreak();

            using (new StackTraceContext(this))
            {
                InitRetryManagerDatabaseRepairMethod();

                lock (_syncObject)
                {
                    _controller = new Controller();
                }
            }
        }

        /// <summary>
        /// Called by the HostingFx whenever the service should start work (e.g., service started or continued after paused)
        /// </summary>
        public void Startup()
        {
            using (new StackTraceContext(this))
            {
                lock (_syncObject)
                {
                    _controller.Startup();
                }
            }
        }

        /// <summary>
        /// Called by the HostingFx whenever the service should cease work (e.g., service stopped or paused)
        /// </summary>
        public void Shutdown()
        {
            using (new StackTraceContext(this))
            {
                lock (_syncObject)
                {
                    _controller.Shutdown();
                }
            }
        }

        /// <summary>
        /// Queries cloud for validation on tenant ID / premise key combination
        /// </summary>
        /// <param name="siteAddress"></param>
        /// <param name="tenantId"></param>
        /// <param name="premiseKey"></param>
        /// <param name="wireClaim"></param>
        /// <returns></returns>
        public ValidateTenantConnectionResponse ValidateTenantConnection(String siteAddress, String tenantId, String premiseKey, String wireClaim)
        {
            var result = new ValidateTenantConnectionResponse(String.Empty, null, TenantConnectivityStatus.None);

            using (var stc = new StackTraceContext(this, "{0}, {1}, {2}", siteAddress, tenantId, premiseKey))
            {
                FaultArgumentValidator.ValidateNonEmptyString(siteAddress, "siteAddress", _myTypeName + ".VerifyConnection()");
                FaultArgumentValidator.ValidateNonEmptyString(tenantId, "tenantId", _myTypeName + ".VerifyConnection()");
                FaultArgumentValidator.ValidateNonEmptyString(premiseKey, "premiseKey", _myTypeName + ".VerifyConnection()");

                try
                {
                    Uri siteAddressUri = new Uri(siteAddress);
                    CloudContracts.PremiseAgent premiseAgent = PremiseAgentHelper.GetPremiseAgent(tenantId);
                    MessageLogger logger = new MessageLogger(LogManager.StaticWriteInfo);

                    using (var configurationProxy = new APIConfigurationServiceProxy(
                        new Uri(siteAddress),
                        @"api/configuration",
                        tenantId,
                        premiseKey,
                        wireClaim,
                        premiseAgent,
                        logger))
                    {
                        var configuration = configurationProxy.GetConfiguration();
                        return new ValidateTenantConnectionResponse(
                            configuration.TenantName,
                            configuration.TenantPublicUri,
                            TenantConnectivityStatus.Normal);
                    }
                } 
                catch (FaultException<IncompatibleClientFault> icf)
                {
                    // Incompatible
                    FaultHelper.WriteFaultAsError(this, icf.Detail);
                    CloudConnectivityStateMonitorHelper.NotifyIncompatibleClientFault(icf.Detail);
                    result = new ValidateTenantConnectionResponse(String.Empty, null,  TenantConnectivityStatus.IncompatibleClient);
                }
                catch (FaultException<ConnectivityFault> cf)
                {
                    // Connection information incorrect for tenant
                    FaultHelper.WriteFaultAsError(this, cf.Detail);
                    result = new ValidateTenantConnectionResponse(String.Empty, null, TenantConnectivityStatus.InvalidConnectionInformation);
                }
                catch (FaultException<RetiredEndpointFault> rf)
                {
                    // Endpoint no longer exists
                    FaultHelper.WriteFaultAsError(this, rf.Detail);
                    result = new ValidateTenantConnectionResponse(String.Empty, null, TenantConnectivityStatus.Reconfigure);
                    // NOTE:  don't we need to add fault handling here for redirection to a new SiteAddress?  Seems like this is
                    //        required in order to handle the possible case where VerifyTenantConnection() is the first call
                    //        made which receives the RetiredEndpointFault.
                    // This is outside of the "normal" flow currently only used from UI.
                }
                catch (FaultException<TenantConnectionDisabledFault> tcdf)
                {
                    // Tenant disabled
                    FaultHelper.WriteFaultAsError(this, tcdf.Detail);
                    result = new ValidateTenantConnectionResponse(String.Empty, null, TenantConnectivityStatus.TenantDisabled);
                }
                catch (Exception ex)
                {
                    // Client side exception
                    using (var lm = new LogManager())
                    {
                        lm.WriteError(this, "Messaging Service: An exception was encountered attempting to verify the cloud connection; exception: {0}", ex.ExceptionAsString());
                    }
                    result = new ValidateTenantConnectionResponse(String.Empty, null, TenantConnectivityStatus.CommunicationFailure);
                }

                stc.SetResult(result);
            }

            return result;
        }


        /// <summary>
        /// Clouds the tenant registration.
        /// </summary>
        /// <param name="siteAddress">The site address.</param>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="backOfficeCompanyId">The back office company identifier.</param>
        /// <param name="authenticationToken">The authentication token required for registration</param>
        /// <returns></returns>
        public TenantRegistrationWithErrorInfo CloudTenantRegistration(Uri siteAddress, String tenantId, String backOfficeCompanyId,
            String authenticationToken)
        {
            var premiseAgent = PremiseAgentHelper.GetPremiseAgent(tenantId);
            MessageLogger logger = new MessageLogger(LogManager.StaticWriteInfo);

            try
            {
                using (var registrationProxy = new APITenantRegistrationProxy(
                    siteAddress,
                    @"api\tenantregistration",
                    tenantId,
                    premiseAgent,
                    logger))
                {
                    var registration = registrationProxy.RegisterTenant(backOfficeCompanyId, authenticationToken);
                    return new TenantRegistrationWithErrorInfo()
                    {
                        Succeeded = true,
                        SiteAddressBaseUri = registration.SiteAddressBaseUri,
                        TenantClaim = registration.TenantClaim,
                        TenantId = registration.TenantId,
                        TenantKey = registration.TenantKey,
                        TenantName = registration.TenantName,
                        TenantUrl = registration.TenantUrl
                    };
                }
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                // Special handler for Forbidden
                if (errorMessage.Contains("403 (Forbidden"))
                {
                    errorMessage = "The tenant is already registered for a different data connection.";
                }
                return new TenantRegistrationWithErrorInfo()
                {
                    Succeeded = false,
                    ErrorMessage = errorMessage
                };
            }
        }

        /// <summary>
        /// Clear the connector-tenant registration.
        /// </summary>
        /// <param name="siteAddress">The site address.</param>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="authenticationToken">The authentication token required for registration</param>
        /// <returns></returns>
        public TenantRegistrationWithErrorInfo ClearConnectorTenantRegistration(Uri siteAddress, String tenantId, String authenticationToken)
        {
            var premiseAgent = PremiseAgentHelper.GetPremiseAgent(tenantId);
            MessageLogger logger = new MessageLogger(LogManager.StaticWriteInfo);

            try
            {
                using (var registrationProxy = new APIClearConnectorRegistrationProxy(
                    siteAddress,
                    @"api\clearconnectorregistration",
                    tenantId,
                    premiseAgent,
                    logger))
                {
                    var registration = registrationProxy.ClearConnectorRegistration(authenticationToken);
                    return new TenantRegistrationWithErrorInfo()
                    {
                        Succeeded = true,
                        SiteAddressBaseUri = registration.SiteAddressBaseUri,
                        TenantClaim = registration.TenantClaim,
                        TenantId = registration.TenantId,
                        TenantKey = registration.TenantKey,
                        TenantName = registration.TenantName,
                        TenantUrl = registration.TenantUrl
                    };
                }
            }
            catch (Exception ex)
            {
                return new TenantRegistrationWithErrorInfo()
                {
                    Succeeded = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Clouds the tenant list.
        /// </summary>
        /// <param name="siteAddress">The site address.</param>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="claim">The sage Id claim .</param>
        /// <returns></returns>
        public Cloud.Integration.Interfaces.WebAPI.TenantInfo[] CloudTenantList(Uri siteAddress, String tenantId, String claim)
        {
            //TODO: add validation and exception value ups

            //TODO this may need a retry of some sort in theory...

            var premiseAgent = PremiseAgentHelper.GetPremiseAgent(tenantId);
            MessageLogger logger = new MessageLogger(LogManager.StaticWriteInfo);


            using (var registrationProxy = new APITenantListProxy(
                siteAddress,
                @"/api/tenantlist",
                tenantId,
                claim,
                premiseAgent,
                logger))
            {
                return registrationProxy.TenantList(claim);
            }

        }

        private static readonly String _myTypeName = typeof(MessagingService).FullName;
        private readonly Controller _controller;
        private readonly Object _syncObject = new Object();


        #region IDisposable Members

        /// <summary>
        /// Dispose resources
        /// </summary>
        public void Dispose()
        {
            if (_controller != null)
            {
                _controller.Dispose();
            }
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// Init the retry policy manager's database repairer method externally
        /// </summary>
        private void InitRetryManagerDatabaseRepairMethod()
        {
            RetryPolicyManager.SetDatabaseRepairMethod(DatabaseRepairUtils.RepairDatabaseCoordinator);
        }

        #endregion
    }
}
