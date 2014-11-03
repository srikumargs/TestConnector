using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Sage.Connector.Common;
using Sage.Connector.LinkedSource;

namespace Sage.Connector.StateService.Interfaces.DataContracts
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Namespace = ServiceConstants.V1_SERVICE_NAMESPACE, Name = "BackOfficePluginContract")]
    public sealed class BackOfficePlugin : IExtensibleDataObject
    {
        /// <summary>
        /// TODO KMS: Need more information.. so this looks more like and replaces the existing BackOfficePluginInformation class
        /// </summary>
        public BackOfficePlugin(
            String pluginId, 
            String backofficeProductName,
            String platform,
            String helpUri,
            String backOfficeVersion,
            String loginAdministratorTerm,
            ApplicationSecurityMode applicationSecurityMode,
            String backOfficePluginAutoUpdateProductId,
            String backOfficePluginAutoUpdateProductVersion,
            String backOfficePluginAutoAutoUpdateComponentBaseName,
            String pluginVersion,
            bool runAsUserIsRequried
            )
        {
            PluginId = pluginId;
            BackofficeProductName = backofficeProductName;
            Platform = platform;
            HelpUri = helpUri;
            BackOfficeVersion = backOfficeVersion;

            LoginAdministratorTerm = loginAdministratorTerm;
            ApplicationSecurityMode = applicationSecurityMode;
            BackOfficePluginAutoUpdateProductId =  backOfficePluginAutoUpdateProductId;

            BackOfficePluginAutoUpdateProductVersion = backOfficePluginAutoUpdateProductVersion;
            BackOfficePluginAutoUpdateComponentBaseName = backOfficePluginAutoAutoUpdateComponentBaseName;

            PluginVersion = pluginVersion;
            RunAsUserIsRequried = runAsUserIsRequried;
        }

        /// <summary>
        /// Initializes a new instance of the BackOfficeConnection class from an existing instance and a collection of propertyTuples
        /// </summary>
        /// <param name="source"></param>
        /// <param name="propertyTuples"></param>
        public BackOfficePlugin(BackOfficePlugin source, IEnumerable<PropertyTuple> propertyTuples)
        {
            PluginId = source.PluginId;
            BackofficeProductName = source.BackofficeProductName;
            Platform = source.Platform;
            HelpUri = source.HelpUri;
            BackOfficeVersion = source.BackOfficeVersion;

            LoginAdministratorTerm = source.LoginAdministratorTerm;
            ApplicationSecurityMode = source.ApplicationSecurityMode;

            BackOfficePluginAutoUpdateProductId = source.BackOfficePluginAutoUpdateProductId;
            BackOfficePluginAutoUpdateProductVersion = source.BackOfficePluginAutoUpdateProductVersion;
            BackOfficePluginAutoUpdateComponentBaseName = source.BackOfficePluginAutoUpdateComponentBaseName;

            PluginVersion = source.PluginVersion;
            RunAsUserIsRequried = source.RunAsUserIsRequried;
            
            ExtensionData = source.ExtensionData;

            var myPropertyTuples = propertyTuples.Where(x => x.Item1.DeclaringType == typeof(BackOfficePlugin));
            foreach (var tuple in myPropertyTuples)
            {
                tuple.Item1.SetValue(this, tuple.Item2, null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "PluginId", IsRequired = true, Order = 0)]
        public String PluginId { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "BackofficeProductName", IsRequired = true, Order = 1)]
        public string BackofficeProductName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "Platform", IsRequired = true, Order = 2)]
        public string Platform { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "HelpUri", IsRequired = true, Order = 3)]
        public string HelpUri { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "BackOfficeVersion", IsRequired = true, Order = 4)]
        public string BackOfficeVersion { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "NameValuePairs", IsRequired = true, Order = 5)]
        public string NameValuePairs { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "LoginAdministratorTerm", IsRequired = true, Order = 5)]
        public string LoginAdministratorTerm { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "ApplicationSecurityMode", IsRequired = true, Order = 5)]
        public ApplicationSecurityMode ApplicationSecurityMode { get; set; }

        /// <summary>
        /// Gets or sets the back office plugin automatic update product identifier.
        /// </summary>
        /// <value>
        /// The back office plugin automatic update product identifier.
        /// </value>
        [DataMember(Name = "BackOfficePluginAutoUpdateProductId", IsRequired = true, Order = 5)]
        public string BackOfficePluginAutoUpdateProductId { get; set; }

        /// <summary>
        /// Gets or sets the back office plugin automatic update product version.
        /// </summary>
        /// <value>
        /// The back office plugin automatic update product version.
        /// </value>
        [DataMember(Name = "BackOfficePluginAutoUpdateProductVersion", IsRequired = true, Order = 5)]
        public string BackOfficePluginAutoUpdateProductVersion { get; set; }

        /// <summary>
        /// Gets or sets the name of the back office plugin automatic update component base.
        /// </summary>
        /// <value>
        /// The name of the back office plugin automatic update component base.
        /// </value>
        [DataMember(Name = "BackOfficePluginAutoUpdateComponentBaseName", IsRequired = true, Order = 5)]
        public string BackOfficePluginAutoUpdateComponentBaseName { get; set; }

        /// <summary>
        /// Gets or sets the plugin version.
        /// </summary>
        /// <value>
        /// The plugin version.
        /// </value>
        [DataMember(Name = "PluginVersion", IsRequired = true, Order = 6)]
        public string PluginVersion { get; set; }

        /// <summary>
        /// Gets or sets the run as user is requried.
        /// </summary>
        /// <value>
        /// The run as user is requried.
        /// </value>
        [DataMember(Name = "RunAsUserIsRequried", IsRequired = true, Order = 7)]
        public bool RunAsUserIsRequried { get; set; }


        /// <summary>
        /// To support forward-compatible data contracts
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }

     }
}
