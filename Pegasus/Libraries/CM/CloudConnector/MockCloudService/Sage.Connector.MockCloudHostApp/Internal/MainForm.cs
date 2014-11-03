using System;
using System.Linq;
using System.Windows.Forms;
using Sage.Connector.MockCloudHostApp;
using Sage.Connector.MockCloudHostApp.Internal;
using Sage.Connector.SageCloudService;

namespace Sage.Connector.SageCloudService
{
    /// <summary>
    /// 
    /// </summary>
    internal partial class Form1 : Form
    {
        /// <summary>
        /// 
        /// </summary>
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MockCloudServiceHost.StartService();
            txtSiteAddress.Text = MockCloudServiceHost.SiteAddress;
            txtNotificationAddress.Text = MockCloudServiceHost.DisplayNotificationAddress;
            txtWebAPIAddress.Text = MockCloudServiceHost.DisplayWebAPIAddress;

            AddTenantPanels();
            _timer.Enabled = true;
        }

        private void AddTenantPanels()
        {
            for (Int32 i = MockCloudService.TenantIds.Count() - 1; i >= 0; i--)
            {
                AddTenantPanel(i);
            }
        }

        private void AddTenantPanel(int i)
        {
            TenantPanel panel = null;
            try
            {
                panel = new TenantPanel(i, new ActionInvoker());
                //panel = new TenantPanel();
                //panel.Initialize(i);
                
                panel.Dock = DockStyle.Top;
                panel1.Controls.Add(panel);
                panel = null;
            }
            catch (Exception)
            {

            }
            finally
            {
                if (panel != null)
                    panel.Dispose();
            }
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            MethodInvoker i = delegate()
            {
                foreach (var panel in panel1.Controls)
                {
                    TenantPanel tPanel = panel as TenantPanel;
                    if(tPanel.Enabled)
                        tPanel.UpdateCounts();
                }
            };


            if (this.InvokeRequired)
            {
                this.BeginInvoke(i);
            }
            else
            {
                i();
            }
        }

        private void btnVersionInfo_Click(object sender, EventArgs e)
        {
            ShowVersionInfoDialog();
        }

        private void ShowVersionInfoDialog()
        {
            using (var form = new VersionInformationForm(MockCloudService.RetrieveVersionInformation()))
            {
                if (form.ShowDialog(Parent) == DialogResult.OK)
                {
                    MockCloudService.SetVersionInformation(form.VersionInformation);
                }
            }
        }

        private void txtSiteAddress_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            txtSiteAddress.SelectAll();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _timer.Enabled = false;
            foreach (TenantPanel tp in panel1.Controls)
            {
                if (tp.AutomationRunning) tp.StopAutomation();
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            RemoveTenantPremiseConfigurations();
            MockCloudServiceHost.StopService();
        }

        private void RemoveTenantPremiseConfigurations()
        {
            foreach (Control c in panel1.Controls)
            {
                if (c.GetType() == typeof(TenantPanel))
                {
                    TenantPanel tenant = (TenantPanel)c;
                    if (tenant.AutomatedPremise)
                    {
                        Sage.Connector.Utilities.ConfigurationSettingFactory.DeleteTenant(tenant.TenantId);
                    }
                }
            }
        }
    }
}
