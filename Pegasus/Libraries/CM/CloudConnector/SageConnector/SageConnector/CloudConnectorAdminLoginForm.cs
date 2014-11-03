using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using Sage.Connector.Common;
using Sage.Connector.StateService.Interfaces.DataContracts;
using SageConnector.Internal;
using SageConnector.Properties;
using SageConnector.ViewModel;

namespace SageConnector
{
    /// <summary>
    /// Login form for adding/updating cloud connections also supports delete built in.
    /// </summary>
    public partial class CloudConnectorAdminLoginForm : Form
    {
        private ConfigurationViewModel _configuration = null;
        private ManagementCredentialsNeededResponse _credentials;
        private bool _deleting = false;
        private bool _adminConfirmed = false;
        private int _failedLoginCount = 0;
        private const int MAX_LOGIN_ATTEMPTS = 2;
        private ConnectorPlugin _plugin;

        /// <summary>
        /// Verified as administrator
        /// </summary>
        public bool AdministratorConfirmed { get { return _adminConfirmed; } }

        ///// <summary>
        ///// Username
        ///// </summary>
        //public string Username { get; set; }

        ///// <summary>
        ///// Password
        ///// </summary>
        //public SecureString Password { get; set; }

        /// <summary>
        /// ManagementCredentials
        /// </summary>
        public IDictionary<string, string> ManagementCredentials { get; set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="plugin"></param>
        /// <param name="config"></param>
        /// <param name="credentials"></param>
        /// <param name="deleting"></param>
        public CloudConnectorAdminLoginForm(ConnectorPlugin plugin, ConfigurationViewModel config, ManagementCredentialsNeededResponse credentials, bool deleting)
        {
            InitializeComponent();
            _plugin = plugin;
            _configuration = config;
            _deleting = deleting;
            _credentials = credentials;
            this.ManagementCredentials = new Dictionary<string, string>();
            AdjustDialog();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public CloudConnectorAdminLoginForm()
        {
            InitializeComponent();
            AdjustDialog();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            AcceptValues();
        }

        private void AcceptValues()
        {
            string[] errors = new string[] { };

            //Username = txtMarker.Text.Trim();
            //NOTE: should this be some sort of securestring textbox?
            //Password = txtPassword.Text.ToSecureString();

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
                            _credentials.CurrentValues[item.Key] = list.Text;
                        }
                    }
                }
            }

            //Persist the credentials in we successfully log in for use by main form.
            if (this.ManagementCredentials == null)
            {
                this.ManagementCredentials = new Dictionary<string, string>();
            }
            this.ManagementCredentials = _credentials.CurrentValues ?? new Dictionary<string, string>();

            _backOfficeConnectionsForCredentialsResponse = null;
            Boolean userCancelled = false;
            if (_deleting)
            {
                // In the case of UsesPerCompanyAdministrator, we expect the plugin to confirm the credentials are valid for 1 or more companies.
                // If they are then we also get the list of companies it is valid for and check that the one we are trying to delete
                // is actually one of the valid ones.
                //TODO: JSB - revise this in light of new approaches and ApplicationSecurityMode going away. S100Contractor used to use PerCompanyAdmin
                //if (_plugin.ApplicationSecurityMode == ApplicationSecurityMode.UsesPerCompanyAdministrator)
                //{ 
                //    _adminConfirmed = ConfirmAdministratorLoginAndGetConnections(out errors, out _backOfficeConnectionsForCredentialsResponse, out userCancelled);
                //    if (_adminConfirmed)
                //    {
                //        _adminConfirmed = _backOfficeConnectionsForCredentialsResponse.BackOfficeConnections.Where(x => x.ConnectionInformation == _configuration.BackOfficeConnectionInformation).Any();
                //    }
                //}
                //else
                {
                    _adminConfirmed = ConfirmAdministratorLogin(out errors, out userCancelled);
                }
            }
            else
            {
                _adminConfirmed = ConfirmAdministratorLoginAndGetConnections(out errors, out _backOfficeConnectionsForCredentialsResponse, out userCancelled);
            }

            if (_adminConfirmed)
            {
                if (_deleting && VerifyUserWantsToDelete())
                {
                    PerformDelete();
                }
            }
            else
            {
                if (userCancelled)
                {
                    DialogResult = DialogResult.Cancel;
                }
                else
                {
                    if (errors.GetLength(0) == 0)
                    {
                        //use general failure message if we got here without any actual message.
                        errors = new string[] { Resources.ConnectorLogin_LogonFailed };
                    }

                    ShowLoginErrorMessage(errors);
                    if (++_failedLoginCount > MAX_LOGIN_ATTEMPTS)
                        DialogResult = DialogResult.Cancel;
                    else
                        DialogResult = DialogResult.None;
                }
            }
        }

        private void ShowLoginErrorMessage(string[] msgs)
        {
            string msg = "";
            foreach (string s in msgs)
                msg += string.Format("{0}{1}", s, Environment.NewLine);

            MessageBox.Show(this,
                msg,
                Resources.ConnectorLogin_LoginErrorCaption,
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private bool VerifyUserWantsToDelete()
        {
            bool confirm = false;

            DialogResult result = MessageBox.Show(this, Resources.ConnectorLogin_ConfirmDeleteMessage,
                Resources.ConnectorLogin_ConfirmDeleteCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
                confirm = true;

            return confirm;
        }

        private void PerformDelete()
        {
            try
            {
                if (AdministratorConfirmed)
                {
                    Cursor.Current = Cursors.WaitCursor;
                    ConfigurationViewModelManager manager = new ConfigurationViewModelManager();
                    if (!manager.DeleteConfiguration(_configuration))
                    {
                        throw new Exception(Resources.ConnectorCommon_ConfigurationDataProblem);
                    }
                }
            }
            catch (Exception ex)
            {
                Cursor.Current = Cursors.Default;

                string logErrorMessage = string.Format(
                    Resources.ConnectorLogin_DeleteFailedMessage,
                    string.Empty,
                    ex.ExceptionAsString());

                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteCriticalWithEventLogging(this, "Delete Configuration", logErrorMessage);
                }

                string userErrorMessage = string.Format(
                    Resources.ConnectorLogin_DeleteFailedMessage,
                    Environment.NewLine,
                    ex.Message);
                MessageBox.Show(this, userErrorMessage,
                    Resources.ConnectorLogin_DeleteFailedCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private bool ConfirmAdministratorLogin(out string[] errors, out Boolean userCancelled)
        {
            userCancelled = false;
            bool confirmed = false;
            errors = new string[] { };

            try
            {
                using (var serviceWorker = new ServiceWorkerWithProgress(this))
                {
                    IDictionary<string, string> adminCredentials = _credentials.CurrentValues;

                    var response = serviceWorker.ValidateBackOfficeAdminCredentials(
                            "Validating back office credentials",
                            _plugin.Id,
                            adminCredentials);

                    userCancelled = serviceWorker.UserCancelled;
                    if (!userCancelled)
                    {
                        confirmed = ConnectorViewModel.ProcessValidateBackOfficeAdminCredentialsResponse(response, out errors);
                    }
                }
            }
            catch (Exception)
            {
                confirmed = false;
                errors = new string[] { Resources.ConnectorLogin_NoResponseFromBackoffice };
            }
            return confirmed;
        }

        private bool ConfirmAdministratorLoginAndGetConnections(out string[] errors,
            out BackOfficeConnectionsForCredentialsResponse backOfficeConnectionsForCredentialsResponse,
            out Boolean userCancelled)
        {
            userCancelled = false;
            bool confirmed = false;
            errors = new string[] { };
            backOfficeConnectionsForCredentialsResponse = null;

            ConfigurationViewModelManager configManager = new ConfigurationViewModelManager();
            try
            {
                ValidateBackOfficeAdminCredentialsResponse validateBackOfficeAdminCredentialsResponse = null;

                using (var serviceWorker = new ServiceWorkerWithProgress(this))
                {
                    IDictionary<string, string> adminCredentials = _credentials.CurrentValues;

                    serviceWorker.ValidateBackOfficeAdminCredentialsAndGetBackOfficeConnections(
                        "Validating back office credentials",
                        _plugin.Id,
                        adminCredentials,
                        out validateBackOfficeAdminCredentialsResponse,
                        out backOfficeConnectionsForCredentialsResponse);

                    userCancelled = serviceWorker.UserCancelled;
                    if (!userCancelled)
                    {
                        confirmed = ConnectorViewModel.ProcessValidateBackOfficeAdminCredentialsResponse(validateBackOfficeAdminCredentialsResponse, out errors);
                    }
                }
            }
            catch (Exception)
            {
                confirmed = false;
                errors = new string[] { Resources.ConnectorLogin_NoResponseFromBackoffice };
            }
            return confirmed;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void AdjustDialog()
        {
            this.Text = string.Format(Resources.ConnectorLogin_DialogTitle,
                _plugin.LoginAdministratorTerm);


            //TODO: JSB remove the ApplicationSecurityMode
            if (_plugin.ApplicationSecurityMode == ApplicationSecurityMode.UsesGlobalApplicationSecurityAdministrator)
            {
                this.textBoxLoginInstructions.Text = string.Format(Resources.ConnectorLogon_Instructions,
                    _plugin.PluggedInProductName, _plugin.LoginAdministratorTerm);
            }
            else if (_plugin.ApplicationSecurityMode == ApplicationSecurityMode.UsesPerCompanyAdministrator)
            {
                this.textBoxLoginInstructions.Text = string.Format(Resources.ConnectorLogon_Instructions_PerCompanyAdministrator_AccountReadOnly,
                    _plugin.PluggedInProductName, _plugin.LoginAdministratorTerm);
            }


            //if (!String.IsNullOrEmpty(_plugin.LoginAdministratorAccountNamePrefill))
            //{
            //    txtMarker.Text = _plugin.LoginAdministratorAccountNamePrefill;
            //}

            //if (_plugin.LoginAdministratorAccountNameReadOnly)
            //{
            //    txtUser.Enabled = false;
            //    txtPassword.Focus();
            //}

            if (_credentials != null)
            {
                this.SuspendLayout();
                CredentialsHelper.AddCredentialControls(_credentials.DescriptionsAsString, _credentials.CurrentValues, _dynamicValueControls, this.Controls, lblMarker, txtMarker);
                this.ResumeLayout();    
            }
        }

        private readonly IDictionary<string, Control> _dynamicValueControls = new Dictionary<string, Control>(); 
       

        /// <summary>
        /// 
        /// </summary>
        public BackOfficeConnectionsForCredentialsResponse BackOfficeConnectionsForCredentialsResponse
        { get { return _backOfficeConnectionsForCredentialsResponse; } }

        private BackOfficeConnectionsForCredentialsResponse _backOfficeConnectionsForCredentialsResponse;

        private void lblMarker_Click(object sender, EventArgs e)
        {

        }
    }
}

