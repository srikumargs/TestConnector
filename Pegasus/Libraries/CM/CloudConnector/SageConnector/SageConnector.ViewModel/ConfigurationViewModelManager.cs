using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Security;
using Sage.Connector.Data;
using Sage.Connector.Utilities;

namespace SageConnector.ViewModel
{
    /// <summary>
    /// Manage the set of the configuration view models and the ui backing data
    /// </summary>
    public class ConfigurationViewModelManager
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public ConfigurationViewModelManager()
        {
            _currentConfiguration = null;
            _configurations = new ConfigurationViewModelList();
        }

        /// <summary>
        /// The current configuration in use
        /// </summary>
        public ConfigurationViewModel CurrentConfiguration
        {
            get { return _currentConfiguration; }
            private set { _currentConfiguration = value; }
        }
        ConfigurationViewModel _currentConfiguration;


        /// <summary>
        /// List of the current configuration view models...
        /// </summary>
        public ConfigurationViewModelList Configurations
        {
            get
            { 
                //NOTE: is there a way to get readonly back now we are a binding list
                return _configurations; 
            }
        }
        private ConfigurationViewModelList _configurations;

        /// <summary>
        /// Fill the configuration list
        /// </summary>
        /// <remarks>
        /// Has side effect of changing selected item
        /// </remarks>
        public void FillList()
        {
            _configurations.RaiseListChangedEvents = false;
            if (_configurations.Count == 0)
            {
                PremiseConfigurationRecord[] configurations = GetAllPremiseConfigurationRecords();

                var list = CreateConfigurationViewModelList(configurations);
                foreach (var item in list)
                {
                    _configurations.Add(item);
                }
            }
            else
            {
                MergeList();
            }
            
            CurrentConfiguration = _configurations.FirstOrDefault();
            _configurations.RaiseListChangedEvents = true;
            _configurations.ResetBindings();
        }

        /// <summary>
        /// Safe way to get all PCRs
        /// If the hosting framework is down, we will get back a null list
        /// Instead of bubbling up exceptions to the data grid view in the UI
        /// </summary>
        /// <returns></returns>
        private PremiseConfigurationRecord[] GetAllPremiseConfigurationRecords()
        {
            PremiseConfigurationRecord[] result = null;
            try
            {
                result = ConfigurationSettingFactory.RetrieveAllConfigurations();
            }
            catch (SecurityAccessDeniedException)
            {
                throw; // user is not allowed 
            }
            catch
            {
                // Case where service is unavailable
                // We want to return an empty list of configurations
            }

            return result;
        }

        /// <summary>
        /// Synchronize the Premise configuration database with in memory list.
        /// </summary>
        /// <remarks>
        /// if calling from outside FillList make sure to manage RaiseListChangedEvents and ResetBindings()
        /// </remarks>
        private void MergeList()
        {
            List<string> tenantIdsInList = new List<string>();

            PremiseConfigurationRecord[] newConfigList = GetAllPremiseConfigurationRecords();
            int originalConfigCount = _configurations.Count;
            if (null != newConfigList)
            {
                for (int pcrIndex = 0; pcrIndex < newConfigList.Count(); ++pcrIndex)
                {
                    AddOrUpdateConfiguration(newConfigList[pcrIndex], pcrIndex);
                    tenantIdsInList.Add(newConfigList[pcrIndex].CloudTenantId);
                }
            }

            DeleteLegacyConfigurations(tenantIdsInList);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// if calling from outside FillList make sure to manage RaiseListChangedEvents and ResetBindings()
        /// </remarks>
        /// <param name="premiseConfig"></param>
        /// <param name="insertIndex"></param>
        private void AddOrUpdateConfiguration(PremiseConfigurationRecord premiseConfig, int insertIndex)
        {
            //check if the configuration is in the list.
            ConfigurationViewModel foundTenant = _configurations.FirstOrDefault(item => item.CloudTenantId.Equals(premiseConfig.CloudTenantId));
            
            if (foundTenant == null)
            {
                // Insert into the correct location
                // This code will likely not be hit since when we click add, a new configuration is already added
                // To our list.  So one should always be found.
                ConfigurationViewModel newModel = new ConfigurationViewModel(premiseConfig);
                _configurations.Insert(insertIndex, newModel);
            }
            else
            {
                // Check if the found tenant is in the correct location
                int originalIndex = _configurations.IndexOf(foundTenant);
                if (originalIndex != insertIndex)
                {
                    // Do an in place move
                    _configurations.RemoveAt(originalIndex);
                    _configurations.Insert(insertIndex, foundTenant);
                }
                foundTenant.SynchronizeFromUpdatedPremiseConfiguration(premiseConfig);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// if calling from outside FillList make sure to manage RaiseListChangedEvents and ResetBindings()
        /// </remarks>
        /// <param name="activeTenants"></param>
        private void DeleteLegacyConfigurations(List<string> activeTenants)
        {
            List<ConfigurationViewModel> deleteList = new List<ConfigurationViewModel>();

            foreach (ConfigurationViewModel cvm in _configurations)
            {
                bool modelIsActive = (null != activeTenants.Find(item => item == cvm.CloudTenantId));
                if (!modelIsActive)
                {
                    deleteList.Add(cvm);
                }
            }
           
            foreach (ConfigurationViewModel cvm in deleteList)
            {
                _configurations.Remove(cvm);
            }
        }

        private List<ConfigurationViewModel> CreateConfigurationViewModelList(PremiseConfigurationRecord[] _listOfConfigurations)
        {
            List<ConfigurationViewModel> viewModels = new List<ConfigurationViewModel>();
            if (_listOfConfigurations != null)
            {
                foreach (PremiseConfigurationRecord pcr in _listOfConfigurations)
                {
                    ConfigurationViewModel viewModel = new ConfigurationViewModel(pcr);
                    viewModels.Add(viewModel);
                }
            }
            return viewModels;
        }

        /// <summary>
        /// Create a new tenant
        /// </summary>
        public ConfigurationViewModel CreateNewTenant(ConnectorPlugin plugin)
        {
            ConfigurationViewModel newConfig = null;
            PremiseConfigurationRecord newPcr = ConfigurationSettingFactory.CreateNewTenant();
            newPcr.ConnectorPluginId = plugin.Id;
            newPcr.BackOfficeProductName = plugin.PluggedInProductName;
            newConfig = new ConfigurationViewModel(newPcr);
            return newConfig;
        }

        /// <summary>
        /// Remove configuration from database
        /// </summary>
        /// <param name="config"></param>
        public bool DeleteConfiguration(ConfigurationViewModel config)
        {
            bool bRetVal = ConfigurationSettingFactory.DeleteTenant(config.CloudTenantId);
            //clear current item. if the list has items the UI selection change will
            //set current to whatever is the new current.
            CurrentConfiguration = null;

            return bRetVal;
        }

        /// <summary>
        /// Set the current item to the selected tenant ID.
        /// </summary>
        /// <param name="tenantId"></param>
        public void SetCurrent(string tenantId)
        {
            //TODO: so what should happen if tenant ID is not found and is this the same as a bad tenant id?
            if (null != Configurations)
            {
                foreach (ConfigurationViewModel viewModel in Configurations)
                {
                    if (viewModel.CloudTenantId == tenantId)
                    {
                        CurrentConfiguration = viewModel;
                        break;
                    }
                }
            }
        }


        /// <summary>
        /// Returns false if the tenantId is already in use.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public bool ValidateTenantIdUnique(string tenantId)
        {
            FillList();
            foreach (ConfigurationViewModel model in Configurations)
            {
                if (tenantId.Equals(model.CloudTenantId))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Returns false if the ConnectionKey is already in use.
        /// </summary>
        /// <param name="connectionKey"></param>
        /// <returns></returns>
        public bool ValidateConnectionKeyUnique(string connectionKey)
        {
            FillList();
            foreach (ConfigurationViewModel model in Configurations)
            {
                if (connectionKey.Equals(model.CompositeConnectionKey))
                    return false;
            }
            return true;
        }
    }
}
