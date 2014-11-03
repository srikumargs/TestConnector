using System;
using System.Collections.Generic;
using Sage.Connector.StateService.Interfaces;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.ServiceModel;

namespace Sage.Connector.StateService.Proxy
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class BackOfficeValidationServiceProxy : RetryClientBase<IBackOfficeValidationService>, IBackOfficeValidationService
    {
        /// <summary>
        /// 
        /// </summary>
        public BackOfficeValidationServiceProxy(CreationFunction rawProxyCreationFunction)
            : base(rawProxyCreationFunction)
        { }

        #region IBackOfficeValidationService Members


        /// <summary>
        /// Get the Available Back Office Plugins
        /// </summary>
        /// <returns></returns>
        public BackOfficePluginsResponse GetBackOfficePlugins()
        {
                
            return (BackOfficePluginsResponse)RetvalCallRawProxy((RetvalMethodInvoker)(() =>
                RawProxy.GetBackOfficePlugins()));

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="backOfficeId"></param>
        /// <param name="companyConnectionCredentials"></param>
        /// <returns></returns>
        public ValidateBackOfficeConnectionResponse ValidateBackOfficeConnection(String backOfficeId, IDictionary<string, string> companyConnectionCredentials)
        {
            return (ValidateBackOfficeConnectionResponse)RetvalCallRawProxy((RetvalMethodInvoker)(() =>
                RawProxy.ValidateBackOfficeConnection(backOfficeId, companyConnectionCredentials)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="backOfficeId"></param>
        /// <param name="companyConnectionCredentials"></param>
        /// <returns></returns>
        public ValidateBackOfficeConnectionResponse ValidateBackOfficeConnectionCredentialsAsString(String backOfficeId, string companyConnectionCredentials)
        {
            return (ValidateBackOfficeConnectionResponse)RetvalCallRawProxy((RetvalMethodInvoker)(() =>
                RawProxy.ValidateBackOfficeConnectionCredentialsAsString(backOfficeId, companyConnectionCredentials)));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="backOfficeId"></param>
        /// <param name="credentials"></param>
        /// <returns></returns>
        public ValidateBackOfficeAdminCredentialsResponse ValidateBackOfficeAdminCredentials(String backOfficeId, IDictionary<string, string> credentials)
        {
            return (ValidateBackOfficeAdminCredentialsResponse)RetvalCallRawProxy((RetvalMethodInvoker)(() =>
                RawProxy.ValidateBackOfficeAdminCredentials(backOfficeId, credentials)));
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ManagementCredentialsNeededResponse GetManagementCredentialsNeeded(string backOfficeId)
        {
            return (ManagementCredentialsNeededResponse)RetvalCallRawProxy((RetvalMethodInvoker)(() =>
                RawProxy.GetManagementCredentialsNeeded(backOfficeId)));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="backOfficeId"></param>
        /// <param name="companyManagementCredentials"></param>
        /// <param name="companyConnectionCredentials"></param>
        /// <returns></returns>
        public ConnectionCredentialsNeededResponse GetConnectionCredentialsNeeded(string backOfficeId, IDictionary<string, string> companyManagementCredentials, IDictionary<string, string> companyConnectionCredentials)
        {
            return (ConnectionCredentialsNeededResponse)RetvalCallRawProxy((RetvalMethodInvoker)(() =>
                RawProxy.GetConnectionCredentialsNeeded(backOfficeId, companyManagementCredentials, companyConnectionCredentials)));
        }
    }
}
