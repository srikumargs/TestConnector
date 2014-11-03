using System;

namespace SageConnectorConfiguration.ViewModel
{
    /// <summary>
    /// View model for the Welcome user control
    /// </summary>
    public class WelcomeViewModel : Step
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="rootViewModel"></param>
        public WelcomeViewModel(RootViewModel rootViewModel)
        {
            RootViewModel = rootViewModel;
            Name = "Welcome";
            ID = "Welcome";
        }

        private bool _boNotInstalled = false;
        /// <summary>
        /// Whether a back office exists
        /// </summary>
        public bool BONotInstalled
        {
            get { return _boNotInstalled; }

            set
            {
                if (value != _boNotInstalled)
                {
                    _boNotInstalled = value;
                    RaisePropertyChanged(() => BONotInstalled);
                }
            }
        }

        private bool _allGo;
        /// <summary>
        /// is the wizard valid to launch?
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
            RootViewModel.IsPreviousVisible = false;
            RootViewModel.IsCloseVisible = false;
            RootViewModel.IsCancelVisible = true;
            RootViewModel.IsInstallVisible = false;
            RootViewModel.IsInstallEnabled = false;
            RootViewModel.IsConfigureEnabled = false;
            RootViewModel.IsConfigureVisible = false;
            RootViewModel.IsOKEnabled = false;
            RootViewModel.IsOKVisible = false;

            RootViewModel.IsNextVisible = ProductLibraryHelpers.BackOfficeInstalled();
            RootViewModel.Welcome.BONotInstalled = !RootViewModel.IsNextVisible;
            RootViewModel.Welcome.AllGo = RootViewModel.IsNextVisible;
        }
    }
}
