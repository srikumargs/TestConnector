using System;

namespace Sage.Connector.ConnectorServiceCommon.TaskRetry
{
    /// <summary>
    /// Interface for a retry scheduler
    /// </summary>
    public interface ITaskRetryScheduler
    {
        /// <summary>
        /// The next interval based on current stats
        /// </summary>
        /// <returns>False if we have exceeded our max retry</returns>
        Boolean SetNextInterval();

        /// <summary>
        /// Reset our stats
        /// </summary>
        void Reset();

        /// <summary>
        /// The current retry count
        /// </summary>
        Int32 CurrentRetryCount { get; }

        /// <summary>
        /// The interval used for the last retry
        /// </summary>
        Int32 LastInterval { get; }
    }
}
