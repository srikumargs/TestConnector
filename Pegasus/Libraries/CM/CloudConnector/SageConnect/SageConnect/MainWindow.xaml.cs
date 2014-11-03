using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using Sage.Connector.Management;
using SageConnect.Internal;
using SageConnect.ViewModels;
using Application = System.Windows.Application;

namespace SageConnect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
// ReSharper disable once RedundantExtendsListEntry
    public partial class MainWindow : Window
    {
        private ProgressBarWorker _progressbar;


        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
            if (ApplicationHelpers.IsapplicationAlreadyRunning())
                Application.Current.Shutdown();
            ProgressBarWorker progesBarWorker =
             new ProgressBarWorker();
            progesBarWorker.StartConnecterServices();
         
            InitializeComponent();
            IntitlizePagedesign();
          
            AnimateGrid.LoginForm = LoginFormColumnDefinition;
            AnimateGrid.ConnectionForm = ConnectionColumnDefinition;
            AnimateGrid.DetailColumn = DetailsColumn;
            AnimateGrid.ConnectionsColumn = ConnectionsColumn;
            AnimateGrid.ConnectionStatusDataGrid = ConnectionStatusDataGrid;
            AnimateGrid.ConnectionMessageBlock = ConnectionMessageBlock;
            //ErrorMessageViewModel.ErrorMessageViewModelInstance.ShowInformationMessage(CustomerFacingMessages.InfoMessage_TenantConnectionExists, CustomerFacingMessages.InfoCaption_ConnectionSaved, HelpLinkManager.FindString("Connetion_HelpFile")); 
            ErrorMessageViewModel.ErrorMessageViewModelInstance.HideAllControls();
            CheckforBackOffice();
        }

        private void CheckforBackOffice()
        {
            if (new BackOfficeServiceManager().GetBackOfficePlugins() == null)
            {
                ErrorMessageViewModel.ErrorMessageViewModelInstance.ShowErrorMessage(CustomerFacingMessages.SignIn_BackofficeMissing, CustomerFacingMessages.SignIn_BackofficeMissing_Header);
                ErrorMessagectl.CancelButtonClick += BackOfficeCheckClose;
            }
        }

        private void BackOfficeCheckClose(object sender, RoutedEventArgs routedEventArgs)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// 
        /// </summary>
        private void IntitlizePagedesign()
        {

            ConnectionStatusDataGrid.CanUserAddRows = false;
            ConnectionStatusDataGrid.FrozenColumnCount = 4;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DisplayConnectionDetails_Event(object sender, RoutedEventArgs e)
        {

            AnimateGrid.AnimateGridColumnExpandCollapse(DetailsColumn, true, 450, 0, 0, 0, 250);
           
            //Getting the data form the grid to identify the tenant
            //DataGridRow dataGridRow = (DataGridRow) sender;
            DataGrid dataGrid = (DataGrid)sender;
            int itemcount= dataGrid.Items.Count;
            ConfigurationViewModel tenant = (ConfigurationViewModel)dataGrid.SelectedValue;
            //ConfigurationViewModel tenant = (ConfigurationViewModel) dataGridRow.Item;
            //dataGridRow.IsSelected = true;
            // Calling the Connection to create or view the exiting connection.
            ConnectionsViewModel.InstanceConnectionsViewModel.ConfigurationViewModel = tenant;
            ConnectionSettings.Visibility = Visibility.Visible;
            DisplayControl.Visibility = Visibility.Visible;

            ConnectionSettings.EditSettingsClick -= ConnectionsViewModel.InstanceConnectionsViewModel.EditSettingsClick;
            ConnectionSettings.EditSettingsClick += ConnectionsViewModel.InstanceConnectionsViewModel.EditSettingsClick;
            ConnectionSettings.ClearSettingsClick -=
                ConnectionsViewModel.InstanceConnectionsViewModel.ClearSettingsClick;
            ConnectionSettings.ClearSettingsClick +=
                ConnectionsViewModel.InstanceConnectionsViewModel.ClearSettingsClick;
            ConfirmationDialogControl.ConfirmSettingsClick +=
                ConnectionsViewModel.InstanceConnectionsViewModel.ConfirmSettingsClick;
            ConnectionsViewModel.InstanceConnectionsViewModel.ContentControl = DisplayControl;
            ConnectionsViewModel.InstanceConnectionsViewModel.Connectionselect(itemcount);
            SetRightImage();
        }




        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }


        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(HelpLinkManager.FindString("Connetion_HelpFile"));
        }


        private void SignOutButton_OnClick(object sender, RoutedEventArgs e)
        {
            ConnectionsViewModel.InstanceConnectionsViewModel.SignOut();
            DisplayControl.Visibility = Visibility.Hidden;
            AnimateGrid.CollapseConnectionDetails();
            AnimateGrid.ExpandLoginForm();

        }


        private void ErrorMessageControl_OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            ErrorMessageViewModel.ErrorMessageViewModelInstance.DisplayErrorMessage = Visibility.Hidden;
            if (ErrorMessageViewModel.ErrorMessageViewModelInstance.MonitorOpenstatus)
            {
                Process.Start(ApplicationHelpers.SelectedSiteGroup.DataCloudWebPageUri.ToString());
                ApplicationHelpers.OpenMonitorRunning(false);
            }
        }

        private void DragWindowEvent(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
      
        private void OpenMonitor_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(ApplicationHelpers.SelectedSiteGroup.DataCloudWebPageUri.ToString());
            //ApplicationHelpers.OpenMonitorRunning();
        }

        private void Splitter_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Math.Abs(DetailsColumn.Width.Value) <= 0)
            {
                AnimateGrid.ExpandConnectionDetails();

                SetRightImage();
            }
            else
            {

                AnimateGrid.CollapseConnectionDetails();
                Uri resourceUri = new Uri("Resources/Images/divider_Left.png", UriKind.Relative);
                StreamResourceInfo streamInfo = Application.GetResourceStream(resourceUri);

                if (streamInfo != null)
                {
                    BitmapFrame temp = BitmapFrame.Create(streamInfo.Stream);
                    var brush = new ImageBrush { ImageSource = temp };
                    Splitter.Background = brush;
                }
                //Splitter.Background = (Brush)FindResource(new Uri(@"Resources/Images/divider_right.png"));
            }
        }
        private void SetRightImage()
        {
            Thread.Sleep(1000);
            Uri resourceUri = new Uri("Resources/Images/divider_right.png", UriKind.Relative);
            StreamResourceInfo streamInfo = Application.GetResourceStream(resourceUri);

            if (streamInfo != null)
            {
                BitmapFrame temp = BitmapFrame.Create(streamInfo.Stream);
                var brush = new ImageBrush { ImageSource = temp };
                Splitter.Background = brush;
            }
        }
        private void Sigin_MouseEnter(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            button.Background = (SolidColorBrush) new BrushConverter().ConvertFrom("#379536");
            button.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#565656");
            button.BorderThickness = new Thickness(1);
        }
        private void Sigin_MouseLeave(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            button.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#41a940");
            button.BorderThickness = new Thickness(0);
        }
        private void Sigin_MouseUP(object sender, MouseEventArgs e)
        {
            Button button = (Button)sender;
            button.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#30832f");
            button.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFrom("#3a3a3a");
            button.BorderThickness = new Thickness(1);
           
        }

        private void GettingStartedTextBlock_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Process.Start(HelpLinkManager.FindString("Signin_GettingstartedHelpUri"));
        }

        private void Signin_click(object sender, RoutedEventArgs e)
        {
            if (_progressbar == null) _progressbar = new ProgressBarWorker();
            _progressbar.StartProgressbar();
        }
    }

}
