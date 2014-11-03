using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;
using ConnectorServiceMonitor.ViewModel;

namespace ConnectorServiceMonitor.Internal
{
    internal partial class EnterIPForm : Form
    {
        public EnterIPForm()
        {
            InitializeComponent();
            this._ipAddressLabel.Text = String.Format(_ipAddressLabel.Text, Common.BriefProductName);
            this.Closing += new CancelEventHandler(EnterIPForm_Closing);
        }

        public String Server
        { get { return _ipAddressTextBox.Text; } }

        public IServerRegistry ServerRegistry
        { set { _serverRegistry = value; } }

        #region private event handlers
        private void _ipField_TextChanged(object sender, EventArgs e)
        {
            if (_ipAddressTextBox.Text.Length > 0)
            {
                _okBtn.Enabled = true;
            }
            else
            {
                _okBtn.Enabled = false;
            }
        }

        private void _helpBtn_Click(object sender, EventArgs e)
        {
            DisplayHelp();
        }

        private void EnterIPForm_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            DisplayHelp();
        }

        private void EnterIPForm_Closing(object sender, CancelEventArgs e)
        {
            if (this.DialogResult == DialogResult.OK)
            {
                if (!AcceptDialog())
                {
                    e.Cancel = true;
                }
            }
        }

        private bool AcceptDialog()
        {
            _server = _ipAddressTextBox.Text;
            return ServerValidation.ValidateAndSetServer(_serverRegistry, _server, this);
        }

        private void DisplayHelp()
        {
            _serverRegistry.HelpHandler(this, new ServerHelpEventArgs(HelpContext.EnterIPForm, new HelpEventArgs(new Point()))); 
        }
        #endregion

        private String _server;
        private IServerRegistry _serverRegistry;
    }
}