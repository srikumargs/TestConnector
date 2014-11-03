using System;
using System.ComponentModel.Composition;
using Newtonsoft.Json;
using Sage.Connector.DomainContracts.BackOffice;
using Sage.Connector.DomainMediator.Core.JsonConverters;
using Sage.Connector.Sync.Contracts.CloudIntegration.Responses;

namespace Sage.Connector.DomainMediator.Core
{
    /// <summary>
    /// The SyncCustomers SyncShared Domain Feature implementation.
    /// </summary>
    [Export(typeof(IDomainFeatureRequest))]
    [FeatureMetadataExport(FeatureMessageTypes.ScheduledSynchronization, typeof(FeatureDescriptions), "IScheduledSynchronization")]
    public class ScheduledSynchronization : AbstractDomainMediator
    {
        /// <summary>
        /// The SyncCustomers SyncShared Feature Request Implementation. 
        /// </summary>
        /// <param name="processContext">The <see cref="IProcessContext"/></param>
        /// <param name="requestId">The Connector Request Id</param>
        /// <param name="tenantId">The tenant Id making the request.</param>
        /// <param name="backOfficeCompanyConfiguration">The back office company configuration <see cref="IBackOfficeCompanyConfiguration"/></param>
        /// <param name="payload">String representing the payload to process.</param>
        public override void FeatureRequest(IProcessContext processContext, Guid requestId, string tenantId,
          IBackOfficeCompanyConfiguration backOfficeCompanyConfiguration, string payload)
        {
            var response = new ScheduledSyncResponse();
            String responsePayload = JsonConvert.SerializeObject(response, new DomainMediatorJsonSerializerSettings());
            processContext.ResponseHandler.HandleResponse(requestId, responsePayload);
        }
    }
}
