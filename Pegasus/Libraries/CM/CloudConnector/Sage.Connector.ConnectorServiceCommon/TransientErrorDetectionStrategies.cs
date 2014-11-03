using System;
using System.Data;
using System.Data.SqlServerCe;
using System.IO;
using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;

namespace Sage.Connector.ConnectorServiceCommon
{
    /// <summary>
    /// Base class for our custom detection strategies
    /// </summary>
    public abstract class BaseTransientErrorDetectionStrategy : ITransientErrorDetectionStrategy
    {
        #region ITransientErrorDetectionStrategy Members

        /// <summary>
        /// Return whether or not the exception is considered transient
        /// Relies on derived class implementation of the CheckIsTransient method
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public bool IsTransient(Exception ex)
        {
            if (RetryPolicyManager.SuspendAllRetries)
            {
                // No exceptions will get retried
                return false;
            }

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


        #region Protected Helpers

        /// <summary>
        /// Single place for determining if a generic sql ce exception is
        /// Considered transient based on the native error code
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        protected bool IsTransientSqlCeException(Exception ex)
        {
            // Only retry database transient exception
            // First single out transient native errors for generic SQL CE exception
            // Note: list of native errors can be found at the below URL
            // msdn.microsoft.com/en-us/library/aa256772%28v=sql.80%29.aspx

            // SqlCeException Native Types that are considered transient:
            // 25035 - file sharing violation when trying to get exclusive access (other process using it)
            bool result = false;
            SqlCeException sqlCeEx = ex as SqlCeException;
            if (sqlCeEx != null)
            {
                result = (sqlCeEx.NativeError == 25035);
            }

            return result;
        }

        /// <summary>
        /// Check entity exceptions for transience
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        protected bool IsTransientEntityException(Exception ex)
        {
            bool result = false;
            EntityException entityEx = ex as EntityException;
            if (entityEx != null)
            {
                // Check for an inner SqlCeException
                SqlCeException sqlCeEx = 
                    (entityEx.InnerException == null)
                    ? null : entityEx.InnerException as SqlCeException;

                // True if the inner exception is not a sql ce exception
                // Or it is a transient sql ce exception
                result = sqlCeEx == null || IsTransientSqlCeException(sqlCeEx);
            }

            return result;
        }

        #endregion
    }


    /// <summary>
    /// Transient error detection logic for the generic case
    /// </summary>
    public sealed class DefaultTransientErrorDetectionStrategy : BaseTransientErrorDetectionStrategy
    {
        /// <summary>
        /// Check if the exception is considered transient
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        protected override bool CheckIsTransient(Exception ex)
        {
            return true;
        }
    }

    /// <summary>
    /// Transient error detection logic for remote config
    /// Also used for initializing the back office connection state on tenant
    /// Work coordinator startup
    /// </summary>
    public sealed class GenericTaskTransientErrorDetectionStrategy : BaseTransientErrorDetectionStrategy
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

    /// <summary>
    /// Transient error detection logic for the database cases
    /// </summary>
    public sealed class DatabaseTransientErrorDetectionStrategy : BaseTransientErrorDetectionStrategy
    {
        /// <summary>
        /// Check if the exception is considered transient
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        protected override bool CheckIsTransient(Exception ex)
        {
            // Only retry database transient exception
            return ((null != ex) &&  (
                IsTransientSqlCeException(ex) ||
                IsTransientEntityException(ex) ||
                ex is EntityException ||
                ex is SqlCeLockTimeoutException || 
                ex is SqlCeTransactionInProgressException ||
                ex is MutexTimeoutExeption
            ));
        }
    }

    /// <summary>
    /// Transient error detection logic for the database recovery cases
    /// Includes all normal transient database exceptions, plus anything file.copy
    /// Related since SQL CE is file based and backing it up requires a file.copy call
    /// </summary>
    public sealed class DatabaseRecoveryTransientErrorDetectionStrategy : BaseTransientErrorDetectionStrategy
    {
        /// <summary>
        /// Check if the exception is considered transient
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        protected override bool CheckIsTransient(Exception ex)
        {
            // Return true for all transient db errors
            // As well as file backup/copy issues
            return ((null != ex) && (
                IsTransientSqlCeException(ex) ||
                ex is SqlCeLockTimeoutException ||
                ex is SqlCeTransactionInProgressException ||
                ex is MutexTimeoutExeption ||
                ex is UnauthorizedAccessException ||
                ex is IOException
            ));
        }
    }
}
