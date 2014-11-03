using System;
using System.Threading.Tasks;
using Sage.Connector.Common;
using Sage.Connector.ConnectorServiceCommon;
using Sage.Connector.Logging;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.Connector.StateService.Proxy;
using Sage.Connector.Utilities;
using CloudContracts = Sage.Connector.Cloud.Integration.Interfaces;
using Convert = System.Convert;

namespace Sage.Connector.MessagingService.Internal
{
    internal sealed class CloudConnectivityStateMonitorHelper
    {
        private static void SetUpdateInfo(ConnectorUpdateStatus status, CloudContracts.DataContracts.UpgradeInfo cloudUpgradeInfo)
        {
            try
            {
                UpdateInfo updateInfo = new UpdateInfo(
                    cloudUpgradeInfo.ProductVersion,
                    cloudUpgradeInfo.PublicationDate,
                    cloudUpgradeInfo.UpgradeDescription,
                    cloudUpgradeInfo.UpgradeLinkUri);

                using (var proxy = StateServiceProxyFactory.CreateFromCatalog(
                                "localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                {
                    proxy.SetUpdateInfo(status, updateInfo);
                }
            }
            catch (Exception ex)
            {
                using (LogManager lm = new LogManager())
                {
                    lm.WriteError(null, "Unable to set update version information: " + ex.ExceptionAsString());
                }
            }
        }

        public static void UpdateMaxUptimeUntilRestart(TimeSpan? maxUptime)
        {
            TaskFactoryHelper.StartNewLimited(() =>
            {
                try
                {
                    using (var proxy = StateServiceProxyFactory.CreateFromCatalog(
                                    "localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                    {
                        proxy.SetMaxUptimeBeforeRestart(maxUptime);
                    }
                }
                catch (Exception ex)
                {
                    using (LogManager lm = new LogManager())
                    {
                        lm.WriteError(null, "Unable to set max uptime until restart: " + ex.ExceptionAsString());
                    }
                }
            });
        }

        public static void UpdateLastCommunicationAttempt(string tenantId, DateTime timeToUpdate)
        {
            TaskFactoryHelper.StartNewLimited(() =>
                {
                    try
                    {
                        using (var proxy = StateServiceProxyFactory.CreateFromCatalog(
                                        "localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                        {
                            proxy.UpdateLastAttemptedCommunicationWithCloud(tenantId, timeToUpdate);
                        }
                    }
                    catch (Exception ex)
                    {
                        using (LogManager lm = new LogManager())
                        {
                            lm.WriteError(null, "Unable to set last communication attempt: " + ex.ExceptionAsString());
                        }
                    }
                });
        }

        public static void UpdateLastSuccessfulCommunication(string tenantId, DateTime timeToUpdate)
        {
            TaskFactoryHelper.StartNewLimited(() =>
                {
                    try
                    {
                        using (var proxy = StateServiceProxyFactory.CreateFromCatalog(
                                        "localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                        {
                            proxy.UpdateLastSuccessfulCommunicationWithCloud(tenantId, timeToUpdate);
                            proxy.UpdateTenantConnectivityStatus(tenantId, TenantConnectivityStatus.Normal);
                        }
                    }
                    catch (Exception ex)
                    {
                        using (LogManager lm = new LogManager())
                        {
                            lm.WriteError(null, "Unable to set last successful communication: " + ex.ExceptionAsString());
                        }
                    }
                });
        }

        public static void UpdateNextScheduledCommunication(string tenantId, int millisFromNow)
        {
            // Zero mean we sleep until signaled
            if (0 == millisFromNow)
            {
                return;
            }

            DateTime timeToUpdate = DateTime.UtcNow.AddMilliseconds(millisFromNow);

            TaskFactoryHelper.StartNewLimited(() =>
                {
                    try
                    {
                        using (var proxy = StateServiceProxyFactory.CreateFromCatalog(
                                        "localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                        {
                            proxy.UpdateNextScheduledCommunicationWithCloud(tenantId, timeToUpdate);
                        }
                    }
                    catch (Exception ex)
                    {
                        using (LogManager lm = new LogManager())
                        {
                            lm.WriteError(null, "Unable to set next communication attempt: " + ex.ExceptionAsString());
                        }
                    }
                });
        }

        public static void UpdateTenantConnectivityStatusToIfUriTestSucceeds(string tenantId, Uri uri, TenantConnectivityStatus status)
        {
            TaskFactoryHelper.StartNewLimited(() =>
            {
                try
                {
                    var statusToUpdateTo = TenantConnectivityStatusHelper.GetTenantConnectivityStatus(tenantId, uri);
                    if(statusToUpdateTo == TenantConnectivityStatus.Normal)
                    {
                        statusToUpdateTo = status;
                    }
                    UpdateTenantConnectivityStatus(tenantId, statusToUpdateTo);
                }
                catch (Exception ex)
                {
                    using (LogManager lm = new LogManager())
                    {
                        lm.WriteError(null, "Unable to update tenant connectivity: " + ex.ExceptionAsString());
                    }
                }
            });

        }

        public static void UpdateTenantConnectivityStatus(string tenantId, TenantConnectivityStatus status)
        {
            TaskFactoryHelper.StartNewLimited(() =>
                {
                    try
                    {
                        using (var proxy = StateServiceProxyFactory.CreateFromCatalog(
                                        "localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                        {
                            proxy.UpdateTenantConnectivityStatus(tenantId, status);
                        }
                    }
                    catch (Exception ex)
                    {
                        using (LogManager lm = new LogManager())
                        {                            
                            lm.WriteError(null, "Unable to update tenant connectivity: " + ex.ExceptionAsString());
                        }
                    }
                });
        }

        public static void IncrementRequestCount(string tenantId, uint count)
        {
            TaskFactoryHelper.StartNewLimited(() =>
                {
                    try
                    {
                        using (var proxy = StateServiceProxyFactory.CreateFromCatalog(
                                        "localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                        {
                            proxy.IncrementRequestsReceivedCount(tenantId, count);
                        }
                    }
                    catch (Exception ex)
                    {
                        using (LogManager lm = new LogManager())
                        {
                            lm.WriteError(null, "Unable to update request count:" + ex.ExceptionAsString());
                        }
                    }
                });
        }

        public static void UpdateResponseCount(string tenantId, bool responseIsErrorResponse)
        {
            TaskFactoryHelper.StartNewLimited(() =>
            {
                try
                {
                    using (var proxy = StateServiceProxyFactory.CreateFromCatalog(
                                    "localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                    {
                        if (responseIsErrorResponse)
                        {
                            proxy.IncrementErrorResponsesSentCount(tenantId, 1);
                        }
                        else
                        {
                            proxy.IncrementNonErrorResponsesSentCount(tenantId, 1);
                        }
                        proxy.AdjustRequestsInProgressCount(tenantId, -1);
                    }
                }
                catch (Exception ex)
                {
                    using (LogManager lm = new LogManager())
                    {
                        lm.WriteError(null, "Unable to update response counts: " + ex.ExceptionAsString());
                    }
                }
            });
        }

        public static void IncrementRequestInProgressCount(string tenantId, uint count)
        {
            TaskFactoryHelper.StartNewLimited(() =>
                {
                    try
                    {
                        using (var proxy = StateServiceProxyFactory.CreateFromCatalog(
                                        "localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                        {
                            proxy.AdjustRequestsInProgressCount(tenantId, Convert.ToInt32(count));
                        }
                    }
                    catch (Exception ex)
                    {
                        using (LogManager lm = new LogManager())
                        {
                            lm.WriteError(null, "Unable to update requests in progress:" + ex.ExceptionAsString());
                        }
                    }
                });
        }

        public static void DecrementRequestInProgressCount(string tenantId, uint count)
        {
            TaskFactoryHelper.StartNewLimited(() =>
                {
                    try
                    {
                        using (var proxy = StateServiceProxyFactory.CreateFromCatalog(
                                        "localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                        {
                            proxy.AdjustRequestsInProgressCount(tenantId, -Convert.ToInt32(count));
                        }
                    }
                    catch (Exception ex)
                    {
                        using (LogManager lm = new LogManager())
                        {
                            lm.WriteError(null, "Unable to update requests in progress:" + ex.ExceptionAsString());
                        }
                    }
                });
        }

        public static void NotifyIncompatibleClientFault(CloudContracts.Faults.IncompatibleClientFault icf)
        {
            TaskFactoryHelper.StartNewLimited(() =>
                {
                    if (null != icf)
                    {
                        SetUpdateInfo(ConnectorUpdateStatus.UpdateRequired, icf.CurrentConnectorProductUpgradeInfo);
                    }
                });
        }
    }
}
