
namespace ConnectorServiceMonitor.ViewModel
{
    /// <summary>
    /// The server connection state is used to indicate whether the server-mode
    /// ServiceHost is currently accessible and connected.
    /// </summary>
    public enum ServerConnectionState
    {
        /// <summary>
        /// The connected state is invalid (i.e., OperatingMode.Server)
        /// </summary>
        /// <remarks>
        /// This is default value the runtime automatically initializes any ServerConnectionState instance to
        /// 
        /// Only valid when not OperatingMode.Workstation
        /// </remarks>
        None = 0,

        /// <summary>
        /// The server is currently accessible
        /// </summary>
        /// <remarks>
        /// Only valid when OperatingMode.Workstation
        /// </remarks>
        Connected,

        /// <summary>
        /// The network is available but the server is, for some reason, not responding.  Typically indicates a server-side problem (e.g.,
        /// server machine not running or not connected to network).
        /// </summary>
        ServerNotResponding,

        /// <summary>
        /// The ip address of the server could not be resolved
        /// </summary>
        /// <remarks>
        /// Only valid when OperatingMode.Workstation
        /// </remarks>
        CouldNotResolveServer,

        /// <summary>
        /// The network is currently not accessible
        /// </summary>
        /// <remarks>
        /// Only valid when OperatingMode.Workstation
        /// </remarks>
        NetworkUnavailable,

        /// <summary>
        /// A special case of ServerNotResponding.  In this scenario, the server machine is reachable, but the Service Host service is not.  Typically
        /// indicates that the Service Host is not running on the server, or its ports are being firewalled.
        /// </summary>
        ServiceAddressNotFound
    }
}
