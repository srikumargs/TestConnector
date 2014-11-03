using System;
using System.ServiceModel;
using Sage.Connector.Cloud.Integration.Interfaces.WebAPI;
using Sage.Connector.MessagingService.Interfaces;
using Sage.Connector.StateService.Interfaces.DataContracts;

namespace Sage.Connector.MessagingService.Proxy.Internal
{
    /// <summary>
    /// The most basic proxy for IMessagingService.
    /// </summary>
    internal sealed class RawMessagingServiceProxy : ClientBase<IMessagingService>, IMessagingService
    {
        /// <summary>
        /// 
        /// </summary>
        public RawMessagingServiceProxy()
            : base()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endpointConfigurationName"></param>
        public RawMessagingServiceProxy(string endpointConfigurationName)
            : base(endpointConfigurationName)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endpointConfigurationName"></param>
        /// <param name="remoteAddress"></param>
        public RawMessagingServiceProxy(string endpointConfigurationName, string remoteAddress)
            : base(endpointConfigurationName, remoteAddress)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endpointConfigurationName"></param>
        /// <param name="remoteAddress"></param>
        public RawMessagingServiceProxy(string endpointConfigurationName, EndpointAddress remoteAddress)
            : base(endpointConfigurationName, remoteAddress)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="remoteAddress"></param>
        public RawMessagingServiceProxy(System.ServiceModel.Channels.Binding binding, EndpointAddress remoteAddress)
            : base(binding, remoteAddress)
        { }

        #region IMessagingService Members
        public ValidateTenantConnectionResponse ValidateTenantConnection(string siteAddress, string tenantId, string premiseId, string wireClaim)
        {
            return base.Channel.ValidateTenantConnection(siteAddress, tenantId, premiseId, wireClaim);
        }

        public TenantRegistrationWithErrorInfo CloudTenantRegistration(Uri siteAddress, string tenantId, string backOfficeCompanyId, string authenticationToken)
        {
            return base.Channel.CloudTenantRegistration(siteAddress, tenantId, backOfficeCompanyId, authenticationToken);
        }

        public TenantRegistrationWithErrorInfo ClearConnectorTenantRegistration(Uri siteAddress, String tenantId,
            String authenticationToken)
        {
            return base.Channel.ClearConnectorTenantRegistration(siteAddress, tenantId, authenticationToken);
        }

        public Cloud.Integration.Interfaces.WebAPI.TenantInfo[] CloudTenantList(Uri siteAddress, string tenantId, string claim)
        {
            return base.Channel.CloudTenantList(siteAddress, tenantId, claim);
        }

        #endregion
    }
}
