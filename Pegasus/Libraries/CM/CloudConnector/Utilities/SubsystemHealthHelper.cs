using System;
using Sage.Connector.Common;
using Sage.Connector.ConnectorServiceCommon;
using Sage.Connector.Logging;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.Connector.StateService.Proxy;
using System.Threading.Tasks;

namespace Sage.Connector.Utilities
{
    /// <summary>
    /// Simple helper for raising and clearing subsystem health
    /// </summary>
    public static class SubsystemHealthHelper
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subsystem"></param>
        /// <param name="rawMessage"></param>
        /// <param name="userFacingMessage"></param>
        public static void RaiseSubsystemHealthIssue(Subsystem subsystem, String rawMessage, String userFacingMessage)
        {
            RaiseSubsystemHealthIssue(subsystem, rawMessage, userFacingMessage, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subsystem"></param>
        /// <param name="rawMessage"></param>
        /// <param name="userFacingMessage"></param>
        /// <param name="helpTopicId"></param>
        public static void RaiseSubsystemHealthIssue(Subsystem subsystem, String rawMessage, String userFacingMessage, Int32? helpTopicId)
        {
            TaskFactoryHelper.StartNewLimited(() =>
                {
                    try
                    {
                        using (var proxy = StateServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                        {
                            var msg = new SubsystemHealthMessage(subsystem, rawMessage, userFacingMessage, DateTime.UtcNow, helpTopicId);
                            proxy.RaiseSubsystemHealthIssue(msg);
                        }
                    }
                    catch (Exception ex)
                    {
                        using (var lm = new LogManager())
                        {
                            lm.WriteError(null, "Error raising subsystem health issue: " + ex.ExceptionAsString());
                        }
                    }
                });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subsystem"></param>
        public static void ClearSubsystemHealthIssues(Subsystem subsystem)
        {
            TaskFactoryHelper.StartNewLimited(() =>
                {
                    try
                    {
                        using (var proxy = StateServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                        {
                            proxy.ClearSubsystemHealthIssues(subsystem);
                        }
                    }
                    catch (Exception ex)
                    {
                        using (var lm = new LogManager())
                        {
                            lm.WriteError(null, "Error clearing subsystem health issue: " + ex.ExceptionAsString());
                        }
                    }
                });
        }
    }
}
