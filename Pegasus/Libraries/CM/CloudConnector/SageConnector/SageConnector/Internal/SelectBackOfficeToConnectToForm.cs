using System;
using System.Windows.Forms;
using SageConnector.ViewModel;

namespace SageConnector.Internal
{
    internal partial class SelectBackOfficeToConnectToForm : Form
    {
        public SelectBackOfficeToConnectToForm()
        {
            InitializeComponent();
        }

        public SelectBackOfficeToConnectToForm(ConnectorPluginsCollection plugins)
        {
            _plugins = plugins;
            InitializeComponent();
        }

        private void SelectBackOfficeToConnectToForm_Load(object sender, EventArgs e)
        {
            foreach (var plugin in _plugins)
            {
                _comboBox.Items.Add(plugin.PluggedInProductName);
            }

            _comboBox.SelectedIndex = 0;
        }


        public ConnectorPlugin SelectedConnectorPlugin { get { return _plugins[_comboBox.SelectedIndex]; } }

        private ConnectorPluginsCollection _plugins;
    }
}
