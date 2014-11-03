namespace ConnectorServiceMonitor.Internal
{
    partial class InitializeServer
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InitializeServer));
            this.warningLabel = new System.Windows.Forms.Label();
            this.warningPictureBox = new System.Windows.Forms.PictureBox();
            this.warningPanel = new System.Windows.Forms.Panel();
            this.errorImageList = new System.Windows.Forms.ImageList(this.components);
            this.specifyServer = new ConnectorServiceMonitor.Internal.SpecifyServer();
            ((System.ComponentModel.ISupportInitialize)(this.warningPictureBox)).BeginInit();
            this.warningPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // warningLabel
            // 
            this.warningLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.warningLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.warningLabel.Location = new System.Drawing.Point(32, 0);
            this.warningLabel.Name = "warningLabel";
            this.warningLabel.Size = new System.Drawing.Size(377, 36);
            this.warningLabel.TabIndex = 1;
            this.warningLabel.Text = "You must specify the computer on your network that is the {0} server before running the Connector Service Monitor.";
            this.warningLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // warningPictureBox
            // 
            this.warningPictureBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.warningPictureBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.warningPictureBox.Location = new System.Drawing.Point(0, 0);
            this.warningPictureBox.Margin = new System.Windows.Forms.Padding(0, 0, 0, 20);
            this.warningPictureBox.Name = "warningPictureBox";
            this.warningPictureBox.Padding = new System.Windows.Forms.Padding(0, 2, 0, 2);
            this.warningPictureBox.Size = new System.Drawing.Size(32, 36);
            this.warningPictureBox.TabIndex = 8;
            this.warningPictureBox.TabStop = false;
            // 
            // warningPanel
            // 
            this.warningPanel.BackColor = System.Drawing.SystemColors.Control;
            this.warningPanel.Controls.Add(this.warningLabel);
            this.warningPanel.Controls.Add(this.warningPictureBox);
            this.warningPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.warningPanel.Location = new System.Drawing.Point(0, 0);
            this.warningPanel.Name = "warningPanel";
            this.warningPanel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 6);
            this.warningPanel.Size = new System.Drawing.Size(409, 42);
            this.warningPanel.TabIndex = 10;
            // 
            // errorImageList
            // 
            this.errorImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("errorImageList.ImageStream")));
            this.errorImageList.TransparentColor = System.Drawing.Color.White;
            this.errorImageList.Images.SetKeyName(0, "exclamation");
            // 
            // specifyServer
            // 
            this.specifyServer.AutoSize = true;
            this.specifyServer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.specifyServer.Location = new System.Drawing.Point(0, 42);
            this.specifyServer.Mode = ConnectorServiceMonitor.Internal.ServerSelectionMode.InitializeServer;
            this.specifyServer.Name = "specifyServer";
            this.specifyServer.Size = new System.Drawing.Size(409, 142);
            this.specifyServer.TabIndex = 11;
            // 
            // InitializeServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.specifyServer);
            this.Controls.Add(this.warningPanel);
            this.Name = "InitializeServer";
            this.Size = new System.Drawing.Size(409, 184);
            ((System.ComponentModel.ISupportInitialize)(this.warningPictureBox)).EndInit();
            this.warningPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label warningLabel;
        private System.Windows.Forms.PictureBox warningPictureBox;
        private System.Windows.Forms.Panel warningPanel;
        private SpecifyServer specifyServer;
        private System.Windows.Forms.ImageList errorImageList;
    }
}
