
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Sage.Connector.DBTool.Migration
{
    internal class MigrationInfo
    {
        /// <summary>
        /// Storage for the general info
        /// </summary>
        public MigrationInfo(
               Version priorVersion,
               string priorVersionBackupDir,
               Version currentVersion,
               string instanceDataDir,
               Action<string> writeLine)
        {
            PriorVersion = priorVersion;
            PriorVersionBackupDir = priorVersionBackupDir;
            CurrentVersion = currentVersion;
            InstanceDataDir = instanceDataDir;
            WriteLine = writeLine;
        }

        public Version PriorVersion { get; protected set; }
        public string PriorVersionBackupDir { get; protected set; }
        public Version CurrentVersion { get; protected set; }
        public string InstanceDataDir { get; protected set; }
        public Action<string> WriteLine { get; protected set; }

    #region Work in progress files
        public string GetWipFileNameCreateIfNeeded(string fileName)
        {
            string retval = null;
            if (!_wipFiles.TryGetValue(fileName, out retval))
            {
                //file not found, copy from backup to temp, and mark in WIP files.
                string tempName = Path.GetTempFileName();
                string fullsource = Path.Combine(PriorVersionBackupDir, fileName);
                File.Copy(fullsource, tempName, true);

                _wipFiles[fileName] = tempName;
                retval = tempName;
            }
            return retval;
        }

        public void MoveWipFilesToLiveAndCleanup()
        {
            foreach (KeyValuePair<string, string> kvp in _wipFiles)
            {
                string dest = Path.Combine(InstanceDataDir, kvp.Key);
                File.Copy(kvp.Value, dest, true);
            }
            RemoveWipFiles();
        }

        public void RemoveWipFiles()
        { 
             foreach (KeyValuePair<string, string> kvp in _wipFiles)
            {
                File.Delete(kvp.Value);
            }
        }

        private readonly Dictionary<string, string> _wipFiles = new Dictionary<string, string>();
    #endregion

    #region SchemaMarkers
        static public List<Version> SchemaChangeVersionMarkers { get { return _schemaChangeMarkers.Keys.ToList(); } }

        static public IMigrationStep GetMigrationStepForVersion(Version version)
        {
            IMigrationStep step;
            _schemaChangeMarkers.TryGetValue(version, out step);
            return step;
        }

        static MigrationInfo()
        {
            _schemaChangeMarkers = new SortedDictionary<Version, IMigrationStep>();
            RegisterSchemaChanges();
        }

        static private void RegisterSchemaChanges()
        {
            //NOTE: We do NOT have a file version stamp. It could be nice but logic does not require it.
            _schemaChangeMarkers.Add(new Version("0.1.0.0"), null);
            _schemaChangeMarkers.Add(new Version("1.0.50304.1"), new Migration1_0_50304_1());
            _schemaChangeMarkers.Add(new Version("1.4.51008.1"), new Migration1_4_51008_1());

            //Left as example 
            //Do not upgrade the log store we just write an empty version of it.
            //_schemaChangeMarkers.Add(new Version("1.1.50516.2"), new Migration1_1_50516_2()); 
        }

        /// <summary>
        /// List of schema changed versions.
        /// List must be in order.
        /// Current pattern needs version string 
        ///  * here 
        ///  * in the two supporting upgrade functions.
        ///  * In the sequencing function.
        /// </summary>
        static private readonly SortedDictionary<Version, IMigrationStep> _schemaChangeMarkers;
    #endregion

    }
}
