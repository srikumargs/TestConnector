using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Sage.Connector.AutoUpdate
{
    static class ConfigurationProvider
    {

        //name of the primary auto update configuration file.
        private static string _autoUpdateConfigFile = "AutoUpdate.config.xml";

        //name of the auto update override file for configuration. This is primarily for dev use.
        private static string _autoUpdateNonProductionConfigFile = "AutoUpdate.NonProduction.config.xml";

        //name of the auto update override file for configuration. This is primarily for dev use.
        private static string _autoUpdateOverrideConfigFile = "AutoUpdate.Override.config.xml";



        /// <summary>
        /// Gets the interval setting.
        /// </summary>
        /// <param name="interval">The interval.</param>
        /// <returns></returns>
        internal static string GetIntervalSetting(string interval)
        {
            string retval = GetSettingFromElementWithId("PeroidicUpdate", interval);
            return retval;
        }

        /// <summary>
        /// Gets from configuration file.
        /// </summary>
        /// <param name="packageId">The package identifier.</param>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        /// <remarks>
        /// Gets the requested value from the AutoUpdate.Config.xml file if it exists and is not overriden.
        /// Be default an autoupdate.config.xml file ships with the product.
        /// The AutoUpdate.override.config.xml file is for internal use. This file does not ship with product but is
        /// same format. This allows folks who compile to have a persistent override that lasts thru full rebuilds.
        /// </remarks>
        internal static string GetPackageSetting(string packageId, string key)
        {
            string retval = GetSettingFromElementWithId("Package", "packageId", packageId, key, "Default");
            return retval;
        }


        private static string GetSettingFromElementWithId(string elementName, string elementIdName, string elementIdValue, string key, string fallbackElmentIdValue = null)
        {
            string retval = string.Empty;

            //consider adding logging for these. Do not want to throw here.
            
            if (String.IsNullOrWhiteSpace(elementName)) return retval;
            if (String.IsNullOrWhiteSpace(elementIdName)) return retval;
            if (String.IsNullOrWhiteSpace(elementIdValue)) return retval;
            if (String.IsNullOrWhiteSpace(key)) return retval;


            string targetFile = GetConfigFilePath();
            if (!String.IsNullOrEmpty(targetFile))
            {
                XDocument configXml = XDocument.Load(targetFile);

                string keyDefault = null;
                if (!string.IsNullOrWhiteSpace(fallbackElmentIdValue))
                {
                    var defaultConfig = from c in configXml.Descendants(elementName)
                                        where string.Compare(c.Attribute(elementIdName).Value, fallbackElmentIdValue, StringComparison.OrdinalIgnoreCase) == 0
                                        select (c.Element(key) != null ? c.Element(key).Value : string.Empty);
                    
                    keyDefault = defaultConfig.FirstOrDefault();

                }

                var q = from c in configXml.Descendants(elementName)
                        where string.Compare(c.Attribute(elementIdName).Value, elementIdValue, StringComparison.OrdinalIgnoreCase) == 0
                        select (ConvertToString(c.Element(key)));

                string keyLookup = q.FirstOrDefault();

                if (String.IsNullOrWhiteSpace(keyLookup))
                {
                    retval = keyDefault;
                }
                else
                {
                    retval = keyLookup;
                }
            }
            return retval;
        }

        private static string GetSettingFromElementWithId(string elementName, string key)
        {
            string retval = string.Empty;

            //consider adding logging for these. Do not want to throw here.

            if (String.IsNullOrWhiteSpace(elementName)) return retval;
            if (String.IsNullOrWhiteSpace(key)) return retval;


            string targetFile = GetConfigFilePath();
            if (!String.IsNullOrEmpty(targetFile))
            {
                XDocument configXml = XDocument.Load(targetFile);

        
                var q = from c in configXml.Descendants(elementName)
                        select (ConvertToString(c.Element(key)));

                string keyLookup = q.FirstOrDefault();
                retval = keyLookup;
        
            }
            return retval;
        }

        private static string ConvertToString(XElement xElement)
        {
            string retval = string.Empty;

            if (xElement != null)
            {
                string value = xElement.Value;
                if (!String.IsNullOrWhiteSpace(value))
                {
                    retval = value;
                }
            }

            return retval;
        }

        private static string GetConfigFilePath()
        {
            string targetPath = string.Empty;
            string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string baseFile = Path.Combine(basePath, _autoUpdateConfigFile);
            string nonProductionFile = Path.Combine(basePath, _autoUpdateNonProductionConfigFile);
            string overrideFile = Path.Combine(basePath, _autoUpdateOverrideConfigFile);

            
            if (File.Exists(overrideFile))
            {
                targetPath = overrideFile;
            }
            else if (File.Exists(nonProductionFile))
            {
                targetPath = nonProductionFile;
            }
            else if (File.Exists(baseFile))
            {
                targetPath = baseFile;
            }

            return targetPath;
        }


    }
    internal static class PackageConfigKeys
    {
        /// <summary>
        /// The service URI
        /// </summary>
        public static string ServiceUri = "ServiceUri";

        /// <summary>
        /// The product identifier
        /// </summary>
        public static string ProductId = "ProductId";

        /// <summary>
        /// The product version
        /// </summary>
        public static string ProductVersion = "ProductVersion";

        /// <summary>
        /// The component base name
        /// </summary>
        public static string ComponentBaseName = "ComponentBaseName";
    }

    /// <summary>
    /// 
    /// </summary>
    internal static class IntervalConfigKeys
    {
        /// <summary>
        /// The start interval
        /// </summary>
        public static string StartInterval = "StartInterval";

        /// <summary>
        /// The inactive interval
        /// </summary>
        public static string HeartbeatInterval = "HeartbeatInterval";

        /// <summary>
        /// The query interval
        /// </summary>
        public static string AutoUpdateQueryInterval = "AutoUpdateQueryInterval";

        /// <summary>
        /// The suspend periodic update
        /// </summary>
        public static string SuspendPeriodicUpdate = "SuspendPeriodicUpdate";
    }
}
