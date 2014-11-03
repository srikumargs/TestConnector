using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Asn1.X509;
using Sage.Ssdp.Security.Client;

namespace Sage.Connector.AutoUpdate.Addin
{
    /// <summary>
    /// Local test proxy for the auto update service. Currently only implements the methods used by the connector
    /// download URI must currently be a file URI for this to work
    /// </summary>
    class LocalAutoUpdateClient : IAutoUpdateService, IDisposable
    {
        /// <summary>
        /// Checks for updates.
        /// </summary>
        /// <param name="productId">The product identifier.</param>
        /// <param name="versionId">The version identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public ISoftwareUpdate[] CheckForUpdates(string productId, string versionId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks for updates ex.
        /// </summary>
        /// <param name="applicationInstall">The application install.</param>
        /// <returns></returns>
        public ISoftwareUpdate[] CheckForUpdatesEx(IApplicationInstall applicationInstall)
        {
            List<ISoftwareUpdate> updates = new List<ISoftwareUpdate>();

            Uri serviceLocation = new Uri(UpdateServiceUri);
            string basePath = serviceLocation.LocalPath;

            if (Directory.Exists(basePath))
            {
                //file URI exists
                string targetPath = Path.Combine(basePath, applicationInstall.ProductId, applicationInstall.VersionId);
                if (Directory.Exists(targetPath))
                {
                    //productId and Version ID exist as directories under file URI.
                    List<string> validFilters = new List<string>(){"*.zip","*.exe","*.msi"};

                    foreach (string filePatternFilter in validFilters)
                    {
                        foreach (var file in Directory.EnumerateFiles(targetPath, filePatternFilter))
                        {
                            var update = new LocalSoftwareUpdate()
                            {
                                //AU upload and download results in a .zip.zip name.
                                //emulate that as the code expects it
                                FileName = Path.GetFileName(file + ".zip"),
                                UpdateId = Path.GetFileNameWithoutExtension(file),
                                VersionId = applicationInstall.VersionId,
                                ProductId = applicationInstall.ProductId,
                                Description = file
                            };
                            updates.Add(update);
                        }
                    }
                }
            }
            ISoftwareUpdate[] softwareUpdates = updates.ToArray();
            return softwareUpdates;

        }

        private string RemovePostFix(string baseString, string postfix)
        {
            int targetLen = baseString.Length - postfix.Length;
            string retval = baseString.Remove(targetLen);
            return retval;
        }

        /// <summary>
        /// Downloads the update.
        /// </summary>
        /// <param name="update">The update.</param>
        /// <returns></returns>
        public int DownloadUpdate(ISoftwareUpdate update)
        {
            string targetPath = DownloadDir;
            if (Directory.Exists(targetPath))
            {
                //copy the file and rename it to a zip zip. This is not really a zip.zip
                //but our clean up code will just turn around and delete it. Thus this should not be an issue
                string targetFile = Path.Combine(targetPath, update.FileName);
                File.Copy(update.Description, targetFile);

                //now copy the zip over for real. Get the target name by removing extra .zip we added 
                targetFile = Path.Combine(targetPath, RemovePostFix(update.FileName, ".zip"));
                File.Copy(update.Description, targetFile);
            }
            return 1;
        }

        ///// <summary>
        ///// Extracts the zip file.
        ///// </summary>
        ///// <param name="update">The update.</param>
        ///// <returns></returns>
        ///// <exception cref="System.NotImplementedException"></exception>
        ///// <remarks>
        ///// Was used when we were working with real .zip.zip files.
        ///// This approach had higher fidelity to actual auto update downloads but
        ///// this is less important then having a good experience for the folks working with
        ///// this system. The extra time of making .zip.zips would add up.
        ///// So we moved to straight zips in the folder and faking the zip zips.
        ///// Left in case we need to move back to a higher fidelity version.
        ///// </remarks>
        //private void ExtractZipFile(string file)
        //{
        //    string target = file;
        //    string targetFolder = Path.GetDirectoryName(file);


        //    if (file.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
        //    {
        //        using (var archive = ZipFile.Open(file, ZipArchiveMode.Read))
        //        {
        //            //var baseFolder = Path.GetFileName(_addinPath) ?? String.Empty;

        //            foreach (var entry in archive.Entries)
        //            {
        //                var relative = entry.FullName;

        //                //if (relative.StartsWith(baseFolder + "/", StringComparison.OrdinalIgnoreCase))
        //                //{
        //                //    relative = relative.Remove(0, baseFolder.Length + 1);
        //                //}

        //                if (relative.Contains('/') || relative.Contains('\\'))
        //                {
        //                    relative = relative.Replace('/', '\\');
        //                    Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(targetFolder, relative)) ?? String.Empty);
        //                    if (relative.LastOrDefault() == '\\') continue;
        //                }

        //                entry.ExtractToFile(Path.Combine(targetFolder, relative), true);
        //            }
        //        }
        //    }
    
        //}

        /// <summary>
        /// Downloads the update asynchronous.
        /// </summary>
        /// <param name="update">The update.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public int DownloadUpdateAsync(ISoftwareUpdate update)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Cancels the download.
        /// </summary>
        /// <param name="requestId">The request identifier.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void CancelDownload(int requestId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Downloads the file URI.
        /// </summary>
        /// <param name="update">The update.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public string DownloadFileURI(ISoftwareUpdate update)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Verifies the and unpack.
        /// </summary>
        /// <param name="update">The update.</param>
        /// <param name="fullDownloadPath">The full download path.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public string VerifyAndUnpack(ISoftwareUpdate update, string fullDownloadPath)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks for updates with feeds.
        /// </summary>
        /// <param name="productId">The product identifier.</param>
        /// <param name="versionId">The version identifier.</param>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public ICheckForUpdateResponse CheckForUpdatesWithFeeds(string productId, string versionId, DateTime date)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks for updates ex with feeds.
        /// </summary>
        /// <param name="applicationInstall">The application install.</param>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public ICheckForUpdateResponse CheckForUpdatesExWithFeeds(IApplicationInstall applicationInstall, DateTime date)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets or sets a value indicating whether [use proxy].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use proxy]; otherwise, <c>false</c>.
        /// </value>
        public bool UseProxy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use default proxy].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [use default proxy]; otherwise, <c>false</c>.
        /// </value>
        public bool UseDefaultProxy { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [use default proxy credentials].
        /// </summary>
        /// <value>
        /// <c>true</c> if [use default proxy credentials]; otherwise, <c>false</c>.
        /// </value>
        public bool UseDefaultProxyCredentials { get; set; }

        /// <summary>
        /// Gets or sets the proxy address.
        /// </summary>
        /// <value>
        /// The proxy address.
        /// </value>
        public string ProxyAddress { get; set; }

        /// <summary>
        /// Gets or sets the proxy credential user name.
        /// </summary>
        /// <value>
        /// The proxy credential user name.
        /// </value>
        public string ProxyCredentialUsername { get; set; }

        /// <summary>
        /// Sets the proxy credential password.
        /// </summary>
        /// <value>
        /// The proxy credential password.
        /// </value>
        public string ProxyCredentialPassword { set; private get; }

        /// <summary>
        /// Gets or sets the proxy credential domain.
        /// </summary>
        /// <value>
        /// The proxy credential domain.
        /// </value>
        public string ProxyCredentialDomain { get; set; }

        /// <summary>
        /// Gets or sets the update service URI.
        /// </summary>
        /// <value>
        /// The update service URI.
        /// </value>
        public string UpdateServiceUri { get; set; }

        /// <summary>
        /// Gets or sets the check timeout.
        /// </summary>
        /// <value>
        /// The check timeout.
        /// </value>
        public int CheckTimeout { get; set; }

        /// <summary>
        /// Gets or sets the connect timeout.
        /// </summary>
        /// <value>
        /// The connect timeout.
        /// </value>
        public int ConnectTimeout { get; set; }

        /// <summary>
        /// Gets or sets the download activity timeout.
        /// </summary>
        /// <value>
        /// The download activity timeout.
        /// </value>
        public int DownloadActivityTimeout { get; set; }

        /// <summary>
        /// Gets or sets the download dir.
        /// </summary>
        /// <value>
        /// The download dir.
        /// </value>
        public string DownloadDir { get; set; }



        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
