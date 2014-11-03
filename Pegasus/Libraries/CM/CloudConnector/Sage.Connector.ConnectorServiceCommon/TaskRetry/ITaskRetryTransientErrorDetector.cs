using System;

namespace Sage.Connector.ConnectorServiceCommon.TaskRetry
{
    /// <summary>
    /// Determine if an exception is transient for our purposes
    /// </summary>
    public interface ITaskRetryTransientErrorDetector
    {
        /// <summary>
        /// Returns true if the exception is considered transient
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        bool IsTransient(Exception ex);
    }
}
