using System;
using System.Linq;
using System.Reflection;
using Sage.Connector.Cloud.Integration.Interfaces;
using Sage.Connector.Cloud.Integration.Interfaces.DataContracts;

namespace Sage.Connector.MessagingService.Internal
{
    internal static class RemoteConfigHelper
    {
        /// <summary>
        /// Ensure that the conrfig params object is considered valid for our needs
        /// Just check the minimum: that it is not null, and has normal performance params
        /// </summary>
        /// <param name="configParams"></param>
        /// <returns></returns>
        public static bool IsValidConfigParams(Sage.Connector.Cloud.Integration.Interfaces.WebAPI.Configuration configParams)
        {
            bool isValid =
                configParams != null &&
                null != configParams.ConfigurationBaseUri &&
                !String.IsNullOrEmpty(configParams.ConfigurationBaseUri.ToString()) &&
                !String.IsNullOrEmpty(configParams.ConfigurationResourcePath) &&

                null != configParams.RequestBaseUri &&
                !String.IsNullOrEmpty(configParams.RequestBaseUri.ToString()) &&
                !String.IsNullOrEmpty(configParams.RequestResourcePath) &&

                !String.IsNullOrEmpty(configParams.RequestUploadResourcePath) &&
                !String.IsNullOrEmpty(configParams.ResponseUploadResourcePath) &&

                null != configParams.ResponseBaseUri &&
                !String.IsNullOrEmpty(configParams.ResponseBaseUri.ToString()) &&
                !String.IsNullOrEmpty(configParams.ResponseResourcePath) &&

                null != configParams.NotificationResourceUri &&
                !String.IsNullOrEmpty(configParams.NotificationResourceUri.ToString()) &&

                null != configParams.SiteAddressBaseUri &&
                !String.IsNullOrEmpty(configParams.SiteAddressBaseUri.ToString());

            return isValid;
        }

        /// <summary>
        /// Convert an object to a string enumerating its params names and values
        /// Will go one extra level deep for sub types that are defined in the same assembly
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="isTopLevel"></param>
        /// <returns></returns>
        public static string ObjectPropertiesToString(object obj, bool isTopLevel = true)
        {
            string result = string.Empty;

            try
            {
                if (obj != null)
                {
                    Type type = obj.GetType();
                    PropertyInfo[] propertyInfos = type.GetProperties();
                    result = type.Name;

                    if (propertyInfos != null)
                    {
                        // Check each property in the type
                        foreach (PropertyInfo propertyInfo in propertyInfos)
                        {
                            // Get the value of this property for this object
                            string propertyValue = "null";
                            object propertyValueObj = null;
                            try
                            {
                                propertyValueObj = propertyInfo.GetValue(obj, null);
                            }
                            catch
                            {
                                // Catch any reflection issues
                            }

                            // Property value is set
                            if (propertyValueObj != null)
                            {

                                if (isTopLevel && propertyInfo.PropertyType.Assembly == type.Assembly)
                                {
                                    // We're at the top level, and this is a type defined in the same assembly
                                    // Try to conver it as well
                                    propertyValue = ObjectPropertiesToString(propertyValueObj, false);
                                }
                                else
                                {
                                    try
                                    {
                                        // Get the string value of the object
                                        propertyValue = propertyValueObj.ToString();
                                    }
                                    catch
                                    {
                                        // Catch any conversion issues
                                    }
                                }
                            }

                            // Write out the name and value for this property
                            result += String.Format("{0}{1}{2}: {3}",
                                Environment.NewLine,
                                (isTopLevel) ? string.Empty : "\t",
                                propertyInfo.Name,
                                propertyValue);
                        }
                    }
                }
            }
            catch
            {
                // This method is just for logging, so to be sure it does not cause
                // Any issues, just catch all
            }

            return result;
        }
    }
}