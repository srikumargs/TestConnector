namespace ConnectorServiceMonitor.Internal
{
    partial class ChangeServer
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
            this.currentServerLabel = new System.Windows.Forms.Label();
            this.specifyServer = new ConnectorServiceMonitor.Internal.SpecifyServer();
            this.SuspendLayout();
            // 
            // currentServerLabel
            // 
            this.currentServerLabel.AutoSize = true;
            this.currentServerLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.currentServerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.currentServerLabel.Location = new System.Drawing.Point(0, 0);
            this.currentServerLabel.Name = "currentServerLabel";
            this.currentServerLabel.Size = new System.Drawing.Size(0, 13);
            this.currentServerLabel.TabIndex = 1;
            // 
            // specifyServer
            // 
            this.specifyServer.AutoSize = true;
            this.specifyServer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.specifyServer.Location = new System.Drawing.Point(0, 13);
            this.specifyServer.Mode = ConnectorServiceMonitor.Internal.ServerSelectionMode.ChangeServer;
            this.specifyServer.Name = "specifyServer";
            this.specifyServer.Size = new System.Drawing.Size(406, 139);
            this.specifyServer.TabIndex = 2;
            // 
            // ChangeServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.specifyServer);
            this.Controls.Add(this.currentServerLabel);
            this.Name = "ChangeServer";
            this.Size = new System.Drawing.Size(406, 152);
            this.Load += new System.EventHandler(this.ChangeServer_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label currentServerLabel;
        private SpecifyServer specifyServer;
    }
}
