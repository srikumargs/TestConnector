using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sage.Connector.DispatchService.Interfaces;
using Sage.ServiceModel;

namespace Sage.Connector.DispatchService.Proxy
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class AutoUpdateServiceProxy : RetryClientBase<IAutoUpdateService>, IAutoUpdateService
    {
        /// <summary>
        /// 
        /// </summary>
        public AutoUpdateServiceProxy(RetryClientBase<IAutoUpdateService>.CreationFunction rawProxyCreationFunction)
            : base(rawProxyCreationFunction)
        { }

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
            return (bool) RetvalCallRawProxy((RetvalMethodInvoker)(() =>
                RawProxy.DownloadBackOfficePlugin(backOfficeId, autoUpdateUri, autoUpdateProductId, autoUpdateProductVersion, autoUpdateComponentBaseName)));
        }

        /// <summary>
        /// Checks for updates.
        /// </summary>
        /// <returns></returns>
        public bool CheckForUpdates()
        {
            return (bool)RetvalCallRawProxy((RetvalMethodInvoker)(() =>
                RawProxy.CheckForUpdates()));
        }
    }
}
