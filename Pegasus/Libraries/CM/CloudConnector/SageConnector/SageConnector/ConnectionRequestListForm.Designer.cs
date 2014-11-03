namespace SageConnector
{
    partial class ConnectionRequestListForm
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
                if (_applyRefreshDataTimer != null)
                {
                    _applyRefreshDataTimer.Dispose();
                    _applyRefreshDataTimer = null;
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectionRequestListForm));
            this.btnClose = new System.Windows.Forms.Button();
            this.dgRequestList = new System.Windows.Forms.DataGridView();
            this.colRequestStatusImage = new System.Windows.Forms.DataGridViewImageColumn();
            this.colTimeRequested = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRequestType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRequestSummary = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRequestProjectName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRequestedBy = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRequestId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTimeElapsed = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SelectionMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.selectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.clearSelectionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnDeleteRequests = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.panelButtons = new System.Windows.Forms.Panel();
            this.Selections = new System.Windows.Forms.Button();
            this.lnkHelpLink = new System.Windows.Forms.LinkLabel();
            this.lblCompanyNameData = new System.Windows.Forms.Label();
            this.lblBackofficeNameData = new System.Windows.Forms.Label();
            this.picTenantCompanyStatusImage = new System.Windows.Forms.PictureBox();
            this.picBOCompanyStatusImage = new System.Windows.Forms.PictureBox();
            this.tblCompanyInformation = new System.Windows.Forms.TableLayoutPanel();
            this.picArrow = new System.Windows.Forms.PictureBox();
            this.lblBackOfficeLabel = new System.Windows.Forms.Label();
            this.lblCloudLabel = new System.Windows.Forms.Label();
            this.lblAsOfLabel = new System.Windows.Forms.Label();
            this.lblLastRefreshTime = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dgRequestList)).BeginInit();
            this.SelectionMenuStrip.SuspendLayout();
            this.panelButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picTenantCompanyStatusImage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBOCompanyStatusImage)).BeginInit();
            this.tblCompanyInformation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picArrow)).BeginInit();
            this.SuspendLayout();
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.AutoSize = true;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnClose.Location = new System.Drawing.Point(731, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "&Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // dgRequestList
            // 
            this.dgRequestList.AllowUserToAddRows = false;
            this.dgRequestList.AllowUserToOrderColumns = true;
            this.dgRequestList.AllowUserToResizeRows = false;
            this.dgRequestList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgRequestList.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgRequestList.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgRequestList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgRequestList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgRequestList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colRequestStatusImage,
            this.colTimeRequested,
            this.colRequestType,
            this.colRequestSummary,
            this.colRequestProjectName,
            this.colRequestedBy,
            this.colRequestId,
            this.colTimeElapsed});
            this.dgRequestList.ContextMenuStrip = this.SelectionMenuStrip;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.Format = "G";
            dataGridViewCellStyle2.NullValue = null;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgRequestList.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgRequestList.Location = new System.Drawing.Point(12, 82);
            this.dgRequestList.Name = "dgRequestList";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgRequestList.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgRequestList.RowHeadersVisible = false;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.dgRequestList.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dgRequestList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgRequestList.Size = new System.Drawing.Size(808, 283);
            this.dgRequestList.TabIndex = 3;
            this.dgRequestList.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dgRequestList_DataBindingComplete);
            this.dgRequestList.SelectionChanged += new System.EventHandler(this.dgRequestList_SelectionChanged);
            // 
            // colRequestStatusImage
            // 
            this.colRequestStatusImage.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.colRequestStatusImage.HeaderText = "colRequestStatusImage";
            this.colRequestStatusImage.Name = "colRequestStatusImage";
            this.colRequestStatusImage.ReadOnly = true;
            this.colRequestStatusImage.Width = 126;
            // 
            // colTimeRequested
            // 
            this.colTimeRequested.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colTimeRequested.HeaderText = "Started on";
            this.colTimeRequested.MinimumWidth = 10;
            this.colTimeRequested.Name = "colTimeRequested";
            this.colTimeRequested.ReadOnly = true;
            this.colTimeRequested.Width = 81;
            // 
            // colRequestType
            // 
            this.colRequestType.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colRequestType.HeaderText = "Type";
            this.colRequestType.Name = "colRequestType";
            this.colRequestType.ReadOnly = true;
            this.colRequestType.Width = 56;
            // 
            // colRequestSummary
            // 
            this.colRequestSummary.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colRequestSummary.HeaderText = "Details";
            this.colRequestSummary.Name = "colRequestSummary";
            this.colRequestSummary.ReadOnly = true;
            // 
            // colRequestProjectName
            // 
            this.colRequestProjectName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colRequestProjectName.HeaderText = "Project name";
            this.colRequestProjectName.Name = "colRequestProjectName";
            this.colRequestProjectName.ReadOnly = true;
            this.colRequestProjectName.Width = 94;
            // 
            // colRequestedBy
            // 
            this.colRequestedBy.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.colRequestedBy.HeaderText = "Requested by";
            this.colRequestedBy.MinimumWidth = 10;
            this.colRequestedBy.Name = "colRequestedBy";
            this.colRequestedBy.ReadOnly = true;
            this.colRequestedBy.Width = 98;
            // 
            // colRequestId
            // 
            this.colRequestId.HeaderText = "RequestId";
            this.colRequestId.Name = "colRequestId";
            this.colRequestId.ReadOnly = true;
            this.colRequestId.Visible = false;
            // 
            // colTimeElapsed
            // 
            this.colTimeElapsed.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colTimeElapsed.HeaderText = "Elapsed time";
            this.colTimeElapsed.Name = "colTimeElapsed";
            this.colTimeElapsed.ReadOnly = true;
            this.colTimeElapsed.Width = 92;
            // 
            // SelectionMenuStrip
            // 
            this.SelectionMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectToolStripMenuItem,
            this.clearSelectionsToolStripMenuItem});
            this.SelectionMenuStrip.Name = "SelectionMenuStrip";
            this.SelectionMenuStrip.Size = new System.Drawing.Size(157, 48);
            // 
            // selectToolStripMenuItem
            // 
            this.selectToolStripMenuItem.Name = "selectToolStripMenuItem";
            this.selectToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.selectToolStripMenuItem.Text = "Select all rows";
            this.selectToolStripMenuItem.Click += new System.EventHandler(this.selectToolStripMenuItem_Click);
            // 
            // clearSelectionsToolStripMenuItem
            // 
            this.clearSelectionsToolStripMenuItem.Name = "clearSelectionsToolStripMenuItem";
            this.clearSelectionsToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
            this.clearSelectionsToolStripMenuItem.Text = "Clear selections";
            this.clearSelectionsToolStripMenuItem.Click += new System.EventHandler(this.clearSelectionsToolStripMenuItem_Click);
            // 
            // btnDeleteRequests
            // 
            this.btnDeleteRequests.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteRequests.AutoSize = true;
            this.btnDeleteRequests.Enabled = false;
            this.btnDeleteRequests.Location = new System.Drawing.Point(621, 3);
            this.btnDeleteRequests.Name = "btnDeleteRequests";
            this.btnDeleteRequests.Size = new System.Drawing.Size(104, 23);
            this.btnDeleteRequests.TabIndex = 4;
            this.btnDeleteRequests.Text = "Cance&l request(s)";
            this.btnDeleteRequests.UseVisualStyleBackColor = true;
            this.btnDeleteRequests.Click += new System.EventHandler(this.btnDeleteRequests_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.AutoSize = true;
            this.btnRefresh.Location = new System.Drawing.Point(542, 3);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 1;
            this.btnRefresh.Text = "&Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // panelButtons
            // 
            this.panelButtons.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelButtons.Controls.Add(this.Selections);
            this.panelButtons.Controls.Add(this.btnDeleteRequests);
            this.panelButtons.Controls.Add(this.btnRefresh);
            this.panelButtons.Controls.Add(this.btnClose);
            this.panelButtons.Location = new System.Drawing.Point(9, 371);
            this.panelButtons.Name = "panelButtons";
            this.panelButtons.Size = new System.Drawing.Size(813, 29);
            this.panelButtons.TabIndex = 5;
            // 
            // Selections
            // 
            this.Selections.Image = global::SageConnector.Properties.Resources.selelection_up;
            this.Selections.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.Selections.Location = new System.Drawing.Point(1, 3);
            this.Selections.Name = "Selections";
            this.Selections.Size = new System.Drawing.Size(75, 23);
            this.Selections.TabIndex = 7;
            this.Selections.Text = "&Selections";
            this.Selections.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Selections.UseVisualStyleBackColor = true;
            this.Selections.Click += new System.EventHandler(this.Selections_Click);
            // 
            // lnkHelpLink
            // 
            this.lnkHelpLink.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lnkHelpLink.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lnkHelpLink.Image = ((System.Drawing.Image)(resources.GetObject("lnkHelpLink.Image")));
            this.lnkHelpLink.LinkArea = new System.Windows.Forms.LinkArea(0, 4);
            this.lnkHelpLink.LinkBehavior = System.Windows.Forms.LinkBehavior.NeverUnderline;
            this.lnkHelpLink.LinkColor = System.Drawing.Color.White;
            this.lnkHelpLink.Location = new System.Drawing.Point(783, 5);
            this.lnkHelpLink.Name = "lnkHelpLink";
            this.lnkHelpLink.Size = new System.Drawing.Size(20, 17);
            this.lnkHelpLink.TabIndex = 19;
            this.lnkHelpLink.TabStop = true;
            this.lnkHelpLink.Text = "    ";
            this.lnkHelpLink.VisitedLinkColor = System.Drawing.Color.White;
            this.lnkHelpLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkHelpLink_LinkClicked);
            // 
            // lblCompanyNameData
            // 
            this.lblCompanyNameData.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblCompanyNameData.AutoSize = true;
            this.lblCompanyNameData.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCompanyNameData.Location = new System.Drawing.Point(216, 37);
            this.lblCompanyNameData.Name = "lblCompanyNameData";
            this.lblCompanyNameData.Size = new System.Drawing.Size(116, 13);
            this.lblCompanyNameData.TabIndex = 2;
            this.lblCompanyNameData.Text = "Tenant company name";
            // 
            // lblBackofficeNameData
            // 
            this.lblBackofficeNameData.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblBackofficeNameData.AutoSize = true;
            this.lblBackofficeNameData.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBackofficeNameData.Location = new System.Drawing.Point(5, 37);
            this.lblBackofficeNameData.Name = "lblBackofficeNameData";
            this.lblBackofficeNameData.Size = new System.Drawing.Size(90, 13);
            this.lblBackofficeNameData.TabIndex = 3;
            this.lblBackofficeNameData.Text = "Back office name";
            // 
            // picTenantCompanyStatusImage
            // 
            this.picTenantCompanyStatusImage.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.picTenantCompanyStatusImage.Location = new System.Drawing.Point(186, 31);
            this.picTenantCompanyStatusImage.Name = "picTenantCompanyStatusImage";
            this.picTenantCompanyStatusImage.Size = new System.Drawing.Size(24, 25);
            this.picTenantCompanyStatusImage.TabIndex = 4;
            this.picTenantCompanyStatusImage.TabStop = false;
            // 
            // picBOCompanyStatusImage
            // 
            this.picBOCompanyStatusImage.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.picBOCompanyStatusImage.Location = new System.Drawing.Point(101, 31);
            this.picBOCompanyStatusImage.Name = "picBOCompanyStatusImage";
            this.picBOCompanyStatusImage.Size = new System.Drawing.Size(24, 25);
            this.picBOCompanyStatusImage.TabIndex = 5;
            this.picBOCompanyStatusImage.TabStop = false;
            // 
            // tblCompanyInformation
            // 
            this.tblCompanyInformation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tblCompanyInformation.ColumnCount = 7;
            this.tblCompanyInformation.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblCompanyInformation.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tblCompanyInformation.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.tblCompanyInformation.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tblCompanyInformation.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblCompanyInformation.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblCompanyInformation.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 342F));
            this.tblCompanyInformation.Controls.Add(this.lblBackofficeNameData, 0, 1);
            this.tblCompanyInformation.Controls.Add(this.lblCompanyNameData, 4, 1);
            this.tblCompanyInformation.Controls.Add(this.picArrow, 2, 1);
            this.tblCompanyInformation.Controls.Add(this.lnkHelpLink, 6, 0);
            this.tblCompanyInformation.Controls.Add(this.lblBackOfficeLabel, 0, 0);
            this.tblCompanyInformation.Controls.Add(this.lblCloudLabel, 4, 0);
            this.tblCompanyInformation.Controls.Add(this.lblAsOfLabel, 5, 0);
            this.tblCompanyInformation.Controls.Add(this.lblLastRefreshTime, 5, 1);
            this.tblCompanyInformation.Controls.Add(this.picBOCompanyStatusImage, 1, 1);
            this.tblCompanyInformation.Controls.Add(this.picTenantCompanyStatusImage, 3, 1);
            this.tblCompanyInformation.Location = new System.Drawing.Point(12, 12);
            this.tblCompanyInformation.Name = "tblCompanyInformation";
            this.tblCompanyInformation.Padding = new System.Windows.Forms.Padding(2);
            this.tblCompanyInformation.RowCount = 2;
            this.tblCompanyInformation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tblCompanyInformation.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tblCompanyInformation.Size = new System.Drawing.Size(808, 64);
            this.tblCompanyInformation.TabIndex = 10;
            // 
            // picArrow
            // 
            this.picArrow.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.picArrow.Location = new System.Drawing.Point(131, 33);
            this.picArrow.Name = "picArrow";
            this.picArrow.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.picArrow.Size = new System.Drawing.Size(49, 22);
            this.picArrow.TabIndex = 20;
            this.picArrow.TabStop = false;
            // 
            // lblBackOfficeLabel
            // 
            this.lblBackOfficeLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblBackOfficeLabel.AutoSize = true;
            this.lblBackOfficeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBackOfficeLabel.Location = new System.Drawing.Point(5, 7);
            this.lblBackOfficeLabel.Name = "lblBackOfficeLabel";
            this.lblBackOfficeLabel.Size = new System.Drawing.Size(72, 13);
            this.lblBackOfficeLabel.TabIndex = 21;
            this.lblBackOfficeLabel.Text = "Back office";
            // 
            // lblCloudLabel
            // 
            this.lblCloudLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblCloudLabel.AutoSize = true;
            this.lblCloudLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCloudLabel.Location = new System.Drawing.Point(216, 7);
            this.lblCloudLabel.Name = "lblCloudLabel";
            this.lblCloudLabel.Size = new System.Drawing.Size(29, 13);
            this.lblCloudLabel.TabIndex = 22;
            this.lblCloudLabel.Text = "Site";
            // 
            // lblAsOfLabel
            // 
            this.lblAsOfLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblAsOfLabel.AutoSize = true;
            this.lblAsOfLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAsOfLabel.Location = new System.Drawing.Point(338, 7);
            this.lblAsOfLabel.Name = "lblAsOfLabel";
            this.lblAsOfLabel.Size = new System.Drawing.Size(75, 13);
            this.lblAsOfLabel.TabIndex = 23;
            this.lblAsOfLabel.Text = "Status as of";
            // 
            // lblLastRefreshTime
            // 
            this.lblLastRefreshTime.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblLastRefreshTime.AutoSize = true;
            this.lblLastRefreshTime.Location = new System.Drawing.Point(338, 37);
            this.lblLastRefreshTime.Name = "lblLastRefreshTime";
            this.lblLastRefreshTime.Size = new System.Drawing.Size(123, 13);
            this.lblLastRefreshTime.TabIndex = 8;
            this.lblLastRefreshTime.Text = "12:12:12 AM 5/12/2012";
            // 
            // ConnectionRequestListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(834, 412);
            this.Controls.Add(this.tblCompanyInformation);
            this.Controls.Add(this.panelButtons);
            this.Controls.Add(this.dgRequestList);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(850, 450);
            this.Name = "ConnectionRequestListForm";
            this.Text = "ConnectionRequestListForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConnectionRequestListForm_FormClosing);
            this.Load += new System.EventHandler(this.ConnectionRequestListForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgRequestList)).EndInit();
            this.SelectionMenuStrip.ResumeLayout(false);
            this.panelButtons.ResumeLayout(false);
            this.panelButtons.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picTenantCompanyStatusImage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picBOCompanyStatusImage)).EndInit();
            this.tblCompanyInformation.ResumeLayout(false);
            this.tblCompanyInformation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picArrow)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.DataGridView dgRequestList;
        private System.Windows.Forms.Button btnDeleteRequests;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Panel panelButtons;
        private System.Windows.Forms.Label lblCompanyNameData;
        private System.Windows.Forms.PictureBox picTenantCompanyStatusImage;
        private System.Windows.Forms.LinkLabel lnkHelpLink;
        private System.Windows.Forms.Label lblBackofficeNameData;
        private System.Windows.Forms.PictureBox picBOCompanyStatusImage;
        private System.Windows.Forms.TableLayoutPanel tblCompanyInformation;
        private System.Windows.Forms.PictureBox picArrow;
        private System.Windows.Forms.DataGridViewImageColumn colRequestStatusImage;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTimeRequested;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRequestType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRequestSummary;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRequestProjectName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRequestedBy;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRequestId;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTimeElapsed;
        private System.Windows.Forms.Label lblBackOfficeLabel;
        private System.Windows.Forms.Label lblCloudLabel;
        private System.Windows.Forms.Label lblAsOfLabel;
        private System.Windows.Forms.Label lblLastRefreshTime;
        private System.Windows.Forms.ContextMenuStrip SelectionMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem selectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem clearSelectionsToolStripMenuItem;
        private System.Windows.Forms.Button Selections;
    }
}