using System;
using System.ServiceModel;
using Sage.Connector.StateService.Interfaces;
using Sage.Connector.StateService.Interfaces.DataContracts;

namespace Sage.Connector.StateService.Proxy.Internal
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class RawTenantValidationServiceProxy : ClientBase<ITenantValidationService>, ITenantValidationService
    {
        /// <summary>
        /// 
        /// </summary>
        public RawTenantValidationServiceProxy()
            : base()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endpointConfigurationName"></param>
        public RawTenantValidationServiceProxy(string endpointConfigurationName)
            : base(endpointConfigurationName)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endpointConfigurationName"></param>
        /// <param name="remoteAddress"></param>
        public RawTenantValidationServiceProxy(string endpointConfigurationName, string remoteAddress)
            : base(endpointConfigurationName, remoteAddress)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endpointConfigurationName"></param>
        /// <param name="remoteAddress"></param>
        public RawTenantValidationServiceProxy(string endpointConfigurationName, EndpointAddress remoteAddress)
            : base(endpointConfigurationName, remoteAddress)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="remoteAddress"></param>
        public RawTenantValidationServiceProxy(System.ServiceModel.Channels.Binding binding, EndpointAddress remoteAddress)
            : base(binding, remoteAddress)
        { }

        #region ITenantValidateService Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteAddress"></param>
        /// <param name="tenantId"></param>
        /// <param name="premiseId"></param>
        /// <param name="wireClaim"></param>
        /// <returns></returns>
        public ValidateTenantConnectionResponse ValidateTenantConnection(String siteAddress, String tenantId, String premiseId, String wireClaim)
        { return base.Channel.ValidateTenantConnection(siteAddress, tenantId, premiseId, wireClaim); }

        #endregion
    }
}
