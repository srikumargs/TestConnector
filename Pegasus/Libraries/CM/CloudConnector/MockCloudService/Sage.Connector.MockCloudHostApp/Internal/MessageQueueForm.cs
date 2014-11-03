using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Sage.Connector.MockCloudHostApp.Internal
{
    /// <summary>
    /// Show a mock internal message queue
    /// </summary>
    public partial class MessageQueueForm : Form
    {
        private readonly IEnumerable<object> _items;
        
        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="items"></param>
        public MessageQueueForm(IEnumerable<object> items)
        {
            InitializeComponent();
            _items= items;
        }

        private void MessageQueueForm_Load(object sender, EventArgs e)
        {
            Object[] objs = _items.ToArray();
            if (objs.Length > 0)
            {
                lstItems.Items.AddRange(objs);

                lstItems.SelectedIndex = lstItems.Items.Count -1 ;                
            }
        }

        private void SetItemDetails(object obj)
        {
            // Bump up the display to 256k to help the QA teams inspect their data.
            string display = (obj == null ? string.Empty : GetAsJsonString(obj));
            string trimed = display.Substring(0, Math.Min(256000, display.Length));
            txtItemDetails.Text = trimed;
        }

        private string GetAsJsonString(object obj)
        {
            return JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lstItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            object obj = lstItems.SelectedItem;
            SetItemDetails(obj);
        }
    }
}
