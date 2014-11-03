namespace Sage.Connector.MockCloudHostApp
{
    partial class TenantPanel
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.grpTenant = new System.Windows.Forms.GroupBox();
            this.btnOutbox = new System.Windows.Forms.Button();
            this.btnInbox = new System.Windows.Forms.Button();
            this.chkDisableTenant = new System.Windows.Forms.CheckBox();
            this._countTextBox = new System.Windows.Forms.TextBox();
            this.btnRequestConfiguration = new System.Windows.Forms.Button();
            this.lblErrors = new System.Windows.Forms.Label();
            this.txtErrorCount = new System.Windows.Forms.TextBox();
            this.txtRequestCount = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtSiteId = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtConnectionKey = new System.Windows.Forms.TextBox();
            this.txtOutboxCount = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtInboxCount = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cboAction = new System.Windows.Forms.ComboBox();
            this.btnInvoke = new System.Windows.Forms.Button();
            this.grpTenant.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpTenant
            // 
            this.grpTenant.AutoSize = true;
            this.grpTenant.Controls.Add(this.btnOutbox);
            this.grpTenant.Controls.Add(this.btnInbox);
            this.grpTenant.Controls.Add(this.chkDisableTenant);
            this.grpTenant.Controls.Add(this._countTextBox);
            this.grpTenant.Controls.Add(this.btnRequestConfiguration);
            this.grpTenant.Controls.Add(this.lblErrors);
            this.grpTenant.Controls.Add(this.txtErrorCount);
            this.grpTenant.Controls.Add(this.txtRequestCount);
            this.grpTenant.Controls.Add(this.label9);
            this.grpTenant.Controls.Add(this.txtSiteId);
            this.grpTenant.Controls.Add(this.label8);
            this.grpTenant.Controls.Add(this.txtConnectionKey);
            this.grpTenant.Controls.Add(this.txtOutboxCount);
            this.grpTenant.Controls.Add(this.label3);
            this.grpTenant.Controls.Add(this.txtInboxCount);
            this.grpTenant.Controls.Add(this.label4);
            this.grpTenant.Controls.Add(this.cboAction);
            this.grpTenant.Controls.Add(this.btnInvoke);
            this.grpTenant.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpTenant.Location = new System.Drawing.Point(10, 10);
            this.grpTenant.Name = "grpTenant";
            this.grpTenant.Size = new System.Drawing.Size(899, 109);
            this.grpTenant.TabIndex = 0;
            this.grpTenant.TabStop = false;
            this.grpTenant.Text = "Site #{0}";
            // 
            // btnOutbox
            // 
            this.btnOutbox.Location = new System.Drawing.Point(817, 43);
            this.btnOutbox.Name = "btnOutbox";
            this.btnOutbox.Size = new System.Drawing.Size(73, 23);
            this.btnOutbox.TabIndex = 40;
            this.btnOutbox.Text = "Outbox";
            this.btnOutbox.UseVisualStyleBackColor = true;
            this.btnOutbox.Click += new System.EventHandler(this.btnOutbox_Click);
            // 
            // btnInbox
            // 
            this.btnInbox.Location = new System.Drawing.Point(817, 17);
            this.btnInbox.Name = "btnInbox";
            this.btnInbox.Size = new System.Drawing.Size(73, 23);
            this.btnInbox.TabIndex = 39;
            this.btnInbox.Text = "Inbox";
            this.btnInbox.UseVisualStyleBackColor = true;
            this.btnInbox.Click += new System.EventHandler(this.btnInbox_Click);
            // 
            // chkDisableTenant
            // 
            this.chkDisableTenant.AutoSize = true;
            this.chkDisableTenant.Location = new System.Drawing.Point(418, 47);
            this.chkDisableTenant.Name = "chkDisableTenant";
            this.chkDisableTenant.Size = new System.Drawing.Size(61, 17);
            this.chkDisableTenant.TabIndex = 38;
            this.chkDisableTenant.Text = "Disable";
            this.chkDisableTenant.UseVisualStyleBackColor = true;
            this.chkDisableTenant.CheckedChanged += new System.EventHandler(this.chkDisableTenant_CheckedChanged);
            // 
            // _countTextBox
            // 
            this._countTextBox.Location = new System.Drawing.Point(624, 45);
            this._countTextBox.Name = "_countTextBox";
            this._countTextBox.Size = new System.Drawing.Size(42, 20);
            this._countTextBox.TabIndex = 28;
            this._countTextBox.Text = "1";
            this._countTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnRequestConfiguration
            // 
            this.btnRequestConfiguration.AutoSize = true;
            this.btnRequestConfiguration.Location = new System.Drawing.Point(418, 17);
            this.btnRequestConfiguration.Name = "btnRequestConfiguration";
            this.btnRequestConfiguration.Size = new System.Drawing.Size(121, 23);
            this.btnRequestConfiguration.TabIndex = 35;
            this.btnRequestConfiguration.Text = "Change/View Config";
            this.btnRequestConfiguration.UseVisualStyleBackColor = true;
            this.btnRequestConfiguration.Click += new System.EventHandler(this.btnRequestConfiguration_Click);
            // 
            // lblErrors
            // 
            this.lblErrors.AutoSize = true;
            this.lblErrors.Location = new System.Drawing.Point(719, 72);
            this.lblErrors.Name = "lblErrors";
            this.lblErrors.Size = new System.Drawing.Size(37, 13);
            this.lblErrors.TabIndex = 36;
            this.lblErrors.Text = "Errors:";
            // 
            // txtErrorCount
            // 
            this.txtErrorCount.ForeColor = System.Drawing.Color.Maroon;
            this.txtErrorCount.Location = new System.Drawing.Point(769, 69);
            this.txtErrorCount.Name = "txtErrorCount";
            this.txtErrorCount.ReadOnly = true;
            this.txtErrorCount.Size = new System.Drawing.Size(42, 20);
            this.txtErrorCount.TabIndex = 35;
            this.txtErrorCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtRequestCount
            // 
            this.txtRequestCount.Location = new System.Drawing.Point(672, 45);
            this.txtRequestCount.Name = "txtRequestCount";
            this.txtRequestCount.ReadOnly = true;
            this.txtRequestCount.Size = new System.Drawing.Size(42, 20);
            this.txtRequestCount.TabIndex = 34;
            this.txtRequestCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(719, 48);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(44, 13);
            this.label9.TabIndex = 33;
            this.label9.Text = "Outbox:";
            // 
            // txtSiteId
            // 
            this.txtSiteId.Location = new System.Drawing.Point(115, 19);
            this.txtSiteId.Name = "txtSiteId";
            this.txtSiteId.ReadOnly = true;
            this.txtSiteId.Size = new System.Drawing.Size(297, 20);
            this.txtSiteId.TabIndex = 24;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(719, 22);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(36, 13);
            this.label8.TabIndex = 32;
            this.label8.Text = "Inbox:";
            // 
            // txtConnectionKey
            // 
            this.txtConnectionKey.Location = new System.Drawing.Point(115, 45);
            this.txtConnectionKey.Multiline = true;
            this.txtConnectionKey.Name = "txtConnectionKey";
            this.txtConnectionKey.ReadOnly = true;
            this.txtConnectionKey.Size = new System.Drawing.Size(297, 45);
            this.txtConnectionKey.TabIndex = 25;
            this.txtConnectionKey.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.txtConnectionKey_MouseDoubleClick);
            // 
            // txtOutboxCount
            // 
            this.txtOutboxCount.Location = new System.Drawing.Point(769, 45);
            this.txtOutboxCount.Name = "txtOutboxCount";
            this.txtOutboxCount.ReadOnly = true;
            this.txtOutboxCount.Size = new System.Drawing.Size(42, 20);
            this.txtOutboxCount.TabIndex = 31;
            this.txtOutboxCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(42, 13);
            this.label3.TabIndex = 26;
            this.label3.Text = "Site ID:";
            // 
            // txtInboxCount
            // 
            this.txtInboxCount.Location = new System.Drawing.Point(769, 19);
            this.txtInboxCount.Name = "txtInboxCount";
            this.txtInboxCount.ReadOnly = true;
            this.txtInboxCount.Size = new System.Drawing.Size(42, 20);
            this.txtInboxCount.TabIndex = 30;
            this.txtInboxCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 48);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 13);
            this.label4.TabIndex = 27;
            this.label4.Text = "Connection key:";
            // 
            // cboAction
            // 
            this.cboAction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAction.DropDownWidth = 250;
            this.cboAction.FormattingEnabled = true;
            this.cboAction.Location = new System.Drawing.Point(545, 19);
            this.cboAction.Name = "cboAction";
            this.cboAction.Size = new System.Drawing.Size(168, 21);
            this.cboAction.TabIndex = 27;
            this.cboAction.SelectedIndexChanged += new System.EventHandler(this._site1ActionComboBox_SelectedIndexChanged);
            // 
            // btnInvoke
            // 
            this.btnInvoke.AutoSize = true;
            this.btnInvoke.Location = new System.Drawing.Point(545, 43);
            this.btnInvoke.Name = "btnInvoke";
            this.btnInvoke.Size = new System.Drawing.Size(73, 23);
            this.btnInvoke.TabIndex = 29;
            this.btnInvoke.Text = "Invoke";
            this.btnInvoke.UseVisualStyleBackColor = true;
            this.btnInvoke.Click += new System.EventHandler(this._site1InvokeButton_Click);
            // 
            // TenantPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.grpTenant);
            this.Name = "TenantPanel";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.Size = new System.Drawing.Size(919, 129);
            this.Load += new System.EventHandler(this.TenantPanel_Load);
            this.grpTenant.ResumeLayout(false);
            this.grpTenant.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox grpTenant;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtSiteId;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtConnectionKey;
        private System.Windows.Forms.TextBox txtOutboxCount;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtInboxCount;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cboAction;
        private System.Windows.Forms.Button btnInvoke;
        private System.Windows.Forms.TextBox txtRequestCount;
        private System.Windows.Forms.Button btnRequestConfiguration;
        private System.Windows.Forms.Label lblErrors;
        private System.Windows.Forms.TextBox txtErrorCount;
        private System.Windows.Forms.TextBox _countTextBox;
        private System.Windows.Forms.CheckBox chkDisableTenant;
        private System.Windows.Forms.Button btnInbox;
        private System.Windows.Forms.Button btnOutbox;
    }
}
