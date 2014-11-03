using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Sage.Connector.Common;
using Sage.Connector.LinkedSource;

namespace Sage.Connector.StateService.Interfaces.DataContracts
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Namespace = ServiceConstants.V1_SERVICE_NAMESPACE, Name = "IntegratedConnectionStateContract")]
    public sealed class IntegratedConnectionState : IExtensibleDataObject
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="tenantName"></param>
        /// <param name="tenantUri"></param>
        /// <param name="integrationEnabledStatus"></param>
        /// <param name="tenantConnectivityStatus"></param>
        /// <param name="backOfficeConnectivityStatus"></param>
        /// <param name="lastAttemptedCommunicationWithCloud"></param>
        /// <param name="lastSuccessfulCommunicationWithCloud"></param>
        /// <param name="nextScheduledCommunicationWithCloud"></param>
        /// <param name="requestsReceivedCount"></param>
        /// <param name="nonErrorResponsesSentCount"></param>
        /// <param name="errorResponsesSentCount"></param>
        /// <param name="requestsInProgressCount"></param>
        /// <param name="backOfficeCompanyName"></param>
        /// <param name="backOfficePluginInformation"></param>
        public IntegratedConnectionState(String tenantId,
            String tenantName,
            Uri tenantUri,
            IntegrationEnabledStatus integrationEnabledStatus,
            TenantConnectivityStatus tenantConnectivityStatus,
            BackOfficeConnectivityStatus backOfficeConnectivityStatus,
            DateTime lastAttemptedCommunicationWithCloud,
            DateTime lastSuccessfulCommunicationWithCloud,
            DateTime nextScheduledCommunicationWithCloud,
            UInt32 requestsReceivedCount,
            UInt32 nonErrorResponsesSentCount,
            UInt32 errorResponsesSentCount,
            UInt32 requestsInProgressCount,
            String backOfficeCompanyName,
            BackOfficePluginInformation backOfficePluginInformation)
        {
            TenantId = tenantId;
            TenantName = tenantName;
            TenantUri = tenantUri;
            IntegrationEnabledStatus = integrationEnabledStatus;
            TenantConnectivityStatus = tenantConnectivityStatus;
            BackOfficeConnectivityStatus = backOfficeConnectivityStatus;
            LastAttemptedCommunicationWithCloud = lastAttemptedCommunicationWithCloud;
            LastSuccessfulCommunicationWithCloud = lastSuccessfulCommunicationWithCloud;
            NextScheduledCommunicationWithCloud = nextScheduledCommunicationWithCloud;
            RequestsReceivedCount = requestsReceivedCount;
            NonErrorResponsesSentCount = nonErrorResponsesSentCount;
            ErrorResponsesSentCount = errorResponsesSentCount;
            RequestsInProgressCount = requestsInProgressCount;
            BackOfficeCompanyName = backOfficeCompanyName;
            BackOfficePluginInformation = backOfficePluginInformation;
        }
                
        /// <summary>
        /// Initializes a new instance of the IntegratedConnectionState class from an existing instance and a collection of propertyTuples
        /// </summary>
        /// <param name="source"></param>
        /// <param name="propertyTuples"></param>
        public IntegratedConnectionState(IntegratedConnectionState source, IEnumerable<PropertyTuple> propertyTuples)
        {
            TenantId = source.TenantId;
            TenantName = source.TenantName;
            TenantUri = source.TenantUri;
            IntegrationEnabledStatus = source.IntegrationEnabledStatus;
            TenantConnectivityStatus = source.TenantConnectivityStatus;
            BackOfficeConnectivityStatus = source.BackOfficeConnectivityStatus;
            LastAttemptedCommunicationWithCloud = source.LastAttemptedCommunicationWithCloud;
            LastSuccessfulCommunicationWithCloud = source.LastSuccessfulCommunicationWithCloud;
            NextScheduledCommunicationWithCloud = source.NextScheduledCommunicationWithCloud;
            RequestsReceivedCount = source.RequestsReceivedCount;
            NonErrorResponsesSentCount = source.NonErrorResponsesSentCount;
            ErrorResponsesSentCount = source.ErrorResponsesSentCount;
            RequestsInProgressCount = source.RequestsInProgressCount;
            BackOfficeCompanyName = source.BackOfficeCompanyName;
            BackOfficePluginInformation = source.BackOfficePluginInformation;
            ExtensionData = source.ExtensionData;

            var myPropertyTuples = propertyTuples.Where(x => x.Item1.DeclaringType == typeof(IntegratedConnectionState));
            foreach (var tuple in myPropertyTuples)
            {
                tuple.Item1.SetValue(this, tuple.Item2, null);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "TenantId", IsRequired = true, Order = 0)]
        public String TenantId { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "TenantName", IsRequired = true, Order = 1)]
        public String TenantName { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "TenantUri", IsRequired = true, Order = 2)]
        public Uri TenantUri { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "IntegrationEnabledStatus", IsRequired = true, Order = 3)]
        public IntegrationEnabledStatus IntegrationEnabledStatus { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "TenantConnectivityStatus", IsRequired = true, Order = 4)]
        public TenantConnectivityStatus TenantConnectivityStatus { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "BackOfficeConnectivityStatus", IsRequired = true, Order = 5)]
        public BackOfficeConnectivityStatus BackOfficeConnectivityStatus { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "LastAttemptedCommunicationWithCloud", IsRequired = true, Order = 6)]
        public DateTime LastAttemptedCommunicationWithCloud { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "LastSuccessfulCommunicationWithCloud", IsRequired = true, Order = 7)]
        public DateTime LastSuccessfulCommunicationWithCloud { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "NextScheduledCommunicationWithCloud", IsRequired = true, Order = 8)]
        public DateTime NextScheduledCommunicationWithCloud { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "RequestsReceivedCount", IsRequired = true, Order = 9)]
        public UInt32 RequestsReceivedCount { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "NonErrorResponsesSentCount", IsRequired = true, Order = 10)]
        public UInt32 NonErrorResponsesSentCount { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "ErrorResponsesSentCount", IsRequired = true, Order = 11)]
        public UInt32 ErrorResponsesSentCount { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "RequestsInProgressCount", IsRequired = true, Order = 12)]
        public UInt32 RequestsInProgressCount { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "BackOfficeCompanyName", IsRequired = true, Order = 13)]
        public String BackOfficeCompanyName { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "BackOfficePluginInformation", IsRequired = true, Order = 14)]
        public BackOfficePluginInformation BackOfficePluginInformation { get; private set; }

        /// <summary>
        /// To support forward-compatible data contracts
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }
    }
}
