using System;
using System.Windows.Forms;
using ConnectorServiceMonitor.ViewModel;

namespace ConnectorServiceMonitor.Internal
{
    internal partial class InitializeServer : UserControl
    {
        public InitializeServer()
        {
            InitializeComponent();
            this.warningLabel.Text = String.Format(this.warningLabel.Text, Common.BriefProductName);
            this.specifyServer.ServerSuccessfullySpecified += new EventHandler(specifyServer_ServerSuccessfullySpecified);
            this.warningPictureBox.Image = this.errorImageList.Images["exclamation"];
        }

        public IServerRegistry ServerRegistry
        {
            set
            {
                _serverRegistry = value;
                specifyServer.ServerRegistry = _serverRegistry;
            }
        }

        public event EventHandler ServerSuccessfullySpecified = delegate { };

        private void specifyServer_ServerSuccessfullySpecified(object sender, EventArgs e)
        {
            ServerSuccessfullySpecified(sender, e);
        }

        private IServerRegistry _serverRegistry;
    }
}
