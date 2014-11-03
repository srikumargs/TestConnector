using System;
using System.Data.SqlServerCe;
using System.Linq;
using Sage.Connector.Common;

namespace Sage.Connector.ConnectorServiceCommon
{
    /// <summary>
    /// Enum for the types of db corruption result types
    /// </summary>
    public enum DBCorruptionResultType
    {
        /// <summary>
        /// No db corruption detected
        /// </summary>
        None = 0,

        /// <summary>
        /// Corruption detected, but repair failed
        /// </summary>
        Failed,

        /// <summary>
        /// Corruption detected, repair succceeded
        /// </summary>
        Succeeded
    }

    /// <summary>
    /// Helper methods for detecting and repairing database corruption
    /// </summary>
    public static class DatabaseCorruptionHelper
    {
        #region Public Methods

        /// <summary>
        /// Repackage a database full message into a less 'misleading'
        /// disk/database full message
        /// </summary>
        /// <param name="ex"></param>
        public static void TranslateDatabaseFullException(Exception ex)
        {
            var ceError = GetDatabaseFullExceptionData(ex);
            if (null != ceError)
            {
                // Get the database full path from the exception
                string dbFullPath =
                    (ceError.ErrorParameters != null &&
                     ceError.ErrorParameters.Any())
                    ? ceError.ErrorParameters[0]
                    : String.Empty;

                throw new System.IO.IOException(
                    "You have reached the maximum size of the database" +
                    (String.IsNullOrEmpty(dbFullPath) ? "." : ": " + dbFullPath));
            }
        }
        
        /// <summary>
        /// Detect and handle a dabatase corruption exception
        /// </summary>
        /// <param name="ex">The exception thrown</param>
        /// <param name="failedAction">The initial action that threw the exception</param>
        /// <param name="databaseRepairer">The method that handles db repairs</param>
        /// <returns></returns>
        public static DBCorruptionResultType HandleDatabaseCorruptionException(
            Exception ex,
            Action failedAction,
            Func<string, string, Action, bool> databaseRepairer)
        {
            // Check for a corrupt database exception
            SqlCeError ceError = GetCorruptDatabaseExceptionData(ex);
            if (ceError != null)
            {
                // Get the database full path from the exception
                string dbFullPath =
                    (ceError.ErrorParameters != null && ceError.ErrorParameters.Count() > 0)
                    ? ceError.ErrorParameters[0]
                    : String.Empty;

                if (!String.IsNullOrEmpty(dbFullPath))
                {
                    // Split out the file name and path
                    string dbName = System.IO.Path.GetFileName(dbFullPath);
                    string dbPath = System.IO.Path.GetDirectoryName(dbFullPath); 

                    // Attempt the repair and return results based on 
                    // Whether the repair succeeded
                    return (databaseRepairer(dbName, dbPath, failedAction))
                        ? DBCorruptionResultType.Succeeded
                        : DBCorruptionResultType.Failed;
                }
            }

            // DB corruption exception not detected
            return DBCorruptionResultType.None;
        }

        /// <summary>
        /// Retry the action that we think detected the database corruption
        /// To determine if the db is in fact corrupt
        /// </summary>
        /// <param name="failedAction"></param>
        /// <returns></returns>
        public static bool VerifyDatabaseCorruption(Action failedAction)
        {
            try
            {
                failedAction();

                // Didn't throw an exception
                // So this database is not corrupt
                return false;
            }
            catch (Exception ex)
            {
                return (GetCorruptDatabaseExceptionData(ex) != null);
            }
        }

        /// <summary>
        /// Verify a repaired database by retrying the action that initially 
        /// Caused us to detect the corrupt database
        /// Note: ANY sqlCeException will be considered a failure here!
        /// </summary>
        /// <param name="failedAction"></param>
        /// <returns></returns>
        public static bool VerifyDatabaseAfterRepair(Action failedAction)
        {
            try
            {
                failedAction();
                return true;
            }
            catch (Exception ex)
            {
                using (var logger = new SimpleTraceLogger())
                {
                    string errorMessage;
                    if (ex is SqlCeException || 
                        (ex.InnerException != null && ex.InnerException is SqlCeException))
                    {
                        // Hit another SqlCeException, consider recovery a failure
                        // An example of one that we might get after recovery failed
                        // Is a "table not found" error
                        errorMessage = String.Format("Could not repair corrupt database, exception: {0}", (ex == null) ? "null" : ex.ToString());
                        logger.WriteCriticalWithEventLogging(null, "CorruptDatabaseRecovery", errorMessage);

                        // Failed
                        return false;
                    }

                    // Hit an exception unrelated to the database
                    // Consider the repair to have succeeded
                    errorMessage = String.Format("Re-attempting initial action after database recovery threw an non-database exception: {0}", (ex == null) ? "null" : ex.ToString());
                    logger.WriteError(null, "CorruptDatabaseRecovery: {0}", errorMessage);

                    // Succeeded
                    return true;
                }
            }
        }

        #endregion


        #region Private Methods

        /// <summary>
        /// Return the specific SqlCeError if the provided exception
        /// Is in fact for a corrupt database
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private static SqlCeError GetCorruptDatabaseExceptionData(Exception ex)
        {
             // Exception must be of type SqlCeException
            SqlCeException ceException = ex as SqlCeException;
            if (ceException != null)
            {
                foreach (SqlCeError ceError in ceException.Errors)
                {
                    // Search only for the corrupt database native error
                    if (ceError.NativeError == 25017)
                    {
                        return ceError;
                    }
                }
            }

            // If there is an inner exception, check that as well
            if (ex.InnerException != null)
            {
                // Recursive call with inner exception, if there is one
                return GetCorruptDatabaseExceptionData(ex.InnerException);
            }

            // All other cases, return null
            return null;
        }

        /// <summary>
        /// Return the specific SqlCeError if the provided exception
        /// Is in fact for a full database
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        private static SqlCeError GetDatabaseFullExceptionData(Exception ex)
        {
            // Exception must be of type SqlCeException
            SqlCeException ceException = ex as SqlCeException;
            if (ceException != null)
            {
                foreach (SqlCeError ceError in ceException.Errors)
                {
                    // Search only for the corrupt database native error
                    if (ceError.NativeError == 25104)
                    {
                        return ceError;
                    }
                }
            }

            // If there is an inner exception, check that as well
            if (ex.InnerException != null)
            {
                // Recursive call with inner exception, if there is one
                return GetDatabaseFullExceptionData(ex.InnerException);
            }

            // All other cases, return null
            return null;
        }

        #endregion
    }
}
