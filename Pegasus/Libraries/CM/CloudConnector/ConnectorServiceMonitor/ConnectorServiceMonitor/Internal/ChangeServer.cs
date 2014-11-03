using System;
using System.Windows.Forms;
using ConnectorServiceMonitor.ViewModel;

namespace ConnectorServiceMonitor.Internal
{
    internal partial class ChangeServer : UserControl
    {
        public ChangeServer()
        {
            InitializeComponent();
            this.currentServerLabel.Text = Common.ReplaceKnownTerms(Strings.SERVER);
            this.specifyServer.ServerSuccessfullySpecified += new EventHandler(specifyServer_ServerSuccessfullySpecified);
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

        private void ChangeServer_Load(Object sender, EventArgs e)
        {
            if (_serverRegistry.CatalogServicePort != _serverRegistry.DefaultCatalogServicePort)
            {
                this.currentServerLabel.Text = String.Format(Common.ReplaceKnownTerms(Strings.SERVER), String.Format("{0} (port {1})", _serverRegistry.Host, _serverRegistry.CatalogServicePort));
            }
            else
            {
                this.currentServerLabel.Text = String.Format(Common.ReplaceKnownTerms(Strings.SERVER), _serverRegistry.Host);
            }
        }

        private void specifyServer_ServerSuccessfullySpecified(Object sender, EventArgs e)
        {
            ServerSuccessfullySpecified(sender, e);
        }

        private IServerRegistry _serverRegistry;
    }
}
