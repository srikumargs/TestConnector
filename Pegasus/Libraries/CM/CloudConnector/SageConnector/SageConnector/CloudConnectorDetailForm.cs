using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sage.Connector.Common;
using Sage.Connector.Configuration.Contracts.Data;
using Sage.Connector.Configuration.Contracts.Data.SelectionValueTypes;
using Sage.Connector.Configuration.Mediator.JsonConverters;
using Sage.Connector.DomainMediator.Core.JsonConverters;
using Sage.Connector.Management;
using Sage.Connector.StateService.Interfaces.DataContracts;
using SageConnector.Internal;
using SageConnector.Properties;
using SageConnector.ViewModel;

namespace SageConnector
{
    /// <summary>
    /// Cloud Connector Details Form
    /// </summary>
    public partial class CloudConnectorDetailForm : Form
    {
        private ConfigurationViewModel _configuration = null;
        private BackOfficeConnection[] _connections = null;
        private BackOfficeConnectionsForCredentialsResponse _backOfficeConnectionsForCredentialsResponse = null;
        private bool _showEndpointAddress = false;
        private bool _forEdit = false;
        private bool _canceling = true;
        private ConnectionCredentialsNeededResponse _credentials;

        /// <summary>
        /// Cloud Connector Details form constructor
        /// </summary>
        /// <param name="config"></param>
        /// <param name="credentials"></param>
        /// <param name="isNew"></param>
        /// <param name="showEndpointAddress"></param>
        public CloudConnectorDetailForm(ConfigurationViewModel config, ConnectionCredentialsNeededResponse credentials, bool isNew, bool showEndpointAddress)
        {
            Debug.Assert(config != null, "ConfigurationViewModel cannot be null");
            _configuration = config;
            _credentials = credentials;

            //Initialize the Backoffice connection to disabled until they select a company.
            if (isNew)
            {
                _configuration.BackOfficeConnectionEnabledToReceive = false;
            }

            _forEdit = !isNew;
            _showEndpointAddress = showEndpointAddress;
            InitializeComponent();
        }

        private void CloudConnectorDetailForm_Load(object sender, EventArgs e)
        {
            Cursor now = Cursor.Current;
            Cursor.Current = Cursors.WaitCursor;

            try
            { InitializeForm(); }
            finally
            { Cursor.Current = now; }
        }

        private void InitializeForm()
        {
            try
            {
                ConfigureDiagnostics(_showEndpointAddress);
                BindData();
            }
            catch (Exception ex)
            {
                string logErrorMsg = string.Format(
                    Properties.Resources.ConnectorDetails_ErrorInitializingForAddTenant,
                    string.Empty,
                    ex.ExceptionAsString());
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteCriticalWithEventLogging(this, "Windowing", logErrorMsg);
                }


                string userErrorMsg = string.Format(
                    Properties.Resources.ConnectorDetails_ErrorInitializingForAddTenant,
                    Environment.NewLine,
                    ex.Message);
                MessageBox.Show(this, userErrorMsg,
                    Resources.ConnectorCommon_ErrorCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
            UpdatePluggedInProductHeaders();
            InitializeCompanyEnabledCheckbox();
            //UpdateForCredentials();
        }

        //private void UpdateForCredentials()
        //{
        //    if (_credentials != null && _forEdit == false)
        //    {
        //        string userId = null;
        //        if (_credentials.Descriptions.ContainsKey(_userIdKey))
        //        {
        //            _credentials.CurrentValues.TryGetValue(_userIdKey, out userId);
        //        }

        //        string password = null;
        //        if (_credentials.Descriptions.ContainsKey(_passwordKey))
        //        {
        //            _credentials.CurrentValues.TryGetValue(_passwordKey, out password);
        //        }

        //        //if (!String.IsNullOrWhiteSpace(userId))
        //        //{
        //        //    txtPremiseUsername.Text = userId;
        //        //    //_configuration.BackOfficeUserName = userId;
        //        //}

        //        //if (!String.IsNullOrWhiteSpace(password))
        //        //{
        //        //    txtMarker.Text = password;
        //        //    //_configuration.BackOfficeUserPassword = password;
        //        //}
        //    }
        //}

        private void InitializeCompanyEnabledCheckbox(bool forceChecked = false)
        {
            bool currentState = _configuration.BackOfficeConnectionEnabledToReceive;
            if (!_forEdit) currentState = true;
            //if (_configuration.BackOfficeCompanySelected)
            {
                chkPremiseEnabledReceive.Enabled = true;
                _configuration.BackOfficeConnectionEnabledToReceive = forceChecked ? true : currentState;
            }
            //else
            //{
            //    chkPremiseEnabledReceive.Enabled = false;
            //    _configuration.BackOfficeConnectionEnabledToReceive = false;
            //}
        }

        private void UpdatePluggedInProductHeaders()
        {

            var connectorPlugin = ConnectorPluginsViewModel.GetConnectorPlugins().Where(x => x.Id == _configuration.ConnectorPluginId).SingleOrDefault();


            grpPremiseDetails.Text = string.Format(Resources.ConnectorMain_GrpBoxLabelBackofficeConnectionDetails,
                connectorPlugin != null ? connectorPlugin.PluggedInProductName : ConnectorViewModel.BackOfficeProductTermSentenceCaps);
        }

        private void ConfigureDiagnostics(bool showEndpoint)
        {
            if (showEndpoint)
            {
                pnlEndpoint.Visible = true;
            }
        }

        private void BindData()
        {
            txtEndpointAddress.DataBindings.Add(new Binding("Text", _configuration, "CloudEndpoint"));

            txtTenantCompanyName.DataBindings.Add(new Binding("Text", _configuration, "CloudCompanyName", false, DataSourceUpdateMode.OnPropertyChanged));
            txtTenantConnectionKey.DataBindings.Add(new Binding("Text", _configuration, "CompositeConnectionKey"));
            lnkTenantSiteUrl.DataBindings.Add(new Binding("Text", _configuration, "CloudCompanyUrl", false, DataSourceUpdateMode.OnPropertyChanged));
            chkPremiseEnabledReceive.DataBindings.Add(new Binding("Checked", _configuration, "BackOfficeConnectionEnabledToReceive"));
            chkTenantEnabledReceive.DataBindings.Add(new Binding("Checked", _configuration, "CloudConnectionEnabledToReceive"));
            chkTenantEnabledSend.DataBindings.Add(new Binding("Checked", _configuration, "CloudConnectionEnabledToSend"));
        }

        /// <summary>
        /// Returns connections with displayable connection information
        /// </summary>
        public BackOfficeConnectionsForCredentialsResponse BackOfficeConnectionsForCredentialsResponse
        {
            get { return _backOfficeConnectionsForCredentialsResponse; }
            set { _backOfficeConnectionsForCredentialsResponse = value; }
        }

        /// <summary>
        /// Returns the tenant id for this connection
        /// </summary>
        public string TenantId
        {
            get { return _configuration.CloudTenantId; }
        }

        ///// <summary>
        ///// User name of the admin
        ///// </summary>
        //public string User { get; set; }

        ///// <summary>
        ///// Password of the admin
        ///// </summary>
        //public String Password { get; set; }

        private void DisplayMessages(string msg)
        {
            if (!string.IsNullOrEmpty(msg))
            {
                ShowInformationDialog(msg, "Validation Error");
            }
        }

        private bool ValidateForm()
        {
            List<string> messages = new List<string>();
            Cursor origCursor = Cursor.Current;

            try
            {
                Cursor.Current = Cursors.WaitCursor;
                _configuration.ValidateRequiredFields(ref messages);
                _configuration.ValidateTenantIdUnique(ref messages);
                if (!_configuration.AllowMultipleCompanyConnections())
                    _configuration.ValidateCompanyUnique(ref messages);
            }
            catch (Exception ex)
            {
                Cursor.Current = origCursor;
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteCriticalWithEventLogging(this, "Validate Configuration", Resources.ConnectorDetails_ErrorValidatingTenantId + ": " + ex.ExceptionAsString());
                }

                messages.Add(Resources.ConnectorDetails_ErrorValidatingTenantId);
            }
            finally
            {
                Cursor.Current = origCursor;
            }
            if (messages.Count > 0)
            {
                ShowInformationDialog(messages, Resources.ConnectorDetails_ConfigurationErrorCaption);
                return false;
            }
            return true;
        }


        /// <summary>
        /// Conditionally show a dialog with all error messages
        /// Return false only if there are errors and the user clicks cancel
        /// </summary>
        /// <param name="backOfficeStatus"></param>
        /// <param name="backOfficeDisplayMessages"></param>
        /// <param name="tenantStatus"></param>
        /// <param name="cloudDisplayMessages"></param>
        /// <returns></returns>
        private bool VerifySaveWithConditionalDialog(
            BackOfficeConnectivityStatus backOfficeStatus,
            string[] backOfficeDisplayMessages,
            TenantConnectivityStatus tenantStatus,
            string[] cloudDisplayMessages)
        {
            if (backOfficeStatus == BackOfficeConnectivityStatus.Normal &&
                tenantStatus == TenantConnectivityStatus.Normal)
            {
                // Either everything is fine, no dialog necessary
                return true;
            }

            // Get the formatted detail strings
            string backOfficeDisplayMessage = CreateMessageFromArray(backOfficeDisplayMessages, true);
            string cloudDisplayMessage = CreateMessageFromArray(cloudDisplayMessages, true);

            // Full message for the back office part
            string fullBackOfficeMessage =
                (backOfficeStatus == BackOfficeConnectivityStatus.Normal)
                ? string.Empty
                : String.Format("{0}{1}{2}",
                    Environment.NewLine,
                    Resources.ConnectorDetails_PremiseConnectionStatusNotOk,
                    backOfficeDisplayMessage);

            // Full message for the cloud part
            string spacer = (string.IsNullOrEmpty(fullBackOfficeMessage))
                ? Environment.NewLine
                : String.Concat(Environment.NewLine, Environment.NewLine);
            string fullCloudMessage =
                (tenantStatus == TenantConnectivityStatus.Normal)
                ? string.Empty
                : String.Format("{0}{1}{2}",
                    spacer,
                    Resources.ConnectorDetails_TenantConnectionStatusNotOk,
                    cloudDisplayMessage);

            // Messages could still be empty if connections are disabled
            // If we have nothing to report, then stop here
            if (String.IsNullOrEmpty(fullBackOfficeMessage) && String.IsNullOrEmpty(fullCloudMessage))
            {
                return true;
            }

            // Combine everything
            string finalMessage = String.Format(
                Resources.ConnectorDetails_CompleteConnectionIssue_Message,
                Environment.NewLine,
                fullBackOfficeMessage,
                fullCloudMessage);

            // Show the dialog
            DialogResult dialogResult = System.Windows.Forms.DialogResult.None;
            using (var form = new ErrorOnSaveForm(finalMessage))
            {
                dialogResult = form.ShowDialog(this);
            }

            // Return whether the user clicked OK
            return (dialogResult == System.Windows.Forms.DialogResult.OK);
        }

        private void ShowPremiseConnectionStatus(bool connectionOk, string[] msgs)
        {
            string msg = CreateMessageFromArray(msgs, false);

            if (connectionOk)
            {
                ShowInformationDialog(Resources.ConnectorDetails_ConnectionStatusOk + msg,
                    Resources.ConnectorDetails_ConnectionStatusCaption);
            }
            else
            {
                ShowWarningDialog(Resources.ConnectorDetails_PremiseConnectionStatusNotOk + msg,
                    Resources.ConnectorDetails_ConnectionStatusCaption);
            }
        }

        private void ShowTenantConnectionStatus(bool connectionOk, string[] msgs)
        {
            string msg = CreateMessageFromArray(msgs, false);

            if (connectionOk)
            {
                ShowInformationDialog(Resources.ConnectorDetails_ConnectionStatusOk + msg,
                    Resources.ConnectorDetails_ConnectionStatusCaption);
            }
            else
            {
                ShowWarningDialog(Resources.ConnectorDetails_TenantConnectionStatusNotOk + msg,
                    Resources.ConnectorDetails_ConnectionStatusCaption);
            }
        }

        private void ShowWarningDialog(string msg, string caption)
        {
            //TODO: value this up into something that shows multiple lines of error info better.
            //and that maybe does not ask them to be "ok" with a warning...
            MessageBox.Show(this, msg, caption, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void ShowWarningDialog(List<string> messages, string caption)
        {
            string msg = "";
            if (messages != null)
            {
                foreach (string s in messages)
                {
                    msg += s + Environment.NewLine;
                }
            }
            ShowInformationDialog(msg, caption);
        }


        private void ShowInformationDialog(string msg, string caption)
        {
            MessageBox.Show(this, msg, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowInformationDialog(List<string> messages, string caption)
        {
            string msg = "";
            if (messages != null)
            {
                foreach (string s in messages)
                {
                    msg += s + Environment.NewLine;
                }
            }
            ShowInformationDialog(msg, caption);
        }

        private string CreateMessageFromArray(string[] info, bool bulleted)
        {
            string msg = string.Empty;

            if (info != null)
            {
                msg += "\n";
                foreach (string s in info)
                {
                    // Format message with an optional bullet point
                    msg += String.Format("{0}{1} {2}",
                        Environment.NewLine,
                        (bulleted) ? "\u2022" : string.Empty,
                        s);
                }
            }

            return msg;
        }
        private void lnkHelpLink_Click(object sender, EventArgs e)
        {
            ConnectorUtilities.ShowDetailFormHelp();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        //bool _connectionInitialized = false;
        private readonly IDictionary<string, Control> _dynamicValueControls = new Dictionary<string, Control>();
        private void CloudConnectorDetailForm_Shown(object sender, EventArgs e)
        {
            //_connections = ConnectorViewModel.ProcessBackOfficeConnectionsForCredentialsResponse(_backOfficeConnectionsForCredentialsResponse);

            if (_credentials != null)
            {
                this.SuspendLayout();
                CredentialsHelper.AddCredentialControls(_credentials.DescriptionsAsString, _credentials.CurrentValues, _dynamicValueControls, grpPremiseDetails.Controls, lblMarker, txtMarker);
                this.ResumeLayout();
            }

            _connections = GetBackOfficeConnectionFromCredentials(_credentials);

            //string currentName = _configuration.BackOfficeCompanyName;
            //string currentConnectionInformation = _configuration.BackOfficeConnectionInformation;
            //TODO: look at setting current selection based on info from back office.

            //cbCompanyName.DisplayMember = "Name";
            //cbCompanyName.DataSource = _connections;

            //int foundIndex = -1;
            //if (!String.IsNullOrEmpty(currentName))
            //{
            //    for (int i = 0; i < _connections.Length; i++)
            //    {
            //        if (_connections[i].ConnectionInformation == currentConnectionInformation)
            //        {
            //            foundIndex = i;
            //            break;
            //        }
            //    }
            //}

            //cbCompanyName.SelectedIndex = foundIndex;
            //_connectionInitialized = true;
            //this.cbCompanyName.SelectedIndexChanging += new System.ComponentModel.CancelEventHandler(this.cbCompanyName_SelectedIndexChanging);
        }

        private BackOfficeConnection[] GetBackOfficeConnectionFromCredentials(ConnectionCredentialsNeededResponse credentials)
        {
            BackOfficeConnection[] result = { };
            List<BackOfficeConnection> companies = new List<BackOfficeConnection>();
            var j = JObject.Parse(credentials.DescriptionsAsString);
            if (j["CompanyId"] != null)
            {
                var description = j["CompanyId"];
                if (description != null)
                {
                    bool hasName = description["ValueName"] != null;
                    bool hasId = description["ValueId"] != null;
                    bool hasDescription = description["ValueDescription"] != null;

                    //got to have a name or are done
                    if (hasName)
                    {
                        IList<string> emptyList = new List<string>() as IList<string>;

                        IList<string> names = (hasName ? description["ValueName"].ToObject<IList<string>>() : emptyList);
                        IList<string> ids = (hasId ? description["ValueId"].ToObject<IList<string>>() : emptyList);
                        IList<string> descriptions = (hasDescription ? description["ValueDescription"].ToObject<IList<string>>() : emptyList);

                        //for now do not allow partial list items all must be present
                        bool noNulls = ((names != null) && (ids != null) && (descriptions != null));
                        bool sameSizeNotEmpty = (noNulls && (names.Count == ids.Count) && (ids.Count == descriptions.Count) && (names.Count != emptyList.Count));
                        if (sameSizeNotEmpty)
                        {
                            //we finally know all list parts are here, are IList<string> and not empty and the same length. Finally time to make a connectionlist
                            int count = names.Count;
                            for (int i = 0; i < count; i++)
                            {
                                BackOfficeConnection company = new BackOfficeConnection(ids[i], descriptions[i], names[i]);
                                companies.Add(company);
                            }
                        }
                        result = companies.ToArray();
                    }

                }
            }
            return result;
        }

        private bool VerifyConnectionChangeDesired()
        {

            DialogResult response = System.Windows.Forms.DialogResult.No;
            if (_forEdit)
            {
                response = MessageBox.Show(Resources.ConnectorDetails_AreYouSureYouWantToUpdateMessage,
                    Resources.ConnectorDetails_AreYouSureYouWantToUpdateCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            }
            else
            {
                return true;
            }
            return response == DialogResult.Yes;
        }

        private void CloudConnectorDetailForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_canceling)
            {
                _configuration.RevertConfigurationToOriginalValues();
            }
        }

        //private void cbCompanyName_SelectedIndexChanging(object sender, CancelEventArgs e)
        //{
        //    //TODO: need to deal with company no longer present in new data list. (rights removed)
        //    //    This may not be an issue.  STO admins always have access to all databases.
        //    //TODO: Done/removed - determine if old name field should be removed or just left invisible for best data flow.

        //    ComboBoxEx cb = sender as ComboBoxEx;

        //    if (cb != null)
        //    {
        //        if (_connectionInitialized)
        //        {
        //            int selectedIndex = cb.SelectedIndex;

        //            if (VerifyConnectionChangeDesired())
        //            {
        //                _configuration.BackOfficeCompanyName = _connections[selectedIndex].Name;
        //                _configuration.BackOfficeConnectionInformation = _connections[selectedIndex].ConnectionInformation;
        //                _configuration.BackOfficeConnectionInformationDisplayable = _connections[selectedIndex].DisplayableConnectionInformation;
        //            }
        //            else
        //            {
        //                cb.SelectedIndex = cb.LastAcceptedSelectedIndex;
        //            }
        //            //If previous company selected is valid, then don't alter connection state.
        //            InitializeCompanyEnabledCheckbox(cb.LastAcceptedSelectedIndex < 0);
        //        }
        //    }
        //}

        private void lnkTenantSiteUrl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Uri uri = null;
            if (Uri.TryCreate(lnkTenantSiteUrl.Text, UriKind.Absolute, out uri))
            {
                ConnectorUtilities.ShowTenantSite(uri);
            }
        }

        #region Save Related Methods

        /// <summary>
        /// Handler for the save button click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            _canceling = false;
            if (!SaveConfiguration())
            {
                DialogResult = DialogResult.None;
                _canceling = true;
                return;
            }

       
        }


        /// <summary>
        /// Actual save logic
        /// </summary>
        /// <returns></returns>
        private bool SaveConfiguration()
        {
            // Don't save if the form is invalid
            if (!ValidateForm())
                return false;

            // Now save the configuration
            try
            {
                //NOTE: Way to much biz logic in this app, lot should be oved to the view model. 
                //The service worker with progress stuff if a big item to move.

                // Verify the connections
                // Do this before the save as some model data might change
                string cloudCompanyName, cloudCompanyUrl;
                string[] backOfficeDisplayMessages, cloudDisplayMessages;

                UpdateBackOfficeConnectionCredentialsInMemory();

                Boolean userCancelled = false;
                ValidateBackOfficeConnectionResponse validateBackOfficeConnectionResponse = null;
                ValidateTenantConnectionResponse validateTenantConnectionResponse = null;
                using (var serviceWorker = new ServiceWorkerWithProgress(this))
                {
                    serviceWorker.ValidateConnections(
                        "Testing connections",
                        _configuration.ConnectorPluginId,
                        _configuration.BackOfficeConnectionCredentials,
                        _configuration.CloudEndpoint,
                        _configuration.CloudTenantId,
                        _configuration.CloudPremiseKey,
                        out validateBackOfficeConnectionResponse,
                        out validateTenantConnectionResponse);
                    if (serviceWorker.UserCancelled)
                        return false;

                    BackOfficeConnectivityStatus backOfficeStatus = VerifyBackOfficeConnection(false, validateBackOfficeConnectionResponse, out backOfficeDisplayMessages, out userCancelled);
                    if (userCancelled)
                        return false;

                    TenantConnectivityStatus tenantStatus = VerifyTenantConnection(false, validateTenantConnectionResponse, out cloudCompanyName, out cloudCompanyUrl, out cloudDisplayMessages, out userCancelled);
                    if (userCancelled)
                        return false;


                    // Update the model with the cloud company data
                    if (!string.IsNullOrEmpty(cloudCompanyName) || !string.IsNullOrEmpty(cloudCompanyUrl))
                    {
                        _configuration.CloudCompanyName = cloudCompanyName;
                        _configuration.CloudCompanyUrl = cloudCompanyUrl;
                    }
                    //TODO: JSB find right location for this
                    _configuration.BackOfficeCompanyName = validateBackOfficeConnectionResponse.CompanyNameForDisplay;
                    _configuration.BackOfficeCompanyUniqueIndentifier = validateBackOfficeConnectionResponse.CompanyUniqueIndentifier;

                    // Show any errors before saving
                    // And allow the user to cancel
                    if (!VerifySaveWithConditionalDialog(
                            backOfficeStatus,
                            backOfficeDisplayMessages,
                            tenantStatus,
                            cloudDisplayMessages))
                    {
                        // Had some errors and the user opted to cancel the save
                        return false;
                    }

                    var result = ConfigurationHelpers.RegisterConnection(
                        new Uri(_configuration.CloudEndpoint),
                        _configuration.CloudTenantId,
                        _configuration.BackOfficeCompanyUniqueIndentifier,
                        String.Empty);

                    if (!result.Successful)
                        throw new Exception("Registration failed.");

                    // Update the PCR via the config service
                    _configuration.Save();

                    // Update the state service with the results of our verifications above
                    UpdateConnectionStatusesInStateService(
                        _configuration.CloudTenantId,
                        backOfficeStatus,
                        tenantStatus);

                    // Refresh all connection statuses for this config
                    _configuration.RefreshConnectionStatuses();
                }
            }
            catch (Exception ex)
            {
                string logErrorMessage = string.Format(
                    Resources.ConnectorDetails_SaveFailedMessage, string.Empty, ex.ExceptionAsString());
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteCriticalWithEventLogging(this, "Save Configuration", logErrorMessage);
                }

                string userErrorMessage = string.Format(
                    Resources.ConnectorDetails_SaveFailedMessage, Environment.NewLine, ex.Message);
                MessageBox.Show(this, userErrorMessage,
                    Resources.ConnectorDetails_SaveFailedCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Update the _credentials based on configuration and then update the BackOfficeConnectionCredentials based on that.
        /// </summary>
        /// <remarks>
        /// This seems like bad flow. Look at getting this into the view model in a better way.
        /// </remarks>
        private void UpdateBackOfficeConnectionCredentialsInMemory()
        {
            ////Put the values for user name, password and back office connection info into object and then into json.
            //string credentials = BackOfficeCredentialsUtilities.CreateConnectionCredentialsForMock(_configuration.BackOfficeUserName,
            //    _configuration.BackOfficeUserPassword, _configuration.BackOfficeConnectionInformation);
            //_configuration.BackOfficeConnectionCredentials = credentials;
            if (_credentials != null)
            {
                //if (_credentials.Descriptions.ContainsKey(_userIdKey))
                //    _credentials.CurrentValues[_userIdKey] = Username;

                //if (_credentials.Descriptions.ContainsKey(_passwordKey))
                //_credentials.CurrentValues[_passwordKey] = Password.ToNonSecureString();

                //loop thru added controls
                //extract current values
                //put current values in to the Idictionary.

                foreach (var item in _dynamicValueControls)
                {
                    if (item.Value is TextBox)
                    {
                        if (_credentials.CurrentValues.ContainsKey(item.Key))
                        {
                            _credentials.CurrentValues[item.Key] = item.Value.Text;
                        }
                    }

                    ComboBox list = item.Value as ComboBox;
                    if (list != null)
                    {
                        if (_credentials.CurrentValues.ContainsKey(item.Key))
                        {
                            string value = string.Empty;
                            ListItem selectedItem = (ListItem)list.SelectedItem;
                            if(selectedItem != null)
                                value = selectedItem.Id;
                            _credentials.CurrentValues[item.Key] = value;
                        }
                    }
                }
            }

            IDictionary<string, string> connectionCredentials = _credentials.CurrentValues ?? new Dictionary<string, string>();

            //TODO: JSB Review where we want this and if ti really wants to be a dict<string,string>
            string credentialsAsJson = JsonConvert.SerializeObject(connectionCredentials);
            _configuration.BackOfficeConnectionCredentials = credentialsAsJson;
        }

        #endregion


        #region Connection Validation Coordinators

        /// <summary>
        /// Handler for the test back office connection button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPremiseTestConnection_Click(object sender, EventArgs e)
        {
            // Test the connection
            string[] displayMesssages;
            Boolean userCancelled = false;

            //make sure credentials are up to date before we verify
            UpdateBackOfficeConnectionCredentialsInMemory();

            BackOfficeConnectivityStatus backOfficeStatus =
                VerifyBackOfficeConnection(true, null, out displayMesssages, out userCancelled);

            if (!userCancelled)
            {
                // Show the message in all cases, success or failure
                ShowPremiseConnectionStatus(backOfficeStatus == BackOfficeConnectivityStatus.Normal, displayMesssages);
            }
        }

        /// <summary>
        /// Validate the back office connection
        /// Can either affect changes, or not if just a test
        /// </summary>
        /// <param name="isTest"></param>
        /// <param name="response"></param>
        /// <param name="displayMessages"></param>
        /// <param name="userCancelled"></param>
        private BackOfficeConnectivityStatus VerifyBackOfficeConnection(
            bool isTest,
            ValidateBackOfficeConnectionResponse response,
            out string[] displayMessages,
            out Boolean userCancelled)
        {
            // Init out params
            userCancelled = false;
            displayMessages = new string[] { };

            // Init
            BackOfficeConnectivityStatus result = BackOfficeConnectivityStatus.None;

            try
            {
                if (response == null)
                {
                    using (var serviceWorker = new ServiceWorkerWithProgress(this))
                    {
                        response = serviceWorker.ValidateBackOfficeConnection(_configuration.ConnectorPluginId,
                            "Testing back office connection",
                            _configuration.BackOfficeConnectionCredentials);

                        userCancelled = serviceWorker.UserCancelled;
                        if (userCancelled)
                            return result;
                    }
                }

                // Perform the verification
                result = ConnectorUtilities.ProcessValidateBackOfficeConnectionResponse(response, out displayMessages);
                //TODO:fix structure of where this goes
                _configuration.BackOfficeCompanyName = response.CompanyNameForDisplay;
                _configuration.BackOfficeCompanyName = response.CompanyUniqueIndentifier;
            }
            catch (Exception)
            {
                // All exceptions are caught and formatted by the utility call above
            }

            // Return the status
            return result;
        }

        /// <summary>
        /// Handler for the test tenant connection button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTenantTestConnection_Click(object sender, EventArgs e)
        {
            // Test the connection
            string cloudCompanyName, cloudCompanyUrl;
            string[] displayMessages;
            Boolean userCancelled = false;
            TenantConnectivityStatus tenantStatus =
                VerifyTenantConnection(true, null, out cloudCompanyName, out cloudCompanyUrl, out displayMessages, out userCancelled);

            if (!userCancelled)
            {
                if (!string.IsNullOrEmpty(cloudCompanyName) || !string.IsNullOrEmpty(cloudCompanyUrl))
                {
                    // Update the cloud company info in the UI, even for a test
                    // Will only be persisted if the user also clicks save
                    _configuration.CloudCompanyName = cloudCompanyName;
                    _configuration.CloudCompanyUrl = cloudCompanyUrl;
                }

                // Show the message in all cases, success or failure
                ShowTenantConnectionStatus(tenantStatus == TenantConnectivityStatus.Normal, displayMessages);
            }
        }

        /// <summary>
        /// Verify the cloud connection
        /// </summary>
        /// <param name="isTest"></param>
        /// <param name="response"></param>
        /// <param name="cloudCompanyName"></param>
        /// <param name="cloudCompanyUrl"></param>
        /// <param name="displayMessages"></param>
        /// <param name="userCancelled"></param>
        /// <returns></returns>
        private TenantConnectivityStatus VerifyTenantConnection(
            bool isTest,
            ValidateTenantConnectionResponse response,
            out string cloudCompanyName,
            out string cloudCompanyUrl,
            out string[] displayMessages,
            out Boolean userCancelled)
        {
            // Init out params
            cloudCompanyName = string.Empty;
            cloudCompanyUrl = string.Empty;
            displayMessages = new string[] { };
            userCancelled = false;

            // Init
            TenantConnectivityStatus result = TenantConnectivityStatus.None;

            try
            {
                if (response == null)
                {
                    using (var serviceWorker = new ServiceWorkerWithProgress(this))
                    {
                        response = serviceWorker.ValidateTenantConnection(
                            "Testing website connection",
                            _configuration.CloudEndpoint,
                            _configuration.CloudTenantId,
                            _configuration.CloudPremiseKey);

                        userCancelled = serviceWorker.UserCancelled;
                        if (userCancelled)
                            return result;
                    }
                }

                // Perform the verification
                result = ConnectorUtilities.ProcessValidateTenantConnectionResponse(
                    response, out cloudCompanyName, out cloudCompanyUrl, out displayMessages);
            }
            catch (Exception)
            {
                // All exceptions are caught and formatted by the utility call above
            }

            // Return the verification response
            return result;
        }


        /// <summary>
        /// Update the state service
        /// Just wraps up the utility call and provides a wait cursor
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="backOfficeStatus"></param>
        /// <param name="tenantStatus"></param>
        private void UpdateConnectionStatusesInStateService(
            string tenantId,
            BackOfficeConnectivityStatus backOfficeStatus,
            TenantConnectivityStatus tenantStatus)
        {
            Cursor origCursor = Cursor.Current;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                ConnectorUtilities.UpdateConnectionStatusesInStateService(
                        _configuration.CloudTenantId,
                        backOfficeStatus,
                        tenantStatus);
            }
            catch (Exception)
            {
                // All exceptions are caught and formatted by the utility call above
            }
            finally
            {
                Cursor.Current = origCursor;
            }
        }

        #endregion

        private void CancelInProgressRequests_Click(object sender, EventArgs e)
        {
            using (ConnectionRequestListForm form = new ConnectionRequestListForm(_configuration))
            {
                form.ShowDialog(this);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// May no longer be needed remove if so.
    /// </remarks>
    public class ComboBoxEx : ComboBox
    {
        /// <summary>
        /// 
        /// </summary>
        public event CancelEventHandler SelectedIndexChanging;

        /// <summary>
        /// 
        /// </summary>
        [Browsable(false)]
        public int LastAcceptedSelectedIndex { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ComboBoxEx()
        {
            LastAcceptedSelectedIndex = -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected void OnSelectedIndexChanging(CancelEventArgs e)
        {
            var selectedIndexChanging = SelectedIndexChanging;
            if (selectedIndexChanging != null)
                selectedIndexChanging(this, e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            if (LastAcceptedSelectedIndex != SelectedIndex)
            {
                var cancelEventArgs = new CancelEventArgs();
                OnSelectedIndexChanging(cancelEventArgs);

                if (!cancelEventArgs.Cancel)
                {
                    LastAcceptedSelectedIndex = SelectedIndex;
                    base.OnSelectedIndexChanged(e);
                }
                else
                    SelectedIndex = LastAcceptedSelectedIndex;
            }
        }
    }
}

