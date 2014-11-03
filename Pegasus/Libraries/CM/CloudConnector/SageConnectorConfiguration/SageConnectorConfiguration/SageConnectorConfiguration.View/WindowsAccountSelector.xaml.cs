using System;
using System.Windows;
using System.Windows.Controls;
using SageConnectorConfiguration.ViewModel;

namespace SageConnectorConfiguration.View
{
    /// <summary>
    /// designer class for WindowsAccountSelector.xaml
    /// </summary>
    public partial class WindowsAccountSelector : UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        public WindowsAccountSelector()
        {
            InitializeComponent();
            ConfigureForOS();
        }

        public void Initialize(RootViewModel rootVM)
        {
            this.DataContext = rootVM.WindowsAccountSelection;
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
                _userAccountRadioBtn.IsChecked = true;
                _localSystemAccountRadioBtn.IsEnabled = false;
            }
            else
            {
                _localSystemAccountRadioBtn.IsChecked = true;
            }
        }

        /// <summary>
        /// capture password 
            // TODO could change this to use the properties together with the ok button onChange event in order to enable
            //  the OK button only if valid and matching passwords ??
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void PasswordChangedHandler(object sender, RoutedEventArgs args)
        {
            (this.DataContext as WindowsAccountSelectorViewModel).Password = (sender as PasswordBox).Password;
        }

        /// <summary>
        /// capture password again in the confirmation box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void ConfirmPasswordChangedHandler(object sender, RoutedEventArgs args)
        {
            (this.DataContext as WindowsAccountSelectorViewModel).ConfirmPassword = (sender as PasswordBox).Password;
        }

        private void BrowseButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            (this.DataContext as WindowsAccountSelectorViewModel).BrowseButton();
        }
    }
}
