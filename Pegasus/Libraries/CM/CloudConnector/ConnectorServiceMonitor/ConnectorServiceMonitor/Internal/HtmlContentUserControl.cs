using System;
using System.Windows.Forms;
using Sage.Connector.MonitorService.Interfaces.DataContracts;
using ConnectorServiceMonitor.ViewModel;
using System.Runtime.InteropServices;

namespace ConnectorServiceMonitor.Internal
{
    internal partial class HtmlContentUserControl : UserControl
    {
        public HtmlContentUserControl()
        {
            InitializeComponent();

            int feature = FEATURE_DISABLE_NAVIGATION_SOUNDS;
            NativeMethods.CoInternetSetFeatureEnabled(feature, SET_FEATURE_ON_PROCESS, true);
        }

        public void SetDocumentText(String documentText)
        {
            _documentText = documentText;
            if (_loaded && _webBrowser.Document != null)
            {
                try
                {
                    var rect = _webBrowser.Document.GetElementsByTagName("body")[0].ScrollRectangle;
                    scroll = rect.Top;
                }
                catch (Exception) { }
                _webBrowser.DocumentText = documentText;
            }
        }

        private Int32 scroll;

        private void HtmlContentUserControl_Load(object sender, EventArgs e)
        {
            _webBrowser.DocumentText = _documentText;
            _loaded = true;
        }

        private void _webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            _webBrowser.Document.Window.ScrollTo(0, scroll);
        }

        private Boolean _loaded;
        private String _documentText;

        private EventHandler _changeSettings = delegate(Object o, EventArgs e) { };
        public event EventHandler ChangeSettings
        {
            add { _changeSettings += value; }
            remove { _changeSettings -= value; }
        }

        private EventHandler _applyUpdate = delegate(Object o, EventArgs e) { };
        public event EventHandler ApplyUpdate
        {
            add { _applyUpdate += value; }
            remove { _applyUpdate -= value; }
        }

        protected void _webBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {

            if (e.Url.OriginalString == "about:blank")
            { 
                return;
            }

            if (e.Url.Scheme == "csm" && e.Url.Host == "changesettings")
            {
                e.Cancel = true;
                _changeSettings(this, null);
                return;
            }


            if (e.Url.Scheme == "csm" && e.Url.Host == "applyupdate")
            {
                e.Cancel = true;
                _applyUpdate(this, null);
                return;
            }
        }
        private const int FEATURE_DISABLE_NAVIGATION_SOUNDS = 21;
        private const int SET_FEATURE_ON_THREAD = 0x00000001;
        private const int SET_FEATURE_ON_PROCESS = 0x00000002;
        private const int SET_FEATURE_IN_REGISTRY = 0x00000004;
        private const int SET_FEATURE_ON_THREAD_LOCALMACHINE = 0x00000008;
        private const int SET_FEATURE_ON_THREAD_INTRANET = 0x00000010;
        private const int SET_FEATURE_ON_THREAD_TRUSTED = 0x00000020;
        private const int SET_FEATURE_ON_THREAD_INTERNET = 0x00000040;
        private const int SET_FEATURE_ON_THREAD_RESTRICTED = 0x00000080;
    }

    /// <summary>
    /// Encapsulated native methods
    /// </summary>
    internal static class NativeMethods
    {
        [DllImport("urlmon.dll")]
        [PreserveSig]
        [return: MarshalAs(UnmanagedType.Error)]
        internal static extern int CoInternetSetFeatureEnabled(
        int FeatureEntry,
        [MarshalAs(UnmanagedType.U4)] int dwFlags,
        bool fEnable);

    }
}

