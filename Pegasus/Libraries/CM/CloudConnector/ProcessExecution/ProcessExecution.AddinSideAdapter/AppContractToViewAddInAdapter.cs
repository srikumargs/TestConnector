using System;
using Sage.Connector.ProcessExecution.AddinView;
using Sage.Connector.ProcessExecution.AddinView.Events;
using Sage.Connector.ProcessExecution.Interfaces;

namespace Sage.Connector.ProcessExecution.AddinSideAdapter
{
    /// <summary>
    /// AppContract to View AddIn Adapter
    /// </summary>
    public class AppContractToViewAddInAdapter : IApp, IDisposable
    {

        // Track whether Dispose has been called. 
        private bool _disposed;

        private readonly IAppContract _contract;

        private readonly System.AddIn.Pipeline.ContractHandle _handle;

        private readonly AppNotificationEventHandlerViewToContractAddInAdapter _appNotificationHandler;

// ReSharper disable once InconsistentNaming
        private static readonly System.Reflection.MethodInfo _AppNotificationEventAddFire;

        /// <summary>
        /// Application Notification event adds and removes event handlers
        /// </summary>
        public event EventHandler<AppNotificationEventArgs> AppNotification
        {
           
            add
            {
                if (_AppNotification == null)
                {
                    _contract.AppNotificationEventAdd(_appNotificationHandler);
                }
                _AppNotification += value;
            }
            remove
            {
                _AppNotification -= value;
                if (_AppNotification == null)
                {
                    _contract.AppNotificationEventRemove(_appNotificationHandler);
                }
            }
        }

        /// <summary>
        /// Contract to View AddIn Adapter
        /// </summary>
        static AppContractToViewAddInAdapter()
        {
            _AppNotificationEventAddFire = typeof(AppContractToViewAddInAdapter).GetMethod("Fire_AppNotification", ((System.Reflection.BindingFlags)(36)));
        }

        /// <summary>
        /// App Contract to View AddIn Adapter
        /// </summary>
        /// <param name="contract"><see cref="IAppContract"/></param>
        public AppContractToViewAddInAdapter(IAppContract contract)
        {
            _contract = contract;
            _handle = new System.AddIn.Pipeline.ContractHandle(contract);
            _appNotificationHandler = new AppNotificationEventHandlerViewToContractAddInAdapter(this, _AppNotificationEventAddFire);
        }

// ReSharper disable once InconsistentNaming
        private event EventHandler<AppNotificationEventArgs> _AppNotification;

        /// <summary>
        /// Fire the App Notification event
        /// </summary>
        /// <param name="args"></param>
        internal virtual void Fire_AppNotification(AppNotificationEventArgs args)
        {
            if ((_AppNotification == null))
            {
            }
            else
            {
                _AppNotification.Invoke(this, args);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public LoggerCore GetLogger()
        {
            return LoggingAddInAdapter.ContractToViewAdapter(_contract.GetLogger());
        }

        /// <summary>
        /// The the Application contract from this adapter
        /// </summary>
        /// <returns><see cref="IAppContract"/></returns>
        internal IAppContract GetSourceContract()
        {
            return _contract;
        }

        /// <summary>
        /// Dispose properly of this instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method. 
            // Therefore, you should call GC.SupressFinalize to 
            // take this object off the finalization queue 
            // and prevent finalization code for this object 
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios. 
        /// If disposing equals true, the method has been called directly 
        /// or indirectly by a user's code. Managed and unmanaged resources 
        /// can be disposed. 
        /// If disposing equals false, the method has been called by the 
        /// runtime from inside the finalizer and you should not reference 
        /// other objects. Only unmanaged resources can be disposed. 
        /// </summary>
        /// <param name="disposing">When false, the method has been called by the 
        /// runtime from inside the finalizer and you should not reference 
        /// other objects. Only unmanaged resources can be disposed. 
        ///  </param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called. 
            if (!_disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources. 
                if (disposing)
                {
                    // Dispose managed resources.
                    _handle.Dispose();
                }

                // Note disposing has been done.
                _disposed = true;

            }
        }

    }
}
