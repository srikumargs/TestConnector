using System;
using System.Collections.Generic;
using System.Security;
using System.Windows.Forms;
using Sage.Connector.Common;
using SageConnector.Properties;
using SageConnector.ViewModel;

namespace SageConnector
{
    /// <summary>
    /// Different reasons for invoking this form
    /// </summary>
    internal enum AccountSelectionFormMode
    {
        /// <summary>
        /// Default for initializer
        /// </summary>
        None = 0,

        /// <summary>
        /// Add or change the configuration on the service
        /// </summary>
        EditConfiguration,

        /// <summary>
        /// Login specified does not work
        /// </summary>
        LoginError,

        /// <summary>
        /// The specified account name does not exist or is invalid
        /// </summary>
        AccountError,

        /// <summary>
        /// The specified account does not have the proper file system rights
        /// </summary>
        AccountAccessError
    }

    internal partial class AccountSelectionForm : Form
    {
        /// <summary>
        /// Enumeration for possible selections so we can tell if things changed
        /// </summary>
        private enum AccountTypeSelection
        {
            /// <summary>
            /// Nothing selected
            /// </summary>
            None = 0,

            /// <summary>
            /// Stock account selected
            /// </summary>
            Stock,

            /// <summary>
            /// User account selected
            /// </summary>
            User
        }

        /// <summary>
        /// Private default constructor
        /// </summary>
        private AccountSelectionForm()
        {
        }

        /// <summary>
        /// Constructor with mode
        /// </summary>
        /// <param name="mode"></param>
        public AccountSelectionForm(AccountSelectionFormMode mode)
        {
            // Set any private members
            _mode = mode;

            // Init
            InitializeComponent();

            // Set the text of the default stock account option
            this.lblStockAccount.Text = StockAccountDisplayNames[ConnectorRegistryUtils.ConnectorServiceStockAccount];

            // Set the picture box image, etc. based on mode
            ConfigureForMode();

            // Set OS-specific features
            ConfigureForOS();

            // Set the currently selected service account user, if any
            ConfigureForCurrentServiceAccount();
        }

        /// <summary>
        /// Get the user name entered
        /// </summary>
        public string User { get { return _user; } }

        /// <summary>
        /// Get the password entered
        /// </summary>
        public SecureString Password
        {
            get
            {
                if (_password == null)
                {
                    return null;
                }
                return _password.ToSecureString();
            }
        }

        /// <summary>
        /// Determine whether a re-install is necessary based on user actions
        /// </summary>
        public bool RequiresReInstall
        {
            get { return (_accountNameChanged || _passwordChanged || (_initialSelection != _finalSelection)); }
        }


        #region Private Methods
        
        /// <summary>
        /// Setup UI items specific to the form mode
        /// </summary>
        private void ConfigureForMode()
        {
            string descriptionLabelText = string.Empty;

            switch(_mode)
            {
                // Either new service or edit existing one
                case AccountSelectionFormMode.EditConfiguration:
                    descriptionLabelText += String.Format(
                        Resources.ConnectorAccountSelection_SelectionText,
                        ConnectorRegistryUtils.BriefProductName);
                    pictureBox1.Image = Resources.Image_edit_configuration;
                    break;
               
                // All login error scenarios related to the login are handled the same way
                case AccountSelectionFormMode.LoginError:
                case AccountSelectionFormMode.AccountError:
                    descriptionLabelText += Resources.ConnectorAccountSelection_AccountSelectionErrorText;
                    pictureBox1.Image = Resources.Image_config_error;
                    break;

                // Special error message for account access issues
                case AccountSelectionFormMode.AccountAccessError:
                    descriptionLabelText += Resources.ConnectorAccountSelection_AccountSelectionAccessErrorText;
                    pictureBox1.Image = Resources.Image_config_error;
                    break;
               
                // Unhandled form mode value, includes None state
                default:
                    throw new ArgumentException(
                        String.Format("Unhandled account selection form mode '{0}'",
                        Enum.GetName(typeof(AccountSelectionFormMode), _mode)));
            }

            // Set the description label
            descriptionLabel.Text = descriptionLabelText;

            // Set the help link text
            lnkHelpLink.Text = Resources.ConnectorAccountSelectionForm_HelpLinkText;

            // Default select to the specified account
            rbSpecifiedAccount.Checked = true;
        }

        /// <summary>
        /// OS-specific configuration
        /// Windows 2003 or earlier must use a user service account, so need to disable the stock account
        /// </summary>
        private void ConfigureForOS()
        {
            OperatingSystem os = Environment.OSVersion;
            if (os != null &&
                os.Platform < PlatformID.Win32NT ||
                    (os.Platform == PlatformID.Win32NT &&
                        (os.Version.Major < 5 || (os.Version.Major == 5 && os.Version.Minor <= 2))
                    )
                )
            {
                // Windows 2003 or earlier detected (v.5.2)
                // Disable stock account option
                rbStockAccount.Enabled = false;
                rbSpecifiedAccount.Checked = true;
                lblStockAccount.Enabled = false;
                lblStockAccount.Text += Resources.ConnectorAccountSelectionForm_StockAccountNotAvailableText;
            }
        }

        private void ConfigureForCurrentServiceAccount()
        {
            // Get the current service account
            string currentServiceAccountUser = null;
            try
            {
                currentServiceAccountUser = ConnectorServiceUtils.GetServiceAccountUserName();
            }
            catch
            {
                // Error reading registry, use default account
            }

            if (Program.UpgradeAccountSelection)
            {
                currentServiceAccountUser = Program.User;
            }

            // Proceed if we got back the current service account user
            if (!string.IsNullOrEmpty(currentServiceAccountUser))
            {
                // Check for the stock account
                if (StockAccountUtils.GetStockAccountFromLoginString(currentServiceAccountUser) == 
                        ConnectorRegistryUtils.ConnectorServiceStockAccount)
                {
                    // Match!  But only set if enabled
                    if (rbStockAccount.Enabled)
                    {
                        rbStockAccount.Checked = true;
                    }
                }

                // Not the stock account
                else 
                {
                    rbSpecifiedAccount.Checked = true;
                    tbAccountName.Text = currentServiceAccountUser;
                    tbPassword.Text = tbConfirmPassword.Text = new string(' ', 15);
                }
            }

            // Reset the changed flags for the text fields and radio buttons
            _accountNameChanged = false;
            _passwordChanged = false;

            if (!Program.UpgradeAccountSelection)
            {
                _initialSelection = GetAccountTypeSelection();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void okButton_Click(object sender, EventArgs e)
        {
            _user = null;
            _password = null;

            if (rbSpecifiedAccount.Checked)
            {
                string user = GetEffectiveUserName();
                string password = tbPassword.Text;
                string confirmPassword = tbConfirmPassword.Text;
                bool isStockAccount = IsStockAccount(user);

                string errorText = null;
                if (string.IsNullOrEmpty(user))
                {
                    // No user entered
                    errorText = Resources.ConnectorAccountSelection_MissingUserText;
                }
                else if (!isStockAccount &&
                    (string.IsNullOrEmpty(password) || (_accountNameChanged && !_passwordChanged)))
                {
                    // For user (non-stock) accounts only
                    // A password must be entered, and if the user name has changed so must the password
                    errorText = Resources.ConnectorAccountSelection_MissingPasswordText;
                }
                else if (!isStockAccount && !password.Equals(confirmPassword))
                {
                    // For user (non-stock) accounts only
                    // Passwords don't match.  Password field ignored for stock accounts.
                    errorText = Resources.ConnectorAccountSelection_PasswordsNotEqualText;
                }

                if (!string.IsNullOrEmpty(errorText))
                {
                    string caption = Resources.ConnectorAccountSelection_AccountSelectionErrorCaption;
                    MessageBox.Show(this, errorText, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Set user
                _user = user;

                // Set password, ignore for stock accounts
                _password = isStockAccount ? null : password;
            }
            else
            {
                // Stock account selected
                // Be specific here, do not rely on defaults!
                _user = StockAccountUtils.GetHostingFrameworkParamForStockAccountType(
                    ConnectorRegistryUtils.ConnectorServiceStockAccount);
            }

            DialogResult = DialogResult.OK;
            this.Close();
        }

        private string GetEffectiveUserName()
        {
            string accountName = tbAccountName.Text;

            // Check for a stock account first
            KnownStockAccountType stockAccountType = StockAccountUtils.GetStockAccountFromLoginString(accountName);
            if (stockAccountType != KnownStockAccountType.None)
            {
                // Detected a stock account, return the correct HF token for this type
                accountName = StockAccountUtils.GetHostingFrameworkParamForStockAccountType(stockAccountType);
            }

            return accountName;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lnkHelpLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ConnectorUtilities.ShowAccountSelectionFormHelp();
        }

        #endregion


        #region Private Members

        private readonly AccountSelectionFormMode _mode;
        private string _user = null;
        private string _password = null;
        private AccountTypeSelection _initialSelection;
        private AccountTypeSelection _finalSelection;

        private bool _accountNameChanged = false;
        private bool _passwordChanged = false;

        #endregion

        private void rbStockAccount_CheckedChanged(object sender, EventArgs e)
        {
            // Disable unrelated controls
            SetUserAccountControlsEnabled(false);
        }

        private void rbSpecifiedAccount_CheckedChanged(object sender, EventArgs e)
        {
            // Enable related controls
            SetUserAccountControlsEnabled(true);
        }

        private void SetUserAccountControlsEnabled(bool enabled)
        {
            tbAccountName.Enabled = enabled;
            tbPassword.Enabled = enabled;
            tbConfirmPassword.Enabled = enabled;
            lblPassword.Enabled = enabled;
            lblConfirmPassword.Enabled = enabled;
        }

        private AccountTypeSelection GetAccountTypeSelection()
        {
            if (rbStockAccount.Checked)
            {
                return AccountTypeSelection.Stock;
            }
            if (rbSpecifiedAccount.Checked)
            {
                return AccountTypeSelection.User;
            }

            return AccountTypeSelection.None;
        }

        private void tbAccountName_TextChanged(object sender, EventArgs e)
        {
            _accountNameChanged = true;
        }

        private void tbPassword_TextChanged(object sender, EventArgs e)
        {
            _passwordChanged = true;
        }

        private void AccountSelectionForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _finalSelection = GetAccountTypeSelection();
        }

        /// <summary>
        /// Determine if login is a stock account
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        private static bool IsStockAccount(string login)
        {
            if (string.IsNullOrEmpty(login))
            {
                return false;
            }
            return StockAccountUtils.GetStockAccountFromLoginString(login) != KnownStockAccountType.None;
        }

        /// <summary>
        /// The string to display next to whatever stock account option we provide in the UI
        /// </summary>
        private static readonly Dictionary<KnownStockAccountType, string> StockAccountDisplayNames
            = new Dictionary<KnownStockAccountType, string>()
                   {
                      {KnownStockAccountType.LocalSystem, "Local System account"},
                      {KnownStockAccountType.LocalService, "Local Service account"},
                      {KnownStockAccountType.NetworkService, "Network Service account"}
                  };
    }
}
