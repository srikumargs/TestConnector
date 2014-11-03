using System;
using System.AddIn;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using Sage.Connector.DomainContracts.BackOffice;
using Sage.Connector.DomainMediator.Core;
using Sage.Connector.ProcessExecution.AddinView;
using Sage.Connector.ProcessExecution.AddinView.Events;
using BackOfficeCompanyConfiguration = Sage.Connector.ProcessExecution.AddinView.BackOfficeCompanyConfiguration;

namespace Sage.Connector.ProcessExecution.Addin
{
    /// <summary>
    /// The Process Execution Addin handles the request for any domain mediator feature request. 
    /// </summary>
    [AddIn("ProcessExecution", Version = "1.0.0.0")]
    public class ProcessExecution : IProcessRequest, IResponseHandler, IDisposable
    {
        [ImportMany]
#pragma warning disable 649
        private IEnumerable<Lazy<IDomainFeatureRequest, IFeatureMetaData>> _domainFeatureHandlers;
#pragma warning restore 649

        // Track whether Dispose has been called. 
        private bool _disposed;

        private readonly CompositionContainer _container;

        private IApp _app;
        private LoggerCore _logger;
        private PluginLogger _pluginLogger;
        private CancellationTokenSource _cancellationTokenSource;
        private PluginProcessContext _processContext;

        private EventHandler<AppNotificationEventArgs> _appNotificationHandler;

        /// <summary>
        /// The Response event handler containing <see cref="ResponseEventArgs"/>
        /// </summary>
        public event EventHandler<ResponseEventArgs> ProcessResponse;

        /// <summary>
        /// Constructor used to load the catalog of domain feature request handlers.  
        /// </summary>
        public ProcessExecution()
        {
            //Debugger.Break();
            //where to look for the primary domain mediation assemblies.
            string domainMediationPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            SafeDirectoryCatalog catalog = new SafeDirectoryCatalog(domainMediationPath);

            //Create the CompositionContainer with the parts in the catalog
            _container = new CompositionContainer(catalog.Catalog);

            //Fill the imports of this object
            try
            {
                _container.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                Debug.Print(compositionException.ToString());
                throw;
            }


        }

        /// <summary>
        /// Initialize the application
        /// </summary>
        /// <param name="appObj">The <see cref="IApp"/></param>
        public void Initialize(IApp appObj)
        {
            _app = appObj;
            _appNotificationHandler = HandleAppNotification;
            _app.AppNotification += _appNotificationHandler;

            //prep the session context parts
            _logger = _app.GetLogger();
            _pluginLogger = new PluginLogger(_logger);
            _cancellationTokenSource = new CancellationTokenSource();

            String noBlob = Environment.GetEnvironmentVariable("SAGE_CONNECTOR_NOBLOB", EnvironmentVariableTarget.Machine);
            uint entityCount = (!String.IsNullOrEmpty(noBlob) && noBlob == "1")
                ? uint.MaxValue
                : 100; // TODO: Hard-coded '100' entity chunk count as the 'unit of work' per response cycle
            _processContext = new PluginProcessContext(this, _pluginLogger, _logger, _cancellationTokenSource.Token, entityCount);
        }

        /// <summary>
        /// Cancellation was requested so trigger the cancellation token source.
        /// </summary>
        public void RequestCancellation()
        {
            _cancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Handle the application notification 
        /// </summary>
        /// <param name="sender">The sender of the notification</param>
        /// <param name="e">The <see cref="AppNotificationEventArgs"/></param>
        private void HandleAppNotification(object sender, AppNotificationEventArgs e)
        {
            Console.WriteLine(@"AddIn: Something happened, time to unsubscribe from this event");
            _app.AppNotification -= _appNotificationHandler;
        }

        /// <summary>
        /// Process the request from the cloud to the handling domain mediator and then the backoffice.
        /// </summary>
        /// <param name="requestId">The request id</param>
        /// <param name="tenantId">The tenant id</param>
        /// <param name="trackingId">The tracking identifier.</param>
        /// <param name="backOfficeCompanyConfiguration">back office company configuration <see cref="AddinView.BackOfficeCompanyConfiguration" /></param>
        /// <param name="featureId">The feature id</param>
        /// <param name="payload">The request payload</param>
        /// <exception cref="System.ApplicationException"></exception>
        public void ProcessRequest(Guid requestId, string tenantId, Guid trackingId, BackOfficeCompanyConfiguration backOfficeCompanyConfiguration, string featureId, string payload)
        {

            _logger.AdvanceActivityState(null, requestId, tenantId, trackingId, _logger.State8_InvokingDomainMediation, _logger.InProgress);


#if(DEBUG)
            _pluginLogger.Write(this, LogLevel.Verbose, "Enter ProcessRequest in new Process");
#endif
            Debug.Print("type name: {0}", GetType().Name);
            Debug.Print("type AssemblyQualifiedName: {0}", GetType().AssemblyQualifiedName);
            Debug.Print("x64 bit process {0}", Environment.Is64BitProcess);
            Debug.Print(".. message {0}", payload);

            //string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //string targetPath;
            //if (String.IsNullOrWhiteSpace(backOfficeCompanyConfiguration.BackOfficeId))
            //{
            //    targetPath = Path.Combine(basePath, "Discovery");

            //}
            //else
            //{
            //    targetPath = Path.Combine(basePath, "Plugins");
            //    targetPath = Path.Combine(targetPath, backOfficeCompanyConfiguration.BackOfficeId);
            //}
            //AssemblyResolver.AddResolvePath(targetPath);
            //AssemblyResolver.Enabled = true;

            _processContext.RequestId = requestId;
            _processContext.TenantId = tenantId;
            _processContext.TrackingId = trackingId;



            //TODO: look at if this needs to be faster.
            //TODO: look at how commands are bound. A spelling error in file prevented command from being found, with no error feedback
            foreach (Lazy<IDomainFeatureRequest, IFeatureMetaData> featureHandler in _domainFeatureHandlers)
            {
                if (featureHandler.Metadata.Name.Equals(featureId, StringComparison.CurrentCultureIgnoreCase))
                {
                    featureHandler.Value.FeatureRequest(_processContext, requestId, tenantId, backOfficeCompanyConfiguration, payload);

                    return;
                }
            }
            Debug.Print("{0} Domain Mediator for operation Not Found!", featureId);
            throw new ApplicationException(String.Format(" Domain Mediator for '{0}' operation Not Found!", featureId));
        }

        /// <summary>
        /// We may need more information returned for the response, depends on if the same process execution is going to handle more than one request at a time. 
        /// </summary>
        /// <param name="requestId">The request id</param>
        /// <param name="payload">The response payload</param>
        /// <param name="completed">True if this is the final response payload, otherwise false.</param>
        public void HandleResponse(Guid requestId, string payload, bool completed)
        {
            //we will come thru here multiple times for some requests. This will update the time stamp for each pass.
            //It will not tell time to first response unless we change the advance code.
            //TODO: maybe change the advance to keep first rather then last for a stamp?
            _logger.AdvanceActivityState(null, requestId, _processContext.TenantId, _processContext.TrackingId, _logger.State11_DomainMediationComplete, _logger.InProgress);

            ProcessResponse.Invoke(this, new ProcessResponseEventArgs(requestId, _processContext.TenantId, _processContext.TrackingId, payload, completed));
        }

        /// <summary>
        /// Process Response event args class is a convenience class to initialize the arguments
        /// from the constructor.
        /// </summary>
        internal class ProcessResponseEventArgs : ResponseEventArgs
        {
            private readonly string _payload;
            private readonly Guid _requestId;
            private readonly bool _completed;
            private readonly Guid _trackingId;
            private readonly string _tenantId;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="requestId">The request id</param>
            /// <param name="tenantId">The tenant identifier.</param>
            /// <param name="trackingId">The tracking identifier.</param>
            /// <param name="payload">The response payload</param>
            /// <param name="completed">True if the response if complete, otherwise false.</param>
            internal ProcessResponseEventArgs(Guid requestId, string tenantId, Guid trackingId, string payload, bool completed = true)
            {
                _payload = payload;
                _requestId = requestId;
                _completed = completed;
                _tenantId = tenantId;
                _trackingId = trackingId;
            }

            /// <summary>
            ///The response payload
            /// </summary>
            public override string Payload
            {
                get { return _payload; }
            }

            /// <summary>
            /// The request id
            /// </summary>
            public override Guid RequestId { get { return _requestId; } }

            /// <summary>
            /// True if the response if complete, otherwise false.
            /// </summary>
            public override bool Completed { get { return _completed; } }
            
            /// <summary>
            /// Gets the tenant identifier.
            /// </summary>
            public override string TenantId { get { return _tenantId; } }

            /// <summary>
            /// Gets the tracking identifier.
            /// </summary>
            public override Guid TrackingId { get { return _trackingId; } }
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
                    _container.Dispose();
                    _cancellationTokenSource.Dispose();
                }

                // Note disposing has been done.
                _disposed = true;

            }
        }
    }
}
