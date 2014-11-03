using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Sage.Connector.AutoUpdate.Addin;
using Sage.Connector.Common;
using Sage.Connector.Data;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.Connector.StateService.Proxy;

namespace Sage.Connector.AutoUpdate
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// This class is mostly a helper so that Dispatch service does not have all this auto update specific code embedded in it.
    /// The file actions that are triggered here are for the most part all things that need to happen in an admin context. Given
    /// where our plugins are located currently in the program files tree they require admin rights to update.
    /// </remarks>
    sealed public class ActiveUpdaters: IDisposable
    {
        private ConcurrentDictionary<string, AddinUpdater> _addinUpdaters = new ConcurrentDictionary<string, AddinUpdater>();
        private AddinUpdateTimer _addinUpdateTimer;
        private Func<PremiseConfigurationRecord[]> _getConfigurations;
        private readonly Action _checkForUpdatesCompleteAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActiveUpdaters" /> class.
        /// </summary>
        /// <param name="getConfigurations">The get configurations.</param>
        /// <param name="checkForUpdatesCompleteAction">The check for updates complete action.</param>
        public ActiveUpdaters(Func<PremiseConfigurationRecord[]> getConfigurations, Action checkForUpdatesCompleteAction)
        {
            _getConfigurations = getConfigurations;
            _checkForUpdatesCompleteAction = checkForUpdatesCompleteAction;
        }

        /// <summary>
        /// Gets the updaters.
        /// </summary>
        /// <value>
        /// The updaters.
        /// </value>
        public IEnumerable<AddinUpdater> Updaters
        {
            get { return _addinUpdaters.Values; }
        }

        /// <summary>
        /// Creates the addin update handler associated with the dispatch controller.
        /// </summary>
        public void CreateUpdateHandlers()
        {
            AddinUpdater updater;

            //setup process execution package
            updater = AutoUpdateManager.CreateProcessExecutionUpdaterAndApply();
            if (updater != null) _addinUpdaters.TryAdd(updater.PackageId, updater);

            //setup discovery plugins package
            updater = AutoUpdateManager.CreateDiscoveryUpdaterAndApply();
            if (updater != null) _addinUpdaters.TryAdd(updater.PackageId, updater);

            //setup connector core package
            updater = AutoUpdateManager.CreateConnectorCoreUpdaterAndCheck();
            if (updater != null) _addinUpdaters.TryAdd(updater.PackageId, updater);


            string pluginBasePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            UpdateCurrentTenantBackOfficePlugins(pluginBasePath);

            var intervalProvider = new AutoUpdateTimerIntervalProvider();

            _addinUpdateTimer = new AddinUpdateTimer(
                    () => CheckForAutoUpdates(),
                    intervalProvider
                );
            //set last update for the update timer. this will prevent an automatic check when 
            //the start interval passes. It will instead way for the full interval.
            _addinUpdateTimer.SetLastUpdate();
        }

        private void CreateUpdaterCheckAndAddToList(string pluginBasePath, string backOfficeId, BackOfficePlugin plugin)
        {
            string addinPath = AutoUpdateManager.GetBackOfficePluginPath(pluginBasePath, backOfficeId);
            if (!Directory.Exists(addinPath))
                Directory.CreateDirectory(addinPath);

            string auProductId = plugin.BackOfficePluginAutoUpdateProductId;
            string auProductVersion = plugin.BackOfficePluginAutoUpdateProductVersion;
            string auComponentBaseName = plugin.BackOfficePluginAutoUpdateComponentBaseName;

            string packageId = AutoUpdateManager.GetBackOfficePluginPackageId(backOfficeId);
            Uri downloadUri = AutoUpdateManager.GetAutoUpdateAddress(packageId);

            //note value adjustments were done in state service.
            AddinUpdater updater = AutoUpdateManager.CreateUpdaterAndApply(packageId, downloadUri, auProductId, auProductVersion, auComponentBaseName, addinPath);
            if (updater != null) _addinUpdaters.TryAdd(updater.PackageId, updater);
        }

        /// <summary>
        /// Checks for automatic updates.
        /// </summary>
        /// <returns></returns>
        public bool CheckForAutoUpdates()
        {
            //consider adding a lock, does not seem needed currently.

            //for each of the addin up-daters have them check to see if there is an update, but have to use the latest version number and such.
            //however do not do the install that will be up to the dispatcher.

            bool foundUpdate = false;
            AddinUpdater updater;
            string packageId;
            //check PE


            packageId = AutoUpdateManager.GetConnectorCorePackageId();
            updater = FindUpdater(packageId);
            if (updater != null)
            {
                bool hasUpdate = AutoUpdateManager.CheckForUpdate(updater);
            }

            packageId = AutoUpdateManager.GetProcessExectuionPackageId();
            updater = FindUpdater(packageId);
            if (updater != null)
            {
                bool hasUpdate = AutoUpdateManager.CheckForUpdate(updater);
                foundUpdate = foundUpdate || hasUpdate;
            }

            //Check Discovery
            packageId = AutoUpdateManager.GetDiscoveryPluginsPackageId();
            updater = FindUpdater(packageId);
            if (updater != null)
            {
                bool hasUpdate = AutoUpdateManager.CheckForUpdate(updater);
                foundUpdate = foundUpdate || hasUpdate;
            }

            string pluginBasePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            bool pluginUpdated = CheckCurrentTenantBackOfficePluginsForUpdate(pluginBasePath);
            foundUpdate = foundUpdate || pluginUpdated;

            //we just finished an update so set the last update
            //for now we do not want a manual update check to reset the periodic check.
            //if this changes then add this back in.
            //_addinUpdateTimer.SetLastUpdate();

            //QueuePhantomMessageToForceUpdateApply();
            _checkForUpdatesCompleteAction();

            return foundUpdate;
        }


        private bool CheckCurrentTenantBackOfficePluginsForUpdate(string pluginBasePath)
        {
            //do not really need an active list to know what to check.
            //do need the list to handle pending updates and time intervals.
            //so when its time to check, check them all, add and remove from the update list based on what we get?
            //if the plugin is no longer in the list, leave it be for now or remove it?

            bool foundUpdate = false;

            //get any configurations that may exist
            PremiseConfigurationRecord[] configurations = _getConfigurations();
            if (configurations != null)
            {
                //we have some configurations so we will need to get some data 
                BackOfficePlugin[] installedPlugins = GetPlugins();
                if (installedPlugins == null)
                {
                    //found no plugins so no point in continuing.
                    return foundUpdate;
                }
                var backOfficeIds = (from pcr in configurations
                                     select pcr.ConnectorPluginId).Distinct();
                foreach (var backOfficeId in backOfficeIds)
                {

                    BackOfficePlugin plugin = installedPlugins.FirstOrDefault(p => p.PluginId == backOfficeId);
                    if (plugin == null)
                    {
                        //Did not find a discovery plugin 
                        continue;
                    }
                    if (String.IsNullOrWhiteSpace(plugin.BackOfficeVersion))
                    {
                        //back office no longer installed
                        continue;
                    }

                    string packageId = AutoUpdateManager.GetBackOfficePluginPackageId(backOfficeId);
                    AddinUpdater updater = FindUpdater(packageId);
                    if (updater != null)
                    {
                        Uri serviceUri = AutoUpdateManager.GetAutoUpdateAddress(packageId);
                        updater.ServiceUri = serviceUri;

                        //note value adjustments were done in state service.
                        string productId = plugin.BackOfficePluginAutoUpdateProductId;
                        string productVersion = plugin.BackOfficePluginAutoUpdateProductVersion;
                        string componentBaseName = plugin.BackOfficePluginAutoUpdateComponentBaseName;

                        updater.ProductId = productId;
                        updater.ProductVersion = productVersion;
                        updater.ComponentBaseName = componentBaseName;

                        bool hadUpdate = updater.CheckForUpdates();
                        foundUpdate = foundUpdate || hadUpdate;
                    }
                    else
                    {
                        //we have an plugin that we do not have an updater for.
                        CreateUpdaterCheckAndAddToList(pluginBasePath, backOfficeId, plugin);
                    }
                    //Do we want to remove plugins that no longer have PCR here? No. 
                    //plugins without pcrs could still be in use while creating a pcr.
                    
                }
            }
            return foundUpdate;
        }

        /// <summary>
        /// Deletes the unused automatic update support.
        /// </summary>
        /// <remarks>
        /// We are more limited in what we can do there than may be initially apparent.
        /// Configuration process allows a plugin to be downloaded and used before
        /// there is a saved PCR record. So we if we allow removing a plugin from disk
        /// we risk deleting one that is in use. For now we leave them on disk.
        /// If we need to purge them from disk then we need to add a lock system for configuration so that
        /// when we are initially configuring a connection we can know its in use.
        /// </remarks>
        public void DeleteUnusedAutoUpdateSupport()
        {
            //find any updaters in memory that no longer have pcrs and remove them
            //find any back office plugins on disk that no longer have a pcr/updater
            //Watch out for delete in the middle of an update. May need a lock/crit section.
            //maybe tell object to clean itself up when its a good time..

            PremiseConfigurationRecord[] configurations = _getConfigurations();
            if (configurations != null)
            {
                //we have some configurations so we will need to get some data 
                BackOfficePlugin[] installedPlugins = GetPlugins();
                if (installedPlugins == null)
                {
                    //found no plugins so no point in continuing.
                    return;
                }

                //for each updater check of there is a pcr to match.
                //if we find a back office updater that does not have a pcr mark it for deletion.
                //watch out for what race conditions.
                string[] systemUpdaters = new string[] 
                { 
                    AutoUpdateManager.GetConnectorCorePackageId(), 
                    AutoUpdateManager.GetProcessExectuionPackageId(),
                    AutoUpdateManager.GetDiscoveryPluginsPackageId()
                };

                //updaters deal in package ids, so get the package ids for the still existing back office connections.
                var backOfficeIds = (from pcr in configurations select pcr.ConnectorPluginId).Distinct().ToList();
                var backOfficePackageIds = backOfficeIds.Select(AutoUpdateManager.GetBackOfficePluginPackageId).ToList();

                List<AddinUpdater> updatersWithoutPCRs = new List<AddinUpdater>();

                foreach (var updater in _addinUpdaters.Values)
                {
                    bool isSystemUpdater = systemUpdaters.Any(s => s == updater.PackageId);
                    if (!isSystemUpdater)
                    {
                        bool existsInPlugins = backOfficePackageIds.Contains(updater.PackageId);
                        if (!existsInPlugins)
                        {
                            updatersWithoutPCRs.Add(updater);
                        }
                    }
                }

                //do we need locks or critical sections here? - Should not. collections is safe and will 
                foreach (var updater in updatersWithoutPCRs)
                {
                    AddinUpdater removedUpdater;
                    _addinUpdaters.TryRemove(updater.PackageId, out removedUpdater);
                    //This part is thread safe internally.

                    //see remarks, can not purge package support here after all.
                    //updater.PurgePackageSupport();
                    
                    updater.DisposeWhenPossible();
                }
            }
        }

        private AddinUpdater FindUpdater(string packageId)
        {
            //note: consider if it would be better to require that updaters always have lower case. Now that we are using a concurrent dictionary
            //rather then a list of updaters, we could maybe just do a lookup rather then iterate thru the values. Its going to be a short list
            //so should be close time wise in either case.
            var retval = _addinUpdaters.Values.FirstOrDefault(u => String.Compare(u.PackageId, packageId, StringComparison.OrdinalIgnoreCase) == 0);
            return retval;
        }


        private BackOfficePlugin[] GetPlugins()
        {
            BackOfficePlugin[] retval = null;
            try
            {
                BackOfficePluginsResponse response = null;
                // do the real work
                using (var proxy = BackOfficeValidationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                {
                    response = proxy.GetBackOfficePlugins();
                }
                if (response != null && response.RawErrorMessage.Length == 0 && response.UserFacingMessages.Length == 0)
                {
                    //no errors 
                    retval = response.BackOfficePlugins;
                }
            }
            catch (Exception ex)
            {
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(null, ex.ExceptionAsString());
                }
            }


            return retval;
        }

        private void UpdateCurrentTenantBackOfficePlugins(string pluginBasePath)
        {
            //get any configurations that may exist
            PremiseConfigurationRecord[] configurations = _getConfigurations();
            if (configurations != null)
            {
                //we have some configurations so we will need to get some data 
                BackOfficePlugin[] installedPlugins = GetPlugins();
                if (installedPlugins == null)
                {
                    //found no plugins so no point in continuing.
                    return;
                }
                var backOfficeIds = (from pcr in configurations
                                     select pcr.ConnectorPluginId).Distinct();
                foreach (var backOfficeId in backOfficeIds)
                {

                    BackOfficePlugin plugin = installedPlugins.FirstOrDefault(p => p.PluginId == backOfficeId);
                    if (plugin == null)
                    {
                        //did not find a discovery plugin
                        continue;
                    }
                    if (String.IsNullOrWhiteSpace(plugin.BackOfficeVersion))
                    {
                        //back office no longer installed.
                        continue;
                    }

                    //we have a plugin and we have done any value updates 
                    CreateUpdaterCheckAndAddToList(pluginBasePath, backOfficeId, plugin);
                }
            }
        }

        /// <summary>
        /// Downloads the back office plugin.
        /// </summary>
        /// <param name="backOfficeId">The back office identifier.</param>
        /// <param name="autoUpdateUri">The automatic update URI.</param>
        /// <param name="autoUpdateProductId">The automatic update product identifier.</param>
        /// <param name="autoUpdateProductVersion">The automatic update product version.</param>
        /// <param name="autoUpdateComponentBaseName">Name of the automatic update component base.</param>
        /// <returns></returns>
        public bool DownloadBackOfficePlugin(string backOfficeId, Uri autoUpdateUri, string autoUpdateProductId,
            string autoUpdateProductVersion,
            string autoUpdateComponentBaseName)
        {
            bool retval = false;
            string packageId = AutoUpdateManager.GetBackOfficePluginPackageId(backOfficeId);

            var alreadyAnUpdater = FindUpdater(packageId);
            if (alreadyAnUpdater == null)
            {
                //not already an updater for this. This means its not a known plugin. Safe to download it.
                retval = AutoUpdateManager.DownloadBackOfficePlugin(backOfficeId, autoUpdateUri, autoUpdateProductId,
                    autoUpdateProductVersion,
                    autoUpdateComponentBaseName);
            }
            else
            {
                //we already have an updater for this.
                retval = true;
            }

            return retval;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
           Dispose(true);
            //all managed code so do not really need the a finalizer, so no suppress
            //GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposeing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool disposeing)
        {
            try
            {
                if (_addinUpdaters != null)
                {
                    foreach (AddinUpdater updater in _addinUpdaters.Values)
                    {
                        updater.Dispose();
                    }
                }
            }
            finally
            {
                _addinUpdaters = null;
            }

            try
            {
                if (_addinUpdateTimer != null)
                {

                    _addinUpdateTimer.Dispose();

                }
            }
            finally
            {
                _addinUpdateTimer = null;
            }
        }
    }
}
