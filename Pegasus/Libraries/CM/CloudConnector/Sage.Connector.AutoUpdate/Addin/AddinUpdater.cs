using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using Sage.Connector.Common;
using Sage.Connector.Logging;
using Sage.Ssdp.Security.Client;

namespace Sage.Connector.AutoUpdate.Addin
{
    /// <summary>
    /// Class for handling the automatic updating of addin files.
    /// </summary>
    public sealed class AddinUpdater : IAddinUpdater
    {
        #region Private Members

        private Timer _timer;
        private DateTime _lastUpdateCheck = DateTime.MinValue;
        private Mutex _lockUpdate = new Mutex(false, AddinNames.Lock);
        private CancellationTokenSource _cancel = new CancellationTokenSource();
        private readonly object _lock = new object();
        private readonly string _packageId;
        private string _productId;
        private readonly string _addinPath;
        private string _backupPath;
        private string _stagingPath;
        private string _productVersion;
        private string _currentUpdate;
        private volatile bool _updateAvailable;
        private AddinNames _addinNames;
        private bool _disposed;
        private string _addinComponentBaseName;
        private Uri _downloadSource;
        private string _updateFolderName;
        private int _updateFailureCount;
        private bool _isCustomUpdate;
        private bool _isCustomBackup;
        private Action<ISoftwareUpdate, string> _customCheckAction;
        private string _stagedUpdateFile;

        #endregion

        #region Private Methods

        /// <summary>
        /// Returns only the filename part from an absolute or relative path spec. 
        /// </summary>
        /// <param name="fileSpec">The absolute or relative path to process.</param>
        /// <returns>The filename part of the file spec.</returns>
        private static string GetFileName(string fileSpec)
        {
            return String.IsNullOrEmpty(fileSpec) ? String.Empty : Path.GetFileName(fileSpec);
        }

        /// <summary>
        /// Performs a file copy or delete after checking for the existence of the file.
        /// </summary>
        /// <param name="sourceFile">The source file to perform the operation on.</param>
        /// <param name="destFile">The destination file name if performing a file copy</param>
        private static void FileOperation(string sourceFile, string destFile = null)
        {
            if (String.IsNullOrEmpty(sourceFile)) throw new ArgumentNullException("sourceFile");

            if (!File.Exists(sourceFile)) return;

            if (String.IsNullOrEmpty(destFile))
            {
                File.Delete(sourceFile);
            }
            else
            {
                File.Copy(sourceFile, destFile, true);
            }
        }

        /// <summary>
        /// Deletes all files from the specified directory. Optionally creates the directory if it
        /// does not exist.
        /// </summary>
        /// <param name="path">The name of the the directory path to purge files from.</param>
        /// <param name="createIfNotExist">True if the directory should be created, otherwise false.</param>
        /// <returns>True if the folder was cleared, otherwise false.</returns>
        private static bool PurgeFolder(string path, bool createIfNotExist = false)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    if (createIfNotExist)
                    {
                        Directory.CreateDirectory(path);
                    }
                    return true;
                }

                foreach (var file in Directory.EnumerateFileSystemEntries(path).Where(File.Exists))
                {
                    File.Delete(file);
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Determines if the file to be transferred is valid or not. The version file is considered
        /// invalid, as is any string that is null or empty.
        /// </summary>
        /// <param name="fileName">The name of the file to process.</param>
        /// <returns>True if the filename is valid, otherwise false.</returns>
        private static bool IsValidFile(string fileName)
        {
            if (String.IsNullOrEmpty(fileName)) return false;

            return (!GetFileName(fileName).Equals(AddinNames.Version, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Determines if the file to be transferred is something other than an assembly or pdb debug file.
        /// </summary>
        /// <param name="fileName">The name of the file to process.</param>
        /// <returns>True if the filename is not an assembly or pdb file, otherwise false.</returns>
        private static bool IsNonAssembly(string fileName)
        {
            if (String.IsNullOrEmpty(fileName)) return false;

            if (GetFileName(fileName).Equals(AddinNames.Version, StringComparison.OrdinalIgnoreCase)) return false;
            if (GetFileName(fileName).Equals(AddinNames.Update, StringComparison.OrdinalIgnoreCase)) return false;
            if (Path.GetExtension(fileName).Equals(".dll", StringComparison.OrdinalIgnoreCase)) return false;
            if (Path.GetExtension(fileName).Equals(".pdb", StringComparison.OrdinalIgnoreCase)) return false;

            return true;
        }

        /// <summary>
        /// Gets the version information from the addin.version file located in the specified directory.
        /// </summary>
        /// <param name="path">The path to use for locating the addin.version file.</param>
        /// <returns>The version if found, otherwise null.</returns>
        private static string ReadVersionFile(string path)
        {
            if (!File.Exists(Path.Combine(path, AddinNames.Version))) return null;

            try
            {
                using (var stream = new StreamReader(Path.Combine(path, AddinNames.Version)))
                {
                    return stream.ReadToEnd().Trim();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Creates/ writes the addin.version file with the specified version in the specified path.
        /// </summary>
        /// <param name="path">The path to generate the addin.version file.</param>
        /// <param name="version">The version to write to the addin.version file.</param>
        private static void WriteVersionFile(string path, string version)
        {
            if (String.IsNullOrEmpty(path)) throw new ArgumentNullException("path");
            if (String.IsNullOrEmpty(version)) throw new ArgumentNullException("version");

            using (var stream = new StreamWriter(Path.Combine(path, AddinNames.Version), false))
            {
                stream.Write(version);
            }
        }


        /// <summary>
        /// Gets the version information from the addin.update file located in the specified directory.
        /// </summary>
        /// <param name="path">The path to use for locating the addin.update file.</param>
        /// <returns>The update file contents if found, otherwise null.</returns>
        private static string ReadUpdateFile(string path)
        {
            if (!File.Exists(Path.Combine(path, AddinNames.Update))) return null;

            try
            {
                using (var stream = new StreamReader(Path.Combine(path, AddinNames.Update)))
                {
                    return stream.ReadToEnd().Trim();
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Creates/ writes the addin.update file with the specified update key in the specified path.
        /// </summary>
        /// <param name="path">The path to generate the addin.update file.</param>
        /// <param name="update">The update.</param>
        /// <exception cref="System.ArgumentNullException">path
        /// or
        /// update</exception>
        private static void WriteUpdateFile(string path, string update)
        {
            if (String.IsNullOrEmpty(path)) throw new ArgumentNullException("path");
            if (String.IsNullOrEmpty(update)) throw new ArgumentNullException("update");

            using (var stream = new StreamWriter(Path.Combine(path, AddinNames.Update), false))
            {
                stream.Write(update);
            }
        }


        ///// <summary>
        ///// Copies non .dll and .pdb files from the source folder to the destination folder. This handles the 
        ///// tenant databases
        ///// </summary>
        ///// <param name="sourcePath">The source path to copy files from.</param>
        ///// <param name="destPath">The destination path for the files to be copied to.</param>
        //private static void CopyNonAssemblies(string sourcePath, string destPath)
        //{
        //    if (String.IsNullOrEmpty(sourcePath)) throw new ArgumentNullException("sourcePath");
        //    if (String.IsNullOrEmpty(destPath)) throw new ArgumentNullException("destPath");
        //    if (!Directory.Exists(sourcePath)) throw new ArgumentException("sourcePath does not exist");
        //    if (!Directory.Exists(destPath)) throw new ArgumentException("destPath does not exist");

        //    var files = Directory.EnumerateFileSystemEntries(sourcePath).Where(IsNonAssembly).ToList();

        //    foreach (var file in files)
        //    {
        //        if (File.Exists(file))
        //        {
        //            File.Copy(file, Path.Combine(destPath, GetFileName(file)), true);
        //        }
        //    }
        //}


        /// <summary>
        /// Adds the contents of a folder to the specified compressed archive.
        /// </summary>
        /// <param name="archive">The compressed archive to add the folder contents to.</param>
        /// <param name="folder">The name of the child folder which is relative to the addin folder.</param>
        /// <param name="log">The enumerable to log any failures to.</param>
        /// <returns>This is written to capture all contents, including empty folders.</returns>
        private void BackupFolder(ZipArchive archive, string folder, ICollection<string> log)
        {
            if (archive == null) throw new ArgumentNullException("archive");
            if (String.IsNullOrEmpty(folder)) throw new ArgumentNullException("folder");
            if (log == null) throw new ArgumentNullException("log");

            archive.CreateEntry(folder + "/");

            foreach (var file in Directory.EnumerateFileSystemEntries(Path.Combine(_addinPath, folder.Replace('/', '\\'))))
            {
                try
                {
                    var relative = folder + "/" + GetFileName(file);

                    if (File.Exists(file))
                    {
                        var ext = Path.GetExtension(file) ?? String.Empty;

                        if (ext.Equals(".dll", StringComparison.OrdinalIgnoreCase) || ext.Equals(".pdb", StringComparison.OrdinalIgnoreCase))
                        {
                            archive.CreateEntryFromFile(file, relative, CompressionLevel.Fastest);
                        }
                    }
                    else if (Directory.Exists(file))
                    {
                        BackupFolder(archive, relative, log);
                    }
                }
                catch (Exception)
                {
                    log.Add(file);
                }
            }
        }

        /// <summary>
        /// Creates a compressed file based on the current addin folder version and transfers all files
        /// and folders from the addin folder to the new archive.
        /// </summary>
        private void BackupAddinFolder()
        {
            Directory.CreateDirectory(_backupPath);

            using (var lm = new LogManager())
            {
                lm.WriteInfo(this, "Creating update backup for update \'{0}\'.", _currentUpdate);
            }

            var archiveFileName = Path.Combine(_backupPath, String.Format("backup_{0}.zip", _currentUpdate));

            FileOperation(archiveFileName);

            using (var archive = ZipFile.Open(archiveFileName, ZipArchiveMode.Create))
            {
                var list = new List<string>();

                foreach (var file in Directory.EnumerateFileSystemEntries(_addinPath))
                {
                    try
                    {
                        if (File.Exists(file))
                        {
                            var ext = Path.GetExtension(file) ?? String.Empty;

                            if (ext.Equals(".dll", StringComparison.OrdinalIgnoreCase) || ext.Equals(".pdb", StringComparison.OrdinalIgnoreCase))
                            {
                                archive.CreateEntryFromFile(file, GetFileName(file), CompressionLevel.Fastest);
                            }
                        }
                        else if (Directory.Exists(file))
                        {
                            BackupFolder(archive, GetFileName(file), list);
                        }
                    }
                    catch (Exception)
                    {
                        list.Add(file);
                        //using (var lm = new LogManager())
                        //{
                        //    lm.WriteError(this, ex.ExceptionAsString());
                        //}
                    }
                }

                if (list.Count == 0) return;

                var notesFile = Path.Combine(_backupPath, AddinNames.Notes);

                try
                {
                    using (var stream = new StreamWriter(notesFile, false))
                    {
                        stream.WriteLine("Failed to add the following files:");

                        foreach (var line in list)
                        {
                            stream.WriteLine(line);
                        }
                    }

                    archive.CreateEntryFromFile(notesFile, GetFileName(notesFile), CompressionLevel.Fastest);
                }
                finally
                {
                    FileOperation(notesFile);
                }
            }
        }

        /// <summary>
        /// Restores the staging contents to the specified folder. 
        /// </summary>
        /// <param name="path">The name of the path to restore to.</param>
        /// <param name="update">The update to stamp the folder with.</param>
        private void RestoreToFolder(string path, string update)
        {
            if (String.IsNullOrEmpty(path)) throw new ArgumentNullException("path");
            if (String.IsNullOrEmpty(update)) throw new ArgumentNullException("update");

            Directory.CreateDirectory(path);

            var files = Directory.EnumerateFileSystemEntries(_stagingPath).Where(IsValidFile).ToList();

            foreach (var file in files)
            {
                if (file.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                {
                    using (var archive = ZipFile.Open(file, ZipArchiveMode.Read))
                    {
                        var baseFolder = Path.GetFileName(_addinPath) ?? String.Empty;

                        foreach (var entry in archive.Entries)
                        {
                            var relative = entry.FullName;

                            if (relative.StartsWith(baseFolder + "/", StringComparison.OrdinalIgnoreCase))
                            {
                                relative = relative.Remove(0, baseFolder.Length + 1);
                            }

                            if (relative.Contains('/') || relative.Contains('\\'))
                            {
                                relative = relative.Replace('/', '\\');
                                Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(path, relative)) ?? String.Empty);
                                if (relative.LastOrDefault() == '\\') continue;
                            }

                            entry.ExtractToFile(Path.Combine(path, relative), true);
                        }
                    }
                }
                else
                {
                    FileOperation(file, Path.Combine(path, GetFileName(file)));
                }
            }

            //WriteVersionFile(path, version);  
            WriteUpdateFile(path, update);
          
        }

        /// <summary>
        /// Determines if the update operation has been canceled.
        /// </summary>
        /// <returns>True if the update has been canceled or the instance has been disposed of.</returns>
        private bool IsCancelled()
        {
            return ((_cancel == null) || _cancel.IsCancellationRequested);
        }

        /// <summary>
        /// Performs an update check with the auto update online system. If updates are available, then they
        /// are downloaded and unpacked in version sorted order.
        /// </summary>
        /// <returns>
        /// True if an update is available, otherwise false.
        /// </returns>
        private bool DoCheckForUpdates()
        {
            bool forceNoUpdate = CheckForForceNoUpdate();
            if (forceNoUpdate)
            {
                using (var lm = new LogManager())
                {
                    lm.WriteInfo(this, "Found no updates flag for package id '{0}', stopped check.",  _packageId);
                }
                return false;
            }
            
            
            var version = _productVersion;
            var stagingUpdate = string.Empty;
            
            //refresh current version info from disk if it exists
            var currentUpdate = ReadUpdateFile(_addinPath);
            if (currentUpdate != null)
            {
                _currentUpdate = currentUpdate;
            }
            
            //do not check based on version that will always come in.
            if (_updateAvailable)
            {
                stagingUpdate = ReadUpdateFile(_stagingPath);
            }

            IAutoUpdateService auService;
            IDisposable auServiceDisposable;

            if (_downloadSource.IsAbsoluteUri &&  _downloadSource.IsFile)
            {
                //use our "local" updater to allow file based sourcing.
                var serviceClient = new LocalAutoUpdateClient();
                auService = serviceClient;
                auServiceDisposable = serviceClient;
            }
            else
            {
                //use the real Sage Auto Update client
                var serviceClient = new AutoUpdateClient();
                auService = serviceClient;
                auServiceDisposable = serviceClient;
            }

            try
            {
                auService.UpdateServiceUri = _downloadSource.ToString();
                auService.DownloadActivityTimeout = AddinTimes.DownloadActivityTimeout;
                auService.CheckTimeout = AddinTimes.CheckTimeout;
                auService.ConnectTimeout = AddinTimes.ConnectTimeout;
                auService.DownloadDir = _stagingPath;

                var applicationInstallation = new AppInstall
                {
                    ProductId = _productId,
                    VersionId = version,
                    SerialNumber = String.Empty,
                    Locale = String.Empty
                };

                using (var lm = new LogManager())
                {
                    lm.WriteInfo(this, "Doing update check on URI:'{0}' for product id:'{1}', with product version:'{2}' for package id '{3}'.",
                        auService.UpdateServiceUri,
                        _productId,
                        version,
                        _packageId);
                }


                var updates = auService.CheckForUpdatesEx(applicationInstallation);

                using (var lm = new LogManager())
                {
                    lm.WriteInfo(this, "Found {0} updates to product id '{1}' with product version '{2}'.", updates.Count(),
                        _productId,
                        version);
                }

                try
                {
                    if (updates.Length == 0) return false;
                    if (IsCancelled()) return true;

                    var update = FindApplicableUpdate(updates);
                    if (update == null)
                    {
                        DoCustomCheckAction(null, null);
                        using (var lm = new LogManager())
                        {
                            lm.WriteInfo(this, "No applicable update found for package id: '{0}'.", _packageId);
                        }
                        return false;
                    }

                    //check that we are not about to download the version that we already have in place.
                    string proposedUpdateId = GetUpdateIdentifier(update);

                    bool isSameUpdateVersion = IsSameUpdateVersion(_addinComponentBaseName, _currentUpdate, proposedUpdateId);
                    if (isSameUpdateVersion)
                    {
                        DoCustomCheckAction(null, null);
                        using (var lm = new LogManager())
                        {
                            lm.WriteInfo(this, "Latest update found for package id: '{0}' is already installed.", _packageId);
                        }
                        return false;
                    }

                    bool enforceIsGreaterVersion = AllowOnlyGreaterVersionsToUpdate;
                    if (enforceIsGreaterVersion)
                    {
                        bool installedIsHigherThenProposed = IsGreaterUpdateVersion(_addinComponentBaseName, _currentUpdate, proposedUpdateId);
                        if (installedIsHigherThenProposed)
                        {
                            DoCustomCheckAction(null, null);
                            using (var lm = new LogManager())
                            {
                                lm.WriteInfo(this, "For package id: '{0}' the update found '{1}' has a lower version number then what is already installed '{2}'.", _packageId, proposedUpdateId, _currentUpdate);
                            }
                            return false;
                        }
                    }

                    //check that we do not already have this version in staging waiting to apply.
                    //if we have version info in staging we have a pending update. So return will be true
                    //but we do not want to download it as we already have it.
                    if (!String.IsNullOrWhiteSpace(stagingUpdate))
                    {
                        isSameUpdateVersion = IsSameUpdateVersion(_addinComponentBaseName, _currentUpdate, stagingUpdate);
                        if (isSameUpdateVersion)
                        {
                            DoCustomCheckAction(null, null);
                            using (var lm = new LogManager())
                            {
                                lm.WriteInfo(this, "Latest update found for package id: '{0}' is already staged for install.", _packageId);
                            }
                            return true;
                        }
                    }

                    DoStagingDownload(auService, update, proposedUpdateId);

                    DoCustomCheckAction(update, _stagedUpdateFile);
                    using (var lm = new LogManager())
                    {
                        lm.WriteInfo(this, "Update downloaded staged, backup complete for product id: '{0}', version: '{1}', package id: '{2}', baseName: '{3}', package version:'{4}'.", _productId, version, _packageId, _addinComponentBaseName, proposedUpdateId);
                    }
                }
                finally
                {
                    _lastUpdateCheck = DateTime.Now;
                }

                return true;
            }
            catch (Exception ex)
            {
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, "Failed check for updates for product id: '{1}', version: '{2}', package id: '{3}'. Exception: {0}", ex.ExceptionAsString(), _productId, version, _packageId);
                }
            }
            finally
            {
                auServiceDisposable.Dispose();
            }
            
            return false;
        }

        private void DoCustomCheckAction(ISoftwareUpdate update, string updatePath)
        {
            if (_customCheckAction != null)
            {
                _customCheckAction(update, updatePath);
            }
        }


        private void DoStagingDownload(IAutoUpdateService auService, ISoftwareUpdate update, string proposedUpdateId)
        {
            try
            {
                //prepare to get the new update
                PurgeFolder(_stagingPath, true);

                auService.DownloadUpdate(update);

                //this will remove the .xxx.zip downloaded, leaving us with just the xxx, zip or whatever we downloaded from the end of DownloadUpdate
                string updateFileName = update.FileName;
                string fullUpdatePath = Path.Combine(_stagingPath, updateFileName);
                FileOperation(fullUpdatePath);

                
                int dotZipExtensionStringLength = 4;
                string extractedUpdate = fullUpdatePath.Remove(fullUpdatePath.Length - dotZipExtensionStringLength);
                _stagedUpdateFile = extractedUpdate;

                //WriteVersionFile(_stagingPath, update.VersionId);
                WriteUpdateFile(_stagingPath, proposedUpdateId);
                
                using (var lm = new LogManager())
                {
                    lm.WriteInfo(this, "Staging Download: Downloaded update to {0}.", fullUpdatePath);
                }

                if (!HasCustomBackup)
                    BackupAddinFolder();

                _updateAvailable = true;
            }
            catch (Exception ex)
            {

                PurgeFolder(_stagingPath);
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }
                throw;
            }
        }

        private bool CheckForForceNoUpdate()
        {
            string targetFile = Path.Combine(_addinPath, AddinNames.NoUpdateFlag);
            bool retval = File.Exists(targetFile);
            return retval;
        }

        private ISoftwareUpdate FindApplicableUpdate(ISoftwareUpdate[] updates)
        {
            var filteredList = updates.Where(item => item.UpdateId.StartsWith(_addinComponentBaseName, StringComparison.OrdinalIgnoreCase)).ToList();

            var comparer = new ISoftwareUpdateComparerTrailingUpdateId(_addinComponentBaseName);
            filteredList.Sort(comparer);
            var update = filteredList.LastOrDefault();
            
            using (var lm = new LogManager())
            {
                string updateIdChosen = "None";
                if (update != null && !string.IsNullOrWhiteSpace(update.UpdateId))
                {
                    updateIdChosen = update.UpdateId;
                }
                
                lm.WriteInfo(this, "Found {0} updates matching with component base name '{1}'. Choosing {2}.", filteredList.Count(), _addinComponentBaseName, updateIdChosen);
            }
            
            return update;
        }

        private string GetUpdateIdentifier(ISoftwareUpdate softwareUpdate)
        {
            return softwareUpdate.UpdateId;
        }

        private bool IsSameUpdateVersion(string componentBaseName, string nameA, string nameB)
        {
            var comparer = new ISoftwareUpdateComparerTrailingUpdateId(componentBaseName);
            bool retval = (comparer.Compare(nameA, nameB) == 0);
            return retval;
        }

        private bool IsGreaterUpdateVersion(string componentBaseName, string nameA, string nameB)
        {
            var comparer = new ISoftwareUpdateComparerTrailingUpdateId(componentBaseName);
            bool retval = (comparer.Compare(nameA, nameB) > 0);
            return retval;
        }

        /// <summary>
        /// Transfers the contents of the staging folder to the addin folder. When complete, the process
        /// will update the addin folders version with the version in staging.
        /// </summary>
        /// <returns>True if the staging folder was processed and cleared of all contents before returning.</returns>
        private bool DoApplyUpdate()
        {
            if (!Directory.Exists(_stagingPath)) return true;

            var stagingUpdate = ReadUpdateFile(_stagingPath);

            if (!File.Exists(Path.Combine(_stagingPath, AddinNames.Update)) || String.IsNullOrEmpty(stagingUpdate))
            {
                _updateAvailable = false;
                _stagedUpdateFile = string.Empty;
                return PurgeFolder(_stagingPath);
            }

            //if we already have a version with the same update id then skip the install.
            //if not replace with the version from the download. This maybe a greater, lower or base name change.
            //This gives us the a good recovery strategy in the field. Old algorithm used to require a newer version.
            bool isSameUpdateVersion = IsSameUpdateVersion(_addinComponentBaseName, _currentUpdate, stagingUpdate);
            if (isSameUpdateVersion)
            {
                _updateAvailable = false;
                _stagedUpdateFile = string.Empty;
                return PurgeFolder(_stagingPath);
            }

            using (var lm = new LogManager())
            {
                lm.WriteInfo(this, "Updating the add in folder {1} to update '{0}'. There have been {2} update application failures for this update", stagingUpdate, _updateFolderName, _updateFailureCount);
            }

            var rename = Path.Combine(Path.GetDirectoryName(_addinPath) ?? String.Empty, Path.GetRandomFileName());
            var staging = Path.Combine(Path.GetDirectoryName(_addinPath) ?? String.Empty, Path.GetRandomFileName());

            try
            {
                try
                {
                    //unpack staging zip to a new folder in prep for swap.
                    RestoreToFolder(staging, stagingUpdate);
                    
                    //rename/move the existing addin folder to a temp folder name.
                    Directory.Move(_addinPath, rename);

                    try
                    {
                        //we no longer have databases to copy over but leaving as marker
                        //in case we need to copy something else.
                        //CopyNonAssemblies(rename, staging);
                        Directory.Move(staging, _addinPath);
                    }
                    catch (Exception ex)
                    {
                        //clear out the add in path in preparation of putting original back.
                        Directory.Delete(_addinPath);
                        Directory.Move(rename, _addinPath);
                        using (var lm = new LogManager())
                        {
                            lm.WriteInfo(this, "Exception while moving the staging folder '{1}' to the add in folder '{2}'.{0}{3}", Environment.NewLine, staging, _addinPath, ex.ExceptionAsString());
                        }
                        throw;
                    }

                    Directory.Delete(rename, true);
                }
                catch (Exception)
                {
                    if (Directory.Exists(staging))
                    {
                        Directory.Delete(staging, true);
                    }
                    throw;
                }

                //_currentVersion = stagingVersion;
                _currentUpdate = stagingUpdate;
                _updateAvailable = false;
                
                _stagedUpdateFile = string.Empty;
                PurgeFolder(_stagingPath);
                ResetUpdateFailureCount();
            }
            catch (Exception ex)
            {
                using (var lm = new LogManager())
                {
                    lm.WriteError(this,"Apply update failed. There have been {1} update failures for this update, and {2} total update failures for this package. Exception: {0}", ex.ExceptionAsString(), _updateFailureCount, _totalUpdateFailureCount);
                }
                return false;
            }

            return true;
        }

        /// <summary>
        /// Dispose of managed and unmanaged resources.
        /// </summary>
        /// <param name="disposing">True if resources should be cleaned up.</param>
        private void Dispose(bool disposing)
        {
            if (!disposing || _disposed) return;

            if (_cancel != null)
            {
                _cancel.Cancel();
            }

            _disposed = true;
            lock (_lock)
            {
                if (_timer != null)
                {
                    _timer.Dispose();
                    _timer = null;
                }

                if (_cancel != null)
                {
                    _cancel.Dispose();
                    _cancel = null;
                }

                if (_lockUpdate != null)
                {
                    _lockUpdate.Dispose();
                    _lockUpdate = null;
                }
            }
        }

        /// <summary>
        /// Determine if a transfer from the staging folder is currently in progress.
        /// </summary>
        /// <returns>True if there are files in the staging directory, otherwise false.</returns>
        private bool StagingTransferInProgress()
        {
            //Really only lets us know that a downloaded version is in staging. It maybe stale.
            return (Directory.Exists(_stagingPath) && Directory.EnumerateFileSystemEntries(_stagingPath).Any());
        }

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <param name="downloadSource">The download source.</param>
        /// <param name="productId">The product identifier.</param>
        /// <param name="defaultVersion">The default version.</param>
        /// <param name="addinPath">The addin path.</param>
        /// <param name="addinComponentBaseName">The addin component prefix.</param>
        /// <exception cref="System.ArgumentNullException">productId
        /// or
        /// defaultVersion
        /// or
        /// addinPath</exception>
        /// <exception cref="System.IO.IOException">addinPath does not exist</exception>
        public AddinUpdater(string packageId, Uri downloadSource, string productId, string defaultVersion, string addinPath, string addinComponentBaseName)
        {
            if (String.IsNullOrEmpty(packageId)) throw new ArgumentNullException("productId");
            if (downloadSource == null) throw new ArgumentNullException("downloadSource");
            if (String.IsNullOrEmpty(productId)) throw new ArgumentNullException("productId");
            if (String.IsNullOrEmpty(defaultVersion)) throw new ArgumentNullException("defaultVersion");
            if (String.IsNullOrEmpty(addinPath)) throw new ArgumentNullException("addinPath");
            if (!Directory.Exists(addinPath)) throw new IOException("addinPath does not exist");

            //_timer = new Timer(Callback, this, AddinTimes.StartInterval, Timeout.Infinite);

            _packageId = packageId;
            _downloadSource = downloadSource;
            _productId = productId;
            _addinPath = addinPath;
            _addinComponentBaseName = addinComponentBaseName;
            _stagedUpdateFile = string.Empty;
            

            var path = Path.GetDirectoryName(_addinPath) ?? String.Empty;
            string targetName = Path.GetFileName(_addinPath);
            _updateFolderName = targetName;
            _addinNames = new AddinNames(targetName);

            _stagingPath = Path.Combine(path, _addinNames.Staging);
            _backupPath = Path.Combine(path, _addinNames.Backup);
            _updateAvailable = StagingTransferInProgress();

            _currentUpdate = ReadUpdateFile(_addinPath);
            if (String.IsNullOrEmpty(_currentUpdate))
            {
                //update this when version format changes. Note that prefix includes the dot separator
                _currentUpdate = addinComponentBaseName + "0.0";
            }

            //_currentVersion = ReadVersionFile(_addinPath);
            //if (String.IsNullOrEmpty(_currentVersion))
            //{
                _productVersion = defaultVersion;
            //}
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~AddinUpdater()
        {
            Dispose(false);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Dispose of managed and unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Determines if an update is available and ready to be applied.
        /// </summary>
        /// <returns>True if an update is available, otherwise false.</returns>
        /// <remarks>
        /// Allows for hiding the available update using a global mutex (can be controlled
        /// by an external process).
        /// </remarks>
        public bool IsUpdateAvailable()
        {
            //good choke point to disable the auto update feature if needed.
            //_updateAvailable = false;
            //return _updateAvailable;
            
            if (_disposed) return false;

            if (!_updateAvailable) 
                return false;

            if (CheckForForceNoUpdate())
                return false;

            try
            {
                if (!_lockUpdate.WaitOne(0)) 
                    return false;

                try
                {
                    return true;
                }
                finally
                {
                    _lockUpdate.ReleaseMutex();
                }
            }
            catch (Exception)
            {
                return _updateAvailable;
            }
        }

        /// <summary>
        /// Should the apply update.
        /// </summary>
        /// <returns></returns>
        public bool ShouldApplyUpdate()
        {
            bool retval = true;
            //retval = (_updateFailureCount < 5);
            return retval;
        }

        /// <summary>
        /// Applies the update that is available in the staging folder.
        /// </summary>
        /// <returns>True if the available update was applied, or no updates to apply,  false in case of application error.</returns>
        public bool ApplyUpdate()
        {
            if (_isCustomUpdate) return true;

            if (_disposed) return false;

            if (!Monitor.TryEnter(_lock)) return false;
            try
            {
                if (_disposed) return false;

                if (IsUpdateAvailable())
                {
                    return DoApplyUpdate();
                }
            }
            finally
            {
                Monitor.Exit(_lock);
            }

            return true;
        }
        
        /// <summary>
        /// Performs a forced check for updates on the staging server.
        /// </summary>
        /// <returns>True is an update has been found and downloaded to the staging folder.</returns>
        public bool CheckForUpdates()
        {
            if (_disposed) return false;

            if (!Monitor.TryEnter(_lock)) 
                return false;

            try
            {
                if (_disposed) return false;

                bool retval = DoCheckForUpdates();
                retval = (_updateAvailable || retval);
                
                return retval;
            }
            finally
            {
                Monitor.Exit(_lock);
            }
        }

        /// <summary>
        /// Determines if an update is currently in progress.
        /// </summary>
        /// <returns>True if an update is in progress, otherwise false.</returns>
        public bool UpdateInProgress()
        {
            if (_disposed) return false;

            if (!Monitor.TryEnter(_lock)) return true;

            Monitor.Exit(_lock);

            return false;
        }


        /// <summary>
        /// Disposes the when possible.
        /// </summary>
        public void DisposeWhenPossible()
        {
            if (_disposed) return;
            Dispose();
        }

        /// <summary>
        /// Removes the package staging downloads, backups, and main folder.
        /// </summary>
        /// <remarks>
        /// Make a best effort attempt to remove the plugin from disk.
        /// this is not a normal operation. Expected to only be used when removing back office plugins.
        /// remove backups
        /// remove pending
        /// remove main folder - note we did not create it, so be careful with this when refactoring.
        /// We may want restructure some as this calls into question allocation of responsibilities.
        /// </remarks>        
        public void PurgePackageSupport()
        {
            //make best attempt if were locked when this happens, we leave the package
            //otherwise take the lock, cleanup and move on.
            if (!Monitor.TryEnter(_lock)) return;

            _updateAvailable = false;


            List<String> targetPaths = new List<string>() { _backupPath, _stagingPath, _addinPath };
            foreach (string path in targetPaths)
            {
                try
                {
                    Directory.Delete(path, true);
                }
                catch (Exception)
                {
                    //swallow errors in delete. We are making a best attempt.
                }
            }
            Monitor.Exit(_lock);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Return the current version of the addin path contents.
        /// </summary>
        public string ProductVersion
        {
            get { return _productVersion; }
            set { _productVersion = value; }
        }

        /// <summary>
        /// Return the current version of the addin path contents.
        /// </summary>
        public string CurrentUpdate
        {
            get { return _currentUpdate; }
        }

        /// <summary>
        /// Returns the addin path that is being monitored by the instance.
        /// </summary>
        public string AddinPath
        {
            get { return _addinPath; }
        }

        /// <summary>
        /// Gets or sets the backup path.
        /// </summary>
        /// <value>
        /// The backup path.
        /// </value>
        public string BackupPath 
        { 
            get { return _backupPath; }
            set { _backupPath = value; }
        }

        /// <summary>
        /// Gets or sets the staging path.
        /// </summary>
        /// <value>
        /// The staging path.
        /// </value>
        public string StagingPath
        {
            get { return _stagingPath; }
            set { _stagingPath = value; }
        }

        /// <summary>
        /// Returns the product id associated with the instance.
        /// </summary>
        public string ProductId
        {
            get { return _productId; }
            set { _productId = value; }
        }

        /// <summary>
        /// Gets the package identifier.
        /// </summary>
        /// <value>
        /// The package identifier.
        /// </value>
        public string PackageId {
            get { return _packageId; }
        }

        /// <summary>
        /// Gets or sets the service URI.
        /// </summary>
        /// <value>
        /// The service URI.
        /// </value>
        public Uri ServiceUri
        {
            get { return _downloadSource; } 
            set { _downloadSource = value; }
        }

        /// <summary>
        /// Gets or sets the name of the component base.
        /// </summary>
        /// <value>
        /// The name of the component base.
        /// </value>
        public string ComponentBaseName
        {
            get { return _addinComponentBaseName; }
            set { _addinComponentBaseName = value; }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether this instance has custom backup.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has custom backup; otherwise, <c>false</c>.
        /// </value>
        public bool HasCustomBackup
        {
            get { return _isCustomBackup; }
            set { _isCustomBackup = value; }
        }


        /// <summary>
        /// Gets or sets a value indicating whether this instance has custom update.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance has custom update; otherwise, <c>false</c>.
        /// </value>
        public bool HasCustomUpdate
        {
            get { return _isCustomUpdate; }
            set { _isCustomUpdate = value; }    
        }
        
        /// <summary>
        /// Gets the update failure count, since the last successful update or start of updates.
        /// </summary>
        /// <value>
        /// The update failure count.
        /// </value>
        public int UpdateFailureCount
        {
            get { return _updateFailureCount; }
        }

        /// <summary>
        /// Gets the total update failure count for the running service.
        /// </summary>
        /// <value>
        /// The total update failure count.
        /// </value>
        public int TotalUpdateFailureCount
        {
            get { return _totalUpdateFailureCount; }
        }

        /// <summary>
        /// Increments the failure count.
        /// </summary>
        /// <returns></returns>
        public int IncrementFailureCount()
        {
            if (_updateFailureCount < int.MaxValue) _updateFailureCount++;
            if (_totalUpdateFailureCount < int.MaxValue) _totalUpdateFailureCount++;

            return _updateFailureCount;
        }

        /// <summary>
        /// Resets the update failure count.
        /// </summary>
        public void ResetUpdateFailureCount()
        {
            _updateFailureCount = 0;
        }

        private int _totalUpdateFailureCount = 0;
        /// <summary>
        /// Gets or sets the custom check action.
        /// </summary>
        /// <value>
        /// The custom check action.
        /// </value>
        public Action<ISoftwareUpdate, string> CustomCheckAction
        {
            get { return _customCheckAction; }
            set { _customCheckAction = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [allow only greater versions to update].
        /// </summary>
        /// <value>
        /// <c>true</c> if [allow only greater versions to update]; otherwise, <c>false</c>.
        /// </value>
        public bool AllowOnlyGreaterVersionsToUpdate { get; set; }

        #endregion


    }
}