using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sage.CRE.HostingFramework.LinkedSource;
using System.IO;
using System.Reflection;
using System.Globalization;

namespace ConnectorServiceMonitor.Internal
{
    internal partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
        }

        public String MachineName
        { 
            get { return _machineNameTextBox.Text; }
            set { _machineNameTextBox.Text = value; }
        }

        public Int32 PortNumber
        {
            get { return Convert.ToInt32(_portNumberTextBox.Text, CultureInfo.InvariantCulture); }
            set { _portNumberTextBox.Text = Convert.ToString(value, CultureInfo.InvariantCulture); }
        }
    }
}
