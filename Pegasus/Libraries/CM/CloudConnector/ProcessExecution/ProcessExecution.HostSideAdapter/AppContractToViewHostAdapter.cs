using System;
using System.AddIn.Pipeline;
using Sage.Connector.ProcessExecution.Interfaces;
using Sage.Connector.ProcessExecution.Interfaces.Events;

namespace Sage.Connector.ProcessExecution.HostSideAdapter
{
    /// <summary>
    /// App Contract to View Host Adapter
    /// </summary>
    public class AppContractToViewHostAdapter : IApp, IDisposable
    {
// ReSharper disable once InconsistentNaming
        private static readonly System.Reflection.MethodInfo _AppNotificationEventAddFire;

        private readonly IAppContract _contract;
        private readonly ContractHandle _handle;
        private readonly AppNotificationEventHandlerViewToContractHostAdapter _appNotificationHandler;
// ReSharper disable once InconsistentNaming
        private event EventHandler<AppNotificationEventArgs> _appNotification;


        // Track whether Dispose has been called. 
        private bool _disposed;

        /// <summary>
        /// App Notification event
        /// </summary>
        public event EventHandler<AppNotificationEventArgs> AppNotification
        {
            add
            {
                if (_appNotification == null)
                {
                    _contract.AppNotificationEventAdd(_appNotificationHandler);
                }
                _appNotification += value;
            }
            remove
            {
                _appNotification -= value;
                if (_appNotification == null)
                {
                    _contract.AppNotificationEventRemove(_appNotificationHandler);
                }
            }
        }
  
        /// <summary>
        /// Constructor 
        /// </summary>
        static AppContractToViewHostAdapter()
        {
            _AppNotificationEventAddFire = typeof(AppContractToViewHostAdapter).GetMethod("Fire_AppNotification", ((System.Reflection.BindingFlags)(36)));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="contract"><see cref="IAppContract"/></param>
        public AppContractToViewHostAdapter(IAppContract contract)
        {
            _contract = contract;
            _handle = new ContractHandle(contract);
            _appNotificationHandler = new AppNotificationEventHandlerViewToContractHostAdapter(this, _AppNotificationEventAddFire);
        }


        /// <summary>
        /// Fire the App Notification event
        /// </summary>
        /// <param name="args"><see cref="AppNotificationEventArgs"/></param>
        internal virtual void Fire_AppNotification(AppNotificationEventArgs args)
        {
            if ((_appNotification == null))
            {
            }
            else
            {
                _appNotification.Invoke(this, args);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ILogging GetLogger()
        {
            return LoggingHostAdapter.ContractToViewAdapter(_contract.GetLogger());
        }

        /// <summary>
        /// Get the App source contract
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
