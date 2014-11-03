using Sage.Connector.ProcessExecution.Interfaces.Events;

namespace Sage.Connector.ProcessExecution.RequestActivator
{

    /// <summary>
    /// Cancel notification event args. 
    /// Currently it is a simplistic string, but could contain more information 
    /// depending on need. 
    /// </summary>
    // ReSharper disable once InconsistentNaming
    public class ConnectorNotificationEventArgs : AppNotificationEventArgs
    {
        /// <summary>
        /// Get the data for the cancel notification
        /// TODO KMS: expand on this simple implementation
        /// </summary>
        public override string Data
        {
            get { return "Cancel"; }
        }
    }
}
