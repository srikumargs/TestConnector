using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Threading;

namespace SageConnectorConfiguration.ViewModel
{
    /// <summary>
    /// view model for the connector configuration user control
    /// </summary>
    public class ConfigureViewModel : Step
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootViewModel"></param>
        public ConfigureViewModel(RootViewModel rootViewModel)
        {
            RootViewModel = rootViewModel;
            Name = "Service Configuration";
            ID = "ServiceConfiguration";
        }

        private bool _configurationFailed = false;
        /// <summary>
        /// 
        /// </summary>
        public bool ConfigurationFailed
        {
            get { return _configurationFailed; }
            set
            {
                if (value != _configurationFailed)
                {
                    _configurationFailed = value;
                    RaisePropertyChanged(() => ConfigurationFailed);
                }
            }
        }

        private bool _cloudConnectionFailed = false;
        /// <summary>
        /// general cloud connection failure
        /// </summary>
        public bool CloudConnectionFailed
        {
            get { return _cloudConnectionFailed; }
            set
            {
                if (value != _cloudConnectionFailed)
                {
                    _cloudConnectionFailed = value;
                    RaisePropertyChanged(() => CloudConnectionFailed);
                }
            }
        }

        private bool _tenantConnectionExists = false;
        /// <summary>
        /// 
        /// </summary>
        public bool TenantConnectionExists
        {
            get { return _tenantConnectionExists; }
            set
            {
                if (value != _tenantConnectionExists)
                {
                    _tenantConnectionExists = value;
                    RaisePropertyChanged(() => TenantConnectionExists);
                }
            }
        }

        private DispatcherTimer _timer = new DispatcherTimer();

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
            RootViewModel.IsOKEnabled = false;
            RootViewModel.IsOKVisible = false;

            _timer.Interval = TimeSpan.FromMilliseconds(100);
            _timer.Tick += _timer_Tick;

            ConfigureButtonClick();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ConfigureButtonClick()
        {
            _timer.IsEnabled = true;

            ConfigurationFailed = false;
            CloudConnectionFailed = false;
            TenantConnectionExists = false;
            RootViewModel.IsConfigureEnabled = false;
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            ConfigureConnector();
        }

        private void ConfigureConnector()
        {
            RootViewModel.IsBusy = true;

            BackgroundWorker bkgndWorker = new BackgroundWorker();
            bkgndWorker.WorkerSupportsCancellation = true;
            bkgndWorker.WorkerReportsProgress = true;
            bkgndWorker.DoWork += new DoWorkEventHandler(bkgndWorker_DoWork);
            //bkgndWorker.ProgressChanged += new ProgressChangedEventHandler(bkgndWorker_ProgressChanged); // progress bar is now indeterminate - so progress reporting no longer used
            bkgndWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bkgndWorker_RunWorkerCompleted);

            _timer.IsEnabled = false;
            RootViewModel.IsBusy = false;

            if (bkgndWorker.IsBusy != true)
                bkgndWorker.RunWorkerAsync(this);
        }

        void bkgndWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if ((e.Cancelled == true) || ((bool)e.Result == false))
            {
                RootViewModel.IsConfigureEnabled = false;
            }
            else if (!(e.Error == null))
            {
                RootViewModel.IsConfigureEnabled = false;
                ConfigurationFailed = true;
            }
            else
            {
                ConfigurationFailed = false;
                RootViewModel.MoveNext();
            }
        }

        // progress bar is now indeterminate - so progress reporting no longer usedTODO fix this with binding
        //void bkgndWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        //{
            //Feedback.Value = e.ProgressPercentage;

            //if (e.ProgressPercentage == 10)
            //    _FeedbackTextBlock.Text = "Initializing connector...";
            //else if (e.ProgressPercentage == 50)
            //    _FeedbackTextBlock.Text = "Verifying tenant connection...";
            //else
            //    _FeedbackTextBlock.Text = "Configuring connector...";
        //}

        void bkgndWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;
            ConfigureViewModel context = e.Argument as ConfigureViewModel;

            bool success = false;

            if (InitializeConnector(context))
            {
                success = ValidateConnector(context);
            }

            if (!success)
                e.Cancel = true;
            e.Result = success;
        }

        private bool InitializeConnector(ConfigureViewModel context)
        {
            var success = ProductLibraryHelpers.InstallService(context.RootViewModel.Connection.EffectiveUser, context.RootViewModel.Connection.EffectivePassword);
            return success;
        }

        private bool ValidateConnector(ConfigureViewModel context)
        {
            var success = false;
            try
            {
                bool tenantConnectionExists = ProductLibraryHelpers.TenantConnectionExists(context.RootViewModel.Connection.ConnectionKey);
                context.TenantConnectionExists = true;
                success = !tenantConnectionExists;
            }
            catch (Exception ex)
            {
                string msg = ex.Message;

                context.CloudConnectionFailed = true;
                return false;
            }

            return success;
        }
    }
}
