using System;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using Sage.Connector.Common;
using Sage.Connector.Common.DataContracts;
using Sage.Connector.MonitorService.Interfaces;
using Sage.Connector.MonitorService.Interfaces.DataContracts;
using Sage.Connector.MonitorService.Internal;
using Sage.Connector.StateService.Proxy;
using Sage.CRE.HostingFramework.Proxy;
using HostingFxInterfaces = Sage.CRE.HostingFramework.Interfaces;

namespace Sage.Connector.MonitorService
{
    /// <summary>
    /// A configuration service for storing and retrieving premise-cloud connectivity attributes
    /// </summary>
    /// <remarks>
    /// Note that this service was initaly percall, multipule. We were seeing socket leaks.
    /// Tested with Single, Mutlipule and it seemed to not have issues.
    /// However given the lower number of users, single stingle seems a better choice, also prevents threading issues 
    /// with recovery in case of failure etc. 
    /// </remarks>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single,
        ConfigurationName = "ConnectorMonitorService")]
    public sealed class MonitorService : IMonitorService
    {
        #region IMonitorService Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ConnectorServiceState GetConnectorServiceState()
        {
            ConnectorServiceState result = GetConnectorServiceStateFromSystem();
            return result;
        }

        /// <summary>
        /// Retrieves the recently created request entries as well as all currently in-progress ones
        /// </summary>
        /// <param name="recentEntriesThreshold"></param>
        /// <returns></returns>
        public RequestState[] GetRecentAndInProgressRequestsState(TimeSpan recentEntriesThreshold)
        {
            RequestState[] result = GetRecentAndInProgressRequestsStateFromSystem(recentEntriesThreshold);
            return result;
        }

        private RequestState[] GetRecentAndInProgressRequestsStateFromSystem(TimeSpan recentEntriesThreshold)
        {
            RequestState[] result = null;

            try
            {
                var proxy = GetStateServiceProxy();
                result = proxy.GetRecentAndInProgressRequestsState(recentEntriesThreshold);

            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(this, ex.ExceptionAsString());
                }
                ReleaseStateServiceProxy();
            }

            return result;
        }

        private ConnectorServiceState GetConnectorServiceStateFromSystem()
        {
            ConnectorServiceState result = null;

            String fileVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;

            try
            {
                using (var logger = new SimpleTraceLogger())
                {
                    if (!ConnectorServiceUtils.IsServiceRegistered(logger))
                    {
                        result = new ConnectorServiceState(ConnectorServiceConnectivityStatus.ServiceNotRegistered, null, fileVersion, null);
                    }
                    else if (!ConnectorServiceUtils.IsServiceRunning(logger))
                    {
                        result = new ConnectorServiceState(ConnectorServiceConnectivityStatus.ServiceNotRunning, null, fileVersion, null);
                    }
                    else if (!ConnectorServiceUtils.IsServiceReady())
                    {
                        result = new ConnectorServiceState(ConnectorServiceConnectivityStatus.ServiceNotReady, null, fileVersion, null);
                    }
                    else
                    {
                        HostingFxInterfaces.ServiceInfo[] serviceInfos = null;
                        using (var catalogProxy = CatalogServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                        {
                            serviceInfos = catalogProxy.GetServiceInfo().ToArray();
                        }

                        // Connector service is registered, running, and ready ... we should be able to talk to it
                        var proxy = GetStateServiceProxy();
                        var state = proxy.GetConnectorState();
                        result = new ConnectorServiceState(ConnectorServiceConnectivityStatus.Connected, state, fileVersion, serviceInfos);
                    }
                }
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(this, ex.ExceptionAsString());
                }
                result = new ConnectorServiceState(ConnectorServiceConnectivityStatus.ConnectivityError, null, fileVersion, null);
                ReleaseStateServiceProxy();
            }

            return result;
        }
        private StateServiceProxy GetStateServiceProxy()
        {
            if (_stateServiceProxy == null)
                _stateServiceProxy = StateServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber);

            return _stateServiceProxy;
        }

        private void ReleaseStateServiceProxy()
        {
            if (_stateServiceProxy != null)
            {
                try
                {
                    _stateServiceProxy.Dispose();
                    _stateServiceProxy = null;
                }
                catch
                {
                    // throw away any issues from disposing the proxy
                }
            }
        }


        #endregion

        private static StateServiceProxy _stateServiceProxy;
        private static readonly String _myTypeName = typeof(MonitorService).FullName;
        private static readonly Object _syncObject = new Object();
        //private static readonly StateCache _stateCache = new StateCache();
        
    }
}
