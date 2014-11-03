using System;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
using Sage.Connector.Common;
using Sage.CRE.HostingFramework.Interfaces;

namespace Sage.Connector.ConnectorServiceCommon
{
    /// <summary>
    /// Provide an enumeration of retry purposes which are mapped to
    /// Lower level types withing the retry manager
    /// </summary>
    public enum RetryPurpose
    {
        /// <summary>
        /// No RetryPurpose (default value automatically initialized by runtime)
        /// </summary>
        None = 0,

        /// <summary>
        /// Retry for accessing the queue database
        /// </summary>
        QueueStore,

        /// <summary>
        /// Retry for accessing the log database
        /// </summary>
        LogStore,

        /// <summary>
        /// Retry for accessing the configuration database
        /// </summary>
        ConfigurationStore,

        /// <summary>
        /// Retry for putting responses to the cloud
        /// </summary>
        CloudPut,

        /// <summary>
        /// Retry for uploading to the cloud
        /// </summary>
        CloudUpload, 

        /// <summary>
        /// Retry for attempting to recover a corrupt database
        /// </summary>
        DatabaseCorruptionRecovery
    }

    /// <summary>
    /// Access the different defined retry policies
    /// </summary>
    public static class RetryPolicyManager
    {
        #region Public Members

        /// <summary>
        /// Execute an action using a custom provided retry policy
        /// </summary>
        /// <param name="retryPolicy"></param>
        /// <param name="action"></param>
        /// <param name="logger"></param>
        public static void ExecuteCustomPolicyInRetry(
            RetryPolicy retryPolicy,
            Action action,
            ILogging logger)
        {
            AddLoggerToRetryPolicy(retryPolicy, logger);
            retryPolicy.ExecuteAction(action);
        }

        /// <summary>
        /// Version of the below call without a custom error detection strategy
        /// </summary>
        /// <param name="retryPurpose"></param>
        /// <param name="action"></param>
        /// <param name="logger"></param>
        public static void ExecuteInRetry(
            RetryPurpose retryPurpose, 
            Action action,
            ILogging logger)
        {
            ExecuteInRetry(retryPurpose, action, logger, null);
        }

        /// <summary>
        /// Public method to perform the provided action in a retry loop
        /// Using the specified retry policy type
        /// </summary>
        /// <param name="retryPurpose"></param>
        /// <param name="action"></param>
        /// <param name="logger"></param>
        /// <param name="customErrorDetectionStrategy"></param>
        public static void ExecuteInRetry(RetryPurpose retryPurpose, Action action, ILogging logger, ITransientErrorDetectionStrategy customErrorDetectionStrategy)
        {
            EventHandler<RetryingEventArgs> retryHandler;

            RetryPolicy retryPolicy = GetRetryPolicy(retryPurpose, logger, out retryHandler, customErrorDetectionStrategy);

            try
            {
                if (retryHandler != null)
                {
                    retryPolicy.Retrying += retryHandler;
                }

                // Execute the action
                // Exception for all retries having failed must be caught by the caller
                // With the notable exception of a corrupt database exception, which we
                // Try to handle in line if possible
                retryPolicy.ExecuteAction(action);
            }
            catch (Exception ex)
            {
                // First repackage database full exception
                DatabaseCorruptionHelper.TranslateDatabaseFullException(ex);
                // Otherwise, attempt auto corruption fix
                if (DatabaseCorruptionHelper.HandleDatabaseCorruptionException(ex, action, _databaseRepairer) !=
                    DBCorruptionResultType.Succeeded)
                {
                    // Either this was not a corrupt db sqlceexception,
                    // Or corruption was detected but could not be handled
                    throw;
                }
            }
            finally
            {
                 if (retryHandler != null)
                {
                    retryPolicy.Retrying -= retryHandler;
                }
            }
        }

        /// <summary>
        /// Externally set the method to call when we encounter database corruption
        /// </summary>
        /// <param name="method"></param>
        public static void SetDatabaseRepairMethod(Func<string, string, Action, bool> method)
        {
            lock (_syncObject)
            {
                _databaseRepairer = method;
            }
        }

        /// <summary>
        /// Property to control whether we suspend all retries
        /// Determined by static hosting framework data
        /// </summary>
        public static bool SuspendAllRetries
        {
            get
            {
                if ((AppDomainStaticData.Dictionary == null) ||
                    (!AppDomainStaticData.Dictionary.ContainsKey(AppDomainStaticDataKey.IsStopping)))
                {
                    return false;
                }

                // Suspend retries if the hosting framework is currently stopping
                // Do not want to hold that process up by being stuck in a retry!
                bool result;
                Boolean.TryParse(AppDomainStaticData.Dictionary[AppDomainStaticDataKey.IsStopping], out result);
                return result;
            }
        }

        /// <summary>
        /// Utility to check if an exception is considered transient for the retry policy used
        /// For use with unit tests
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="retryPurpose"></param>
        /// <returns></returns>
        public static bool IsTransientExceptionForRetryPolicy(Exception ex, RetryPurpose retryPurpose)
        {
            EventHandler<RetryingEventArgs> retryHandler;

            RetryPolicy retryPolicy = GetRetryPolicy(retryPurpose, null, out retryHandler, null);
            bool isTransient = retryPolicy.ErrorDetectionStrategy.IsTransient(ex);
            return isTransient;
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// Get the correct retry policy based on the provided type
        /// </summary>
        /// <param name="retryPurpose"></param>
        /// <param name="logger"></param>
        /// <param name="customErrorDetectionStrategy">only used by some retry policies</param>
        /// <param name="retryDelegate"></param>
        /// <returns></returns>
        private static RetryPolicy GetRetryPolicy(RetryPurpose retryPurpose, ILogging logger, out EventHandler<RetryingEventArgs> retryDelegate, ITransientErrorDetectionStrategy customErrorDetectionStrategy = null)
        {
            RetryPolicy retryPolicy;
            switch (retryPurpose)
            {
                case RetryPurpose.LogStore:
                    retryPolicy = CreateDatabaseFixedIntervalPolicy();
                    break;
                case RetryPurpose.DatabaseCorruptionRecovery:
                    retryPolicy = CreateDatabaseRecoveryPolicy();
                    break;
                case RetryPurpose.CloudPut:
                    retryPolicy = CreatePutResponsePolicy();
                    break;
                case RetryPurpose.QueueStore:
                case RetryPurpose.ConfigurationStore:
                    retryPolicy = CreateDatabaseExponentialBackoffPolicy();
                    break;
                case RetryPurpose.CloudUpload:
                    retryPolicy = CreateFileUploadRetryPolicy(customErrorDetectionStrategy);
                    break;
                default:
                    throw new ArgumentException(
                        String.Format("Unrecognized retry purpose '{0}'",
                            Enum.GetName(typeof(RetryPurpose), retryPurpose)));
            }

            // Add logging to this policy if provided
            retryDelegate = AddLoggerToRetryPolicy(retryPolicy, logger);
            
            return retryPolicy;
        }

        /// <summary>
        /// Attach a logger to the retrying event of a retry policy
        /// </summary>
        /// <param name="retryPolicy"></param>
        /// <param name="logger"></param>
        /// <returns>The event delegate to be called during a retry.</returns>
        private static EventHandler<RetryingEventArgs> AddLoggerToRetryPolicy(RetryPolicy retryPolicy, ILogging logger)
        {
            if (logger != null)
            {
                EventHandler<RetryingEventArgs> retryDelegate = (sender, args) => logger.WriteError(
                    null,
                    "Retry - Count:{0}, Delay:{1}, Exception:{2}",
                    args.CurrentRetryCount,
                    args.Delay,
                    args.LastException.ExceptionAsString());

                return retryDelegate;
            }

            return null;
        }

        /// <summary>
        /// Get a new instance of the retry policy for uploading files to the cloud
        /// </summary>
        /// <param name="customErrorDetectionStrategy"></param>
        /// <returns></returns>
        private static RetryPolicy CreateFileUploadRetryPolicy(
            ITransientErrorDetectionStrategy customErrorDetectionStrategy = null)
        {
            // Create the exponential backoff strategy
            ExponentialBackoff exponentialBackoffStrategy = new ExponentialBackoff(
                "File Upload Exponential Backoff",
                ConnectorRegistryUtils.ExponentialBackoffRetryCount,
                ConnectorRegistryUtils.ExponentialBackoffMinBackoff,
                ConnectorRegistryUtils.ExponentialBackoffMaxBackoff,
                ConnectorRegistryUtils.ExponentialBackoffDeltaBackoff,
                ConnectorRegistryUtils.ExponentialBackoffFirstFastRetry);

            // Create the policy off the strategy 
            RetryPolicy fileUploadExponentialBackoffPolicy = null;
            if (customErrorDetectionStrategy != null)
            {
                // Custom policy provided
                fileUploadExponentialBackoffPolicy = 
                    new RetryPolicy(customErrorDetectionStrategy, exponentialBackoffStrategy);
            }
            else
            {
                // No custom policy, use default
                fileUploadExponentialBackoffPolicy =
                    new RetryPolicy<DefaultTransientErrorDetectionStrategy>(exponentialBackoffStrategy);
            }

            return fileUploadExponentialBackoffPolicy;
        }

        /// <summary>
        /// Get a new instance of the database fixed interval retry policy
        /// </summary>
        private static RetryPolicy CreateDatabaseFixedIntervalPolicy()
        {
            // Create the fixed interval strategy
            FixedInterval fixedIntervalStrategy = new FixedInterval(
                "Database Fixed Interval",
                ConnectorRegistryUtils.FixedIntervalRetryCount,
                ConnectorRegistryUtils.FixedIntervalRetryInterval,
                ConnectorRegistryUtils.FixedIntervalFirstFastRetry);

            // Create the policy off the strategy
            RetryPolicy dbFixedIntervalPolicy =
                new RetryPolicy<DatabaseTransientErrorDetectionStrategy>(fixedIntervalStrategy);

            return dbFixedIntervalPolicy;
        }

        /// <summary>
        /// Get a new instance of the database recovery fixed interval retry policy
        /// </summary>
        private static RetryPolicy CreateDatabaseRecoveryPolicy()
        {
            // Create the fixed interval strategy
            FixedInterval fixedIntervalStrategy = new FixedInterval(
                "Database Backup Fixed Interval",
                ConnectorRegistryUtils.CrucialFixedIntervalRetryCount,
                ConnectorRegistryUtils.FixedIntervalRetryInterval,
                ConnectorRegistryUtils.FixedIntervalFirstFastRetry);

            // Create the policy off the strategy
            RetryPolicy dbFixedIntervalPolicy =
                new RetryPolicy<DatabaseRecoveryTransientErrorDetectionStrategy>(fixedIntervalStrategy);

            return dbFixedIntervalPolicy;
        }

        /// <summary>
        /// Get a new instance of the default exponential backoff retry policy
        /// </summary>
        private static RetryPolicy CreatePutResponsePolicy()
        {
            // Create the exponential backoff strategy
            ExponentialBackoff exponentialBackoffStrategy = new ExponentialBackoff(
                "Default Exponential Backoff",
                ConnectorRegistryUtils.ExponentialBackoffRetryCount,
                ConnectorRegistryUtils.ExponentialBackoffMinBackoff,
                ConnectorRegistryUtils.ExponentialBackoffMaxBackoff,
                ConnectorRegistryUtils.ExponentialBackoffDeltaBackoff,
                ConnectorRegistryUtils.ExponentialBackoffFirstFastRetry);

            // Create the policy off the strategy
            RetryPolicy defaultExponentialBackoffPolicy =
                new RetryPolicy<DefaultTransientErrorDetectionStrategy>(exponentialBackoffStrategy);

            return defaultExponentialBackoffPolicy;
        }

        /// <summary>
        /// Get a new instance of the database exponential backoff retry policy
        /// </summary>
        private static RetryPolicy CreateDatabaseExponentialBackoffPolicy()
        {
            // Create the exponential backoff strategy
            ExponentialBackoff exponentialBackoffStrategy = new ExponentialBackoff(
                "Database Exponential Backoff",
                ConnectorRegistryUtils.ExponentialBackoffRetryCount,
                ConnectorRegistryUtils.ExponentialBackoffMinBackoff,
                ConnectorRegistryUtils.ExponentialBackoffMaxBackoff,
                ConnectorRegistryUtils.ExponentialBackoffDeltaBackoff,
                ConnectorRegistryUtils.ExponentialBackoffFirstFastRetry);

            // Create the policy off the strategy
            RetryPolicy dbExponentialBackoffPolicy =
                new RetryPolicy<DatabaseTransientErrorDetectionStrategy>(exponentialBackoffStrategy);

            return dbExponentialBackoffPolicy;
        }

        #endregion


        #region Private Members

        /// <summary>
        /// Lock for the dictionary storing which dbs have had hard corruptions detected
        /// </summary>
        private static readonly Object _syncObject = new Object();

        /// <summary>
        /// The method for handling database repairs
        /// Not set to readonly since we need to set this externally
        /// </summary>
        private static Func<string, string, Action, bool> _databaseRepairer =
            delegate (string databaseFilename, string storagePath, Action failedAction) { return false; };

        #endregion
    }
}
