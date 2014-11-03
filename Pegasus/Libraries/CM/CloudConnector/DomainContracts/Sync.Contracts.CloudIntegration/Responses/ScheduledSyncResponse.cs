using Sage.Connector.DomainContracts.Responses;

namespace Sage.Connector.Sync.Contracts.CloudIntegration.Responses
{
    /// <summary>
    /// Response to signal connector is ready to start processing synchronization requests
    /// </summary>
    public class ScheduledSyncResponse : Response
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ScheduledSyncResponse()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="response"></param>
        public ScheduledSyncResponse(Response response)
        {
            if (response == null)
                return;

            Status = response.Status;
            Diagnoses = response.Diagnoses;
        }
    }
}
