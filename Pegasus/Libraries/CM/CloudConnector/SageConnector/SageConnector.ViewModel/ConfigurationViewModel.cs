using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Sage.Connector.Common;
using Sage.Connector.ConnectorServiceCommon;
using Sage.Connector.Data;
using Sage.Connector.StateService.Interfaces;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.Connector.StateService.Proxy;
using Sage.Connector.Utilities;

namespace SageConnector.ViewModel
{
    /// <summary>
    /// View Model to wrap the Premise Configuration Record.
    /// </summary>
    public class ConfigurationViewModel : INotifyPropertyChanged
    {
        #region Properties and Private Members

        private PremiseConfigurationRecord _repository;
        private const string SAGE_CONNECTOR_ALLOW_MULTIPLE_CONNECTIONS = "SAGE_CONNECTOR_ALLOW_MULTIPLE_CONNECTIONS";

        /// <summary>
        /// Returns original Premise Key
        /// </summary>
        public string OriginalConfigurationPremiseKey
        {
            get
            {
                return _repository.CloudPremiseKey;
            }
        }

        /// <summary>
        /// Returns original Tenant Id
        /// </summary>
        public string OriginalTenantId
        {
            get
            {
                return _repository.CloudTenantId;
            }
        }

        /// <summary>
        /// Premise key pass thru
        /// </summary>
        public string CloudPremiseKey
        {
            get { return _cloudPremiseKey; }
            set
            {
                _cloudPremiseKey = value;
                IsDirty = true;
            }
        }
        string _cloudPremiseKey;

        /// <summary>
        /// Tenant Id pass thru
        /// </summary>
        public string CloudTenantId
        {
            get { return _cloudTenantId; }
            set
            {
                _cloudTenantId = value;
                IsDirty = true;
            }
        }
        string _cloudTenantId;


    /// <summary>
        /// Endpoint pass thru
        /// </summary>
        public string CloudEndpoint
        {
            get { return _cloudEndpoint; }
            set
            {
                _cloudEndpoint = value;
                IsDirty = true;
            }
        }
        string _cloudEndpoint;

        /// <summary>
        /// Min unavailable cloud polling interval
        /// </summary>
        public Int32 MinCommunicationFailureRetryInterval
        {
            get { return _minCommunicationFailureRetryInterval; }
            set
            {
                _minCommunicationFailureRetryInterval = value;
                IsDirty = true;
            }
        }
        Int32 _minCommunicationFailureRetryInterval;

        /// <summary>
        /// Max unavailable cloud polling interval
        /// </summary>
        public Int32 MaxCommunicationFailureRetryInterval
        {
            get { return _maxCommunicationFailureRetryInterval; }
            set
            {
                _maxCommunicationFailureRetryInterval = value;
                IsDirty = true;
            }
        }
        Int32 _maxCommunicationFailureRetryInterval;



        /// <summary>
        /// Connection key as it appears in the UI. Composite of TenantId and PremiseKey and SiteAddress
        /// </summary>
        public string CompositeConnectionKey
        {
            get
            {
                if (String.IsNullOrEmpty(CloudTenantId) && String.IsNullOrEmpty(CloudPremiseKey))
                {
                    //if both parts these parts of are key are empty just show nothing. Not just a dash URI
                    return String.Empty;
                }

                string retval = string.Concat(
                    CloudTenantId, 
                    _compositeConnectionKeySeperator,
                    CloudPremiseKey);

                string cloudUriEncoded = Utils.ToBase64(CloudEndpoint);
                if(!string.IsNullOrWhiteSpace(cloudUriEncoded))
                {
                    retval = string.Concat(
                        retval,
                        _compositeConnectionKeySeperator,
                        cloudUriEncoded);
                }

                return retval;
            }
            set
            {
                string id = string.Empty;
                string key = string.Empty;
                string uri = string.Empty;
                
                // if we are setting blank or have no split points, id/key is blanked
                // but endpoint is left alone
                if (!String.IsNullOrWhiteSpace(value))
                {
                    int splitPoint = value.IndexOf(_compositeConnectionKeySeperator);
                    if (splitPoint >= 0)
                    {
                        // we have at least one split point
                        id = value.Substring(0, splitPoint);
                        key = (splitPoint + 1 < value.Length) ?
                            value.Substring(splitPoint + 1) : string.Empty;

                        // now see if 'key' can be split for key and uri
                        if (!String.IsNullOrWhiteSpace(key))
                        {
                            splitPoint = key.IndexOf(_compositeConnectionKeySeperator);
                            if (splitPoint >= 0)
                            {
                                // we have yet another split point
                                uri = (splitPoint + 1 < key.Length) ?
                                    key.Substring(splitPoint + 1) : string.Empty;
                                key = key.Substring(0, splitPoint);
                            }
                        }
                    }
                }

                CloudTenantId = id.Trim();
                CloudPremiseKey = key.Trim();
                if (!String.IsNullOrWhiteSpace(uri))
                {
                    string decodedUri = Utils.FromBase64(uri.Trim());
                    if (!String.IsNullOrWhiteSpace(decodedUri))
                    {
                        CloudEndpoint = decodedUri;
                    }
                }
            }
        }
        char _compositeConnectionKeySeperator = ':';

        /// <summary>
        /// Should the connection to the cloud be able to receive new work from the cloud
        /// </summary>
        public bool CloudConnectionEnabledToReceive
        {
            get { return CloudConnectionEnabledToReceive_Private; }
            set
            {
                CloudConnectionEnabledToReceive_Private = value;
                IsDirty = true;
                NotifyPropertyChanged("CloudConnectionEnabledToReceive");
            }
        }
        /// <summary>
        /// Private version, just sets member var
        /// </summary>
        private bool CloudConnectionEnabledToReceive_Private
        {
            get { return _cloudConnectionEnabledToReceive; }
            set { _cloudConnectionEnabledToReceive = value; }
        }
        bool _cloudConnectionEnabledToReceive;

        /// <summary>
        /// Should the connection to the cloud be able to send work to the cloud
        /// </summary>
        public bool CloudConnectionEnabledToSend
        {
            get { return CloudConnectionEnabledToSend_Private; }
            set 
            {
                CloudConnectionEnabledToSend_Private = value;
                IsDirty = true;
                NotifyPropertyChanged("CloudConnectionEnabledToSend");
            }
        }
        /// <summary>
        /// Private version, just sets member var
        /// </summary>
        private bool CloudConnectionEnabledToSend_Private
        {
            get { return _cloudConnectionEnabledToSend; }
            set { _cloudConnectionEnabledToSend = value; }
        }
        bool _cloudConnectionEnabledToSend;

        /// <summary>
        /// Returns enum associated with current active status of cloud connection.
        /// </summary>
        public ConnectionActiveStatus CloudConnectionStatus
        {
            get { return _cloudConnectionStatus; }
            set
            {
                _cloudConnectionStatus = value;
                NotifyPropertyChanged("CloudConnectionStatus");
                NotifyPropertyChanged("CloudConnectionStatusImage");
            }
        }
        ConnectionActiveStatus _cloudConnectionStatus = ConnectionActiveStatus.None;

        /// <summary>
        /// Returns an image associated with the Status
        /// </summary>
        public Image CloudConnectionStatusImage
        {
            get
            {
                return ActiveImage(CloudConnectionStatus);
            }
        }

        /// <summary>
        /// Should the connection to the back office be active for starting new work.
        /// </summary>
        public bool BackOfficeConnectionEnabledToReceive
        {
            get { return BackOfficeConnectionEnabledToReceive_Private; }
            set
            {
                BackOfficeConnectionEnabledToReceive_Private = value;
                IsDirty = true;
                NotifyPropertyChanged("BackOfficeConnectionEnabledToReceive");
            }
        }
        /// <summary>
        /// Private version of above, just sets member var
        /// </summary>
        private bool BackOfficeConnectionEnabledToReceive_Private
        {
            get { return _backOfficeConnectionEnabledToReceive; }
            set { _backOfficeConnectionEnabledToReceive = value; }
        }
        bool _backOfficeConnectionEnabledToReceive;

        /// <summary>
        /// Returns enum associated with current active status of premise connection.
        /// </summary>
        public ConnectionActiveStatus BackOfficeConnectionStatus
        {
            get { return _backOfficeConnectionStatus; }
            set
            {
                _backOfficeConnectionStatus = value;
                NotifyPropertyChanged("BackOfficeConnectionStatus");
                NotifyPropertyChanged("BackOfficeConnectionStatusImage");
            }
        }
        ConnectionActiveStatus _backOfficeConnectionStatus = ConnectionActiveStatus.None;

        /// <summary>
        /// Returns and image associated with the status
        /// </summary>
        public Image BackOfficeConnectionStatusImage
        {
            get
            {
                return ActiveImage(BackOfficeConnectionStatus);
            }
        }

        /// <summary>
        /// pass thru for cloud company name
        /// </summary>
        public string CloudCompanyName
        {
            get { return _cloudCompanyName; }
            set
            {
                _cloudCompanyName = value;
                IsDirty = true;
                NotifyPropertyChanged("CloudCompanyName");
            }
        }
        string _cloudCompanyName;

        /// <summary>
        /// pass thru for cloud URL
        /// </summary>
        public string CloudCompanyUrl
        {
            get { return _cloudCompanyUrl; }
            set
            {
                _cloudCompanyUrl = value;
                IsDirty = true;
                NotifyPropertyChanged("CloudCompanyUrl");
            }
        }
        string _cloudCompanyUrl;

        /// <summary>
        /// Is a company selected for this configuration
        /// </summary>
        public bool BackOfficeCompanySelected
        {
            get{ return !string.IsNullOrEmpty(BackOfficeCompanyName);}
        }

        /// <summary>
        /// Pass thru for the back office "company" name
        /// </summary>
        public string BackOfficeCompanyName
        {
            get { return _backOfficeName; }
            set
            {
                _backOfficeName = value;
                IsDirty = true;
                NotifyPropertyChanged("BackOfficeCompanyName");
            }
        }
        string _backOfficeName;

        /// <summary>
        /// Are the current values in the backing database.
        /// </summary>
        public bool IsDirty
        {
            get { return _isDirty; }
            set { _isDirty = value; }
        }
        bool _isDirty;


        /// <summary>
        /// 
        /// </summary>
        public String ConnectorPluginId
        {
            get { return _connectorPluginId; }
            set
            {
                _connectorPluginId = value;
                IsDirty = true;
                NotifyPropertyChanged("ConnectorPluginId");
            }
        }
        String _connectorPluginId;

        /// <summary>
        /// 
        /// </summary>
        public string BackOfficeConnectionCredentials
        {
            get { return _backOfficeConnectionCredentials; }
            set
            {
                _backOfficeConnectionCredentials = value;
                IsDirty = true;
                NotifyPropertyChanged("BackOfficeConnectionCredentials");
            }
        }
        private String _backOfficeConnectionCredentials;


        /// <summary>
        /// 
        /// </summary>
        public string BackOfficeCompanyUniqueIndentifier
        {
            get { return _backOfficeCompanyUniqueIndentifier; }
            set
            {
                _backOfficeCompanyUniqueIndentifier = value;
                IsDirty = true;
                NotifyPropertyChanged("BackOfficeCompanyUniqueIndentifier");
            }
        }
        private String _backOfficeCompanyUniqueIndentifier;

        /// <summary>
        /// 
        /// </summary>
        public string CloudTenantClaim
        {
            get { return _cloudTenantClaim; }
            set
            {
                _cloudTenantClaim = value;
                IsDirty = true;
                NotifyPropertyChanged("CloudTenantClaim");
            }
        }

        private String _cloudTenantClaim;

        #endregion


        #region Constructors

        /// <summary>
        /// Create an empty ConfigurationViewModel
        /// </summary>
        public ConfigurationViewModel()
        {
            IsDirty = true;
        }

        /// <summary>
        /// Create a new ConfigurationViewModel
        /// </summary>
        /// <param name="repository"></param>
        public ConfigurationViewModel(PremiseConfigurationRecord repository)
        {
            InitializeFromPremiseConfigRecord(repository);
        }

        #endregion


        private void InitializeFromPremiseConfigRecord(PremiseConfigurationRecord record)
        {
            CloudPremiseKey = record.CloudPremiseKey;
            CloudTenantId = record.CloudTenantId;
            CloudEndpoint = record.SiteAddress;
            CloudConnectionEnabledToReceive = record.CloudConnectionEnabledToReceive;
            CloudConnectionEnabledToSend = record.CloudConnectionEnabledToSend;
            CloudCompanyUrl = record.CloudCompanyUrl;
            CloudCompanyName = record.CloudCompanyName;
            MinCommunicationFailureRetryInterval = record.MinCommunicationFailureRetryInterval;
            MaxCommunicationFailureRetryInterval = record.MaxCommunicationFailureRetryInterval;

            BackOfficeConnectionEnabledToReceive = record.BackOfficeConnectionEnabledToReceive;
            BackOfficeCompanyName = record.BackOfficeCompanyName;
            BackOfficeConnectionCredentials = record.BackOfficeConnectionCredentials;
            BackOfficeCompanyUniqueIndentifier = record.BackOfficeCompanyUniqueId;
            CloudTenantClaim = record.CloudTenantClaim;
            
            ConnectorPluginId = record.ConnectorPluginId;

            _repository = record;
            IsDirty = false;
        }

        /// <summary>
        /// Reverts all values to original.
        /// </summary>
        public void RevertConfigurationToOriginalValues()
        {
            InitializeFromPremiseConfigRecord(_repository);
            NotifyPropertyChanged("");
        }

        private Image ActiveImage(ConnectionActiveStatus status)
        {
            switch (status)
            {
                case ConnectionActiveStatus.Inactive:
                    return ResourcesViewModel.WhiteLight;
                case ConnectionActiveStatus.Broken:
                    return ResourcesViewModel.RedLight;
                case ConnectionActiveStatus.Active:
                    return ResourcesViewModel.GreenLight;
                default:
                    return ResourcesViewModel.BlankLight;
            }
        }

        /// <summary>
        /// Refresh Connection status
        /// </summary>
        public void RefreshConnectionStatuses()
        {
            using (var proxy = StateServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
            {
                RefreshConnectionStatuses(proxy.GetConnectorState());
            }
        }

        /// <summary>
        /// Refreshes connection state for Backoffice and Cloud
        /// </summary>
        /// <param name="connectorState"></param>
        public void RefreshConnectionStatuses(ConnectorState connectorState)
        {
            if (connectorState != null)
            {
                // get matching connection
                IntegratedConnectionState integratedConnectionState = null;
                foreach (var s in connectorState.IntegratedConnectionStates)
                {
                    if (s.TenantId == CloudTenantId)
                    {
                        integratedConnectionState = s;
                        break;
                    }
                }
               
                // Backoffice
                bool backOfficeConnectionEnabledToReceive, backOfficeUpdateRequired;
                BackOfficeConnectionStatus = ConnectionActiveStatusHelper.GetBackOfficeConnectionStatus(
                    integratedConnectionState, 
                    out backOfficeConnectionEnabledToReceive,
                    out backOfficeUpdateRequired);

                // Update stored backoffice enabled status
                // Use internal property since we don't want to affect dirty status
                BackOfficeConnectionEnabledToReceive_Private = backOfficeConnectionEnabledToReceive;
                
                // Cloud
                bool cloudConnectionEnabledToReceive, cloudConnectionEnabledToSend, connectorUpdateRequired;
                CloudConnectionStatus = ConnectionActiveStatusHelper.GetCloudConnectivityStatus(
                    connectorState,
                    integratedConnectionState,
                    out cloudConnectionEnabledToReceive,
                    out cloudConnectionEnabledToSend,
                    out connectorUpdateRequired);

                // Update stored cloud enabled statuses
                // Use internal properties since we don't want to affect dirty status
                CloudConnectionEnabledToReceive_Private = cloudConnectionEnabledToReceive;
                CloudConnectionEnabledToSend_Private = cloudConnectionEnabledToSend;
            }
        }

        /// <summary>
        /// Validate that the minimum fields are filled in and valid
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        public bool ValidateRequiredFields(ref List<string> messages)
        {
            bool valid = true;

            if (string.IsNullOrWhiteSpace(CloudTenantId))
            {
                messages.Add(string.Format(ResourcesViewModel.ConnectorDetails_RequiredFieldsErrorMessage, "connection key"));
                valid = false;
            }

            return valid;
        }

        /// <summary>
        /// Verify that the TenantId is not already in use.
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        public bool ValidateTenantIdUnique(ref List<string> messages)
        {
            bool valid = true;

            if (!CloudTenantId.Equals(_repository.CloudTenantId)
                && !string.IsNullOrEmpty(_repository.CloudTenantId))
            {
                messages.Add(ResourcesViewModel.ConnectorDetails_ToChangeWebsiteCreateNewConnection);
                valid = false;
            }
            else
            {
                if (string.IsNullOrEmpty(_repository.CloudTenantId))
                {
                    if (!ValidateTenantId())
                    {
                        messages.Add(ResourcesViewModel.ConnectorDetails_TenantIdAlreadyInUse);
                        valid = false;
                    }
                }
                if (!CloudPremiseKey.Equals(_repository.CloudPremiseKey))
                {
                    if (!ValidateConnectionKey())
                    {
                        messages.Add(ResourcesViewModel.ConnectorDetails_ConnectionKeyInUse);
                        valid = false;
                    }
                }
            }
            return valid;
        }

        /// <summary>
        /// Returns false if the company name has been selected.
        /// </summary>
        /// <param name="messages"></param>
        /// <returns></returns>
        public bool ValidateCompanyUnique(ref List<string> messages)
        {
            //TODO: JSB moved this to BackOfficeCompanyName, suspect this needs to be a back office unique id of some sort.
            if (String.IsNullOrEmpty(BackOfficeCompanyName))
                return true;

            if (!BackOfficeCompanyName.Equals(_repository.BackOfficeCompanyName))
            {
                ConfigurationViewModelManager _manager = new ConfigurationViewModelManager();
                _manager.FillList();

                foreach (ConfigurationViewModel model in _manager.Configurations)
                {
                    if (BackOfficeCompanyName.Equals(model.BackOfficeCompanyName))
                    {
                        messages.Add(ResourcesViewModel.ConnectorDetails_CompanyDatabaseInUse);
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Determines whether multiple connections can be made to the same tenant
        /// </summary>
        /// <returns></returns>
        public bool AllowMultipleCompanyConnections()
        {
            bool retval = false;
            string variableTest = Environment.GetEnvironmentVariable(SAGE_CONNECTOR_ALLOW_MULTIPLE_CONNECTIONS);
            if (!string.IsNullOrEmpty(variableTest) && variableTest.Equals("1"))
            {
                retval = true;
            }
            return retval;
        }

        private bool ValidateTenantId()
        {
            ConfigurationViewModelManager manager = new ConfigurationViewModelManager();
            return manager.ValidateTenantIdUnique(CloudTenantId);
        }

        private bool ValidateConnectionKey()
        {
            ConfigurationViewModelManager manager = new ConfigurationViewModelManager();
            return manager.ValidateConnectionKeyUnique(CompositeConnectionKey);
        }

        /// <summary>
        /// Save the record
        /// </summary>
        public void Save()
        {
            if (IsDirty)
            {
                string existingTenentId = _repository.CloudTenantId;
                _repository.CloudPremiseKey = CloudPremiseKey;
                _repository.CloudTenantId = CloudTenantId;
                _repository.SiteAddress = CloudEndpoint;
                _repository.CloudConnectionEnabledToReceive = CloudConnectionEnabledToReceive;
                _repository.CloudConnectionEnabledToSend = CloudConnectionEnabledToSend;
                _repository.CloudCompanyUrl = CloudCompanyUrl;
                _repository.CloudCompanyName = CloudCompanyName;
                _repository.MinCommunicationFailureRetryInterval = MinCommunicationFailureRetryInterval;
                _repository.MaxCommunicationFailureRetryInterval = MaxCommunicationFailureRetryInterval;

                _repository.BackOfficeConnectionEnabledToReceive = BackOfficeConnectionEnabledToReceive;
                _repository.BackOfficeCompanyName = BackOfficeCompanyName;
                _repository.BackOfficeConnectionCredentials = BackOfficeConnectionCredentials;
                _repository.BackOfficeCompanyUniqueId = BackOfficeCompanyUniqueIndentifier;
                _repository.CloudTenantClaim = CloudTenantClaim;

                //See if configuration already exits in database.
                if (ConfigurationSettingFactory.RetrieveConfiguration(existingTenentId) != null)
                {
                    if (!ConfigurationSettingFactory.UpdateConfiguration(_repository))
                    {
                        throw new Exception(ResourcesViewModel.ConnectorDetails_ConfigurationDataProblem);
                    }
                }
                else
                {
                    if (!ConfigurationSettingFactory.SaveNewTenant(_repository))
                    {
                        throw new Exception(ResourcesViewModel.ConnectorDetails_ConfigurationDataProblem);
                    }
                }
            }

            // Save complete
            IsDirty = false;
        }

        /// <summary>
        /// Update the view model from a premiseConfig record. This is ONLY valid
        /// if the premise configuration record is for the same tenant.
        /// This is intended for use in updating a record with fresh data from the database.
        /// If there is unsaved data in the record it will be lost.
        /// </summary>
        /// <param name="premiseConfig"></param>
        public void SynchronizeFromUpdatedPremiseConfiguration(PremiseConfigurationRecord premiseConfig)
        {
            string invalidOpMessage = "Can only Synchronize configuration view model with a PremiseConfigurationRecord the same tenant Id";
            if (CloudTenantId != premiseConfig.CloudTenantId)
                throw new InvalidOperationException(invalidOpMessage);

             InitializeFromPremiseConfigRecord(premiseConfig);

            //update the repository so any fields we do not touch here are properly carried along
            _repository = premiseConfig;

            //pretty everything could have changed so just say its all invalid.
            NotifyPropertyChanged("");
        }


        #region INotifyPropertyChanged Members

        /// <summary>
        /// PropertyChanged event
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }

        #endregion
    }
}
