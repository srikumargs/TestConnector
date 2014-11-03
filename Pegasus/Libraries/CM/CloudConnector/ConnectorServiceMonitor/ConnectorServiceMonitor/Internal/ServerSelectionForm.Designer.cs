namespace ConnectorServiceMonitor.Internal
{
    /// <summary>
    /// Class used to allow the user to specifiy a server
    /// </summary>
    partial class ServerSelectionForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerSelectionForm));
            this.closeButton = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.buttonPanel = new System.Windows.Forms.Panel();
            this.buttonSpacerPanel = new System.Windows.Forms.Panel();
            this.helpButton = new System.Windows.Forms.Button();
            this.panelStack = new Sage.CRE.Core.UI.PanelStack();
            this.initializeServerPanel = new Sage.CRE.Core.UI.PanelStackPanel();
            this.successPanel = new Sage.CRE.Core.UI.PanelStackPanel();
            this.changeServerPanel = new Sage.CRE.Core.UI.PanelStackPanel();
            this.initializeServer = new ConnectorServiceMonitor.Internal.InitializeServer();
            this.specifyServerSuccess = new ConnectorServiceMonitor.Internal.SpecifyServerSuccess();
            this.changeServer = new ConnectorServiceMonitor.Internal.ChangeServer();
            this.buttonPanel.SuspendLayout();
            this.panelStack.SuspendLayout();
            this.initializeServerPanel.SuspendLayout();
            this.successPanel.SuspendLayout();
            this.changeServerPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // closeButton
            // 
            this.closeButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.closeButton.Location = new System.Drawing.Point(251, 0);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 23);
            this.closeButton.TabIndex = 1;
            this.closeButton.Text = "&Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "");
            // 
            // buttonPanel
            // 
            this.buttonPanel.Controls.Add(this.closeButton);
            this.buttonPanel.Controls.Add(this.buttonSpacerPanel);
            this.buttonPanel.Controls.Add(this.helpButton);
            this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonPanel.Location = new System.Drawing.Point(10, 162);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.Size = new System.Drawing.Size(407, 23);
            this.buttonPanel.TabIndex = 12;
            // 
            // buttonSpacerPanel
            // 
            this.buttonSpacerPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.buttonSpacerPanel.Location = new System.Drawing.Point(326, 0);
            this.buttonSpacerPanel.Name = "buttonSpacerPanel";
            this.buttonSpacerPanel.Size = new System.Drawing.Size(6, 23);
            this.buttonSpacerPanel.TabIndex = 3;
            // 
            // helpButton
            // 
            this.helpButton.Dock = System.Windows.Forms.DockStyle.Right;
            this.helpButton.Location = new System.Drawing.Point(332, 0);
            this.helpButton.Name = "helpButton";
            this.helpButton.Size = new System.Drawing.Size(75, 23);
            this.helpButton.TabIndex = 2;
            this.helpButton.Text = "&Help";
            this.helpButton.UseVisualStyleBackColor = true;
            this.helpButton.Click += new System.EventHandler(this.helpButton_Click);
            // 
            // panelStack
            // 
            this.panelStack.ActivePanel = this.initializeServerPanel;
            this.panelStack.Controls.Add(this.initializeServerPanel);
            this.panelStack.Controls.Add(this.successPanel);
            this.panelStack.Controls.Add(this.changeServerPanel);
            this.panelStack.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelStack.Location = new System.Drawing.Point(10, 10);
            this.panelStack.Name = "panelStack";
            this.panelStack.Panels.AddRange(new Sage.CRE.Core.UI.PanelStackPanel[] {
            this.initializeServerPanel,
            this.changeServerPanel,
            this.successPanel});
            this.panelStack.Size = new System.Drawing.Size(407, 152);
            this.panelStack.TabIndex = 13;
            // 
            // initializeServerPanel
            // 
            this.initializeServerPanel.Controls.Add(this.initializeServer);
            this.initializeServerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.initializeServerPanel.Location = new System.Drawing.Point(0, 0);
            this.initializeServerPanel.Name = "initializeServerPanel";
            this.initializeServerPanel.Size = new System.Drawing.Size(407, 152);
            this.initializeServerPanel.TabIndex = 4;
            // 
            // successPanel
            // 
            this.successPanel.Controls.Add(this.specifyServerSuccess);
            this.successPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.successPanel.Location = new System.Drawing.Point(0, 0);
            this.successPanel.Name = "successPanel";
            this.successPanel.Size = new System.Drawing.Size(407, 152);
            this.successPanel.TabIndex = 6;
            // 
            // changeServerPanel
            // 
            this.changeServerPanel.Controls.Add(this.changeServer);
            this.changeServerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.changeServerPanel.Location = new System.Drawing.Point(0, 0);
            this.changeServerPanel.Name = "changeServerPanel";
            this.changeServerPanel.Size = new System.Drawing.Size(407, 152);
            this.changeServerPanel.TabIndex = 5;
            // 
            // initializeServer
            // 
            this.initializeServer.AutoSize = true;
            this.initializeServer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.initializeServer.Location = new System.Drawing.Point(0, 0);
            this.initializeServer.Name = "initializeServer";
            this.initializeServer.Size = new System.Drawing.Size(407, 152);
            this.initializeServer.TabIndex = 0;
            // 
            // specifyServerSuccess
            // 
            this.specifyServerSuccess.AutoSize = true;
            this.specifyServerSuccess.Dock = System.Windows.Forms.DockStyle.Fill;
            this.specifyServerSuccess.Location = new System.Drawing.Point(0, 0);
            this.specifyServerSuccess.Name = "specifyServerSuccess";
            this.specifyServerSuccess.Size = new System.Drawing.Size(407, 152);
            this.specifyServerSuccess.TabIndex = 0;
            // 
            // changeServer
            // 
            this.changeServer.AutoSize = true;
            this.changeServer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.changeServer.Location = new System.Drawing.Point(0, 0);
            this.changeServer.Name = "changeServer";
            this.changeServer.Size = new System.Drawing.Size(407, 152);
            this.changeServer.TabIndex = 0;
            // 
            // ServerSelectionForm
            // 
            this.AcceptButton = this.closeButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(427, 195);
            this.Controls.Add(this.panelStack);
            this.Controls.Add(this.buttonPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ServerSelectionForm";
            this.Padding = new System.Windows.Forms.Padding(10);
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Specify {0} Server";
            this.Load += new System.EventHandler(this.ServerSelectionWizard_Load);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.ServerSelection_HelpRequested);
            this.buttonPanel.ResumeLayout(false);
            this.panelStack.ResumeLayout(false);
            this.initializeServerPanel.ResumeLayout(false);
            this.initializeServerPanel.PerformLayout();
            this.successPanel.ResumeLayout(false);
            this.successPanel.PerformLayout();
            this.changeServerPanel.ResumeLayout(false);
            this.changeServerPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Panel buttonPanel;
        private System.Windows.Forms.Panel buttonSpacerPanel;
        private System.Windows.Forms.Button helpButton;
        private Sage.CRE.Core.UI.PanelStack panelStack;
        private Sage.CRE.Core.UI.PanelStackPanel initializeServerPanel;
        private Sage.CRE.Core.UI.PanelStackPanel changeServerPanel;
        private Sage.CRE.Core.UI.PanelStackPanel successPanel;
        private InitializeServer initializeServer;
        private ChangeServer changeServer;
        private SpecifyServerSuccess specifyServerSuccess;
    }
}