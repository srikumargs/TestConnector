
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using Sage.Connector.Common;
using Sage.Connector.Management;
using Sage.Connector.StateService.Interfaces.DataContracts;
using SageConnect.Control;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SageConnect.Internal
{
    /// <summary>
    /// Worker Class with Progressbar
    /// </summary>
    public class ProgressBarWorker
    {
        private Window _progresswindow;
        private bool _checkforbackofficestatus=false;
        //private Thread progressBarThread;
        private void ShowProgressBar(bool textvisible,string displaytext, bool bigscreen = false, bool owner =false )
        {

            CircularProgressBar circularProgressBar = new CircularProgressBar
            {
                Background = Brushes.Transparent,
                Height = 30,
                Width = 30,
                Margin = new Thickness(0)
            };
            _progresswindow = new Window
            {
                Width = (bigscreen? 750:220),
                Height = (bigscreen? 650:60),
                Opacity = .75,
                
                WindowStyle = WindowStyle.None,
                AllowsTransparency = true,
                Background = Brushes.WhiteSmoke,
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(1),

            };
            if (owner)
            {
                _progresswindow.Owner = Application.Current.MainWindow;
                _progresswindow.Left = Application.Current.MainWindow.Left;
                _progresswindow.Top = Application.Current.MainWindow.Top;
            }
            else
                _progresswindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            Grid grid = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            grid.Margin = new Thickness(0);

            ColumnDefinition gridCol1 = new ColumnDefinition();
            gridCol1.Width = new GridLength(40);
            grid.ColumnDefinitions.Add(gridCol1);


            circularProgressBar.HorizontalAlignment = HorizontalAlignment.Center;
            circularProgressBar.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumn(circularProgressBar, 0);
            Grid.SetRow(circularProgressBar, 0);
            grid.Children.Add(circularProgressBar);
            if (textvisible)
            {
                ColumnDefinition gridCol2 = new ColumnDefinition();
                grid.ColumnDefinitions.Add(gridCol2);

                RowDefinition gridRow = new RowDefinition();
                //RowDefinition gridRow1 = new RowDefinition();

                TextBlock t = new TextBlock
                {
                    Text = displaytext,
                    FontWeight = FontWeights.Bold,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                grid.RowDefinitions.Add(gridRow);
                Grid.SetColumn(t, 1);
                Grid.SetRow(t, 1);
                grid.Children.Add(t);
            }
            //grid.RowDefinitions.Add(gridRow1);



            _progresswindow.Content = grid;
            _progresswindow.Cursor = Cursors.Wait;
            _progresswindow.ShowDialog();
        }

        private BackgroundWorker _worker;
        /// <summary>
        /// To Start the Connector Services worker
        /// </summary>
        private void StartConnecterServicesWorker()
        {
            _worker = new BackgroundWorker { WorkerReportsProgress = true }; // variable declared in the class
            _worker.DoWork += StartservicesWorkerDoWork;
            _worker.ProgressChanged += StartservicesWorkerProgressChanged;
            _worker.RunWorkerCompleted += StartservicesWorkerRunWorkerCompleted;
        }
        /// <summary>
        /// To Start the Connector Services
        /// </summary>
        public bool StartConnecterServices()
        {
            StartConnecterServicesWorker();
            _worker.RunWorkerAsync();
            ShowProgressBar(true, "Initializing Connector Service");
            return _checkforbackofficestatus;
        }
  
        private void StartservicesWorkerRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //_checkforbackofficestatus=CheckforBackOffice();

        }

        private void StartservicesWorkerDoWork(object sender, DoWorkEventArgs e)
        {

            _worker.ReportProgress(StartServices() ? 100 : 00);

        }

        private void StartservicesWorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage == 100)
            {
                _progresswindow.Close();
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        private bool StartServices()
        {
            try
            {
                return ConfigurationHelpers.InstallAndStartServices();
            }
            catch (Exception ex)
            {
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(null, ex.ExceptionAsString());
                }
                return false;
            }
        }

        /// <summary>
        /// To Start the Progress Bar for Loading Tenant Lsit
        /// </summary>
        public void StartProgressbar()
        {
            ShowProgressBar(false,"");
        }
  

        private BackgroundWorker _backgroundWorker;
        private ManagementCredentialsNeededResponse _managementCredentialsNeededResponse;
        private BackOfficePluginsResponse _backOfficePluginsResponse;
        private ValidateBackOfficeAdminCredentialsResponse _validateBackOfficeAdminCredentialsResponse;
        private ConnectionCredentialsNeededResponse _connectionCredentialsNeededResponse;
        private ValidateBackOfficeConnectionResponse _validateBackOfficeConnectionResponse;
        private FeatureResponse _featureResponse;
        //private readonly BackOfficeServiceManager _backOfficeServiceManager;

        private void InitializeBackgroundWorker(DoWorkEventHandler doWorkEventHandler, RunWorkerCompletedEventHandler runWorkerCompletedEventHandler, Object argument)
        {
            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.WorkerSupportsCancellation = true;
            _backgroundWorker.WorkerReportsProgress=true ;
            _backgroundWorker.DoWork +=  doWorkEventHandler;
            _backgroundWorker.RunWorkerCompleted += runWorkerCompletedEventHandler;
            _backgroundWorker.RunWorkerAsync(argument);
        }

        #region GetManagementCredentialsNeeded
        /// <summary>
        /// Get the Management Credentials based on the back office id
        /// </summary>
        ///
        public ManagementCredentialsNeededResponse GetManagementCredentialsNeeded(string backofficeid,String progressstate)
        {


            InitializeBackgroundWorker(
            BackGroundWorker_DoWork_GetManagementCredentialsNeeded,
            BackgroundWorker_WorkCompleted_GetManagementCredentialsNeeded,
            new Tuple<string>(backofficeid));
            ShowProgressBar(true, progressstate, true, true);
            return _managementCredentialsNeededResponse;

        }
        private void BackGroundWorker_DoWork_GetManagementCredentialsNeeded(object sender, DoWorkEventArgs e)
        {
            try
            {
                Thread.SpinWait(2000);
                Tuple<string> argument = (Tuple<string>)e.Argument;
                BackOfficeServiceManager backOfficeServiceManager = new BackOfficeServiceManager();
                var response = backOfficeServiceManager.GetManagementCredentialsNeeded(argument.Item1);
                e.Result = response;
            }
            catch (Exception ex)
            {
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(null, ex.ExceptionAsString());
                }
            }
        }
        private void BackgroundWorker_WorkCompleted_GetManagementCredentialsNeeded(Object sender, RunWorkerCompletedEventArgs e)
        {

            _managementCredentialsNeededResponse = (ManagementCredentialsNeededResponse)e.Result;
            _progresswindow.Close();
        } 
        #endregion

        #region GetBackOfficePlugins
        /// <summary>
        /// Get the Back office Plugins details 
        /// </summary>
        ///
        public BackOfficePluginsResponse GetBackOfficePlugins(String progressstate)
        {


            InitializeBackgroundWorker(
            BackGroundWorker_DoWork_GetBackOfficePlugins,
            BackgroundWorker_WorkCompleted_GetBackOfficePlugins,
            null);
            ShowProgressBar(true, progressstate, true, true);
            return _backOfficePluginsResponse;

        }
        private void BackGroundWorker_DoWork_GetBackOfficePlugins(object sender, DoWorkEventArgs e)
        {
            try
            {
                Thread.SpinWait(2000);
                BackOfficeServiceManager backOfficeServiceManager = new BackOfficeServiceManager();
                var backOfficePluginsResponse = backOfficeServiceManager.GetBackOfficePlugins();
                e.Result = backOfficePluginsResponse;
            }
            catch (Exception ex)
            {
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(null, ex.ExceptionAsString());
                }
            }
        }
        private void BackgroundWorker_WorkCompleted_GetBackOfficePlugins(Object sender, RunWorkerCompletedEventArgs e)
        {

            _backOfficePluginsResponse = (BackOfficePluginsResponse)e.Result;
            _progresswindow.Close();
        } 
        #endregion

        #region BackOfficePlugin
        /// <summary>
        /// Download BackOffice Plugin
        /// </summary>
        ///
        public void DownloadBackOfficePlugin(BackOfficePlugin backOfficePlugin,String progressstate)
        {


            InitializeBackgroundWorker(
            BackGroundWorker_DoWork_DownloadBackOfficePlugin,
            BackgroundWorker_WorkCompleted_DownloadBackOfficePlugin,
            new Tuple<BackOfficePlugin>(backOfficePlugin));
            ShowProgressBar(true, progressstate, true, true);


        }
        private void BackGroundWorker_DoWork_DownloadBackOfficePlugin(object sender, DoWorkEventArgs e)
        {
            try
            {
                Thread.SpinWait(2000);
                Tuple<BackOfficePlugin> argument = (Tuple<BackOfficePlugin>)e.Argument;
                ConfigurationHelpers.DownloadBackOfficePlugin(argument.Item1.PluginId,
                    argument.Item1.BackOfficePluginAutoUpdateProductId,
                    argument.Item1.BackOfficePluginAutoUpdateProductVersion,
                    argument.Item1.BackOfficePluginAutoUpdateComponentBaseName);
            }
            catch (Exception ex)
            {
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(null, ex.ExceptionAsString());
                }
            }
        }
        private void BackgroundWorker_WorkCompleted_DownloadBackOfficePlugin(Object sender, RunWorkerCompletedEventArgs e)
        {
            _progresswindow.Close();
        }
        
        #endregion
        
        #region ValidateBackOfficeAdminCredentials
        /// <summary>
        /// Validate the Admin credentials provided with back office 
        /// </summary>
        ///
        public ValidateBackOfficeAdminCredentialsResponse ValidateBackOfficeAdminCredentials(string backofficeid, IDictionary<string, string> credentials,String progressstate)
        {


            InitializeBackgroundWorker(
            BackGroundWorker_DoWork_ValidateBackOfficeAdminCredentials,
            BackgroundWorker_WorkCompleted_ValidateBackOfficeAdminCredentials,
            new Tuple<string, IDictionary<string, string>>(backofficeid, credentials));
            ShowProgressBar(true, progressstate, true, true);
            return _validateBackOfficeAdminCredentialsResponse;

        }
        private void BackGroundWorker_DoWork_ValidateBackOfficeAdminCredentials(object sender, DoWorkEventArgs e)
        {
            try
            {
                Thread.SpinWait(2000);
                Tuple<string, IDictionary<string, string>> argument =
                    (Tuple<string, IDictionary<string, string>>)e.Argument;
                BackOfficeServiceManager backOfficeServiceManager = new BackOfficeServiceManager();
                var response =
                    backOfficeServiceManager.ValidateBackOfficeAdminCredentials(argument.Item1,
                        argument.Item2);
                e.Result = response;
            }
            catch (Exception ex)
            {
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(null, ex.ExceptionAsString());
                }
            }

        }
        private void BackgroundWorker_WorkCompleted_ValidateBackOfficeAdminCredentials(Object sender, RunWorkerCompletedEventArgs e)
        {
            _validateBackOfficeAdminCredentialsResponse = (ValidateBackOfficeAdminCredentialsResponse)e.Result;
            _progresswindow.Close();
        } 
        #endregion

        #region GetConnectionCredentialsNeeded
        /// <summary>
        /// Get ConnectionCredentials Detils from the Back office 
        /// </summary>
        ///
        public ConnectionCredentialsNeededResponse GetConnectionCredentialsNeeded(string backofficeid, IDictionary<string, string> companyManagementCredentials, IDictionary<string, string> companyConnectionCredentials, String progressstate)
        {


            InitializeBackgroundWorker(
            BackGroundWorker_DoWork_GetConnectionCredentialsNeeded,
            BackgroundWorker_WorkCompleted_GetConnectionCredentialsNeeded,
            new Tuple<string, IDictionary<string, string>, IDictionary<string, string>>(backofficeid, companyManagementCredentials, companyConnectionCredentials));
            ShowProgressBar(true, progressstate, true, true);
            return _connectionCredentialsNeededResponse;

        }
        private void BackGroundWorker_DoWork_GetConnectionCredentialsNeeded(object sender, DoWorkEventArgs e)
        {
            try
            {
                Thread.SpinWait(2000);
                Tuple<string, IDictionary<string, string>, IDictionary<string, string>> argument
                    = (Tuple<string, IDictionary<string, string>, IDictionary<string, string>>)e.Argument;
                var response =
                       new BackOfficeServiceManager().GetConnectionCredentialsNeeded(argument.Item1,
                           argument.Item2, argument.Item3);
                e.Result = response;
            }
            catch (Exception ex)
            {
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(null, ex.ExceptionAsString());
                }
            }

        }
        private void BackgroundWorker_WorkCompleted_GetConnectionCredentialsNeeded(Object sender, RunWorkerCompletedEventArgs e)
        {
            _connectionCredentialsNeededResponse = (ConnectionCredentialsNeededResponse)e.Result;
            _progresswindow.Close();
        } 
        #endregion

        #region ValidateBackOfficeConnectionCredentialsAsString
        /// <summary>
        /// Validate BackOffice Connection Credentials Supplied
        /// </summary>
        ///
        public ValidateBackOfficeConnectionResponse ValidateBackOfficeConnectionCredentialsAsString(string backofficeid,
            string companyConnectionCredentials, String progressstate)
        {


            InitializeBackgroundWorker(
            BackGroundWorker_DoWork_ValidateBackOfficeConnectionCredentialsAsString,
            BackgroundWorker_WorkCompleted_ValidateBackOfficeConnectionCredentialsAsString,
            new Tuple<string, string>(backofficeid, companyConnectionCredentials));
            ShowProgressBar(true, progressstate, true, true);
            return _validateBackOfficeConnectionResponse;

        }
        private void BackGroundWorker_DoWork_ValidateBackOfficeConnectionCredentialsAsString(object sender, DoWorkEventArgs e)
        {
            try
            {
                Thread.SpinWait(2000);
                Tuple<string, string> argument = (Tuple<string, string>)e.Argument;
                var response =
                       new BackOfficeServiceManager().ValidateBackOfficeConnectionCredentialsAsString(argument.Item1,
                           argument.Item2);
                e.Result = response;
            }
            catch (Exception ex)
            {
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(null, ex.ExceptionAsString());
                }
            }

        }
        private void BackgroundWorker_WorkCompleted_ValidateBackOfficeConnectionCredentialsAsString(Object sender, RunWorkerCompletedEventArgs e)
        {
            _validateBackOfficeConnectionResponse = (ValidateBackOfficeConnectionResponse)e.Result;
            _progresswindow.Close();
        } 
        #endregion

        #region GetFeatureResponse
        /// <summary>
        /// Get Feature Configuration Response
        /// </summary>
        ///
        public FeatureResponse GetFeatureResponse(string backofficeid, string backOfficeCredentials, string featureId, string tenantId, string payload, string progressstate)
        {


            InitializeBackgroundWorker(
            BackGroundWorker_DoWork_GetFeatureResponse,
            BackgroundWorker_WorkCompleted_GetFeatureResponse,
            new Tuple<string, string, string, string, string>(backofficeid, backOfficeCredentials, featureId, tenantId, payload));
            ShowProgressBar(true, progressstate, true, true);
            return _featureResponse;

        }
        private void BackGroundWorker_DoWork_GetFeatureResponse(object sender, DoWorkEventArgs e)
        {
            try
            {
                Thread.SpinWait(2000);
                Tuple<string, string, string, string, string> argument = (Tuple<string, string, string, string, string>)e.Argument;
                var response =
                       new FeatureServiceManager().GetFeatureResponse(argument.Item1,
                           argument.Item2, argument.Item3, argument.Item4, argument.Item5);
                e.Result = response;
            }
            catch (Exception ex)
            {
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(null, ex.ExceptionAsString());
                }
            }

        }
        private void BackgroundWorker_WorkCompleted_GetFeatureResponse(Object sender, RunWorkerCompletedEventArgs e)
        {
            _featureResponse = (FeatureResponse)e.Result;
            _progresswindow.Close();
        } 
        #endregion
    }
}
