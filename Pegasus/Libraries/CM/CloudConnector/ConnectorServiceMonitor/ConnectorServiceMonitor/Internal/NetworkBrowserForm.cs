using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ConnectorServiceMonitor.ViewModel;
using Sage.Connector.Common;
using Sage.CRE.Core.UI;
using Sage.Net;

namespace ConnectorServiceMonitor.Internal
{
    internal partial class NetworkBrowserForm : Form
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public NetworkBrowserForm()
        {
            InitializeComponent();
            this._selectLbl.Text = String.Format(this._selectLbl.Text, Common.BriefProductName);
            this.Closing += new CancelEventHandler(NetworkBrowser_Closing);
        }

        public String Server
        { get { return _server; } }

        public IServerRegistry ServerRegistry
        { set { _serverRegistry = value; } }

        #region private event handlers
        private void NetworkBrowser_Load(Object sender, EventArgs e)
        {
            _portTextBox.Text = Convert.ToString(_serverRegistry.DefaultCatalogServicePort);
            PostMessage(this.Handle, WM_POPULATE, IntPtr.Zero, IntPtr.Zero);
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_POPULATE)
            {
                lock (_syncObject)
                {
                    if (_progressForm == null)
                    {
                        _progressForm = new ProgressForm();
                        _progressForm.UserCanRequestCancel = true;
                        _progressForm.UserCancelled += new EventHandler(_progressForm_UserCancelled);

                        _backgroundWorker = new System.ComponentModel.BackgroundWorker();
                        _backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(_backgroundWorker_Populate);
                        _backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(_backgroundWorker_PopulateCompleted);
                        _backgroundWorker.WorkerSupportsCancellation = true;
                        _backgroundWorker.RunWorkerAsync();
                        _progressForm.Text = "Browse Network";
                        _progressForm.ShowDialog(this.ParentForm);
                    }
                }
            }

            base.WndProc(ref m);
        }

        void _progressForm_UserCancelled(Object sender, EventArgs e)
        {
            lock (_syncObject)
            {
                try
                {
                    _backgroundWorker.CancelAsync();
                    _progressForm.Dispose();
                    _progressForm = null;

                    Close();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.ExceptionAsString());
                }
            }
        }

        private void _backgroundWorker_Populate(Object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            _progressForm.WaitUntilReady();
            _progressForm.ShowMarqueeProgressBar();
            LoadServerList();
        }

        private void _backgroundWorker_PopulateCompleted(Object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            lock (_syncObject)
            {
                try
                {
                    if (_progressForm != null && !_progressForm.UserHasRequestedCancel)
                    {
                        _progressForm.Dispose();
                        _progressForm = null;

                        MethodInvoker i = delegate() { _listView.Items[0].Selected = true; };

                        if (this.InvokeRequired)
                        {
                            this.BeginInvoke(i);
                        }
                        else
                        {
                            try
                            {
                                i();
                            }
                            catch (Exception ex) { Trace.WriteLine(ex.ExceptionAsString()); }

                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.ExceptionAsString());
                }
            }
        }

        private void _helpBtn_Click(Object sender, EventArgs e)
        { DisplayHelp(); }

        private void NetworkBrowser_HelpRequested(Object sender, HelpEventArgs hlpevent)
        { DisplayHelp(); }

        private void NetworkBrowser_Closing(Object sender, CancelEventArgs e)
        {
            if (this.DialogResult == DialogResult.OK)
            {
                if (!AcceptDialog())
                {
                    e.Cancel = true;
                }
            }
        }
        #endregion

        private delegate void StringDelegate(String myArg2);

        #region private methods
        /// <summary>
        /// Initialize and load the server list
        /// </summary>
        private void LoadServerList()
        {
            INetworkInfo netInfo = (INetworkInfo)new NetworkInfo();
            IServerCollection servers = netInfo.Servers;
            servers.Populate();

            StringDelegate i = new StringDelegate(delegate(String s)
                {
                    _listView.Items.Add(s, 0);
                    _listView.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.ColumnContent);
                });

            foreach (IServer s in servers)
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(i, s.Name);
                }
                else
                {
                    try
                    {
                        i(s.Name);
                    }
                    catch (Exception ex) { Trace.WriteLine(ex.ExceptionAsString()); }
                }
            }
        }

        private IServerRegistry _serverRegistry;

        private Boolean AcceptDialog()
        {
            if (_listView.SelectedItems.Count > 0)
            {
                _server = String.Format("{0}:{1}", _listView.SelectedItems[0].Text, _portTextBox.Text);
                return ConnectorServiceMonitor.Internal.ServerValidation.ValidateAndSetServer(_serverRegistry, _server, this);
            }
            else
            {
                return false;
            }
        }

        private void _listView_MouseDoubleClick(Object sender, MouseEventArgs e)
        {
            if (AcceptDialog())
            {
                this.DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void DisplayHelp()
        {
            _serverRegistry.HelpHandler(this, new ServerHelpEventArgs(HelpContext.NetworkBrowserForm, new HelpEventArgs(new Point())));
        }
        #endregion

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060:MovePInvokesToNativeMethodsClass"), DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool PostMessage(
            IntPtr hWnd, // handle to destination window
            uint Msg, // message
            IntPtr wParam, // first message parameter
            IntPtr lParam // second message parameter
            );

        private const int WM_POPULATE = 0x0400 + 0x0997;  //WM_USER + 997

        private String _server;
        private BackgroundWorker _backgroundWorker = new BackgroundWorker();
        private ProgressForm _progressForm;
        private Object _syncObject = new Object();
    }
}

