namespace SageConnector
{
    partial class CloudConnectorAdminLoginForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CloudConnectorAdminLoginForm));
            this.txtMarker = new System.Windows.Forms.TextBox();
            this.lblMarker = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblPasswordIncorrectMessage = new System.Windows.Forms.Label();
            this.textBoxLoginInstructions = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtMarker
            // 
            this.txtMarker.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMarker.Location = new System.Drawing.Point(98, 43);
            this.txtMarker.Name = "txtMarker";
            this.txtMarker.Size = new System.Drawing.Size(314, 20);
            this.txtMarker.TabIndex = 12;
            this.txtMarker.Visible = false;
            // 
            // lblMarker
            // 
            this.lblMarker.AutoSize = true;
            this.lblMarker.Location = new System.Drawing.Point(12, 46);
            this.lblMarker.Name = "lblMarker";
            this.lblMarker.Size = new System.Drawing.Size(82, 13);
            this.lblMarker.TabIndex = 11;
            this.lblMarker.Text = "Marker text ww:";
            this.lblMarker.Visible = false;
            this.lblMarker.Click += new System.EventHandler(this.lblMarker_Click);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(256, 350);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 20;
            this.btnOk.Text = "&OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(337, 350);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 22;
            this.btnCancel.Text = "&Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblPasswordIncorrectMessage
            // 
            this.lblPasswordIncorrectMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblPasswordIncorrectMessage.AutoSize = true;
            this.lblPasswordIncorrectMessage.ForeColor = System.Drawing.Color.Red;
            this.lblPasswordIncorrectMessage.Location = new System.Drawing.Point(12, 355);
            this.lblPasswordIncorrectMessage.Name = "lblPasswordIncorrectMessage";
            this.lblPasswordIncorrectMessage.Size = new System.Drawing.Size(230, 13);
            this.lblPasswordIncorrectMessage.TabIndex = 23;
            this.lblPasswordIncorrectMessage.Text = "Incorrect username and password combination.";
            this.lblPasswordIncorrectMessage.Visible = false;
            // 
            // textBoxLoginInstructions
            // 
            this.textBoxLoginInstructions.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxLoginInstructions.Location = new System.Drawing.Point(99, 7);
            this.textBoxLoginInstructions.Multiline = true;
            this.textBoxLoginInstructions.Name = "textBoxLoginInstructions";
            this.textBoxLoginInstructions.ReadOnly = true;
            this.textBoxLoginInstructions.Size = new System.Drawing.Size(314, 30);
            this.textBoxLoginInstructions.TabIndex = 24;
            this.textBoxLoginInstructions.TabStop = false;
            this.textBoxLoginInstructions.Text = "Type the User ID and Password for the Sage Timberline Office Application Administ" +
    "rator.";
            // 
            // CloudConnectorAdminLoginForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(424, 384);
            this.Controls.Add(this.textBoxLoginInstructions);
            this.Controls.Add(this.lblPasswordIncorrectMessage);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.txtMarker);
            this.Controls.Add(this.lblMarker);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(880, 500);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(440, 168);
            this.Name = "CloudConnectorAdminLoginForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Administrator Login";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtMarker;
        private System.Windows.Forms.Label lblMarker;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblPasswordIncorrectMessage;
        private System.Windows.Forms.TextBox textBoxLoginInstructions;
    }
}