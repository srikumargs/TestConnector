using System;

namespace ConnectorServiceMonitor.ViewModel
{
    /// <summary>
    /// A result object which returns information regarding a server mode host connection test.
    /// </summary>
    [Serializable]
    public class ServerConnectionTestResult
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverConnectionState"></param>
        /// <param name="serverNameUsageRecommendation"></param>
        /// <param name="hostName"></param>
        /// <param name="serviceAddress"></param>
        public ServerConnectionTestResult(ServerConnectionState serverConnectionState, ServerNameUsageRecommendation serverNameUsageRecommendation, String hostName, String serviceAddress)
        {
            _serverConnectionState = serverConnectionState;
            _serverNameUsageRecommendation = serverNameUsageRecommendation;
            _hostAndPort = hostName;
            _serviceAddress = serviceAddress;
        }

        /// <summary>
        /// Whether the server-mode ServiceHost is currently accessible and connected.
        /// </summary>
        public ServerConnectionState ServerConnectionState
        { get { return _serverConnectionState; } }

        /// <summary>
        /// Indicates how the ServerName should be provided.
        /// </summary>
        public ServerNameUsageRecommendation ServerNameUsageRecommendation
        { get { return _serverNameUsageRecommendation; } }

        /// <summary>
        /// Host name or ip address of server-mode host
        /// </summary>
        public String HostAndPort
        { get { return _hostAndPort; } }

        /// <summary>
        /// Returns the address of the service
        /// </summary>
        public String ServiceAddress
        { get { return _serviceAddress; } }

        private ServerConnectionState _serverConnectionState = ServerConnectionState.None;
        private ServerNameUsageRecommendation _serverNameUsageRecommendation = ServerNameUsageRecommendation.None;
        private String _hostAndPort = String.Empty;
        private String _serviceAddress = String.Empty;
    }
}
