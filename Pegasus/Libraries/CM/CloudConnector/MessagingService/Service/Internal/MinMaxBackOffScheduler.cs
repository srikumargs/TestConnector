using System;
using Sage.Diagnostics;

namespace Sage.Connector.MessagingService.Internal
{
    /// <summary>
    /// A simple scheduler that understands min and max intervals; and backs off when no work has been done
    /// Uses the config params for an AVAILABLE cloud, and determines peak vs. normal hours
    /// </summary>
    internal class MinMaxBackOffScheduler : IScheduler
    {
        /// <summary>
        /// Initializes a new instance of the BasicMinMaxBackOffScheduler class
        /// </summary>
        /// <param name="configParams"></param>
        public MinMaxBackOffScheduler(Cloud.Integration.Interfaces.WebAPI.Configuration configParams)
        {
            ArgumentValidator.ValidateNonNullReference(configParams, "configParams", _myTypeName + ".ctor()");
            _configParams = configParams;
            _lastInterval = Convert.ToUInt32(configParams.MinCommunicationFailureRetryInterval.TotalMilliseconds);
        }

        public Int32 GetTimeToNextWork(Boolean someWorkDone)
        {
            // Default to the min request interval
            UInt32 result = Convert.ToUInt32(_configParams.MinCommunicationFailureRetryInterval.TotalMilliseconds);

            if (!someWorkDone)
            {
                // If work was not done, double the wait time up to the max allowable interval
                result = System.Math.Min(Convert.ToUInt32(_configParams.MaxCommunicationFailureRetryInterval.TotalMilliseconds), _lastInterval * 2);
            }

            // Store the most recent interval used
            _lastInterval = result;

            return (Int32) result;
        }

        private static readonly String _myTypeName = typeof(MinMaxBackOffScheduler).FullName;
        private readonly Cloud.Integration.Interfaces.WebAPI.Configuration _configParams;
        private UInt32 _lastInterval;
    }
}
