using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceModel;
using Sage.Connector.AutoUpdate;
using Sage.Connector.ConnectorServiceCommon;
using Sage.Connector.DispatchService.Interfaces;
using Sage.Connector.Logging;
using Sage.Connector.Utilities;
using Sage.CRE.HostingFramework.Interfaces;

namespace Sage.Connector.DispatchService
{
    /// <summary>
    /// Dispatch a message to appropriate binder for invocation and response
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single, ConfigurationName = "DispatchService")]
    [SingletonServiceControl(StartMethod = "Startup", StopMethod = "Shutdown")]
    public sealed class DispatchService : IDispatchService, IAutoUpdateService
    {
        #region Private Members

        private static readonly String _myTypeName = typeof(DispatchService).FullName;
        private readonly DispatchController _controller;
        private readonly Object _syncObject = new Object();

        #endregion


        /// <summary>
        /// If a SYSTEM environment variable SAGE_CONNECTOR_DISPATCH_SERVICE_BREAK is set to 1,, then 
        /// fire a debugger breakpoint.  This allows us to decide to break into the HostingFramework
        /// startup code without changing any code.
        /// 
        /// Can use the RUNME-ClearMessagingServiceBreak and RUNME-SetMessagingServiceBreak shortcuts
        /// to facilitate changing this variable.
        /// </summary>
        [Conditional("DEBUG")]
        private static void ConditionalDebugBreak()
        {
            String doBreak = Environment.GetEnvironmentVariable("SAGE_CONNECTOR_DISPATCH_SERVICE_BREAK", EnvironmentVariableTarget.Machine);
            if (!String.IsNullOrEmpty(doBreak) && doBreak == "1")
            {
                Debugger.Break();
            }
        }

       /// <summary>
        /// Initializes a new instance of the MessagingService class
        /// </summary>
        public DispatchService()
       {
            ConditionalDebugBreak();

            using (new StackTraceContext(this))
            {
                InitRetryManagerDatabaseRepairMethod();

                lock (_syncObject)
                {
                    _controller = new DispatchController();
                }
            }
        }

        /// <summary>
        /// Called by the HostingFx whenever the service should start work (e.g., service started or continued after paused)
        /// </summary>
        public void Startup()
        {
            using (new StackTraceContext(this))
            {
                lock (_syncObject)
                {
                    _controller.Startup();
                }
            }
        }

        /// <summary>
        /// Called by the HostingFx whenever the service should cease work (e.g., service stopped or paused)
        /// </summary>
        public void Shutdown()
        {
            using (new StackTraceContext(this))
            {
                lock (_syncObject)
                {
                    _controller.Shutdown();
                }
            }
        }

        /// <summary>
        /// Cancels any in-progress work for the tenant
        /// </summary>
        /// <param name="tenantId"></param>
        public void CancelInProgressWork(string tenantId)
        {
            using (new StackTraceContext(this))
            {
                lock (_syncObject)
                {
                    _controller.CancelInProgressWork(tenantId);
                }
            }
        }

        /// <summary>
        /// Request cancellation of a specific request
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="activityTrackingContextId"></param>
        public void CancelWork(string tenantId, string activityTrackingContextId)
        {
            using (new StackTraceContext(this))
            {
                lock (_syncObject)
                {
                    _controller.CancelWork(tenantId, activityTrackingContextId);
                }
            }
        }

        /// <summary>
        /// Request list of in progress requests
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public IEnumerable<RequestWrapper> InProgressWork(string tenantId)
        {
            using (new StackTraceContext(this))
            {
                lock (_syncObject)
                {
                    return _controller.InProgressWork(tenantId);
                }
            }
        }


        #region Private Methods

        /// <summary>
        /// Init the retry policy manager's database repairer method externally
        /// </summary>
        private void InitRetryManagerDatabaseRepairMethod()
        {
            RetryPolicyManager.SetDatabaseRepairMethod(DatabaseRepairUtils.RepairDatabaseCoordinator);
        }

        #endregion

        /// <summary>
        /// Downloads the back office plugin.
        /// </summary>
        /// <param name="backOfficeId">The back office identifier.</param>
        /// <param name="autoUpdateUri">The automatic update URI.</param>
        /// <param name="autoUpdateProductId">The automatic update product identifier.</param>
        /// <param name="autoUpdateProductVersion">The automatic update product version.</param>
        /// <param name="autoUpdateComponentBaseName">Name of the automatic update component base.</param>
        /// <returns></returns>
        public bool DownloadBackOfficePlugin(string backOfficeId, Uri autoUpdateUri, string autoUpdateProductId, string autoUpdateProductVersion,
            string autoUpdateComponentBaseName)
        {
            return _controller.DownloadBackOfficePlugin(backOfficeId, autoUpdateUri, autoUpdateProductId, autoUpdateProductVersion,
                autoUpdateComponentBaseName);
        }

        /// <summary>
        /// Checks for updates.
        /// </summary>
        public bool CheckForUpdates()
        {
            _controller.CheckForAutoUpdates();
            return true;
        }
    }
}
