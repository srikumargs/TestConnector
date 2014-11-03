using System;
using System.Drawing;
using System.Windows.Forms;
using ConnectorServiceMonitor.ViewModel;
using Sage.Activation;
using Sage.Connector.Common;
using System.Diagnostics;

namespace ConnectorServiceMonitor.Internal
{
    internal partial class SpecifyServer : UserControl
    {
        #region private fields
        private ServerSelectionMode _mode = ServerSelectionMode.InitializeServer;
        #endregion

        public SpecifyServer()
        {
            InitializeComponent();
            this.whatLinkLabel.Text = String.Format(this.whatLinkLabel.Text, Common.BriefProductName);
        }

        public event EventHandler ServerSuccessfullySpecified = delegate { };

        public ServerSelectionMode Mode
        {
            get { return _mode; }
            set { _mode = value; }
        }

        public IServerRegistry ServerRegistry
        { set { _serverRegistry = value; } }

        private IServerRegistry _serverRegistry;

        #region private event handlers
        private void SpecifyServer_Load(object sender, EventArgs e)
        {
            if (Mode == ServerSelectionMode.InitializeServer)
            {
                this.specifyServerLabel.Text = Strings.SPECIFY_SERVER;
            }
            else
            {
                this.specifyServerLabel.Text = Common.ReplaceKnownTerms(Strings.CHANGE_SERVER);
            }
        }

        private void selectServerLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Cursor oldCursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;
            try
            {
                using (NetworkBrowserForm form = new NetworkBrowserForm())
                {
                    form.ServerRegistry = _serverRegistry;
                    if (form.ShowDialog(this) == DialogResult.OK)
                    {
                        ServerSuccessfullySpecified(this, new StringEventArgs(form.Server));
                    }
                }
            }
            catch (Exception ex) 
            {
                Trace.WriteLine(ex.ExceptionAsString());
            }
            finally
            {
                this.Cursor = oldCursor;
            }
        }

        private void enterIPAddressLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Cursor oldCursor = this.Cursor;
            this.Cursor = Cursors.WaitCursor;
            try
            {
                using (EnterIPForm form = new EnterIPForm())
                {
                    form.ServerRegistry = _serverRegistry;
                    if (form.ShowDialog(this) == DialogResult.OK)
                    {
                        ServerSuccessfullySpecified(this, new StringEventArgs(form.Server));
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ExceptionAsString());
            }
            finally
            {
                this.Cursor = oldCursor;
            }
        }
        #endregion

        private void whatLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            _serverRegistry.HelpHandler(this, new ServerHelpEventArgs(HelpContext.SpecifyServer_WhatIsServer, new HelpEventArgs(new Point()))); 
        }

        #region private methods
        #endregion
    }
}
