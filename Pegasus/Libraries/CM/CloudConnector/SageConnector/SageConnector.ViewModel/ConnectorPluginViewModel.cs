using System;
using System.IO;
using System.Linq;
using Sage.Configuration;
using Sage.Connector.Common;
using Sage.Connector.StateService.Interfaces.DataContracts;

namespace SageConnector.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class ConnectorPlugin
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectorPlugin" /> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="pluggedInProductName">Name of the plugged in product.</param>
        /// <param name="platform">The platform.</param>
        /// <param name="helpBaseUrl">The help base URL.</param>
        /// <param name="loginAdministratorTerm">The login administrator term.</param>
        /// <param name="pluginRelativeLocation">The plugin relative location.</param>
        /// <param name="pluginRoot">The plugin root.</param>
        /// <param name="applicationSecurityMode">The application security mode.</param>
        /// <param name="productVersion">The product version.</param>
        /// <param name="pluginAutoUpdateProductId">The plugin automatic update product identifier.</param>
        /// <param name="pluginAutoUpdateProductVersion">The plugin automatic update product version.</param>
        /// <param name="pluginAutoUpdateComponentBaseName">Name of the plugin automatic update component base.</param>
        public ConnectorPlugin(
            String id,
            String pluggedInProductName,
            String platform,
            String helpBaseUrl,
            String loginAdministratorTerm,
            String pluginRelativeLocation,
            String pluginRoot,
            ApplicationSecurityMode applicationSecurityMode,
            String productVersion,
            String pluginAutoUpdateProductId,
            String pluginAutoUpdateProductVersion,
            String pluginAutoUpdateComponentBaseName
            )
        {
            Id = id;
            PluggedInProductName = pluggedInProductName;
            Platform = platform;
            HelpBaseUrl = helpBaseUrl;
            LoginAdministratorTerm = loginAdministratorTerm;
            PluginRelativeLocation = pluginRelativeLocation;
            PluginRoot = pluginRoot;
            ProductVersion = productVersion;

            ApplicationSecurityMode = applicationSecurityMode;
            
            PluginProductId = pluginAutoUpdateProductId;
            PluginProductVersion = pluginAutoUpdateProductVersion;
            PluginComponetBaseName = pluginAutoUpdateComponentBaseName;
        }

        /// <summary>
        /// 
        /// </summary>
        public String Id { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public String PluggedInProductName { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public String Platform { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public String HelpBaseUrl { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public String LoginAdministratorTerm { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public String PluginRelativeLocation { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public String PluginRoot { get; private set; }

        /// <summary>
        /// Gets the product version.
        /// </summary>
        /// <value>
        /// The product version.
        /// </value>
        public String ProductVersion { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is installed.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is installed; otherwise, <c>false</c>.
        /// </value>
        public bool IsInstalled {
            get { return !String.IsNullOrWhiteSpace(ProductVersion); } 
        }

        /// <summary>
        /// 
        /// </summary>
        public String PluginProductId { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public String PluginProductVersion { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public String PluginComponetBaseName { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ApplicationSecurityMode ApplicationSecurityMode
        { get; private set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class ConnectorPluginsCollection : SortableBindingList<ConnectorPlugin>
    {

    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class ConnectorPluginsViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public static ConnectorPluginsCollection GetConnectorPlugins()
        {
            if (_pluginsCollection == null || !_pluginsCollection.Any())
                _pluginsCollection = new ConnectorPluginsCollection();
            return _pluginsCollection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pluginsCollection"></param>
        public static void SetConnectorPlugins(ConnectorPluginsCollection pluginsCollection)
        {
            _pluginsCollection = pluginsCollection;
        }
        private static ConnectorPluginsCollection _pluginsCollection = null;
    }
}
