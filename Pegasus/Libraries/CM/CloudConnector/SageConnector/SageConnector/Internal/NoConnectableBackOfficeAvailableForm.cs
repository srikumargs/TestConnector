using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;
using SageConnector.ViewModel;

namespace SageConnector.Internal
{
    internal partial class NoConnectableBackOfficeAvailableForm : Form
    {
        public NoConnectableBackOfficeAvailableForm()
        {
            InitializeComponent();
        }

        private void NoBackOfficeAvailableForm_Load(object sender, EventArgs e)
        {
            this.Text = String.Format(this.Text, ConnectorViewModel.ProductName);
            label1.Text = String.Format(label1.Text, ConnectorViewModel.ProductName, FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion);
            linkLabel1.Text = "See System Requirements in Help for more information.";
            linkLabel1.Links.Add("See ".Length, "System Requirements".Length, ConnectorViewModel.SystemRequiresmentsHelpUri);
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(e.Link.LinkData.ToString());
        }
    }
}
