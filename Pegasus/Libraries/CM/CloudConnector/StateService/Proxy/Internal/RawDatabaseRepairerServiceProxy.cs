using System;
using System.ServiceModel;
using Sage.Connector.StateService.Interfaces;

namespace Sage.Connector.StateService.Proxy.Internal
{
    internal sealed class RawDatabaseRepairerServiceProxy : ClientBase<IDatabaseRepairerService>, IDatabaseRepairerService
    {
        /// <summary>
        /// 
        /// </summary>
        public RawDatabaseRepairerServiceProxy()
            : base()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endpointConfigurationName"></param>
        public RawDatabaseRepairerServiceProxy(string endpointConfigurationName)
            : base(endpointConfigurationName)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endpointConfigurationName"></param>
        /// <param name="remoteAddress"></param>
        public RawDatabaseRepairerServiceProxy(string endpointConfigurationName, string remoteAddress)
            : base(endpointConfigurationName, remoteAddress)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endpointConfigurationName"></param>
        /// <param name="remoteAddress"></param>
        public RawDatabaseRepairerServiceProxy(string endpointConfigurationName, EndpointAddress remoteAddress)
            : base(endpointConfigurationName, remoteAddress)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="remoteAddress"></param>
        public RawDatabaseRepairerServiceProxy(System.ServiceModel.Channels.Binding binding, EndpointAddress remoteAddress)
            : base(binding, remoteAddress)
        { }


        #region IDatabaseRepairer Members

        /// <summary>
        /// Repair the provided database
        /// </summary>
        /// <param name="databaseFilename"></param>
        /// <param name="storagePath"></param>
        /// <returns></returns>
        public bool RepairDatabase(string databaseFilename, string storagePath)
        { return base.Channel.RepairDatabase(databaseFilename, storagePath); }

        /// <summary>
        /// Handle hard database corruption
        /// </summary>
        /// <param name="databaseFilename"></param>
        /// <param name="ex"></param>
        public void HandleHardDatabaseCorruption(string databaseFilename, Exception ex)
        { base.Channel.HandleHardDatabaseCorruption(databaseFilename, ex); }

        #endregion
    }
}
