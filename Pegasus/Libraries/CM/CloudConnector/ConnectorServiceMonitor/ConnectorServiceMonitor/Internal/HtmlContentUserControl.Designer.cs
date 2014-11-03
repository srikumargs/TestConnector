namespace ConnectorServiceMonitor.Internal
{
    partial class HtmlContentUserControl
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
            this._webBrowser = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // _webBrowser
            // 
            this._webBrowser.AllowWebBrowserDrop = false;
            this._webBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
            this._webBrowser.IsWebBrowserContextMenuEnabled = false;
            this._webBrowser.Location = new System.Drawing.Point(0, 0);
            this._webBrowser.MinimumSize = new System.Drawing.Size(20, 20);
            this._webBrowser.Name = "_webBrowser";
            this._webBrowser.Size = new System.Drawing.Size(506, 209);
            this._webBrowser.TabIndex = 3;
            this._webBrowser.WebBrowserShortcutsEnabled = false;
            this._webBrowser.DocumentCompleted += new System.Windows.Forms.WebBrowserDocumentCompletedEventHandler(this._webBrowser_DocumentCompleted);
            this._webBrowser.Navigating += new System.Windows.Forms.WebBrowserNavigatingEventHandler(this._webBrowser_Navigating);
            // 
            // HtmlContentUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._webBrowser);
            this.Name = "HtmlContentUserControl";
            this.Size = new System.Drawing.Size(506, 209);
            this.Load += new System.EventHandler(this.HtmlContentUserControl_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser _webBrowser;
    }
}
