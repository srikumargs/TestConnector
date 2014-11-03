using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Sage.Connector.Management
{
    /// <summary>
    /// 
    /// </summary>
    internal class SiteGroupManager
    {

        //name of the primary auto update configuration file.
        private static string _cloudSiteConfigFile = "CloudSite.config.xml";

        private static string _cloudSiteNonProductionConfigFile = "CloudSite.NonProduction.config.xml";

        //name of the auto update override file for configuration. This is primarily for dev use.
        private static string _cloudSiteOverrideConfigFile = "CloudSite.Override.config.xml";


        private Dictionary<string, SiteGroup> _siteGroups;

        /// <summary>
        /// Reads the configuration.
        /// </summary>
        public void ReadConfig()
        {
            string targetFile = GetConfigFilePath();
            if (!String.IsNullOrEmpty(targetFile))
            {
                XDocument configXml = XDocument.Load(targetFile);

                string elementName = "SiteGroup";
                var q = from c in configXml.Descendants(elementName)
                    select new SiteGroup(
                        ConvertToString(c.Attribute("siteId")),
                        ConvertToUri(c.Element("ConnectorServiceUri")),
                        ConvertToString(c.Element("ClientId")),
                        ConvertToString(c.Element("Scope")),
                        ConvertToBool(c.Attribute("isDefault")),
                        ConvertToUri(c.Element("DatacloudWebPageUri")),
                        ConvertToString(c.Element("DataCloudWebPageDisplayName"))
                        );

                _siteGroups = q.ToDictionary(x => x.Id);
            }
        }

        public SiteGroup FindDefaultGroup()
        {
            var sg = from g in _siteGroups.Values
                where g.IsDefault
                select g;

            SiteGroup retval = sg.FirstOrDefault();
            return retval;
        }

        public SiteGroup FindCloudSiteUri(Uri uri)
        {
            var sg = from g in _siteGroups.Values
                where g.CloudSiteUri == uri 
                select g;

            SiteGroup retval = sg.FirstOrDefault();
            return retval;
        }

        internal void OverWriteServiceAndPlatformUriIfEmpty(string key, Uri uri)
        {
            if (_siteGroups.ContainsKey(key))
            {
                SiteGroup sg = _siteGroups[key];
                if (sg.CloudSiteUri == null)
                    sg.CloudSiteUri = uri;
                
                if (sg.ConnectorServiceUri == null)
                    sg.ConnectorServiceUri = uri;
            }
        }

        /// <summary>
        /// Sites the groups.
        /// </summary>
        /// <returns></returns>
        public IList<SiteGroup> SiteGroups()
        {
            IList<SiteGroup> list = _siteGroups.Values.ToList();
            return list;
        }
    

        private static string ConvertToString(XAttribute xAttribute)
        {
            string retval = string.Empty;

            if (xAttribute != null)
            {
                string value = xAttribute.Value;
                if (!String.IsNullOrWhiteSpace(value))
                {
                    retval = value;
                }
            }

            return retval;
        }

        private static bool ConvertToBool(XAttribute xAttribute)
        {
            bool retval = false;

            if (xAttribute != null)
            {
                string value = xAttribute.Value;
                if (!String.IsNullOrWhiteSpace(value))
                {
                    if (string.Compare(value, "true", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        retval = true;
                    }
                }
            }
            
            return retval;
        }
        private static Uri ConvertToUri(XElement xElement)
        {   
            Uri retval = null;

            if (xElement != null)
            {
                string value = xElement.Value;
            
                if (!String.IsNullOrWhiteSpace(value))
                {
                    Uri.TryCreate(value, UriKind.Absolute, out retval);
                }
            }
            
            return retval;
        }

        private static String ConvertToString(XElement xElement)
        {
            if (xElement != null)
            {
                return xElement.Value;
            }

            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static string GetConfigFilePath()
        {
            //this is much the same as ConfigurationProvider in AutoUpdate. Consider moving to a common base.
            string targetPath = string.Empty;
            string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string baseFile = Path.Combine(basePath, _cloudSiteConfigFile);
            string nonproductionfile = Path.Combine(basePath, _cloudSiteNonProductionConfigFile);
            string overrideFile = Path.Combine(basePath, _cloudSiteOverrideConfigFile);


            if (File.Exists(overrideFile))
            {
                targetPath = overrideFile;
            }
            else if (File.Exists(nonproductionfile))
            {
                targetPath = nonproductionfile;
            }
            else if (File.Exists(baseFile))
            {
                targetPath = baseFile;
            }

            return targetPath;
        }

    }
}