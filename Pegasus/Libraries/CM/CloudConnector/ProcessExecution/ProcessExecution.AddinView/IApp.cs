using Sage.Connector.ProcessExecution.AddinView.Events;


namespace Sage.Connector.ProcessExecution.AddinView
{
    /// <summary>
    /// The App AddIn View Interface
    /// </summary>
    public interface IApp
    {
        /// <summary>
        /// The App Notification Event 
        /// </summary>
        event System.EventHandler<AppNotificationEventArgs> AppNotification;

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        LoggerCore GetLogger();
    }
}
