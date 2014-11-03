using System;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using ConnectorServiceMonitor.Internal;
using ConnectorServiceMonitor.ViewModel;
using IWshRuntimeLibrary;
using Sage.Connector.Common;
using Sage.Connector.Common.DataContracts;
using Sage.Connector.MonitorService.Interfaces;
using Sage.Connector.MonitorService.Interfaces.DataContracts;
using Sage.Connector.MonitorService.Proxy;
using Sage.Connector.StateService.Interfaces.DataContracts;
using ContentAlignment = System.Drawing.ContentAlignment;
using HostingFxInterfaces = Sage.CRE.HostingFramework.Interfaces;
using HostingFxProxy = Sage.CRE.HostingFramework.Proxy;

namespace ConnectorServiceMonitor
{
    internal partial class MainForm : Form
    {
        public MainForm()
        {
            var serverRegistry = GetServerRegistry();
            if (serverRegistry.IsConfigured && !Program.ForceGui)
            {
                ShowInTaskbar = false;
                WindowState = FormWindowState.Minimized;
            }

            InitializeComponent();

            _imageManager = new ImageManager();
            _startPageHtmlRenderer = new StartPageHtmlRenderer(_imageManager, this);
            _generalPageHtmlRenderer = new GeneralPageHtmlRenderer(_imageManager, this);
            _generalHtmlContentUserControl.ChangeSettings += new EventHandler(_htmlContentUserControl_ChangeSettings);
            _connectionsHtmlContentUserControl.ChangeSettings += new EventHandler(_htmlContentUserControl_ChangeSettings);
            _requestsHtmlContentUserControl.ChangeSettings += new EventHandler(_htmlContentUserControl_ChangeSettings);
            _settingsUserControl.ChangeSettings += new EventHandler(_htmlContentUserControl_ChangeSettingsApply);

            _generalHtmlContentUserControl.ApplyUpdate += new EventHandler(_htmlContentUserControl_ApplyUpdate);
            _connectionsHtmlContentUserControl.ApplyUpdate += new EventHandler(_htmlContentUserControl_ApplyUpdate);
            _requestsHtmlContentUserControl.ApplyUpdate += new EventHandler(_htmlContentUserControl_ApplyUpdate);

            _connectionsPageHtmlRenderer = new ConnectionsPageHtmlRenderer(_imageManager, this);
            _requestsPageHtmlRenderer = new RequestsPageHtmlRenderer(_imageManager, this);

            _generalHtmlContentUserControl.SetDocumentText(_startPageHtmlRenderer.Render());
            _connectionsHtmlContentUserControl.SetDocumentText(_startPageHtmlRenderer.Render());
            _requestsHtmlContentUserControl.SetDocumentText(_startPageHtmlRenderer.Render());

            this.Text = String.Format(this.Text, Common.MonitorBriefProductName);
            //this._refreshToolStripSplitButton.Image = Sage.CRE.Resources.ServiceMonitor.Refresh;
            _settingsUserControl.ServerRegistry = serverRegistry;
            _settingsUserControl.RefreshIntervalChanged += RefreshIntervalchanged;
            _settingsUserControl.RestoreAutoRefreshInterval(serverRegistry.AutoRefreshInterval);
            if (!serverRegistry.IsConfigured)
            {
                bool quitstartthread = false;
                UpdateIcon(NotifyIconCondition.NotConfigured, NotifyIcon.ServiceNotConfig, Common.ReplaceKnownTerms(Strings.NotifyIcon_NotConfiguredTooltip));
                ShowBalloonTip(Common.ReplaceKnownTerms(Strings.SysTrayBalloon_Title), Common.ReplaceKnownTerms(Strings.SysTrayBalloon_NotConfiguredMessage), ToolTipIcon.Info);
                try
                {
                    if (IsMonitorServiceRegistered())
                        Internal.ConfigureForLocalMachine.Configure();
                }
                catch (Exception ex)
                {
                    quitstartthread = true;
                    using (var logger = new SimpleTraceLogger())
                    {
                        logger.WriteError(null, ex.ExceptionAsString());
                    }
                }

                    var serverRegistration = GetServerRegistration();
                    _runOnLoginToolStripMenuItem.Checked = serverRegistration.RunOnLogin;
                    comboBox1.SelectedIndex = serverRegistration.RequestsShowing;
                    UncheckAllRefreshItems();
                    RestoreAutoRefreshInterval(serverRegistration);
                    _settingsUserControl.RestoreAutoRefreshInterval(serverRegistration.AutoRefreshInterval);
                    if ( !quitstartthread && IsMonitorServiceRegistered())
                    {
                        _machineName = serverRegistration.Host;
                        _portNumber = serverRegistration.CatalogServicePort;
                    }

                    StartWorkerThread();
              
            }
            else
            {
                UpdateIcon(NotifyIconCondition.Offline, NotifyIcon.ServiceOffline, Common.ReplaceKnownTerms(Strings.NotifyIcon_OfflineTooltip));

                var serverRegistration = GetServerRegistration();
                _runOnLoginToolStripMenuItem.Checked = serverRegistration.RunOnLogin;
                comboBox1.SelectedIndex = serverRegistration.RequestsShowing;
                UncheckAllRefreshItems();
                RestoreAutoRefreshInterval(serverRegistration);
                _settingsUserControl.RestoreAutoRefreshInterval(serverRegistration.AutoRefreshInterval);
                _machineName = serverRegistration.Host;
                _portNumber = serverRegistration.CatalogServicePort;

                StartWorkerThread();
            }
        }
        /// <summary>
        /// Determines whether the monitor service is registered.
        /// </summary>
        /// <returns></returns>
        private  bool IsMonitorServiceRegistered()
        {
            bool retval;
            using (var logger = new SimpleTraceLogger())
            {
                retval = ConnectorMonitorServiceUtils.IsServiceRegistered(logger);
            }
            return retval;
        }
        private void _htmlContentUserControl_ApplyUpdate(object sender, EventArgs e)
        {
            //need to fire off the update file pointed to by the file update.
            if (_cachedConnectorState != null)
            {
                var updateStatus = _cachedConnectorState.ConnectorUpdateStatus;
                if (updateStatus == ConnectorUpdateStatus.UpdateAvailable || updateStatus == ConnectorUpdateStatus.UpdateRequired)
                {
                    var updateInfo = _cachedConnectorState.ConnectorUpdateInfo;
                    if (updateInfo != null && updateInfo.UpdateLinkUri != null)
                    {
                        Uri updateUri = updateInfo.UpdateLinkUri;
                        if (updateUri.IsFile && System.IO.File.Exists(updateUri.LocalPath))
                        {
                            LaunchUpdate(updateUri.LocalPath);
                        }
                    }
                }
            }
        }

        
        private void LaunchUpdate(string updatePath)
        {
            //do not need to use the process info version of this
            try
            {
                Process.Start(updatePath);
                //do not want to wait or keep process as we are about to be shut down.                
            }
            catch (Exception ex)
            {
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(null, ex.ExceptionAsString());
                }
            }
        }

        void _htmlContentUserControl_ChangeSettings(object sender, EventArgs e)
        {
            _tabControl.SelectTab(3);
            //_settingsToolStripButton_Click(sender, e);
        }
        void _htmlContentUserControl_ChangeSettingsApply(object sender, EventArgs e)
        {
            _settingsToolStripButton_Click(sender, e);
        }
        private void StartWorkerThread()
        {
            _terminateWorkerThreadEvent = new ManualResetEvent(false);
            _pulseWorkerThreadEvent = new AutoResetEvent(false);
            _thread = new Thread(new ThreadStart(WorkerThreadFunction));
            _thread.IsBackground = true;
            _thread.Start();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            var serverRegistry = GetServerRegistry();
            if (serverRegistry.IsConfigured && !Program.ForceGui)
            {
                Visible = false;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _notifyIcon.Visible = false;
            _exiting = true;
            {
                TerminateWorkerThread();
                Close();
                Application.Exit();
            }
        }

        private void TerminateWorkerThread()
        {
            if (_terminateWorkerThreadEvent != null)
            {
                _terminateWorkerThreadEvent.Set();
            }

            if (_pulseWorkerThreadEvent != null)
            {
                _pulseWorkerThreadEvent.Set();
            }

            if (_thread != null)
            {
                _thread.Join();
            }
        }

        private void AbortWorkerThread()
        {
            if (_thread != null)
            {
                _thread.Abort();
            }
        }

        private void ShowMonitorWindow(object sender, EventArgs e)
        {
            Visible = true;
            PostMessage(this.Handle, WM_CONNECTOR_SHOW_NORMAL, IntPtr.Zero, IntPtr.Zero);
        }

        private void UpdateIcon(NotifyIconCondition newNotifyIconCondition, Icon icon, String tooltipText)
        {
            Debug.Assert(tooltipText.Length < 64, "The NotifyIcon's tooltip text must be less than 64 characters");

            _notifyIcon.Icon = icon;
            _notifyIcon.Text = tooltipText.Substring(0, Math.Min(63, tooltipText.Length)); // NotifyIcon's tooltip text must be less than 64 characters, truncate to ensure
            _notifyIconCondition = newNotifyIconCondition;
            this.Icon = icon;
        }

        private void ShowBalloonTip(String title, String text, ToolTipIcon icon)
        { _notifyIcon.ShowBalloonTip(10000, title, text, icon); }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Trace.WriteLine(String.Format("ConnectorServiceMonitor processing MainForm_FormClosing: {0} {1}", e.CloseReason.ToString(), _exiting));

            if (e.CloseReason == CloseReason.UserClosing && !_exiting)
            {
                e.Cancel = true;
                Visible = false;
            }
        }

        private void _monitorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Visible = true;
            PostMessage(this.Handle, WM_CONNECTOR_SHOW_NORMAL, IntPtr.Zero, IntPtr.Zero);
        }

        private void _settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_notifyIconCondition == NotifyIconCondition.NotConfigured && IsMonitorServiceRegistered())
                    Internal.ConfigureForLocalMachine.Configure();
            var serverRegistration = GetServerRegistration();
            //if (serverRegistration.ChangeServer(this.Visible ? this.Handle : IntPtr.Zero))
            //{
                AbortWorkerThread();
               
                _runOnLoginToolStripMenuItem.Checked = serverRegistration.RunOnLogin;
                comboBox1.SelectedIndex = serverRegistration.RequestsShowing;
                UncheckAllRefreshItems();
                RestoreAutoRefreshInterval(serverRegistration);
                _settingsUserControl.RestoreAutoRefreshInterval(serverRegistration.AutoRefreshInterval);
                _machineName = serverRegistration.Host;
                _portNumber = serverRegistration.CatalogServicePort;

                StartWorkerThread();
            //}
        }

        private void WorkerThreadFunction()
        {
            try
            {
                UpdateStatusStripText(Strings.StatusStrip_RefreshingMessage);

                do
                {
                    RefreshMonitorData();
                } while (0 != WaitHandle.WaitAny(new WaitHandle[] { _terminateWorkerThreadEvent, _pulseWorkerThreadEvent }, AutoRefreshInterval));
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ExceptionAsString());
            }
        }

        private Int32 AutoRefreshInterval
        {
            get
            {
                Int32 result = 0;

                lock (_syncRoot)
                {
                    result = _autoRefreshInterval;
                }

                return result;
            }

            set
            {
                lock (_syncRoot)
                {
                    _autoRefreshInterval = value;
                }
            }
        }

        private DateTime LastRefreshDateTime
        {
            get
            {
                DateTime result = DateTime.MinValue;

                lock (_syncRoot)
                {
                    result = _lastRefreshDateTime;
                }

                return result;
            }

            set
            {
                lock (_syncRoot)
                {
                    _lastRefreshDateTime = value;
                }
            }
        }

        /// <summary>
        /// Make sure this happens on the correct thread.
        /// </summary>
        private void RequestToolStripButtonClick()
        {

            if (this.InvokeRequired)
            {
                BeginInvoke((MethodInvoker) RequestToolStripButtonClick);
            }
            else
            {
                _settingsToolStripButton_Click(null, null);    
            }
            
        }

        private void RefreshMonitorData()
        {
            Boolean error = true;
            try
            {
                if (_machineName == "<unknown>" && IsMonitorServiceRegistered())
                {
                    RequestToolStripButtonClick();
                    return;
                }

                var now = DateTime.Now;
                LastRefreshDateTime = _lastRefreshDateTime = now;

                UpdateStatusStripImage(Sage.CRE.Resources.ServiceMonitor.Network.ToBitmap());

                //List<InstanceServiceInfo> instanceServiceInfos = null;
                //using (var proxy = HostingFxProxy.CatalogServiceProxyFactory.CreateFromCatalog(_machineName, _portNumber))
                //{
                //    instanceServiceInfos = new List<InstanceServiceInfo>(proxy.GetServiceInfo().Select((x) => new InstanceServiceInfo("Monitor", x)));
                //}

                HostingFxInterfaces.Status monitorServiceStatus;

                //note: this MonitorServiceProxyFactory is hosting fx, where as the next is locally defined.
                //Did not move this to a persistent connection as this causes up to a 1:30 delay in the app noticing that
                //the service is off line. We still get significant reduction in port use by having persistent connector monitor connection
                using (var proxy = HostingFxProxy.MonitorServiceProxyFactory.CreateFromCatalog(_machineName, _portNumber))
                {
                    monitorServiceStatus = proxy.GetStatus();
                }

                ConnectorServiceState connectorServiceState = null;
                RequestState[] requestState = null;
                //Using and releasing as in the pattern commented below causes a number of tcp port resources to enter the time_wait state
                //putting some pressure on kernel resources. By keeping connection open we actually reduce over all resource consumption.
                //using (var proxy = MonitorServiceProxyFactory.CreateFromCatalog(_machineName, _portNumber))
                //{
                //    connectorServiceState = proxy.GetConnectorServiceState();
                //    requestState = proxy.GetRecentAndInProgressRequestsState(_recentEntriesThreshold);
                //}

                var monitorProxy = GetConnectorMonitorServiceProxy();
                connectorServiceState = monitorProxy.GetConnectorServiceState();
                requestState = monitorProxy.GetRecentAndInProgressRequestsState(_recentEntriesThreshold);
                UpdateStatus(new ServiceStatus(_machineName, _portNumber, monitorServiceStatus, connectorServiceState, requestState));

                //if (connectorServiceState.ServiceInfos != null)
                //{
                //    instanceServiceInfos.AddRange(connectorServiceState.ServiceInfos.Select((x) => new InstanceServiceInfo("Connector", x)));
                //}

                error = false;
            }
            catch (Exception)
            {
                // would like to use None rather than null, but HostingFx.Interfaces didn't define one
                UpdateStatus(new ServiceStatus(_machineName, _portNumber, null, null, null));
                
                //release the proxies start over next time.
                ReleaseConnectorMonitorServiceProxy();
            }
            finally
            {
                if (error)
                {
                    UpdateStatusStripText(Strings.StatusStrip_ErrorMessage);
                    UpdateStatusStripImage(Sage.CRE.Resources.ServiceMonitor.Error);
                }
                else
                {
                    UpdateStatusStripText(String.Format(Strings.StatusStrip_LastRefreshMessageFormat, LastRefreshDateTime.ToString("G", CultureInfo.CurrentCulture)));
                    UpdateStatusStripImage(null);
                }
            }
        }


        private MonitorServiceProxy GetConnectorMonitorServiceProxy()
        {
            if (_connectorMonitorServiceProxy == null)
                _connectorMonitorServiceProxy = MonitorServiceProxyFactory.CreateFromCatalog(_machineName, _portNumber);

            return _connectorMonitorServiceProxy;
        }

        private void ReleaseConnectorMonitorServiceProxy()
        {
            if (_connectorMonitorServiceProxy != null)
            {
                try
                {
                    _connectorMonitorServiceProxy.Dispose();
                    _connectorMonitorServiceProxy = null;
                }
                catch
                {
                    // throw away any issues from disposing the proxy
                }
            }
        }

        private MonitorServiceProxy _connectorMonitorServiceProxy;

        private void UpdateStatus(ServiceStatus status)
        {
            if (this.InvokeRequired)
            {
                var asyncResult = BeginInvoke((MethodInvoker)delegate() { UpdateStatus(status); });
                //Adding end forces the system to wait for the result.
                //This also fixes a massive memory leak when we take longer to process the list then it takes to get more items.
                EndInvoke(asyncResult);  
            } 
            else
            {
                try
                {
                    ConnectorStateHelper statusHelper = null;
                    if (status != null && status.ConnectorServiceState != null)
                        statusHelper = new ConnectorStateHelper(status.ConnectorServiceState.ConnectorState, status.Requests);
                    UpdateNotifyIconForStatus(status, statusHelper);
                    UpdateGeneralPage(status, statusHelper);
                    UpdateConnectionsPage(status, statusHelper);
                    UpdateRequestsPage(status);

                    if (status != null && status.ConnectorServiceState != null)
                    {
                        _cachedConnectorState = status.ConnectorServiceState.ConnectorState;
                    }
                }
                catch (Exception ex) { Trace.WriteLine(ex.ExceptionAsString()); }
            }
        }
        
        private void UpdateGeneralPage(ServiceStatus status, ConnectorStateHelper statusHelper)
        { _generalHtmlContentUserControl.SetDocumentText(_generalPageHtmlRenderer.Render(status, statusHelper)); }

        private void UpdateConnectionsPage(ServiceStatus status, ConnectorStateHelper statusHelper)
        { _connectionsHtmlContentUserControl.SetDocumentText(_connectionsPageHtmlRenderer.Render(status, statusHelper)); }

        private void UpdateRequestsPage(ServiceStatus status)
        { _requestsHtmlContentUserControl.SetDocumentText(_requestsPageHtmlRenderer.Render(status)); }

        private enum NotifyIconCondition
        {
            None = 0,
            Offline,
            Normal,
            ConnectivityError,
            ServiceNotReady,
            ServiceNotRegistered,
            ServiceNotRunning,
            ServiceBusy,
            UpdateAvailable,
            UpdateRequired,
            NotConfigured,
            OneOrMoreCloudConnectionProblems,
            OneOrMorePremiseConnectionProblems,
            OneOrMoreCloudAndPremiseConnectionProblems
        }

        private void UpdateNotifyIconForStatus(ServiceStatus status, ConnectorStateHelper stateHelper)
        {
            if (this.InvokeRequired)
            {
                BeginInvoke((MethodInvoker)delegate() { UpdateNotifyIconForStatus(new Tuple<ServiceStatus, ConnectorStateHelper>(status, stateHelper)); });
            }
            else
            {
                try
                {
                    UpdateNotifyIconForStatus(new Tuple<ServiceStatus, ConnectorStateHelper>(status, stateHelper));
                }
                catch (Exception ex) { Trace.WriteLine(ex.ExceptionAsString()); }
            }
        }

        private void UpdateNotifyIconForStatus(Tuple<ServiceStatus, ConnectorStateHelper> status)
        {
            var serviceStatus = status.Item1;
            var stateHelper = status.Item2;
            bool isUpdatePending = false;

            Icon icon = NotifyIcon.ServiceOffline;
            NotifyIconCondition newNotifyIconCondition = NotifyIconCondition.Offline;
            String tooltipText = Common.ReplaceKnownTerms(Strings.NotifyIcon_OfflineTooltip);
            String ballontipText = String.Empty;
            ToolTipIcon ballonTipIcon = ToolTipIcon.Info;
            if (_notifyIconCondition == NotifyIconCondition.Normal)
            {
                ballontipText = Common.ReplaceKnownTerms(Strings.SysTrayBalloon_OfflineMessage);
                ballonTipIcon = ToolTipIcon.Error;
            }

            if (serviceStatus.MonitorServiceStatus.HasValue)
            {
                switch (serviceStatus.MonitorServiceStatus)
                {
                    case HostingFxInterfaces.Status.Ready:
                        switch (serviceStatus.ConnectorServiceStatus)
                        {
                            case ConnectorServiceConnectivityStatus.Connected:
                                icon = NotifyIcon.ServiceNormal;
                                newNotifyIconCondition = NotifyIconCondition.Normal;
                                tooltipText = Common.ReplaceKnownTerms(Strings.NotifyIcon_NormalTooltip);
                                if (_notifyIconCondition != NotifyIconCondition.Normal)
                                {
                                    ballontipText = Common.ReplaceKnownTerms(Strings.SysTrayBalloon_NormalMessage);
                                    ballonTipIcon = ToolTipIcon.Info;
                                }
                                break;
                            case ConnectorServiceConnectivityStatus.ConnectivityError:
                                icon = NotifyIcon.ServiceError;
                                newNotifyIconCondition = NotifyIconCondition.ConnectivityError;
                                tooltipText = Common.ReplaceKnownTerms(Strings.NotifyIcon_ConnectivityErrorTooltip);
                                if (_notifyIconCondition == NotifyIconCondition.Normal)
                                {
                                    ballontipText = Common.ReplaceKnownTerms(Strings.SysTrayBalloon_ConnectivityErrorMessage);
                                    ballonTipIcon = ToolTipIcon.Error;
                                }
                                break;
                            case ConnectorServiceConnectivityStatus.ServiceNotReady:
                                icon = NotifyIcon.ServiceBusy;
                                newNotifyIconCondition = NotifyIconCondition.ServiceNotReady;
                                tooltipText = Common.ReplaceKnownTerms(Strings.NotifyIcon_ServiceNotReadyTooltip);
                                if (_notifyIconCondition == NotifyIconCondition.Normal)
                                {
                                    ballontipText = Common.ReplaceKnownTerms(Strings.SysTrayBalloon_ServiceNotReadyMessage);
                                    ballonTipIcon = ToolTipIcon.Error;
                                }
                                break;
                            case ConnectorServiceConnectivityStatus.ServiceNotRegistered:
                                icon = NotifyIcon.ServiceNotConfig;
                                newNotifyIconCondition = NotifyIconCondition.ServiceNotRegistered;
                                tooltipText = Common.ReplaceKnownTerms(Strings.NotifyIcon_ServiceNotRegisteredTooltip);
                                if (_notifyIconCondition == NotifyIconCondition.Normal)
                                {
                                    ballontipText = Common.ReplaceKnownTerms(Strings.SysTrayBalloon_ServiceNotRegisteredMessage);
                                    ballonTipIcon = ToolTipIcon.Error;
                                }
                                break;
                            case ConnectorServiceConnectivityStatus.ServiceNotRunning:
                                icon = NotifyIcon.ServiceError;
                                newNotifyIconCondition = NotifyIconCondition.ServiceNotRunning;
                                tooltipText = Common.ReplaceKnownTerms(Strings.NotifyIcon_ServiceNotRunningTooltip);
                                if (_notifyIconCondition == NotifyIconCondition.Normal)
                                {
                                    ballontipText = Common.ReplaceKnownTerms(Strings.SysTrayBalloon_ServiceNotRunningMessage);
                                    ballonTipIcon = ToolTipIcon.Error;
                                }

                                break;
                        }
                        break;

                    case HostingFxInterfaces.Status.Recycling:
                    case HostingFxInterfaces.Status.SevicingRequest:
                        icon = NotifyIcon.ServiceBusy;
                        newNotifyIconCondition = NotifyIconCondition.ServiceBusy;
                        tooltipText = Common.ReplaceKnownTerms(Strings.NotifyIcon_ServiceBusyTooltip);
                        break;
                }
            }


            if (newNotifyIconCondition == NotifyIconCondition.Normal)
            {
                Boolean cloudConnectionProblems = stateHelper.AggregateTenantStatus == ConnectorStatusEnum.Broken || stateHelper.AggregateTenantStatus == ConnectorStatusEnum.BrokenAndProcessing;
                Boolean premiseConnectionProblems = stateHelper.AggregateBackofficeStatus == ConnectorStatusEnum.Broken || stateHelper.AggregateBackofficeStatus == ConnectorStatusEnum.BrokenAndProcessing;
                if (cloudConnectionProblems && !premiseConnectionProblems)
                {
                    icon = NotifyIcon.ServiceError;
                    newNotifyIconCondition = NotifyIconCondition.OneOrMoreCloudConnectionProblems;
                    tooltipText = Common.ReplaceKnownTerms(Strings.NotifyIcon_CloudConnectionProblemsTooltip);
                    if (_notifyIconCondition != NotifyIconCondition.OneOrMoreCloudConnectionProblems)
                    {
                        ballontipText = Common.ReplaceKnownTerms(Strings.SysTrayBalloon_CloudConnectionProblemsMessage);
                        ballonTipIcon = ToolTipIcon.Error;
                    }
                }
                if (premiseConnectionProblems && !cloudConnectionProblems)
                {
                    icon = NotifyIcon.ServiceError;
                    newNotifyIconCondition = NotifyIconCondition.OneOrMorePremiseConnectionProblems;
                    tooltipText = Common.ReplaceKnownTerms(Strings.NotifyIcon_PremiseConnectionProblemsTooltip);
                    if (_notifyIconCondition != NotifyIconCondition.OneOrMorePremiseConnectionProblems)
                    {
                        ballontipText = Common.ReplaceKnownTerms(Strings.SysTrayBalloon_PremiseConnectionProblemsMessage);
                        ballonTipIcon = ToolTipIcon.Error;
                    }
                }

                if (premiseConnectionProblems && cloudConnectionProblems)
                {
                    icon = NotifyIcon.ServiceError;
                    newNotifyIconCondition = NotifyIconCondition.OneOrMoreCloudAndPremiseConnectionProblems;
                    tooltipText = Common.ReplaceKnownTerms(Strings.NotifyIcon_CloudAndPremiseConnectionProblemsTooltip);
                    if (_notifyIconCondition != NotifyIconCondition.OneOrMoreCloudAndPremiseConnectionProblems)
                    {
                        ballontipText = Common.ReplaceKnownTerms(Strings.SysTrayBalloon_CloudAndPremiseConnectionProblemsMessage);
                        ballonTipIcon = ToolTipIcon.Error;
                    }
                }
            }

            if (newNotifyIconCondition == NotifyIconCondition.Normal)
            {
                if (serviceStatus.ConnectorServiceState.ConnectorState.ConnectorUpdateStatus != Sage.Connector.StateService.Interfaces.DataContracts.ConnectorUpdateStatus.None)
                {
                    switch (serviceStatus.ConnectorServiceState.ConnectorState.ConnectorUpdateStatus)
                    {
                        case Sage.Connector.StateService.Interfaces.DataContracts.ConnectorUpdateStatus.UpdateAvailable:
                            icon = NotifyIcon.ServiceWarning;
                            newNotifyIconCondition = NotifyIconCondition.UpdateAvailable;
                            tooltipText = Common.ReplaceKnownTerms(Strings.NotifyIcon_UpdateAvailableTooltip);
                            //if (_notifyIconCondition != NotifyIconCondition.UpdateAvailable)
                            //{
                                ballontipText = Common.ReplaceKnownTerms(Strings.SysTrayBalloon_UpdateAvailableMessage);
                                ballonTipIcon = ToolTipIcon.Info;
                            //}
                            isUpdatePending = true;
                            break;
                        case Sage.Connector.StateService.Interfaces.DataContracts.ConnectorUpdateStatus.UpdateRequired:
                            icon = NotifyIcon.ServiceError;
                            newNotifyIconCondition = NotifyIconCondition.UpdateRequired;
                            tooltipText = Common.ReplaceKnownTerms(Strings.NotifyIcon_UpdateRequiredTooltip);
                            //if (_notifyIconCondition != NotifyIconCondition.UpdateRequired)
                            //{
                                ballontipText = Common.ReplaceKnownTerms(Strings.SysTrayBalloon_UpdateRequiredMessage);
                                ballonTipIcon = ToolTipIcon.Error;
                            //}
                            isUpdatePending = true;
                            break;
                    }
                }
            }

            //check to see if we should force a notification to remind about an update.
            bool showBallon = (newNotifyIconCondition != _notifyIconCondition);
            if (!showBallon && isUpdatePending)
            {
                bool intervalHasPassed = HasUpdateDisplayIntervalPassed();
                if (intervalHasPassed)
                {
                    bool isUpdateNotifction = IsUpdateNotification(newNotifyIconCondition);
                    if (isUpdateNotifction)
                    {
                        showBallon = true;
                        SetLastPendingUpdateDisplayTime();
                    }
                }
            }

            if (showBallon)
            {
                if (!String.IsNullOrEmpty(ballontipText))
                {
                    ShowBalloonTip(Common.ReplaceKnownTerms(Strings.SysTrayBalloon_Title), ballontipText, ballonTipIcon);

                    //if its the first show of the notification setup the last update, which will start the system
                    bool isUpdateNotifction = IsUpdateNotification(newNotifyIconCondition);
                    if (isUpdateNotifction && _lastPendingUpdateDisplayTime == DateTime.MinValue)
                    {
                        SetLastPendingUpdateDisplayTime();
                    }
                }
            }

            UpdateIcon(newNotifyIconCondition, icon, tooltipText);
        }

        private bool HasUpdateDisplayIntervalPassed()
        {
            bool retval = false;
            if (_lastPendingUpdateDisplayTime != DateTime.MinValue && _pendingUpdateReminderInterval != TimeSpan.Zero)
            {
                DateTime now = DateTime.Now;
                bool intervalHasPassed = ((now - _lastPendingUpdateDisplayTime) > _pendingUpdateReminderInterval);
                retval = intervalHasPassed;

            }
            return retval;
        }

        private void SetLastPendingUpdateDisplayTime()
        {
            _lastPendingUpdateDisplayTime = DateTime.Now;
        }

        private bool IsUpdateNotification(NotifyIconCondition condition)
        {
            bool isUpdateNotifction = (condition == NotifyIconCondition.UpdateAvailable ||
                                          condition == NotifyIconCondition.UpdateRequired);

            return isUpdateNotifction;
        }

        private NotifyIconCondition _notifyIconCondition;

        private void UpdateStatusStripText(String text)
        {
            if (this.InvokeRequired)
            {
                BeginInvoke((MethodInvoker)delegate() { UpdateStatusStripText(text); });
            }
            else
            {
                try
                {
                    if (_toolStripStatusLabel.Text != text)
                    {
                        _toolStripStatusLabel.Text = text;
                        _toolStripStatusLabel.Image = _imageManager.RefreshIcon;
                        _toolStripStatusLabel.ImageAlign= ContentAlignment.MiddleLeft ;
                    }
                }
                catch (Exception ex) { Trace.WriteLine(ex.ExceptionAsString()); }
            }
        }

        private void UpdateStatusStripImage(Image image)
        {
            if (this.InvokeRequired)
            {
                BeginInvoke((MethodInvoker)delegate() { UpdateStatusStripImage(image); });
            }
            else
            {
                try
                {
                    if (toolStripStatusLabel1.Image != image)
                    {
                        toolStripStatusLabel1.Image = image;
                    }
                }
                catch (Exception ex) { Trace.WriteLine(ex.ExceptionAsString()); }
            }
        }

        private void UncheckAllRefreshItems()
        {
            //_fiveSecondsToolStripMenuItem.Checked = false;
            //_tenSecondsToolStripMenuItem.Checked = false;
            //_thirtySecondsToolStripMenuItem.Checked = false;
            //_sixtySecondsToolStripMenuItem.Checked = false;
            //_twoMinutesToolStripMenuItem.Checked = false;
            //_fiveMinutesToolStripMenuItem.Checked = false;
            //_tenMinutesToolStripMenuItem.Checked = false;

            _contextFiveSecondsToolStripMenuItem.Checked = false;
            _contextTenSecondsToolStripMenuItem.Checked = false;
            _contextThirtySecondsToolStripMenuItem.Checked = false;
            _contextSixtySecondsToolStripMenuItem.Checked = false;
            _contextTwoMinutesToolStripMenuItem.Checked = false;
            _contextFiveMinutesToolStripMenuItem.Checked = false;
            _contextTenMinutesToolStripMenuItem.Checked = false;
        }

        private void RefreshIntervalchanged(object sender, EventArgs e)
        {
            RadioButton radio = (RadioButton) sender;
            RefreshTimeConstants refreshTimeConstants = (RefreshTimeConstants) radio.Tag;
            RefreshInterval(refreshTimeConstants);

        }

        private void RefreshInterval(RefreshTimeConstants refreshTimeConstants)
        {
            int time = Convert.ToInt32(refreshTimeConstants);
            AutoRefreshInterval = Convert.ToInt32(TimeSpan.FromSeconds(time).TotalMilliseconds);
            SaveAutoRefreshInterval(refreshTimeConstants, GetServerRegistration());
            if (_pulseWorkerThreadEvent != null)
            {
                _pulseWorkerThreadEvent.Set();
            }
        }

        //private void _fiveSecondsToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    UncheckAllRefreshItems();
        //    _fiveSecondsToolStripMenuItem.Checked = _contextFiveSecondsToolStripMenuItem.Checked = true;
        //    AutoRefreshInterval = Convert.ToInt32(TimeSpan.FromSeconds(5).TotalMilliseconds);
        //    SaveAutoRefreshInterval(GetServerRegistration());
        //    if (_pulseWorkerThreadEvent != null)
        //    {
        //        _pulseWorkerThreadEvent.Set();
        //    }
        //}

        //private void _tenSecondsToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    UncheckAllRefreshItems();
        //    _tenSecondsToolStripMenuItem.Checked = _contextTenSecondsToolStripMenuItem.Checked = true;
        //    AutoRefreshInterval = Convert.ToInt32(TimeSpan.FromSeconds(10).TotalMilliseconds);
        //    SaveAutoRefreshInterval(GetServerRegistration());
        //    if (_pulseWorkerThreadEvent != null)
        //    {
        //        _pulseWorkerThreadEvent.Set();
        //    }
        //}

        //private void _thirtySecondsToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    UncheckAllRefreshItems();
        //    _thirtySecondsToolStripMenuItem.Checked = _contextThirtySecondsToolStripMenuItem.Checked = true;
        //    AutoRefreshInterval = Convert.ToInt32(TimeSpan.FromSeconds(30).TotalMilliseconds);
        //    SaveAutoRefreshInterval(GetServerRegistration());
        //    if (_pulseWorkerThreadEvent != null)
        //    {
        //        _pulseWorkerThreadEvent.Set();
        //    }
        //}

        //private void _sixtySecondsToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    UncheckAllRefreshItems();
        //    _sixtySecondsToolStripMenuItem.Checked = _contextSixtySecondsToolStripMenuItem.Checked = true;
        //    AutoRefreshInterval = Convert.ToInt32(TimeSpan.FromSeconds(60).TotalMilliseconds);
        //    SaveAutoRefreshInterval(GetServerRegistration());
        //    if (_pulseWorkerThreadEvent != null)
        //    {
        //        _pulseWorkerThreadEvent.Set();
        //    }
        //}

        //private void _twoMinutesToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    UncheckAllRefreshItems();
        //    _twoMinutesToolStripMenuItem.Checked = _contextTwoMinutesToolStripMenuItem.Checked = true;
        //    AutoRefreshInterval = Convert.ToInt32(TimeSpan.FromMinutes(2).TotalMilliseconds);
        //    SaveAutoRefreshInterval(GetServerRegistration());
        //    if (_pulseWorkerThreadEvent != null)
        //    {
        //        _pulseWorkerThreadEvent.Set();
        //    }
        //}

        //private void _fiveMinutesToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    UncheckAllRefreshItems();
        //    _fiveMinutesToolStripMenuItem.Checked = _contextFiveMinutesToolStripMenuItem.Checked = true;
        //    AutoRefreshInterval = Convert.ToInt32(TimeSpan.FromMinutes(5).TotalMilliseconds);
        //    SaveAutoRefreshInterval(GetServerRegistration());
        //    if (_pulseWorkerThreadEvent != null)
        //    {
        //        _pulseWorkerThreadEvent.Set();
        //    }
        //}

        //private void _tenMinutesToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    UncheckAllRefreshItems();
        //    _tenMinutesToolStripMenuItem.Checked = _contextTenMinutesToolStripMenuItem.Checked = true;
        //    AutoRefreshInterval = Convert.ToInt32(TimeSpan.FromMinutes(10).TotalMilliseconds);
        //    SaveAutoRefreshInterval(GetServerRegistration());
        //    if (_pulseWorkerThreadEvent != null)
        //    {
        //        _pulseWorkerThreadEvent.Set();
        //    }
        //}

        //private void _noneToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    UncheckAllRefreshItems();
        //    AutoRefreshInterval = -1;
        //    SaveAutoRefreshInterval(GetServerRegistration());
        //    if (_pulseWorkerThreadEvent != null)
        //    {
        //        _pulseWorkerThreadEvent.Set();
        //    }
        //}

        private void _nowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_pulseWorkerThreadEvent != null)
            {
                _pulseWorkerThreadEvent.Set();
            }
        }

        private void _refreshToolStripSplitButton_ButtonClick(object sender, EventArgs e)
        {
            if (_pulseWorkerThreadEvent != null)
            {
                _pulseWorkerThreadEvent.Set();
            }
        }

        private void _settingsToolStripButton_Click(object sender, EventArgs e)
        { _settingsToolStripMenuItem_Click(sender, e); }

        private void _runOnLoginToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var serverRegistration = GetServerRegistration();
            serverRegistration.RunOnLogin = (sender as ToolStripMenuItem).Checked;
        }

        private static ServerRegistrationParams GetServerRegistrationParams()
        { return new ServerRegistrationParams(Common.ServerRegistrySubKeyPath, Common.DefaultCatalogServicePortNumber, new HelpHandler(HelpHandler)); }

        private static ServerRegistration GetServerRegistration()
        { return new ServerRegistration(GetServerRegistrationParams()); }

        private static ServerRegistry GetServerRegistry()
        { return new ServerRegistry(GetServerRegistrationParams()); }

        private static void HelpHandler(Object sender, ServerHelpEventArgs eventArgs)
        {
            Uri helpUri = null;
            switch (eventArgs.Context)
            {
                case HelpContext.EnterIPForm:
                    helpUri = CreateHelpUriFromFragment(Strings.HelpPathFragment_EnterIPForm);
                    break;
                case HelpContext.NetworkBrowserForm:
                    helpUri = CreateHelpUriFromFragment(Strings.HelpPathFragment_NetworkBrowserForm);
                    break;
                case HelpContext.ServerSelectionForm_Change:
                    helpUri = CreateHelpUriFromFragment(Strings.HelpPathFragment_ServerSelectionForm_Change);
                    break;
                case HelpContext.ServerSelectionForm_Initialize:
                    helpUri = CreateHelpUriFromFragment(Strings.HelpPathFragment_ServerSelectionForm_Initialize);
                    break;
                case HelpContext.SpecifyServer_WhatIsServer:
                    helpUri = CreateHelpUriFromFragment(Strings.HelpPathFragment_SpecifyServer_WhatIsServer);
                    break;
                case HelpContext.MainForm:
                    helpUri = CreateHelpUriFromFragment(Strings.HelpPathFragment_MainForm);
                    break;
            }
            if (helpUri != null)
            {
                ProcessStartInfo sInfo = new ProcessStartInfo(helpUri.AbsoluteUri);
                Process.Start(sInfo);
            }
        }

        /// <summary>
        /// Create the full UI for a help topic from a help path fragment
        /// </summary>
        /// <param name="helpPathFragment"></param>
        /// <returns></returns>
        static private Uri CreateHelpUriFromFragment(string helpPathFragment)
        {
            string baseHelpUrl = Common.ProductHelpBaseUrl;
            string fullHelpUrl = baseHelpUrl + helpPathFragment;
            Uri help = new Uri(fullHelpUrl);
            return help;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        { HelpHandler(sender, new ServerHelpEventArgs(HelpContext.MainForm, e)); }

        private void RestoreAutoRefreshInterval(ServerRegistration serverRegistration)
        {
            switch (serverRegistration.AutoRefreshInterval)
            {
                case 0:
                    RefreshInterval(RefreshTimeConstants.Fiveseconds);
                    break;
                case 1:
                    RefreshInterval(RefreshTimeConstants.TenSeconds);
                    break;
                case 2:
                    RefreshInterval(RefreshTimeConstants.ThirtySeconds);
                    break;
                case 3:
                    RefreshInterval(RefreshTimeConstants.SixtySeconds);
                    break;
                case 4:
                    RefreshInterval(RefreshTimeConstants.TwoMinutes);
                    break;
                case 5:
                    RefreshInterval(RefreshTimeConstants.FiveMinutes);
                    break;
                case 6:
                    RefreshInterval(RefreshTimeConstants.TenMinutes);
                    break;
            }
        }

       



        private void SaveAutoRefreshInterval(RefreshTimeConstants refreshTimeConstants,ServerRegistration serverRegistration)
        {
            Int32 newInterval = -1;
            switch (refreshTimeConstants)
            {
                case RefreshTimeConstants.Fiveseconds:
                    newInterval = 0;
                    break;
                case RefreshTimeConstants.TenSeconds:
                    newInterval = 1;
                    break;
                case RefreshTimeConstants.ThirtySeconds:
                    newInterval = 2;
                    break;
                case RefreshTimeConstants.SixtySeconds:
                    newInterval = 3;
                    break;
                case RefreshTimeConstants.TwoMinutes:
                    newInterval = 4;
                    break;
                case RefreshTimeConstants.FiveMinutes:
                    newInterval = 5;
                    break;
                case RefreshTimeConstants.TenMinutes:
                    newInterval = 6;
                    break;
                default:
                    newInterval = -1;
                    break;
            }
            serverRegistration.AutoRefreshInterval = newInterval;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    _recentEntriesThreshold = TimeSpan.FromSeconds(0);
                    break;
                case 1:
                    _recentEntriesThreshold = TimeSpan.FromHours(1);
                    break;
                case 2:
                    _recentEntriesThreshold = TimeSpan.FromDays(1);
                    break;
                case 3:
                    _recentEntriesThreshold = TimeSpan.FromDays(7);
                    break;
            }

            _nowToolStripMenuItem_Click(null, null);
        }

        private void _notifyIcon_BalloonTipClicked(object sender, EventArgs e)
        { ShowMonitorWindow(null, null); }

        private void _notifyIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                ShowMonitorWindow(null, null);
            }
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_CONNECTOR_SHOW_NORMAL)
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                ShowInTaskbar = true;
                WindowState = FormWindowState.Normal;
                this.Activate();
                if ((null != _tabControl) && (_tabControl.TabCount > 0))
                    _tabControl.SelectedTab = _tabControl.TabPages[0];
            }
            else if (m.Msg == WM_QUERYENDSESSION)
            {
                Trace.WriteLine(String.Format("ConnectorServiceMonitor processing WM_QUERYENDSESSION: {0} {1}", m.LParam, m.WParam));

                // Indicate it is ok to let the system shut us down
                m.Result = (IntPtr)1;
            }
            else if (m.Msg == WM_ENDSESSION)
            {
                Trace.WriteLine(String.Format("ConnectorServiceMonitor processing WM_ENDSESSION: {0} {1}", m.LParam, m.WParam));

                m.Result = (IntPtr)0;

                if (m.WParam == (IntPtr)1)
                {
                    // Invoke normal Exit handling logic to actually do the shutdown
                    exitToolStripMenuItem_Click(null, null);
                }
            }
        }

        private const int WM_CONNECTOR_SHOW_NORMAL = 0x0400 + 0x0995;  //WM_USER + 995
        private const int WM_QUERYENDSESSION = 0x11;
        private const int WM_ENDSESSION = 0x16;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass"), DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool PostMessage(
            IntPtr hWnd, // handle to destination window
            uint Msg, // message
            IntPtr wParam, // first message parameter
            IntPtr lParam // second message parameter
            );

        private Thread _thread;
        private readonly Object _syncRoot = new Object();
        private TimeSpan _pendingUpdateReminderInterval = TimeSpan.FromHours(2);
        private DateTime _lastPendingUpdateDisplayTime = DateTime.MinValue;
        private Boolean _exiting;
        private Int32 _autoRefreshInterval = -1;
        private DateTime _lastRefreshDateTime;
        private ManualResetEvent _terminateWorkerThreadEvent;
        private AutoResetEvent _pulseWorkerThreadEvent;
        private String _machineName;
        private Int32 _portNumber;
        private readonly ImageManager _imageManager;
        private readonly StartPageHtmlRenderer _startPageHtmlRenderer;
        private readonly GeneralPageHtmlRenderer _generalPageHtmlRenderer;
        private readonly ConnectionsPageHtmlRenderer _connectionsPageHtmlRenderer;
        private readonly RequestsPageHtmlRenderer _requestsPageHtmlRenderer;
        private TimeSpan _recentEntriesThreshold = TimeSpan.FromSeconds(0);
        private ConnectorState _cachedConnectorState;
      
        //public const int WM_NCLBUTTONDOWN = 0xA1;
        //public const int HT_CAPTION = 0x2;

        //[DllImportAttribute("user32.dll")]
        //public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        //[DllImportAttribute("user32.dll")]
        //public static extern bool ReleaseCapture();

        //private void Form1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        //{
        //    if (e.Button == MouseButtons.Left)
        //    {
        //        ReleaseCapture();
        //        SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
        //    }
        //}

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        
       
    }

    
   
}
