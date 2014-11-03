using System.AddIn.Contract;
using Sage.Connector.ProcessExecution.AddinView;

namespace Sage.Connector.ProcessExecution.Interfaces
{
    /// <summary>
    /// The application contract interface
    /// </summary>
    public interface IAppContract : IContract
    {
        /// <summary>
        /// Application notification handler add method
        /// </summary>
        void AppNotificationEventAdd(IAppNotificationEventHandler handler);

        /// <summary>
        /// Application notification handler remove method
        /// </summary>
        void AppNotificationEventRemove(IAppNotificationEventHandler handler);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ILoggingContract GetLogger();
    }

    #region Events fired by the application

    /// <summary>
    /// App Notification Event Handler interface contract
    /// </summary>
    public interface IAppNotificationEventHandler : IContract
    {
        /// <summary>
        /// Handler method for the app notification
        /// </summary>
        /// <param name="args"><see cref="IAppNotificationEventArgs"/></param>
        void Handler(IAppNotificationEventArgs args);
    }

    /// <summary>
    /// App Notification Event Args interface contract
    /// </summary>
    public interface IAppNotificationEventArgs : IContract
    {
        /// <summary>
        /// App Notification data 
        /// </summary>
        string Data
        {
            get;
        }
    }
    #endregion
}