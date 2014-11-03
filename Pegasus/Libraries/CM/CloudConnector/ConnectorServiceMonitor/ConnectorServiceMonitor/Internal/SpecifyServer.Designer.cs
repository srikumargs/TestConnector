namespace ConnectorServiceMonitor.Internal
{
    partial class SpecifyServer
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
            this.enterIPAddressLinkLabel = new System.Windows.Forms.LinkLabel();
            this.orLabel = new System.Windows.Forms.Label();
            this.selectServerLinkLabel = new System.Windows.Forms.LinkLabel();
            this.specifyServerLabel = new System.Windows.Forms.Label();
            this.whatLinkLabel = new System.Windows.Forms.LinkLabel();
            this.helpLinksPanel = new System.Windows.Forms.Panel();
            this.helpLinksPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // enterIPAddressLinkLabel
            // 
            this.enterIPAddressLinkLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.enterIPAddressLinkLabel.LinkArea = new System.Windows.Forms.LinkArea(0, 50);
            this.enterIPAddressLinkLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.enterIPAddressLinkLabel.Location = new System.Drawing.Point(0, 57);
            this.enterIPAddressLinkLabel.Name = "enterIPAddressLinkLabel";
            this.enterIPAddressLinkLabel.Padding = new System.Windows.Forms.Padding(30, 4, 6, 0);
            this.enterIPAddressLinkLabel.Size = new System.Drawing.Size(402, 25);
            this.enterIPAddressLinkLabel.TabIndex = 14;
            this.enterIPAddressLinkLabel.TabStop = true;
            this.enterIPAddressLinkLabel.Text = "Enter the IP Address or name of the computer.";
            this.enterIPAddressLinkLabel.UseCompatibleTextRendering = true;
            this.enterIPAddressLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.enterIPAddressLinkLabel_LinkClicked);
            // 
            // orLabel
            // 
            this.orLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.orLabel.Location = new System.Drawing.Point(0, 40);
            this.orLabel.Name = "orLabel";
            this.orLabel.Padding = new System.Windows.Forms.Padding(30, 4, 0, 0);
            this.orLabel.Size = new System.Drawing.Size(402, 17);
            this.orLabel.TabIndex = 17;
            this.orLabel.Text = "or";
            // 
            // selectServerLinkLabel
            // 
            this.selectServerLinkLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.selectServerLinkLabel.LinkArea = new System.Windows.Forms.LinkArea(0, 64);
            this.selectServerLinkLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.selectServerLinkLabel.Location = new System.Drawing.Point(0, 21);
            this.selectServerLinkLabel.Name = "selectServerLinkLabel";
            this.selectServerLinkLabel.Padding = new System.Windows.Forms.Padding(30, 6, 0, 0);
            this.selectServerLinkLabel.Size = new System.Drawing.Size(402, 19);
            this.selectServerLinkLabel.TabIndex = 13;
            this.selectServerLinkLabel.TabStop = true;
            this.selectServerLinkLabel.Text = "Select the computer from the list of computers on your network.";
            this.selectServerLinkLabel.UseCompatibleTextRendering = true;
            this.selectServerLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.selectServerLinkLabel_LinkClicked);
            // 
            // specifyServerLabel
            // 
            this.specifyServerLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.specifyServerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.specifyServerLabel.Location = new System.Drawing.Point(0, 0);
            this.specifyServerLabel.Name = "specifyServerLabel";
            this.specifyServerLabel.Padding = new System.Windows.Forms.Padding(20, 6, 0, 0);
            this.specifyServerLabel.Size = new System.Drawing.Size(402, 21);
            this.specifyServerLabel.TabIndex = 15;
            this.specifyServerLabel.Text = "To specify this computer:";
            // 
            // whatLinkLabel
            // 
            this.whatLinkLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.whatLinkLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.whatLinkLabel.Location = new System.Drawing.Point(6, 6);
            this.whatLinkLabel.Name = "whatLinkLabel";
            this.whatLinkLabel.Padding = new System.Windows.Forms.Padding(0, 4, 0, 0);
            this.whatLinkLabel.Size = new System.Drawing.Size(390, 17);
            this.whatLinkLabel.TabIndex = 18;
            this.whatLinkLabel.TabStop = true;
            this.whatLinkLabel.Text = "What is the {0} server?";
            this.whatLinkLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.whatLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.whatLinkLabel_LinkClicked);
            // 
            // helpLinksPanel
            // 
            this.helpLinksPanel.Controls.Add(this.whatLinkLabel);
            this.helpLinksPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.helpLinksPanel.Location = new System.Drawing.Point(0, 82);
            this.helpLinksPanel.Name = "helpLinksPanel";
            this.helpLinksPanel.Padding = new System.Windows.Forms.Padding(6);
            this.helpLinksPanel.Size = new System.Drawing.Size(402, 52);
            this.helpLinksPanel.TabIndex = 21;
            // 
            // SpecifyServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.helpLinksPanel);
            this.Controls.Add(this.enterIPAddressLinkLabel);
            this.Controls.Add(this.orLabel);
            this.Controls.Add(this.selectServerLinkLabel);
            this.Controls.Add(this.specifyServerLabel);
            this.Name = "SpecifyServer";
            this.Size = new System.Drawing.Size(402, 134);
            this.Load += new System.EventHandler(this.SpecifyServer_Load);
            this.helpLinksPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.LinkLabel enterIPAddressLinkLabel;
        private System.Windows.Forms.Label orLabel;
        private System.Windows.Forms.LinkLabel selectServerLinkLabel;
        private System.Windows.Forms.Label specifyServerLabel;
        private System.Windows.Forms.LinkLabel whatLinkLabel;
        private System.Windows.Forms.Panel helpLinksPanel;
    }
}
