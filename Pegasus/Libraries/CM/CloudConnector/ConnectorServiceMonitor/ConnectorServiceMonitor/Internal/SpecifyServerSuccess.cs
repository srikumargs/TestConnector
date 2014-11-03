using System;
using System.Windows.Forms;
using ConnectorServiceMonitor.ViewModel;

namespace ConnectorServiceMonitor.Internal
{
    internal partial class SpecifyServerSuccess : UserControl
    {
        public SpecifyServerSuccess()
        {
            InitializeComponent();
        }

        public void SetServerName(String serverName)
        {
            this.successLabel.Text = string.Format(Common.ReplaceKnownTerms(Strings.SERVER), serverName);
        }
    }
}
