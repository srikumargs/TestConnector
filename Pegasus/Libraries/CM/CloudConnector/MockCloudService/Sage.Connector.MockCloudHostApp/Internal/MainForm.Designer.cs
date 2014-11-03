namespace Sage.Connector.SageCloudService
{
    partial class Form1
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
            this.txtSiteAddress = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this._timer = new System.Windows.Forms.Timer(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnVersionInfo = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtNotificationAddress = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtWebAPIAddress = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtSiteAddress
            // 
            this.txtSiteAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSiteAddress.Location = new System.Drawing.Point(118, 77);
            this.txtSiteAddress.Name = "txtSiteAddress";
            this.txtSiteAddress.ReadOnly = true;
            this.txtSiteAddress.Size = new System.Drawing.Size(702, 20);
            this.txtSiteAddress.TabIndex = 1;
            this.txtSiteAddress.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.txtSiteAddress_MouseDoubleClick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Site address:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(193, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "The MockCloudService is now running.";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(12, 37);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(866, 37);
            this.label5.TabIndex = 9;
            this.label5.Text = "NOTE:  you may need to configure your firewall to allow incoming connections at t" +
    "he specified port number below \r\nin order to successfully communicate with this " +
    "MockCloudService from another machine.";
            // 
            // _timer
            // 
            this._timer.Interval = 1000;
            this._timer.Tick += new System.EventHandler(this._timer_Tick);
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoScroll = true;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Location = new System.Drawing.Point(15, 162);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(932, 289);
            this.panel1.TabIndex = 10;
            // 
            // btnVersionInfo
            // 
            this.btnVersionInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnVersionInfo.AutoSize = true;
            this.btnVersionInfo.Location = new System.Drawing.Point(826, 75);
            this.btnVersionInfo.Name = "btnVersionInfo";
            this.btnVersionInfo.Size = new System.Drawing.Size(121, 23);
            this.btnVersionInfo.TabIndex = 38;
            this.btnVersionInfo.Tag = "test";
            this.btnVersionInfo.Text = "Version Information";
            this.btnVersionInfo.UseVisualStyleBackColor = true;
            this.btnVersionInfo.Click += new System.EventHandler(this.btnVersionInfo_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 106);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 13);
            this.label3.TabIndex = 40;
            this.label3.Text = "Notification Address:";
            // 
            // txtNotificationAddress
            // 
            this.txtNotificationAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNotificationAddress.Location = new System.Drawing.Point(118, 103);
            this.txtNotificationAddress.Name = "txtNotificationAddress";
            this.txtNotificationAddress.ReadOnly = true;
            this.txtNotificationAddress.Size = new System.Drawing.Size(702, 20);
            this.txtNotificationAddress.TabIndex = 39;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 132);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(91, 13);
            this.label4.TabIndex = 42;
            this.label4.Text = "WebAPI Address:";
            // 
            // txtWebAPIAddress
            // 
            this.txtWebAPIAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWebAPIAddress.Location = new System.Drawing.Point(118, 129);
            this.txtWebAPIAddress.Name = "txtWebAPIAddress";
            this.txtWebAPIAddress.ReadOnly = true;
            this.txtWebAPIAddress.Size = new System.Drawing.Size(702, 20);
            this.txtWebAPIAddress.TabIndex = 41;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(959, 457);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtWebAPIAddress);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtNotificationAddress);
            this.Controls.Add(this.btnVersionInfo);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtSiteAddress);
            this.MinimumSize = new System.Drawing.Size(975, 450);
            this.Name = "Form1";
            this.Text = "MockCloudHostApp";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtSiteAddress;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Timer _timer;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnVersionInfo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtNotificationAddress;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtWebAPIAddress;

    }
}

