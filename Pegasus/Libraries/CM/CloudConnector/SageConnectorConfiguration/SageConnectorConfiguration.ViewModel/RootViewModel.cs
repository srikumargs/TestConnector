using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using SageConnectorConfiguration.Model;

namespace SageConnectorConfiguration.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    public class RootViewModel : ViewModelBase
    {
        ReadOnlyCollection<CommandViewModel> _commands;
        RelayCommand _nextCommand;
        RelayCommand _validateUserCommand;
        RelayCommand _configureBOCommand;

        /// <summary>
        /// 
        /// </summary>
        public RootViewModel()
        {
            WizardSteps = new ObservableCollection<Step>();

            Connection = new Connection();

            Welcome = new WelcomeViewModel(this);
            WindowsAccountSelection = new WindowsAccountSelectorViewModel(this);
            Install = new InstallViewModel(this);
            Configure = new ConfigureViewModel(this);
            ConfigureTenant = new ConfigureTenantViewModel(this);
            Done = new DoneViewModel(this);

            WizardSteps.Add(Welcome);
            WizardSteps.Add(WindowsAccountSelection);
            WizardSteps.Add(Install);
            WizardSteps.Add(Configure);
            WizardSteps.Add(ConfigureTenant);
            WizardSteps.Add(Done);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Initialize()
        {
            CurrentIndex = 0; // welcome page
            NextVisibleIndex = GetNextVisibleIndex(CurrentIndex);

            // set the first step as active
            WizardSteps[CurrentIndex].IsVisible = true;
            WizardSteps[CurrentIndex].IsCurrentlySelected = true;
            WizardSteps[CurrentIndex].Initialize();
        }

        /// <summary>
        /// back office to tenant/cloud connection
        /// </summary>
        public Connection Connection { get; set; }

        Int32 _currentIndex = -1;
        /// <summary>
        /// currently selected index in the wizard steps
        /// </summary>
        public Int32 CurrentIndex
        {
            get { return _currentIndex; }
            set
            {
                if (value != _currentIndex)
                {
                    _currentIndex = value;
                    RaisePropertyChanged(() => CurrentIndex);
                }
            }
        }

        Int32 _nextVisibleIndex = -1;
        /// <summary>
        /// next visible index in the wizard steps
        /// </summary>
        public Int32 NextVisibleIndex
        {
            get { return _nextVisibleIndex; }
            set
            {
                if (value != _nextVisibleIndex)
                {
                    _nextVisibleIndex = value;
                    RaisePropertyChanged(() => NextVisibleIndex);
                }
            }
        }

        
        private bool _isBusy = false;
        /// <summary>
        /// 
        /// </summary>
        public bool IsBusy
        {
            get { return _isBusy; }

            set
            {
                if (value != _isBusy)
                {
                    _isBusy = value;
                    RaisePropertyChanged(() => IsBusy);
                }
            }
        }

        private Boolean _isCancelVisible = true;
        /// <summary>
        /// 
        /// </summary>
        public Boolean IsCancelVisible
        {
            get { return _isCancelVisible; }
            set
            {
                if (value != _isCancelVisible)
                {
                    _isCancelVisible = value;
                    RaisePropertyChanged(() => IsCancelVisible);
                }
            }
        }

        private Boolean _isCloseVisible = true;
        /// <summary>
        /// 
        /// </summary>
        public Boolean IsCloseVisible
        {
            get { return _isCloseVisible; }
            set
            {
                if (value != _isCloseVisible)
                {
                    _isCloseVisible = value;
                    RaisePropertyChanged(() => IsCloseVisible);
                }
            }
        }

        private Boolean _isInstallEnabled = true;
        /// <summary>
        /// 
        /// </summary>
        public Boolean IsInstallEnabled
        {
            get { return _isInstallEnabled; }
            set
            {
                if (value != _isInstallEnabled)
                {
                    _isInstallEnabled = value;
                    RaisePropertyChanged(() => IsInstallEnabled);
                }
            }
        }

        private Boolean _isInstallVisible = true;
        /// <summary>
        /// 
        /// </summary>
        public Boolean IsInstallVisible
        {
            get { return _isInstallVisible; }
            set
            {
                if (value != _isInstallVisible)
                {
                    _isInstallVisible = value;
                    RaisePropertyChanged(() => IsInstallVisible);
                }
            }
        }

        private Boolean _isOKVisible = true;
        /// <summary>
        /// 
        /// </summary>
        public Boolean IsOKVisible
        {
            get { return _isOKVisible; }
            set
            {
                if (value != _isOKVisible)
                {
                    _isOKVisible = value;
                    RaisePropertyChanged(() => IsOKVisible);
                }
            }
        }

        private Boolean _isOKEnabled = true;
        /// <summary>
        /// 
        /// </summary>
        public Boolean IsOKEnabled
        {
            get { return _isOKEnabled; }
            set
            {
                if (value != _isOKEnabled)
                {
                    _isOKEnabled = value;
                    RaisePropertyChanged(() => IsOKEnabled);
                }
            }
        }

        private Boolean _isConfigureVisible = true;
        /// <summary>
        /// 
        /// </summary>
        public Boolean IsConfigureVisible
        {
            get { return _isConfigureVisible; }
            set
            {
                if (value != _isConfigureVisible)
                {
                    _isConfigureVisible = value;
                    RaisePropertyChanged(() => IsConfigureVisible);
                }
            }
        }

        private Boolean _isConfigureEnabled = true;
        /// <summary>
        /// 
        /// </summary>
        public Boolean IsConfigureEnabled
        {
            get { return _isConfigureEnabled; }
            set
            {
                if (value != _isConfigureEnabled)
                {
                    _isConfigureEnabled = value;
                    RaisePropertyChanged(() => IsConfigureEnabled);
                }
            }
        }

        private Boolean _isNextVisible = true;
        /// <summary>
        /// 
        /// </summary>
        public Boolean IsNextVisible
        {
            get { return _isNextVisible; }
            set
            {
                if (value != _isNextVisible)
                {
                    _isNextVisible = value;
                    RaisePropertyChanged(() => IsNextVisible);
                }
            }
        }

        private Boolean _isPreviousVisible = true;
        /// <summary>
        /// 
        /// </summary>
        public Boolean IsPreviousVisible
        {
            get { return _isPreviousVisible; }
            set
            {
                if (value != _isPreviousVisible)
                {
                    _isPreviousVisible = value;
                    RaisePropertyChanged(() => IsPreviousVisible);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ObservableCollection<Step> WizardSteps { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public WelcomeViewModel Welcome { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public InstallViewModel Install { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ConfigureViewModel Configure { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public WindowsAccountSelectorViewModel WindowsAccountSelection { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ConfigureTenantViewModel ConfigureTenant { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DoneViewModel Done { get; set; }

        /// <summary>
        /// Returns a read-only list of commands 
        /// that the UI can display and execute.
        /// </summary>
        public ReadOnlyCollection<CommandViewModel> Commands
        {
            get
            {
                if (_commands == null)
                {
                    List<CommandViewModel> cmds = this.CreateCommands();
                    _commands = new ReadOnlyCollection<CommandViewModel>(cmds);
                }
                return _commands;
            }
        }

        List<CommandViewModel> CreateCommands()
        {
            return new List<CommandViewModel>
            {
                new CommandViewModel(
                    "Next",
                    new RelayCommand(param => this.MoveNext(), param => this.IsNextVisible)),

                new CommandViewModel(
                    "OK",
                    new RelayCommand(param => this.WindowsAccountSelection.ValidateUser(), param => (this.IsOKVisible && this.IsOKEnabled))),

                new CommandViewModel(
                    "Install",
                    new RelayCommand(param => this.Install.InstallButtonClick())),

                new CommandViewModel(
                    "Configure",
                    new RelayCommand(param => this.Configure.ConfigureButtonClick())),

                new CommandViewModel(
                    "Configure Connection",
                    new RelayCommand(param => this.ConfigureTenant.ConfigureTenantButtonClick(), param => (this.IsConfigureVisible && this.IsConfigureEnabled)))
            };
        }

        /// <summary>
        /// 
        /// </summary>
        public ICommand NextCommand
        {
            get
            {
                if (_nextCommand == null)
                {
                    _nextCommand = new RelayCommand(
                        param => this.MoveNext(),
                        param => this.IsNextVisible
                        );
                }
                return _nextCommand;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void MoveNext()
        {
            if (CurrentIndex != -1)
            {
                WizardSteps[CurrentIndex].IsVisible = false;

                if ((NextVisibleIndex != -1) && (WizardSteps.Count > NextVisibleIndex))
                {
                    WizardSteps[CurrentIndex].IsVisited = true;
                    WizardSteps[NextVisibleIndex].IsVisible = true;
                    WizardSteps[NextVisibleIndex].IsCurrentlySelected = true;
                    WizardSteps[NextVisibleIndex].Initialize();

                    CurrentIndex = NextVisibleIndex;
                    NextVisibleIndex = GetNextVisibleIndex(CurrentIndex);
                }
            }
        }

        private void GoToIndex(Int32 index)
        {
            if (CurrentIndex != -1)
            {
                WizardSteps[CurrentIndex].IsVisible = false;

                if ((index != -1) && (WizardSteps.Count > index))
                {
                    for (Int32 i = CurrentIndex; i < index; i++)
                    {
                        WizardSteps[i].IsVisited = true;
                    }

                    WizardSteps[index].IsVisible = true;
                    WizardSteps[index].IsCurrentlySelected = true;
                    WizardSteps[index].Initialize();

                    CurrentIndex = index;
                    NextVisibleIndex = GetNextVisibleIndex(CurrentIndex);
                }
            }
        }

        private Int32 GetNextVisibleIndex(Int32 currentIndex)
        {
            for (Int32 i = currentIndex + 1; i < WizardSteps.Count; i++)
            {
                if (WizardSteps[i].IsEnabled)
                {
                    return i;
                }
            }

            return -1;
        }

        private Int32 GetPreviousVisibleIndex(Int32 currentIndex)
        {
            for (Int32 i = currentIndex - 1; i < WizardSteps.Count; i--)
            {
                if (WizardSteps[i].IsEnabled)
                {
                    return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        public ICommand ValidateUserCommand
        {
            get
            {
                if (_validateUserCommand == null)
                {
                    _validateUserCommand = new RelayCommand(
                        param => this.ValidateUser(),
                        param => (this.IsOKVisible && this.IsOKEnabled)
                        );
                }
                return _validateUserCommand;
            }
        }

        private void ValidateUser()
        {
            WindowsAccountSelection.ValidateUser();
            if (WindowsAccountSelection.AllGo)
            {
                if (!ProductLibraryHelpers.InstallRequired(Connection.CurrentPackageVersion))
                    GoToIndex(3); // proceed to Configure (the 3rd step in the wizard)
                else
                    MoveNext(); // proceed to install
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ICommand ConfigureBOCommand
        {
            get
            {
                if (_configureBOCommand == null)
                {
                    _configureBOCommand = new RelayCommand(
                        param => this.ConfigureBO(),
                        param => (this.IsConfigureVisible && this.IsConfigureEnabled)
                        );
                }
                return _configureBOCommand;
            }
        }

        private void ConfigureBO()
        {
            bool success = ConfigureTenant.ConfigureTenantButtonClick();
            if (success == true)
            {
                MoveNext(); // proceed to Done
            }
        }
    }
}
