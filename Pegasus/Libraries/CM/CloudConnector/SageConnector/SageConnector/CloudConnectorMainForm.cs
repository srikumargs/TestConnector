using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel.Security;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;
using Sage.Connector.Common;
using Sage.Connector.Configuration.Contracts.Data;
using Sage.Connector.Configuration.Mediator.JsonConverters;
using Sage.Connector.DomainMediator.Core.JsonConverters;
using Sage.Connector.Logging;
using Sage.Connector.Management;
using Sage.Connector.StateService.Interfaces.DataContracts;
using SageConnector.Internal;
using SageConnector.Properties;
using SageConnector.ViewModel;

namespace SageConnector
{
    /// <summary>
    /// Sage Cloud Connector Main Form
    /// </summary>
    public partial class CloudConnectorMainForm : Form
    {
        private ConfigurationViewModelManager _configManager;
        private ConnectorState _connectorState;
        private bool _showEndpointAddress;

        private Dictionary<FeatureMetadata, IList<PropertyDefinition>> _metaPropertyLists;

        private readonly ConnectorPluginsCollection _pluginsCollection = new ConnectorPluginsCollection();

        /// <summary>
        /// Cloud connector main form constructor
        /// </summary>
        public CloudConnectorMainForm()
        {
            InitializeComponent();
        }

        private void lnkHelpLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ConnectorUtilities.ShowMainFormHelp();
        }

        private void InitializeTenants(ref Boolean accessIsDenied)
        {
            accessIsDenied = false;
            Cursor origCursor = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                _configManager = new ConfigurationViewModelManager();
                if (null != _configManager)
                {
                    try
                    {
                        _configManager.FillList();
                        BindConfigurationDataGrid();
                        ApplyRowAlignment();
                        CheckStateAndEnableButtons();
                    }
                    catch (SecurityAccessDeniedException)
                    {
                        accessIsDenied = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Cursor.Current = origCursor;
                LogAndShowRefreshGridErrorMessage(ex);
            }
            finally
            {
                Cursor.Current = origCursor;
            }
        }

        private void ApplyRowAlignment()
        {
            for (Int32 i = 0; i < dgConnections.Rows.Count; i++)
            {
                dgConnections.Rows[i].Cells[0].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgConnections.Rows[i].Cells[4].Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }
        }

        private void BindConfigurationDataGrid()
        {
            dgConnections.AutoGenerateColumns = false;

            colPremiseCompany.DataPropertyName = "BackOfficeCompanyName";
            colPremiseConnectionStatus.DataPropertyName = "BackOfficeConnectionStatusImage";
            colTenantCompany.DataPropertyName = "CloudCompanyName";
            colTenantConnectionStatus.DataPropertyName = "CloudConnectionStatusImage";
            colConnectorLabel.DataPropertyName = "ConnectionImage";

            dgConnections.DataSource = _configManager.Configurations;
        }

        private void BindConfigurationDetails()
        {
            bool itemSelected = _configManager.Configurations.Count > 0;

            if (itemSelected)
            {
                txtPremiseCompanyName.DataBindings.Clear();
                txtPremiseDatabasePath.DataBindings.Clear();
                chkPremiseActiveStatusReceive.DataBindings.Clear();
                lnkTenantSiteUrl.DataBindings.Clear();
                txtTenantCompanyName.DataBindings.Clear();
                chkTenantActiveStatusReceive.DataBindings.Clear();
                chkTenantActiveStatusSend.DataBindings.Clear();
                txtPremiseCompanyName.DataBindings.Add(new Binding("Text", _configManager.CurrentConfiguration, "BackOfficeCompanyName"));
                //txtPremiseDatabasePath.DataBindings.Add(new Binding("Text", _configManager.CurrentConfiguration, "BackOfficeConnectionInformationDisplayable"));
                chkPremiseActiveStatusReceive.DataBindings.Add(new Binding("Checked", _configManager.CurrentConfiguration, "BackOfficeConnectionEnabledToReceive"));
                txtTenantCompanyName.DataBindings.Add(new Binding("Text", _configManager.CurrentConfiguration, "CloudCompanyName"));
                lnkTenantSiteUrl.DataBindings.Add(new Binding("Text", _configManager.CurrentConfiguration, "CloudCompanyUrl"));
                chkTenantActiveStatusReceive.DataBindings.Add(new Binding("Checked", _configManager.CurrentConfiguration, "CloudConnectionEnabledToReceive"));
                chkTenantActiveStatusSend.DataBindings.Add(new Binding("Checked", _configManager.CurrentConfiguration, "CloudConnectionEnabledToSend"));

            }
        }

        private IDictionary<FeatureMetadata, IList<PropertyDefinition>> GetFeatureConfigurationProperties()
        {
            if (_metaPropertyLists != null) return _metaPropertyLists;

            FeatureResponse featureResponse = MakeFeatureRequest("Getting feature configuration properties",
                "GetFeatureConfigurationProperties", "");

            if (featureResponse.UserFacingMessages.Any())
            {
                StringBuilder sb = new StringBuilder();
                foreach (string msg in featureResponse.UserFacingMessages)
                {
                    sb.AppendLine(msg);
                }
                string errorMsgs = sb.ToString();

                MessageBox.Show(this, errorMsgs,
                 Resources.ConnectorCommon_ErrorCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

            if (featureResponse.RawErrorMessage.Any())
            {
                using (var lm = new LogManager())
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine(string.Empty);
                    foreach (string msg in featureResponse.RawErrorMessage)
                    {
                        sb.AppendLine(msg);
                    }
                    string errorMsgs = sb.ToString();
                    string logMessage = string.Format("Raw error messages from MakeFeatureRequest:{0}", errorMsgs);
                    lm.WriteError(this, logMessage);
                }

            }

            var cfg = new DomainMediatorJsonSerializerSettings
            {
                ContractResolver = new DictionaryFriendlyContractResolver()
            };
            cfg.Converters.Add(new AbstractDataTypeConverter());
            cfg.Converters.Add(new AbstractSelectionValueTypesConverter());
            var featurePropDefsList =
                JsonConvert.DeserializeObject<IList<KeyValuePair<FeatureMetadata, IList<PropertyDefinition>>>>(
                    featureResponse.Payload, cfg);


            _metaPropertyLists = (featurePropDefsList != null)
                                    ? featurePropDefsList.ToDictionary(x => x.Key, y => y.Value)
                                    : new Dictionary<FeatureMetadata, IList<PropertyDefinition>>();
            return _metaPropertyLists;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="featureId"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        private FeatureResponse MakeFeatureRequest(string caption, string featureId, string payload)
        {
            using (var serviceWorker = new ServiceWorkerWithProgress(this))
            {
                FeatureResponse featureResponse = serviceWorker.FeatureRequest(caption,
                                                                               _configManager.CurrentConfiguration.ConnectorPluginId,
                                                                               _configManager.CurrentConfiguration.BackOfficeConnectionCredentials,
                                                                               featureId,
                                                                               _configManager.CurrentConfiguration.CloudTenantId,
                                                                               payload);
                return featureResponse;
            }
        }
        private void CheckStateAndEnableButtons()
        {
            bool areItemsInList = false;
            bool isItemSelected = false;

            if (_configManager != null)
            {
                isItemSelected = (_configManager.CurrentConfiguration != null);

                if (_configManager.Configurations != null)
                {
                    areItemsInList = (_configManager.Configurations.Count > 0);
                }
            }

            btnDeleteConnection.Enabled = isItemSelected;
            btnEditConnection.Enabled = isItemSelected;
            btnFeatureConfigurations.Enabled = isItemSelected;
            btnManageRequests.Enabled = isItemSelected;

            btnRefresh.Enabled = areItemsInList;


        }

        private void dgConnections_SelectionChanged(object sender, EventArgs e)
        {
            ConfigurationViewModel selectedConfig = GetCurrentRowDataBoundItem();
            if (selectedConfig != null)
            {
                _configManager.SetCurrent(selectedConfig.CloudTenantId);
                BindConfigurationDetails();
            }

            CheckStateAndEnableButtons();
        }

        private void btnCloseConnector_Click(object sender, EventArgs e)
        {
            CloseApplication();
        }

        private void btnAddConnection_Click(object sender, EventArgs e)
        {
            //TODO: GetConnectorPluginsCollection is getting called 2x. Look at if they both need to be in place.
            GetConnectorPluginsCollection();
            if (!ConnectorViewModel.IsAnyConnectableBackOfficeAvailable())
            {
                using (var form = new NoConnectableBackOfficeAvailableForm())
                {
                    form.ShowDialog(this);
                }
            }
            else
            {
                ShowDetailsForm(false);
            }
        }
        private void btnManageRequests_Click(object sender, EventArgs e)
        {
            var connectorPlugin = GetConnectorPlugin();

            if (!ConnectorViewModel.IsConnectorPluginAvailable(connectorPlugin))
            {
                using (var form = new BackOfficeNotAvailableForm(connectorPlugin))
                {
                    form.ShowDialog(this);
                }
            }
            else
            {
                if (dgConnections.Rows.Count > 0)
                    ShowRequestListForm();
            }
        }


        private void btnEditConnection_Click(object sender, EventArgs e)
        {


            var connectorPlugin = GetConnectorPlugin();

            if (!ConnectorViewModel.IsConnectorPluginAvailable(connectorPlugin))
            {
                using (var form = new BackOfficeNotAvailableForm(connectorPlugin))
                {
                    form.ShowDialog(this);
                }
            }
            else
            {
                if (dgConnections.Rows.Count > 0)
                    ShowDetailsForm(true);
            }
        }

        private ConnectorPlugin GetConnectorPlugin()
        {
            return ConnectorPluginsViewModel.GetConnectorPlugins()
                .SingleOrDefault(x => x.Id == _configManager.CurrentConfiguration.ConnectorPluginId);

        }

        private void btnDeleteConnection_Click(object sender, EventArgs e)
        {
            if (dgConnections.Rows.Count > 0)
                DeleteCurrentConfiguration();
        }

        private void DeleteCurrentConfiguration()
        {
            // Prevent the refresh data get thread from doing any work
            lock (_refreshSyncObject)
            {
                // Want to re-get any existing refresh data when we're done
                _connectorStateForRefresh = null;

                DialogResult result;
                GetConnectorPluginsCollection();
                var connectorPlugin = GetConnectorPlugin();

                if (!ConnectorViewModel.IsConnectorPluginAvailable(connectorPlugin))
                {
                    using (var form = new BackOfficeNotAvailableForm(connectorPlugin))
                    {
                        form.ShowDialog(this);
                    }
                }
                else
                {
                    var admin = GetAdminCredentialsNeeded(connectorPlugin.Id);
                    using (var adminLogin = new CloudConnectorAdminLoginForm(connectorPlugin, _configManager.CurrentConfiguration, admin, true))
                    {
                        result = adminLogin.ShowDialog();
                    }
                    if (result == DialogResult.OK)
                        RefreshConnectionGrid();
                }
            }
        }


        private void ShowRequestListForm()
        {
            BackOfficeConnectionsForCredentialsResponse backOfficeConnectionsForCredentialsResponse;
            IDictionary<string, string> managementCredentials;
            lock (_refreshSyncObject)
            {
                GetConnectorPluginsCollection();
                var connectorPlugin = GetConnectorPlugin();

                if (!ConnectorViewModel.IsConnectorPluginAvailable(connectorPlugin))
                {
                    using (var form = new BackOfficeNotAvailableForm(connectorPlugin))
                    {
                        form.ShowDialog(this);
                    }
                }
                else
                {
                    //Need to separate backOfficeConnectionsForCredentialsResponse out. Do not need to take the time to get it here, or the company list its flowing back.
                    bool okToShowForm = IsOkToShowForm(connectorPlugin, out managementCredentials, out backOfficeConnectionsForCredentialsResponse);
                    if (okToShowForm)
                    {
                        ConnectionRequestListForm requestListForm = null;
                        try
                        {
                            requestListForm = new ConnectionRequestListForm(_configManager.CurrentConfiguration);

                            requestListForm.ShowDialog();
                        }
                        finally
                        {
                            if (requestListForm != null)
                            {
                                requestListForm.Dispose();
                            }
                        }
                    }
                }
            }
        }

        private ConnectorPluginsCollection GetConnectorPluginsCollection()
        {
            if (_pluginsCollection.Any())
                return _pluginsCollection;

            //currently we do not actually use the errors. Needs to be valued up to show to the user.
            //we just fill the list from the response
#pragma warning disable 219
            String[] errors;
#pragma warning restore 219

            try
            {
                BackOfficePluginsResponse backOfficePluginsResponse;

                ConnectorPluginsCollection pluginsFound = new ConnectorPluginsCollection();

                using (var serviceWorker = new ServiceWorkerWithProgress(this))
                {
                    backOfficePluginsResponse = serviceWorker.BackOfficePlugins(
                        "Getting back office plugins");

                    bool userCancelled = serviceWorker.UserCancelled;
                    if (!userCancelled)
                    {
                        BackOfficePlugin[] backOfficePlugins = ConnectorViewModel.ProcessBackOfficePluginsResponse(backOfficePluginsResponse);
                        foreach (var backOfficePlugin in backOfficePlugins)
                        {
                            pluginsFound.Add(new ConnectorPlugin(backOfficePlugin.PluginId,
                                backOfficePlugin.BackofficeProductName,
                                backOfficePlugin.Platform,
                                backOfficePlugin.HelpUri, //move to cloud side?
                                backOfficePlugin.LoginAdministratorTerm, //  remove or add to back office config metadata
                                "relative location remove", //remove
                                "plugin root remove", // - add to backoffice config metadata or remove
                                backOfficePlugin.ApplicationSecurityMode, //TODO determine this one
                                backOfficePlugin.BackOfficeVersion,
                                backOfficePlugin.BackOfficePluginAutoUpdateProductId,
                                backOfficePlugin.BackOfficePluginAutoUpdateProductVersion,
                                backOfficePlugin.BackOfficePluginAutoUpdateComponentBaseName
                                ));
                        }
                    }
                }

                //TODO: add logging to dump plugin list to logs possibly.

                //filter for only the plugins that are installed.
                foreach (var plugin in pluginsFound)
                {
                    if(plugin.IsInstalled)
                    _pluginsCollection.Add(plugin);
                }
                
            }
            catch (Exception)
            {
// ReSharper disable once RedundantAssignment
                errors = new[] { Resources.ConnectorLogin_NoResponseFromBackoffice };
               
                //TODO KMS: show list of error messages like the  admin login form
            }
            ConnectorPluginsViewModel.SetConnectorPlugins(_pluginsCollection);

            return _pluginsCollection;
        }

        private ConnectorPlugin GetConnectorPluginToUse(bool forEdit, ConfigurationViewModel currentConfig, out Boolean userCancelled)
        {
            userCancelled = false;
            ConnectorPlugin result = null;
            if (forEdit)
            {
                result = GetConnectorPluginsCollection().SingleOrDefault(x => x.Id == currentConfig.ConnectorPluginId);
            }
            else
            {
                var availableConnectorPlugins = GetConnectorPluginsCollection();
                if (availableConnectorPlugins.Any())
                {
                    if (availableConnectorPlugins.Count > 1)
                    {
                        using (var form = new SelectBackOfficeToConnectToForm(availableConnectorPlugins))
                        {
                            if (form.ShowDialog(this) == DialogResult.OK)
                            {
                                result = form.SelectedConnectorPlugin;
                            }
                            else
                            {
                                userCancelled = true;
                            }
                        }
                    }
                    else
                    {
                        result = availableConnectorPlugins.First();
                    }
                }
            }

            return result;
        }


        private void ShowFeatureConfigurationsForm()
        {
            BackOfficeConnectionsForCredentialsResponse backOfficeConnectionsForCredentialsResponse;
            IDictionary<string, string> managementCredentials;

            // Prevent the refresh data get thread from doing any work
            lock (_refreshSyncObject)
            {

                Boolean userCancelled;
                var connectorPlugin = GetConnectorPluginToUse(true, _configManager.CurrentConfiguration, out userCancelled);
                if (connectorPlugin != null)
                {
                    // Check if it's ok for us to open the details form
                    // This will open the admin login form
                    bool okShowDetails = IsOkToShowForm(connectorPlugin, out managementCredentials, out backOfficeConnectionsForCredentialsResponse);
                    if (okShowDetails)
                    {

                        using (var configurationFeaturesForm = new CloudConnectorFeatureConfigurationsForm(
                            _configManager.CurrentConfiguration,
                            null,
                            _metaPropertyLists))
                        {

                            configurationFeaturesForm.ShowDialog();
                        }
                    }

                }
                else
                {
                    if (!userCancelled)
                    {
                        using (var form = new NoConnectableBackOfficeAvailableForm())
                        {
                            form.ShowDialog(this);
                        }
                    }
                }

            }
        }

        private void ShowDetailsForm(bool forEdit)
        {
            string returnTenantId = null;
            bool removeNewConfig = false;
            ConfigurationViewModel newConfig = null;
            DialogResult result;
            BackOfficeConnectionsForCredentialsResponse backOfficeConnectionsForCredentialsResponse;
            IDictionary<string, string> managementCredentials;

            // Prevent the refresh data get thread from doing any work
            lock (_refreshSyncObject)
            {
                // Want to re-get any existing refresh data when we're done
                _connectorStateForRefresh = null;

                Boolean userCancelled;
                var connectorPlugin = GetConnectorPluginToUse(forEdit, _configManager.CurrentConfiguration, out userCancelled);
                if (connectorPlugin != null)
                {
                    //we are about to go into the edit path.
                    string backOfficeId = connectorPlugin.Id;
                    string auProductId = connectorPlugin.PluginProductId;
                    string auProductVersion = connectorPlugin.PluginProductVersion;
                    string auComponentBaseName = connectorPlugin.PluginComponetBaseName;

                    bool auPartsPresent = (
                        !String.IsNullOrWhiteSpace(backOfficeId)
                        && !String.IsNullOrWhiteSpace(auProductId)
                        && !String.IsNullOrWhiteSpace(auProductVersion)
                        && !String.IsNullOrWhiteSpace(auComponentBaseName)
                        );

                    if(auPartsPresent)
                    { 
                        ConfigurationHelpers.DownloadBackOfficePlugin(backOfficeId, auProductId, auProductVersion, auComponentBaseName);
                    }

                    // Check if it's ok for us to open the details form
                    // This will open the admin login form
                    bool okShowDetails = IsOkToShowForm(connectorPlugin, out managementCredentials, out backOfficeConnectionsForCredentialsResponse);
                    if (okShowDetails)
                    {
                        //TODO: JSB move this conversion to view model?
                        var currentConfig = _configManager.CurrentConfiguration;
                        string connectionCredentialsString = (currentConfig == null ? null : currentConfig.BackOfficeConnectionCredentials);
                        
                        IDictionary<string, string> connectionCredentials;
                        if (String.IsNullOrEmpty(connectionCredentialsString) || !forEdit)
                        {
                            connectionCredentials = new Dictionary<string, string>();
                        }
                        else
                        {
                            connectionCredentials = JsonConvert.DeserializeObject<IDictionary<string, string>>(connectionCredentialsString);    
                        }
                        
                        //get connection details
                        var connectionDetails = GetConnectionCredentialsNeeded(connectorPlugin.Id, managementCredentials, connectionCredentials);
                        
                        CloudConnectorDetailForm configurationDetailForm = null;
                        try
                        {
                            if (forEdit)
                            {
                                configurationDetailForm = new CloudConnectorDetailForm(_configManager.CurrentConfiguration, connectionDetails, false, _showEndpointAddress);
                            }
                            else
                            {
                                newConfig = _configManager.CreateNewTenant(connectorPlugin);
                                configurationDetailForm = new CloudConnectorDetailForm(newConfig, connectionDetails, true, _showEndpointAddress);
                                _configManager.Configurations.Add(newConfig);
                                removeNewConfig = true;
                            }

                            configurationDetailForm.BackOfficeConnectionsForCredentialsResponse = backOfficeConnectionsForCredentialsResponse;

                            result = configurationDetailForm.ShowDialog();
                            if (result == DialogResult.OK)
                            {
                                returnTenantId = configurationDetailForm.TenantId;
                                removeNewConfig = false;
                            }
                        }
                        finally
                        {
                            if (configurationDetailForm != null)
                            {
                                configurationDetailForm.Dispose();
                            }

                            if (removeNewConfig)
                            {
                                _configManager.Configurations.Remove(newConfig);
                            }
                        }

                        if (result == DialogResult.OK)
                        {
                            RefreshConnectionGrid(returnTenantId);
                            RefreshConnectorState();
                            _detailsFormId = Guid.NewGuid();
                        }
                    }
                }
                else
                {
                    if (!userCancelled)
                    {
                        using (var form = new NoConnectableBackOfficeAvailableForm())
                        {
                            form.ShowDialog(this);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Check if it's ok to open the details form by checking admin login.
        /// Make sure refresh thread is suspended before calling this to prevent any
        /// Skipping in the progress bar.
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="managementCredentials"></param>
        /// <param name="backOfficeConnectionsForCredentialsResponse"></param>
        /// <returns></returns>
        private bool IsOkToShowForm(ConnectorPlugin plugin, out IDictionary<string, string> managementCredentials, out BackOfficeConnectionsForCredentialsResponse backOfficeConnectionsForCredentialsResponse)
        {
            bool okShowDetails = false;
            backOfficeConnectionsForCredentialsResponse = null;
            managementCredentials = null;

            var admin = GetAdminCredentialsNeeded(plugin.Id);

            using (var adminLogin = new CloudConnectorAdminLoginForm(plugin, _configManager.CurrentConfiguration, admin, false))
            {
                adminLogin.ShowDialog();
                if (adminLogin.DialogResult == DialogResult.OK && adminLogin.AdministratorConfirmed)
                {
                    backOfficeConnectionsForCredentialsResponse = adminLogin.BackOfficeConnectionsForCredentialsResponse;
                    managementCredentials = adminLogin.ManagementCredentials;
                    okShowDetails = true;
                }
            }

            return okShowDetails;
        }

        private void RefreshConnectionGrid()
        {
            RefreshConnectionGrid(null);
        }

        private ConfigurationViewModel GetCurrentRowDataBoundItem()
        {
            ConfigurationViewModel item = null;
            try
            {
                if (dgConnections.CurrentRow != null && dgConnections.CurrentRow.DataBoundItem != null)
                {
                    item = (dgConnections.CurrentRow.DataBoundItem as ConfigurationViewModel);
                }
            }
            catch (IndexOutOfRangeException)
            {
                // If the rows have been cleared, for example as a result of the hosting framework going down
                // Then we will get this exception
            }

            return item;
        }

        private void RefreshConnectionGrid(string tenantId)
        {
            //Save off the item currently selected
            bool firstItem = dgConnections.Rows.Count < 1;
            string selectedItem = "";

            if (tenantId != null)
            {
                selectedItem = tenantId;
            }
            else
            {
                ConfigurationViewModel selectedConfig = GetCurrentRowDataBoundItem();
                if (!firstItem && selectedConfig != null)
                    selectedItem = selectedConfig.CloudTenantId;
            }

            Cursor origCursor = Cursor.Current;
            try
            {
                if (_configManager != null)
                {
                    Cursor.Current = Cursors.WaitCursor;
                    _configManager.FillList();
                    ApplyRowAlignment();

                    if (!firstItem)
                        ReselectCurrentRow(selectedItem);
                    CheckStateAndEnableButtons();
                }
            }
            catch (Exception ex)
            {
                Cursor.Current = origCursor;
                dgConnections.Rows.Clear();  //If error occurs, rows may be invalid.  Clear existing rows.

                LogAndShowRefreshGridErrorMessage(ex);
            }
            finally
            {
                Cursor.Current = origCursor;
            }
        }

        private void LogAndShowRefreshGridErrorMessage(Exception ex)
        {
            using (var logger = new SimpleTraceLogger())
            {
                logger.WriteCriticalWithEventLogging(this, "Windowing", "Error refreshing connections: " + ex.ExceptionAsString());
            }

            MessageBox.Show(this, string.Format(
                Resources.ConnectorMain_ErrorRefreshingConnectionList, Environment.NewLine, ex.Message),
                Resources.ConnectorCommon_ErrorCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ReselectCurrentRow(string tenantId)
        {
            if (dgConnections.Rows.Count < 1)
            {
                ClearDetails();
                return;
            }

            foreach (DataGridViewRow row in dgConnections.Rows)
            {
                ConfigurationViewModel rowModel = row.DataBoundItem as ConfigurationViewModel;
                if (rowModel != null && rowModel.CloudTenantId.Equals(tenantId))
                {
                    row.Selected = true;
                    _configManager.SetCurrent(tenantId);
                    BindConfigurationDetails();
                    break;
                }
            }
        }

        //Clear details if no connections left
        private void ClearDetails()
        {
            //Note: this is still needed with the current binding code
            txtPremiseDatabasePath.Text = String.Empty;
            txtTenantCompanyName.Text = String.Empty;
            lnkTenantSiteUrl.Text = String.Empty;
            txtPremiseCompanyName.Text = String.Empty;
        }

        /// <summary>
        /// Verify services are started.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            ConfigureDiagnostics();
            Boolean accessIsDenied = false;
            InitializeTenants(ref accessIsDenied);
            if (accessIsDenied)
            {
                MessageBox.Show(string.Format("You must be logged on as an administrator or be a member of the Administrators group on this computer to run {0}.",
                    ConnectorViewModel.MainFormTitle), @"Administrators Only", MessageBoxButtons.OK, MessageBoxIcon.Error);
                CloseApplication();
                return;
            }

            SetFormTitlesBaseOnProduct();
            SetFormVersionNumber();

            // Default update links to not visible
            lnkConnectorUpdate.Visible = false;
            lnkBackofficeUpdate.Visible = false;
            lnkBackofficeInstall.Visible = false;

            // Do an initial setup of the statuses
            // The refresh process will be set up below to handle periodic updates
            RefreshConnectorState();
            RefreshAllConnectionStatuses();

            // set the backoffice & skyfile column headers to align with the lights
            dgConnections.Columns[0].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgConnections.Columns[0].SortMode = DataGridViewColumnSortMode.NotSortable;
            dgConnections.Columns[4].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgConnections.Columns[4].SortMode = DataGridViewColumnSortMode.NotSortable;

            // Start processes to get and update the UI with refreshed data
            // The thread will get refresh data without interfering with the UI
            // Then the forms timer will check for refreshed data and perform the quicker operation
            // Of swapping it in to the UI
            StartGetRefreshDataThread();
            SetupApplyRefreshDataProcess();
        }

        private void ConfigureDiagnostics()
        {
            _showEndpointAddress = DeveloperFlags.ShowEndPointAddress();
            if (_showEndpointAddress)
            {
                //NOTE: We no longer have in house only controls on this page. Left stub incase they come back.
            }
        }

        //Updating titles based on plugged in product
        private void SetFormTitlesBaseOnProduct()
        {
            Text = ConnectorViewModel.MainFormTitle;
            colPremiseCompany.HeaderText = string.Format(Resources.ConnectorMain_BackofficeColumnHeader,
                ConnectorViewModel.BackOfficeProductTermSentenceCaps);
            grpCompanyInformation.Text = string.Format(Resources.ConnectorMain_GrpBoxLabelBackofficeConnectionDetails,
                ConnectorViewModel.BackOfficeProductTermSentenceCaps);
        }

        private void SetFormVersionNumber()
        {
            String versionNumber;
            versionNumber = ConnectorViewModel.ApplicationVersion();
            if (DeveloperFlags.ShowEndPointAddress())
            {
                versionNumber = String.Format(CultureInfo.InvariantCulture, "{0} {1}", versionNumber, ConnectorViewModel.GetBuildSourceInformation());
            }
            lblVersionNumber.Text = versionNumber;
        }

        private void dgConnections_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (dgConnections.Rows.Count > 0)
                ShowDetailsForm(true);
        }

        private void CloseApplication()
        {
            TerminateGetRefreshDataThread();
            Close();
        }

        private void lnkConnectorUpdate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (_connectorState != null && _connectorState.ConnectorUpdateInfo != null)
            {
                // Prevent the refresh data get thread from doing any work
                lock (_refreshSyncObject)
                {
                    // Want to re-get any existing refresh data when we're done
                    _connectorStateForRefresh = null;

                    using (var form = new ConnectorUpdateForm(_connectorState))
                    {
                        form.ShowDialog(this);
                    }
                }
            }
            else
            {
                string text = Resources.ConnectorSimpleConnectorUpdateRequired_Message;
                string caption = Resources.ConnectorSimpleConnectorUpdateRequired_Caption;
                MessageBoxIcon icon = MessageBoxIcon.Error;

                if (_connectorState != null && _connectorState.ConnectorUpdateStatus == ConnectorUpdateStatus.UpdateAvailable)
                {
                    text = Resources.ConnectorSimpleConnectorUpdateAvailable_Message;
                    caption = Resources.ConnectorSimpleConnectorUpdateAvailable_Caption;
                    icon = MessageBoxIcon.Information;
                }

                MessageBox.Show(text, caption, MessageBoxButtons.OK, icon);
            }
        }

        private void lnkBackofficeUpdate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string text = Resources.ConnectorBackofficeUpdateRequired_Message;
            string caption = Resources.ConnectorBackofficeUpdateRequired_Caption;
            MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void lnkTenantSiteUrl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Uri uri;
            if (Uri.TryCreate(lnkTenantSiteUrl.Text, UriKind.Absolute, out uri))
            {
                ConnectorUtilities.ShowTenantSite(uri);
            }
        }

        private void lnkBackofficeInstall_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Prevent the refresh data get thread from doing any work
            lock (_refreshSyncObject)
            {
                using (var form = new BackOfficeNotAvailableForm())
                {
                    form.ShowDialog(this);
                }
            }
        }


        #region Refresh Related Methods

        /// <summary>
        /// Handler for refresh button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            // Prevent the refresh get thread from doing any work
            lock (_refreshSyncObject)
            {
                // Want to re-get any existing refresh data when we're done
                _connectorStateForRefresh = null;

                RefreshConnectionGrid();
                ValidateAndUpdateActiveStatusOnAllConnections();
                lblLastRefresh.Text = DateTime.Now.ToLocalTime().ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Update all connections statuses
        /// Should only be called by the refresh button because the detail form updates all of this on Save().
        /// Note: make sure any call to this stops the refresh thread first, since we don't want to interfere
        /// With the progress bar
        /// </summary>
        private void ValidateAndUpdateActiveStatusOnAllConnections()
        {
            try
            {
                using (var serviceWorker = new ServiceWorkerWithProgress(this))
                {
                    serviceWorker.ValidateAndUpdateActiveStatusOnAllConnections(
                        "Refresh",
                        _configManager.Configurations);
                }
            }
            catch (Exception)
            {
                MessageBox.Show(
                    this, Resources.ConnectorMain_ErrorUpdatingActiveStatusMessages,
                    Resources.ConnectorMain_ErrorUpdatingActiveStatusCaption,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            // Refresh the grid view
            dgConnections.Refresh();
        }

        /// <summary>
        /// Get the current connector state from the state service
        /// </summary>
        private void RefreshConnectorState()
        {
            Cursor cursorBefore = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                _connectorState = ConnectorViewModel.GetConnectorState();
                UpdateForConnectorState();
            }
            finally
            {
                Cursor.Current = cursorBefore;
            }
            lblLastRefresh.Text = DateTime.Now.ToLocalTime().ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Update links based on current connector state
        /// </summary>
        private void UpdateForConnectorState()
        {
            if (_connectorState == null) return;

            ConnectorUpdateStatus status = _connectorState.ConnectorUpdateStatus;
            switch (status)
            {
                case ConnectorUpdateStatus.UpdateAvailable:
                    lnkConnectorUpdate.Text = Resources.ConnectorMain_LinkConnectorUpdateAvailable;
                    lnkConnectorUpdate.Visible = true;
                    break;

                case ConnectorUpdateStatus.UpdateRequired:
                    lnkConnectorUpdate.Text = Resources.ConnectorMain_LinkConnectorUpdateRequired;
                    lnkConnectorUpdate.Visible = true;
                    break;

                case ConnectorUpdateStatus.None:
                    lnkConnectorUpdate.Visible = false;
                    break;
            }



            bool backOfficeUpdateRequired = false;
            if (_connectorState != null && _connectorState.IntegratedConnectionStates != null)
            {
                // this computes whether _any_ back office connection has indicated it is incompatible
                backOfficeUpdateRequired =
                    _connectorState.IntegratedConnectionStates.Any(
                        x => x.BackOfficeConnectivityStatus == BackOfficeConnectivityStatus.Incompatible);
            }
            if (backOfficeUpdateRequired)
            {
                // tell the user that one (or more) back office connections has indicated it is incompatible;  it would probably
                // be a better user experience to tell them specifically which 
                lnkBackofficeUpdate.Text = String.Format(Resources.ConnectorMain_LinkBackOfficeUpdateRequired,
                    ConnectorViewModel.BackOfficeProductTermSentenceCaps);
            }
            lnkBackofficeUpdate.Visible = backOfficeUpdateRequired;
            var currentRow = GetCurrentRowDataBoundItem();
            if (currentRow != null)
            {
                ReselectCurrentRow(currentRow.CloudTenantId);
            }
            UpdateSystemMessages();
        }


        /// <summary>
        /// Update the system messages displayed
        /// </summary>
        private void UpdateSystemMessages()
        {
            tbSystemMessages.Clear();
            if ((null != _connectorState) && (null != _connectorState.SubsystemHealthMessages))
            {
                Dictionary<Subsystem, uint> subSytemIssueCount = new Dictionary<Subsystem, uint>();

                // Iterate backwards to get the most current sub-system health first
                for (int index = _connectorState.SubsystemHealthMessages.Length - 1;
                    index >= 0;
                    index--)
                {
                    SubsystemHealthMessage message = _connectorState.SubsystemHealthMessages[index];

                    // Limit the number of displayed issues a sub system can raise
                    if (!subSytemIssueCount.ContainsKey(message.Subsystem))
                    {
                        subSytemIssueCount[message.Subsystem] = 0;
                    }
                    if (++subSytemIssueCount[message.Subsystem] <= ConnectorRegistryUtils.SubsystemHealthDisplayLimit)
                    {
                        String messageLine = String.Format(
                            CultureInfo.CurrentCulture,
                            Resources.ConnectorMain_SystemMessageListBoxRow,
                            message.Subsystem.ToString(),
                            message.TimestampUtc.ToLocalTime().ToShortDateString(),
                            message.TimestampUtc.ToLocalTime().ToLongTimeString(),
                            message.UserFacingMessage);

                        tbSystemMessages.Text += messageLine + Environment.NewLine;
                    }
                }
            }
        }

        #endregion

        #region Timer To Populate UI With Refresh Data

        /// <summary>
        /// Set up and kick off the apply refresh data timer
        /// </summary>
        private void SetupApplyRefreshDataProcess()
        {
            _applyRefreshDataTimer = new System.Windows.Forms.Timer();
            _applyRefreshDataTimer.Tick += ApplyRefreshDataHandler;
            _applyRefreshDataTimer.Interval = ConnectorRegistryUtils.ConnectorApplyRefreshDataInterval;
            _applyRefreshDataTimer.Start();
        }

        /// <summary>
        /// Coordinates the execution of the method to refresh connection statuses
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplyRefreshDataHandler(object sender, EventArgs e)
        {
            try
            {
                // Disable timer for debug purposes, in case someone
                // Sets the interval to be very short
                _applyRefreshDataTimer.Enabled = false;

                // Call the actual apply refresh data method
                ApplyRefreshData();
            }
            finally
            {
                // Re-enable when complete
                _applyRefreshDataTimer.Enabled = true;
            }
        }

        /// <summary>
        /// Does the actual work for the periodic refresh
        /// By applying the refresh data object that the get thread populated
        /// To the actual ConnectorState object that we use
        /// </summary>
        private void ApplyRefreshData()
        {
            // Update with any available refresh data obtained by the refresh thread
            // Only do a best attempt the get the sync object, since we don't want to
            // Hold up the UI thread for any reason.  Otherwise this could block if the 
            // Get thread currently held the lock.
            if (Monitor.TryEnter(_refreshSyncObject, 0))
            {
                try
                {
                    if (_connectorStateForRefresh != null)
                    {
                        // Do the update and reset the refresh data
                        // Note: don't update the last refresh timestamp for the background refresher
                        _connectorState = _connectorStateForRefresh;
                        _connectorStateForRefresh = null;
                        UpdateForConnectorState();

                        // Refresh the UI with our new states
                        RefreshAllConnectionStatuses();
                    }
                }
                catch (Exception)
                {
                    // Eat all exceptions
                }
                finally
                {
                    // Make sure we no longer hold the lock
                    Monitor.Exit(_refreshSyncObject);
                }
            }
        }

        private void RefreshAllConnectionStatuses()
        {
            // Update the ball state for all connections from the state service
            foreach (var model in _configManager.Configurations)
            {
                model.RefreshConnectionStatuses(_connectorState);
            }
        }

        /// <summary>
        /// Timer to execute periodic application of refresh data, if available
        /// Note: This MUST be a Forms.Timer, since we want it to execute on the 
        /// Application's UI thread.  Also, it does not pre-empt executing application code.
        /// </summary>
        private System.Windows.Forms.Timer _applyRefreshDataTimer;

        #endregion


        #region Thread To Get Refresh Data

        /// <summary>
        /// Setup and start the thread that will get refresh data
        /// </summary>
        private void StartGetRefreshDataThread()
        {
            _terminateGetRefreshDataThreadEvent = new ManualResetEvent(false);
            _getRefreshDataThread = new Thread(GetRefreshDataWorker);
            _getRefreshDataThread.IsBackground = true;
            _getRefreshDataThread.Start();
        }

        /// <summary>
        /// A thread created for this function will periodically get the connector
        /// State from the State Service and update the UI
        /// </summary>
        private void GetRefreshDataWorker()
        {
            try
            {
                do
                {
                    // Attempt to get and then release the lock, since we never want to block the UI thread
                    // If we were able to obtain the lock, then no potentially conficting actions
                    // In the UI thread will be going on (details form open, refresh button clicked).
                    // If one of those actions starts after we release this lock, then we will catch 
                    // That on setting the refresh object, after the work has been done
                    if (CanObtainLock())
                    {
                        // Store the current details form timestamp
                        Guid tmpDetailsFormId = _detailsFormId;

                        // Get the current connector state
                        // This is the time consuming part of the get refresh action
                        // So do not hold a lock while we do it
                        ConnectorState tmpConnectorState = ConnectorViewModel.GetConnectorState();

                        // If we cannot get a lock now, then a conflicting action started while we
                        // Were fetching the data, in which case we do not want to populate the refresh data.
                        // If we can get in, then hold the lock for this quick action so that no 
                        // Conflicting action now starts up.
                        if (Monitor.TryEnter(_refreshSyncObject, 0))
                        {
                            try
                            {
                                // If the details form timestamp has changed, then that means we hit the
                                // Very unlikely edge case where after the above GetConnectorState() call began
                                // The user opened, edited and completed a save on the detail form.  In which case,
                                // Our tmpConnectorState is potentially out of date, so don't update the refresh
                                // Data object
                                if (tmpDetailsFormId.Equals(_detailsFormId))
                                {
                                    // Timestamps are equal, so no details form changes were made
                                    // Now we're OK to populate the refresh data object
                                    _connectorStateForRefresh = tmpConnectorState;
                                }
                            }
                            finally
                            {
                                // Make sure we no longer hold the lock
                                Monitor.Exit(_refreshSyncObject);
                            }
                        }
                    }
                }
                while (0 != WaitHandle.WaitAny(
                    new WaitHandle[] { _terminateGetRefreshDataThreadEvent },
                    ConnectorRegistryUtils.ConnectorGetRefreshDataInterval));
            }
            catch (Exception)
            {
                // Use a regular lock here because we always want to unset the data
                // In case of an exception
                lock (_refreshSyncObject)
                {
                    // Unset the refresh data
                    _connectorStateForRefresh = null;
                }
            }
        }

        /// <summary>
        /// Encapsulate check on whether we can get a lock on the refresh sync object
        /// Releases the lock immediately if it got it.
        /// </summary>
        /// <returns></returns>
        private bool CanObtainLock()
        {
            bool result = false;
            if (Monitor.TryEnter(_refreshSyncObject, 0))
            {
                Monitor.Exit(_refreshSyncObject);
                result = true;
            }

            return result;
        }

        /// <summary>
        /// Kill the get refresh data thread
        /// </summary>
        private void TerminateGetRefreshDataThread()
        {
            if (_terminateGetRefreshDataThreadEvent != null)
            {
                _terminateGetRefreshDataThreadEvent.Set();
            }

            if (_getRefreshDataThread != null)
            {
                // Try to wait for completion
                if (!_getRefreshDataThread.Join(ConnectorRegistryUtils.ConnectorGetRefreshDataThreadTimeout))
                {
                    // Abort if still not done
                    _getRefreshDataThread.Abort();
                }
            }
        }

        private ManualResetEvent _terminateGetRefreshDataThreadEvent;
        private Thread _getRefreshDataThread;
        private ConnectorState _connectorStateForRefresh;
        private Guid _detailsFormId = Guid.NewGuid();
        private readonly object _refreshSyncObject = new object();

        #endregion

// ReSharper disable once UnusedMember.Local
        private void btnTestAdmin_Click(object sender, EventArgs e)
        {
            //todo add connection test code
        }

        

        private ManagementCredentialsNeededResponse GetAdminCredentialsNeeded(string pluginId)
        {
            bool userCancelled;

            //currently we do not actually use the errors. Needs to be valued up to show to the user.
            //we just fill the list from the response
#pragma warning disable 219
            String[] errors;
#pragma warning restore 219
            ManagementCredentialsNeededResponse managementCredentialsNeededResponse = null;
            try
            {
                using (var serviceWorker = new ServiceWorkerWithProgress(this))
                {
                    managementCredentialsNeededResponse = serviceWorker.AdminCredentialsNeeded("Getting back office admin credentials", pluginId);
                    //populate first level of the descriptions. need dedicated code to go deeper.
                    //if (!String.IsNullOrEmpty(managementCredentialsNeededResponse.DescriptionsAsString))
                    //{
                    //    managementCredentialsNeededResponse.Descriptions =
                    //        JsonConvert.DeserializeObject<Dictionary<string, object>>(managementCredentialsNeededResponse.DescriptionsAsString);
                    //}
                    userCancelled = serviceWorker.UserCancelled;
                    if (!userCancelled)
                    {
                       //TODO: do right thing if canceled
                       
                    }
                }
            }
            catch (Exception)
            {
                // ReSharper disable once RedundantAssignment
                errors = new string[] { Resources.ConnectorLogin_NoResponseFromBackoffice };

                //TODO: JSB show list of error messages like the  admin login form
            }

            return managementCredentialsNeededResponse;
        }

        private ConnectionCredentialsNeededResponse GetConnectionCredentialsNeeded(string pluginId, IDictionary<string, string> managementCredentials, IDictionary<string, string> connectionCredentials)
        {
            bool userCancelled;

            //currently we do not actually use the errors. Needs to be valued up to show to the user.
            //we just fill the list from the response
#pragma warning disable 219
            String[] errors;
#pragma warning restore 219
            ConnectionCredentialsNeededResponse connectionCredentialsNeededResponse = null;
            try
            {
                using (var serviceWorker = new ServiceWorkerWithProgress(this))
                {
                    connectionCredentialsNeededResponse = serviceWorker.ConnectionCredentialsNeeded("Getting connection credentials", pluginId, managementCredentials, connectionCredentials);
                    //if (!String.IsNullOrEmpty(connectionCredentialsNeededResponse.DescriptionsAsString))
                    //{
                    //    connectionCredentialsNeededResponse.Descriptions =
                    //        JsonConvert.DeserializeObject<Dictionary<string, object>>(connectionCredentialsNeededResponse.DescriptionsAsString);    
                    //}
                    

                    userCancelled = serviceWorker.UserCancelled;
                    if (!userCancelled)
                    {
                       //TODO: do right thing if canceled
                       
                    }
                }
            }
            catch (Exception)
            {
                // ReSharper disable once RedundantAssignment
                errors = new[] { Resources.ConnectorLogin_NoResponseFromBackoffice };

                //TODO: JSB show list of error messages like the  admin login form
            }

            return connectionCredentialsNeededResponse;
        }

        private void btnFeatureConfigurations_Click(object sender, EventArgs e)
        {
            if (dgConnections.Rows.Count == 0)
            {
                btnFeatureConfigurations.Enabled = false;
            }

            var featureConfigProperties = GetFeatureConfigurationProperties();

            if (featureConfigProperties == null || !featureConfigProperties.Any())
            {
                //Display message... not features available. 
                btnFeatureConfigurations.Enabled = false;
                return;
            }
            var connectorPlugin = GetConnectorPlugin();

            if (!ConnectorViewModel.IsConnectorPluginAvailable(connectorPlugin))
            {
                using (var form = new BackOfficeNotAvailableForm(connectorPlugin))
                {
                    form.ShowDialog(this);
                }
            }
            else
            {
                ShowFeatureConfigurationsForm();
            }
        }

    }
}
