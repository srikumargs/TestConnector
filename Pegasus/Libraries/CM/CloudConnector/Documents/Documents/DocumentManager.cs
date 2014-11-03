using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Sage.Connector.ConnectorServiceCommon;
using Sage.Connector.Data;
using Sage.Connector.Logging;
using Sage.Diagnostics;

namespace Sage.Connector.Documents
{
    /// <summary>
    /// Document Manager
    /// </summary>
    public class DocumentManager
    {
        /// <summary>
        /// Defines the tenant's sent directory cleanup policy
        /// </summary>
        public enum SentDocumentCleanupPolicy
        {
            /// <summary>
            /// No SentDocumentCleanupPolicy (default value automatically initialized by runtime)
            /// </summary>
            None = 0,

            /// <summary>
            /// Files are deleted when sent
            /// </summary>
            Immediate,

            /// <summary>
            /// Files are deleted after a number of days
            /// </summary>
            DaysOld,

            /// <summary>
            /// Files are deleted if the directory exceeds a size limit
            /// </summary>
            DirectoryMegabyteSize,

            /// <summary>
            /// Files are never automatically deleted
            /// </summary>
            Never
        }

        #region Policy Defaults
        /// <summary>
        /// The default storage days for sent files
        /// </summary>
        public static int DefaultDayStorage = 30;

        /// <summary>
        /// The default tenant directory size for sent files
        /// </summary>
        public static int DefaultDirectoryMegabyteSize = 1024;

        /// <summary>
        /// The default sent file cleanup policy
        /// </summary>
        public static SentDocumentCleanupPolicy DefaultCleanupPolicy = SentDocumentCleanupPolicy.Immediate;

        /// <summary>
        /// The default name of the sent document folder
        /// </summary>
        public static String DefaultSentFileDirectoryName = "Sent";

        /// <summary>
        /// The default name of the content folder
        /// </summary>
        public static String DefaultContentFileDirectoryName = "Content";

        /// <summary>
        /// The default response blobs directory name
        /// </summary>
        public static String DefaultResponseBlobsDirectoryName = "ResponseBlobs";

        /// <summary>
        /// The named mutext to control access to a named tenant folder
        /// </summary>
        private static string NamedTenantFolderMutexName(string tenantId)
        {
            return "Sage.Connector.Documents." + tenantId;
        }

        /// <summary>
        /// The timeout to wait on the mutex
        /// </summary>
        private static int _waitTimeout = 60000;
        #endregion

        #region Private Folder Resolution Helpers

        /// <summary>
        /// If a SYSTEM environment variable SAGE_CONNECTOR_DOCUMENT_MANAGER_BASEPATH is set, then use that path
        /// instead of the system service's default path.  Due to our testing environment, it nest
        /// the entire runtime tree deep in our project structure, resulting in unrealistic long paths.
        /// </summary>
        private static string DocumentManagerBasePath()
        {
            String environmentBasePath =
                Environment.GetEnvironmentVariable("SAGE_CONNECTOR_DOCUMENT_MANAGER_BASEPATH", EnvironmentVariableTarget.Machine);
            string documentBasePath = (string.IsNullOrEmpty(environmentBasePath) ? Utils.DocumentManagerPath : environmentBasePath);
            return documentBasePath;

        }

        /// <summary>
        /// Constructs a 'tenant' folder by appending the tenant identifier
        /// to the base documents folder.
        /// 
        /// IOException - path is file / unknown network name
        /// UnauthorizedAccessException - permission
        /// ArgumentException - blank, invalid path chars
        /// ArgumentNullException - path blank
        /// PathTooLongException - 248 path 260 file name
        /// DirectoryNotFound - invalid /unmapped drive
        /// NotSupportedException
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        private static string TenantFolder(string tenantId)
        {
            string documentManagerBasePath = DocumentManagerBasePath();
            string tenantFolder = Path.Combine(documentManagerBasePath, tenantId);

            if (!Directory.Exists(tenantFolder))
            {
                Directory.CreateDirectory(tenantFolder);
            }
            return tenantFolder;
        }

        /// <summary>
        /// Constructs a 'sent' fold in the tenant folder
        /// 
        /// IOException - path is file / unknown network name
        /// UnauthorizedAccessException - permission
        /// ArgumentException - blank, invalid path chars
        /// ArgumentNullException - path blank
        /// PathTooLongException - 248 path 260 file name
        /// DirectoryNotFound - invalid /unmapped drive
        /// NotSupportedException
        ///
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        private static string TenantSentFolder(string tenantId)
        {
            // TODO: Utilize Configuration Sent Folder Naming? Perhaps even full Naming?
            string tenantFolder = TenantFolder(tenantId);
            string tenantSentFolder = Path.Combine(tenantFolder, DefaultSentFileDirectoryName);
            if (!Directory.Exists(tenantSentFolder))
            {
                Directory.CreateDirectory(tenantSentFolder);
            }
            return tenantSentFolder;
        }

        /// <summary>
        /// Constructs a 'content' folder
        /// 
        /// IOException - path is file / unknown network name
        /// UnauthorizedAccessException - permission
        /// ArgumentException - blank, invalid path chars
        /// ArgumentNullException - path blank
        /// PathTooLongException - 248 path 260 file name
        /// DirectoryNotFound - invalid /unmapped drive
        /// NotSupportedException
        ///
        /// </summary>
        /// <returns></returns>
        private static string ContentFolder()
        {
            string contentFolder = TenantFolder(DefaultContentFileDirectoryName);
            if (!Directory.Exists(contentFolder))
            {
                Directory.CreateDirectory(contentFolder);
            }
            return contentFolder;
        }

        /// <summary>
        /// Retrieves the tenant 'sent' folder cleanup policy
        /// </summary>
        /// <param name="premiseConfiguration"></param>
        /// <returns></returns>
        private static SentDocumentCleanupPolicy TenantSentDocumentCleanupPolicy(PremiseConfigurationRecord premiseConfiguration)
        {
            if (null == premiseConfiguration)
                return DefaultCleanupPolicy;

            return (SentDocumentCleanupPolicy)premiseConfiguration.SentDocumentStoragePolicy;
        }

        #endregion

        #region Private File and Folder Helpers

        /// <summary>
        /// Calculates directory size in megabytes
        /// 
        /// IOException - path is file / unknown network name
        /// UnauthorizedAccessException - permission
        /// ArgumentException - blank, invalid path chars
        /// ArgumentNullException - path blank
        /// PathTooLongException - 248 path 260 file name
        /// DirectoryNotFound - invalid /unmapped drive
        /// NotSupportedException
        /// 
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        private static long DirectoryFileSize(string directory)
        {
            string[] directoryFiles = Directory.GetFiles(directory);
            long directoryFileSizeInBytes = directoryFiles.Sum(fileName => new FileInfo(fileName).Length);
            return directoryFileSizeInBytes / 1024 / 1024;
        }


        /// <summary>
        /// Cached collection of directory files ordered by date (oldest first)
        /// 
        /// IOException - path is file / unknown network name
        /// UnauthorizedAccessException - permission
        /// ArgumentException - blank, invalid path chars
        /// ArgumentNullException - path blank
        /// PathTooLongException - 248 path 260 file name
        /// DirectoryNotFound - invalid /unmapped drive
        /// NotSupportedException
        /// 
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        private static SortedList<DateTime, List<FileInfo>> CachedDirectoryFiles(string directory)
        {
            // TODO: Linq to object to sort the directory files by creation dates
            string[] directoryFiles = Directory.GetFiles(directory);
            SortedList<DateTime, List<FileInfo>> fileInfos = new SortedList<DateTime, List<FileInfo>>();
            foreach (string filename in directoryFiles)
            {              
                FileInfo info = new FileInfo(filename);
                List<FileInfo> list = null;
                if (!fileInfos.ContainsKey(info.CreationTimeUtc))
                {
                    list = fileInfos[info.CreationTimeUtc] = new List<FileInfo>();
                }
                else
                {
                    list = fileInfos[info.CreationTimeUtc];
                }
                list.Add(info);
            }

            return fileInfos;
        }

        /// <summary>
        /// Moves the specified file to the tenant's sent folder
        /// 
        /// IOException - path is file / unknown network name
        /// UnauthorizedAccessException - permission
        /// ArgumentException - blank, invalid path chars
        /// ArgumentNullException - path blank
        /// PathTooLongException - 248 path 260 file name
        /// DirectoryNotFound - invalid /unmapped drive
        /// NotSupportedException
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="filePath"></param>
        private static void MoveFileToSent(string tenantId, string filePath)
        {
            string sTenantSentFolderMutexName = NamedTenantFolderMutexName(tenantId);

            Boolean createdNew = false;
            using (var mutex = new Mutex(false, sTenantSentFolderMutexName, out createdNew, Utils.AllowEveryoneMutexSecurity))
            {
                try
                {
                    if (mutex.WaitOne(_waitTimeout))
                    {
                        FileInfo fileInfo = new FileInfo(filePath);
                        string sTenantSentFolder = TenantSentFolder(tenantId);
                        string sentFilePath = Path.Combine(sTenantSentFolder, fileInfo.Name);
                        File.Move(filePath, sentFilePath);
                    }
                }
                finally
                {
                    try
                    {
                        mutex.ReleaseMutex();
                    }
                    catch (ApplicationException)
                    {
                        // Swallowing 'not owned' mutex exception
                    }
                }
            }
        }

        /// <summary>
        /// Deletes the specified file
        /// </summary>
        /// <param name="filePath"></param>
        public static void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        /// <summary>
        /// Delete multiple files
        /// </summary>
        /// <param name="filePaths"></param>
        public static void DeleteFiles(IEnumerable<string> filePaths)
        {
            filePaths.ToList().ForEach(filePath => DeleteFile(filePath));
        }

        /// <summary>
        /// Deletes the entire directory
        /// (overrides read-only flags, deletes files in sub-directories)
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns></returns>
        private static void DeleteDirectory(string directoryPath)
        {
            if (Directory.Exists(directoryPath))
            {
                string[] files = Directory.GetFiles(directoryPath);
                foreach (string file in files)
                {
                    File.SetAttributes(file, FileAttributes.Normal);
                    File.Delete(file);
                }

                string[] directories = Directory.GetDirectories(directoryPath);
                foreach (string directory in directories)
                {
                    DeleteDirectory(directory);
                }

                Directory.Delete(directoryPath, false);
            }
        }

        /// <summary>
        /// Centralized exception handling
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="ex"></param>
        /// <param name="logManager"></param>
        private static void FileExceptionHandler(Object caller, Exception ex, LogManager logManager)
        {
            UnauthorizedAccessException uae = ex as UnauthorizedAccessException;
            ArgumentNullException ane = ex as ArgumentNullException;
            PathTooLongException ptle = ex as PathTooLongException;
            DirectoryNotFoundException dnfe = ex as DirectoryNotFoundException;
            IOException ioe = ex as IOException;
            ArgumentException ae = ex as ArgumentException;
            NotSupportedException nse = ex as NotSupportedException;

            if ((null != uae) && (null != logManager))
            {
                logManager.WriteError(caller, "Permission problem (Unauthorized Access): " + uae.Message);
            }
            else if ((null != ane) && (null != logManager))
            {
                logManager.WriteError(caller, "Path Problem (Argument Null): " + ane.Message);
            }
            else if ((null != ptle) && (null != logManager))
            {
                logManager.WriteError(caller, "Path Problem (Path Too Long): " + ptle.Message);
            }
            else if ((null != dnfe) && (null != logManager))
            {
                logManager.WriteError(caller, "Path Problem (Directory Not Found): " + dnfe.Message);
            }
            else if ((null != ioe) && (null != logManager))
            {
                logManager.WriteError(caller, "Path Problem (IO): " + ioe.Message);
            }
            else if ((null != ae) && (null != logManager))
            {
                logManager.WriteError(caller, "Path Problem (Argument): " + ae.Message);
            }
            else if ((null != nse) && (null != logManager))
            {
                logManager.WriteError(caller, "General Error (Not Supported): " + nse.Message);
            }
            else if ((null != ex) && (null != logManager))
            {
                logManager.WriteError(caller, "General Error (Uncategorized): " + ex.Message);
            }
        }

        #endregion

        #region Private Policy Deletions

        /// <summary>
        /// Iterates sorted list of files, deleting files older than specified days
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="daysOld"></param>
        private static void DeleteOldFilesByAge(string tenantId, int daysOld)
        {
            string sTenantSentFolderMutexName = NamedTenantFolderMutexName(tenantId);

            Boolean createdNew = false;
            using (var mutex = new Mutex(false, sTenantSentFolderMutexName, out createdNew, Utils.AllowEveryoneMutexSecurity))
            {
                try
                {
                    if (mutex.WaitOne(_waitTimeout))
                    {
                        DateTime cutoffDate = DateTime.UtcNow.AddDays(-daysOld);
                        string sTenantSentFolder = TenantSentFolder(tenantId);
                        SortedList<DateTime, List<FileInfo>> fileList = CachedDirectoryFiles(sTenantSentFolder);
                        IEnumerator<KeyValuePair<DateTime, List<FileInfo>>> enumerator = fileList.GetEnumerator();
                        if (enumerator != null)
                        {
                            while (enumerator.MoveNext())
                            {
                                if (enumerator.Current.Key > cutoffDate)
                                {
                                    break;
                                }
                                foreach (FileInfo fileInfo in enumerator.Current.Value)
                                {
                                    DeleteFile(fileInfo.FullName);
                                }
                            }
                        }
                    }
                }
                finally
                {
                    try
                    {
                        mutex.ReleaseMutex();
                    }
                    catch (ApplicationException)
                    {
                        // Swallowing 'not owned' mutex exception
                    }
                }
            }
        }

        /// <summary>
        /// Iteratoes sorted list of files, deleting files until currentCapcity is less than the limit
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="capacityLimit"></param>
        private static void DeleteOldFilesForCapacity(string tenantId, long capacityLimit)
        {
            string sTenantSentFolderMutexName = NamedTenantFolderMutexName(tenantId);

            Boolean createdNew = false;
            using (var mutex = new Mutex(false, sTenantSentFolderMutexName, out createdNew, Utils.AllowEveryoneMutexSecurity))
            {
                try
                {
                    if (mutex.WaitOne(_waitTimeout))
                    {
                        string sTenantSentFolder = TenantSentFolder(tenantId);

                        long currentCapacity = DirectoryFileSize(sTenantSentFolder);
                        if (currentCapacity <= capacityLimit)
                        {
                            return;
                        }
                        long mbToDelete = currentCapacity - capacityLimit;

                        SortedList<DateTime, List<FileInfo>> fileList = CachedDirectoryFiles(sTenantSentFolder);
                        IEnumerator<KeyValuePair<DateTime, List<FileInfo>>> enumerator = fileList.GetEnumerator();
                        if (enumerator != null)
                        {
                            while (enumerator.MoveNext())
                            {
                                foreach (FileInfo fileInfo in enumerator.Current.Value)
                                {
                                    DeleteFile(fileInfo.FullName);
                                    mbToDelete -= fileInfo.Length / 1024 / 1024;
                                    if (mbToDelete <= 0)
                                    {
                                        break;
                                    }
                                }
                                if (mbToDelete <= 0)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                finally
                {
                    try
                    {
                        mutex.ReleaseMutex();
                    }
                    catch (ApplicationException)
                    {
                        // Swallowing 'not owned' mutex exception
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Returns a full path to store the output of a request file
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="requestId"></param>
        /// <param name="fileType"></param>
        /// <returns></returns>
        public string GetFilePathLocation(string tenantId, Guid requestId, string fileType)
        {
            ArgumentValidator.ValidateNonEmptyString(tenantId, "tenantId", _myTypeName + ".GetFilePathLocation()");
            ArgumentValidator.ValidateNonEmptyString(fileType, "fileType", _myTypeName + ".GetFilePathLocation()");

            // TODO: Consider potential value up of creating separate folders for each 'request'
            string retFilePath = string.Empty;
            string sTenantSentFolderMutexName = NamedTenantFolderMutexName(tenantId);

            Boolean createdNew = false;
            using (var mutex = new Mutex(false, sTenantSentFolderMutexName, out createdNew, Utils.AllowEveryoneMutexSecurity))
            {
                try
                {
                    if (mutex.WaitOne(_waitTimeout))
                    {
                        string tenantFolder = TenantFolder(tenantId);
                        retFilePath = Path.Combine(tenantFolder, requestId + "." + fileType);
                    }
                }
                finally
                {
                    try
                    {
                        mutex.ReleaseMutex();
                    }
                    catch (ApplicationException)
                    {
                        // Swallowing 'not owned' mutex exception
                    }
                }
            }

            // TODO: Verify that the location is available?
            //if (File.Exists(retFilePath))
            //{
            //    throw new IOException();
            //}

            return retFilePath;
        }

        /// <summary>
        /// Notifies document manager that a file has been deleted
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="logManager"></param>
        /// <param name="premiseConfiguration"></param>
        /// <param name="filePath"></param>
        public void FileSent(string tenantId, LogManager logManager, PremiseConfigurationRecord premiseConfiguration, string filePath)
        {
            ArgumentValidator.ValidateNonEmptyString(tenantId, "tenantId", _myTypeName + ".FileSent()");
            try
            {
                switch (TenantSentDocumentCleanupPolicy(premiseConfiguration))
                {
                    case SentDocumentCleanupPolicy.None:
                        // Take no action
                        break;
                    case SentDocumentCleanupPolicy.Immediate:
                        DeleteFile(filePath);
                        break;
                    case SentDocumentCleanupPolicy.Never:
                    case SentDocumentCleanupPolicy.DaysOld:
                    case SentDocumentCleanupPolicy.DirectoryMegabyteSize:
                        MoveFileToSent(tenantId, filePath);
                        break;
                }
                EnforceSentCleanupPolicy(tenantId, logManager, premiseConfiguration);
            }
            catch (Exception ex)
            {
                FileExceptionHandler(this, ex, logManager);
            }
        }

        /// <summary>
        /// Enforces the tenant cleanup policy for the current tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="logManager"></param>
        /// <param name="premiseConfiguration"></param>
        public void EnforceSentCleanupPolicy(
            String tenantId,
            LogManager logManager,
            PremiseConfigurationRecord premiseConfiguration)
        {
            ArgumentValidator.ValidateNonEmptyString(tenantId, "tenantId", _myTypeName + ".EnforceSentCleanupPolicy()");
            try
            {
                switch (TenantSentDocumentCleanupPolicy(premiseConfiguration))
                {
                    case SentDocumentCleanupPolicy.None:
                    case SentDocumentCleanupPolicy.Immediate:
                    case SentDocumentCleanupPolicy.Never:
                        // Take no action
                        break;
                    case SentDocumentCleanupPolicy.DaysOld:
                        DeleteOldFilesByAge(
                            tenantId,
                            premiseConfiguration != null ?
                                premiseConfiguration.SentDocumentStorageDays :
                                DefaultDayStorage);
                        break;
                    case SentDocumentCleanupPolicy.DirectoryMegabyteSize:
                        DeleteOldFilesForCapacity(
                            tenantId,
                            premiseConfiguration != null ?
                                premiseConfiguration.SentDocumentStorageMBs :
                                DefaultDirectoryMegabyteSize);
                        break;
                }
            }
            catch (Exception ex)
            {
                FileExceptionHandler(this, ex, logManager);
            }
        }

        /// <summary>
        /// Deletes all files relating to a tenant
        /// </summary>
        /// <param name="tenantId"></param>
        public void DeleteTenant(string tenantId)
        {
            string sTenantSentFolderMutexName = NamedTenantFolderMutexName(tenantId);

            Boolean createdNew = false;
            using (var mutex = new Mutex(false, sTenantSentFolderMutexName, out createdNew, Utils.AllowEveryoneMutexSecurity))
            {
                try
                {
                    if (mutex.WaitOne(_waitTimeout))
                    {
                        string sTenantFolder = TenantFolder(tenantId);
                        DeleteDirectory(sTenantFolder);
                    }
                }
                finally
                {
                    try
                    {
                        mutex.ReleaseMutex();
                    }
                    catch (ApplicationException)
                    {
                        // Swallowing 'not owned' mutex exception
                    }
                }
            }
        }

        private static string ContentPath(string uniqueIdentifier)
        {
            return Path.Combine(ContentFolder(), uniqueIdentifier);
        }

        /// <summary>
        /// Store tenant content
        /// </summary>
        /// <param name="uniqueIdentifier"></param>
        /// <param name="content"></param>
        public static void StoreContent(string uniqueIdentifier, string content)
        {
            DeleteContent(uniqueIdentifier);

            var contentPath = ContentPath(uniqueIdentifier);

            using (StreamWriter outfile = new StreamWriter(contentPath))
            {
                outfile.Write(content);
            }
        }

        /// <summary>
        /// Retrieve stored tenant content
        /// </summary>
        /// <param name="uniqueIdentifier"></param>
        /// <returns></returns>
        public static string RetrieveContent(string uniqueIdentifier)
        {
            var contentPath = ContentPath(uniqueIdentifier);

            if (File.Exists(contentPath))
            {
                using (StreamReader sr = new StreamReader(contentPath))
                {
                    return sr.ReadToEnd();
                }
            }

            return String.Empty;
        }

        /// <summary>
        /// Delete stored tenant content
        /// </summary>
        /// <param name="uniqueIdentifier"></param>
        public static void DeleteContent(string uniqueIdentifier)
        {
            var contentPath = ContentPath(uniqueIdentifier);

            if (File.Exists(contentPath))
            {
                File.Delete(contentPath);
            }
        }
        
        /// <summary>
        /// Tenants the data storage folder.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <returns></returns>
        /// <remarks>
        /// Results intended to be passed to the DM layer and beyond.
        /// This allows some integration with clean up etc.
        /// </remarks>
        public static string GetTenantDataStorageFolder(string tenantId)
        {
            return TenantFolder(tenantId);
        }

        /// <summary>
        /// Gets the response BLOB path.
        /// </summary>
        /// <param name="tenantId">The tenant identifier.</param>
        /// <returns>
        /// Response goes in tenant data folder if a tenantId is supplied.
        /// </returns>
        public static string GetResponseBlobPath(string tenantId)
        {
            string folder;
            if (String.IsNullOrWhiteSpace(tenantId))
            {
                folder = Path.Combine(DocumentManagerBasePath(), DefaultResponseBlobsDirectoryName);    
            }
            else
            {
                folder = Path.Combine(TenantFolder(tenantId), DefaultResponseBlobsDirectoryName);    
            }
            
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            return folder;
        }

        #endregion

        /// <summary>
        /// Before changing configuration, allow unit test to invoke various policies
        /// This method will be deprecated when configuration is added and injection
        /// of configuration information is supported.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="filePath"></param>
        /// <param name="policy"></param>
        /// <param name="daysToStore"></param>
        /// <param name="mbToStore"></param>
        public static void FileSentForUnitTesting(string tenantId, string filePath, SentDocumentCleanupPolicy policy, int daysToStore, long mbToStore)
        {
            switch (policy)
            {
                case SentDocumentCleanupPolicy.None:
                    // Take no action
                    break;
                case SentDocumentCleanupPolicy.Immediate:
                    DeleteFile(filePath);
                    break;
                case SentDocumentCleanupPolicy.Never:
                    MoveFileToSent(tenantId, filePath);
                    break;
                case SentDocumentCleanupPolicy.DaysOld:
                    MoveFileToSent(tenantId, filePath);
                    DeleteOldFilesByAge(tenantId, daysToStore);
                    break;
                case SentDocumentCleanupPolicy.DirectoryMegabyteSize:
                    MoveFileToSent(tenantId, filePath);
                    DeleteOldFilesForCapacity(tenantId, mbToStore);
                    break;
            }
        }

        /// <summary>
        /// The path to the tenant sent folder (for unit testing)
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public static string SentFolderForUnitTesting(string tenantId)
        {
            return TenantSentFolder(tenantId);
        }

        private static readonly String _myTypeName = typeof(DocumentManager).FullName;
    }
}
