using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using ConnectorServiceMonitor.ViewModel;
using System.Text;

namespace ConnectorServiceMonitor.Internal
{
    internal sealed class StartPageHtmlRenderer : HtmlRenderer
    {
        public StartPageHtmlRenderer(ImageManager imageManager, Control control)
            :base(imageManager, control)
        {}

        public String Render()
        {
            var sb = new StringBuilder(Strings.General_WelcomeHtmlDocument);
            sb.Replace("{Message}", Strings.General_WelcomeHtmlFragment);
            ReplaceCommonFormatting(sb);
            return sb.ToString();
        }
    }
}
