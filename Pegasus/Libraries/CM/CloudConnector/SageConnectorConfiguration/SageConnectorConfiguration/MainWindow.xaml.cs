using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using SageConnectorConfiguration.ViewModel;

namespace SageConnectorConfiguration
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            RootViewModel = new RootViewModel();
            //RootViewModel.MainWindow = this;

            this.DataContext = RootViewModel;

            _welcomeUserControl.Initialize(RootViewModel);
            _windowsAccountSelectorControl.Initialize(RootViewModel);
            _installUserControl.Initialize(RootViewModel);
            _configureUserControl.Initialize(RootViewModel);
            _configureTenantUserControl.Initialize(RootViewModel);
            _doneUserControl.Initialize(RootViewModel);

            _contentUserControls.Add(_welcomeUserControl);
            _contentUserControls.Add(_windowsAccountSelectorControl);
            _contentUserControls.Add(_installUserControl);
            _contentUserControls.Add(_configureUserControl);
            _contentUserControls.Add(_configureTenantUserControl);
            _contentUserControls.Add(_doneUserControl);

            this.RootViewModel.Initialize();

            CommandBinding closeCommand = new CommandBinding(ApplicationCommands.Close);
            this.CommandBindings.Add(closeCommand);
            closeCommand.Executed += closeCommand_Executed;
        }

        void closeCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private List<UserControl> _contentUserControls = new List<UserControl>();

        /// <summary>
        /// 
        /// </summary>
        public RootViewModel RootViewModel { get; set; }

        private void _listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}
