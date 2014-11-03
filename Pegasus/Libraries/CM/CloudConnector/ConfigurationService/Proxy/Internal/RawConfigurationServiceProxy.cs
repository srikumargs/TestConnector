using System.ServiceModel;
using Sage.Connector.ConfigurationService.Interfaces;
using Sage.Connector.Data;

namespace Sage.Connector.ConfigurationService.Proxy.Internal
{
    internal sealed class RawConfigurationServiceProxy : ClientBase<IConfigurationService>, IConfigurationService
    {
        /// <summary>
        /// 
        /// </summary>
        public RawConfigurationServiceProxy()
            : base()
        {
            Initialize();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endpointConfigurationName"></param>
        public RawConfigurationServiceProxy(string endpointConfigurationName)
            : base(endpointConfigurationName)
        {
            Initialize();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endpointConfigurationName"></param>
        /// <param name="remoteAddress"></param>
        public RawConfigurationServiceProxy(string endpointConfigurationName, string remoteAddress)
            : base(endpointConfigurationName, remoteAddress)
        {
            Initialize();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endpointConfigurationName"></param>
        /// <param name="remoteAddress"></param>
        public RawConfigurationServiceProxy(string endpointConfigurationName, EndpointAddress remoteAddress)
            : base(endpointConfigurationName, remoteAddress)
        {
            Initialize();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="remoteAddress"></param>
        public RawConfigurationServiceProxy(System.ServiceModel.Channels.Binding binding, EndpointAddress remoteAddress)
            : base(binding, remoteAddress)
        {
            Initialize();
        }

        private void Initialize()
        {
            // allow impersonation; the ConfigurationService requires to be able to impersonate in order to authenticate the 
            // client caller
            ClientCredentials.Windows.AllowedImpersonationLevel = System.Security.Principal.TokenImpersonationLevel.Impersonation;
        }

        #region IConfigurationService Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public PremiseConfigurationRecord[] GetConfigurations()
        {
            return base.Channel.GetConfigurations();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public PremiseConfigurationRecord CreateNewConfiguration()
        {
            return base.Channel.CreateNewConfiguration();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newConfiguration"></param>
        public void AddConfiguration(PremiseConfigurationRecord newConfiguration)
        {
            base.Channel.AddConfiguration(newConfiguration);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        public void DeleteConfiguration(string tenantId)
        {
            base.Channel.DeleteConfiguration(tenantId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public PremiseConfigurationRecord GetConfiguration(string tenantId)
        {
            return base.Channel.GetConfiguration(tenantId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="updatedConfiguration"></param>
        public void UpdateConfiguration(PremiseConfigurationRecord updatedConfiguration)
        {
            base.Channel.UpdateConfiguration(updatedConfiguration);
        }

        /// <summary>
        /// Retrieves LogEntries
        /// </summary>
        /// <returns></returns>
        public LogEntryRecord[] GetLogEntries()
        {
            return base.Channel.GetLogEntries();
        }

        /// <summary>
        /// Retrieves QueueEntries
        /// </summary>
        /// <returns></returns>
        public QueueEntryRecord[] GetQueueEntries()
        {
            return base.Channel.GetQueueEntries();
        }

        #endregion
    }
}
