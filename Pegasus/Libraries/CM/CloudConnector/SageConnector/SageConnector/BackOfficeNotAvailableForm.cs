using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using SageConnector.ViewModel;

namespace SageConnector
{
    internal partial class BackOfficeNotAvailableForm : Form
    {
        public BackOfficeNotAvailableForm()
        {
            InitializeComponent();
        }

        public BackOfficeNotAvailableForm(ConnectorPlugin plugin)
        {
            _plugin = plugin;
            InitializeComponent();
        }

        private void BackOfficeNotAvailableForm_Load(object sender, EventArgs e)
        {
            this.Text = String.Format(this.Text, ConnectorViewModel.ProductName);
            label1.Text = String.Format(label1.Text, ConnectorViewModel.ProductName, _plugin != null ? _plugin.PluggedInProductName : ("the " + ConnectorViewModel.BackOfficeProductTermLowercase), FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion);
            linkLabel1.Text = "See System Requirements in Help for more information.";
            linkLabel1.Links.Add("See ".Length, "System Requirements".Length, ConnectorViewModel.SystemRequiresmentsHelpUri);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(e.Link.LinkData.ToString());
        }

        private ConnectorPlugin _plugin;
    }
}
