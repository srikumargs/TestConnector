using System;
using System.DirectoryServices.AccountManagement;
using System.Xml;

namespace SageConnectorConfiguration.ViewModel
{
    /// <summary>
    /// View model for the windows account selector
    /// </summary>
    public class WindowsAccountSelectorViewModel : Step
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rootViewModel"></param>
        public WindowsAccountSelectorViewModel(RootViewModel rootViewModel)
        {
            RootViewModel = rootViewModel;
            Name = "Windows Account Selection";
            ID = "WindowsAccountSelection";
        }

        /// <summary>
        /// Whether this is an explicit user account
        /// </summary>
        public bool IsUserAccount
        {
            get { return RootViewModel.Connection.IsUserAccount; }

            set
            {
                if (value != RootViewModel.Connection.IsUserAccount)
                {
                    RootViewModel.Connection.IsUserAccount = value;
                    RaisePropertyChanged(() => IsUserAccount);
                }
            }
        }

        private bool _isLocalAccount = false;
        /// <summary>
        /// Whether this is a predefined local account
        /// </summary>
        public bool IsLocalAccount
        {
            get { return _isLocalAccount; }

            set
            {
                if (value != _isLocalAccount)
                {
                    _isLocalAccount = value;
                    RaisePropertyChanged(() => IsLocalAccount);
                }
            }
        }

        /// <summary>
        /// Whether this is a predefined local system account
        /// </summary>
        public bool IsLocalSystemAccount
        {
            get { return RootViewModel.Connection.IsLocalSystemAccount; }

            set
            {
                if (value != RootViewModel.Connection.IsLocalSystemAccount)
                {
                    RootViewModel.Connection.IsLocalSystemAccount = value;
                    RaisePropertyChanged(() => IsLocalSystemAccount);
                }
            }
        }

        private bool _invalidUserAccount = false;

        /// <summary>
        /// Whether the user is valid
        /// </summary>
        public bool InvalidUserAccount
        {
            get { return _invalidUserAccount; }

            set
            {
                if (value != _invalidUserAccount)
                {
                    _invalidUserAccount = value;
                    RaisePropertyChanged(() => InvalidUserAccount);
                }
            }
        }

        private bool _invalidPassword = false;

        /// <summary>
        /// Whether the password is valid
        /// </summary>
        public bool InvalidPassword
        {
            get { return _invalidPassword; }

            set
            {
                if (value != _invalidPassword)
                {
                    _invalidPassword = value;
                    RaisePropertyChanged(() => InvalidPassword);
                }
            }
        }

        private bool _passwordsDontMatch = false;

        /// <summary>
        /// Whether the two passwords matches
        /// </summary>
        public bool PasswordsDontMatch
        {
            get { return _passwordsDontMatch; }

            set
            {
                if (value != _passwordsDontMatch)
                {
                    _passwordsDontMatch = value;
                    RaisePropertyChanged(() => PasswordsDontMatch);
                }
            }
        }

        /// <summary>
        /// The specific user account
        /// </summary>
        public String UserAccount
        {
            get { return RootViewModel.Connection.WindowsUserAccount; }

            set
            {
                if (value != RootViewModel.Connection.WindowsUserAccount)
                {
                    RootViewModel.Connection.WindowsUserAccount = value;
                    RaisePropertyChanged(() => UserAccount);
                }
            }
        }

        /// <summary>
        /// The specific user password
        /// </summary>
        public String Password
        {
            get { return RootViewModel.Connection.WindowsPassword; }

            set
            {
                if (value != RootViewModel.Connection.WindowsPassword)
                {
                    RootViewModel.Connection.WindowsPassword = value;
                    RaisePropertyChanged(() => Password);
                }
            }
        }

        private String _confirmPassword;
        /// <summary>
        /// The confirmed user password
        /// </summary>
        public String ConfirmPassword
        {
            get { return _confirmPassword; }

            set
            {
                if (value != _confirmPassword)
                {
                    _confirmPassword = value;
                    RaisePropertyChanged(() => ConfirmPassword);
                }
            }
        }

        /// <summary>
        /// The configuration file location
        /// </summary>
        public String ConfigurationLocation
        {
            get { return RootViewModel.Connection.ConfigurationLocation; }

            set
            {
                if (value != RootViewModel.Connection.ConfigurationLocation)
                {
                    RootViewModel.Connection.ConfigurationLocation = value;
                    RaisePropertyChanged(() => ConfigurationLocation);
                }
            }
        }

        private bool _invalidConfigurationFileLocation = false;
        /// <summary>
        /// Whether the specified configuration file location is valid
        /// </summary>
        public bool InvalidConfigurationFileLocation
        {
            get { return _invalidConfigurationFileLocation; }
            set
            {
                if (value != _invalidConfigurationFileLocation)
                {
                    _invalidConfigurationFileLocation = value;
                    RaisePropertyChanged(() => InvalidConfigurationFileLocation);
                }
            }
        }

        private bool _validXML = false;
        /// <summary>
        /// the user picked a valid configuration file
        /// </summary>
        public bool ValidXML
        {
            get { return _validXML; }
            set
            {
                if (value != _validXML)
                {
                    _validXML = value;
                    RaisePropertyChanged(() => ValidXML);
                }
            }
        }

        private string _tenantName;
        /// <summary>
        /// The cloud tenant name
        /// </summary>
        public string TenantName
        {
            get { return _tenantName; }

            set
            {
                if (value != _tenantName)
                {
                    _tenantName = value;
                    RaisePropertyChanged(() => TenantName);
                }
            }
        }

        private string _tenantURL;
        /// <summary>
        /// The cloud tenant URL
        /// </summary>
        public string TenantURL
        {
            get { return _tenantURL; }

            set
            {
                if (value != _tenantURL)
                {
                    _tenantURL = value;
                    RaisePropertyChanged(() => TenantURL);
                }
            }
        }

        private bool _allGo = false;
        /// <summary>
        /// Whether the two passwords matches
        /// </summary>
        public bool AllGo
        {
            get { return _allGo; }

            set
            {
                if (value != _allGo)
                {
                    _allGo = value;
                    RaisePropertyChanged(() => AllGo);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Initialize()
        {
            // this control is about to display
            RootViewModel.IsNextVisible = false;
            RootViewModel.IsPreviousVisible = false;
            RootViewModel.IsCloseVisible = false;
            RootViewModel.IsCancelVisible = true;
            RootViewModel.IsInstallVisible = false;
            RootViewModel.IsInstallEnabled = false;
            RootViewModel.IsConfigureEnabled = false;
            RootViewModel.IsConfigureVisible = false;
            RootViewModel.IsOKEnabled = true;
            RootViewModel.IsOKVisible = true;

            InvalidConfigurationFileLocation = false;
            ConfigurationLocation = string.Empty;

            //ConfigureForOS(vm); // moved to designer code

            // TODO: prompt for server name and credentials only when connector service is not running...??
        }

        // TODO should move this validation to the connection?
        /// <summary>
        /// 
        /// </summary>
        public void ValidateUser()
        {
            RootViewModel.IsBusy = true;
            bool success = true;

            InvalidPassword = false;
            InvalidUserAccount = false;
            PasswordsDontMatch = false;

            if (IsUserAccount)
            {
                if (Password == null)
                {
                    InvalidPassword = true;
                    success = false;
                }

                // validate credentials and make sure passwords match
                if (success && (Password != ConfirmPassword))
                {
                    PasswordsDontMatch = true;
                    success = false;
                }

                if (success)
                {
                    using (PrincipalContext context = new PrincipalContext(ContextType.Domain))
                    {
                        try
                        {
                            success = context.ValidateCredentials(UserAccount, Password);
                        }
                        catch (Exception)
                        {
                            InvalidUserAccount = true;
                        }
                    }
                    InvalidUserAccount = !success;
                }
            }

            RootViewModel.IsBusy = false;

            ValidateXML();
            if (success && ValidXML)
                AllGo = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void BrowseButton()
        {
            // Set and restore current directory after browse operation
            var currentDirectory = System.IO.Directory.GetCurrentDirectory();
            try
            {
                var browseDlg = new Microsoft.Win32.OpenFileDialog();
                browseDlg.CheckFileExists = true;
                browseDlg.CheckPathExists = true;
                browseDlg.DefaultExt = ".xml";
                browseDlg.Filter = "XML Files (*.xml)|*.xml";
                browseDlg.Multiselect = false;
                browseDlg.Title = "Sage Configuration File";

                var result = browseDlg.ShowDialog();

                if (result == true)
                {
                    ConfigurationLocation = browseDlg.FileName;

                    ValidateXMLFile();
                }       
            }
            finally
            {
                System.IO.Directory.SetCurrentDirectory(currentDirectory);
            }
        }

        private void ValidateXMLFile()
        {
            RootViewModel.Connection.TenantName =
                RootViewModel.Connection.TenantURL =
                RootViewModel.Connection.ConnectionKey =
                RootViewModel.Connection.CurrentPackageVersion = string.Empty;

            GetTenantConfiguration();
            ValidateXML();

            if (ValidXML)
            {
                TenantName = RootViewModel.Connection.TenantName;
                TenantURL = RootViewModel.Connection.TenantURL;
            }
        }

        private void GetTenantConfiguration()
        {
            using (var reader = new XmlTextReader(RootViewModel.Connection.ConfigurationFileLocation))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        switch (reader.Name)
                        {
                            case "Name":
                                reader.Read();
                                if (reader.NodeType == XmlNodeType.Text)
                                    RootViewModel.Connection.TenantName = reader.Value;
                                break;
                            case "URL":
                                reader.Read();
                                if (reader.NodeType == XmlNodeType.Text)
                                    RootViewModel.Connection.TenantURL = reader.Value;
                                break;
                            case "ConnectionKey":
                                reader.Read();
                                if (reader.NodeType == XmlNodeType.Text)
                                    RootViewModel.Connection.ConnectionKey = reader.Value;
                                break;
                            case "ProductVersion":
                                reader.Read();
                                if (reader.NodeType == XmlNodeType.Text)
                                    RootViewModel.Connection.CurrentPackageVersion = reader.Value;
                                break;
                        }
                    }
                }
            }
        }

        private void ValidateXML()
        {
            ValidXML = !(String.IsNullOrEmpty(RootViewModel.Connection.TenantName)) &&
                       !(String.IsNullOrEmpty(RootViewModel.Connection.TenantURL)) &&
                       !(String.IsNullOrEmpty(RootViewModel.Connection.ConnectionKey)) &&
                       !(String.IsNullOrEmpty(RootViewModel.Connection.CurrentPackageVersion));
            InvalidConfigurationFileLocation = !ValidXML;
        }
    }
}
