using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Sage.Connector.MockCloudHostApp
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ConfigurationForm : Form
    {
        private Sage.Connector.Cloud.Integration.Interfaces.WebAPI.Configuration _configuration;
        private bool _makeReadonly;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="requestPending"></param>
        public ConfigurationForm(Sage.Connector.Cloud.Integration.Interfaces.WebAPI.Configuration configuration, bool requestPending)
        {
            _configuration = configuration;
            _makeReadonly = requestPending;
            View = new ConfigParamsView(configuration);
            InitializeComponent();
        }

        /// <summary>
        /// 
        /// </summary>
        public ConfigParamsView View { get; set; }

        private void ConfigurationForm_Load(object sender, EventArgs e)
        {
            BindData();
            MakeFormReadOnly(_makeReadonly);
        }

        private void BindData()
        {
            if (this.View != null)
            {
                txtConfigurationBaseURI.DataBindings.Add(new Binding("Text", this.View, "ConfigurationBaseURI"));
                txtConfigurationResourcePath.DataBindings.Add(new Binding("Text", this.View, "ConfigurationResourcePath"));
                txtRequestBaseURI.DataBindings.Add(new Binding("Text", this.View, "RequestBaseURI"));
                txtRequestResourcePath.DataBindings.Add(new Binding("Text", this.View, "RequestResourcePath"));
                txtResponseBaseURI.DataBindings.Add(new Binding("Text", this.View, "ResponseBaseURI"));
                txtResponseResourcePath.DataBindings.Add(new Binding("Text", this.View, "ResponseResourcePath"));
                txtRequestUploadResourcePath.DataBindings.Add(new Binding("Text", this.View, "RequestUploadResourcePath"));
                txtResponseUploadResourcePath.DataBindings.Add(new Binding("Text", this.View, "ResponseUploadResourcePath"));
                txtNotificationResourceURI.DataBindings.Add(new Binding("Text", this.View, "NotificationResourceURI"));
                txtMinimumConnectorProductVersion.DataBindings.Add(new Binding("Text", this.View, "MinimumConnectorProductVersion"));
                txtUpgradeConnectorProductVersion.DataBindings.Add(new Binding("Text", this.View, "UpgradeConnectorProductVersion"));
                txtUpgradeConnectorPublicationDate.DataBindings.Add(new Binding("Text", this.View, "UpgradeConnectorProductDate"));
                txtUpgradeConnectorDescription.DataBindings.Add(new Binding("Text", this.View, "UpgradeConnectorDescription"));
                txtUpgradeConnectorLinkURI.DataBindings.Add(new Binding("Text", this.View, "UpgradeConnectorLinkURI"));
                txtTenantPublicURI.DataBindings.Add(new Binding("Text", this.View, "TenantPublicURI"));
                txtTenantName.DataBindings.Add(new Binding("Text", this.View, "TenantName"));
                txtMaximumBlobSize.DataBindings.Add(new Binding("Text", this.View, "MaximumBlobSize"));
                txtLargeResponseSizeThreshold.DataBindings.Add(new Binding("Text", this.View, "LargeResponseSizeThreshold"));
                txtSuggestedConnectorUptimeDuration.DataBindings.Add(new Binding("Text", this.View, "SuggestedConnectorUptimeDuration"));
                txtMinimumCommunicationFailureRetryInterval.DataBindings.Add(new Binding("Text", this.View, "MinimumCommFailureRetryInterval"));
                txtMaximumCommunicationFailureRetryInterval.DataBindings.Add(new Binding("Text", this.View, "MaximumCommFailureRetryInterval"));
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            List<string> errors;
            if (ValidateConfigParamSettings(out errors))
            {
                DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
            else 
            {
                string error = "Unable to continue:" + Environment.NewLine;
                foreach (string s in errors)
                    error += (Environment.NewLine + s);
                MessageBox.Show(error, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateConfigParamSettings(out List<string> errors)
        {
            bool valid = true;
            errors = new List<string>();

            return valid;
        }

        private void MakeFormReadOnly(bool makeReadOnly)
        {
            bool enabledState = !makeReadOnly;
            foreach (Control c in this.Controls)
            {
                if (c.GetType() == typeof(TextBox))
                    ((TextBox)c).Enabled = enabledState;
            }
            btnOk.Enabled = enabledState;
        }
    }
}
