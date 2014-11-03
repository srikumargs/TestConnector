using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Sage.Connector.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class HelpLinkLoader
    {

        //name of the primary auto update configuration file.
        //something like "SageConnect.ViewModels.CustomerHelpLinks.resx";
        private static string _cloudSiteConfigFile;

        //something like  = "SageConnect.ViewModels.CustomerHelpLinks.NonProduction.resx"
        private static string _cloudSiteNonProductionConfigFile;

        //name of the auto update override file for configuration. This is primarily for dev use.
        //something like "SageConnect.ViewModels.CustomerHelpLinks.Override.resx"
        private static string _cloudSiteOverrideConfigFile;

        private static string _nonProduction = "NonProduction.";
        private static string _override = "Override.";

        private Dictionary<string, UriEntry> _uriEntries;

        /// <summary>
        /// Initializes a new instance of the <see cref="HelpLinkLoader"/> class.
        /// </summary>
        /// <param name="baseFileName">Name of the base file.</param>
        /// <param name="extension">The extension.</param>
        public HelpLinkLoader(string baseFileName, string extension)

        {
            _cloudSiteConfigFile = string.Format("{0}{1}", baseFileName, extension);
            _cloudSiteNonProductionConfigFile = string.Format("{0}{1}{2}", baseFileName,  _nonProduction ,extension);
            _cloudSiteOverrideConfigFile = string.Format("{0}{1}{2}", baseFileName, _override, extension);
        }

        /// <summary>
        /// Reads the configuration.
        /// </summary>
        public void ReadConfig()
        {
            string targetFile = GetConfigFilePath();
            if (!String.IsNullOrEmpty(targetFile))
            {
                XDocument configXml = XDocument.Load(targetFile);

                string elementName = "data";
                var q = from c in configXml.Descendants(elementName)
                    select new UriEntry(
                        ConvertToString(c.Attribute("name")),
                        ConvertToUri(c.Element("value")),
                        ConvertToString(c.Element("Comment"))
                        );

                _uriEntries = q.ToDictionary(x => x.Id);
            }
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public UriEntry GetValue(string key)
        {
            UriEntry retval;
            _uriEntries.TryGetValue(key, out retval);

            return retval;
        }

        /// <summary>
        /// Sites the groups.
        /// </summary>
        /// <returns></returns>
        public IList<UriEntry> UriEntries()
        {
            IList<UriEntry> list = _uriEntries.Values.ToList();
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

    /// <summary>
    /// 
    /// </summary>
    public class UriEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UriEntry"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="helpUri">The help URI.</param>
        /// <param name="comment">The comment.</param>
        public UriEntry(string id, Uri helpUri, string comment)
        {
            Id = id;
            HelpUri = helpUri;
            Comment = comment;
        }


        /// <summary>
        /// The identifier
        /// </summary>
        public string Id;
        /// <summary>
        /// The help URI
        /// </summary>
        public Uri HelpUri;
        /// <summary>
        /// The comment
        /// </summary>
        public string Comment;

    }
}