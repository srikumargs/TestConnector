using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Xml;
using Sage.Connector.Common;
using CloudDataContracts = Sage.Connector.Cloud.Integration.Interfaces.DataContracts;

namespace Sage.Connector.ConnectorServiceCommon
{
    /// <summary>
    /// General utility helper class
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// A test upload session info needed by the mock cloud service
        /// And by the put response worker to identify a mock test
        /// And avoid trying to talk to cloud storage
        /// </summary>
        public static CloudDataContracts.UploadSessionInfo MockUploadSessionInfo
        { get { return _mockUploadSessionInfo; } }

        private static readonly CloudDataContracts.UploadSessionInfo _mockUploadSessionInfo =
            new CloudDataContracts.UploadSessionInfo("mockSessionKey", "mockBlobName", new Uri("http://www.test.com"), string.Empty, 250*1024);

        /// <summary>
        /// A test download session info needed by the mock cloud service
        /// And by the put response worker to identify a mock test
        /// And avoid trying to talk to cloud storage
        /// </summary>
        public static CloudDataContracts.DownloadSessionInfo MockDownloadSessionInfo
        { get { return _mockDownloadSessionInfo; } }

        private static readonly CloudDataContracts.DownloadSessionInfo _mockDownloadSessionInfo =
            new CloudDataContracts.DownloadSessionInfo("mockSessionKey", "mockBlobName", new Uri("http://www.test.com"), string.Empty, 0);

        /// <summary>
        /// Serialize an object using the DataContractJsonSerializer
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string JsonSerialize(object item)
        {
            using (var ms = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(item.GetType());
                serializer.WriteObject(ms, item);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }

        /// <summary>
        /// Deserialize a string into an object using the DataContractJsonSerializer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public static T JsonDeserialize<T>(String item)
        {
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(item)))
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                return (T)serializer.ReadObject(ms);
            }
        }

        /// <summary>
        /// Deserialize a string into an object using the DataContractJsonSerializer
        /// </summary>
        /// <param name="t"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static object JsonDeserialize(Type t, String item)
        {
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(item)))
            {
                var serializer = new DataContractJsonSerializer(t);
                return serializer.ReadObject(ms);
            }
        }

        /// <summary>
        /// Helper to convert a string to base 64 representation
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToBase64(string value)
        {
            try
            {
                Byte[] baseUriBytes = Encoding.UTF8.GetBytes(value);
                string base64String = System.Convert.ToBase64String(baseUriBytes, Base64FormattingOptions.None);
                return base64String;
            }
            catch
            {
                // Supplied nothing
                return null;
            }
        }

        /// <summary>
        /// Helper to convert from base 64 representation
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string FromBase64(string value)
        {
            try
            {
                Byte[] base64String = System.Convert.FromBase64String(value);
                String uriAsString = Encoding.UTF8.GetString(base64String);
                return uriAsString;
            }
            catch
            {
                // Supplied nothing or invalid base 64 formatting
                return null;
            }
        }

        /// <summary>
        /// The path to the binder plugins
        /// </summary>
        public static String BinderPluginsPath
        { get { return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "BinderPlugins"); } }

        /// <summary>
        /// The path to store documents for the document manager
        /// </summary>
        public static String DocumentManagerPath
        { get { return Path.Combine(ConnectorServiceUtils.InstanceApplicationDataFolder, "Documents"); } }

        /// <summary>
        /// 
        /// </summary>
        public static MutexSecurity AllowEveryoneMutexSecurity
        {
            get
            {
                MutexSecurity result = new MutexSecurity();
                result.AddAccessRule(new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), MutexRights.Synchronize | MutexRights.Modify, AccessControlType.Allow));
                return result;
            }
        }
    }
}
