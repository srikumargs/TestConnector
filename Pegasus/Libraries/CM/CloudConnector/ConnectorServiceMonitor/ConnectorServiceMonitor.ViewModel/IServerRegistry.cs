using System;

namespace ConnectorServiceMonitor.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    public enum HelpContext
    {
        /// <summary>
        /// 
        /// </summary>
        None = 0,

        /// <summary>
        /// 
        /// </summary>
        EnterIPForm,

        /// <summary>
        /// 
        /// </summary>
        NetworkBrowserForm,

        /// <summary>
        /// 
        /// </summary>
        ServerSelectionForm_Initialize,

        /// <summary>
        /// 
        /// </summary>
        ServerSelectionForm_Change,

        /// <summary>
        /// 
        /// </summary>
        SpecifyServer_WhatIsServer,

        /// <summary>
        /// 
        /// </summary>
        MainForm
    }

    /// <summary>
    /// 
    /// </summary>
    public sealed class ServerHelpEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="helpEventArgs"></param>
        public ServerHelpEventArgs(HelpContext context, EventArgs helpEventArgs)
        {
            _context = context;
            _helpEventArgs = helpEventArgs;
        }

        /// <summary>
        /// 
        /// </summary>
        public HelpContext Context
        { get { return _context; } }

        /// <summary>
        /// 
        /// </summary>
        public EventArgs HelpEventArgs
        { get { return _helpEventArgs; } }

        private readonly HelpContext _context;
        private readonly EventArgs _helpEventArgs;
    }

    /// <summary>
    /// Delegate for any help handler callbacks.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="hlpevent"></param>
    public delegate void HelpHandler(object sender, ServerHelpEventArgs hlpevent);

    /// <summary>
    /// Provides a mechanism for external clients to request that the local service host reconfigure itself
    /// </summary>
    public interface IServerRegistry
    {
        /// <summary>
        /// Returns whether the local machine is currently configured to point to a server-mode host
        /// </summary>
        Boolean IsConfigured
        { get; }

        /// <summary>
        /// Attempt to contact a candidate host and return whether or not it currently has server-mode functionality running.
        /// </summary>
        /// <param name="hostAndPort">host name or ip address to test</param>
        /// <returns></returns>
        ServerConnectionTestResult TestCandidateServer(String hostAndPort);

        /// <summary>
        /// Change the server-mode host that will be contacted by the local machine
        /// </summary>
        /// <param name="hostAndPort">host name or ip address to test</param>
        /// <returns></returns>
        RegistrationResponse RegisterServer(String hostAndPort);

        /// <summary>
        /// The host name (or IP address) where the server can be located (via the CatalogService)
        /// </summary>
        String Host
        { get; }

        /// <summary>
        /// The port number of the CatalogService running on the Host
        /// </summary>
        Int32 CatalogServicePort
        { get; }

        /// <summary>
        /// The port that should be treated as the default for the Catalog Service (i.e., this port will be used if no port is supplied by the user)
        /// </summary>
        Int32 DefaultCatalogServicePort
        { get; }

        /// <summary>
        /// 
        /// </summary>
        HelpHandler HelpHandler
        { get; }

        /// <summary>
        /// 
        /// </summary>
        Boolean RunOnLogin
        { get; set; }

        /// <summary>
        /// 
        /// </summary>
        Int32 AutoRefreshInterval
        { get; set; }

        /// <summary>
        /// 
        /// </summary>
        Int32 RequestsShowing
        { get; set; }
    }
}
