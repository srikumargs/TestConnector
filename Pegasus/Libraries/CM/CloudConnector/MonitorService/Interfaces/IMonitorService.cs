using System;
using System.Net.Security;
using System.ServiceModel;
using Sage.Connector.Common.DataContracts;
using Sage.Connector.MonitorService.Interfaces.DataContracts;
using Sage.Connector.LinkedSource;

namespace Sage.Connector.MonitorService.Interfaces
{
    /// <summary>
    /// CRUD Management of Premise-Cloud Configurations
    /// </summary>
    [ServiceContract(SessionMode = SessionMode.Allowed, Namespace = ServiceConstants.V1_SERVICE_NAMESPACE, ProtectionLevel = ProtectionLevel.EncryptAndSign)]
    public interface IMonitorService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        ConnectorServiceState GetConnectorServiceState();

        /// <summary>
        /// Retrieves the recently created request entries as well as all currently in-progress ones
        /// </summary>
        /// <param name="recentEntriesThreshold"></param>
        /// <returns></returns>
        [OperationContract]
        RequestState[] GetRecentAndInProgressRequestsState(TimeSpan recentEntriesThreshold);
    }
}
