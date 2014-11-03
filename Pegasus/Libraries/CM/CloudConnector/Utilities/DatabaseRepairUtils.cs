using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlServerCe;
using Sage.Connector.Common;
using Sage.Connector.ConnectorServiceCommon;
using System.Threading;
using System.Data;
using Sage.Connector.StateService.Proxy;

namespace Sage.Connector.Utilities
{
    /// <summary>
    /// Various methods needed for database repair
    /// </summary>
    public static class DatabaseRepairUtils
    {
        #region Public Methods

        /// <summary>
        /// Special case logic for coordinating the various calls that are necessary 
        /// When handling database recovery. This is used by all services for their respective app domains,
        /// With the notable exception of the state service itself.
        /// </summary>
        /// <param name="databaseFilename"></param>
        /// <param name="storePath"></param>
        /// <param name="action"></param>
        public static bool RepairDatabaseCoordinator(string databaseFilename, string storePath, Action action)
        {
            // STEP 1: Verify that the database is in fact corrupt
            // By re-executing the action that detected the corruption
            if (!DatabaseCorruptionHelper.VerifyDatabaseCorruption(action))
            {
                // Database not corrupt, we're done
                return true;
            }

            // STEP 2: Database was corrupt, attempt to repair it
            bool dbRepairSuccessful = false;
            using (var proxy = DatabaseRepairerServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
            {
                dbRepairSuccessful = proxy.RepairDatabase(databaseFilename, storePath);
            }

            if (dbRepairSuccessful)
            {
                // STEP 3: Database repair attempt succeeded
                // Re-attempt the initial action to make sure things worked
                dbRepairSuccessful = DatabaseCorruptionHelper.VerifyDatabaseAfterRepair(action);
            }

            if (!dbRepairSuccessful)
            {
                // STEP 4: Either the repair failed, or re-verification after the repair failed
                // This is a hard database corruption, so handle that
                using (var proxy = DatabaseRepairerServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                {
                    proxy.HandleHardDatabaseCorruption(databaseFilename,
                        new Exception("Attempt to repair corrupt database failed"));
                }
            }

            // Return whether or not we repaired the database
            return dbRepairSuccessful;
        }

        /// <summary>
        /// Attempt to backup and then repair the database provided
        /// This method is called by the state service for in line repair attemtps
        /// And by the db tool for one off repair attempts
        /// </summary>
        /// <param name="sdfFile"></param>
        /// <param name="sdfBackupDestinationFile"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static bool RepairDatabase(
            string sdfFile,
            string sdfBackupDestinationFile,
            ILogging logger)
        {
            // Check existing database file
            if (string.IsNullOrEmpty(sdfFile) || !File.Exists(sdfFile))
            {
                throw new ArgumentException("Database file does not exist");
            }

            // Check backup destination
            string sdfBackupDestinationPath = Path.GetDirectoryName(sdfBackupDestinationFile);
            if (String.IsNullOrEmpty(sdfBackupDestinationPath) ||
                String.IsNullOrEmpty(Path.GetFileName(sdfBackupDestinationFile)))
            {
                throw new ArgumentException("Invalid backup destination file name");
            }

            // Flags needed in the retry
            bool operationSuccessful = false;
            bool backupComplete = false;

            // Create the exclusive access connection string
            string connectionString = CreateConnectionString(sdfFile, true);

            // Setup the retry action
            // We just handle the repair attempt here.  No initial verification
            // Or final re-verification, which is handled by the caller, who has access
            // To the initial failed action.
            Action backupAndRepairRetryAction = new Action(() =>
            {
                using (var repairEngine = new SqlCeEngine(connectionString))
                {
                    // Make sure we can get exclusive access to the sdf file
                    // This prevents the below Repair call from internally getting a 
                    // SqlCeException with native error code 25035 (file sharing violation) 
                    // And returning a false
                    using (FileStream file = File.Open(sdfFile, FileMode.Open, FileAccess.Read, FileShare.None)) { }

                    // STEP 1: Backup database
                    if (!backupComplete)
                    {
                        // First back up the existing database
                        if (!Directory.Exists(sdfBackupDestinationPath))
                        {
                            // Create the missing directory
                            Directory.CreateDirectory(sdfBackupDestinationPath);
                        }
                        File.Copy(sdfFile, sdfBackupDestinationFile, true);
                        backupComplete = true;
                    }

                    // STEP 2: Attempt to repair the database
                    repairEngine.Repair(null, RepairOption.RecoverAllOrFail);

                    // If we made it this far without exception, consider
                    // The operation a success
                    operationSuccessful = true;
                }
            });

            // Execute the repair retry action
            RetryPolicyManager.ExecuteInRetry(
                RetryPurpose.DatabaseCorruptionRecovery,
                backupAndRepairRetryAction,
                logger);

            return operationSuccessful;
        }

        /// <summary>
        /// Test connection to a specific database
        /// </summary>
        /// <param name="databaseFilePath"></param>
        /// <returns></returns>
        public static bool TestConnection(string databaseFilePath)
        {
            if (string.IsNullOrEmpty(databaseFilePath) ||
                !File.Exists(databaseFilePath))
            {
                // Nothing to do
                return false;
            }

            // Exclusive access not needed just to test the connection
            string connectionString = CreateConnectionString(databaseFilePath);

            // Try to connect
            using (SqlCeEngine engine = new SqlCeEngine(connectionString))
            {
                using (SqlCeConnection connection = new SqlCeConnection(connectionString))
                {
                    connection.Open();
                }
            }

            // Done
            return true;
        }

        /// <summary>
        /// Hard corrupt the database by editing the file directly
        /// </summary>
        /// <param name="databaseFilePath"></param>
        /// <param name="backupFilePath"></param>
        /// <returns></returns>
        public static bool HardCorruptDatabase(string databaseFilePath, string backupFilePath)
        {
            if (string.IsNullOrEmpty(databaseFilePath) ||
                string.IsNullOrEmpty(backupFilePath) ||
                !File.Exists(databaseFilePath))
            {
                // Nothing to do
                return false;
            }

            // Do the backup first
            BackupDatabase(databaseFilePath, backupFilePath);

            // Corrupt the existing database
            // Insert some text to the middle of the file
            var allLines = File.ReadAllLines(databaseFilePath).ToList();
            allLines.Insert(allLines.Count() / 2, "I corrupt thee");
            File.WriteAllLines(databaseFilePath, allLines.ToArray());

            // Done
            return true;
        }

        #endregion


        #region Private Methods

        private static string CreateConnectionString(string databaseFilePath, bool isExclusive = false)
        {
            string connectionString = "Data Source = " + databaseFilePath + "; Max Database Size=2048 ";
            if (isExclusive)
            {
                connectionString += "; Mode = Exclusive";
            }
            return connectionString;
        }

        private static void BackupDatabase(string databaseFilePath, string backupFilePath)
        {
            // Just do a simple copy
            File.Copy(databaseFilePath, backupFilePath, true);
        }

        #endregion
    }
}
