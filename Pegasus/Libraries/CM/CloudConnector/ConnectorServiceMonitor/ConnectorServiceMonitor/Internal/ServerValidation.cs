using System;
using System.Threading;
using System.Windows.Forms;
using ConnectorServiceMonitor.ViewModel;

namespace ConnectorServiceMonitor.Internal
{
    internal class ServerValidation
    {
        /// <summary>
        /// Validate a server name or address as a Sage server.
        /// </summary>
        /// <param name="serverRegistry"></param>
        /// <param name="server">The name or address to validate and set if valid</param>
        /// <param name="parent">The parent window</param>
        public static Boolean ValidateAndSetServer(IServerRegistry serverRegistry, String server, Control parent)
        {
            if (!server.Contains(":"))
            {
                server = String.Format("{0}:{1}", server, serverRegistry.DefaultCatalogServicePort);
            }

            Boolean result = false;

            Cursor oldCursor = parent.Cursor;
            ServerConnectionTestResult testResult;
            try
            {
                parent.Cursor = Cursors.WaitCursor;

                testResult = serverRegistry.TestCandidateServer(server);
            }
            finally
            {
                parent.Cursor = oldCursor;
            }

            switch (testResult.ServerConnectionState)
            {
                case ServerConnectionState.Connected:
                    RegistrationResponse regResponse = serverRegistry.RegisterServer(testResult.HostAndPort);
                    if ((regResponse & RegistrationResponse.Failed) != 0)
                    {
                        MessageBox.Show(parent, String.Format(Thread.CurrentThread.CurrentUICulture, Common.ReplaceKnownTerms(Strings.MACHINE_IS_NOT_A_SERVER_FORMAT), server), Strings.SERVER_ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        result = true;
                    }
                    break;
                case ServerConnectionState.CouldNotResolveServer:
                    MessageBox.Show(parent, String.Format(Thread.CurrentThread.CurrentUICulture, Strings.UNABLE_TO_FIND_SERVER_FORMAT, server), Strings.SERVER_ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case ServerConnectionState.NetworkUnavailable:
                    MessageBox.Show(parent, Strings.NETWORK_CONNECTION_NOT_FOUND, Strings.SERVER_ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case ServerConnectionState.ServerNotResponding:
                    MessageBox.Show(parent, String.Format(Thread.CurrentThread.CurrentUICulture, Strings.MACHINE_NOT_RESPONDING, server), Strings.SERVER_ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
                case ServerConnectionState.ServiceAddressNotFound:
                    MessageBox.Show(parent, String.Format(Thread.CurrentThread.CurrentUICulture, Common.ReplaceKnownTerms(Strings.MACHINE_DOES_NOT_HAVE_SERVER_FORMAT), server), Strings.SERVER_ERROR, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    break;
            }

            return result;
        }
    }
}
