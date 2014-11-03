using System;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;

namespace SageConnectorConfiguration
{
    /// <summary>
    /// 
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Cursors))]
    internal class CursorExtensionConverter : MarkupExtension, IValueConverter
    {
        /// <summary>
        /// 
        /// </summary>
        public CursorExtensionConverter()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return instance;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value is bool)
            {
                if((bool)value)
                    return Cursors.Wait;
                else
                    return Cursors.Arrow;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Cursors)
            {
                if (value == Cursors.Wait)
                    return true;
                else
                    return false;
            }
            return null;
        }

        private static CursorExtensionConverter instance = new CursorExtensionConverter();
    }
}
