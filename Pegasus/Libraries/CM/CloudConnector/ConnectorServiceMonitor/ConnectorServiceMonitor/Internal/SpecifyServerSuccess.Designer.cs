namespace ConnectorServiceMonitor.Internal
{
    partial class SpecifyServerSuccess
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SpecifyServerSuccess));
            this.successPictureBox = new System.Windows.Forms.PictureBox();
            this.successLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.successPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // successPictureBox
            // 
            this.successPictureBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.successPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("successPictureBox.Image")));
            this.successPictureBox.Location = new System.Drawing.Point(0, 0);
            this.successPictureBox.Name = "successPictureBox";
            this.successPictureBox.Size = new System.Drawing.Size(16, 150);
            this.successPictureBox.TabIndex = 2;
            this.successPictureBox.TabStop = false;
            // 
            // successLabel
            // 
            this.successLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.successLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.successLabel.Location = new System.Drawing.Point(16, 0);
            this.successLabel.Name = "successLabel";
            this.successLabel.Size = new System.Drawing.Size(398, 44);
            this.successLabel.TabIndex = 3;
            this.successLabel.Text = "The {0} server is {1}.";
            // 
            // SpecifyServerSuccess
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.successLabel);
            this.Controls.Add(this.successPictureBox);
            this.Name = "SpecifyServerSuccess";
            this.Size = new System.Drawing.Size(414, 150);
            ((System.ComponentModel.ISupportInitialize)(this.successPictureBox)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox successPictureBox;
        private System.Windows.Forms.Label successLabel;
    }
}
