
namespace Sage.Connector.ProcessExecution.Interfaces.Events
{
    /// <summary>
    /// App Notification Event Args abstract class
    /// </summary>
    public abstract class AppNotificationEventArgs : System.EventArgs
    {
        /// <summary>
        /// App Notification Data
        /// </summary>
        public abstract string Data
        {
            get;
        }
    }
}
