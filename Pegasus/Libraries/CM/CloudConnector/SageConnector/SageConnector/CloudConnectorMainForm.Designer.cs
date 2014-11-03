
namespace SageConnector
{
    partial class CloudConnectorMainForm
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

                if (_terminateGetRefreshDataThreadEvent != null)
                {
                    _terminateGetRefreshDataThreadEvent.Dispose();
                    _terminateGetRefreshDataThreadEvent = null;
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CloudConnectorMainForm));
            this.dgConnections = new System.Windows.Forms.DataGridView();
            this.colPremiseCompany = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPremiseConnectionStatus = new System.Windows.Forms.DataGridViewImageColumn();
            this.colConnectorLabel = new System.Windows.Forms.DataGridViewImageColumn();
            this.colTenantConnectionStatus = new System.Windows.Forms.DataGridViewImageColumn();
            this.colTenantCompany = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chkPremiseActiveStatusReceive = new System.Windows.Forms.CheckBox();
            this.lblPremiseActiveStatus = new System.Windows.Forms.Label();
            this.txtPremiseDatabasePath = new System.Windows.Forms.TextBox();
            this.lblPremiseDatabasePath = new System.Windows.Forms.Label();
            this.txtPremiseCompanyName = new System.Windows.Forms.TextBox();
            this.lblPremiseCompanyName = new System.Windows.Forms.Label();
            this.btnDeleteConnection = new System.Windows.Forms.Button();
            this.btnEditConnection = new System.Windows.Forms.Button();
            this.btnAddConnection = new System.Windows.Forms.Button();
            this.btnCloseConnector = new System.Windows.Forms.Button();
            this.lblVersion = new System.Windows.Forms.Label();
            this.lblSkyfireConnections = new System.Windows.Forms.Label();
            this.lblLine = new System.Windows.Forms.Label();
            this.lnkHelpLink = new System.Windows.Forms.LinkLabel();
            this.lblVersionNumber = new System.Windows.Forms.Label();
            this.grpCompanyInformation = new System.Windows.Forms.GroupBox();
            this.btnManageRequests = new System.Windows.Forms.Button();
            this.grpTenantInformation = new System.Windows.Forms.GroupBox();
            this.lnkTenantSiteUrl = new System.Windows.Forms.LinkLabel();
            this.chkTenantActiveStatusSend = new System.Windows.Forms.CheckBox();
            this.lblTenantCompanyName = new System.Windows.Forms.Label();
            this.chkTenantActiveStatusReceive = new System.Windows.Forms.CheckBox();
            this.txtTenantCompanyName = new System.Windows.Forms.TextBox();
            this.lblTenantActiveStatus = new System.Windows.Forms.Label();
            this.lblTenantDatabasePath = new System.Windows.Forms.Label();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.grpConnector = new System.Windows.Forms.GroupBox();
            this.tbSystemMessages = new System.Windows.Forms.TextBox();
            this.lnkConnectorUpdate = new System.Windows.Forms.LinkLabel();
            this.lnkBackofficeUpdate = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.lblLastRefresh = new System.Windows.Forms.Label();
            this.lnkBackofficeInstall = new System.Windows.Forms.LinkLabel();
            this.btnFeatureConfigurations = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgConnections)).BeginInit();
            this.grpCompanyInformation.SuspendLayout();
            this.grpTenantInformation.SuspendLayout();
            this.grpConnector.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgConnections
            // 
            this.dgConnections.AllowUserToAddRows = false;
            this.dgConnections.AllowUserToDeleteRows = false;
            this.dgConnections.AllowUserToResizeColumns = false;
            this.dgConnections.AllowUserToResizeRows = false;
            this.dgConnections.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgConnections.BackgroundColor = System.Drawing.SystemColors.Control;
            this.dgConnections.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgConnections.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgConnections.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgConnections.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colPremiseCompany,
            this.colPremiseConnectionStatus,
            this.colConnectorLabel,
            this.colTenantConnectionStatus,
            this.colTenantCompany});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgConnections.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgConnections.Location = new System.Drawing.Point(12, 33);
            this.dgConnections.MultiSelect = false;
            this.dgConnections.Name = "dgConnections";
            this.dgConnections.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgConnections.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgConnections.RowHeadersVisible = false;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dgConnections.RowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dgConnections.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgConnections.Size = new System.Drawing.Size(632, 92);
            this.dgConnections.TabIndex = 0;
            this.dgConnections.SelectionChanged += new System.EventHandler(this.dgConnections_SelectionChanged);
            this.dgConnections.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.dgConnections_MouseDoubleClick);
            // 
            // colPremiseCompany
            // 
            this.colPremiseCompany.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colPremiseCompany.DataPropertyName = "BackOfficeCompanyName";
            this.colPremiseCompany.HeaderText = "PluggedInProduct company";
            this.colPremiseCompany.Name = "colPremiseCompany";
            this.colPremiseCompany.ReadOnly = true;
            this.colPremiseCompany.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // colPremiseConnectionStatus
            // 
            this.colPremiseConnectionStatus.HeaderText = "";
            this.colPremiseConnectionStatus.MinimumWidth = 20;
            this.colPremiseConnectionStatus.Name = "colPremiseConnectionStatus";
            this.colPremiseConnectionStatus.ReadOnly = true;
            this.colPremiseConnectionStatus.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colPremiseConnectionStatus.Width = 20;
            // 
            // colConnectorLabel
            // 
            this.colConnectorLabel.HeaderText = "";
            this.colConnectorLabel.Image = global::SageConnector.Properties.Resources.arrow32x16;
            this.colConnectorLabel.Name = "colConnectorLabel";
            this.colConnectorLabel.ReadOnly = true;
            this.colConnectorLabel.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colConnectorLabel.Width = 65;
            // 
            // colTenantConnectionStatus
            // 
            this.colTenantConnectionStatus.HeaderText = "";
            this.colTenantConnectionStatus.MinimumWidth = 20;
            this.colTenantConnectionStatus.Name = "colTenantConnectionStatus";
            this.colTenantConnectionStatus.ReadOnly = true;
            this.colTenantConnectionStatus.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.colTenantConnectionStatus.Width = 20;
            // 
            // colTenantCompany
            // 
            this.colTenantCompany.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colTenantCompany.DataPropertyName = "CloudCompanyName";
            this.colTenantCompany.HeaderText = "Sage company";
            this.colTenantCompany.Name = "colTenantCompany";
            this.colTenantCompany.ReadOnly = true;
            this.colTenantCompany.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // chkPremiseActiveStatusReceive
            // 
            this.chkPremiseActiveStatusReceive.AutoSize = true;
            this.chkPremiseActiveStatusReceive.Enabled = false;
            this.chkPremiseActiveStatusReceive.Location = new System.Drawing.Point(123, 83);
            this.chkPremiseActiveStatusReceive.Name = "chkPremiseActiveStatusReceive";
            this.chkPremiseActiveStatusReceive.Size = new System.Drawing.Size(77, 17);
            this.chkPremiseActiveStatusReceive.TabIndex = 7;
            this.chkPremiseActiveStatusReceive.TabStop = false;
            this.chkPremiseActiveStatusReceive.Text = "To receive";
            this.chkPremiseActiveStatusReceive.UseVisualStyleBackColor = true;
            // 
            // lblPremiseActiveStatus
            // 
            this.lblPremiseActiveStatus.AutoSize = true;
            this.lblPremiseActiveStatus.Location = new System.Drawing.Point(9, 84);
            this.lblPremiseActiveStatus.Name = "lblPremiseActiveStatus";
            this.lblPremiseActiveStatus.Size = new System.Drawing.Size(105, 13);
            this.lblPremiseActiveStatus.TabIndex = 4;
            this.lblPremiseActiveStatus.Text = "Connection enabled:";
            // 
            // txtPremiseDatabasePath
            // 
            this.txtPremiseDatabasePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPremiseDatabasePath.Location = new System.Drawing.Point(123, 53);
            this.txtPremiseDatabasePath.Name = "txtPremiseDatabasePath";
            this.txtPremiseDatabasePath.ReadOnly = true;
            this.txtPremiseDatabasePath.Size = new System.Drawing.Size(496, 20);
            this.txtPremiseDatabasePath.TabIndex = 6;
            this.txtPremiseDatabasePath.TabStop = false;
            // 
            // lblPremiseDatabasePath
            // 
            this.lblPremiseDatabasePath.AutoSize = true;
            this.lblPremiseDatabasePath.Location = new System.Drawing.Point(9, 56);
            this.lblPremiseDatabasePath.Name = "lblPremiseDatabasePath";
            this.lblPremiseDatabasePath.Size = new System.Drawing.Size(89, 13);
            this.lblPremiseDatabasePath.TabIndex = 2;
            this.lblPremiseDatabasePath.Text = "Data connection:";
            // 
            // txtPremiseCompanyName
            // 
            this.txtPremiseCompanyName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPremiseCompanyName.Location = new System.Drawing.Point(123, 23);
            this.txtPremiseCompanyName.Name = "txtPremiseCompanyName";
            this.txtPremiseCompanyName.ReadOnly = true;
            this.txtPremiseCompanyName.Size = new System.Drawing.Size(496, 20);
            this.txtPremiseCompanyName.TabIndex = 5;
            this.txtPremiseCompanyName.TabStop = false;
            // 
            // lblPremiseCompanyName
            // 
            this.lblPremiseCompanyName.AutoSize = true;
            this.lblPremiseCompanyName.Location = new System.Drawing.Point(9, 26);
            this.lblPremiseCompanyName.Name = "lblPremiseCompanyName";
            this.lblPremiseCompanyName.Size = new System.Drawing.Size(83, 13);
            this.lblPremiseCompanyName.TabIndex = 0;
            this.lblPremiseCompanyName.Text = "Company name:";
            // 
            // btnDeleteConnection
            // 
            this.btnDeleteConnection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteConnection.Enabled = false;
            this.btnDeleteConnection.Location = new System.Drawing.Point(569, 131);
            this.btnDeleteConnection.Name = "btnDeleteConnection";
            this.btnDeleteConnection.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteConnection.TabIndex = 4;
            this.btnDeleteConnection.Text = global::SageConnector.Properties.Resources.ConnectorMain_ButtonDelete;
            this.btnDeleteConnection.UseVisualStyleBackColor = true;
            this.btnDeleteConnection.Click += new System.EventHandler(this.btnDeleteConnection_Click);
            // 
            // btnEditConnection
            // 
            this.btnEditConnection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEditConnection.Enabled = false;
            this.btnEditConnection.Location = new System.Drawing.Point(339, 131);
            this.btnEditConnection.Name = "btnEditConnection";
            this.btnEditConnection.Size = new System.Drawing.Size(75, 23);
            this.btnEditConnection.TabIndex = 3;
            this.btnEditConnection.Text = global::SageConnector.Properties.Resources.ConnectorMain_ButtonEdit;
            this.btnEditConnection.UseVisualStyleBackColor = true;
            this.btnEditConnection.Click += new System.EventHandler(this.btnEditConnection_Click);
            // 
            // btnAddConnection
            // 
            this.btnAddConnection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddConnection.Location = new System.Drawing.Point(258, 131);
            this.btnAddConnection.Name = "btnAddConnection";
            this.btnAddConnection.Size = new System.Drawing.Size(75, 23);
            this.btnAddConnection.TabIndex = 2;
            this.btnAddConnection.Text = global::SageConnector.Properties.Resources.ConnectorMain_ButtonAdd;
            this.btnAddConnection.UseVisualStyleBackColor = true;
            this.btnAddConnection.Click += new System.EventHandler(this.btnAddConnection_Click);
            // 
            // btnCloseConnector
            // 
            this.btnCloseConnector.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCloseConnector.Location = new System.Drawing.Point(569, 556);
            this.btnCloseConnector.Name = "btnCloseConnector";
            this.btnCloseConnector.Size = new System.Drawing.Size(75, 23);
            this.btnCloseConnector.TabIndex = 12;
            this.btnCloseConnector.Text = global::SageConnector.Properties.Resources.ConnectorMain_ButtonClose;
            this.btnCloseConnector.UseVisualStyleBackColor = true;
            this.btnCloseConnector.Click += new System.EventHandler(this.btnCloseConnector_Click);
            // 
            // lblVersion
            // 
            this.lblVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblVersion.AutoSize = true;
            this.lblVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVersion.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblVersion.Location = new System.Drawing.Point(12, 568);
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(33, 13);
            this.lblVersion.TabIndex = 13;
            this.lblVersion.Text = "Build:";
            // 
            // lblSkyfireConnections
            // 
            this.lblSkyfireConnections.AutoSize = true;
            this.lblSkyfireConnections.Location = new System.Drawing.Point(12, 13);
            this.lblSkyfireConnections.Name = "lblSkyfireConnections";
            this.lblSkyfireConnections.Size = new System.Drawing.Size(93, 13);
            this.lblSkyfireConnections.TabIndex = 14;
            this.lblSkyfireConnections.Text = "Sage connections";
            // 
            // lblLine
            // 
            this.lblLine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLine.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblLine.Location = new System.Drawing.Point(104, 20);
            this.lblLine.Name = "lblLine";
            this.lblLine.Size = new System.Drawing.Size(520, 2);
            this.lblLine.TabIndex = 15;
            // 
            // lnkHelpLink
            // 
            this.lnkHelpLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkHelpLink.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lnkHelpLink.Image = ((System.Drawing.Image)(resources.GetObject("lnkHelpLink.Image")));
            this.lnkHelpLink.LinkArea = new System.Windows.Forms.LinkArea(0, 4);
            this.lnkHelpLink.LinkColor = System.Drawing.Color.White;
            this.lnkHelpLink.Location = new System.Drawing.Point(627, 11);
            this.lnkHelpLink.Name = "lnkHelpLink";
            this.lnkHelpLink.Size = new System.Drawing.Size(17, 17);
            this.lnkHelpLink.TabIndex = 16;
            this.lnkHelpLink.TabStop = true;
            this.lnkHelpLink.Text = "    ";
            this.lnkHelpLink.VisitedLinkColor = System.Drawing.Color.White;
            this.lnkHelpLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkHelpLink_LinkClicked);
            // 
            // lblVersionNumber
            // 
            this.lblVersionNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblVersionNumber.AutoSize = true;
            this.lblVersionNumber.Location = new System.Drawing.Point(49, 568);
            this.lblVersionNumber.Name = "lblVersionNumber";
            this.lblVersionNumber.Size = new System.Drawing.Size(24, 13);
            this.lblVersionNumber.TabIndex = 18;
            this.lblVersionNumber.Text = "X.X";
            // 
            // grpCompanyInformation
            // 
            this.grpCompanyInformation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpCompanyInformation.Controls.Add(this.btnManageRequests);
            this.grpCompanyInformation.Controls.Add(this.lblPremiseCompanyName);
            this.grpCompanyInformation.Controls.Add(this.chkPremiseActiveStatusReceive);
            this.grpCompanyInformation.Controls.Add(this.txtPremiseCompanyName);
            this.grpCompanyInformation.Controls.Add(this.lblPremiseActiveStatus);
            this.grpCompanyInformation.Controls.Add(this.lblPremiseDatabasePath);
            this.grpCompanyInformation.Controls.Add(this.txtPremiseDatabasePath);
            this.grpCompanyInformation.Location = new System.Drawing.Point(12, 169);
            this.grpCompanyInformation.Name = "grpCompanyInformation";
            this.grpCompanyInformation.Size = new System.Drawing.Size(632, 116);
            this.grpCompanyInformation.TabIndex = 8;
            this.grpCompanyInformation.TabStop = false;
            this.grpCompanyInformation.Text = "PluggedInProduct company";
            // 
            // btnManageRequests
            // 
            this.btnManageRequests.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnManageRequests.Location = new System.Drawing.Point(513, 79);
            this.btnManageRequests.Name = "btnManageRequests";
            this.btnManageRequests.Size = new System.Drawing.Size(106, 23);
            this.btnManageRequests.TabIndex = 8;
            this.btnManageRequests.Text = "Cance&l requests";
            this.btnManageRequests.UseVisualStyleBackColor = true;
            this.btnManageRequests.Click += new System.EventHandler(this.btnManageRequests_Click);
            // 
            // grpTenantInformation
            // 
            this.grpTenantInformation.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpTenantInformation.Controls.Add(this.lnkTenantSiteUrl);
            this.grpTenantInformation.Controls.Add(this.chkTenantActiveStatusSend);
            this.grpTenantInformation.Controls.Add(this.lblTenantCompanyName);
            this.grpTenantInformation.Controls.Add(this.chkTenantActiveStatusReceive);
            this.grpTenantInformation.Controls.Add(this.txtTenantCompanyName);
            this.grpTenantInformation.Controls.Add(this.lblTenantActiveStatus);
            this.grpTenantInformation.Controls.Add(this.lblTenantDatabasePath);
            this.grpTenantInformation.Location = new System.Drawing.Point(12, 300);
            this.grpTenantInformation.Name = "grpTenantInformation";
            this.grpTenantInformation.Size = new System.Drawing.Size(632, 136);
            this.grpTenantInformation.TabIndex = 12;
            this.grpTenantInformation.TabStop = false;
            this.grpTenantInformation.Text = "Sage connection details";
            // 
            // lnkTenantSiteUrl
            // 
            this.lnkTenantSiteUrl.Location = new System.Drawing.Point(120, 58);
            this.lnkTenantSiteUrl.Name = "lnkTenantSiteUrl";
            this.lnkTenantSiteUrl.Size = new System.Drawing.Size(499, 19);
            this.lnkTenantSiteUrl.TabIndex = 13;
            this.lnkTenantSiteUrl.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkTenantSiteUrl_LinkClicked);
            // 
            // chkTenantActiveStatusSend
            // 
            this.chkTenantActiveStatusSend.AutoSize = true;
            this.chkTenantActiveStatusSend.Enabled = false;
            this.chkTenantActiveStatusSend.Location = new System.Drawing.Point(124, 108);
            this.chkTenantActiveStatusSend.Name = "chkTenantActiveStatusSend";
            this.chkTenantActiveStatusSend.Size = new System.Drawing.Size(65, 17);
            this.chkTenantActiveStatusSend.TabIndex = 12;
            this.chkTenantActiveStatusSend.TabStop = false;
            this.chkTenantActiveStatusSend.Text = "To send";
            this.chkTenantActiveStatusSend.UseVisualStyleBackColor = true;
            // 
            // lblTenantCompanyName
            // 
            this.lblTenantCompanyName.AutoSize = true;
            this.lblTenantCompanyName.Location = new System.Drawing.Point(10, 28);
            this.lblTenantCompanyName.Name = "lblTenantCompanyName";
            this.lblTenantCompanyName.Size = new System.Drawing.Size(83, 13);
            this.lblTenantCompanyName.TabIndex = 0;
            this.lblTenantCompanyName.Text = "Company name:";
            // 
            // chkTenantActiveStatusReceive
            // 
            this.chkTenantActiveStatusReceive.AutoSize = true;
            this.chkTenantActiveStatusReceive.Enabled = false;
            this.chkTenantActiveStatusReceive.Location = new System.Drawing.Point(124, 85);
            this.chkTenantActiveStatusReceive.Name = "chkTenantActiveStatusReceive";
            this.chkTenantActiveStatusReceive.Size = new System.Drawing.Size(77, 17);
            this.chkTenantActiveStatusReceive.TabIndex = 11;
            this.chkTenantActiveStatusReceive.TabStop = false;
            this.chkTenantActiveStatusReceive.Text = "To receive";
            this.chkTenantActiveStatusReceive.UseVisualStyleBackColor = true;
            // 
            // txtTenantCompanyName
            // 
            this.txtTenantCompanyName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTenantCompanyName.Location = new System.Drawing.Point(124, 25);
            this.txtTenantCompanyName.Name = "txtTenantCompanyName";
            this.txtTenantCompanyName.ReadOnly = true;
            this.txtTenantCompanyName.Size = new System.Drawing.Size(495, 20);
            this.txtTenantCompanyName.TabIndex = 9;
            this.txtTenantCompanyName.TabStop = false;
            // 
            // lblTenantActiveStatus
            // 
            this.lblTenantActiveStatus.AutoSize = true;
            this.lblTenantActiveStatus.Location = new System.Drawing.Point(10, 86);
            this.lblTenantActiveStatus.Name = "lblTenantActiveStatus";
            this.lblTenantActiveStatus.Size = new System.Drawing.Size(105, 13);
            this.lblTenantActiveStatus.TabIndex = 4;
            this.lblTenantActiveStatus.Text = "Connection enabled:";
            // 
            // lblTenantDatabasePath
            // 
            this.lblTenantDatabasePath.AutoSize = true;
            this.lblTenantDatabasePath.Location = new System.Drawing.Point(10, 58);
            this.lblTenantDatabasePath.Name = "lblTenantDatabasePath";
            this.lblTenantDatabasePath.Size = new System.Drawing.Size(68, 13);
            this.lblTenantDatabasePath.TabIndex = 2;
            this.lblTenantDatabasePath.Text = "Site address:";
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefresh.Location = new System.Drawing.Point(488, 556);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 19;
            this.btnRefresh.Text = "&Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // grpConnector
            // 
            this.grpConnector.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpConnector.Controls.Add(this.tbSystemMessages);
            this.grpConnector.Location = new System.Drawing.Point(12, 451);
            this.grpConnector.Name = "grpConnector";
            this.grpConnector.Size = new System.Drawing.Size(632, 91);
            this.grpConnector.TabIndex = 20;
            this.grpConnector.TabStop = false;
            this.grpConnector.Text = "System messages";
            // 
            // tbSystemMessages
            // 
            this.tbSystemMessages.CausesValidation = false;
            this.tbSystemMessages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbSystemMessages.Location = new System.Drawing.Point(3, 16);
            this.tbSystemMessages.Multiline = true;
            this.tbSystemMessages.Name = "tbSystemMessages";
            this.tbSystemMessages.ReadOnly = true;
            this.tbSystemMessages.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbSystemMessages.Size = new System.Drawing.Size(626, 72);
            this.tbSystemMessages.TabIndex = 14;
            // 
            // lnkConnectorUpdate
            // 
            this.lnkConnectorUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lnkConnectorUpdate.AutoSize = true;
            this.lnkConnectorUpdate.Location = new System.Drawing.Point(216, 555);
            this.lnkConnectorUpdate.Name = "lnkConnectorUpdate";
            this.lnkConnectorUpdate.Size = new System.Drawing.Size(133, 13);
            this.lnkConnectorUpdate.TabIndex = 21;
            this.lnkConnectorUpdate.TabStop = true;
            this.lnkConnectorUpdate.Text = "Connector update required";
            this.lnkConnectorUpdate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lnkConnectorUpdate.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkConnectorUpdate_LinkClicked);
            // 
            // lnkBackofficeUpdate
            // 
            this.lnkBackofficeUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lnkBackofficeUpdate.AutoSize = true;
            this.lnkBackofficeUpdate.Location = new System.Drawing.Point(216, 568);
            this.lnkBackofficeUpdate.Name = "lnkBackofficeUpdate";
            this.lnkBackofficeUpdate.Size = new System.Drawing.Size(138, 13);
            this.lnkBackofficeUpdate.TabIndex = 22;
            this.lnkBackofficeUpdate.TabStop = true;
            this.lnkBackofficeUpdate.Text = "Back office update required";
            this.lnkBackofficeUpdate.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lnkBackofficeUpdate.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkBackofficeUpdate_LinkClicked);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 555);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 23;
            this.label2.Text = "Last refresh:";
            // 
            // lblLastRefresh
            // 
            this.lblLastRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblLastRefresh.AutoSize = true;
            this.lblLastRefresh.Location = new System.Drawing.Point(74, 555);
            this.lblLastRefresh.Name = "lblLastRefresh";
            this.lblLastRefresh.Size = new System.Drawing.Size(98, 13);
            this.lblLastRefresh.TabIndex = 24;
            this.lblLastRefresh.Text = "12/2/1492 4:20:00";
            // 
            // lnkBackofficeInstall
            // 
            this.lnkBackofficeInstall.AutoSize = true;
            this.lnkBackofficeInstall.Location = new System.Drawing.Point(216, 543);
            this.lnkBackofficeInstall.Name = "lnkBackofficeInstall";
            this.lnkBackofficeInstall.Size = new System.Drawing.Size(131, 13);
            this.lnkBackofficeInstall.TabIndex = 25;
            this.lnkBackofficeInstall.TabStop = true;
            this.lnkBackofficeInstall.Text = "Back office install required";
            this.lnkBackofficeInstall.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkBackofficeInstall_LinkClicked);
            // 
            // btnFeatureConfigurations
            // 
            this.btnFeatureConfigurations.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFeatureConfigurations.Location = new System.Drawing.Point(420, 131);
            this.btnFeatureConfigurations.Name = "btnFeatureConfigurations";
            this.btnFeatureConfigurations.Size = new System.Drawing.Size(143, 23);
            this.btnFeatureConfigurations.TabIndex = 26;
            this.btnFeatureConfigurations.Text = "&Feature Configurations...";
            this.btnFeatureConfigurations.UseVisualStyleBackColor = true;
            this.btnFeatureConfigurations.Click += new System.EventHandler(this.btnFeatureConfigurations_Click);
            // 
            // CloudConnectorMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(656, 590);
            this.Controls.Add(this.btnFeatureConfigurations);
            this.Controls.Add(this.lnkBackofficeInstall);
            this.Controls.Add(this.lblLastRefresh);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.lnkBackofficeUpdate);
            this.Controls.Add(this.lnkConnectorUpdate);
            this.Controls.Add(this.grpConnector);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.grpTenantInformation);
            this.Controls.Add(this.grpCompanyInformation);
            this.Controls.Add(this.lblVersionNumber);
            this.Controls.Add(this.lnkHelpLink);
            this.Controls.Add(this.lblLine);
            this.Controls.Add(this.lblSkyfireConnections);
            this.Controls.Add(this.btnAddConnection);
            this.Controls.Add(this.lblVersion);
            this.Controls.Add(this.btnEditConnection);
            this.Controls.Add(this.btnDeleteConnection);
            this.Controls.Add(this.dgConnections);
            this.Controls.Add(this.btnCloseConnector);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(672, 628);
            this.Name = "CloudConnectorMainForm";
            this.Text = "Sage Connector";
            ((System.ComponentModel.ISupportInitialize)(this.dgConnections)).EndInit();
            this.grpCompanyInformation.ResumeLayout(false);
            this.grpCompanyInformation.PerformLayout();
            this.grpTenantInformation.ResumeLayout(false);
            this.grpTenantInformation.PerformLayout();
            this.grpConnector.ResumeLayout(false);
            this.grpConnector.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgConnections;
        private System.Windows.Forms.CheckBox chkPremiseActiveStatusReceive;
        private System.Windows.Forms.Label lblPremiseActiveStatus;
        private System.Windows.Forms.TextBox txtPremiseDatabasePath;
        private System.Windows.Forms.Label lblPremiseDatabasePath;
        private System.Windows.Forms.TextBox txtPremiseCompanyName;
        private System.Windows.Forms.Label lblPremiseCompanyName;
        private System.Windows.Forms.Button btnAddConnection;
        private System.Windows.Forms.Button btnEditConnection;
        private System.Windows.Forms.Button btnDeleteConnection;
        private System.Windows.Forms.Button btnCloseConnector;
        private System.Windows.Forms.Label lblVersion;
        private System.Windows.Forms.Label lblSkyfireConnections;
        private System.Windows.Forms.Label lblLine;
        private System.Windows.Forms.LinkLabel lnkHelpLink;
        private System.Windows.Forms.Label lblVersionNumber;
        private System.Windows.Forms.GroupBox grpCompanyInformation;
        private System.Windows.Forms.GroupBox grpTenantInformation;
        private System.Windows.Forms.Label lblTenantCompanyName;
        private System.Windows.Forms.CheckBox chkTenantActiveStatusReceive;
        private System.Windows.Forms.TextBox txtTenantCompanyName;
        private System.Windows.Forms.Label lblTenantActiveStatus;
        private System.Windows.Forms.Label lblTenantDatabasePath;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.GroupBox grpConnector;
        private System.Windows.Forms.LinkLabel lnkConnectorUpdate;
        private System.Windows.Forms.LinkLabel lnkBackofficeUpdate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblLastRefresh;
        private System.Windows.Forms.CheckBox chkTenantActiveStatusSend;
        private System.Windows.Forms.LinkLabel lnkTenantSiteUrl;
        private System.Windows.Forms.Button btnManageRequests;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPremiseCompany;
        private System.Windows.Forms.DataGridViewImageColumn colPremiseConnectionStatus;
        private System.Windows.Forms.DataGridViewImageColumn colConnectorLabel;
        private System.Windows.Forms.DataGridViewImageColumn colTenantConnectionStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTenantCompany;
        private System.Windows.Forms.TextBox tbSystemMessages;
        private System.Windows.Forms.LinkLabel lnkBackofficeInstall;
        private System.Windows.Forms.Button btnFeatureConfigurations;
    }
}