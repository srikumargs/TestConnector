using System.Security.AccessControl;

namespace ConnectorServiceMonitor
{
    internal partial class MainForm
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
                if (_terminateWorkerThreadEvent != null)
                {
                    _terminateWorkerThreadEvent.Dispose();
                }
                if (_pulseWorkerThreadEvent != null)
                {
                    _pulseWorkerThreadEvent.Dispose();
                }
                if(_imageManager != null)
                {
                    _imageManager.Dispose();
                }
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this._notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this._contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this._monitorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSplitButton1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this._contextFiveSecondsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._contextTenSecondsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._contextThirtySecondsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._contextSixtySecondsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._contextTwoMinutesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._contextFiveMinutesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._contextTenMinutesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._contextNoneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this._runOnLoginToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this._exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this._tabControl = new System.Windows.Forms.TabControl();
            this._generalTabPage = new System.Windows.Forms.TabPage();
            this._generalHtmlContentUserControl = new ConnectorServiceMonitor.Internal.HtmlContentUserControl();
            this._connectionsTabPage = new System.Windows.Forms.TabPage();
            this._connectionsHtmlContentUserControl = new ConnectorServiceMonitor.Internal.HtmlContentUserControl();
            this._requestsTabPage = new System.Windows.Forms.TabPage();
            this._requestsHtmlContentUserControl = new ConnectorServiceMonitor.Internal.HtmlContentUserControl();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this._settingsTabPage = new System.Windows.Forms.TabPage();
            this._settingsUserControl = new ConnectorServiceMonitor.SettingsUserControl();
            this._toolStrip = new System.Windows.Forms.ToolStrip();
            this.SageDatacloudImage = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this._statusStrip = new System.Windows.Forms.StatusStrip();
            this._toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this._contextMenuStrip.SuspendLayout();
            this._tabControl.SuspendLayout();
            this._generalTabPage.SuspendLayout();
            this._connectionsTabPage.SuspendLayout();
            this._requestsTabPage.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this._settingsTabPage.SuspendLayout();
            this._toolStrip.SuspendLayout();
            this._statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // _notifyIcon
            // 
            this._notifyIcon.ContextMenuStrip = this._contextMenuStrip;
            this._notifyIcon.Visible = true;
            this._notifyIcon.BalloonTipClicked += new System.EventHandler(this._notifyIcon_BalloonTipClicked);
            this._notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this._notifyIcon_MouseClick);
            // 
            // _contextMenuStrip
            // 
            this._contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._monitorToolStripMenuItem,
            this._settingsToolStripMenuItem,
            this.toolStripSeparator3,
            this.toolStripSplitButton1,
            this.toolStripSeparator4,
            this._runOnLoginToolStripMenuItem,
            this.toolStripSeparator1,
            this._exitToolStripMenuItem});
            this._contextMenuStrip.Name = "contextMenuStrip1";
            this._contextMenuStrip.Size = new System.Drawing.Size(143, 132);
            // 
            // _monitorToolStripMenuItem
            // 
            this._monitorToolStripMenuItem.Name = "_monitorToolStripMenuItem";
            this._monitorToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this._monitorToolStripMenuItem.Text = "&Monitor...";
            this._monitorToolStripMenuItem.Click += new System.EventHandler(this._monitorToolStripMenuItem_Click);
            // 
            // _settingsToolStripMenuItem
            // 
            this._settingsToolStripMenuItem.Name = "_settingsToolStripMenuItem";
            this._settingsToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this._settingsToolStripMenuItem.Text = "&Settings...";
            this._settingsToolStripMenuItem.Click += new System.EventHandler(this._settingsToolStripMenuItem_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(139, 6);
            // 
            // toolStripSplitButton1
            // 
            this.toolStripSplitButton1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.toolStripSeparator2,
            this._contextFiveSecondsToolStripMenuItem,
            this._contextTenSecondsToolStripMenuItem,
            this._contextThirtySecondsToolStripMenuItem,
            this._contextSixtySecondsToolStripMenuItem,
            this._contextTwoMinutesToolStripMenuItem,
            this._contextFiveMinutesToolStripMenuItem,
            this._contextTenMinutesToolStripMenuItem,
            this._contextNoneToolStripMenuItem});
            this.toolStripSplitButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSplitButton1.Name = "toolStripSplitButton1";
            this.toolStripSplitButton1.Size = new System.Drawing.Size(142, 22);
            this.toolStripSplitButton1.Text = "Refresh";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.toolStripMenuItem1.Size = new System.Drawing.Size(163, 22);
            this.toolStripMenuItem1.Text = "&Now";
            this.toolStripMenuItem1.Click += new System.EventHandler(this._nowToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(160, 6);
            // 
            // _contextFiveSecondsToolStripMenuItem
            // 
            this._contextFiveSecondsToolStripMenuItem.CheckOnClick = true;
            this._contextFiveSecondsToolStripMenuItem.Name = "_contextFiveSecondsToolStripMenuItem";
            this._contextFiveSecondsToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this._contextFiveSecondsToolStripMenuItem.Text = "Every &5 seconds";
            // 
            // _contextTenSecondsToolStripMenuItem
            // 
            this._contextTenSecondsToolStripMenuItem.CheckOnClick = true;
            this._contextTenSecondsToolStripMenuItem.Name = "_contextTenSecondsToolStripMenuItem";
            this._contextTenSecondsToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this._contextTenSecondsToolStripMenuItem.Text = "Every &10 seconds";
            // 
            // _contextThirtySecondsToolStripMenuItem
            // 
            this._contextThirtySecondsToolStripMenuItem.CheckOnClick = true;
            this._contextThirtySecondsToolStripMenuItem.Name = "_contextThirtySecondsToolStripMenuItem";
            this._contextThirtySecondsToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this._contextThirtySecondsToolStripMenuItem.Text = "Every &30 seconds";
            // 
            // _contextSixtySecondsToolStripMenuItem
            // 
            this._contextSixtySecondsToolStripMenuItem.CheckOnClick = true;
            this._contextSixtySecondsToolStripMenuItem.Name = "_contextSixtySecondsToolStripMenuItem";
            this._contextSixtySecondsToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this._contextSixtySecondsToolStripMenuItem.Text = "Every &60 seconds";
            // 
            // _contextTwoMinutesToolStripMenuItem
            // 
            this._contextTwoMinutesToolStripMenuItem.CheckOnClick = true;
            this._contextTwoMinutesToolStripMenuItem.Name = "_contextTwoMinutesToolStripMenuItem";
            this._contextTwoMinutesToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this._contextTwoMinutesToolStripMenuItem.Text = "Every &2 minutes";
            // 
            // _contextFiveMinutesToolStripMenuItem
            // 
            this._contextFiveMinutesToolStripMenuItem.CheckOnClick = true;
            this._contextFiveMinutesToolStripMenuItem.Name = "_contextFiveMinutesToolStripMenuItem";
            this._contextFiveMinutesToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this._contextFiveMinutesToolStripMenuItem.Text = "Every &5 minutes";
            // 
            // _contextTenMinutesToolStripMenuItem
            // 
            this._contextTenMinutesToolStripMenuItem.CheckOnClick = true;
            this._contextTenMinutesToolStripMenuItem.Name = "_contextTenMinutesToolStripMenuItem";
            this._contextTenMinutesToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this._contextTenMinutesToolStripMenuItem.Text = "Every &10 minutes";
            // 
            // _contextNoneToolStripMenuItem
            // 
            this._contextNoneToolStripMenuItem.Name = "_contextNoneToolStripMenuItem";
            this._contextNoneToolStripMenuItem.Size = new System.Drawing.Size(163, 22);
            this._contextNoneToolStripMenuItem.Text = "N&one";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(139, 6);
            // 
            // _runOnLoginToolStripMenuItem
            // 
            this._runOnLoginToolStripMenuItem.CheckOnClick = true;
            this._runOnLoginToolStripMenuItem.Name = "_runOnLoginToolStripMenuItem";
            this._runOnLoginToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this._runOnLoginToolStripMenuItem.Text = "Run on login";
            this._runOnLoginToolStripMenuItem.Click += new System.EventHandler(this._runOnLoginToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(139, 6);
            // 
            // _exitToolStripMenuItem
            // 
            this._exitToolStripMenuItem.Name = "_exitToolStripMenuItem";
            this._exitToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
            this._exitToolStripMenuItem.Text = "E&xit";
            this._exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // _tabControl
            // 
            this._tabControl.Controls.Add(this._generalTabPage);
            this._tabControl.Controls.Add(this._connectionsTabPage);
            this._tabControl.Controls.Add(this._requestsTabPage);
            this._tabControl.Controls.Add(this._settingsTabPage);
            this._tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tabControl.Location = new System.Drawing.Point(0, 25);
            this._tabControl.Name = "_tabControl";
            this._tabControl.SelectedIndex = 0;
            this._tabControl.Size = new System.Drawing.Size(746, 326);
            this._tabControl.TabIndex = 7;
            // 
            // _generalTabPage
            // 
            this._generalTabPage.Controls.Add(this._generalHtmlContentUserControl);
            this._generalTabPage.Location = new System.Drawing.Point(4, 22);
            this._generalTabPage.Name = "_generalTabPage";
            this._generalTabPage.Padding = new System.Windows.Forms.Padding(3);
            this._generalTabPage.Size = new System.Drawing.Size(738, 300);
            this._generalTabPage.TabIndex = 2;
            this._generalTabPage.Text = "General";
            this._generalTabPage.UseVisualStyleBackColor = true;
            // 
            // _generalHtmlContentUserControl
            // 
            this._generalHtmlContentUserControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._generalHtmlContentUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this._generalHtmlContentUserControl.Location = new System.Drawing.Point(3, 3);
            this._generalHtmlContentUserControl.Name = "_generalHtmlContentUserControl";
            this._generalHtmlContentUserControl.Size = new System.Drawing.Size(732, 294);
            this._generalHtmlContentUserControl.TabIndex = 0;
            // 
            // _connectionsTabPage
            // 
            this._connectionsTabPage.Controls.Add(this._connectionsHtmlContentUserControl);
            this._connectionsTabPage.Location = new System.Drawing.Point(4, 22);
            this._connectionsTabPage.Name = "_connectionsTabPage";
            this._connectionsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this._connectionsTabPage.Size = new System.Drawing.Size(738, 300);
            this._connectionsTabPage.TabIndex = 3;
            this._connectionsTabPage.Text = "Connections";
            this._connectionsTabPage.UseVisualStyleBackColor = true;
            // 
            // _connectionsHtmlContentUserControl
            // 
            this._connectionsHtmlContentUserControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._connectionsHtmlContentUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this._connectionsHtmlContentUserControl.Location = new System.Drawing.Point(3, 3);
            this._connectionsHtmlContentUserControl.Name = "_connectionsHtmlContentUserControl";
            this._connectionsHtmlContentUserControl.Size = new System.Drawing.Size(732, 294);
            this._connectionsHtmlContentUserControl.TabIndex = 1;
            // 
            // _requestsTabPage
            // 
            this._requestsTabPage.Controls.Add(this._requestsHtmlContentUserControl);
            this._requestsTabPage.Controls.Add(this.tableLayoutPanel1);
            this._requestsTabPage.Location = new System.Drawing.Point(4, 22);
            this._requestsTabPage.Name = "_requestsTabPage";
            this._requestsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this._requestsTabPage.Size = new System.Drawing.Size(738, 300);
            this._requestsTabPage.TabIndex = 4;
            this._requestsTabPage.Text = "Requests";
            this._requestsTabPage.UseVisualStyleBackColor = true;
            // 
            // _requestsHtmlContentUserControl
            // 
            this._requestsHtmlContentUserControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this._requestsHtmlContentUserControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this._requestsHtmlContentUserControl.Location = new System.Drawing.Point(3, 40);
            this._requestsHtmlContentUserControl.Name = "_requestsHtmlContentUserControl";
            this._requestsHtmlContentUserControl.Size = new System.Drawing.Size(732, 257);
            this._requestsHtmlContentUserControl.TabIndex = 1;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.comboBox1, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(732, 37);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(51, 27);
            this.label1.TabIndex = 0;
            this.label1.Text = "Showing:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // comboBox1
            // 
            this.comboBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "All requests that are currently in-progress",
            "All requests received in the last hour",
            "All requests received in the last day",
            "All requests received in the last week"});
            this.comboBox1.Location = new System.Drawing.Point(60, 3);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(669, 21);
            this.comboBox1.TabIndex = 1;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // _settingsTabPage
            // 
            this._settingsTabPage.Controls.Add(this._settingsUserControl);
            this._settingsTabPage.Location = new System.Drawing.Point(4, 22);
            this._settingsTabPage.Name = "_settingsTabPage";
            this._settingsTabPage.Padding = new System.Windows.Forms.Padding(3);
            this._settingsTabPage.Size = new System.Drawing.Size(738, 300);
            this._settingsTabPage.TabIndex = 5;
            this._settingsTabPage.Text = "Settings";
            this._settingsTabPage.UseVisualStyleBackColor = true;
            // 
            // _settingsUserControl
            // 
            this._settingsUserControl.Location = new System.Drawing.Point(0, 6);
            this._settingsUserControl.Name = "_settingsUserControl";
            this._settingsUserControl.Size = new System.Drawing.Size(732, 293);
            this._settingsUserControl.TabIndex = 0;
            // 
            // _toolStrip
            // 
            this._toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this._toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.SageDatacloudImage,
            this.toolStripLabel1,
            this.toolStripLabel2});
            this._toolStrip.Location = new System.Drawing.Point(0, 0);
            this._toolStrip.Name = "_toolStrip";
            this._toolStrip.Size = new System.Drawing.Size(746, 25);
            this._toolStrip.TabIndex = 8;
            this._toolStrip.Text = "toolStrip1";
            // 
            // SageDatacloudImage
            // 
            this.SageDatacloudImage.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.SageDatacloudImage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.SageDatacloudImage.Image = ((System.Drawing.Image)(resources.GetObject("SageDatacloudImage.Image")));
            this.SageDatacloudImage.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.SageDatacloudImage.Name = "SageDatacloudImage";
            this.SageDatacloudImage.Size = new System.Drawing.Size(23, 22);
            this.SageDatacloudImage.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripLabel1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolStripLabel1.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(0, 22);
            this.toolStripLabel1.TextImageRelation = System.Windows.Forms.TextImageRelation.Overlay;
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.toolStripLabel2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripLabel2.Image = global::ConnectorServiceMonitor.Properties.Resources.sage_data_cloud_connector_monitor_logo;
            this.toolStripLabel2.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(116, 22);
            this.toolStripLabel2.Text = "                                                       ";
            // 
            // _statusStrip
            // 
            this._statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._toolStripStatusLabel,
            this.toolStripStatusLabel1});
            this._statusStrip.Location = new System.Drawing.Point(0, 351);
            this._statusStrip.Name = "_statusStrip";
            this._statusStrip.Size = new System.Drawing.Size(746, 22);
            this._statusStrip.TabIndex = 9;
            this._statusStrip.Text = "statusStrip1";
            // 
            // _toolStripStatusLabel
            // 
            this._toolStripStatusLabel.Name = "_toolStripStatusLabel";
            this._toolStripStatusLabel.Size = new System.Drawing.Size(731, 17);
            this._toolStripStatusLabel.Spring = true;
            this._toolStripStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(746, 373);
            this.Controls.Add(this._tabControl);
            this.Controls.Add(this._statusStrip);
            this.Controls.Add(this._toolStrip);
            this.MinimumSize = new System.Drawing.Size(740, 340);
            this.Name = "MainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "{0}";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this._contextMenuStrip.ResumeLayout(false);
            this._tabControl.ResumeLayout(false);
            this._generalTabPage.ResumeLayout(false);
            this._connectionsTabPage.ResumeLayout(false);
            this._requestsTabPage.ResumeLayout(false);
            this._requestsTabPage.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this._settingsTabPage.ResumeLayout(false);
            this._toolStrip.ResumeLayout(false);
            this._toolStrip.PerformLayout();
            this._statusStrip.ResumeLayout(false);
            this._statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon _notifyIcon;
        private System.Windows.Forms.ContextMenuStrip _contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem _monitorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.TabControl _tabControl;
        private System.Windows.Forms.ToolStrip _toolStrip;
        private System.Windows.Forms.StatusStrip _statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel _toolStripStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripMenuItem _settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripMenuItem toolStripSplitButton1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem _contextFiveSecondsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _contextTenSecondsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _contextThirtySecondsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _contextSixtySecondsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _contextTwoMinutesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _contextFiveMinutesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _contextTenMinutesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem _contextNoneToolStripMenuItem;
        private System.Windows.Forms.TabPage _generalTabPage;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem _runOnLoginToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton SageDatacloudImage;
        private Internal.HtmlContentUserControl _generalHtmlContentUserControl;
        private System.Windows.Forms.TabPage _connectionsTabPage;
        private Internal.HtmlContentUserControl _connectionsHtmlContentUserControl;
        private System.Windows.Forms.TabPage _requestsTabPage;
        private Internal.HtmlContentUserControl _requestsHtmlContentUserControl;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.TabPage _settingsTabPage;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private SettingsUserControl _settingsUserControl;
    }
}

