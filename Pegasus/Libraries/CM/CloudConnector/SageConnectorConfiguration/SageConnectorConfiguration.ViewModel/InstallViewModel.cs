using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Threading;

namespace SageConnectorConfiguration.ViewModel
{
    /// <summary>
    /// view model for the install user control
    /// </summary>
    public class InstallViewModel : Step
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootViewModel"></param>
        public InstallViewModel(RootViewModel rootViewModel)
        {
            RootViewModel = rootViewModel;
            Name = "Install";
            ID = "Install";
        }

        private bool _installFailed = false;
        /// <summary>
        /// Whether any of the install steps has failed
        /// </summary>
        public bool InstallFailed
        {
            get { return _installFailed; }
            set
            {
                if (value != _installFailed)
                {
                    _installFailed = value;
                    RaisePropertyChanged(() => InstallFailed);
                }
            }
        }

        private string _installMachineText;
        /// <summary>
        /// 
        /// </summary>
        public String InstallMachineText
        {
            get { return _installMachineText; }

            set
            {
                if (value != _installMachineText)
                {
                    _installMachineText = value;
                    RaisePropertyChanged(() => InstallMachineText);
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

            InstallMachineText = "The Sage Connector will be installed on this machine: " + RootViewModel.Connection.InstallMachine;
            InstallFailed = false;

            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += _timer_Tick;

            InstallButtonClick();
        }

        /// <summary>
        /// install the connector
        /// </summary>
        public void InstallButtonClick()
        {
            _timer.IsEnabled = true;

            RootViewModel.IsInstallEnabled = false;
            InstallFailed = false;
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            ProcessInstallConnector();
        }

        private void ProcessInstallConnector()
        {
            RootViewModel.IsBusy = true;

            BackgroundWorker bkgndWorker = new BackgroundWorker();
            bkgndWorker.WorkerSupportsCancellation = true;
            bkgndWorker.WorkerReportsProgress = true;
            bkgndWorker.DoWork += new DoWorkEventHandler(bkgndWorker_DoWork);
            //bkgndWorker.ProgressChanged += new ProgressChangedEventHandler(bkgndWorker_ProgressChanged); // changed this to indeterminate progress -- so, no longer used
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
                RootViewModel.IsInstallEnabled = true;
                InstallFailed = true;
            }
            else if (!(e.Error == null))
            {
                RootViewModel.IsInstallEnabled = true;
                InstallFailed = true;
            }
            else
            {
                InstallFailed = false;
                RootViewModel.MoveNext();
            }
        }

        // changed this to indeterminate progress -- so, no longer used
        //void bkgndWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        //{
            //Feedback.Value = e.ProgressPercentage;

            //if (e.ProgressPercentage == 10)
            //    _FeedbackTextBlock.Text = "Installing connector...";
        //}

        private String ConnectorInstallPath
        {
            get
            {
                var executingAssemblyPath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                if (String.IsNullOrEmpty(executingAssemblyPath))
                {
                    executingAssemblyPath = AppDomain.CurrentDomain.BaseDirectory;
                    if (String.IsNullOrEmpty(executingAssemblyPath))
                        executingAssemblyPath = System.IO.Directory.GetCurrentDirectory();
                }
                return System.IO.Path.Combine(executingAssemblyPath, "SageConnectorSetup.exe");
            }
        }

        void bkgndWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker bw = sender as BackgroundWorker;
            InstallViewModel context = e.Argument as InstallViewModel;

            bool success = false;

            ProcessStartInfo psi = new ProcessStartInfo(ConnectorInstallPath);
            Process myProcess = Process.Start(psi);
            myProcess.WaitForExit();
            if (myProcess.HasExited) // TODO - we need an exit code different than 0 for install cancellation... Can't continue to configuration in this case.
            {
                success = (myProcess.ExitCode == 0);

                if (myProcess.ExitCode == 1602)
                    e.Cancel = true;
            }
            else
            {
                success = false;
            }

            e.Result = success;
        }
    }
}
