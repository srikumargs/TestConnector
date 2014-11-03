using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sage.Connector.DBTool.Internal;

namespace Sage.Connector.DBTool.Migration
{
    /// <summary>
    /// Class to perform migrations
    /// </summary>
    /// <remarks>
    /// Note that there is a tension between chaining version upgrades
    /// and allowing for consolidated upgrades.
    /// 
    /// DB version upgrades are expected to happen but not happen a lot. We are
    /// only talking about changes to the configuration database and possibly new databases
    /// it is expected that the queue and log databases are backed up but not upgraded.
    /// Note that script files are expected to carry along with the database
    /// </remarks>
    internal class MigrationController
    {
        /// <summary>
        /// Do a database schema and file migration.
        /// This is the main entry point
        /// </summary>
        /// <param name="priorVersion"></param>
        /// <param name="priorVersionBackupDir"></param>
        /// <param name="currentVersion"></param>
        /// <param name="instanceDataDir"></param>
        /// <param name="writeLine"></param>
        /// <returns></returns>
        /// <remarks>
        /// Backups must have been done already
        /// </remarks>
        internal ExitCode Migrate(
                        string priorVersion,
                        string priorVersionBackupDir,
                        string currentVersion,
                        string instanceDataDir,
                        Action<string> writeLine)
        {
            ExitCode retval = ExitCode.OneOrMoreFailuresOccurred;

            //Get versions and validate parameters
            Version priorDbVersion;
            Version currentDbVersion;

            bool parametersValidated = Version.TryParse(priorVersion, out priorDbVersion);
            parametersValidated &= Version.TryParse(currentVersion, out currentDbVersion);
            parametersValidated &= Directory.Exists(priorVersionBackupDir);
            parametersValidated &= Directory.Exists(instanceDataDir);
            parametersValidated &= (writeLine != null);

            if (parametersValidated)
            {
                MigrationInfo info = new MigrationInfo( priorDbVersion,
                                                        priorVersionBackupDir,
                                                        currentDbVersion,
                                                        instanceDataDir,
                                                        writeLine);
                retval = Migrate(info);
            }
            return retval;
        }

        /// <summary>
        /// Do the actual migration
        /// We have verified the parameters at this point.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        /// <remarks>
        /// We expect that all parameters have had basic validation at this point.
        /// </remarks>
        private ExitCode Migrate(MigrationInfo info)
        {
            //since we are going to try to do a upgrade, we are only successful if the plan succeeds
            ExitCode retval = ExitCode.OneOrMoreFailuresOccurred;

            //before we do anything else lay down the base files so we are always in a run able state
            //from here on out we are on a "good" path.
            ReplaceAllWithBaselinePlan(info);

            retval = ExitCode.MigrationFailedEmptyFilesInPlace;

            MigrationPlan doMigration = GetMigrationPlan(info.PriorVersion, info.CurrentVersion, info.WriteLine);
            if (doMigration != null)
            {
                try
                {
                    retval = doMigration(info);
                }
                catch (Exception ex)
                {
                    //log the exception
                    info.WriteLine(ex.ToString());

                    //eat the exception, serious ones will bubble and kill process.
                }
                finally
                {
                    if (retval != ExitCode.Success)
                    { 
                        //cleanup the wip files
                        CancelUpdateSequence(info);
                    }
                }

            }                        
            return retval;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="priorVersion"></param>
        /// <param name="currentVersion"></param>
        /// <param name="writeLine"> </param>
        /// <returns></returns>
        private MigrationPlan GetMigrationPlan(Version priorVersion, Version currentVersion, Action<string> writeLine)
        {
            //remember we are dealing "interval" logic
            MigrationPlan plan;
            List<Version> schemaChangeMarkers = MigrationInfo.SchemaChangeVersionMarkers;
            List<Version> changes = schemaChangeMarkers.Where((v)=>(priorVersion < v && v <= currentVersion)).ToList();

            Version minimumVersionSchemaChangeVersion = schemaChangeMarkers[0];
            if (priorVersion < minimumVersionSchemaChangeVersion)
            {
                //support for min version to upgrade from or unknown version
                //should never happen - upgrading from version that is before known versions.
                //This is an error - leave working code
                plan = NoOpEmptyDatabasePlan;
                writeLine("Unrecognized database version detected, using with installed baseline databases.");
            }
            else if (currentVersion < priorVersion)
            {
                //should never happen
                //version is moving backwards, maybe someone trying to move backwards from a beta?
                //we either error, do nothing, or replace it all, for now replace everything
                plan = NoOpEmptyDatabasePlan;
                writeLine("Database version downgrade detected, replacing installed baseline databases..");
            }
            else if (changes.Count == 0)
            {
                //(currentVersion == priorVersion)
                //(priorVersion >= mostRecentSchemaChangeVersion)
                //reinstalling the same version or no schema changes to current version from prior.
                //leave the schema configuration alone.
                plan = NoSchemaUpgradeRequiredPlan;
                writeLine("No database schema change detected, ConfigurationStore unchanged.");
            }
            else
            {
                //we are actually crossing at least one schema change.
                //can we ever consolidate ranges? possibly any changes in the middle would be broken unless they were compensating changes ?
                plan = CreateSequenceRunner(changes);
                writeLine(string.Format("Database schema change detected, updating"));
            }
    
            return plan;
        }

        /// <summary>
        /// Replace every file, this is the "make it work" fall back.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        private ExitCode NoOpSuccessPlan(MigrationInfo info)
        {
            return ExitCode.Success;
        }

        /// <summary>
        /// Do nothing and return empty databases case
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        /// <remarks>
        /// </remarks>
        private ExitCode NoOpEmptyDatabasePlan(MigrationInfo info)
        {
            return ExitCode.MigrationFailedEmptyFilesInPlace;
        }

        #region ReplaceAllFilesPlan
        /// <summary>
        /// Replace every file, this is the "make it work" fall back.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        /// <remarks>
        /// This needs to just pull all files from the baseline.
        /// </remarks>
        private ExitCode ReplaceAllWithBaselinePlan(MigrationInfo info)
        {
            ExitCode retval = ExitCode.Success;

            try
            {
                var fileList = GetAllBaselineFiles(info.InstanceDataDir);

                foreach (string fullFileName in fileList)
                    ReplaceFile(fullFileName, info.InstanceDataDir);

                info.WriteLine("All database files updated to installed version.");
            }
            catch (Exception ex)
            {
                //yeah we are are only re-throwing and the retval assign is not needed or fully correct,
                //but it makes a handy place to but a breakpoint and see what is going on while debugging.
                string msg = ex.ToString();
                //retval = ExitCode.OneOrMoreFailuresOccurred;
                throw;
            }

            return retval;
        }

        #endregion ReplaceAllFilesPlan

        #region ReplaceAllButFilesToUpdatePlan
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        /// <remarks>
        /// This needs to pull all files from the baseline other then the ones we commonly upgrade.
        /// Commonly upgraded files will vary with time possibly.
        /// </remarks>
        private ExitCode NoSchemaUpgradeRequiredPlan(MigrationInfo info)
        {
            ExitCode retval = ExitCode.Success;

            try
            {
                //since the blanks are already in place we need to copy only the relevant data files.
                foreach (string fileName in FileNamesToCarryForwardIfPossible())
                {
                    string fullPathOldFile = Path.Combine(info.PriorVersionBackupDir, fileName);
                    ReplaceFile(fullPathOldFile, info.InstanceDataDir);
                }
            }
            catch (Exception ex)
            {
                //yeah we are are only re-throwing and the retval assign is not needed or fully correct,
                //but it makes a handy place to but a breakpoint and see what is going on while debugging.
                string msg = ex.Message;
                //retval = ExitCode.MigrationFailedEmptyFilesInPlace;
                throw;
            }

            return retval;
        }
        
        private IEnumerable<string> GetAllBaselineFiles(string instanceDataDir)
        {
            string blankDbPath = BlankDBFilePath(instanceDataDir);
            var fileList = Directory.EnumerateFiles(blankDbPath, "*", SearchOption.TopDirectoryOnly);
            return fileList;
        }

        private string BlankDBFilePath(string instanceDataDir)
        {
            return Path.Combine(instanceDataDir, "Baseline");
        }

        private IEnumerable<String> FileNamesToCarryForwardIfPossible()
        {
            //NOTE: if we change the upgraded file list version to version this must change.
            //we really only need this to be accurate for the current version.
            string[] files = new string[] { "ConfigurationStore.sdf" };
            return files;
        }

        private void ReplaceFile(string fullSourceFilePath, string fileDestinationDir)
        {
            string fileName = Path.GetFileName(fullSourceFilePath);
            string fullPathOldFile = Path.Combine(fileDestinationDir, fileName);

            //note: replace with copy and overwrite is a bad idea given read only flag on dev systems.
            if (File.Exists(fullPathOldFile))
            {
                //turn off read only if it exists.
                FileAttributes attrib = File.GetAttributes(fullPathOldFile);
                attrib &= ~FileAttributes.ReadOnly;
                File.SetAttributes(fullPathOldFile, attrib);

                File.Delete(fullPathOldFile);
            }
            File.Copy(fullSourceFilePath, fullPathOldFile);            
        }

        #endregion ReplaceAllButFilesToUpdate

        private MigrationPlan CreateSequenceRunner(IList<Version> versions)
        {
            MigrationPlan plan = (info) =>
            {
                ExitCode retval = ExitCode.Success;
                foreach (Version v in versions)
                {
                    IMigrationStep step = MigrationInfo.GetMigrationStepForVersion(v);
                    retval = step.Upgrade(info);
                    if (retval != ExitCode.Success) break;
                }

                if (retval == ExitCode.Success)
                { 
                    retval = FinishUpdateSequence(info);
                }
                //note: failure cases are cleaned up higher in stack

                return retval;
            };

            return plan;
        }

        private ExitCode FinishUpdateSequence(MigrationInfo info)
        {
            //before we started the upgrade we put all blank files in place in case of issues
            //so the software would run.
            //Now we need to update the configuration store so we do not lose connections
            //and then we need to move the wip files in. Wip files may update configuration store again.

            //The no schema upgrade required plan will cover any files that need to be updated 
            //before we apply the wip files, They will get over written if they are also a wip file.
            ExitCode retval = NoSchemaUpgradeRequiredPlan(info);

            if (retval == ExitCode.Success) { 
                info.MoveWipFilesToLiveAndCleanup();
            }

            return retval;
        }

        private void CancelUpdateSequence(MigrationInfo info)
        {
            info.RemoveWipFiles();
        }
        
        /// <summary>
        /// Standard signature for the plans
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        private delegate ExitCode MigrationPlan(MigrationInfo info);

    }
}

