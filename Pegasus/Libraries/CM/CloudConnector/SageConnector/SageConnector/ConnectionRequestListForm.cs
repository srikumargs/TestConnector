using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Sage.Connector.Common;
using Sage.Connector.NotificationService.Proxy;
using SageConnector.Properties;
using SageConnector.ViewModel;

namespace SageConnector
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ConnectionRequestListForm : Form
    {
        /// <summary>
        /// Constructor takes Configuration View Model
        /// </summary>
        /// <param name="config"></param>
        public ConnectionRequestListForm(ConfigurationViewModel config)
        {
            InitializeComponent();
            _configuration = config;
        }

        private ConnectionRequestListViewModel _view;
        private readonly ConfigurationViewModel _configuration;
        private NotificationCallbackInstanceHelper _callbackInstance;
        private NotificationSubscriptionServiceProxy _notificationSubscriptionServiceProxy;

        /// <summary>
        /// Timer to execute periodic application of refresh data, if available
        /// Note: This MUST be a Forms.Timer, since we want it to execute on the 
        /// Application's UI thread.  Also, it does not pre-empt executing application code.
        /// </summary>
        private System.Windows.Forms.Timer _applyRefreshDataTimer;

        private Boolean _bRefreshNeeded = false;

        private PropertyDescriptor SortedProperty { get; set; }
        private ListSortDirection SortDirection { get; set; }

        private void btnDeleteRequests_Click(object sender, EventArgs e)
        {
            DeleteRequests();
        }

        private void DeleteRequests()
        {
            int selectedRows = dgRequestList.SelectedRows.Count;
            if (selectedRows > 0)
            {
                if (MessageBox.Show(string.Format(Resources.ConnectorRequestList_ConfirmDialogMessage,selectedRows.ToString()), 
                    Resources.ConnectorRequestList_ConfirmDialogCaption, 
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                {
                    try
                    {
                        List<string> idsToCancel = new List<string>();
                        foreach (DataGridViewRow row in dgRequestList.SelectedRows)
                        {
                            var rli = row.DataBoundItem as RequestListItem;
                            if (null != rli)
                            {
                                idsToCancel.Add(rli.ActivityTrackingContextId);
                            }
                        }
                        PerformRequestCancelation(idsToCancel);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(string.Format(Resources.ConnectorRequestList_ErrorDeletingText,Environment.NewLine, ex.Message), Resources.ConnectorRequestList_ErrorDeleteingCaption, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ConnectionRequestListForm_Load(object sender, EventArgs e)
        {
            ConfigureDialog();
         
            _view = new ConnectionRequestListViewModel(_configuration.CloudTenantId);
            if (_view == null)
            {
                MessageBox.Show(this,
                    Resources.ConnectorRequestList_ErrorMessageText, 
                    Resources.ConnectorRequestList_ErrorMessageCaption, MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                DialogResult = DialogResult.Cancel;
                this.Close();
            }
            else
            {
                PopulateRequestDataGrid();
                SubscribeToNotifications();
                SetupApplyRefreshDataProcess();
            }
        }

        private void dgRequestList_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            dgRequestList.ClearSelection();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshGrid();
        }
        
        private void lnkHelpLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ConnectorUtilities.ShowRequestFormHelp();
        }

        private void ConnectionRequestListForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            ClearSubscriptions();
        }

        private void dgRequestList_SelectionChanged(object sender, EventArgs e)
        {
            btnDeleteRequests.Enabled = dgRequestList.SelectedRows.Count > 0;
        }

        #region Helper Methods

        private void SaveSortOrder()
        {
            //Save grid sort
            GridSortColumn = dgRequestList.SortedColumn;
            GridSortDirection = dgRequestList.SortOrder == SortOrder.Descending
                                    ? ListSortDirection.Descending
                                    : ListSortDirection.Ascending;
            
            //Save datasource sort
            Type theType = typeof(RequestListItem);
            PropertyDescriptorCollection pdCollection = TypeDescriptor.GetProperties(theType);
            if (GridSortColumn != null)
            {
                SortedProperty = pdCollection.Find(GridSortColumn.DataPropertyName, false);
            }

            SortDirection = dgRequestList.SortOrder == SortOrder.Descending
                                ? ListSortDirection.Descending
                                : ListSortDirection.Ascending;
        }

        private DataGridViewColumn GridSortColumn { get; set; }
        private ListSortDirection GridSortDirection { get; set; }

        private void ApplyGridSort()
        {
            if (GridSortColumn != null)
            {
                dgRequestList.Sort(GridSortColumn, GridSortDirection);
                dgRequestList.Refresh();
            }
        }
        private void SaveGridState()
        {
            SaveSelections();
            SaveSortOrder();
        }
        private void RestoreGridState()
        {
            ApplyGridSort();
            RestoreSelections();
        }
        private void SaveSelections()
        {
            _selectedIds = new List<string>();
            foreach (DataGridViewRow row in dgRequestList.SelectedRows)
            {
                var rli = row.DataBoundItem as RequestListItem;
                _selectedIds.Add(rli.RequestId);
            }
        }
        private List<string> _selectedIds;

        private void RestoreSelections()
        {
            if (_selectedIds.Count > 0)
            {
                foreach (DataGridViewRow row in dgRequestList.Rows)
                {
                    var rli = row.DataBoundItem as RequestListItem;
                    row.Selected = _selectedIds.IndexOf(rli.RequestId) >= 0;
                }
            }
        }
        private void RefreshGrid()
        {
            if (!_view.RefreshingRequests)
            {
                try
                {
                    Cursor.Current = Cursors.WaitCursor;
                    SaveGridState();

                    _view.RefreshRequestList(SortedProperty, SortDirection);
                    dgRequestList.DataSource = _view.Requests;
                    lblLastRefreshTime.Text = DateTime.Now.ToString("G");
                    RestoreGridState();
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }
            }
        }
        private void ClearSubscriptions()
        {
            if (_callbackInstance != null && _notificationSubscriptionServiceProxy != null)
            {
                try
                {
                    _callbackInstance.Unsubscribe(_notificationSubscriptionServiceProxy);
                    _notificationSubscriptionServiceProxy.Disconnect();
                    _notificationSubscriptionServiceProxy.Close();
                }
                catch (Exception ex)
                {
                    using (var logger = new SimpleTraceLogger())
                    {
                        logger.WriteError(null, ex.ExceptionAsString());
                    }
                }
                finally
                {
                    _notificationSubscriptionServiceProxy.Abort();
                    _notificationSubscriptionServiceProxy = null;
                    _callbackInstance = null;
                }
            }
        }

        private void SubscribeToNotifications()
        {
            try
            {
                _callbackInstance = new NotificationCallbackInstanceHelper();
                _notificationSubscriptionServiceProxy =
                    NotificationSubscriptionServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber, _callbackInstance);
                _notificationSubscriptionServiceProxy.Connect();
                
                _callbackInstance.SubscribeBinderElementEnqueued(_notificationSubscriptionServiceProxy, BoundRequestCompleted);
                _callbackInstance.SubscribeBinderElementCompleted(_notificationSubscriptionServiceProxy, BoundRequestCompleted);
            }
            catch (Exception ex )
            {
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(null, ex.ExceptionAsString());
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="elementId"></param>
        public void BoundRequestCompleted(String tenantId, String elementId)
        {
            if (tenantId.Equals(_configuration.CloudTenantId))
            {
                _bRefreshNeeded = true;
            }
        }
        private delegate void NotificationDelegate(string tenantId, string elementId );

        private void PopulateRequestDataGrid()
        {
            dgRequestList.AutoGenerateColumns = false;

            colTimeRequested.DataPropertyName = "TimeRequested";
            colRequestedBy.DataPropertyName = "RequestingUser";
            colRequestId.DataPropertyName = "RequestId";
            colRequestType.DataPropertyName = "RequestType";
            colRequestStatusImage.DataPropertyName = "RequestStatusImage";
            colRequestProjectName.DataPropertyName = "RequestProjectName";
            colRequestSummary.DataPropertyName = "RequestSummary";
            colTimeElapsed.DataPropertyName = "TimeElapsedString";
            RefreshGrid();
        }

        private void PerformRequestCancelation(List<string> activityIds)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                _view.CancelRequest(activityIds);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void ConfigureDialog()
        {
            this.Text = Resources.ConnectorRequestList_DialogCaption;

            //Fill in Information section
            lblBackofficeNameData.Text = _configuration.BackOfficeCompanyName;
            lblCompanyNameData.Text = _configuration.CloudCompanyName;
            int width = _configuration.CloudConnectionStatusImage.Width;
            int height = _configuration.CloudConnectionStatusImage.Height;
            picTenantCompanyStatusImage.Size = new Size(width, height);
            picTenantCompanyStatusImage.Image = _configuration.CloudConnectionStatusImage;
            picBOCompanyStatusImage.Size = new Size(width, height);
            picBOCompanyStatusImage.Image = _configuration.BackOfficeConnectionStatusImage;
            picArrow.Image = Resources.arrow32x16;

            //Configure Grid Data area
            btnDeleteRequests.Text = Resources.ConnectorRequestList_DeleteButton;
            btnClose.Text = Resources.ConnectorRequestList_Close;
            btnRefresh.Text = Resources.ConnectorRequestList_RefreshButton;
            colRequestType.HeaderText = Resources.ConnectorRequestList_ColumnRequestTypeTitle;
            colTimeRequested.HeaderText = Resources.ConnectorRequestList_ColumnTimeRequestedTitle;
            colRequestedBy.HeaderText = Resources.ConnectorRequestList_ColumnRequestedByTitle;
            colRequestProjectName.HeaderText = Resources.ConnectorRequestList_ColumnProjectNameTitle;
            colRequestSummary.HeaderText = Resources.ConnectorRequestList_ColumnRequestSummeryTitle;
            colRequestStatusImage.HeaderText = Resources.ConnectorRequestList_ColumnRequestStatusImageTitle;
            colTimeElapsed.HeaderText = Resources.ConnectorRequestList_ColumnRequestTimeElapsedTitle;
        }

        #endregion

        private void Selections_Click(object sender, EventArgs e)
        {
            SelectionMenuStrip.Show(Selections, new Point(0,0), ToolStripDropDownDirection.AboveRight);
        }

        private void selectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dgRequestList.SelectAll();
        }

        private void clearSelectionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dgRequestList.ClearSelection();
        }

        #region Timer To Populate UI With Refresh Data

        /// <summary>
        /// Set up and kick off the apply refresh data timer
        /// </summary>
        private void SetupApplyRefreshDataProcess()
        {
            _applyRefreshDataTimer = new System.Windows.Forms.Timer();
            _applyRefreshDataTimer.Tick += new EventHandler(ApplyRefreshDataHandler);
            _applyRefreshDataTimer.Interval = ConnectorRegistryUtils.ConnectorApplyRefreshDataInterval;
            _applyRefreshDataTimer.Start();
        }

        /// <summary>
        /// Coordinates the execution of the method to refresh connection statuses
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ApplyRefreshDataHandler(object sender, System.EventArgs e)
        {
            try
            {
                // Disable timer for debug purposes, in case someone
                // Sets the interval to be very short
                _applyRefreshDataTimer.Enabled = false;

                // Call the actual apply refresh data method
                ApplyRefreshData();
            }
            finally
            {
                // Re-enable when complete
                _applyRefreshDataTimer.Enabled = true;
            }
        }

        /// <summary>
        /// Does the actual work for the periodic refresh
        /// </summary>
        private void ApplyRefreshData()
        {
            if (_bRefreshNeeded)
            {
                _bRefreshNeeded = false;
                RefreshGrid();
            }
        }
        #endregion

    }
}
