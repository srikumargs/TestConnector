using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ConnectorServiceMonitor.Internal;
using ConnectorServiceMonitor.ViewModel;

namespace ConnectorServiceMonitor
{
    /// <summary>
    /// Parameters needed to create an instance of a ServerRegistration
    /// </summary>
    public sealed class ServerRegistrationParams
    {
        /// <summary>
        /// Initializes a new instance of the ServerRegistrationParams class
        /// </summary>
        /// <param name="registrySubKeyPath">The registry path (in the HKCU hive) where the registration should be stored (e.g. "SOFTWARE\Sage\&lt;My Product Name&gt;")</param>
        /// <param name="defaultCatalogServicePort">The port that should be treated as the default for the Catalog Service (i.e., this port will be used if no port is supplied by the user)</param>
        /// <param name="helpHandler"></param>
        public ServerRegistrationParams(String registrySubKeyPath, Int32 defaultCatalogServicePort, HelpHandler helpHandler)
        {
            _registrySubKeyPath = registrySubKeyPath;
            _defaultCatalogServicePort = defaultCatalogServicePort;
            if (helpHandler != null)
            {
                _helpHandler = helpHandler;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String RegistrySubKeyPath
        { get { return _registrySubKeyPath; } }

        /// <summary>
        /// 
        /// </summary>
        public Int32 DefaultCatalogServicePort
        { get { return _defaultCatalogServicePort; } }

        /// <summary>
        /// 
        /// </summary>
        public HelpHandler HelpHandler
        { get { return _helpHandler; } }

        private readonly String _registrySubKeyPath;
        private readonly Int32 _defaultCatalogServicePort;
        private readonly HelpHandler _helpHandler = delegate { };
    }
}
