using System;
using System.Collections.ObjectModel;
using SageConnectorConfiguration.Model;

namespace SageConnectorConfiguration.ViewModel
{
    /// <summary>
    /// Configure Tenant View Model
    /// </summary>
    public class ConfigureTenantViewModel : Step
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootViewModel"></param>
        public ConfigureTenantViewModel(RootViewModel rootViewModel)
        {
            RootViewModel = rootViewModel;
            Name = "Tenant Configuration";
            ID = "BOConfiguration";

            BackofficeIDs = new ObservableCollection<String>();
            CompanyNames = new ObservableCollection<String>();
            Connections = new ObservableCollection<BackOfficeConnection>();
        }

        /// <summary>
        /// Collection of installed back office types
        /// </summary>
        public ObservableCollection<String> BackofficeIDs { get; set; }

        /// <summary>
        /// Collection of company names (for the selected back off type)
        /// </summary>
        /// 
        public ObservableCollection<String> CompanyNames { get; set; }

        /// <summary>
        /// Collection of connections (for the selected back off type)
        /// </summary>
        public ObservableCollection<BackOfficeConnection> Connections { get; set; }

        /// <summary>
        /// The currently selected back office type
        /// </summary>
        public String SelectedBackofficeID
        {
            get { return RootViewModel.Connection.BOID; }

            set
            {
                if (value != RootViewModel.Connection.BOID)
                {
                    RootViewModel.Connection.BOID = value;
                    RaisePropertyChanged(() => SelectedBackofficeID);
                }
            }
        }

        /// <summary>
        /// The back office administrator user name
        /// </summary>
        public String BackofficeAdminName
        {
            get { return RootViewModel.Connection.BOAdminName; }

            set
            {
                if (value != RootViewModel.Connection.BOAdminName)
                {
                    RootViewModel.Connection.BOAdminName = value;
                    RaisePropertyChanged(() => BackofficeAdminName);
                }
            }
        }

        /// <summary>
        /// The back office administrator user password
        /// </summary>
        public String BackofficeAdminPassword
        {
            get { return RootViewModel.Connection.BOAdminPassword; }

            set
            {
                if (value != RootViewModel.Connection.BOAdminPassword)
                {
                    RootViewModel.Connection.BOAdminPassword = value;
                    RaisePropertyChanged(() => BackofficeAdminPassword);
                }
            }
        }

        /// <summary>
        /// The selected back office data connection
        /// </summary>
        public BackOfficeConnection SelectedConnection
        {
            get { return RootViewModel.Connection.SelectedConnection; }

            set
            {
                if (value != RootViewModel.Connection.SelectedConnection)
                {
                    RootViewModel.Connection.SelectedConnection = value;
                    RaisePropertyChanged(() => SelectedConnection);
                }
            }
        }

        /// <summary>
        /// The back office user (connector) name
        /// </summary>
        public String BackofficeUserName
        {
            get { return RootViewModel.Connection.BOUserName; }

            set
            {
                if (value != RootViewModel.Connection.BOUserName)
                {
                    RootViewModel.Connection.BOUserName = value;
                    RaisePropertyChanged(() => BackofficeUserName);
                }
            }
        }

        /// <summary>
        /// The back office (connector) password
        /// </summary>
        public String BackofficeUserPassword
        {
            get { return RootViewModel.Connection.BOUserPassword; }

            set
            {
                if (value != RootViewModel.Connection.BOUserPassword)
                {
                    RootViewModel.Connection.BOUserPassword = value;
                    RaisePropertyChanged(() => BackofficeUserPassword);
                }
            }
        }

        private string _boAdminTermID;
        /// <summary>
        /// 
        /// </summary>
        public string BOAdminTermID
        {
            get { return _boAdminTermID; }
            set
            {
                if (value != _boAdminTermID)
                {
                    _boAdminTermID = value;
                    RaisePropertyChanged(() => BOAdminTermID);
                }
            }
        }

        private string _boAdminTermPassword;
        /// <summary>
        /// 
        /// </summary>
        public string BOAdminTermPassword
        {
            get { return _boAdminTermPassword; }
            set
            {
                if (value != _boAdminTermPassword)
                {
                    _boAdminTermPassword = value;
                    RaisePropertyChanged(() => BOAdminTermPassword);
                }
            }
        }

        private bool _BOconnectionFailed = false;
        /// <summary>
        /// Whether the back office connection validation has failed
        /// </summary>
        public bool BOConnectionFailed
        {
            get { return _BOconnectionFailed; }
            set
            {
                if (value != _BOconnectionFailed)
                {
                    _BOconnectionFailed = value;
                    RaisePropertyChanged(() => BOConnectionFailed);
                }
            }
        }

        private string _BOconnectionFailedMessage = "Failed to connect to the back office. Invalid user ID or password for the selected data connection.";
        /// <summary>
        /// user facing error message for when the back office connection validation fails
        /// </summary>
        public string BOConnectionFailedMessage
        {
            get { return _BOconnectionFailedMessage; }
            set
            {
                if (value != _BOconnectionFailedMessage)
                {
                    _BOconnectionFailedMessage = value;
                    RaisePropertyChanged(() => BOConnectionFailedMessage);
                }
            }
        }

        private bool _BOConnectionOK = false;
        /// <summary>
        /// used for binding with the BO info grid for visibility
        /// </summary>
        public bool BOConnectionOK
        {
            get { return _BOConnectionOK; }
            set
            {
                if (value != _BOConnectionOK)
                {
                    _BOConnectionOK = value;
                    RaisePropertyChanged(() => BOConnectionOK);
                }
            }
        }

        private bool _tenantConnectionFailed = false;
        /// <summary>
        /// Whether the cloud connection validation has failed
        /// </summary>
        public bool TenantConnectionFailed
        {
            get { return _tenantConnectionFailed; }
            set
            {
                if (value != _tenantConnectionFailed)
                {
                    _tenantConnectionFailed = value;
                    RaisePropertyChanged(() => TenantConnectionFailed);
                }
            }
        }

        //TODO: this should be in a resource if this dll is part of shipping product
        private string _tenantConnectionFailedMessage = "Failed to connect to the cloud. Invalid tenant or the cloud site is not running.";
        /// <summary>
        /// user facing error message when the cloud connection validation failed
        /// </summary>
        public string TenantConnectionFailedMessage
        {
            get { return _tenantConnectionFailedMessage; }
            set
            {
                if (value != _tenantConnectionFailedMessage)
                {
                    _tenantConnectionFailedMessage = value;
                    RaisePropertyChanged(() => TenantConnectionFailedMessage);
                }
            }
        }

        private bool _noDataConnections = false;
        /// <summary>
        /// Whether we could find any back office data connections
        /// </summary>
        public bool NoDataConnections
        {
            get { return _noDataConnections; }
            set
            {
                if (value != _noDataConnections)
                {
                    _noDataConnections = value;
                    RaisePropertyChanged(() => NoDataConnections);
                }
            }
        }

        private bool _noBackofficeIDs = false;
        /// <summary>
        /// Whether we could find any back office installed
        /// </summary>
        public bool NoBackofficeIDs
        {
            get { return _noBackofficeIDs; }
            set
            {
                if (value != _noBackofficeIDs)
                {
                    _noBackofficeIDs = value;
                    RaisePropertyChanged(() => NoBackofficeIDs);
                }
            }
        }

        private bool _createTenantFailed = false;
        /// <summary>
        /// Whether the tenant was created
        /// </summary>
        public bool CreateTenantFailed
        {
            get { return _createTenantFailed; }
            set
            {
                if (value != _createTenantFailed)
                {
                    _createTenantFailed = value;
                    RaisePropertyChanged(() => CreateTenantFailed);
                }
            }
        }

        private bool _adminCredentialsFailed = false;
        /// <summary>
        /// Whether the back office administrator credentials validation was okay
        /// </summary>
        public bool AdminCredentialsFailed
        {
            get { return _adminCredentialsFailed; }
            set
            {
                if (value != _adminCredentialsFailed)
                {
                    _adminCredentialsFailed = value;
                    RaisePropertyChanged(() => AdminCredentialsFailed);
                }
            }
        }

        private string _adminCredentialsFailedMessage = "Invalid back office ID or password.";
        /// <summary>
        /// user facing error message for when the back office administrator credentials validation fails
        /// </summary>
        public string AdminCredentialsFailedMessage
        {
            get { return _adminCredentialsFailedMessage; }
            set
            {
                if (value != _adminCredentialsFailedMessage)
                {
                    _adminCredentialsFailedMessage = value;
                    RaisePropertyChanged(() => AdminCredentialsFailedMessage);
                }
            }
        }

        /// <summary>
        /// Whether any of the checks have failed in order to display troubleshooting link.
        /// </summary>
        private bool _checksFailed = false;
        /// <summary>
        /// 
        /// </summary>
        public bool ChecksFailed
        {
            get { return _checksFailed; }
            set
            {
                if (value != _checksFailed)
                {
                    _checksFailed = value;
                    RaisePropertyChanged(() => ChecksFailed);
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
            RootViewModel.IsConfigureVisible = true;
            RootViewModel.IsOKEnabled = false;
            RootViewModel.IsOKVisible = false;

            ConfigureBackOfficeTypes(RootViewModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vm"></param>
        public void ConfigureBackOfficeTypes(RootViewModel vm)
        {
            var pluginIds = ProductLibraryHelpers.InstalledConnectorPluginIds();
            if (null != pluginIds)
            {
                BackofficeIDs.Clear();
                foreach (var pluginId in pluginIds)
                    BackofficeIDs.Add(pluginId);
            }

            NoBackofficeIDs = false;
            if (BackofficeIDs.Count == 0)
            {
                NoBackofficeIDs = true;
            }
            else
            {
                SelectedBackofficeID = BackofficeIDs[0];
                //if (BackofficeIDs.Count == 1)
                //    _backofficeIDItems.IsEnabled = false; // TODO: Hide instead? // TODO fix this with binding
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool ValidateBOConnection()
        {
            RootViewModel.IsBusy = true;
            AdminCredentialsFailed = false;

            ValidationResponse response = ProductLibraryHelpers.ValidateBackOfficeAdministrator( BackofficeAdminName,
                                                                                             BackofficeAdminPassword,
                                                                                             SelectedBackofficeID);
            if (response.Success)
            {
                BOConnectionOK = true;

                var connections = ProductLibraryHelpers.RetrieveBackOfficeConnections(BackofficeAdminName,
                                                                                  BackofficeAdminPassword,
                                                                                  SelectedBackofficeID);

                if (connections != null)
                {
                    Connections.Clear();
                    foreach (var connection in connections)
                        Connections.Add(connection);
                }

                if (Connections.Count == 0)
                {
                    NoDataConnections = true;
                    ChecksFailed = true;
                }
                else
                {
                    RootViewModel.IsConfigureEnabled = true;
                }
            }
            else
            {
                AdminCredentialsFailed = true;
                AdminCredentialsFailedMessage = response.UserFacingError;

                RootViewModel.IsConfigureEnabled = false;
                BOConnectionOK = false;
            }

            RootViewModel.IsBusy = false;
            return response.Success;
        }

        /// <summary>
        /// Validate back office and cloud connections and create the tenant configuration record
        /// </summary>
        public bool ConfigureTenantButtonClick()
        {
            RootViewModel.IsBusy = true;
            bool success = true;
            ResetAllFlags();

            ValidationResponse BOConnectionResponse = ProductLibraryHelpers.ValidateBackOfficeConnection(SelectedBackofficeID, 
                                                                                                    BackofficeUserName,
                                                                                                    BackofficeUserPassword,
                                                                                                    SelectedConnection.ConnectionInformation                                                                             
                                                                                                    );
            if (!BOConnectionResponse.Success)
            {
                BOConnectionFailed = true;
                BOConnectionFailedMessage = BOConnectionResponse.UserFacingError;
                success = BOConnectionResponse.Success;
            }

            if (success)
            {
                // validate tenant connection
                ValidationResponse TenantConnectionResponse = ProductLibraryHelpers.ValidateTenantConnection(RootViewModel.Connection.ConnectionKey);

                if (!TenantConnectionResponse.Success)
                {
                    TenantConnectionFailed = true;
                    TenantConnectionFailedMessage = TenantConnectionResponse.UserFacingError;
                    success = TenantConnectionResponse.Success;
                }
            }

            try
            {
                if (success)
                    CreateTenant(); // this will also update an existing tenant
            }
            catch (Exception)
            {
                CreateTenantFailed = true;
                success = false;
            }

            ChecksFailed = !success;
            RootViewModel.IsBusy = false;
            return success;
        }

        private void CreateTenant()
        {
            ProductLibraryHelpers.SaveNewTenantConfiguration(
                BackofficeUserName,
                BackofficeUserPassword,
                SelectedConnection.Name, // backOfficeConnectionName
                SelectedConnection.DisplayName, // backOfficeConnectionDisplayable
                SelectedConnection.ConnectionInformation, // backOfficeConnectionInformation
                RootViewModel.Connection.TenantName,
                RootViewModel.Connection.TenantURL,
                RootViewModel.Connection.ConnectionKey,
                SelectedBackofficeID);
        }

        /// <summary>
        /// 
        /// </summary>
        public void BackofficeIDSelectionChanged()
        {
            if (SelectedBackofficeID != null)
            {
                var adminTerm = ProductLibraryHelpers.PluginAdministratorTerm(SelectedBackofficeID);

                BOAdminTermID = adminTerm + " ID:";
                BOAdminTermPassword = adminTerm + " password:";

                var adminNamePrefill = ProductLibraryHelpers.PluginAdministratorAccountNamePrefill(SelectedBackofficeID);
                if (!String.IsNullOrEmpty(adminNamePrefill))
                    BackofficeAdminName = adminNamePrefill;
                else
                    BackofficeAdminName = String.Empty;

                if (Connections != null)
                    Connections.Clear();

                ResetAllFlags();
            }
        }

        private void ResetAllFlags()
        {
            AdminCredentialsFailed = false;
            NoBackofficeIDs = false;
            NoDataConnections = false;
            ChecksFailed = false;
            BOConnectionFailed = false;
            TenantConnectionFailed = false;
            CreateTenantFailed = false;
        }
    }
}
