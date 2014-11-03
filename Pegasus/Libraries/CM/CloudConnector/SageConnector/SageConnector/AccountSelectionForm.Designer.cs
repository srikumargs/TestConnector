namespace SageConnector
{
    /// <summary>
    /// Select the account to run the service as
    /// </summary>
    partial class AccountSelectionForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AccountSelectionForm));
            this.descriptionLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblStockAccount = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.rbStockAccount = new System.Windows.Forms.RadioButton();
            this.rbSpecifiedAccount = new System.Windows.Forms.RadioButton();
            this.lblUserAccount = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.lblConfirmPassword = new System.Windows.Forms.Label();
            this.tbAccountName = new System.Windows.Forms.TextBox();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.tbConfirmPassword = new System.Windows.Forms.TextBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.lnkHelpLink = new System.Windows.Forms.LinkLabel();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // descriptionLabel
            // 
            this.descriptionLabel.Location = new System.Drawing.Point(65, 10);
            this.descriptionLabel.Name = "descriptionLabel";
            this.descriptionLabel.Size = new System.Drawing.Size(373, 50);
            this.descriptionLabel.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 5;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 35.89743F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 64.10257F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 103F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 219F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 95F));
            this.tableLayoutPanel1.Controls.Add(this.lblStockAccount, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.okButton, 3, 4);
            this.tableLayoutPanel1.Controls.Add(this.cancelButton, 4, 4);
            this.tableLayoutPanel1.Controls.Add(this.rbStockAccount, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this.rbSpecifiedAccount, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblUserAccount, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblPassword, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblConfirmPassword, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.tbAccountName, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.tbPassword, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this.tbConfirmPassword, 3, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 94);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 5;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(449, 157);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // lblStockAccount
            // 
            this.lblStockAccount.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblStockAccount.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.lblStockAccount, 3);
            this.lblStockAccount.Location = new System.Drawing.Point(34, 95);
            this.lblStockAccount.Name = "lblStockAccount";
            this.lblStockAccount.Size = new System.Drawing.Size(112, 13);
            this.lblStockAccount.TabIndex = 1;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(275, 123);
            this.okButton.Margin = new System.Windows.Forms.Padding(3, 3, 3, 11);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 5;
            this.okButton.Text = "&OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(356, 123);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(3, 3, 3, 11);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 6;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // rbStockAccount
            // 
            this.rbStockAccount.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.rbStockAccount.AutoSize = true;
            this.rbStockAccount.Location = new System.Drawing.Point(14, 95);
            this.rbStockAccount.Name = "rbStockAccount";
            this.rbStockAccount.Size = new System.Drawing.Size(14, 13);
            this.rbStockAccount.TabIndex = 0;
            this.rbStockAccount.TabStop = true;
            this.rbStockAccount.UseVisualStyleBackColor = true;
            this.rbStockAccount.CheckedChanged += new System.EventHandler(this.rbStockAccount_CheckedChanged);
            // 
            // rbSpecifiedAccount
            // 
            this.rbSpecifiedAccount.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.rbSpecifiedAccount.AutoSize = true;
            this.rbSpecifiedAccount.Location = new System.Drawing.Point(14, 8);
            this.rbSpecifiedAccount.Name = "rbSpecifiedAccount";
            this.rbSpecifiedAccount.Size = new System.Drawing.Size(14, 13);
            this.rbSpecifiedAccount.TabIndex = 1;
            this.rbSpecifiedAccount.TabStop = true;
            this.rbSpecifiedAccount.UseVisualStyleBackColor = true;
            this.rbSpecifiedAccount.CheckedChanged += new System.EventHandler(this.rbSpecifiedAccount_CheckedChanged);
            // 
            // lblUserAccount
            // 
            this.lblUserAccount.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblUserAccount.AutoSize = true;
            this.lblUserAccount.Location = new System.Drawing.Point(34, 8);
            this.lblUserAccount.Name = "lblUserAccount";
            this.lblUserAccount.Size = new System.Drawing.Size(77, 13);
            this.lblUserAccount.TabIndex = 1;
            this.lblUserAccount.Text = "User account: ";
            // 
            // lblPassword
            // 
            this.lblPassword.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(34, 37);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(59, 13);
            this.lblPassword.TabIndex = 3;
            this.lblPassword.Text = "Password: ";
            // 
            // lblConfirmPassword
            // 
            this.lblConfirmPassword.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblConfirmPassword.AutoSize = true;
            this.lblConfirmPassword.Location = new System.Drawing.Point(34, 66);
            this.lblConfirmPassword.Name = "lblConfirmPassword";
            this.lblConfirmPassword.Size = new System.Drawing.Size(96, 13);
            this.lblConfirmPassword.TabIndex = 4;
            this.lblConfirmPassword.Text = "Confirm password: ";
            // 
            // tbAccountName
            // 
            this.tbAccountName.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.tableLayoutPanel1.SetColumnSpan(this.tbAccountName, 2);
            this.tbAccountName.Location = new System.Drawing.Point(137, 4);
            this.tbAccountName.Name = "tbAccountName";
            this.tbAccountName.Size = new System.Drawing.Size(294, 20);
            this.tbAccountName.TabIndex = 2;
            this.tbAccountName.TextChanged += new System.EventHandler(this.tbAccountName_TextChanged);
            // 
            // tbPassword
            // 
            this.tbPassword.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.tableLayoutPanel1.SetColumnSpan(this.tbPassword, 2);
            this.tbPassword.Location = new System.Drawing.Point(137, 33);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.PasswordChar = '*';
            this.tbPassword.Size = new System.Drawing.Size(294, 20);
            this.tbPassword.TabIndex = 3;
            this.tbPassword.TextChanged += new System.EventHandler(this.tbPassword_TextChanged);
            // 
            // tbConfirmPassword
            // 
            this.tbConfirmPassword.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.tableLayoutPanel1.SetColumnSpan(this.tbConfirmPassword, 2);
            this.tbConfirmPassword.Location = new System.Drawing.Point(137, 62);
            this.tbConfirmPassword.Name = "tbConfirmPassword";
            this.tbConfirmPassword.PasswordChar = '*';
            this.tbConfirmPassword.Size = new System.Drawing.Size(294, 20);
            this.tbConfirmPassword.TabIndex = 4;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(10, 10);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(55, 50);
            this.pictureBox1.TabIndex = 6;
            this.pictureBox1.TabStop = false;
            // 
            // lnkHelpLink
            // 
            this.lnkHelpLink.AutoSize = true;
            this.lnkHelpLink.Location = new System.Drawing.Point(65, 70);
            this.lnkHelpLink.Name = "lnkHelpLink";
            this.lnkHelpLink.Size = new System.Drawing.Size(0, 13);
            this.lnkHelpLink.TabIndex = 7;
            this.lnkHelpLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkHelpLink_LinkClicked);
            // 
            // AccountSelectionForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(449, 251);
            this.Controls.Add(this.lnkHelpLink);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.descriptionLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AccountSelectionForm";
            this.Text = "Account Selection";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AccountSelectionForm_FormClosing);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label descriptionLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.RadioButton rbSpecifiedAccount;
        private System.Windows.Forms.Label lblUserAccount;
        private System.Windows.Forms.RadioButton rbStockAccount;
        private System.Windows.Forms.Label lblStockAccount;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.Label lblConfirmPassword;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.TextBox tbConfirmPassword;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TextBox tbAccountName;
        private System.Windows.Forms.LinkLabel lnkHelpLink;
    }
}