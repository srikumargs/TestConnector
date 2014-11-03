using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;


using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using Sage.Connector.AutoUpdate.Addin;
using Sage.Connector.Common;
using Sage.Connector.Logging;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.Connector.StateService.Proxy;
using Sage.Ssdp.Security.Client;
using Sage.Utilities.Binders;

namespace Sage.Connector.AutoUpdate
{
    /// <summary>
    /// 
    /// </summary>
    public class AutoUpdateManager
    {
        /// <summary>
        /// The process execution product name
        /// </summary>
        public static string ProcessExecutionProductId = "Sage.US.NA.SageDataCloud.Execution.Plugin";

        /// <summary>
        /// The process execution partial path
        /// </summary>
        public static string ProcessExecutionPartialPath = @"Pipeline\Addins\ProcessExecution";

        /// <summary>
        /// The process execution component base name
        /// </summary>
        public static string ProcessExecutionComponentBaseName = "ProcessExecution.";

        private static string _processExecutionPackageId = "ProcessExecution";

        /// <summary>
        /// The discovery product name
        /// </summary>
        public static string DiscoveryProductId = "Sage.US.NA.SageDataCloud.Discovery.Plugin";

        private static string _discoveryPluginsPackageid = "DiscoveryPlugins";

        /// <summary>
        /// The discovery partial path
        /// </summary>
        public static string DiscoveryPartialPath = @"Pipeline\Addins\DiscoveryPlugins";

        //NOTE: the folder name and path is knowledge that is shared with the abstract domain mediator.
        //To mitigate auto update issues this is not drawn from a common dll. 
        //This may want to be revisited at some point and passed thru process execution.

        /// <summary>
        /// The discovery component base name
        /// </summary>
        public static string DiscoveryComponetBaseName = "DiscoveryPlugins.";

        /// <summary>
        /// The back office plugins partial path
        /// </summary>
        public static string BackOfficePluginsPartialPath = @"Pipeline\Addins\BackOfficePlugins";


        //NOTE: the folder name and path is knowledge that is shared with the abstract domain mediator.
        //To mitigate auto update issues this is not drawn from a common dll. 
        //This may want to be revisited at some point and passed thru process execution.


        /// <summary>
        /// The connector core product identifier
        /// </summary>
        public static string ConnectorCoreProductId = "Sage.US.NA.SageDataCloud.Execution.Plugin";

        /// <summary>
        /// The _connector core package identifier
        /// </summary>
        private static string _connectorCorePackageId = "ConnectorCore";

        /// <summary>
        /// The connector core component base name
        /// </summary>
        public static string ConnectorCoreComponentBaseName = "ConnectorCore.";

        //public static string ConnectorCorePartialPath = @"..\Sage Connector";

        private static int _standardRetryCount = 3;

        /// <summary>
        /// Downloads the back office plugin.
        /// </summary>
        /// <param name="backOfficeId">The backOffice identifier.</param>
        /// <param name="autoUpdateUri">The automatic update URI.</param>
        /// <param name="autoUpdateProductId">The automatic update product identifier.</param>
        /// <param name="autoUpdateProductVersion">The automatic update product version.</param>
        /// <param name="autoUpdateComponentBaseName">The addin component prefix.</param>
        /// <returns></returns>
        public static bool DownloadBackOfficePlugin(string backOfficeId, Uri autoUpdateUri, string autoUpdateProductId,
            string autoUpdateProductVersion, string autoUpdateComponentBaseName)
        {
            //Need to watch out for the already installed case. However usage prevents this from being an issue.
            //If the plugin is already present then it could be in use. Which means we need to move to the delayed update for it.

            string basePath = ProductRootPath();
            string addinPath = CreateBackOfficePluginPath(backOfficeId, basePath);
            string packageId = GetBackOfficePluginPackageId(backOfficeId);

            //create and dispose updater as we only need to get the download to happen.
            //note: this pattern seems a bit off and suggests the concerns are should be better separated.
            var updater = CreateUpdaterAndApply(packageId, autoUpdateUri, autoUpdateProductId, autoUpdateProductVersion, autoUpdateComponentBaseName, addinPath);
            
            //free up the updater, a persistent one will be created next update check.
            if (updater != null)
            {
                updater.Dispose();
            }
            
            return true;
        }

        private static string CreateBackOfficePluginPath(string backOfficeId, string basePath)
        {
            if (string.IsNullOrEmpty(backOfficeId)) throw new ArgumentNullException("backOfficeId");
            if (string.IsNullOrEmpty(basePath)) throw new ArgumentNullException("basePath");

            //create path to base of all back office plugins if it does not exist
            string basePluginsFolder = Path.Combine(basePath, BackOfficePluginsPartialPath);
            if (!Directory.Exists(basePluginsFolder))
                Directory.CreateDirectory(basePluginsFolder);

            //now create the specific folder for requested back office plugin
            string addinPath = Path.Combine(basePluginsFolder, backOfficeId);
            if (!Directory.Exists(addinPath))
                Directory.CreateDirectory(addinPath);

            return addinPath;
        }

        /// <summary>
        /// Adds the updater and check.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <param name="downloadSource">The download source.</param>
        /// <param name="productId">The product identifier.</param>
        /// <param name="autoUpdateVersion">The default version.</param>
        /// <param name="autoUpdateComponentBaseName">The addin component prefix.</param>
        /// <param name="addinPath">The addin path.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
            Justification = "factory method")]
        public static AddinUpdater CreateUpdaterAndApply(string packageId, Uri downloadSource, string productId, string autoUpdateVersion,
            string autoUpdateComponentBaseName, string addinPath)
        {
            AddinUpdater updater = null;
            try
            {
                updater = new AddinUpdater(packageId, downloadSource, productId, autoUpdateVersion, addinPath, autoUpdateComponentBaseName);

                // consider changing check logic so that we do not need to do check and then use IsUpdateAvaible
                updater.CheckForUpdates();

                if (updater.IsUpdateAvailable())
                {
                    //copy to avoid capture issues.
                    AddinUpdater updater1 = updater;

                    Func<bool> doWork = updater1.ApplyUpdate;
                    bool updateApplied = RetryWithSleep(doWork, _standardRetryCount);
                    
                    if (!updateApplied)
                    {
                        updater.IncrementFailureCount();
                    }
                }
            }
            catch (Exception ex)
            {
                using (var lm = new LogManager())
                {
                    lm.WriteError(null, "Failure in CreateUpdaterAndApply for package id: '{1}'. Exception: {0}", ex.ExceptionAsString(), packageId);
                }
                if (updater != null)
                {
                    updater.Dispose();
                    updater = null;
                }
            }
            return updater;
        }

        /// <summary>
        /// Creates the updater and check.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <param name="downloadSource">The download source.</param>
        /// <param name="productId">The product identifier.</param>
        /// <param name="autoUpdateVersion">The automatic update version.</param>
        /// <param name="autoUpdateComponentBaseName">Name of the automatic update component base.</param>
        /// <param name="addinPath">The addin path.</param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
            Justification = "factory method")]
        public static AddinUpdater CreateUpdaterAndCheck(string packageId, Uri downloadSource, string productId, string autoUpdateVersion,
            string autoUpdateComponentBaseName, string addinPath)
        {
            AddinUpdater updater = null;
            try
            {
                updater = new AddinUpdater(packageId, downloadSource, productId, autoUpdateVersion, addinPath, autoUpdateComponentBaseName);
                updater.CheckForUpdates();
            }
            catch (Exception ex)
            {
                using (var lm = new LogManager())
                {
                    lm.WriteError(null, "Failure in CreateUpdaterAndCheck for package id: '{1}'. Exception: {0}", ex.ExceptionAsString(), packageId);
                }
                if (updater != null)
                {
                    updater.Dispose();
                    updater = null;
                }
            }
            return updater;
        }

        /// <summary>
        /// Creates the process execution updater and apply.
        /// </summary>
        /// <returns></returns>
        public static AddinUpdater CreateProcessExecutionUpdaterAndApply()
        {
            //setup process execution package
            string pluginBasePath = ProductRootPath();
            var addinPath = GetProcessExecutionPluginPath(pluginBasePath);
            
            string packageId = GetProcessExectuionPackageId();
            Uri downloadUri = GetAutoUpdateAddress(packageId);
            
            string productId = ProcessExecutionProductId;
            string productVersion = GetProductVersion(packageId);
            string componentBaseName = ProcessExecutionComponentBaseName;
            AdjustAutoUpdateValues(packageId, ref productId, ref productVersion, ref componentBaseName);

            AddinUpdater updater = CreateUpdaterAndApply(packageId, downloadUri, productId, productVersion, componentBaseName, addinPath);

            return updater;
        }


        /// <summary>
        /// Creates the discovery updater and apply.
        /// </summary>
        /// <returns></returns>
        public static AddinUpdater CreateDiscoveryUpdaterAndApply()
        {
            //setup discovery plugins package
            string pluginBasePath = ProductRootPath();
            string addinPath = GetDiscoveryPluginsPath(pluginBasePath);

            string packageId = GetDiscoveryPluginsPackageId();
            Uri downloadUri = GetAutoUpdateAddress(packageId);

            string productId = DiscoveryProductId;
            string productVersion = GetProductVersion(packageId);
            string componentBaseName = DiscoveryComponetBaseName;
            AdjustAutoUpdateValues(packageId, ref productId, ref productVersion, ref componentBaseName);

            AddinUpdater updater = CreateUpdaterAndApply(packageId, downloadUri, productId, productVersion, componentBaseName, addinPath);
            
            return updater;
        }


        private static bool RetryWithSleep(Func<bool> function, int count)
        {
            bool retval;
            int i = 0;
            do
            {
                retval = function();
                if (retval == false)
                {
                    Thread.Sleep(500);
                }
                else
                {
                    break;
                }
            } while (i++ < count);
            return retval;
        }
        
        /// <summary>
        /// Creates the discovery updater and apply.
        /// </summary>
        /// <returns></returns>
        public static AddinUpdater CreateConnectorCoreUpdaterAndCheck()
        {
            //setup connector core
            string pluginBasePath = ProductRootPath();
            string addinPath = GetConnectorCorePluginsPath(pluginBasePath);

            string packageId = GetConnectorCorePackageId();
            Uri downloadUri = GetAutoUpdateAddress(packageId);

            string productId = ConnectorCoreProductId;
            string productVersion = GetProductVersion(packageId);
            string componentBaseName = ConnectorCoreComponentBaseName;
            AdjustAutoUpdateValues(packageId,ref productId, ref productVersion, ref componentBaseName);

            
            AddinUpdater updater = null;
            try
            {
                updater = new AddinUpdater(packageId, downloadUri, productId, productVersion, addinPath, componentBaseName);
                updater.StagingPath = GetTempFolder();
                Debug.WriteLine("Connector core staging path: {0}",updater.StagingPath);
                updater.HasCustomBackup = true;
                updater.HasCustomUpdate = true;
                updater.CustomCheckAction = CoreConnectorUpdateCheck;
                updater.AllowOnlyGreaterVersionsToUpdate = true;
                updater.CheckForUpdates();
            }
            catch (Exception ex)
            {
                if (updater != null)
                {
                    updater.Dispose();
                    updater = null;
                }
                using (var lm = new LogManager())
                {
                    lm.WriteError(null, "Failure in CreateConnectorCoreUpdaterAndCheck for package id: '{1}'. Exception: {0}", ex.ExceptionAsString(), packageId);
                }
            }
            return updater;
        }

        private static string GetTempFolder()
        {
            string basePath = Path.GetTempPath();
            string target;

            bool found = false;
            do
            {
                string fileName = Path.GetRandomFileName();
                target = Path.Combine(basePath, fileName);
                if (!Directory.Exists(target))
                {
                    found = true;
                }
                //else it already exists try again...
            } while (!found);

            Directory.CreateDirectory(target);
            return target;
        }

        /// <summary>
        /// Checks for update.
        /// </summary>
        /// <param name="updater">The updater.</param>
        /// <returns></returns>
        public static bool CheckForUpdate(AddinUpdater updater)
        {
            //get fresh values for the AU settings.
            
            //consider at moving this into adjust maybe.
            string packageId = updater.PackageId;
            Uri serviceUri = GetAutoUpdateAddress(packageId);
            updater.ServiceUri = serviceUri;

            string productId = updater.ProductId;
            string productVersion = GetProductVersion(packageId);
            string componentBaseName = updater.ComponentBaseName;
            AdjustAutoUpdateValues(packageId, ref productId, ref productVersion, ref componentBaseName);

            updater.ProductId = productId;
            updater.ProductVersion = productVersion;
            updater.ComponentBaseName = componentBaseName;

            bool hasUpdate = updater.CheckForUpdates();
            return hasUpdate;
        }


        /// <summary>
        /// Gets the product version.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <returns></returns>
        private static string GetProductVersion(string packageId)
        {
            string retval = string.Empty;
            if (String.Compare(packageId, _connectorCorePackageId, StringComparison.OrdinalIgnoreCase) == 0)
            {
                retval = GetConnectorCoreProductVersion();
            }
            else if (String.Compare(packageId, _processExecutionPackageId, StringComparison.OrdinalIgnoreCase) == 0)
            {
                retval = GetProcessExecutionProductVersion();
            }
            else if (String.Compare(packageId, _discoveryPluginsPackageid, StringComparison.OrdinalIgnoreCase) == 0)
            {
                retval = GetDiscoveryPluginProductVersion();
            }

            return retval;
        }

        /// <summary>
        /// Gets the automatic update address.
        /// </summary>
        /// <param name="packageId">The automatic update package identifier.</param>
        /// <returns></returns>
        public static Uri GetAutoUpdateAddress(string packageId)
        {
            //Uri stagingUri = new Uri("https://update.staging.sage.com/au/services/UpdateService");
            //Uri fileUri = new Uri("file://ORB515757/AutoUpdate");
            Uri productionUri = new Uri("https://update.sage.com/au/services/UpdateService");

            //default to production if something bad happens to the auto update config files.
            Uri retval = productionUri;
            string configLookup = ConfigurationProvider.GetPackageSetting(packageId, PackageConfigKeys.ServiceUri);

            if (!String.IsNullOrWhiteSpace(configLookup))
            {
                retval = new Uri(configLookup);
            }

            return retval;
        }

        

        /// <summary>
        /// Gets the name of the back office plugin component base.
        /// </summary>
        /// <param name="backOfficeId">The back office identifier.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">backOfficeId</exception>
        public static string GetBackOfficePluginComponentBaseName(string backOfficeId)
        {
            if (string.IsNullOrEmpty(backOfficeId)) throw new ArgumentNullException("backOfficeId");

            string baseName = "BackOfficePlugins." + backOfficeId;
            return baseName;
        }


        /// <summary>
        /// Gets the back office plugin path.
        /// </summary>
        /// <param name="pluginBasePath">The plugin base path.</param>
        /// <param name="backOfficeId">The back office identifier.</param>
        /// <returns></returns>
        public static string GetBackOfficePluginPath(string pluginBasePath, string backOfficeId)
        {
            string addinPath = CreateBackOfficePluginPath(backOfficeId, pluginBasePath);
            return addinPath;
        }

        /// <summary>
        /// Gets the process execution plugin path.
        /// </summary>
        /// <param name="pluginBasePath">The plugin base path.</param>
        /// <returns></returns>
        public static string GetProcessExecutionPluginPath(string pluginBasePath)
        {
            pluginBasePath = pluginBasePath ?? "";

            var addinPath = Path.Combine(pluginBasePath, AutoUpdateManager.ProcessExecutionPartialPath);
            if (!Directory.Exists(addinPath))
                Directory.CreateDirectory(addinPath);
            return addinPath;
        }

        /// <summary>
        /// Gets the discovery plugins path.
        /// </summary>
        /// <param name="pluginBasePath">The plugin base path.</param>
        /// <returns></returns>
        public static string GetDiscoveryPluginsPath(string pluginBasePath)
        {
            pluginBasePath = pluginBasePath ?? "";
            string addinPath = Path.Combine(pluginBasePath, AutoUpdateManager.DiscoveryPartialPath);
            if (!Directory.Exists(addinPath))
                Directory.CreateDirectory(addinPath);
            return addinPath;
        }

        /// <summary>
        /// Gets the connector core plugins path.
        /// </summary>
        /// <param name="pluginBasePath">The plugin base path.</param>
        /// <returns></returns>
        public static string GetConnectorCorePluginsPath(string pluginBasePath)
        {
            //the we treat the base path as the plugin root for core connector.
            string addinPath = pluginBasePath;
            return addinPath;
        }

        /// <summary>
        /// Gets the discovery plugin product version.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Discovery depends on core, and on contracts.
        /// </remarks>
        public static string GetDiscoveryPluginProductVersion()
        {
            return GetInstallDirAutoUpdateProductVersion();
        }

        /// <summary>
        /// Gets the process execution product version.
        /// </summary>
        /// <returns></returns>
        public static string GetProcessExecutionProductVersion()
        {
            //TODO: does this need to be a compound string? Was "1.10"
            return GetInstallDirAutoUpdateProductVersion();
        }

        /// <summary>
        /// Gets the connector core product version.
        /// </summary>
        /// <returns></returns>
        public static string GetConnectorCoreProductVersion()
        {
            return GetInstallDirAutoUpdateProductVersion();
        }

        /// <summary>
        /// Get the installed product version for auto update.
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private static string GetInstallDirAutoUpdateProductVersion()
        {
            string retval = _defaultAutoUpdateProductVersionIfFileMissing;
            string basePath = ProductRootPath();
            string targetPath = Path.Combine(basePath, _autoUpdateProductVersionFile);
            if (File.Exists(targetPath))
            {
                retval = File.ReadAllText(targetPath);
            }
            return retval;
        }
        private static string _autoUpdateProductVersionFile = "AutoUpdateProductVersion.txt";
        private static string _defaultAutoUpdateProductVersionIfFileMissing = "1.0";

        /// <summary>
        /// Adjusts the automatic update values.
        /// </summary>
        /// <param name="backOfficeId">The back office identifier.</param>
        /// <param name="auProductId">The au product identifier.</param>
        /// <param name="auProductVersion">The au product version.</param>
        /// <param name="auComponentBaseName">Name of the au component base.</param>
        /// <remarks>
        /// Adjust auto update values for back office products that do not yet have auto update values setup or 
        /// for testing and development needs.
        /// </remarks>
        public static void AdjustBackOfficePluginAutoUpdateValues(
            string backOfficeId,
            ref string auProductId,
            ref string auProductVersion,
            ref string auComponentBaseName)
        {
            //Work around for back office plugins that do not yet have the new auto update fields.
            //also allows for testing overrides and such.
            string packageId = GetBackOfficePluginPackageId(backOfficeId);
            AdjustAutoUpdateValues(packageId, ref auProductId, ref auProductVersion, ref auComponentBaseName);
        }


        /// <summary>
        /// Adjusts the automatic update values.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <param name="auProductId">The au product identifier.</param>
        /// <param name="auProductVersion">The au product version.</param>
        /// <param name="auComponentBaseName">Name of the au component base.</param>
        private static void AdjustAutoUpdateValues(
            string packageId,
            ref string auProductId,
            ref string auProductVersion,
            ref string auComponentBaseName)
        {
            string configLookup = ConfigurationProvider.GetPackageSetting(packageId, PackageConfigKeys.ProductVersion);
            if (!String.IsNullOrWhiteSpace(configLookup))
                auProductVersion = configLookup;

            configLookup = ConfigurationProvider.GetPackageSetting(packageId, PackageConfigKeys.ProductId);
            if (!String.IsNullOrWhiteSpace(configLookup))
                auProductId = configLookup;

            configLookup = ConfigurationProvider.GetPackageSetting(packageId, PackageConfigKeys.ComponentBaseName);
            if (!String.IsNullOrWhiteSpace(configLookup))
                auComponentBaseName = configLookup;
        }

        /// <summary>
        /// Gets the process execution package identifier.
        /// </summary>
        /// <returns></returns>
        public static string GetProcessExectuionPackageId()
        {
            return _processExecutionPackageId;
        }

        /// <summary>
        /// Gets the discovery plugins package identifier.
        /// </summary>
        /// <returns></returns>
        public static string GetDiscoveryPluginsPackageId()
        {
            return _discoveryPluginsPackageid;
        }

        /// <summary>
        /// Gets the connector core package identifier.
        /// </summary>
        /// <returns></returns>
        public static string GetConnectorCorePackageId()
        {
            return _connectorCorePackageId;
        }

        /// <summary>
        /// Gets the back office plugin package identifier.
        /// </summary>
        /// <param name="backofficeId">The backoffice identifier.</param>
        /// <returns></returns>
        public static string GetBackOfficePluginPackageId(string backofficeId)
        {
            string retval = string.Format("{0}.{1}", "BackOfficePlugin", backofficeId);
            return retval;
        }

        /// <summary>
        /// Sets the update information.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="cloudUpgradeInfo">The cloud upgrade information.</param>
        public static void SetUpdateInfo(ConnectorUpdateStatus status, UpdateInfo cloudUpgradeInfo)
        {
            try
            {
                UpdateInfo updateInfo = null;

                if (cloudUpgradeInfo != null)
                {
                    updateInfo = new UpdateInfo(
                    cloudUpgradeInfo.ProductVersion,
                    cloudUpgradeInfo.PublicationDate,
                    cloudUpgradeInfo.UpdateDescription,
                    cloudUpgradeInfo.UpdateLinkUri);
                }

                using (var proxy = StateServiceProxyFactory.CreateFromCatalog(
                                "localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                {
                    proxy.SetUpdateInfo(status, updateInfo);
                }
            }
            catch (Exception ex)
            {
                using (LogManager lm = new LogManager())
                {
                    lm.WriteError(null, "Unable to set update version information: " + ex.ExceptionAsString());
                }
            }
        }

        /// <summary>
        /// Cores the connector update check.
        /// </summary>
        /// <param name="update">The update.</param>
        /// <param name="updatePath">The update path.</param>
        private static void CoreConnectorUpdateCheck(ISoftwareUpdate update, string updatePath)
        {
            ConnectorUpdateStatus updateStatus = GetUpdateStatus(update);
            UpdateInfo updateInfo = GetUpdateInfo(update, updatePath);
            
            SetUpdateInfo(updateStatus, updateInfo);
        }

        private static ConnectorUpdateStatus GetUpdateStatus(ISoftwareUpdate update)
        {
            ConnectorUpdateStatus updateStatus = ConnectorUpdateStatus.None;
            if (update == null)
            {
                return updateStatus;
            }
            
            List<string> requiredUpdates = new List<string>() {"Critical", "Important"};
            bool isInRequiredUpdateList = requiredUpdates.Any(i => (String.Compare(i, update.UpdateTypeId, StringComparison.OrdinalIgnoreCase) == 0));
            if (isInRequiredUpdateList)
            {
                updateStatus = ConnectorUpdateStatus.UpdateRequired;
            }
            else
            {
                updateStatus = ConnectorUpdateStatus.UpdateAvailable;
            }

            return updateStatus;
        }

        private static UpdateInfo GetUpdateInfo(ISoftwareUpdate update, string updatePath)
        {
            UpdateInfo updateInfo = null;

            if (update != null)
            {
                string productVersion = GetProductVersionFromUpdate(update);

                //TODO is there any more info to be found about publication date.
                DateTime publicationDate = DateTime.MinValue;

                string updateDescription = Resource.CoreConnector_UpdateDescription;
                //we may want to change this to update.Description eventualy.

                Uri updateLink = null;

                //DT990.
                //we have an issue where "sometimes" the update path does not end in an exe.
                //we are expecting the update path to be an exe file but the exe got chopped.
                bool updatePathExists = File.Exists(updatePath);
                string updatePathToUse = string.Empty;
                if (updatePathExists)
                {
                    updatePathToUse = updatePath;
                }
                else
                {
                    string testPath = String.Concat(updatePath,".exe");
                    bool pathExists = File.Exists(testPath);
                    if (pathExists)
                    {
                        updatePathToUse = testPath;
                    }
                }

                if (!String.IsNullOrWhiteSpace(updatePathToUse))
                {
                    string path = String.Format(@"file://{0}", updatePathToUse);
                    Uri testLink = new Uri(path);

                    string testPath = testLink.LocalPath;
                    if (File.Exists(testPath))
                        updateLink = testLink;

                    //May want to consider moving this to a different spot in the stack.
                    //it works here as part of a late bug fix though.
                    //We have an issue with access to the core connector update file for some users.
                    //they do now have rights to the windows/temp folder. So.. explicitly give everyone read execute rights
                    //to the update. Since windows sets Traverse Bypass by default for everyone, the fact that the user does not have
                    //rights to some directories along the path is a non issue.
                    GiveAllUsersReadExecutePermissionsToFile(testPath);
                }
   
                updateInfo = new UpdateInfo(
                    productVersion,
                    publicationDate,
                    updateDescription,
                    updateLink
                    );
            }
            return updateInfo;

        }

        private static void GiveAllUsersReadExecutePermissionsToFile(string filePath)
        {
            try
            {
                //file path is known good at this point.

                // current security settings.
                FileSecurity fSecurity = File.GetAccessControl(filePath);

                // read execute rights all members of the users group.
                fSecurity.AddAccessRule(
                    new FileSystemAccessRule(
                        new SecurityIdentifier(WellKnownSidType.BuiltinUsersSid, null),
                        FileSystemRights.ReadAndExecute,
                        AccessControlType.Allow));

                // Set the new access settings.
                File.SetAccessControl(filePath, fSecurity);
            }
            catch (Exception ex)
            {
                using (LogManager lm = new LogManager())
                {
                    lm.WriteError(null, "Unable to set core update exe permissions to read and execute for well known group 'Users': " + ex.ExceptionAsString());
                }
            }
        }

        private static string GetProductVersionFromUpdate(ISoftwareUpdate update)
        {
            string retval = string.Empty;
            if (update != null)
            {
                //closest we get to a version if the update id so populte with that.
                retval = update.UpdateId;
            }
            return retval;
        }

        private static string ProductRootPath()
        {
            if (_productRootPath == null)
            {
                _productRootPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "";
            }

            return _productRootPath;
        }

        private static string _productRootPath;
    }
}
