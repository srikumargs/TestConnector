using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using Sage.Connector.Common;
using Sage.Connector.Management;

namespace SageConnect.ViewModels
{
    /// <summary>
    /// 
    /// </summary>
    public static class HelpLinkManager
    {

        /// <summary>
        /// Finds the string.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public static string FindString(string key)
        {
            Uri entry = FindUri(key);
            string retval = null;
            if (entry != null)
            {
                retval = entry.ToString();
            }
            return retval;
        }

        /// <summary>
        /// Finds the URI.
        /// </summary>
        /// <param name="key">The identifier.</param>
        /// <returns></returns>
        public static Uri FindUri(string key)
        {
            UriEntry entry = FindUriEntry(key);
            Uri retval = null;
            if (entry != null)
            {
                retval = entry.HelpUri;
            }
            return retval;
        }

        private static UriEntry FindUriEntry(string key)
        {
            var loader = new HelpLinkLoader(_baseFileName, _extension);
            loader.ReadConfig();
            UriEntry entry = loader.GetValue(key);
            return entry;
        }

        private static string _baseFileName = "SageConnect.ViewModels.HelpLinks.";
        private static string _extension = "resx";
    }
}
