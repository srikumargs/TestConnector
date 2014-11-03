using System.Collections.Generic;
using Newtonsoft.Json;
using Sage.Connector.Configuration.Contracts.Data;
using Sage.Connector.Configuration.Mediator.JsonConverters;
using Sage.Connector.DomainMediator.Core.JsonConverters;
using SageConnector.Internal;

namespace SageConnector
{
    partial class CloudConnectorDetailForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CloudConnectorDetailForm));
            this.lnkHelpLink = new System.Windows.Forms.LinkLabel();
            this.chkPremiseEnabledReceive = new System.Windows.Forms.CheckBox();
            this.btnPremiseTestConnection = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txtMarker = new System.Windows.Forms.TextBox();
            this.lblMarker = new System.Windows.Forms.Label();
            this.chkTenantEnabledReceive = new System.Windows.Forms.CheckBox();
            this.btnTenantTestConnection = new System.Windows.Forms.Button();
            this.lblPremiseActiveStatus = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtTenantCompanyName = new System.Windows.Forms.TextBox();
            this.lblTenantDatabasePath = new System.Windows.Forms.Label();
            this.txtTenantConnectionKey = new System.Windows.Forms.TextBox();
            this.lblTenantCompanyName = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.pnlEndpoint = new System.Windows.Forms.Panel();
            this.txtEndpointAddress = new System.Windows.Forms.TextBox();
            this.lblEndpointAddress = new System.Windows.Forms.Label();
            this.grpTenantDetails = new System.Windows.Forms.GroupBox();
            this.lnkTenantSiteUrl = new System.Windows.Forms.LinkLabel();
            this.chkTenantEnabledSend = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.grpPremiseDetails = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.pnlEndpoint.SuspendLayout();
            this.grpTenantDetails.SuspendLayout();
            this.grpPremiseDetails.SuspendLayout();
            this.SuspendLayout();
            // 
            // lnkHelpLink
            // 
            this.lnkHelpLink.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lnkHelpLink.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lnkHelpLink.Image = ((System.Drawing.Image)(resources.GetObject("lnkHelpLink.Image")));
            this.lnkHelpLink.LinkArea = new System.Windows.Forms.LinkArea(0, 4);
            this.lnkHelpLink.LinkColor = System.Drawing.Color.White;
            this.lnkHelpLink.Location = new System.Drawing.Point(705, 9);
            this.lnkHelpLink.Name = "lnkHelpLink";
            this.lnkHelpLink.Size = new System.Drawing.Size(17, 17);
            this.lnkHelpLink.TabIndex = 5;
            this.lnkHelpLink.TabStop = true;
            this.lnkHelpLink.Text = "          ";
            this.lnkHelpLink.UseCompatibleTextRendering = true;
            this.lnkHelpLink.VisitedLinkColor = System.Drawing.Color.White;
            this.lnkHelpLink.Click += new System.EventHandler(this.lnkHelpLink_Click);
            // 
            // chkPremiseEnabledReceive
            // 
            this.chkPremiseEnabledReceive.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chkPremiseEnabledReceive.AutoSize = true;
            this.chkPremiseEnabledReceive.Location = new System.Drawing.Point(133, 286);
            this.chkPremiseEnabledReceive.Name = "chkPremiseEnabledReceive";
            this.chkPremiseEnabledReceive.Size = new System.Drawing.Size(77, 17);
            this.chkPremiseEnabledReceive.TabIndex = 7;
            this.chkPremiseEnabledReceive.Text = "To receive";
            this.chkPremiseEnabledReceive.UseVisualStyleBackColor = true;
            // 
            // btnPremiseTestConnection
            // 
            this.btnPremiseTestConnection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPremiseTestConnection.Location = new System.Drawing.Point(133, 257);
            this.btnPremiseTestConnection.Name = "btnPremiseTestConnection";
            this.btnPremiseTestConnection.Size = new System.Drawing.Size(100, 23);
            this.btnPremiseTestConnection.TabIndex = 6;
            this.btnPremiseTestConnection.Text = "Test connection";
            this.btnPremiseTestConnection.UseVisualStyleBackColor = true;
            this.btnPremiseTestConnection.Click += new System.EventHandler(this.btnPremiseTestConnection_Click);
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(19, 287);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(99, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Enable connection:";
            // 
            // txtMarker
            // 
            this.txtMarker.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMarker.Location = new System.Drawing.Point(133, 39);
            this.txtMarker.Name = "txtMarker";
            this.txtMarker.PasswordChar = '*';
            this.txtMarker.Size = new System.Drawing.Size(565, 20);
            this.txtMarker.TabIndex = 5;
            this.txtMarker.Visible = false;
            // 
            // lblMarker
            // 
            this.lblMarker.Location = new System.Drawing.Point(19, 42);
            this.lblMarker.Name = "lblMarker";
            this.lblMarker.Size = new System.Drawing.Size(100, 13);
            this.lblMarker.TabIndex = 0;
            this.lblMarker.Text = "labelMarker";
            this.lblMarker.Visible = false;
            // 
            // chkTenantEnabledReceive
            // 
            this.chkTenantEnabledReceive.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chkTenantEnabledReceive.AutoSize = true;
            this.chkTenantEnabledReceive.Location = new System.Drawing.Point(133, 192);
            this.chkTenantEnabledReceive.Name = "chkTenantEnabledReceive";
            this.chkTenantEnabledReceive.Size = new System.Drawing.Size(77, 17);
            this.chkTenantEnabledReceive.TabIndex = 5;
            this.chkTenantEnabledReceive.Text = "To receive";
            this.chkTenantEnabledReceive.UseVisualStyleBackColor = true;
            // 
            // btnTenantTestConnection
            // 
            this.btnTenantTestConnection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTenantTestConnection.Location = new System.Drawing.Point(133, 162);
            this.btnTenantTestConnection.Name = "btnTenantTestConnection";
            this.btnTenantTestConnection.Size = new System.Drawing.Size(100, 23);
            this.btnTenantTestConnection.TabIndex = 4;
            this.btnTenantTestConnection.Text = "Test connection";
            this.btnTenantTestConnection.UseVisualStyleBackColor = true;
            this.btnTenantTestConnection.Click += new System.EventHandler(this.btnTenantTestConnection_Click);
            // 
            // lblPremiseActiveStatus
            // 
            this.lblPremiseActiveStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPremiseActiveStatus.AutoSize = true;
            this.lblPremiseActiveStatus.Location = new System.Drawing.Point(19, 192);
            this.lblPremiseActiveStatus.Name = "lblPremiseActiveStatus";
            this.lblPremiseActiveStatus.Size = new System.Drawing.Size(99, 13);
            this.lblPremiseActiveStatus.TabIndex = 0;
            this.lblPremiseActiveStatus.Text = "Enable connection:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(19, 140);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Site address:";
            // 
            // txtTenantCompanyName
            // 
            this.txtTenantCompanyName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTenantCompanyName.Location = new System.Drawing.Point(133, 110);
            this.txtTenantCompanyName.Name = "txtTenantCompanyName";
            this.txtTenantCompanyName.ReadOnly = true;
            this.txtTenantCompanyName.Size = new System.Drawing.Size(565, 20);
            this.txtTenantCompanyName.TabIndex = 0;
            this.txtTenantCompanyName.TabStop = false;
            // 
            // lblTenantDatabasePath
            // 
            this.lblTenantDatabasePath.AutoSize = true;
            this.lblTenantDatabasePath.Location = new System.Drawing.Point(19, 113);
            this.lblTenantDatabasePath.Name = "lblTenantDatabasePath";
            this.lblTenantDatabasePath.Size = new System.Drawing.Size(83, 13);
            this.lblTenantDatabasePath.TabIndex = 0;
            this.lblTenantDatabasePath.Text = "Company name:";
            // 
            // txtTenantConnectionKey
            // 
            this.txtTenantConnectionKey.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTenantConnectionKey.Location = new System.Drawing.Point(133, 51);
            this.txtTenantConnectionKey.Multiline = true;
            this.txtTenantConnectionKey.Name = "txtTenantConnectionKey";
            this.txtTenantConnectionKey.Size = new System.Drawing.Size(565, 50);
            this.txtTenantConnectionKey.TabIndex = 3;
            // 
            // lblTenantCompanyName
            // 
            this.lblTenantCompanyName.AutoSize = true;
            this.lblTenantCompanyName.Location = new System.Drawing.Point(19, 54);
            this.lblTenantCompanyName.Name = "lblTenantCompanyName";
            this.lblTenantCompanyName.Size = new System.Drawing.Size(84, 13);
            this.lblTenantCompanyName.TabIndex = 0;
            this.lblTenantCompanyName.Text = "Connection key:";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(647, 607);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = global::SageConnector.Properties.Resources.ConnectorDetails_ButtonCancel;
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSave.Location = new System.Drawing.Point(566, 607);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = global::SageConnector.Properties.Resources.ConnectorDetails_ButtonSave;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // pnlEndpoint
            // 
            this.pnlEndpoint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlEndpoint.Controls.Add(this.txtEndpointAddress);
            this.pnlEndpoint.Controls.Add(this.lblEndpointAddress);
            this.pnlEndpoint.Location = new System.Drawing.Point(14, 1);
            this.pnlEndpoint.Name = "pnlEndpoint";
            this.pnlEndpoint.Size = new System.Drawing.Size(686, 28);
            this.pnlEndpoint.TabIndex = 0;
            this.pnlEndpoint.Visible = false;
            // 
            // txtEndpointAddress
            // 
            this.txtEndpointAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEndpointAddress.Location = new System.Drawing.Point(130, 5);
            this.txtEndpointAddress.Name = "txtEndpointAddress";
            this.txtEndpointAddress.Size = new System.Drawing.Size(548, 20);
            this.txtEndpointAddress.TabIndex = 1;
            // 
            // lblEndpointAddress
            // 
            this.lblEndpointAddress.AutoSize = true;
            this.lblEndpointAddress.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEndpointAddress.Location = new System.Drawing.Point(7, 8);
            this.lblEndpointAddress.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.lblEndpointAddress.Name = "lblEndpointAddress";
            this.lblEndpointAddress.Size = new System.Drawing.Size(68, 13);
            this.lblEndpointAddress.TabIndex = 0;
            this.lblEndpointAddress.Text = "Site address:";
            // 
            // grpTenantDetails
            // 
            this.grpTenantDetails.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpTenantDetails.Controls.Add(this.lnkTenantSiteUrl);
            this.grpTenantDetails.Controls.Add(this.chkTenantEnabledSend);
            this.grpTenantDetails.Controls.Add(this.label8);
            this.grpTenantDetails.Controls.Add(this.lblTenantCompanyName);
            this.grpTenantDetails.Controls.Add(this.chkTenantEnabledReceive);
            this.grpTenantDetails.Controls.Add(this.txtTenantConnectionKey);
            this.grpTenantDetails.Controls.Add(this.btnTenantTestConnection);
            this.grpTenantDetails.Controls.Add(this.lblTenantDatabasePath);
            this.grpTenantDetails.Controls.Add(this.lblPremiseActiveStatus);
            this.grpTenantDetails.Controls.Add(this.txtTenantCompanyName);
            this.grpTenantDetails.Controls.Add(this.label3);
            this.grpTenantDetails.Location = new System.Drawing.Point(11, 355);
            this.grpTenantDetails.Name = "grpTenantDetails";
            this.grpTenantDetails.Size = new System.Drawing.Size(710, 241);
            this.grpTenantDetails.TabIndex = 2;
            this.grpTenantDetails.TabStop = false;
            this.grpTenantDetails.Text = "Sage connection details";
            // 
            // lnkTenantSiteUrl
            // 
            this.lnkTenantSiteUrl.Location = new System.Drawing.Point(130, 140);
            this.lnkTenantSiteUrl.Name = "lnkTenantSiteUrl";
            this.lnkTenantSiteUrl.Size = new System.Drawing.Size(565, 19);
            this.lnkTenantSiteUrl.TabIndex = 10;
            this.lnkTenantSiteUrl.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkTenantSiteUrl_LinkClicked);
            // 
            // chkTenantEnabledSend
            // 
            this.chkTenantEnabledSend.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chkTenantEnabledSend.AutoSize = true;
            this.chkTenantEnabledSend.Location = new System.Drawing.Point(133, 215);
            this.chkTenantEnabledSend.Name = "chkTenantEnabledSend";
            this.chkTenantEnabledSend.Size = new System.Drawing.Size(65, 17);
            this.chkTenantEnabledSend.TabIndex = 7;
            this.chkTenantEnabledSend.Text = "To send";
            this.chkTenantEnabledSend.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(133, 19);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(562, 26);
            this.label8.TabIndex = 6;
            this.label8.Text = "Paste the connection key that was created on your Sage site (under Administration" +
    " > Back Office Connection).";
            // 
            // grpPremiseDetails
            // 
            this.grpPremiseDetails.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpPremiseDetails.Controls.Add(this.label7);
            this.grpPremiseDetails.Controls.Add(this.chkPremiseEnabledReceive);
            this.grpPremiseDetails.Controls.Add(this.btnPremiseTestConnection);
            this.grpPremiseDetails.Controls.Add(this.label4);
            this.grpPremiseDetails.Controls.Add(this.txtMarker);
            this.grpPremiseDetails.Controls.Add(this.lblMarker);
            this.grpPremiseDetails.Location = new System.Drawing.Point(11, 27);
            this.grpPremiseDetails.Name = "grpPremiseDetails";
            this.grpPremiseDetails.Size = new System.Drawing.Size(710, 308);
            this.grpPremiseDetails.TabIndex = 1;
            this.grpPremiseDetails.TabStop = false;
            this.grpPremiseDetails.Text = "{0} connection details";
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(133, 17);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(565, 13);
            this.label7.TabIndex = 8;
            this.label7.Text = "Enter the credentials for your back office security.";
            // 
            // CloudConnectorDetailForm
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(734, 641);
            this.Controls.Add(this.grpPremiseDetails);
            this.Controls.Add(this.grpTenantDetails);
            this.Controls.Add(this.pnlEndpoint);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lnkHelpLink);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(750, 800);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(750, 576);
            this.Name = "CloudConnectorDetailForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Sage Connection Details";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CloudConnectorDetailForm_FormClosing);
            this.Load += new System.EventHandler(this.CloudConnectorDetailForm_Load);
            this.Shown += new System.EventHandler(this.CloudConnectorDetailForm_Shown);
            this.pnlEndpoint.ResumeLayout(false);
            this.pnlEndpoint.PerformLayout();
            this.grpTenantDetails.ResumeLayout(false);
            this.grpTenantDetails.PerformLayout();
            this.grpPremiseDetails.ResumeLayout(false);
            this.grpPremiseDetails.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.LinkLabel lnkHelpLink;
        private System.Windows.Forms.TextBox txtMarker;
        private System.Windows.Forms.Label lblMarker;
        private System.Windows.Forms.TextBox txtTenantCompanyName;
        private System.Windows.Forms.Label lblTenantDatabasePath;
        private System.Windows.Forms.TextBox txtTenantConnectionKey;
        private System.Windows.Forms.Label lblTenantCompanyName;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnTenantTestConnection;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkPremiseEnabledReceive;
        private System.Windows.Forms.Button btnPremiseTestConnection;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkTenantEnabledReceive;
        private System.Windows.Forms.Label lblPremiseActiveStatus;
        private System.Windows.Forms.Panel pnlEndpoint;
        private System.Windows.Forms.TextBox txtEndpointAddress;
        private System.Windows.Forms.Label lblEndpointAddress;
        private System.Windows.Forms.GroupBox grpTenantDetails;
        private System.Windows.Forms.GroupBox grpPremiseDetails;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.CheckBox chkTenantEnabledSend;
        private System.Windows.Forms.LinkLabel lnkTenantSiteUrl;

    }
}