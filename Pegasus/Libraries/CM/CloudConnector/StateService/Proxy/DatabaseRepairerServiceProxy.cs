using System;
using Sage.Connector.StateService.Interfaces;
using Sage.ServiceModel;

namespace Sage.Connector.StateService.Proxy
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class DatabaseRepairerServiceProxy : RetryClientBase<IDatabaseRepairerService>, IDatabaseRepairerService
    {
        /// <summary>
        /// 
        /// </summary>
        public DatabaseRepairerServiceProxy(RetryClientBase<IDatabaseRepairerService>.CreationFunction rawProxyCreationFunction)
            : base(rawProxyCreationFunction)
        { }


        #region IDatabaseRepairer Members

        /// <summary>
        /// Repair the given database
        /// </summary>
        /// <param name="databaseFilename"></param>
        /// <param name="storagePath"></param>
        /// <returns>Whether or not the repair succeeded</returns>
        public bool RepairDatabase(string databaseFilename, string storagePath)
        {
            return (bool)RetvalCallRawProxy((RetvalMethodInvoker)delegate()
            {
                return RawProxy.RepairDatabase(databaseFilename, storagePath);
            });
        }

        /// <summary>
        /// Handle hard database corruption
        /// </summary>
        /// <param name="databaseFilename"></param>
        /// <param name="ex"></param>
        public void HandleHardDatabaseCorruption(string databaseFilename, Exception ex)
        {
            VoidCallRawProxy((VoidMethodInvoker)delegate() { RawProxy.HandleHardDatabaseCorruption(databaseFilename, ex); });
        }

        #endregion
    }
}
