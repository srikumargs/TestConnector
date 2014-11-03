using System;

namespace Sage.Connector.MessagingService.Internal
{
    /// <summary>
    /// A simple scheduler that returns the same interval every time
    /// </summary>
    internal sealed class ConstantIntervalScheduler : IScheduler
    {
        /// <summary>
        /// Initializes a new instance of the ConstantIntervalScheduler class
        /// </summary>
        /// <param name="interval"></param>
        public ConstantIntervalScheduler(Int32 interval)
        {
            _interval = interval;
        }

        public int GetTimeToNextWork(bool someWorkDone)
        {
            // Special case for infinite: allow work completion until drained
            if (someWorkDone && (_interval == System.Threading.Timeout.Infinite))
            {
                return 0;
            }
            return _interval;
        }

        private readonly Int32 _interval;
    }
}
