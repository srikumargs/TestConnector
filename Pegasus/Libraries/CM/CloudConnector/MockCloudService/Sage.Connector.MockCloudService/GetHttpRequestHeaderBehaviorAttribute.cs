using System;
using System.ServiceModel.Description;
using Sage.Connector.SageCloudService;
using Sage.Connector.Cloud.Integration.Interfaces.DataContracts;
using Sage.Connector.Cloud.Integration.Interfaces.EndpointBehaviors;
using Sage.Connector.Cloud.Integration.Interfaces.Faults;
using Sage.Connector.Cloud.Integration.Interfaces.MessageInspectors;
using Sage.Connector.LinkedSource;


namespace Sage.CRE.Cloud.WebService.Connector.Extensibility
{
    /// <summary>
    /// Add this attribute to the service definition to get header values
    /// From all incoming requests and perform message validation/authentication
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class GetHttpRequestHeaderBehaviorAttribute : Attribute, IServiceBehavior
    {

        #region IServiceBehavior Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceDescription"></param>
        /// <param name="serviceHostBase"></param>
        /// <param name="endpoints"></param>
        /// <param name="bindingParameters"></param>
        public void AddBindingParameters(
            ServiceDescription serviceDescription,
            System.ServiceModel.ServiceHostBase serviceHostBase,
            System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints,
            System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceDescription"></param>
        /// <param name="serviceHostBase"></param>
        public void ApplyDispatchBehavior(
            ServiceDescription serviceDescription,
            System.ServiceModel.ServiceHostBase serviceHostBase)
        {
            // Create the delegate that gets the premise key given the tenant Id
            GetPremiseKeyMethod getPremiseKeyMethod =
                new GetPremiseKeyMethod(PremiseKeyHelper.GetPremiseKeyForTenant);

            // Create the delegate that checks for tenant disabled faults
            CheckTenantDisabled checkTenantDisabledMethod =
                new CheckTenantDisabled(TenantDisabledChecker);

            // Create the delegate that checks for tenant disabled faults
            CheckRetiredEndpoint checkRetiredEndpoint =
                new CheckRetiredEndpoint(RetiredEndpiontChecker);

            // Create the delegate that checks for tenant disabled faults
            CheckIncompatibleClient checkIncompatibleClient =
                new CheckIncompatibleClient(IncompatibleClientChecker);

            // Create the logger delegate
            MessageLogger logger = new MessageLogger(TraceUtils.WriteLine);

            // Add the behavior to each service endpoint
            foreach (ServiceEndpoint endpoint in serviceDescription.Endpoints)
            {
                if (!endpoint.Address.ToString().EndsWith("MockCloudServiceInjection.svc"))
                {
                    endpoint.Behaviors.Add(new GetHttpRequestHeaderDispatchBehavior(
                        getPremiseKeyMethod,
                        checkTenantDisabledMethod,
                        checkRetiredEndpoint,
                        checkIncompatibleClient,
                        logger));
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceDescription"></param>
        /// <param name="serviceHostBase"></param>
        public void Validate(
            ServiceDescription serviceDescription,
            System.ServiceModel.ServiceHostBase serviceHostBase)
        { }

        #endregion


        #region Temporary Delegates For Fault Checking

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="premiseAgent"></param>
        /// <param name="message"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        private bool TenantDisabledChecker(Guid tenantId, PremiseAgent premiseAgent, ref String message, out TenantConnectionDisabledAction action)
        {
            bool? cloudDisabled = MockCloudService.TenantDisabled(tenantId.ToString());
            bool disabled = false;
            action = TenantConnectionDisabledAction.None;

            if (cloudDisabled == null)
            {
                disabled = true;
                action = TenantConnectionDisabledAction.Delete;
            }
            else if (cloudDisabled == true)
            {
                action = TenantConnectionDisabledAction.DisableOnly;
                disabled = true;
            }
            return disabled;
        }

        /// <summary>
        /// TODO: provide real checker for retired endpoint fault scenario
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="premiseAgent"></param>
        /// <param name="endpoint"></param>
        /// <param name="message"></param>
        /// <param name="siteAddressBaseUri"></param>
        /// <returns></returns>
        private bool RetiredEndpiontChecker(Guid tenantId, PremiseAgent premiseAgent, Uri endpoint, ref String message, out Uri siteAddressBaseUri)
        {
            siteAddressBaseUri = new Uri(MockCloudServiceHost.SiteAddress);
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantGuid"></param>
        /// <param name="premiseAgent"></param>
        /// <param name="message"></param>
        /// <param name="minimumInterfaceVersion"></param>
        /// <param name="minimumConnectorProductVersion"></param>
        /// <param name="currentConnectorProductUpgradeInfo"></param>
        /// <returns></returns>
        private bool IncompatibleClientChecker(
            Guid tenantGuid,
            PremiseAgent premiseAgent,
            ref String message,
            out String minimumInterfaceVersion,
            out String minimumConnectorProductVersion,
            out UpgradeInfo currentConnectorProductUpgradeInfo)
        {
            bool clientNotCompatible = false;
            UpgradeView versionInfo = MockCloudService.RetrieveVersionInformation();
            minimumInterfaceVersion = versionInfo.MinInterfaceVersion;
            minimumConnectorProductVersion = versionInfo.MinProductVersion;

            currentConnectorProductUpgradeInfo = null;
            if (minimumInterfaceVersion.CompareTo(premiseAgent.InterfaceVersion) > 0 ||
                minimumConnectorProductVersion.CompareTo(premiseAgent.ConnectorProductVersion) > 0)
            {
                currentConnectorProductUpgradeInfo = MockCloudService.GetUpgradeInfoForRequiredUpdate();
                clientNotCompatible = true;
            }
            return clientNotCompatible;
        }

        #endregion
    }
}
