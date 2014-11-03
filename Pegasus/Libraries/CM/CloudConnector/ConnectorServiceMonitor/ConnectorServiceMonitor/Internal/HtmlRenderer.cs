using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using ConnectorServiceMonitor.ViewModel;
using System.Text;

namespace ConnectorServiceMonitor.Internal
{
    internal class HtmlRenderer
    {
        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public HtmlRenderer(ImageManager imageManager, Control control)
        {
            _imageManager = imageManager;
            _fontSizeInPoints = control.Font.SizeInPoints.ToString(CultureInfo.InvariantCulture);
            _fontFamilyName = control.Font.FontFamily.Name;
            _control = control;
            _graphicsForMeasure = control.CreateGraphics();

            
            ComputeCol1(_graphicsForMeasure, control.Font);
            ComputeCol2(_graphicsForMeasure, control.Font);
            
        }
        ~HtmlRenderer()

        {
            if (_graphicsForMeasure != null)
            {
                _graphicsForMeasure.Dispose();
                _graphicsForMeasure = null;
            }
        }

    protected virtual void ComputeCol1(Graphics g, Font font)
        {
            _maxColumn1Width = 0;
            _maxRowHeight = 0;
        }

        protected virtual void ComputeCol2(Graphics g, Font font)
        {
            _maxColumn2Width = 0;
        }
        Control _control;

        protected Int32 ComputeColWidth(String input, Int32 oldWidth)
        {
            //using(Graphics g = _control.CreateGraphics())
            //{
            //    return Math.Max(oldWidth, Convert.ToInt32(g.MeasureString(input, _control.Font).Width));
            //}
            return Math.Max(oldWidth, Convert.ToInt32(_graphicsForMeasure.MeasureString(input, _control.Font).Width));
        }

        protected Int32 AddToColWidth(String input, Int32 oldWidth)
        {
            //using (Graphics g = _control.CreateGraphics())
            //{
            //    return oldWidth + Convert.ToInt32(g.MeasureString(input, _control.Font).Width);
            //}
            return oldWidth + Convert.ToInt32(_graphicsForMeasure.MeasureString(input, _control.Font).Width);
        }

        protected void ReplaceCommonFormatting(StringBuilder sb)
        {
            sb.Replace("{AdditionalTableHtml}", String.Empty);
            sb.Replace("{AdditionalHtml}", String.Empty);
            sb.Replace("{Column1Width}", _maxColumn1Width.ToString(CultureInfo.InvariantCulture));
            sb.Replace("{Column2Width}", _maxColumn2Width.ToString(CultureInfo.InvariantCulture));
            sb.Replace("{RowHeight}", _maxRowHeight.ToString(CultureInfo.InvariantCulture));
            sb.Replace("{EmptyRow}", Strings.General_2ColRow_Empty_HtmlFragment);
            sb.Replace("{Common.BriefProductName}", Common.BriefProductName);
            sb.Replace("{FieldName}", String.Empty);
            sb.Replace("{ImageHtmlFragment}", String.Empty);
            sb.Replace("{FieldValue}", String.Empty);
            ReplaceFontSizeAndFamily(sb);
            ReplaceWindowBackgroundColor(sb);
            ReplaceControlBackgroundColor(sb);
            ReplaceControlLightBackgroundColor(sb);
        }

        protected static void ReplaceWindowBackgroundColor(StringBuilder sb)
        { ReplaceBackgroundColor(sb, "{WindowBackgroundColor}", SystemColors.Window); }

        protected static void ReplaceControlBackgroundColor(StringBuilder sb)
        { ReplaceBackgroundColor(sb, "{ControlBackgroundColor}", SystemColors.Control); }

        protected static void ReplaceControlLightBackgroundColor(StringBuilder sb)
        { ReplaceBackgroundColor(sb, "{ControlLightBackgroundColor}", SystemColors.ControlLight); }

        protected static void ReplaceBackgroundColor(StringBuilder sb, String tag, Color replacementColor)
        { sb.Replace(tag, replacementColor.R.ToString("X2", CultureInfo.InvariantCulture) + replacementColor.G.ToString("X2", CultureInfo.InvariantCulture) + replacementColor.B.ToString("X2", CultureInfo.InvariantCulture)); }

        protected void ReplaceFontSizeAndFamily(StringBuilder sb)
        { sb.Replace("{FontSizeInPoints}", _fontSizeInPoints).Replace("{FontFamilyName}", _fontFamilyName); }

        protected static void AppendNewTable(StringBuilder sb, String table1)
        {
            sb.Replace("{AdditionalTableHtml}", Strings.General_2ColTable_HtmlFragment);
        }

        protected static void EndCurrentTable(StringBuilder sb)
        {
            //AppendNewBoldRow(sb, String.Empty, String.Empty);
            sb.Replace("{AdditionalHtml}", String.Empty);
        }

        public enum Side
        {
            None=0,
            Left, 
            Right
        }
        protected string StatusImage(ConnectorStatusEnum status, Side dir)
        {
            switch (status)
            {
                case ConnectorStatusEnum.None:
                    {
                        return Strings.General_ImageHtmlFragment.Replace("{BitmapSource}", _imageManager.BlankBitmapFileName);
                    }
                case ConnectorStatusEnum.OkAndProcessing:
                    {
                        return Strings.General_ImageHtmlFragment.Replace("{BitmapSource}", dir == Side.Left ? _imageManager.GreenLightWorkingFileName : _imageManager.GreenLightWorkingRightSideFileName);
                    }
                case ConnectorStatusEnum.DisabledAndProcessing:
                    {
                        return Strings.General_ImageHtmlFragment.Replace("{BitmapSource}", dir == Side.Left ? _imageManager.WhiteLightWorkingFileName : _imageManager.WhiteLightWorkingRightSideFileName);
                    }
                case ConnectorStatusEnum.BrokenAndProcessing:
                    {
                        return Strings.General_ImageHtmlFragment.Replace("{BitmapSource}", dir == Side.Left ? _imageManager.RedLightWorkingFileName : _imageManager.RedLightWorkingRightSideFileName);
                    }
                case ConnectorStatusEnum.Ok:
                    {
                        return Strings.General_ImageHtmlFragment.Replace("{BitmapSource}", _imageManager.GreenLightFileName);
                    }
                case ConnectorStatusEnum.Disabled:
                    {
                        return Strings.General_ImageHtmlFragment.Replace("{BitmapSource}", _imageManager.WhiteLightFileName);
                    }
                case ConnectorStatusEnum.Broken:
                    {
                        return Strings.General_ImageHtmlFragment.Replace("{BitmapSource}", _imageManager.RedLightFileName);
                    }
                default:
                    {
                        return Strings.General_ImageHtmlFragment.Replace("{BitmapSource}", _imageManager.RedLightFileName);
                    }
            }
        }

        protected string StatusArrowImage(bool status, Side dir)
        {
            if (status)
            {
                return Strings.General_ImageHtmlFragment.Replace("{BitmapSource}", _imageManager.ArrowFileName);
            }
            return Strings.General_ImageHtmlFragment.Replace("{BitmapSource}", _imageManager.ArrowBrokenFileName);
        }

        #region Private fields
        protected readonly ImageManager _imageManager;    //= null; (automatically intialized by runtime)
        protected readonly String _fontSizeInPoints;      //= null; (automatically intialized by runtime)
        protected readonly String _fontFamilyName;        //= null; (automatically intialized by runtime)
        protected UInt32 _maxColumn1Width;       //= 0; (automatically intialized by runtime)
        protected UInt32 _maxColumn2Width;       //= 0; (automatically intialized by runtime)
        protected UInt32 _maxRowHeight;      //= 0; (automatically intialized by runtime)
        private Graphics _graphicsForMeasure;   //normally cant hold these for measurement its ok, per ms doc

        #endregion
    }
}
