using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ConnectorServiceMonitor.Internal;
using ConnectorServiceMonitor.ViewModel;
using Sage.Connector.Common;
using Sage.Net;

namespace ConnectorServiceMonitor
{
    /// <summary>
    /// Setting USer control for the Monitor screen
    /// </summary>
    public partial class SettingsUserControl : UserControl
    {
        /// <summary>
        /// Setting user control constructor
        /// </summary>
        public SettingsUserControl()
        {
            InitializeComponent();
            InitilizeLabels();
        }

        private void InitilizeLabels()
        {
            this.specifyServerSuccess = new ConnectorServiceMonitor.Internal.SpecifyServerSuccess();
            _sageconnecterserverlabel.Text = Strings.SettingsUserControl_InitilizeLabels_Sage_Connector_Server;
            _thiscomputerRadio.Text = Strings.SettingsUserControl_InitilizeLabels_This_Computer;
            _anotherComputerRadio.Text = Strings.SettingsUserControl_InitilizeLabels_Select_a_Computer;
            _ipAddressRadio.Text = Strings.SettingsUserControl_InitilizeLabels_Enter_IP_Address_or_Server_Name;

            _5secondsLabel.Text = Strings.SettingsUserControl_InitilizeLabels_Every_5_seconds;
            _10secondslabel.Text = Strings.SettingsUserControl_InitilizeLabels_Every_10_seconds;
            _30secondsLabel.Text = Strings.SettingsUserControl_InitilizeLabels_Every_30_seconds;
            _60secondsLabel.Text = Strings.SettingsUserControl_InitilizeLabels_Every_60_seconds;
            _2minutesLabel.Text = Strings.SettingsUserControl_InitilizeLabels_Every_2_minutes;
            _5minutesLabel.Text = Strings.SettingsUserControl_InitilizeLabels_Every_5_minutes;
            _10MinutesLabel.Text = Strings.SettingsUserControl_InitilizeLabels_Every_10_minutes;
            _neverLabel.Text = Strings.SettingsUserControl_InitilizeLabels_Never;

            _saveButton.Text = Strings.SettingsUserControl_InitilizeLabels_Save;
            _saveButton.Paint +=_saveButton_Paint;
            txtipAddress.Validating += IpAddressLabelOnValidating;
            
            // The Minutes value needs to be specified in the TAG property for the Main form to process the values
            //Use the given format in the tag property = S5 for 5 seconds and M2 for 2 minutes
            _5minuteRadio.Tag = RefreshTimeConstants.FiveMinutes;
            _10minuteRadio.Tag = RefreshTimeConstants.TenMinutes;
            _2minuteRadio.Tag = RefreshTimeConstants.TwoMinutes;
            _5secondRadio.Tag = RefreshTimeConstants.Fiveseconds;
            _10secondRadio.Tag = RefreshTimeConstants.TenSeconds;
            _30secondRadio.Tag = RefreshTimeConstants.ThirtySeconds;
            _60secondRadio.Tag = RefreshTimeConstants.SixtySeconds;
            _neverRadio.Tag = RefreshTimeConstants.None;

            _5minuteRadio.CheckedChanged += RefreshintervalOnCheckedChanged;
            _10minuteRadio.CheckedChanged += RefreshintervalOnCheckedChanged;
            _2minuteRadio.CheckedChanged += RefreshintervalOnCheckedChanged;
            _5secondRadio.CheckedChanged += RefreshintervalOnCheckedChanged;
            _10secondRadio.CheckedChanged += RefreshintervalOnCheckedChanged;
            _30secondRadio.CheckedChanged += RefreshintervalOnCheckedChanged;
            _60secondRadio.CheckedChanged += RefreshintervalOnCheckedChanged;
            _neverRadio.CheckedChanged += RefreshintervalOnCheckedChanged;
            ComboServerList.Text = Strings.SettingsUserControl_DefaultforServerSelection_List_of_Computers_on_your_network;
            
            _5secondRadio.Checked = true;

           
        }

        /// <summary>
        /// To update the Refresh interval in the Control
        /// </summary>
        /// <param name="autoRefreshInterval"></param>
        public void RestoreAutoRefreshInterval(int autoRefreshInterval)
        {
            switch (autoRefreshInterval)
            {
                case 0:
                    _5secondRadio.Checked = true;
                    break;
                case 1:
                    _10secondRadio.Checked = true;
                    break;
                case 2:
                    _30secondRadio.Checked = true;
                    break;
                case 3:
                    _60secondRadio.Checked = true;
                    break;
                case 4:
                    _2minuteRadio.Checked = true;
                    break;
                case 5:
                    _5minuteRadio.Checked = true;
                    break;
                case 6:
                    _10minuteRadio.Checked = true;
                    break;
            }
        }

        private void Updatecurrentsetting()
        {
            if (_serverRegistry.IsConfigured)
            {
                int val=0;
                if (_serverRegistry.Host== "localhost" || _serverRegistry.Host == "127.0.0.1")
                {
                    _thiscomputerRadio.Checked = true;
                    return;
                }

                RestoreAutoRefreshInterval(_serverRegistry.AutoRefreshInterval);
                if (int.TryParse(_serverRegistry.Host.Substring(0, 2), out val))
                {
                    txtipAddress.Text = _serverRegistry.Host;
                    _ipAddressRadio.Checked = true;
                }
                else
                {
                    try
                    {
                        INetworkInfo netInfo = (INetworkInfo) new NetworkInfo();
                        IServerCollection servers = netInfo.Servers;
                        servers.Populate();
                        foreach (IServer s in servers)
                        {
                            try
                            {
                                if (s.Name ==_serverRegistry.Host)
                                {
                                    ComboServerList.Text = _serverRegistry.Host;
                                    _anotherComputerRadio.Checked = true;
                                    return;
                                }
                            }
                            catch (Exception ex)
                            {
                                Trace.WriteLine(ex.ExceptionAsString());
                            }
                        }
                        txtipAddress.Text = _serverRegistry.Host;
                        _ipAddressRadio.Checked = true;
                        
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.ExceptionAsString());
                    }
                }  
            }
        }

        private void IpAddressLabelOnValidating(object sender, CancelEventArgs cancelEventArgs)
        {
            //Regex reg = new Regex("^([0-9]{1,3}\.){3}[0-9]{1,3}$");
            //bool valide = reg.IsMatch(txtportnumber.Text);
        }
        private String _server;
        private IServerRegistry _serverRegistry;
        /// <summary>
        /// 
        /// </summary>
        public IServerRegistry ServerRegistry
        {
            set
            {
                _serverRegistry = value;
                Updatecurrentsetting();
            } }

        /// <summary>
        /// 
        /// </summary>
        public String Server
        { get { return _server; } }

        #region private methods

        /// <summary>
        /// Initialize and load the server list
        /// </summary>
        private void LoadServerList()
        {
            try
            {
                this.Cursor = Cursors.WaitCursor;
                INetworkInfo netInfo = (INetworkInfo)new NetworkInfo();
                IServerCollection servers = netInfo.Servers;
                servers.Populate();

                foreach (IServer s in servers)
                {
                    try
                    {
                        ComboServerList.Items.Add(s.Name);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.ExceptionAsString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Strings.SettingsUserControl_LoadServerList_Error, Strings.SettingsUserControl_LoadServerList_Caption,
                    MessageBoxButtons.OK, MessageBoxIcon.Stop);
                Debug.WriteLine(ex);
            }
            finally
            {
                this.Cursor = Cursors.Arrow;
            }
            
        }

        #endregion
        #region EventHandlers

        /// <summary>
        /// 
        /// </summary>
        public EventHandler RefreshIntervalChanged;

        private void RefreshintervalOnCheckedChanged(object sender, EventArgs eventArgs)
        {
            if (this.RefreshIntervalChanged != null) this.RefreshIntervalChanged(sender, eventArgs);
        }
       

        //private void Enablecontrols(bool enable)
        //{
        //    ComboServerList.Enabled = enable;
        //    txtipAddress.Enabled = enable;
        //    _saveButton.Enabled = enable;

        //}
        //private void _saveButton_EnabledChanged(object sender, System.EventArgs e)
        //{
        //    //_saveButton.ForeColor = sender.enabled == false ? Color.Blue : Color.Red;
        //    //_saveButton.BackColor = Color.AliceBlue;
        //}
        private void _saveButton_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            dynamic btn = (Button)sender;
            dynamic drawBrush = new SolidBrush(btn.ForeColor);
            dynamic sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            _saveButton.Text = string.Empty;
            e.Graphics.DrawString(Strings.SettingsUserControl_InitilizeLabels_Save, btn.Font, drawBrush, e.ClipRectangle, sf);
            drawBrush.Dispose();
            sf.Dispose();

        }
        private void _thiscomputerRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (_thiscomputerRadio.Checked)
            {

            }
        }
        private void _anotherComputerRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (_anotherComputerRadio.Checked)
            {
                ComboServerList.Enabled = true;
                txtipAddress.Enabled = false;
                LoadServerList();

            }
        }
        private void _ipAddressRadio_CheckedChanged(object sender, EventArgs e)
        {
            if (_ipAddressRadio.Checked)
            {
                txtipAddress.Enabled = true;
                ComboServerList.Enabled = false;
                txtipAddress.Focus();

            }
        }
     
        #endregion

        private void _saveButton_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            if (SaveServer())
            {
                _changeSettings(this, null);
                MessageBox.Show(Strings.SettingsUserControl__saveServerChange_Connected_to_server_sucessfully,Strings.SettingsUserControl__saveButton_Click_Server_Changed, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
             this.Cursor = Cursors.Arrow;
        }


        private bool SaveServer()
        {
            if (_ipAddressRadio.Checked && txtipAddress.Text.Length >= 0)
            {
                _server = txtipAddress.Text;
                return ConnectorServiceMonitor.Internal.ServerValidation.ValidateAndSetServer(_serverRegistry, _server,
                    this);
                
            }
            if (_anotherComputerRadio.Checked && ComboServerList.SelectedIndex >= 0 )
            {
                _server = String.Format("{0}:{1}", ComboServerList.SelectedItem, Convert.ToString(_serverRegistry.DefaultCatalogServicePort));
                return ConnectorServiceMonitor.Internal.ServerValidation.ValidateAndSetServer(_serverRegistry, _server, this);
                
            }
            if (_thiscomputerRadio.Checked)
            {
                Internal.ConfigureForLocalMachine.Configure();
                _server = "localhost";
                return Internal.ServerValidation.ValidateAndSetServer(_serverRegistry, _server,
                    this);
            }
            return false;
        }

        private SpecifyServerSuccess specifyServerSuccess;
        private EventHandler _changeSettings = delegate(Object o, EventArgs e) { };
        /// <summary>
        /// Raise Change settings event once the save is completed to 
        /// </summary>
        
        public event EventHandler ChangeSettings
        {
            add { _changeSettings += value; }
            remove { _changeSettings -= value; }
        }
    }
}
