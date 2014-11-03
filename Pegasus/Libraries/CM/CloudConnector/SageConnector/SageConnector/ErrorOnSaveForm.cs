using System;
using System.Windows.Forms;
using SageConnector.Properties;

namespace SageConnector
{
    internal partial class ErrorOnSaveForm : Form
    {
        private ErrorOnSaveForm() { }

        public ErrorOnSaveForm(string message)
        {
            InitializeComponent();
            InitializeContent(message);
        }

        private void InitializeContent(string message)
        {
            // Set the content of the error dialog
            _originalLabelHeight = label1.Height;
            label1.Text = message;
        }

        private void ErrorOnSaveForm_Load(object sender, EventArgs e)
        {
            this.Text = Resources.ConnectorDetails_ConnectionIssues_Caption;
            this.Height += label1.Height - _originalLabelHeight;
        }

        private Int32 _originalLabelHeight;
    }
}
