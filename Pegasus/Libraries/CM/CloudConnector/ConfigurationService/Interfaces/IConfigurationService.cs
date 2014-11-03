using System.Net.Security;
using System.ServiceModel;
using Sage.Connector.Data;
using Sage.Connector.LinkedSource;

namespace Sage.Connector.ConfigurationService.Interfaces
{
    /// <summary>
    /// CRUD Management of Premise-Cloud Configurations
    /// </summary>
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceConstants.V1_SERVICE_NAMESPACE, ProtectionLevel = ProtectionLevel.EncryptAndSign)]
    public interface IConfigurationService
    {
        /// <summary>
        /// Retrieves existing premise-cloud configurations
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(DataAccessFaultException))]
        PremiseConfigurationRecord[] GetConfigurations();

        /// <summary>
        /// Creates a new premise-cloud configuration
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(DataAccessFaultException))]
        PremiseConfigurationRecord CreateNewConfiguration();

        /// <summary>
        /// Adds a new premise-cloud configuration
        /// </summary>
        /// <param name="newConfiguration"></param>
        [OperationContract]
        [FaultContract(typeof(DataAccessFaultException))]
        void AddConfiguration(PremiseConfigurationRecord newConfiguration);

        /// <summary>
        /// Deletes a specific premise-cloud configuration
        /// </summary>
        /// <param name="tenantId"></param>
        [OperationContract]
        [FaultContract(typeof(DataAccessFaultException))]
        void DeleteConfiguration(string tenantId);

        /// <summary>
        /// Retrieves a specific premise-cloud configuration
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(DataAccessFaultException))]
        PremiseConfigurationRecord GetConfiguration(string tenantId);

        /// <summary>
        /// Updates an existing premise-cloud configuration
        /// </summary>
        /// <param name="updatedConfiguration"></param>
        [OperationContract]
        [FaultContract(typeof(DataAccessFaultException))]
        void UpdateConfiguration(PremiseConfigurationRecord updatedConfiguration);

        /// <summary>
        /// Retrieves LogEntries
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(DataAccessFaultException))]
        LogEntryRecord[] GetLogEntries();

        /// <summary>
        /// Retrieves QueueEntries
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        [FaultContract(typeof(DataAccessFaultException))]
        QueueEntryRecord[] GetQueueEntries();
    }
}
