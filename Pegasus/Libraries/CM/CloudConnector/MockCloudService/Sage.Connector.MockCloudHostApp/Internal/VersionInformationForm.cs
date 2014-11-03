using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sage.Connector.SageCloudService;

namespace Sage.Connector.MockCloudHostApp
{
    /// <summary>
    /// 
    /// </summary>
    public partial class VersionInformationForm : Form
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="upgradeView"></param>
        public VersionInformationForm(UpgradeView upgradeView)
        {
            InitializeComponent();

            InitializeFields(upgradeView);
        }

        private void InitializeFields(UpgradeView uv)
        {
            txtCurInterfaceVersion.Text = uv.CurrentInterfaceVersion;
            txtCurProductVersion.Text = uv.CurrentProductVersion;
            txtMinInterfaceVersion.Text = uv.MinInterfaceVersion;
            txtMinProductVersion.Text = uv.MinProductVersion;
            txtRequiredDesc.Text = uv.UpgradeRequiredDescription;
            txtRequiredLink.Text = uv.UpgradeRequiredLink;
            txtAvailableDesc.Text = uv.UpgradeAvailableDescription;
            txtAvailableLink.Text = uv.UpgradeAvailableLink;
        }
        /// <summary>
        /// 
        /// </summary>
        private string AvailableLink
        { get { return txtAvailableLink.Text; } }
        /// <summary>
        /// 
        /// </summary>
        private string AvailableDescription
        { get { return txtAvailableDesc.Text; } }
        /// <summary>
        /// 
        /// </summary>
        private string RequiredLink
        { get { return txtRequiredLink.Text; } }
        /// <summary>
        /// 
        /// </summary>
        private string RequiredDescription
        { get { return txtRequiredDesc.Text; } }
        /// <summary>
        /// 
        /// </summary>
        private string MinInterfaceVersion
        { get { return txtMinInterfaceVersion.Text; } }
        /// <summary>
        /// 
        /// </summary>
        private string CurrentInterfaceVersion
        { get { return txtCurInterfaceVersion.Text; } }
        /// <summary>
        /// 
        /// </summary>
        private string MinProductVersion
        { get { return txtMinProductVersion.Text; } }
        /// <summary>
        /// 
        /// </summary>
        private string CurrentProductVersion
        { get { return txtCurProductVersion.Text; } }

        private void btnOk_Click(object sender, EventArgs e)
        {
            Uri link1, link2;
            bool link1Ok = string.IsNullOrEmpty(AvailableLink) || Uri.TryCreate(AvailableLink, UriKind.Absolute, out link1);
            bool link2Ok = string.IsNullOrEmpty(RequiredLink) || Uri.TryCreate(RequiredLink, UriKind.Absolute, out link2);

            if ( link1Ok && link2Ok )
            {
                DialogResult = System.Windows.Forms.DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Link provided is not well formed", "Error:", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        public UpgradeView VersionInformation
        {
            get
            {
                return new UpgradeView(
                    CurrentProductVersion,
                    MinProductVersion,
                    CurrentInterfaceVersion,
                    MinInterfaceVersion,
                    AvailableDescription,
                    RequiredDescription,
                    AvailableLink,
                    RequiredLink,
                    DateTime.Now,
                    DateTime.Now
                    );
            }
        }
    }
}
