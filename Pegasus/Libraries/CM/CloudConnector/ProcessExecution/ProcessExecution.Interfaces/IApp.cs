using Sage.Connector.ProcessExecution.Interfaces.Events;

namespace Sage.Connector.ProcessExecution.Interfaces
{
    /// <summary>
    /// The App interface
    /// </summary>
    public interface IApp
    {
        /// <summary>
        /// The App Notification event
        /// </summary>
        event System.EventHandler<AppNotificationEventArgs> AppNotification;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ILogging GetLogger();
    }

}
