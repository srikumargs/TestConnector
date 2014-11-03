using System;

namespace Sage.Connector.ConnectorServiceCommon.TaskRetry
{
    /// <summary>
    /// Base class for our custom detection strategies
    /// </summary>
    public abstract class BaseTaskRetryTransientErrorDetector : ITaskRetryTransientErrorDetector
    {
        #region ITransientErrorDetector Members

        /// <summary>
        /// Return whether or not the exception is considered transient
        /// Relies on derived class implementation of the CheckIsTransient method
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public bool IsTransient(Exception ex)
        {
            return ex != null && 
                (CheckIsTransient(ex) || (ex.InnerException != null && CheckIsTransient(ex.InnerException)));
        }

        #endregion


        #region Abstract Members

        /// <summary>
        /// Derived classes must implement this method to determine if an
        /// Exception is considered transient
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        protected abstract bool CheckIsTransient(Exception ex);

        #endregion
    }


    /// <summary>
    /// Transient error detection logic for remote config
    /// Also used for initializing the back office connection state on tenant
    /// Work coordinator startup
    /// </summary>
    public sealed class GenericTaskRetryTransientErrorDetector : BaseTaskRetryTransientErrorDetector
    {
        /// <summary>
        /// Check if the exception is considered transient
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        protected override bool CheckIsTransient(Exception ex)
        {
            return !(ex is OperationCanceledException || ex is ObjectDisposedException);
        }
    }
}
