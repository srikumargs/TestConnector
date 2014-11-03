using System;
using System.Diagnostics;
using System.Windows.Forms;
using Sage.Connector.StateService.Interfaces.DataContracts;
using SageConnector.Properties;

namespace SageConnector
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ConnectorUpdateForm : Form
    {
        private readonly ConnectorState _connectorState;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        public ConnectorUpdateForm(ConnectorState state)
        {
            _connectorState = state;

            InitializeComponent();

            bool linkFound = false;
            UpdateInfo ui = _connectorState.ConnectorUpdateInfo;
            if (ui != null && ui.UpdateLinkUri != null)
            {
                linkFound = true;
            }
            lnkUpdate.Visible = linkFound;

            string rawMessage;
            string caption;
            if (state.ConnectorUpdateStatus == ConnectorUpdateStatus.UpdateAvailable)
            {
                rawMessage = Resources.ConnectorUpdateFormUpdateAvailableNoVersion_Message;
                caption = Resources.ConnectorSimpleConnectorUpdateAvailable_Caption;
            }
            else 
            {
                rawMessage = Resources.ConnectorUpdateFormUpdateRequiredNoVersion_Message;
                caption = Resources.ConnectorSimpleConnectorUpdateRequired_Caption;
            }

            this.Text = caption;


            string s0ProductVersion = ui.ProductVersion;
            string s1InterfaceVersion = string.Empty; //ui.InterfaceVersion;
            string s2PublicationDate = ui.PublicationDate.ToShortDateString();
            string s3UpdateDescription = ui.UpdateDescription;
            string s4UpdateUri = ui.UpdateLinkUri.ToString();
            string formattedMessage = String.Format(rawMessage,
                    s0ProductVersion,
                    s1InterfaceVersion, 
                    s2PublicationDate, 
                    s3UpdateDescription,
                    s4UpdateUri,
                    Environment.NewLine);
            textMessage.Text = formattedMessage;
        }

        private void lnkUpdate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            String url = _connectorState.ConnectorUpdateInfo.UpdateLinkUri.ToString();
            ProcessStartInfo sInfo = new ProcessStartInfo(url);
            Process.Start(sInfo);
        }

    }
}
