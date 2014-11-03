using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Reflection;
using System.Xml.Linq;
using System.IO;

namespace ConnectorServiceMonitor.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    public static class Common
    {
        static Common()
        {
            var doc =XDocument.Load(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Configuration.xml"));
            var configurationElement = doc.Descendants("Configuration").Single();
            _briefProductName = configurationElement.Descendants("BriefProductName").Single().Value;
            _monitorBriefProductName = configurationElement.Descendants("MonitorBriefProductName").Single().Value;
            _defaultCatalogServicePortNumber = Convert.ToInt32(configurationElement.Descendants("DefaultCatalogServicePortNumber").Single().Value);
            _productHelpBaseUrl = configurationElement.Descendants("ProductHelpBaseUrl").Single().Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static String ReplaceKnownTerms(String input)
        {
            StringBuilder sb = new StringBuilder(input);
            sb.Replace("{BriefProductName}", BriefProductName);
            sb.Replace("{MonitorBriefProductName}", MonitorBriefProductName);
            sb.Replace("{TerseConnectorName}", Strings.Term_TerseConnectorName);
            return sb.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        public static String BriefProductName
        { get { return _briefProductName; } }

        /// <summary>
        /// 
        /// </summary>
        public static Int32 DefaultCatalogServicePortNumber
        { get { return _defaultCatalogServicePortNumber; } }

        /// <summary>
        /// 
        /// </summary>
        public static String ProductHelpBaseUrl
        { get { return _productHelpBaseUrl; } }

        /// <summary>
        /// 
        /// </summary>
        public static String MonitorBriefProductName
        { get { return _monitorBriefProductName; } }

        /// <summary>
        /// 
        /// </summary>
        public static String ServerRegistrySubKeyPath
        { 
            get
            {
                var myLocation = Assembly.GetEntryAssembly().Location;
                return String.Format(@"SOFTWARE\Sage\{0}", myLocation.Replace('\\', '/')); 
            } 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="occurrence"></param>
        /// <returns></returns>
        public static String CreateTimeTillOccurrenceTimeString(DateTime reference, DateTime occurrence)
        {
            String result = String.Empty;

            if (occurrence != DateTime.MinValue)
            {
                var recent = reference - occurrence;
                if (recent > TimeSpan.Zero)
                {
                    result = String.Format("{0} ({1} ago)", occurrence.ToString("G", CultureInfo.CurrentCulture), CreateElapsedTimeString(recent, false));
                }
                else
                {
                    result = String.Format("{0} ({1} from now)", occurrence.ToString("G", CultureInfo.CurrentCulture), CreateElapsedTimeString(-recent, false));
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="elapsedTime"></param>
        /// <param name="upSince"></param>
        /// <returns></returns>
        public static String CreateElapsedSinceTimeString(DateTime reference, TimeSpan elapsedTime, out DateTime upSince)
        {
            upSince = reference - elapsedTime;
            return String.Format("{0} (since {1})", CreateElapsedTimeString(elapsedTime, false), upSince.ToString("G", CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elapsedTime"></param>
        /// <param name="showFractionalSeconds"></param>
        /// <returns></returns>
        public static String CreateElapsedTimeString(TimeSpan elapsedTime, Boolean showFractionalSeconds)
        {
            String result = String.Empty;

            var days = elapsedTime.Days;
            var hours = elapsedTime.Hours;
            var minutes = elapsedTime.Minutes;
            var seconds = elapsedTime.Seconds;
            var milliseconds = elapsedTime.Milliseconds;
            if (elapsedTime.TotalDays >= 1.0)
            {
                result = String.Format("{0} day{1}, {2} hour{3}, {4} minute{5}, {6} second{7}",
                    days, (days > 1) ? "s" : String.Empty,
                    hours, (hours > 1) ? "s" : String.Empty,
                    minutes, (minutes > 1) ? "s" : String.Empty,
                    seconds, (seconds > 1) ? "s" : String.Empty);
            }
            else if (elapsedTime.TotalHours >= 1.0)
            {
                result = String.Format("{0} hour{1}, {2} minute{3}, {4} second{5}",
                    hours, (hours > 1) ? "s" : String.Empty,
                    minutes, (minutes > 1) ? "s" : String.Empty,
                    seconds, (seconds > 1) ? "s" : String.Empty);
            }
            else if (elapsedTime.TotalMinutes >= 1.0)
            {
                result = String.Format("{0} minute{1}, {2} second{3}",
                    minutes, (minutes > 1) ? "s" : String.Empty,
                    seconds, (seconds > 1) ? "s" : String.Empty);
            }
            else
            {
                if (showFractionalSeconds)
                {
                    double value = seconds + (milliseconds / 1000.0);
                    result = String.Format("{0} seconds",
                        value.ToString("0.000"));
                }
                else
                {
                    result = String.Format("{0} second{1}",
                        seconds, (seconds > 1) ? "s" : String.Empty);
                }
            }

            return result;
        }

        private static readonly String _briefProductName;
        private static readonly String _monitorBriefProductName;
        private static readonly Int32 _defaultCatalogServicePortNumber;
        private static readonly String _productHelpBaseUrl;
    }
}
