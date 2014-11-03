using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Sage.Connector.Cloud.Integration.Interfaces.Requests;
using Sage.Connector.Cloud.Integration.Interfaces.Responses;
using Sage.Connector.ConnectorServiceCommon;
using Sage.Connector.Data;
using Sage.Connector.MockCloudHostApp.Internal;
using Sage.Connector.SageCloudService;
using Sage.Connector.Utilities;

namespace Sage.Connector.MockCloudHostApp
{
    internal partial class TenantPanel : UserControl
    {
        private HostAppAutomationWorker _automationWorker;
     
        private readonly ActionInvoker _actionInvoker;
        
        //TODO: map invoke enum to selection 
        // title, enum value, position

        //keep a copy for debugging
        private readonly int _tenantIndex;

        private Timer _automationTimer;
        

        public bool AutomatedPremise
        {
            get
            {
                bool retVal = false;
                if (_automationWorker != null)
                {
                    retVal = _automationWorker.SetupForAutomation;
                }
                return retVal;
            }
        }

        public bool AutomationRunning
        { get; set; }

        public string TenantId
        { get { return _automationWorker.TenantId; } }

        public TenantPanel(int tenantIndex, ActionInvoker actionInvoker)
        {
            InitializeComponent();

            _actionInvoker = actionInvoker;
            _tenantIndex = tenantIndex;

            Initialize(_tenantIndex);
        }

        private void Initialize(int tenantIndex)
        {
            grpTenant.Text = String.Format(grpTenant.Text, tenantIndex);
            InitializeAutomation(tenantIndex);
        }

        private void InitializeAutomation(Int32 tenantIndex)
        {
            string tenantId = MockCloudService.TenantIds.ToArray()[tenantIndex];
            _automationWorker = new HostAppAutomationWorker(
                MockCloudService.AutomationEndpointAddress(tenantId),
                MockCloudService.TenantIds.ToArray()[tenantIndex],
                MockCloudService.RetrievePremiseKey(tenantId),
                MockCloudService.AutomationTestTime(tenantId),
                MockCloudService.AutomationRequestDelay(tenantId),
                MockCloudService.AutomationInitialRequests(tenantId),
                MockCloudService.AutomationContinuedRequests(tenantId),
                MockCloudService.RetrieveConfigParams(tenantId),
                MockCloudService.RetrieveConfigParamsCustomUpdate(tenantId),
                MockCloudService.RetrieveGatewayServiceInfo(tenantId));
        }

        private void InitializePremiseConfiguration()
        {
            if (!string.IsNullOrEmpty(_automationWorker.TenantEndpointAddress))
            {
                //TODO: JSB Review this stuff. Seems off.
                PremiseConfigurationRecord record = ConfigurationSettingFactory.CreateNewTenant();
                string compNumber = (_tenantIndex + 1).ToString();
                if (_tenantIndex + 1 < 10) compNumber = compNumber.Insert(0, "0");

                record.BackOfficeCompanyName = "COMPANY" + compNumber;
                record.BackOfficeConnectionEnabledToReceive = true;
                record.SiteAddress = _automationWorker.TenantEndpointAddress;
                record.CloudConnectionEnabledToReceive = true;
                record.CloudConnectionEnabledToSend = true;
                record.CloudTenantId = _automationWorker.TenantId;
                record.CloudPremiseKey = _automationWorker.PremiseKey;
                //TODO: should we factor out the Mock name and product name?
                record.ConnectorPluginId = "Mock";
                record.BackOfficeProductName = "Mock Back Office Product";
                //TODO: JSB add correct init for BackOfficeConnectionCredentials
                record.BackOfficeConnectionCredentials = string.Empty;

                ConfigurationSettingFactory.SaveNewTenant(record);
            }
        }

        private void TenantPanel_Load(object sender, EventArgs e)
        {
            InitializeInvokeActionEnumInformation();
            InitializePremiseConfiguration();


            cboAction.SelectedIndex = 0;
            txtSiteId.Text = _automationWorker.TenantId;
            txtConnectionKey.Text = ConstructConnectionKey(_automationWorker.TenantId, _automationWorker.PremiseKey, MockCloudServiceHost.SiteAddress);
            AutomationRunning = false;

            if (_automationWorker.InitialRequestsToRun)
            {
                QueueInitialRequestsForAutomation();
            }

            if (_automationWorker.SetupForAutomation)
            {
                StartAutomation();
            }
        }

        private static readonly IList<InvokeActionSelection> _invokeActionList = new List<InvokeActionSelection>();

        private void InitializeInvokeActionEnumInformation()
        {
            SetUpInvokeActionList();
            _site1InvocationCounts = new Dictionary<InvokeActionEnum, int>();

            foreach (var enumVal in Enum.GetValues(typeof(InvokeActionEnum)))
            {
                _site1InvocationCounts.Add((InvokeActionEnum)enumVal, 0);
            }

            foreach (var invokeAction in _invokeActionList)
            {
                cboAction.Items.Add(invokeAction.Text);

            };
        }



        private string ConstructConnectionKey(string tenantIdAsString, string premiseKeyAsString, string baseUriAsString)
        {
            string connectionKey = string.Empty;
            string baseUri = string.Empty;
            if (!string.IsNullOrEmpty(baseUriAsString))
            {
                baseUri = Utils.ToBase64(baseUriAsString);
            }
            connectionKey = string.Concat(tenantIdAsString, ":", premiseKeyAsString, ":", baseUri);
            return connectionKey;
        }
        
        private void StartAutomation()
        {
            _automationTimer = new Timer();
            _automationTimer.Interval = (Int32)(_automationWorker.RequestDelaySeconds * 1000.0);
            _automationWorker.StopTime = DateTime.Now.AddMinutes(_automationWorker.TestTime);

            _automationTimer.Tick += new EventHandler(_automationTimer_Tick);
            AutomationRunning = true;
            AdjustUi();
            _automationTimer.Start();
        }

        private void _automationTimer_Tick(object sender, EventArgs e)
        {
            if (DateTime.Now < _automationWorker.StopTime)
            {

                MethodInvoker i = delegate()
                {
                    DoAutomatedWork();
                };

                if (this.InvokeRequired)
                {
                    this.BeginInvoke(i);
                }
                else
                {
                    i();
                }
            }
            else
            {
                _automationTimer.Stop();
                AutomationRunning = false;
                AdjustUi();
            }
        }

        /// <summary>
        /// Uses the next ConfigurationRequest in the Continued requests to push a request to queue.
        /// </summary>
        private void DoAutomatedWork()
        {
            ConfigurationRequest request = _automationWorker.GetNextWorkItem();
            DoWorkRequest(request);
        }

        //Enables/disables Invoke button during automated run.
        private void AdjustUi()
        {
            EnableInvokeButton(cboAction.SelectedIndex);
        }

        //Pushes all the InitialRequests to the queue in one shot.
        private void QueueInitialRequestsForAutomation()
        {
            try
            {
                foreach (ConfigurationRequest request in _automationWorker.InitialRequests)
                {
                    DoWorkRequest(request);
                }
            }
            catch (Exception)
            {
                //Do nothing... invalid request or no actions
            }
        }

        private void _site1InvokeButton_Click(object sender, EventArgs e)
        {
            InvokeActionEnum? action = GetInvokeAction(cboAction.SelectedIndex);
            if (action == null)
                throw new ApplicationException("action not found");

            Int32 count = System.Convert.ToInt32(_countTextBox.Text);
            for (Int32 i = 0; i < count; i++)
            {
                DoWorkRequest((InvokeActionEnum)action);
            }

            //Reselect the countTextBox to make it easy to change the number
            _countTextBox.Focus();
            if (!string.IsNullOrEmpty(_countTextBox.Text))
            {
                _countTextBox.SelectionStart = 0;
                _countTextBox.SelectionLength = _countTextBox.Text.Length;
                    
            }
        }


        private void DoWorkRequest(InvokeActionEnum action, object passThru = null)
        {
            //Create request
            ConfigurationRequest request = new ConfigurationRequest();
            request.RequestTypeName = action.ToString();
            request.PassThru = passThru;

            DoWorkRequest(request);
        }

        private void DoWorkRequest(ConfigurationRequest request)
        {
            InvokeActionEnum action = (InvokeActionEnum)Enum.Parse(typeof(InvokeActionEnum), request.RequestTypeName);
            IncrementCounter(action);

            _actionInvoker.PushAction(
                action, 
                _automationWorker.TenantId, 
                _automationWorker.PremiseKey, 
                _automationWorker.ConfigParams, 
                _automationWorker.GatewayServiceInfo, 
                request);
            if (action == InvokeActionEnum.DeleteTenant)
            { 
                KillTenant();
            }
        }

        private void IncrementCounter(InvokeActionEnum invoked)
        {
            _site1InvocationCounts[invoked] += 1;
            _site1ActionComboBox_SelectedIndexChanged(null, null);
        }

        private void KillTenant()
        {
            this.Enabled = false;
            if (AutomationRunning)
            {
                _automationTimer.Stop();
            }
        }

        private void _site1ActionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            InvokeActionEnum? action = GetInvokeAction(cboAction.SelectedIndex);
            EnableInvokeButton(action);
            txtRequestCount.Text = "";

            if (action == null)
                return;

            txtRequestCount.Text = _site1InvocationCounts[((InvokeActionEnum)action)].ToString(CultureInfo.CurrentCulture);
        }

        //Doesn't enable invoke button for Custom action.  Custom only used for automation.
        private void EnableInvokeButton(int index)
        {
            InvokeActionEnum? action = GetInvokeAction(index);

            EnableInvokeButton(action);
        }


        //Doesn't enable invoke button for Custom action.  Custom only used for automation.
        private void EnableInvokeButton(InvokeActionEnum? action)
        {
            if (action == null)
        {
                btnInvoke.Enabled = false;
                return;
            }

            if (AutomationRunning)
            {
                btnInvoke.Enabled = false;
            }
            else
            {

                btnInvoke.Enabled = (!action.Equals(InvokeActionEnum.UpdateCustomConfigParams));
            }
            }

        private InvokeActionEnum? GetInvokeAction(int indexSelected)
        {
            if (indexSelected >= 0 && indexSelected < _invokeActionList.Count)
            {
                return _invokeActionList[indexSelected].Action;
            };

            return null;
        }

        /// <summary>
        /// Update the count 
        /// </summary>
        public void UpdateCounts()
        {
            var counts = MockCloudService.StaticExternalPeekTotalMessageCounts(_automationWorker.TenantId);
            txtInboxCount.Text = counts.Item1.ToString(CultureInfo.CurrentCulture);
            txtOutboxCount.Text = counts.Item2.ToString(CultureInfo.CurrentCulture);
            txtErrorCount.Text = counts.Item3.ToString(CultureInfo.CurrentCulture);
        }


        private IDictionary<InvokeActionEnum, int> _site1InvocationCounts = new Dictionary<InvokeActionEnum, int>();

        /// <summary>
        /// End tenants test run
        /// </summary>
        public void StopAutomation()
        {
            _automationTimer.Stop();
        }

        private void btnRequestConfiguration_Click(object sender, EventArgs e)
        {
            ConfigurationUpdate();
        }

        private void ConfigurationUpdate()
        {
            var currentConfig = MockCloudService.UpdateConfigurationRequest(TenantId);
            bool alreadyRequestPending = MockCloudService.IsUpdateConfigurationRequestPending(TenantId);

            using (var form = new ConfigurationForm(currentConfig, alreadyRequestPending))
            {
                if (form.ShowDialog(Parent) == DialogResult.OK)
                {
                    if (form.View.IsDirty)
                    {
                        var updatedConfig = form.View.GetUpdatedConfiguration();
                        DoWorkRequest(InvokeActionEnum.UpdateCustomConfigParams, updatedConfig);
                    }
                }
            }
        }


        private void chkDisableTenant_CheckedChanged(object sender, EventArgs e)
        {
            MockCloudService.DisableTenant(TenantId, chkDisableTenant.Checked);
        }

        private void txtConnectionKey_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            txtConnectionKey.SelectAll();
        }

        private void btnInbox_Click(object sender, EventArgs e)
        {
            IEnumerable<Response> items = MockCloudService.StaticExternalPeekInboxMessage(TenantId);

            using (Form f = new MessageQueueForm(items))
            { 
                f.Text = string.Format("Inbox for tenant {0}", TenantId);
                f.ShowDialog();
            }
        }

        private void btnOutbox_Click(object sender, EventArgs e)
        {
            IEnumerable<Request> items = MockCloudService.StaticExternalPeekOutboxMessage(TenantId);

            using (Form f = new MessageQueueForm(items))
            {
                f.Text = string.Format("Outbox for tenant {0}", TenantId);
                f.ShowDialog();
            }
        }


        private static void SetUpInvokeActionList()
        {
            if (_invokeActionList.Any())
                return;
            _invokeActionList.Add(new InvokeActionSelection("Loopback request", InvokeActionEnum.Loopback));
            _invokeActionList.Add(new InvokeActionSelection("Get BackOffice Plugins via Metadata", InvokeActionEnum.GetBackOfficePluginConfiguration));
            _invokeActionList.Add(new InvokeActionSelection("Sync Tax Codes", InvokeActionEnum.SyncTaxCodes));
            _invokeActionList.Add(new InvokeActionSelection("Sync Tax Schedules", InvokeActionEnum.SyncTaxSchedules));
            _invokeActionList.Add(new InvokeActionSelection("Sync Customers", InvokeActionEnum.SyncCustomers));
            _invokeActionList.Add(new InvokeActionSelection("Sync Salespersons", InvokeActionEnum.SyncSalespersons));
            _invokeActionList.Add(new InvokeActionSelection("Sync SalespersonCustomers", InvokeActionEnum.SyncSalespersonCustomers));
            _invokeActionList.Add(new InvokeActionSelection("Sync Inventory Items", InvokeActionEnum.SyncInventoryItems));
            _invokeActionList.Add(new InvokeActionSelection("Sync Service Types", InvokeActionEnum.SyncServiceTypes));
            _invokeActionList.Add(new InvokeActionSelection("Sync Invoices", InvokeActionEnum.SyncInvoices));
            _invokeActionList.Add(new InvokeActionSelection("Sync Invoice Balances", InvokeActionEnum.SyncInvoiceBalances));
            _invokeActionList.Add(new InvokeActionSelection("Scheduled Synchronization", InvokeActionEnum.ScheduledSynchronization));
            _invokeActionList.Add(new InvokeActionSelection("Process Quote request", InvokeActionEnum.ProcessQuote));
            _invokeActionList.Add(new InvokeActionSelection("Process Quote To Order request", InvokeActionEnum.ProcessQuoteToOrder));
            _invokeActionList.Add(new InvokeActionSelection("Process Paid Order request", InvokeActionEnum.ProcessPaidOrder));
            _invokeActionList.Add(new InvokeActionSelection("Process Payment request", InvokeActionEnum.ProcessPayment));
            _invokeActionList.Add(new InvokeActionSelection("Process Statements request", InvokeActionEnum.ProcessStatements));
            _invokeActionList.Add(new InvokeActionSelection("Process Work Order to Invoice request", InvokeActionEnum.ServiceProcessWorkOrderToInvoice));
            _invokeActionList.Add(new InvokeActionSelection("Health check message", InvokeActionEnum.HealthCheck));
            _invokeActionList.Add(new InvokeActionSelection("DELETE TENANT", InvokeActionEnum.DeleteTenant));
            _invokeActionList.Add(new InvokeActionSelection("Validate back office is installed", InvokeActionEnum.ValidateBackOfficeIsInstalled));
            _invokeActionList.Add(new InvokeActionSelection("Get plugin information", InvokeActionEnum.GetPluginInformation));
            _invokeActionList.Add(new InvokeActionSelection("Get full plugin information collection", InvokeActionEnum.GetPluginInformationCollection));
            _invokeActionList.Add(new InvokeActionSelection("Get installed back office plugin information collection", InvokeActionEnum.GetInstalledBackOfficePluginInformationCollection));

            _invokeActionList.Add(new InvokeActionSelection("Get GetCompanyConnectionManagementCredentialsNeeded", InvokeActionEnum.GetCompanyConnectionManagementCredentialsNeeded));
            //_invokeActionList.Add(new InvokeActionSelection("Get GetCompanyConnectionCredentialsNeeded", InvokeActionEnum.GetCompanyConnectionCredentialsNeeded));
            //_invokeActionList.Add(new InvokeActionSelection("Get ValidateCompanyConnectionCredentials", InvokeActionEnum.ValidateCompanyConnectionCredentials));
            //_invokeActionList.Add(new InvokeActionSelection("Get ValidateCompanyConnectionManagementCredentials", InvokeActionEnum.ValidateCompanyConnectionManagementCredentials));

        }
        class InvokeActionSelection
        {
            public InvokeActionSelection(String text, InvokeActionEnum action)
            {
                Text = text;
                Action = action;
            }
            public string Text { get; private set; }
            public InvokeActionEnum Action { get; private set; }
        }
       
    }
}
