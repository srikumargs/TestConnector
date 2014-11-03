using System.Windows;
using System.Windows.Controls;
using SageConnectorConfiguration.ViewModel;

namespace SageConnectorConfiguration.View
{
    /// <summary>
    /// designer class for ConfigureTenantUserControl.xaml
    /// </summary>
    public partial class ConfigureTenantUserControl : UserControl
    {
        /// <summary>
        /// ctor
        /// </summary>
        public ConfigureTenantUserControl()
        {
            InitializeComponent();
        }

        public void Initialize(RootViewModel rootVM)
        {
            this.DataContext = rootVM.ConfigureTenant;
        }
        
        private void _backofficeIDItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.DataContext != null)
            {
                (this.DataContext as ConfigureTenantViewModel).BackofficeIDSelectionChanged();

                // TODO should fix this using binding? 
                _adminNameTextBox.IsEnabled = !ProductLibraryHelpers.PluginAdministratorAccountNameReadOnly((this.DataContext as ConfigureTenantViewModel).SelectedBackofficeID); // TODO this should be fixed using binding
                _adminPasswordTextBox.Clear();
                _userNameTextBox.Text = string.Empty;
                _passwordTextBox.Clear();
                _boConnectionInfoGrid.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        /// <summary>
        /// capture the back office admin password
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void AdminPasswordChangedHandler(object sender, RoutedEventArgs args)
        {
            (this.DataContext as ConfigureTenantViewModel).BackofficeAdminPassword = (sender as PasswordBox).Password;
        }

        /// <summary>
        /// capture the back office user password
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void UserPasswordChangedHandler(object sender, RoutedEventArgs args)
        {
            (this.DataContext as ConfigureTenantViewModel).BackofficeUserPassword = (sender as PasswordBox).Password;
        }

        private void validateButton_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as ConfigureTenantViewModel;
            vm.ValidateBOConnection();

            // refresh combo binding
            _companyNameComboBox.SelectedIndex = 0;
        }
    }
}
