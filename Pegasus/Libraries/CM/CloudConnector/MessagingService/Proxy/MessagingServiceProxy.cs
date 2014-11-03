using System;
using Sage.Connector.Cloud.Integration.Interfaces.WebAPI;
using Sage.Connector.MessagingService.Interfaces;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.ServiceModel;

namespace Sage.Connector.MessagingService.Proxy
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class MessagingServiceProxy : RetryClientBase<IMessagingService>, IMessagingService
    {
        /// <summary>
        /// 
        /// </summary>
        public MessagingServiceProxy(RetryClientBase<IMessagingService>.CreationFunction rawProxyCreationFunction)
            : base(rawProxyCreationFunction)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteAddress"></param>
        /// <param name="tenantId"></param>
        /// <param name="premiseId"></param>
        /// <param name="wireClaim"></param>
        public ValidateTenantConnectionResponse ValidateTenantConnection(string siteAddress, string tenantId, string premiseId, string wireClaim)
        {
            return (ValidateTenantConnectionResponse)RetvalCallRawProxy((RetvalMethodInvoker)delegate() { return RawProxy.ValidateTenantConnection(siteAddress, tenantId, premiseId, wireClaim); });
        }

        /// <summary>
        /// Clouds the tenant registration.
        /// </summary>
        /// <param name="siteAddress">The site address.</param>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="backOfficeCompanyId">The back office company identifier.</param>
        /// <param name="authenticationToken">The authentication token required for registration</param>
        /// <returns></returns>
        public TenantRegistrationWithErrorInfo CloudTenantRegistration(Uri siteAddress, string tenantId, string backOfficeCompanyId, string authenticationToken)
        {
            return (TenantRegistrationWithErrorInfo)RetvalCallRawProxy((RetvalMethodInvoker)delegate() { return RawProxy.CloudTenantRegistration(siteAddress, tenantId, backOfficeCompanyId, authenticationToken); });
        }

        /// <summary>
        /// Clear the connector-tenant registraiton
        /// </summary>
        /// <param name="siteAddress"></param>
        /// <param name="tenantId"></param>
        /// <param name="authenticationToken"></param>
        /// <returns></returns>
        public TenantRegistrationWithErrorInfo ClearConnectorTenantRegistration(Uri siteAddress, String tenantId,
            String authenticationToken)
        {
            return (TenantRegistrationWithErrorInfo)RetvalCallRawProxy((RetvalMethodInvoker)delegate() { return RawProxy.ClearConnectorTenantRegistration(siteAddress, tenantId, authenticationToken); });            
        }

        /// <summary>
        /// Clouds the tenant list.
        /// </summary>
        /// <param name="siteAddress">The site address.</param>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <param name="claim">The claim.</param>
        /// <returns></returns>
        public Cloud.Integration.Interfaces.WebAPI.TenantInfo[] CloudTenantList(Uri siteAddress, string tenantId, string claim)
        {
            return (Cloud.Integration.Interfaces.WebAPI.TenantInfo[])RetvalCallRawProxy((RetvalMethodInvoker)delegate() { return RawProxy.CloudTenantList(siteAddress, tenantId, claim); });
        }
    }
}
