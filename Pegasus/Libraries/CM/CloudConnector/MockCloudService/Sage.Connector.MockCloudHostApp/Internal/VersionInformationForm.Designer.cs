using System.CodeDom.Compiler;

namespace Sage.Connector.MockCloudHostApp
{

    partial class VersionInformationForm
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
        [GeneratedCodeAttribute("WinForms", "4.0.0.0")]
        private void InitializeComponent()
        {
            this.txtCurProductVersion = new System.Windows.Forms.TextBox();
            this.lblPeakMaxInterval = new System.Windows.Forms.Label();
            this.txtMinProductVersion = new System.Windows.Forms.TextBox();
            this.lblPeakMinInterval = new System.Windows.Forms.Label();
            this.txtCurInterfaceVersion = new System.Windows.Forms.TextBox();
            this.lblPeakBatchCount = new System.Windows.Forms.Label();
            this.txtMinInterfaceVersion = new System.Windows.Forms.TextBox();
            this.lblPeakThreshold = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.txtRequiredDesc = new System.Windows.Forms.TextBox();
            this.lblRequiredDesc = new System.Windows.Forms.Label();
            this.txtRequiredLink = new System.Windows.Forms.TextBox();
            this.lblRequiredLink = new System.Windows.Forms.Label();
            this.txtAvailableDesc = new System.Windows.Forms.TextBox();
            this.lblAvailableDesc = new System.Windows.Forms.Label();
            this.txtAvailableLink = new System.Windows.Forms.TextBox();
            this.lblAvailableLink = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtCurProductVersion
            // 
            this.txtCurProductVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCurProductVersion.Location = new System.Drawing.Point(242, 84);
            this.txtCurProductVersion.Name = "txtCurProductVersion";
            this.txtCurProductVersion.Size = new System.Drawing.Size(316, 20);
            this.txtCurProductVersion.TabIndex = 62;
            // 
            // lblPeakMaxInterval
            // 
            this.lblPeakMaxInterval.AutoSize = true;
            this.lblPeakMaxInterval.Location = new System.Drawing.Point(12, 87);
            this.lblPeakMaxInterval.Name = "lblPeakMaxInterval";
            this.lblPeakMaxInterval.Size = new System.Drawing.Size(174, 13);
            this.lblPeakMaxInterval.TabIndex = 68;
            this.lblPeakMaxInterval.Text = "Current Connector Product Version:";
            // 
            // txtMinProductVersion
            // 
            this.txtMinProductVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMinProductVersion.Location = new System.Drawing.Point(242, 58);
            this.txtMinProductVersion.Name = "txtMinProductVersion";
            this.txtMinProductVersion.Size = new System.Drawing.Size(316, 20);
            this.txtMinProductVersion.TabIndex = 61;
            // 
            // lblPeakMinInterval
            // 
            this.lblPeakMinInterval.AutoSize = true;
            this.lblPeakMinInterval.Location = new System.Drawing.Point(12, 61);
            this.lblPeakMinInterval.Name = "lblPeakMinInterval";
            this.lblPeakMinInterval.Size = new System.Drawing.Size(181, 13);
            this.lblPeakMinInterval.TabIndex = 67;
            this.lblPeakMinInterval.Text = "Minimum Connector Product Version:";
            // 
            // txtCurInterfaceVersion
            // 
            this.txtCurInterfaceVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtCurInterfaceVersion.Location = new System.Drawing.Point(242, 32);
            this.txtCurInterfaceVersion.Name = "txtCurInterfaceVersion";
            this.txtCurInterfaceVersion.Size = new System.Drawing.Size(316, 20);
            this.txtCurInterfaceVersion.TabIndex = 60;
            // 
            // lblPeakBatchCount
            // 
            this.lblPeakBatchCount.AutoSize = true;
            this.lblPeakBatchCount.Location = new System.Drawing.Point(12, 35);
            this.lblPeakBatchCount.Name = "lblPeakBatchCount";
            this.lblPeakBatchCount.Size = new System.Drawing.Size(157, 13);
            this.lblPeakBatchCount.TabIndex = 66;
            this.lblPeakBatchCount.Text = "Current Cloud Interface Version:";
            // 
            // txtMinInterfaceVersion
            // 
            this.txtMinInterfaceVersion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMinInterfaceVersion.Location = new System.Drawing.Point(242, 6);
            this.txtMinInterfaceVersion.Name = "txtMinInterfaceVersion";
            this.txtMinInterfaceVersion.Size = new System.Drawing.Size(316, 20);
            this.txtMinInterfaceVersion.TabIndex = 59;
            // 
            // lblPeakThreshold
            // 
            this.lblPeakThreshold.AutoSize = true;
            this.lblPeakThreshold.Location = new System.Drawing.Point(12, 9);
            this.lblPeakThreshold.Name = "lblPeakThreshold";
            this.lblPeakThreshold.Size = new System.Drawing.Size(164, 13);
            this.lblPeakThreshold.TabIndex = 65;
            this.lblPeakThreshold.Text = "Minimum Cloud Interface Version:";
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(483, 228);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 68;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOk
            // 
            this.btnOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOk.Location = new System.Drawing.Point(402, 228);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 67;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // txtRequiredDesc
            // 
            this.txtRequiredDesc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRequiredDesc.Location = new System.Drawing.Point(242, 162);
            this.txtRequiredDesc.Name = "txtRequiredDesc";
            this.txtRequiredDesc.Size = new System.Drawing.Size(316, 20);
            this.txtRequiredDesc.TabIndex = 65;
            // 
            // lblRequiredDesc
            // 
            this.lblRequiredDesc.AutoSize = true;
            this.lblRequiredDesc.Location = new System.Drawing.Point(12, 165);
            this.lblRequiredDesc.Name = "lblRequiredDesc";
            this.lblRequiredDesc.Size = new System.Drawing.Size(153, 13);
            this.lblRequiredDesc.TabIndex = 70;
            this.lblRequiredDesc.Text = "Upgrade Required Description:";
            // 
            // txtRequiredLink
            // 
            this.txtRequiredLink.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtRequiredLink.Location = new System.Drawing.Point(242, 188);
            this.txtRequiredLink.Name = "txtRequiredLink";
            this.txtRequiredLink.Size = new System.Drawing.Size(316, 20);
            this.txtRequiredLink.TabIndex = 66;
            // 
            // lblRequiredLink
            // 
            this.lblRequiredLink.AutoSize = true;
            this.lblRequiredLink.Location = new System.Drawing.Point(12, 191);
            this.lblRequiredLink.Name = "lblRequiredLink";
            this.lblRequiredLink.Size = new System.Drawing.Size(120, 13);
            this.lblRequiredLink.TabIndex = 72;
            this.lblRequiredLink.Text = "Upgrade Required Link:";
            // 
            // txtAvailableDesc
            // 
            this.txtAvailableDesc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAvailableDesc.Location = new System.Drawing.Point(242, 110);
            this.txtAvailableDesc.Name = "txtAvailableDesc";
            this.txtAvailableDesc.Size = new System.Drawing.Size(316, 20);
            this.txtAvailableDesc.TabIndex = 63;
            // 
            // lblAvailableDesc
            // 
            this.lblAvailableDesc.AutoSize = true;
            this.lblAvailableDesc.Location = new System.Drawing.Point(12, 113);
            this.lblAvailableDesc.Name = "lblAvailableDesc";
            this.lblAvailableDesc.Size = new System.Drawing.Size(153, 13);
            this.lblAvailableDesc.TabIndex = 74;
            this.lblAvailableDesc.Text = "Upgrade Available Description:";
            // 
            // txtAvailableLink
            // 
            this.txtAvailableLink.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAvailableLink.Location = new System.Drawing.Point(242, 136);
            this.txtAvailableLink.Name = "txtAvailableLink";
            this.txtAvailableLink.Size = new System.Drawing.Size(316, 20);
            this.txtAvailableLink.TabIndex = 64;
            // 
            // lblAvailableLink
            // 
            this.lblAvailableLink.AutoSize = true;
            this.lblAvailableLink.Location = new System.Drawing.Point(12, 139);
            this.lblAvailableLink.Name = "lblAvailableLink";
            this.lblAvailableLink.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblAvailableLink.Size = new System.Drawing.Size(120, 13);
            this.lblAvailableLink.TabIndex = 76;
            this.lblAvailableLink.Text = "Upgrade Available Link:";
            // 
            // VersionInformationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(572, 263);
            this.Controls.Add(this.txtAvailableLink);
            this.Controls.Add(this.lblAvailableLink);
            this.Controls.Add(this.txtAvailableDesc);
            this.Controls.Add(this.lblAvailableDesc);
            this.Controls.Add(this.txtRequiredLink);
            this.Controls.Add(this.lblRequiredLink);
            this.Controls.Add(this.txtRequiredDesc);
            this.Controls.Add(this.lblRequiredDesc);
            this.Controls.Add(this.txtCurProductVersion);
            this.Controls.Add(this.lblPeakMaxInterval);
            this.Controls.Add(this.txtMinProductVersion);
            this.Controls.Add(this.lblPeakMinInterval);
            this.Controls.Add(this.txtCurInterfaceVersion);
            this.Controls.Add(this.lblPeakBatchCount);
            this.Controls.Add(this.txtMinInterfaceVersion);
            this.Controls.Add(this.lblPeakThreshold);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.MinimumSize = new System.Drawing.Size(294, 183);
            this.Name = "VersionInformationForm";
            this.Text = "Version Info:";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtCurProductVersion;
        private System.Windows.Forms.Label lblPeakMaxInterval;
        private System.Windows.Forms.TextBox txtMinProductVersion;
        private System.Windows.Forms.Label lblPeakMinInterval;
        private System.Windows.Forms.TextBox txtCurInterfaceVersion;
        private System.Windows.Forms.Label lblPeakBatchCount;
        private System.Windows.Forms.TextBox txtMinInterfaceVersion;
        private System.Windows.Forms.Label lblPeakThreshold;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.TextBox txtRequiredDesc;
        private System.Windows.Forms.Label lblRequiredDesc;
        private System.Windows.Forms.TextBox txtRequiredLink;
        private System.Windows.Forms.Label lblRequiredLink;
        private System.Windows.Forms.TextBox txtAvailableDesc;
        private System.Windows.Forms.Label lblAvailableDesc;
        private System.Windows.Forms.TextBox txtAvailableLink;
        private System.Windows.Forms.Label lblAvailableLink;

    }
}