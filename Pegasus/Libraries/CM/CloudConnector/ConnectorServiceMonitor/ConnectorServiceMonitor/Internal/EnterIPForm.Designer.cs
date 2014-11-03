namespace ConnectorServiceMonitor.Internal
{
    partial class EnterIPForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._ipAddressTextBox = new System.Windows.Forms.TextBox();
            this._ipAddressLabel = new System.Windows.Forms.Label();
            this._cancelBtn = new System.Windows.Forms.Button();
            this._okBtn = new System.Windows.Forms.Button();
            this.buttonPanel = new System.Windows.Forms.Panel();
            this.buttonSpacerPanel2 = new System.Windows.Forms.Panel();
            this.buttonSpacerPanel1 = new System.Windows.Forms.Panel();
            this._helpBtn = new System.Windows.Forms.Button();
            this.buttonPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // _ipAddressTextBox
            // 
            this._ipAddressTextBox.Dock = System.Windows.Forms.DockStyle.Top;
            this._ipAddressTextBox.Location = new System.Drawing.Point(10, 27);
            this._ipAddressTextBox.Name = "_ipAddressTextBox";
            this._ipAddressTextBox.Size = new System.Drawing.Size(464, 20);
            this._ipAddressTextBox.TabIndex = 0;
            this._ipAddressTextBox.TextChanged += new System.EventHandler(this._ipField_TextChanged);
            // 
            // _ipAddressLabel
            // 
            this._ipAddressLabel.AutoSize = true;
            this._ipAddressLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this._ipAddressLabel.Location = new System.Drawing.Point(10, 10);
            this._ipAddressLabel.Name = "_ipAddressLabel";
            this._ipAddressLabel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 4);
            this._ipAddressLabel.Size = new System.Drawing.Size(227, 17);
            this._ipAddressLabel.TabIndex = 1;
            this._ipAddressLabel.Text = "Enter the IP Address or name of the {0} server:";
            // 
            // _cancelBtn
            // 
            this._cancelBtn.CausesValidation = false;
            this._cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this._cancelBtn.Dock = System.Windows.Forms.DockStyle.Right;
            this._cancelBtn.Location = new System.Drawing.Point(308, 6);
            this._cancelBtn.Name = "_cancelBtn";
            this._cancelBtn.Size = new System.Drawing.Size(75, 23);
            this._cancelBtn.TabIndex = 1;
            this._cancelBtn.Text = "Cancel";
            this._cancelBtn.UseVisualStyleBackColor = true;
            // 
            // _okBtn
            // 
            this._okBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            this._okBtn.Dock = System.Windows.Forms.DockStyle.Right;
            this._okBtn.Enabled = false;
            this._okBtn.Location = new System.Drawing.Point(227, 6);
            this._okBtn.Name = "_okBtn";
            this._okBtn.Size = new System.Drawing.Size(75, 23);
            this._okBtn.TabIndex = 0;
            this._okBtn.Text = "OK";
            this._okBtn.UseVisualStyleBackColor = true;
            // 
            // buttonPanel
            // 
            this.buttonPanel.CausesValidation = false;
            this.buttonPanel.Controls.Add(this._okBtn);
            this.buttonPanel.Controls.Add(this.buttonSpacerPanel2);
            this.buttonPanel.Controls.Add(this._cancelBtn);
            this.buttonPanel.Controls.Add(this.buttonSpacerPanel1);
            this.buttonPanel.Controls.Add(this._helpBtn);
            this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.buttonPanel.Location = new System.Drawing.Point(10, 47);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Padding = new System.Windows.Forms.Padding(0, 6, 0, 0);
            this.buttonPanel.Size = new System.Drawing.Size(464, 29);
            this.buttonPanel.TabIndex = 4;
            // 
            // buttonSpacerPanel2
            // 
            this.buttonSpacerPanel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonSpacerPanel2.Location = new System.Drawing.Point(302, 6);
            this.buttonSpacerPanel2.Name = "buttonSpacerPanel2";
            this.buttonSpacerPanel2.Size = new System.Drawing.Size(6, 23);
            this.buttonSpacerPanel2.TabIndex = 3;
            // 
            // buttonSpacerPanel1
            // 
            this.buttonSpacerPanel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonSpacerPanel1.Location = new System.Drawing.Point(383, 6);
            this.buttonSpacerPanel1.Name = "buttonSpacerPanel1";
            this.buttonSpacerPanel1.Size = new System.Drawing.Size(6, 23);
            this.buttonSpacerPanel1.TabIndex = 1;
            // 
            // _helpBtn
            // 
            this._helpBtn.CausesValidation = false;
            this._helpBtn.Dock = System.Windows.Forms.DockStyle.Right;
            this._helpBtn.Location = new System.Drawing.Point(389, 6);
            this._helpBtn.Name = "_helpBtn";
            this._helpBtn.Size = new System.Drawing.Size(75, 23);
            this._helpBtn.TabIndex = 2;
            this._helpBtn.Text = "&Help";
            this._helpBtn.UseVisualStyleBackColor = true;
            this._helpBtn.Click += new System.EventHandler(this._helpBtn_Click);
            // 
            // EnterIPForm
            // 
            this.AcceptButton = this._okBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this._cancelBtn;
            this.ClientSize = new System.Drawing.Size(484, 90);
            this.Controls.Add(this.buttonPanel);
            this.Controls.Add(this._ipAddressTextBox);
            this.Controls.Add(this._ipAddressLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EnterIPForm";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Enter IP Address or Name";
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.EnterIPForm_HelpRequested);
            this.buttonPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox _ipAddressTextBox;
        private System.Windows.Forms.Label _ipAddressLabel;
        private System.Windows.Forms.Button _cancelBtn;
        private System.Windows.Forms.Button _okBtn;
        private System.Windows.Forms.Panel buttonPanel;
        private System.Windows.Forms.Button _helpBtn;
        private System.Windows.Forms.Panel buttonSpacerPanel2;
        private System.Windows.Forms.Panel buttonSpacerPanel1;


    }
}