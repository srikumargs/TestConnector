using System;
using System.Net.Security;
using System.ServiceModel;
using Sage.Connector.LinkedSource;

namespace Sage.Connector.StateService.Interfaces
{
    /// <summary>
    /// Manage aspects of database repairs
    /// </summary>
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceConstants.V1_SERVICE_NAMESPACE, ProtectionLevel = ProtectionLevel.EncryptAndSign)]
    public interface IDatabaseRepairerService
    {
        /// <summary>
        /// Handler for repairing a database
        /// </summary>
        /// <param name="databaseFilename"></param>
        /// <param name="storagePath"></param>
        [OperationContract]
        bool RepairDatabase(string databaseFilename, string storagePath);

        /// <summary>
        /// Handler for a hard database corruption
        /// </summary>
        /// <param name="databaseFilename"></param>
        /// <param name="ex"></param>
        [OperationContract]
        void HandleHardDatabaseCorruption(string databaseFilename, Exception ex);
    }
}
