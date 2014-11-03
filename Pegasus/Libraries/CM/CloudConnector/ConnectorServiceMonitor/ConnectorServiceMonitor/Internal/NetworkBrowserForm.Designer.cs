namespace ConnectorServiceMonitor.Internal

{
    partial class NetworkBrowserForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NetworkBrowserForm));
            this._selectLbl = new System.Windows.Forms.Label();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this._cancelBtn = new System.Windows.Forms.Button();
            this._okBtn = new System.Windows.Forms.Button();
            this.buttonPanel = new System.Windows.Forms.Panel();
            this.buttonSpacerPanel2 = new System.Windows.Forms.Panel();
            this.buttonSpacerPanel1 = new System.Windows.Forms.Panel();
            this._helpBtn = new System.Windows.Forms.Button();
            this._listView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel1 = new System.Windows.Forms.Panel();
            this._portTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _selectLbl
            // 
            resources.ApplyResources(this._selectLbl, "_selectLbl");
            this._selectLbl.Name = "_selectLbl";
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Computers.ico");
            // 
            // _cancelBtn
            // 
            this._cancelBtn.CausesValidation = false;
            this._cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this._cancelBtn, "_cancelBtn");
            this._cancelBtn.Name = "_cancelBtn";
            this._cancelBtn.UseVisualStyleBackColor = true;
            // 
            // _okBtn
            // 
            this._okBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this._okBtn, "_okBtn");
            this._okBtn.Name = "_okBtn";
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
            resources.ApplyResources(this.buttonPanel, "buttonPanel");
            this.buttonPanel.Name = "buttonPanel";
            // 
            // buttonSpacerPanel2
            // 
            resources.ApplyResources(this.buttonSpacerPanel2, "buttonSpacerPanel2");
            this.buttonSpacerPanel2.Name = "buttonSpacerPanel2";
            // 
            // buttonSpacerPanel1
            // 
            resources.ApplyResources(this.buttonSpacerPanel1, "buttonSpacerPanel1");
            this.buttonSpacerPanel1.Name = "buttonSpacerPanel1";
            // 
            // _helpBtn
            // 
            this._helpBtn.CausesValidation = false;
            resources.ApplyResources(this._helpBtn, "_helpBtn");
            this._helpBtn.Name = "_helpBtn";
            this._helpBtn.UseVisualStyleBackColor = true;
            this._helpBtn.Click += new System.EventHandler(this._helpBtn_Click);
            // 
            // _listView
            // 
            this._listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            resources.ApplyResources(this._listView, "_listView");
            this._listView.FullRowSelect = true;
            this._listView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this._listView.MultiSelect = false;
            this._listView.Name = "_listView";
            this._listView.SmallImageList = this.imageList1;
            this._listView.UseCompatibleStateImageBehavior = false;
            this._listView.View = System.Windows.Forms.View.Details;
            this._listView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this._listView_MouseDoubleClick);
            // 
            // columnHeader1
            // 
            resources.ApplyResources(this.columnHeader1, "columnHeader1");
            // 
            // panel1
            // 
            this.panel1.CausesValidation = false;
            this.panel1.Controls.Add(this._portTextBox);
            this.panel1.Controls.Add(this.label1);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // _portTextBox
            // 
            resources.ApplyResources(this._portTextBox, "_portTextBox");
            this._portTextBox.Name = "_portTextBox";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // NetworkBrowserForm
            // 
            this.AcceptButton = this._okBtn;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelBtn;
            this.Controls.Add(this._listView);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.buttonPanel);
            this.Controls.Add(this._selectLbl);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "NetworkBrowserForm";
            this.ShowInTaskbar = false;
            this.Load += new System.EventHandler(this.NetworkBrowser_Load);
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.NetworkBrowser_HelpRequested);
            this.buttonPanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _selectLbl;
        private System.Windows.Forms.Button _cancelBtn;
        private System.Windows.Forms.Button _okBtn;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Panel buttonPanel;
        private System.Windows.Forms.Panel buttonSpacerPanel2;
        private System.Windows.Forms.Panel buttonSpacerPanel1;
        private System.Windows.Forms.Button _helpBtn;
        private System.Windows.Forms.ListView _listView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox _portTextBox;
        private System.Windows.Forms.Label label1;
    }
}