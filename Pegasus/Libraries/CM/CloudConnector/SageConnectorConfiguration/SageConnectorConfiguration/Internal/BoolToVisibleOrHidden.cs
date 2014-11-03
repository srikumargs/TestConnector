using System;
using System.Windows;
using System.Windows.Data;

namespace SageConnectorConfiguration
{
    /// <summary>
    /// Simple mapper from bool to visiblity
    /// </summary>
    internal class BoolToVisibleOrHidden : IValueConverter
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public BoolToVisibleOrHidden() { }

        /// <summary>
        /// 
        /// </summary>
        public bool Collapse { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public bool Reverse { get; set; }

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
            bool bValue = (bool)value;
            if (bValue != Reverse) { return Visibility.Visible; }
            else
            {
                if (Collapse)
                    return Visibility.Collapsed;
                else
                    return Visibility.Hidden;
            }
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
            Visibility visibility = (Visibility)value;
            if (visibility == Visibility.Visible)
                return !Reverse;
            else
                return Reverse;
        }
    }
}
