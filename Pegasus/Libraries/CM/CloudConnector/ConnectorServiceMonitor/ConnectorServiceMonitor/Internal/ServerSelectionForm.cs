using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Sage.Activation;
using System.Drawing;
using ConnectorServiceMonitor.ViewModel;

namespace ConnectorServiceMonitor.Internal
{
    /// <summary>
    /// The type of modes the Server Selection Wizard can be started in
    /// </summary>
    internal enum ServerSelectionMode
    {
        /// <summary>
        /// Start up in initialize server mode
        /// </summary>
        InitializeServer,

        /// <summary>
        /// Startup in Change Server Mode
        /// </summary>
        ChangeServer
    }

    /// <summary>
    /// Class used to allow the user to identify a server
    /// </summary>
    internal partial class ServerSelectionForm : Form
    {
        #region Fields
        /// <summary>
        /// The mode the wizard should be run in.
        /// </summary>
        private ServerSelectionMode _mode = ServerSelectionMode.InitializeServer;
        private IServerRegistry _serverRegistry = null;

        #endregion

        #region Construction

        /// <summary>
        /// Constructor
        /// </summary>
        public ServerSelectionForm()
        {
            InitializeComponent();
            this.Text = String.Format(this.Text, Common.BriefProductName);
            this.initializeServer.ServerSuccessfullySpecified += new EventHandler(OnServerSuccessfullySpecified);
            this.changeServer.ServerSuccessfullySpecified += new EventHandler(OnServerSuccessfullySpecified);
        }

        #endregion

        #region Public Properties
        /// <summary>
        /// The mode the wizard should be run in.
        /// </summary>
        public ServerSelectionMode Mode
        {
            set { _mode = value; }
        }

        public IServerRegistry ServerRegistry
        {
            set {
                _serverRegistry = value;
                initializeServer.ServerRegistry = _serverRegistry;
                changeServer.ServerRegistry = _serverRegistry;
            }
        }
        #endregion

        #region private event handlers
        /// <summary>
        /// Initialize the form
        /// </summary>
        /// <param name="sender">Event source</param>
        /// <param name="e">Event args</param>
        private void ServerSelectionWizard_Load(object sender, EventArgs e)
        {
            Application.EnableVisualStyles();

            if (_mode == ServerSelectionMode.InitializeServer)
            {
                panelStack.ActivePanel = this.initializeServerPanel;
            }
            else
            {
                panelStack.ActivePanel = this.changeServerPanel;
                this.Text = Common.ReplaceKnownTerms(Strings.CHANGE_SERVER_TITLE);
            }
        }

        private void OnServerSuccessfullySpecified(object sender, EventArgs e)
        {
            StringEventArgs stringEventArgs = e as StringEventArgs;

            String host = stringEventArgs.Value;
            Int32 port = _serverRegistry.DefaultCatalogServicePort;
            if (host.Contains(":"))
            {
                port = Convert.ToInt32(host.Substring(host.LastIndexOf(':') + 1, host.Length - host.LastIndexOf(':') - 1));
                host = host.Substring(0, host.LastIndexOf(':'));
            }

            if (port != _serverRegistry.DefaultCatalogServicePort)
            {
                this.specifyServerSuccess.SetServerName(String.Format("{0} (port {1})", host, port));
            }
            else
            {
                this.specifyServerSuccess.SetServerName(host);
            }


            this.panelStack.ActivePanel = this.successPanel;
            this.closeButton.Text = "&OK";
            this.helpButton.Visible = false;
            this.buttonSpacerPanel.Visible = false;
        }
        
        private void closeButton_Click(object sender, EventArgs e)
        {
            if (panelStack.ActivePanel == this.successPanel)
            {
                DialogResult = DialogResult.OK;
            }
            else
            {
                DialogResult = DialogResult.Cancel;
            }
        }

        private void helpButton_Click(object sender, EventArgs e)
        {
            DisplayHelp();
        }

        private void ServerSelection_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            DisplayHelp();
        }
        #endregion

        #region private methods
        private void DisplayHelp()
        {
            if (_mode == ServerSelectionMode.InitializeServer)
            {
                _serverRegistry.HelpHandler(this, new ServerHelpEventArgs(HelpContext.ServerSelectionForm_Initialize, new HelpEventArgs(new Point()))); 
            }
            else
            {
                _serverRegistry.HelpHandler(this, new ServerHelpEventArgs(HelpContext.ServerSelectionForm_Change, new HelpEventArgs(new Point())));
            }
        }
        #endregion
    }
}