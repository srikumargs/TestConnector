using System;
using Sage.Connector.Common;

namespace Sage.Connector.ConnectorServiceCommon.TaskRetry
{
    /// <summary>
    /// Base class that handles common retry scheduler logic
    /// </summary>
    public abstract class BaseTaskRetryScheduler : ITaskRetryScheduler
    {
        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public BaseTaskRetryScheduler()
        {
            // Provide defaults
            MaxRetryCount = 1;

            // Reset stats
            Reset();
        }

        /// <summary>
        /// Fully-specified constructor
        /// </summary>
        /// <param name="maxRetryCount"></param>
        public BaseTaskRetryScheduler(int maxRetryCount)
            : this()
        {
            MaxRetryCount = maxRetryCount;
        }

        #endregion


        #region IRetryScheduler Members

        /// <summary>
        /// The next interval based on current stats
        /// </summary>
        /// <returns>False if we exceeded our max retry count</returns>
        public Boolean SetNextInterval()
        {
            // Increase the retry count
            if (++CurrentRetryCount > MaxRetryCount)
            {
                // We exceeded our max retry count
                return false;
            }
            
            // Get the interval from the derived class implementation
            LastInterval = GetNextInterval();

            return true;
        }

        /// <summary>
        /// Reset our stats
        /// </summary>
        public void Reset()
        {
            CurrentRetryCount = 0;
            LastInterval = 0;
        }

        /// <summary>
        /// The current retry count
        /// </summary>
        public Int32 CurrentRetryCount
        {
            get { return _currentRetryCount; }
            protected set { _currentRetryCount = value; }
        }

        /// <summary>
        /// The interval used for the last retry
        /// </summary>
        public Int32 LastInterval
        {
            get { return _lastInterval; }
            protected set { _lastInterval = value;  }
        }

        #endregion


        #region Abstract Methods

        /// <summary>
        /// Derived class determines how to calculate the next interval to use
        /// </summary>
        /// <returns></returns>
        protected abstract Int32 GetNextInterval();

        #endregion


        #region Other Public Members

        /// <summary>
        /// Maximum number of retries we can attempt
        /// </summary>
        public Int32 MaxRetryCount
        {
            get { return _maxRetryCount; }
            set { _maxRetryCount = value; }
        }

        #endregion


        #region Private Members

        private Int32 _currentRetryCount;
        private Int32 _lastInterval;
        private Int32 _maxRetryCount;

        #endregion
    }


    /// <summary>
    /// Exponential backoff retry scheduler
    /// </summary>
    public sealed class ExponentialBackoffTaskRetryScheduler : BaseTaskRetryScheduler
    {
        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public ExponentialBackoffTaskRetryScheduler()
            : base()
        { 
            // Provide defaults
            MaxRetryCount = ConnectorRegistryUtils.ExponentialBackoffRetryCount;
            MinBackoff = ConnectorRegistryUtils.ExponentialBackoffMinBackoff;
            MaxBackoff = ConnectorRegistryUtils.ExponentialBackoffMaxBackoff;
            DeltaBackoff = ConnectorRegistryUtils.ExponentialBackoffDeltaBackoff;
        }

        /// <summary>
        /// Fully-specified constructor
        /// </summary>
        /// <param name="maxRetryCount"></param>
        /// <param name="minBackoff"></param>
        /// <param name="maxBackoff"></param>
        /// <param name="deltaBackoff"></param>
        public ExponentialBackoffTaskRetryScheduler(
            Int32 maxRetryCount, 
            TimeSpan minBackoff, 
            TimeSpan maxBackoff, 
            TimeSpan deltaBackoff)
            : base(maxRetryCount)
        {
            MinBackoff = minBackoff;
            MaxBackoff = maxBackoff;
            DeltaBackoff = deltaBackoff;
        }

        #endregion


        #region Overrides

        /// <summary>
        /// Overridden get next interval
        /// </summary>
        /// <returns></returns>
        protected override Int32 GetNextInterval()
        {
            // Get the max backoff or the last backoff plus the delta
            // Whichever is less
            Int32 result = Math.Min(
                (Int32)MaxBackoff.TotalMilliseconds,
                LastInterval + (Int32)DeltaBackoff.TotalMilliseconds);

            return result;
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// The min timespan to wait before retrying
        /// </summary>
        public TimeSpan MinBackoff
        {
            get { return _minBackoff; }
            set { _minBackoff = value; }
        }

        /// <summary>
        /// The max timespan to wait before retrying
        /// </summary>
        public TimeSpan MaxBackoff
        {
            get { return _maxBackoff; }
            set { _maxBackoff = value; }
        }

        /// <summary>
        /// The delta to increase the backoff by
        /// </summary>
        public TimeSpan DeltaBackoff
        {
            get { return _deltaBackoff; }
            set { _deltaBackoff = value; }
        }

        #endregion

        #region Private Members

        private TimeSpan _minBackoff;
        private TimeSpan _maxBackoff;
        private TimeSpan _deltaBackoff;

        #endregion
    }

    /// <summary>
    /// Fixed interval task retry scheduler
    /// </summary>
    public sealed class FixedIntervalTaskRetryScheduler : BaseTaskRetryScheduler
    {
        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        public FixedIntervalTaskRetryScheduler()
            : base()
        {
            // Provide defaults
            MaxRetryCount = ConnectorRegistryUtils.FixedIntervalRetryCount;
            RetryInterval = ConnectorRegistryUtils.FixedIntervalRetryInterval;
        }

        /// <summary>
        /// Fully-specified constructor
        /// </summary>
        /// <param name="maxRetryCount"></param>
        /// <param name="retryInterval"></param>
        public FixedIntervalTaskRetryScheduler(
            Int32 maxRetryCount,
            TimeSpan retryInterval)
            : base(maxRetryCount)
        {
            RetryInterval = retryInterval;
        }

        #endregion


        #region Overrides

        /// <summary>
        /// Overridden get next interval
        /// </summary>
        /// <returns></returns>
        protected override Int32 GetNextInterval()
        {
            // Always the same for this scheduler
            return (Int32)RetryInterval.TotalMilliseconds;
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// The fixed retry interval
        /// </summary>
        public TimeSpan RetryInterval
        {
            get { return _retryInterval; }
            set { _retryInterval = value; }
        }

        #endregion

        #region Private Members

        private TimeSpan _retryInterval;

        #endregion
    }
}
