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
    [DataContract(Namespace = ServiceConstants.V1_SERVICE_NAMESPACE, Name = "BackOfficePluginInformationContract")]
    public sealed class BackOfficePluginInformation : IExtensibleDataObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BackOfficePluginInformation"/> class.
        /// </summary>
        /// <param name="pluginId">The plugin identifier.</param>
        /// <param name="productName">Name of the product.</param>
        /// <param name="integrationInterfacesVersion">The integration interfaces version.</param>
        /// <param name="productPluginFileVersion">The product plugin file version.</param>
        /// <param name="productVersion">The product version.</param>
        /// <param name="autoUpdateProductId">The automatic update product identifier.</param>
        /// <param name="autoUpdateProductVersion">The automatic update product version.</param>
        /// <param name="autoUpdateComponentBaseName">Name of the automatic update component base.</param>
        /// <param name="runAsUserRequired">The run as user required.</param>
        /// <param name="platform">The platform.</param>
        public BackOfficePluginInformation(
            String pluginId,
            String productName,
            String integrationInterfacesVersion,
            String productPluginFileVersion,
            String productVersion,
            String autoUpdateProductId,
            String autoUpdateProductVersion,
            String autoUpdateComponentBaseName,
            Boolean runAsUserRequired,
            String platform
            )
        {
            PluginId = pluginId;
            ProductName = productName;
            IntegrationInterfaceVersion = integrationInterfacesVersion;
            ProductPluginFileVersion = productPluginFileVersion;
            ProductVersion = productVersion;

            AutoUpdateProductId = autoUpdateProductId;
            AutoUpdateProductVersion = autoUpdateProductVersion;
            AutoUpdateComponentBaseName = autoUpdateComponentBaseName;
            RunAsUserRequried = runAsUserRequired;
            Platform = platform;
        }
                
        /// <summary>
        /// Initializes a new instance of the BackOfficePluginInformation class from an existing instance and a collection of propertyTuples
        /// </summary>
        /// <param name="source"></param>
        /// <param name="propertyTuples"></param>
        public BackOfficePluginInformation(BackOfficePluginInformation source, IEnumerable<PropertyTuple> propertyTuples)
        {
            PluginId = source.PluginId;
            ProductName = source.ProductName;
            IntegrationInterfaceVersion = source.IntegrationInterfaceVersion;
            ProductPluginFileVersion = source.ProductPluginFileVersion;
            ProductVersion = source.ProductVersion;

            AutoUpdateProductId = source.AutoUpdateProductId;
            AutoUpdateProductVersion = source.AutoUpdateProductVersion;
            AutoUpdateComponentBaseName = source.AutoUpdateComponentBaseName;
            RunAsUserRequried = source.RunAsUserRequried;
            Platform = source.Platform;


            ExtensionData = source.ExtensionData;

            var myPropertyTuples = propertyTuples.Where(x => x.Item1.DeclaringType == typeof(BackOfficePluginInformation));
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
        [DataMember(Name = "ProductName", IsRequired = true, Order = 1)]
        public String ProductName { get; private set; }
        
        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "IntegrationInterfaceVersion", IsRequired = true, Order = 2)]
        public String IntegrationInterfaceVersion { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "ProductPluginFileVersion", IsRequired = true, Order = 3)]
        public String ProductPluginFileVersion { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "ProductVersion", IsRequired = true, Order = 4)]
        public String ProductVersion { get; private set; }

        /// <summary>
        /// Gets the automatic update product identifier.
        /// </summary>
        /// <value>
        /// The automatic update product identifier.
        /// </value>
        [DataMember(Name = "AutoUpdateProductId", IsRequired = true, Order = 5)]
        public String AutoUpdateProductId { get; private set; }
        /// <summary>
        /// Gets the automatic update product version.
        /// </summary>
        /// <value>
        /// The automatic update product version.
        /// </value>
        [DataMember(Name = "AutoUpdateProductVersion", IsRequired = true, Order = 6)]
        public String AutoUpdateProductVersion { get; private set; }
        /// <summary>
        /// Gets the name of the automatic update component base.
        /// </summary>
        /// <value>
        /// The name of the automatic update component base.
        /// </value>
        [DataMember(Name = "AutoUpdateComponentBaseName", IsRequired = true, Order = 7)]
        public String AutoUpdateComponentBaseName { get; private set; }
        /// <summary>
        /// Gets a value indicating whether [run as user requried].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [run as user requried]; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "RunAsUserRequried", IsRequired = true, Order = 8)]
        public bool RunAsUserRequried { get; private set; }
        /// <summary>
        /// Gets the platform.
        /// </summary>
        /// <value>
        /// The platform.
        /// </value>
        [DataMember(Name = "Platform", IsRequired = true, Order = 9)]
        public String Platform { get; private set; }

        /// <summary>
        /// To support forward-compatible data contracts
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }
    }
}
