using System;
using Sage.Connector.Common;

namespace Sage.Connector.ConnectorServiceCommon.TaskRetry
{
    /// <summary>
    /// A factory class responsible for handing out schedulers
    /// </summary>
    public static class TaskRetrySchedulerFactory
    {
        /// <summary>
        /// Creates a task retry scheduler for initializing back office connection data
        /// </summary>
        /// <returns></returns>
        public static ITaskRetryScheduler CreateInitBackOfficeConnectionTaskRetryScheduler()
        {
            ITaskRetryScheduler result = new FixedIntervalTaskRetryScheduler(
                ConnectorRegistryUtils.InitBackOfficeConnectionStateRetryCount,
                ConnectorRegistryUtils.FixedIntervalRetryInterval);
                    
            return result;
        }

        /// <summary>
        /// Creates a task retry scheduler for getting remote config data
        /// </summary>
        /// <param name="minBackoff"></param>
        /// <param name="maxBackoff"></param>
        /// <returns></returns>
        public static ITaskRetryScheduler CreateGetRemoteConfigTaskRetryScheduler(Int32 minBackoff, Int32 maxBackoff)
        {
            ITaskRetryScheduler result = new ExponentialBackoffTaskRetryScheduler(
                Int32.MaxValue,
                new TimeSpan(0, 0, 0, 0, minBackoff),
                new TimeSpan(0, 0, 0, 0, maxBackoff),
                ConnectorRegistryUtils.ExponentialBackoffDeltaBackoff);

            return result;
        }
    }
}
