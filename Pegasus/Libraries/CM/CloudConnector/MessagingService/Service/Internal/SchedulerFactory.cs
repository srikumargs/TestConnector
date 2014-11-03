using System;
using System.Threading;
using Sage.Connector.Data;

namespace Sage.Connector.MessagingService.Internal
{
    /// <summary>
    /// A factory class responsible for handing out schedulers
    /// </summary>
    internal static class SchedulerFactory
    {
        /// <summary>
        /// Creates a scheduler based on the purpose and configuration
        /// </summary>
        /// <param name="purpose"></param>
        /// <param name="configParams"></param>
        /// <param name="pcr"></param>
        /// <returns></returns>
        public static IScheduler Create(SchedulerPurpose purpose, Sage.Connector.Cloud.Integration.Interfaces.WebAPI.Configuration configParams, PremiseConfigurationRecord pcr)
        {
            IScheduler result = null;

            switch (purpose)
            {
                case SchedulerPurpose.RequestAvailability:
                    result = new MinMaxBackOffScheduler(configParams);
                    break;
                case SchedulerPurpose.GetRequests:
                    result = new MinMaxBackOffScheduler(configParams);
                    break;

                case SchedulerPurpose.PutResponses:
                    result = new ConstantIntervalScheduler(Timeout.Infinite);
                    break;

                case SchedulerPurpose.None:
                default:
                    throw new InvalidOperationException(String.Format("Unable to create scheduler for {0}", purpose));
            }

            return result;
        }
    }
}
