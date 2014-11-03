using System;
using System.Collections.Generic;
using System.ServiceModel;
using Sage.Connector.StateService.Interfaces;
using Sage.Connector.StateService.Interfaces.DataContracts;

namespace Sage.Connector.StateService.Proxy.Internal
{
    internal sealed class RawBackOfficeValidationServiceProxy : ClientBase<IBackOfficeValidationService>, IBackOfficeValidationService
    {
        /// <summary>
        /// 
        /// </summary>
        public RawBackOfficeValidationServiceProxy()
            : base()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endpointConfigurationName"></param>
        public RawBackOfficeValidationServiceProxy(string endpointConfigurationName)
            : base(endpointConfigurationName)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endpointConfigurationName"></param>
        /// <param name="remoteAddress"></param>
        public RawBackOfficeValidationServiceProxy(string endpointConfigurationName, string remoteAddress)
            : base(endpointConfigurationName, remoteAddress)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endpointConfigurationName"></param>
        /// <param name="remoteAddress"></param>
        public RawBackOfficeValidationServiceProxy(string endpointConfigurationName, EndpointAddress remoteAddress)
            : base(endpointConfigurationName, remoteAddress)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="remoteAddress"></param>
        public RawBackOfficeValidationServiceProxy(System.ServiceModel.Channels.Binding binding, EndpointAddress remoteAddress)
            : base(binding, remoteAddress)
        { }

        #region IBackOfficeValidation Members

        /// <summary>
        /// Get the available back office plugins
        /// </summary>
        /// <returns></returns>
        public BackOfficePluginsResponse GetBackOfficePlugins()
        {
            return base.Channel.GetBackOfficePlugins();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="backOfficeId"></param>
        /// <param name="companyConnectionCredentials"></param>
        /// <returns></returns>
        public ValidateBackOfficeConnectionResponse ValidateBackOfficeConnection(String backOfficeId, IDictionary<string, string> companyConnectionCredentials)
        { return base.Channel.ValidateBackOfficeConnection(backOfficeId, companyConnectionCredentials); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="backOfficeId"></param>
        /// <param name="companyConnectionCredentials"></param>
        /// <returns></returns>
        public ValidateBackOfficeConnectionResponse ValidateBackOfficeConnectionCredentialsAsString(String backOfficeId, string companyConnectionCredentials)
        { return base.Channel.ValidateBackOfficeConnectionCredentialsAsString(backOfficeId, companyConnectionCredentials); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="backOfficeId"></param>
        /// <param name="credentials"></param>
        /// <returns></returns>
        public ValidateBackOfficeAdminCredentialsResponse ValidateBackOfficeAdminCredentials(String backOfficeId, IDictionary<string, string> credentials)
        { return base.Channel.ValidateBackOfficeAdminCredentials(backOfficeId, credentials); }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ManagementCredentialsNeededResponse GetManagementCredentialsNeeded(String backOfficeId)
        { return base.Channel.GetManagementCredentialsNeeded(backOfficeId); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="backOfficeId"></param>
        /// <param name="companyManagementCredentials"></param>
        /// <param name="companyConnectionCredentials"></param>
        /// <returns></returns>
        public ConnectionCredentialsNeededResponse GetConnectionCredentialsNeeded(string backOfficeId, IDictionary<string, string> companyManagementCredentials, IDictionary<string, string> companyConnectionCredentials)
        { return base.Channel.GetConnectionCredentialsNeeded(backOfficeId, companyManagementCredentials, companyConnectionCredentials); }
    }
}
