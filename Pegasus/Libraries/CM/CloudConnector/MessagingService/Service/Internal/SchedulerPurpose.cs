
namespace Sage.Connector.MessagingService.Internal
{
    /// <summary>
    /// Used to indicate the purpose for which a scheduler is requested of the SchedulerFactory
    /// </summary>
    internal enum SchedulerPurpose
    {
        /// <summary>
        /// No SchedulerPurpose (default value automatically initialized by runtime)
        /// </summary>
        None = 0,

        /// <summary>
        /// Scheduler for RequestAvailability
        /// </summary>
        RequestAvailability,

        /// <summary>
        /// Scheduler for GetRequests
        /// </summary>
        GetRequests,

        /// <summary>
        /// Scheduler for PutResponses
        /// </summary>
        PutResponses
    }
}
