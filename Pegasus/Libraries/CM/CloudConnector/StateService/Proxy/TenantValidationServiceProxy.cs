using System;
using Sage.Connector.StateService.Interfaces;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.ServiceModel;

namespace Sage.Connector.StateService.Proxy
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class TenantValidationServiceProxy : RetryClientBase<ITenantValidationService>, ITenantValidationService
    {
        /// <summary>
        /// 
        /// </summary>
        public TenantValidationServiceProxy(RetryClientBase<ITenantValidationService>.CreationFunction rawProxyCreationFunction)
            : base(rawProxyCreationFunction)
        { }

        #region ITenantValidationService Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteAddress"></param>
        /// <param name="tenantId"></param>
        /// <param name="premiseId"></param>
        /// <param name="wireClaim"></param>
        /// <returns></returns>
        public ValidateTenantConnectionResponse ValidateTenantConnection(
            String siteAddress, String tenantId, String premiseId, String wireClaim)
        {
            return (ValidateTenantConnectionResponse)RetvalCallRawProxy((RetvalMethodInvoker)delegate()
            {
                return RawProxy.ValidateTenantConnection(siteAddress, tenantId, premiseId, wireClaim);
            });
        }

        #endregion
    }
}
