using System;
using ConnectorServiceMonitor.ViewModel;

namespace ConnectorServiceMonitor.Internal
{
    internal sealed class ConfigureForLocalMachine
    {
        private static ServerRegistrationParams GetServerRegistrationParams()
        { return new ServerRegistrationParams(Common.ServerRegistrySubKeyPath, Common.DefaultCatalogServicePortNumber, null); }

        private static ServerRegistration GetServerRegistration()
        { return new ServerRegistration(GetServerRegistrationParams()); }

        private static ServerRegistry GetServerRegistry()
        { return new ServerRegistry(GetServerRegistrationParams()); }

        public static void Configure()
        {
            var serverRegistry = GetServerRegistry();
            if (!serverRegistry.IsConfigured)
            {
                string server = String.Format("{0}:{1}", "localhost", serverRegistry.DefaultCatalogServicePort);
                serverRegistry.RegisterServer(server);
            }
        }
    }
}
