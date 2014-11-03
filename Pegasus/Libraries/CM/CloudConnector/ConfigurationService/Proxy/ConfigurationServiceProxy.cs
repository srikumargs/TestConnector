using Sage.Connector.ConfigurationService.Interfaces;
using Sage.Connector.Data;
using Sage.ServiceModel;

namespace Sage.Connector.ConfigurationService.Proxy
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ConfigurationServiceProxy : RetryClientBase<IConfigurationService>, IConfigurationService
    {
        /// <summary>
        /// 
        /// </summary>
        public ConfigurationServiceProxy(RetryClientBase<IConfigurationService>.CreationFunction rawProxyCreationFunction)
            : base(rawProxyCreationFunction)
        { }

        #region IConfigurationService Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public PremiseConfigurationRecord[] GetConfigurations()
        {
            return (PremiseConfigurationRecord[])RetvalCallRawProxy((RetvalMethodInvoker)delegate()
            {
                return RawProxy.GetConfigurations();
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public PremiseConfigurationRecord CreateNewConfiguration()
        {
            return (PremiseConfigurationRecord)RetvalCallRawProxy((RetvalMethodInvoker)delegate()
            {
                return RawProxy.CreateNewConfiguration();
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newConfiguration"></param>
        public void AddConfiguration(PremiseConfigurationRecord newConfiguration)
        {
            VoidCallRawProxy((VoidMethodInvoker)delegate() { RawProxy.AddConfiguration(newConfiguration); });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        public void DeleteConfiguration(string tenantId)
        {
            VoidCallRawProxy((VoidMethodInvoker)delegate() { RawProxy.DeleteConfiguration(tenantId);  });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public PremiseConfigurationRecord GetConfiguration(string tenantId)
        {
            return (PremiseConfigurationRecord)RetvalCallRawProxy((RetvalMethodInvoker)delegate()
            {
                return RawProxy.GetConfiguration(tenantId);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="updatedConfiguration"></param>
        public void UpdateConfiguration(PremiseConfigurationRecord updatedConfiguration)
        {
            VoidCallRawProxy((VoidMethodInvoker)delegate() { RawProxy.UpdateConfiguration(updatedConfiguration); });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public LogEntryRecord[] GetLogEntries()
        {
            return (LogEntryRecord[])RetvalCallRawProxy((RetvalMethodInvoker)delegate()
            {
                return RawProxy.GetLogEntries();
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public QueueEntryRecord[] GetQueueEntries()
        {
            return (QueueEntryRecord[])RetvalCallRawProxy((RetvalMethodInvoker)delegate()
            {
                return RawProxy.GetQueueEntries();
            });
        }

        #endregion
    }
}
