
namespace Sage.Connector.ProcessExecution.AddinView.Events
{
    /// <summary>
    /// App Notification Event Args
    /// </summary>
    public abstract class AppNotificationEventArgs : System.EventArgs
    {
        /// <summary>
        /// The data of the app notification event.  
        /// </summary>
        public abstract string Data { get; }
    }
}
