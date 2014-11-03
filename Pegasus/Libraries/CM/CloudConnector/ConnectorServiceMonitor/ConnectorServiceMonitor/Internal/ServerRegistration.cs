using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Sage.CRE.Core.UI;
using ConnectorServiceMonitor.Internal;
using ConnectorServiceMonitor.ViewModel;

namespace ConnectorServiceMonitor
{
    /// <summary>
    /// Class used for inspecting or modifing Server configuration
    /// </summary>
    public sealed class ServerRegistration
    {
        /// <summary>
        /// Initializes a new instance of the ServerRegistration class
        /// </summary>
        /// <param name="registrationParams"></param>
        public ServerRegistration(ServerRegistrationParams registrationParams)
        { _registrationParams = registrationParams; }

        /// <summary>
        /// Determine if a server has been established or not
        /// </summary>
        /// <param name="parentWindowHandle">Handle to the parent window. May be null</param>
        /// <returns>True if a server has been established, False if not</returns>
        public Boolean IsServerEstablished(IntPtr parentWindowHandle)
        {
            Boolean result = false;
            IServerRegistry serverRegistry = GetServerRegistry();
            result = serverRegistry.IsConfigured;
            if (!result)
            {
                HWnd hwnd = new HWnd(parentWindowHandle);
                using (var serverSelection = new ServerSelectionForm())
                {
                    serverSelection.Mode = ServerSelectionMode.InitializeServer;
                    serverSelection.ServerRegistry = serverRegistry;
                    if (serverSelection.ShowDialog(hwnd) == DialogResult.OK)
                    {
                        result = serverRegistry.IsConfigured;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Set the server to point to a different machine
        /// </summary>
        /// <param name="parentWindowHandle">Handle to the parent window. May be null</param>
        /// <returns>True, if the server was changed, False if not</returns>
        public Boolean ChangeServer(IntPtr parentWindowHandle)
        {
            Boolean result = false;
            using (var serverSelectionForm = new ServerSelectionForm())
            {
                var serverRegistry = GetServerRegistry();
                serverSelectionForm.Mode = serverRegistry.IsConfigured ? ServerSelectionMode.ChangeServer : ServerSelectionMode.InitializeServer;
                serverSelectionForm.ServerRegistry = serverRegistry;
                HWnd hwnd = new HWnd(parentWindowHandle);
                if (serverSelectionForm.ShowDialog(hwnd) == DialogResult.OK)
                {
                    result = true;
                }
            }
            return result;
        }

        /// <summary>
        /// The host name (or IP address) where the server can be located (via the CatalogService)
        /// </summary>
        public String Host
        { get { return GetServerRegistry().Host; } }

        /// <summary>
        /// The port number of the CatalogService running on the Host
        /// </summary>
        public Int32 CatalogServicePort
        { get { return GetServerRegistry().CatalogServicePort; } }

        /// <summary>
        /// 
        /// </summary>
        public Boolean RunOnLogin
        {
            get { return GetServerRegistry().RunOnLogin; }
            set { GetServerRegistry().RunOnLogin = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 AutoRefreshInterval
        {
            get { return GetServerRegistry().AutoRefreshInterval; }
            set { GetServerRegistry().AutoRefreshInterval = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 RequestsShowing
        {
            get { return GetServerRegistry().RequestsShowing; }
            set { GetServerRegistry().RequestsShowing = value; }
        }



        private IServerRegistry GetServerRegistry()
        { return new ServerRegistry(_registrationParams); }

        private readonly ServerRegistrationParams _registrationParams;
    }
}
