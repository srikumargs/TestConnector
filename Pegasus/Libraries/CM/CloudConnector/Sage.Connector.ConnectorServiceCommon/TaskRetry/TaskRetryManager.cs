using System;
using System.Threading;
using Sage.Connector.Common;
using Sage.Diagnostics;

namespace Sage.Connector.ConnectorServiceCommon.TaskRetry
{
    /// <summary>
    /// Possible results when retrying an action
    /// </summary>
    public enum TaskRetryResult
    {
        /// <summary>
        /// Default
        /// </summary>
        None = 0,

        /// <summary>
        /// Operation exited because it completed normally
        /// </summary>
        Completed,

        /// <summary>
        /// Operation was cancelled
        /// </summary>
        Cancelled,

        /// <summary>
        /// Operation exceeded the set max retries
        /// </summary>
        ExceededMaxRetries
    }

    /// <summary>
    /// Manage the scheduled replay or retry of a unit of work
    /// </summary>
    public class TaskRetryManager
    {
        #region Constructors
        
        /// <summary>
        /// Private default constructor
        /// </summary>
        private TaskRetryManager(){}

        /// <summary>
        /// Fully-specified constructor
        /// </summary>
        /// <param name="retryScheduler"></param>
        /// <param name="transientErrorDetector"></param>
        /// <param name="logger"></param>
        public TaskRetryManager(
            ITaskRetryScheduler retryScheduler,
            ITaskRetryTransientErrorDetector transientErrorDetector,
            ILogging logger)
        {
            // Validate that required param(s) are not null
            ArgumentValidator.ValidateNonNullReference(retryScheduler, "retryScheduler", _myTypeName + ".ctor()");
            ArgumentValidator.ValidateNonNullReference(logger, "logger", _myTypeName + ".ctor()");

            _retryScheduler = retryScheduler;
            _transientErrorDetector = transientErrorDetector;
            _logger = logger;

            // Reset any stats we are keeping for running actions
            Reset();
        }

        #endregion


        #region Public Methods

        /// <summary>
        /// Execute a retryable action in the manner laid out by this instance of the manager.
        /// There are a few ways execution can end:
        ///     - The operation completed successfully
        ///     - We hit the max number of retries
        ///     - The caller issued a cancel
        /// </summary>
        /// <param name="retryAction"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public TaskRetryResult ExecuteWithRetry(Action retryAction, CancellationToken cancellationToken)
        {
            TaskRetryResult result = TaskRetryResult.None;
            Reset();

            // Nothing to do if there is no action
            if (retryAction != null)
            {
                if (cancellationToken == null)
                {
                    // Provide a local cancellation token
                    cancellationToken = new CancellationTokenSource().Token;
                }

                try
                {
                    // Begin the work item loop
                    do
                    {
                        try
                        {
                            // Execute the work item
                            retryAction();

                            // Completed successfully
                            result = TaskRetryResult.Completed;
                        }
                        catch (Exception ex)
                        {
                            if (ShouldRetry(ex))
                            {
                                // Store the retryable exception as the last exception
                                _lastException = ex;

                                // Advance the scheduler
                                if (!_retryScheduler.SetNextInterval())
                                {
                                    // We have exceeded the max number of retries
                                    result = TaskRetryResult.ExceededMaxRetries;

                                    // Log that we exceeded max retries
                                    _logger.WriteError(
                                        this,
                                        String.Format("Exceeded max retries - Count:{0}, Exception:{1}",
                                            CurrentRetryCount,
                                            ex.ExceptionAsString()));
                                }
                                else
                                {
                                    // Log that we are retrying
                                    _logger.WriteError(
                                        this,
                                        String.Format("Retrying - Count:{0}, Delay:{1}, Exception:{2}",
                                            CurrentRetryCount,
                                            LastInterval,
                                            ex.ExceptionAsString()));
                                }
                            }
                            else
                            {
                                // Not a retryable exception
                                // Throw and exit the while loop
                                throw;
                            }
                        }
                    }
                    while (result == TaskRetryResult.None && !cancellationToken.WaitHandle.WaitOne(LastInterval));

                    if (cancellationToken.IsCancellationRequested)
                    {
                        // Record if we exited the loop because of a cancel
                        result = TaskRetryResult.Cancelled;

                        // Log the cancel
                        _logger.WarningTrace(
                            this, String.Format("Exited retry due to cancel - Count:{0}", CurrentRetryCount));
                    }
                }
                catch (Exception ex)
                {
                    // Set the last exception 
                    _lastException = ex;

                    // Log all exceptions and rethrow
                    _logger.WriteError(this, ex.ExceptionAsString());
                    throw;
                }
            }

            return result;
        }

        /// <summary>
        /// Get the current retry count for an executing action
        /// </summary>
        public Int32 CurrentRetryCount
        {
            get { return _retryScheduler.CurrentRetryCount;  }
        }

        /// <summary>
        /// The most recent retry interval used
        /// </summary>
        public Int32 LastInterval
        {
            get { return _retryScheduler.LastInterval;  }
        }

        /// <summary>
        /// Get the last exception generated as a result of the last executed work item
        /// </summary>
        public Exception LastException
        {
            get { return _lastException; }
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// Reset any current execution stats
        /// </summary>
        private void Reset()
        {
            _retryScheduler.Reset();
            _lastException = null;
        }

        /// <summary>
        /// Determine if we should retry an exception
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private bool ShouldRetry(Exception ex)
        {
            bool result = false;
            if (_transientErrorDetector != null)
            {
                result = _transientErrorDetector.IsTransient(ex);
            }

            return result;
        }

        #endregion


        #region Private Members

        private Exception _lastException;
        private readonly ITaskRetryScheduler _retryScheduler;
        private readonly ITaskRetryTransientErrorDetector _transientErrorDetector;
        private readonly ILogging _logger;

        private static readonly String _myTypeName = typeof(TaskRetryManager).FullName;

        #endregion
    }
}
