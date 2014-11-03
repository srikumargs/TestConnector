namespace ConnectorServiceMonitor
{
    partial class SettingsUserControl
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
            this.panel1 = new System.Windows.Forms.Panel();
            this._ipAddressRadio = new System.Windows.Forms.RadioButton();
            this._saveButton = new System.Windows.Forms.Button();
            this.txtipAddress = new System.Windows.Forms.TextBox();
            this.ComboServerList = new System.Windows.Forms.ComboBox();
            this._anotherComputerRadio = new System.Windows.Forms.RadioButton();
            this._thiscomputerRadio = new System.Windows.Forms.RadioButton();
            this._sageconnecterserverlabel = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this._neverLabel = new System.Windows.Forms.Label();
            this._10MinutesLabel = new System.Windows.Forms.Label();
            this._5minutesLabel = new System.Windows.Forms.Label();
            this._2minutesLabel = new System.Windows.Forms.Label();
            this._60secondsLabel = new System.Windows.Forms.Label();
            this._30secondsLabel = new System.Windows.Forms.Label();
            this._10secondslabel = new System.Windows.Forms.Label();
            this._5secondRadio = new System.Windows.Forms.RadioButton();
            this._10secondRadio = new System.Windows.Forms.RadioButton();
            this._30secondRadio = new System.Windows.Forms.RadioButton();
            this._60secondRadio = new System.Windows.Forms.RadioButton();
            this._2minuteRadio = new System.Windows.Forms.RadioButton();
            this._neverRadio = new System.Windows.Forms.RadioButton();
            this._5minuteRadio = new System.Windows.Forms.RadioButton();
            this._10minuteRadio = new System.Windows.Forms.RadioButton();
            this._5secondsLabel = new System.Windows.Forms.Label();
            this._refreshSettings = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this._ipAddressRadio);
            this.panel1.Controls.Add(this._saveButton);
            this.panel1.Controls.Add(this.txtipAddress);
            this.panel1.Controls.Add(this.ComboServerList);
            this.panel1.Controls.Add(this._anotherComputerRadio);
            this.panel1.Controls.Add(this._thiscomputerRadio);
            this.panel1.Controls.Add(this._sageconnecterserverlabel);
            this.panel1.Location = new System.Drawing.Point(0, 6);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(723, 198);
            this.panel1.TabIndex = 1;
            // 
            // _ipAddressRadio
            // 
            this._ipAddressRadio.AutoSize = true;
            this._ipAddressRadio.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._ipAddressRadio.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this._ipAddressRadio.Location = new System.Drawing.Point(191, 65);
            this._ipAddressRadio.Name = "_ipAddressRadio";
            this._ipAddressRadio.Size = new System.Drawing.Size(14, 13);
            this._ipAddressRadio.TabIndex = 10;
            this._ipAddressRadio.UseVisualStyleBackColor = true;
            this._ipAddressRadio.CheckedChanged += new System.EventHandler(this._ipAddressRadio_CheckedChanged);
            // 
            // _saveButton
            // 
            this._saveButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(65)))), ((int)(((byte)(169)))), ((int)(((byte)(64)))));
            this._saveButton.CausesValidation = false;
            this._saveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this._saveButton.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._saveButton.ForeColor = System.Drawing.Color.White;
            this._saveButton.Location = new System.Drawing.Point(579, 109);
            this._saveButton.Name = "_saveButton";
            this._saveButton.Size = new System.Drawing.Size(131, 37);
            this._saveButton.TabIndex = 9;
            this._saveButton.Text = "button1";
            this._saveButton.UseVisualStyleBackColor = false;
            this._saveButton.Click += new System.EventHandler(this._saveButton_Click);
            // 
            // txtipAddress
            // 
            this.txtipAddress.Location = new System.Drawing.Point(451, 65);
            this.txtipAddress.Name = "txtipAddress";
            this.txtipAddress.Size = new System.Drawing.Size(259, 20);
            this.txtipAddress.TabIndex = 8;
            // 
            // ComboServerList
            // 
            this.ComboServerList.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ComboServerList.FormattingEnabled = true;
            this.ComboServerList.Location = new System.Drawing.Point(451, 35);
            this.ComboServerList.Name = "ComboServerList";
            this.ComboServerList.Size = new System.Drawing.Size(259, 24);
            this.ComboServerList.TabIndex = 5;
            // 
            // _anotherComputerRadio
            // 
            this._anotherComputerRadio.AutoSize = true;
            this._anotherComputerRadio.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._anotherComputerRadio.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this._anotherComputerRadio.Location = new System.Drawing.Point(191, 35);
            this._anotherComputerRadio.Name = "_anotherComputerRadio";
            this._anotherComputerRadio.Size = new System.Drawing.Size(14, 13);
            this._anotherComputerRadio.TabIndex = 3;
            this._anotherComputerRadio.UseVisualStyleBackColor = true;
            this._anotherComputerRadio.CheckedChanged += new System.EventHandler(this._anotherComputerRadio_CheckedChanged);
            // 
            // _thiscomputerRadio
            // 
            this._thiscomputerRadio.AutoSize = true;
            this._thiscomputerRadio.Checked = true;
            this._thiscomputerRadio.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._thiscomputerRadio.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this._thiscomputerRadio.Location = new System.Drawing.Point(191, 9);
            this._thiscomputerRadio.Name = "_thiscomputerRadio";
            this._thiscomputerRadio.Size = new System.Drawing.Size(14, 13);
            this._thiscomputerRadio.TabIndex = 2;
            this._thiscomputerRadio.TabStop = true;
            this._thiscomputerRadio.UseVisualStyleBackColor = true;
            this._thiscomputerRadio.CheckedChanged += new System.EventHandler(this._thiscomputerRadio_CheckedChanged);
            // 
            // _sageconnecterserverlabel
            // 
            this._sageconnecterserverlabel.AutoSize = true;
            this._sageconnecterserverlabel.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._sageconnecterserverlabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this._sageconnecterserverlabel.Location = new System.Drawing.Point(11, 9);
            this._sageconnecterserverlabel.Name = "_sageconnecterserverlabel";
            this._sageconnecterserverlabel.Size = new System.Drawing.Size(0, 18);
            this._sageconnecterserverlabel.TabIndex = 1;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.tableLayoutPanel1);
            this.panel2.Controls.Add(this._refreshSettings);
            this.panel2.Location = new System.Drawing.Point(0, 210);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(723, 82);
            this.panel2.TabIndex = 2;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tableLayoutPanel1.ColumnCount = 8;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this._neverLabel, 7, 1);
            this.tableLayoutPanel1.Controls.Add(this._10MinutesLabel, 5, 1);
            this.tableLayoutPanel1.Controls.Add(this._5minutesLabel, 3, 1);
            this.tableLayoutPanel1.Controls.Add(this._2minutesLabel, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this._60secondsLabel, 7, 0);
            this.tableLayoutPanel1.Controls.Add(this._30secondsLabel, 5, 0);
            this.tableLayoutPanel1.Controls.Add(this._10secondslabel, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this._5secondRadio, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this._10secondRadio, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this._30secondRadio, 4, 0);
            this.tableLayoutPanel1.Controls.Add(this._60secondRadio, 6, 0);
            this.tableLayoutPanel1.Controls.Add(this._2minuteRadio, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this._neverRadio, 6, 1);
            this.tableLayoutPanel1.Controls.Add(this._5minuteRadio, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this._10minuteRadio, 4, 1);
            this.tableLayoutPanel1.Controls.Add(this._5secondsLabel, 1, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(1, 26);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(722, 53);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // _neverLabel
            // 
            this._neverLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._neverLabel.AutoSize = true;
            this._neverLabel.Font = new System.Drawing.Font("Arial", 9.75F);
            this._neverLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this._neverLabel.Location = new System.Drawing.Point(565, 31);
            this._neverLabel.Name = "_neverLabel";
            this._neverLabel.Size = new System.Drawing.Size(153, 16);
            this._neverLabel.TabIndex = 15;
            this._neverLabel.Text = "label13";
            this._neverLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _10MinutesLabel
            // 
            this._10MinutesLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._10MinutesLabel.AutoSize = true;
            this._10MinutesLabel.Font = new System.Drawing.Font("Arial", 9.75F);
            this._10MinutesLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this._10MinutesLabel.Location = new System.Drawing.Point(385, 31);
            this._10MinutesLabel.Name = "_10MinutesLabel";
            this._10MinutesLabel.Size = new System.Drawing.Size(152, 16);
            this._10MinutesLabel.TabIndex = 14;
            this._10MinutesLabel.Text = "label12";
            this._10MinutesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _5minutesLabel
            // 
            this._5minutesLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._5minutesLabel.AutoSize = true;
            this._5minutesLabel.Font = new System.Drawing.Font("Arial", 9.75F);
            this._5minutesLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this._5minutesLabel.Location = new System.Drawing.Point(205, 31);
            this._5minutesLabel.Name = "_5minutesLabel";
            this._5minutesLabel.Size = new System.Drawing.Size(152, 16);
            this._5minutesLabel.TabIndex = 13;
            this._5minutesLabel.Text = "label11";
            this._5minutesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _2minutesLabel
            // 
            this._2minutesLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._2minutesLabel.AutoSize = true;
            this._2minutesLabel.Font = new System.Drawing.Font("Arial", 9.75F);
            this._2minutesLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this._2minutesLabel.Location = new System.Drawing.Point(25, 31);
            this._2minutesLabel.Name = "_2minutesLabel";
            this._2minutesLabel.Size = new System.Drawing.Size(152, 16);
            this._2minutesLabel.TabIndex = 12;
            this._2minutesLabel.Text = "label10";
            this._2minutesLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _60secondsLabel
            // 
            this._60secondsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._60secondsLabel.AutoSize = true;
            this._60secondsLabel.Font = new System.Drawing.Font("Arial", 9.75F);
            this._60secondsLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this._60secondsLabel.Location = new System.Drawing.Point(565, 5);
            this._60secondsLabel.Name = "_60secondsLabel";
            this._60secondsLabel.Size = new System.Drawing.Size(153, 16);
            this._60secondsLabel.TabIndex = 11;
            this._60secondsLabel.Text = "label9";
            this._60secondsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _30secondsLabel
            // 
            this._30secondsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._30secondsLabel.AutoSize = true;
            this._30secondsLabel.Font = new System.Drawing.Font("Arial", 9.75F);
            this._30secondsLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this._30secondsLabel.Location = new System.Drawing.Point(385, 5);
            this._30secondsLabel.Name = "_30secondsLabel";
            this._30secondsLabel.Size = new System.Drawing.Size(152, 16);
            this._30secondsLabel.TabIndex = 10;
            this._30secondsLabel.Text = "label8";
            this._30secondsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _10secondslabel
            // 
            this._10secondslabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._10secondslabel.AutoSize = true;
            this._10secondslabel.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._10secondslabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this._10secondslabel.Location = new System.Drawing.Point(205, 5);
            this._10secondslabel.Name = "_10secondslabel";
            this._10secondslabel.Size = new System.Drawing.Size(152, 16);
            this._10secondslabel.TabIndex = 9;
            this._10secondslabel.Text = "label7";
            this._10secondslabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _5secondRadio
            // 
            this._5secondRadio.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._5secondRadio.AutoSize = true;
            this._5secondRadio.Location = new System.Drawing.Point(4, 4);
            this._5secondRadio.Name = "_5secondRadio";
            this._5secondRadio.Size = new System.Drawing.Size(14, 19);
            this._5secondRadio.TabIndex = 0;
            this._5secondRadio.UseVisualStyleBackColor = true;
            // 
            // _10secondRadio
            // 
            this._10secondRadio.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._10secondRadio.AutoSize = true;
            this._10secondRadio.Location = new System.Drawing.Point(184, 4);
            this._10secondRadio.Name = "_10secondRadio";
            this._10secondRadio.Size = new System.Drawing.Size(14, 19);
            this._10secondRadio.TabIndex = 1;
            this._10secondRadio.UseVisualStyleBackColor = true;
            // 
            // _30secondRadio
            // 
            this._30secondRadio.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._30secondRadio.AutoSize = true;
            this._30secondRadio.Location = new System.Drawing.Point(364, 4);
            this._30secondRadio.Name = "_30secondRadio";
            this._30secondRadio.Size = new System.Drawing.Size(14, 19);
            this._30secondRadio.TabIndex = 2;
            this._30secondRadio.UseVisualStyleBackColor = true;
            // 
            // _60secondRadio
            // 
            this._60secondRadio.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._60secondRadio.AutoSize = true;
            this._60secondRadio.Location = new System.Drawing.Point(544, 4);
            this._60secondRadio.Name = "_60secondRadio";
            this._60secondRadio.Size = new System.Drawing.Size(14, 19);
            this._60secondRadio.TabIndex = 3;
            this._60secondRadio.UseVisualStyleBackColor = true;
            // 
            // _2minuteRadio
            // 
            this._2minuteRadio.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._2minuteRadio.AutoSize = true;
            this._2minuteRadio.Location = new System.Drawing.Point(4, 30);
            this._2minuteRadio.Name = "_2minuteRadio";
            this._2minuteRadio.Size = new System.Drawing.Size(14, 19);
            this._2minuteRadio.TabIndex = 4;
            this._2minuteRadio.UseVisualStyleBackColor = true;
            // 
            // _neverRadio
            // 
            this._neverRadio.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._neverRadio.AutoSize = true;
            this._neverRadio.Location = new System.Drawing.Point(544, 30);
            this._neverRadio.Name = "_neverRadio";
            this._neverRadio.Size = new System.Drawing.Size(14, 19);
            this._neverRadio.TabIndex = 7;
            this._neverRadio.UseVisualStyleBackColor = true;
            // 
            // _5minuteRadio
            // 
            this._5minuteRadio.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._5minuteRadio.AutoSize = true;
            this._5minuteRadio.Location = new System.Drawing.Point(184, 30);
            this._5minuteRadio.Name = "_5minuteRadio";
            this._5minuteRadio.Size = new System.Drawing.Size(14, 19);
            this._5minuteRadio.TabIndex = 5;
            this._5minuteRadio.UseVisualStyleBackColor = true;
            // 
            // _10minuteRadio
            // 
            this._10minuteRadio.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._10minuteRadio.AutoSize = true;
            this._10minuteRadio.Location = new System.Drawing.Point(364, 30);
            this._10minuteRadio.Name = "_10minuteRadio";
            this._10minuteRadio.Size = new System.Drawing.Size(14, 19);
            this._10minuteRadio.TabIndex = 6;
            this._10minuteRadio.UseVisualStyleBackColor = true;
            // 
            // _5secondsLabel
            // 
            this._5secondsLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._5secondsLabel.AutoSize = true;
            this._5secondsLabel.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._5secondsLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this._5secondsLabel.Location = new System.Drawing.Point(25, 5);
            this._5secondsLabel.Name = "_5secondsLabel";
            this._5secondsLabel.Size = new System.Drawing.Size(152, 16);
            this._5secondsLabel.TabIndex = 8;
            this._5secondsLabel.Text = "label6";
            this._5secondsLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _refreshSettings
            // 
            this._refreshSettings.BackColor = System.Drawing.Color.Transparent;
            this._refreshSettings.Font = new System.Drawing.Font("Arial", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this._refreshSettings.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(102)))), ((int)(((byte)(102)))), ((int)(((byte)(102)))));
            this._refreshSettings.Location = new System.Drawing.Point(1, 0);
            this._refreshSettings.Name = "_refreshSettings";
            this._refreshSettings.Size = new System.Drawing.Size(722, 23);
            this._refreshSettings.TabIndex = 0;
            this._refreshSettings.Text = "Refresh Monitor Information Every";
            this._refreshSettings.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // SettingsUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "SettingsUserControl";
            this.Size = new System.Drawing.Size(727, 297);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button _saveButton;
        private System.Windows.Forms.TextBox txtipAddress;
        private System.Windows.Forms.ComboBox ComboServerList;
        private System.Windows.Forms.RadioButton _anotherComputerRadio;
        private System.Windows.Forms.RadioButton _thiscomputerRadio;
        private System.Windows.Forms.Label _sageconnecterserverlabel;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.RadioButton _5secondRadio;
        private System.Windows.Forms.RadioButton _10secondRadio;
        private System.Windows.Forms.RadioButton _30secondRadio;
        private System.Windows.Forms.RadioButton _60secondRadio;
        private System.Windows.Forms.RadioButton _2minuteRadio;
        private System.Windows.Forms.RadioButton _neverRadio;
        private System.Windows.Forms.RadioButton _5minuteRadio;
        private System.Windows.Forms.RadioButton _10minuteRadio;
        private System.Windows.Forms.Label _refreshSettings;
        private System.Windows.Forms.Label _neverLabel;
        private System.Windows.Forms.Label _10MinutesLabel;
        private System.Windows.Forms.Label _5minutesLabel;
        private System.Windows.Forms.Label _2minutesLabel;
        private System.Windows.Forms.Label _60secondsLabel;
        private System.Windows.Forms.Label _30secondsLabel;
        private System.Windows.Forms.Label _10secondslabel;
        private System.Windows.Forms.Label _5secondsLabel;
        private System.Windows.Forms.RadioButton _ipAddressRadio;
    }
}
