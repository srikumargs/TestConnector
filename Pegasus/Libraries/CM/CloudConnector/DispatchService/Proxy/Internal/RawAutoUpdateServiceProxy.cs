using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Sage.Connector.DispatchService.Interfaces;

namespace Sage.Connector.DispatchService.Proxy.Internal
{


    internal sealed class RawAutoUpdateServiceProxy : ClientBase<IAutoUpdateService>, IAutoUpdateService
    {
        /// <summary>
        /// 
        /// </summary>
        public RawAutoUpdateServiceProxy()
            : base()
        { }

                /// <summary>
        /// 
        /// </summary>
        /// <param name="endpointConfigurationName"></param>
        /// <param name="remoteAddress"></param>
        public RawAutoUpdateServiceProxy(string endpointConfigurationName, string remoteAddress)
            : base(endpointConfigurationName, remoteAddress)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endpointConfigurationName"></param>
        /// <param name="remoteAddress"></param>
        public RawAutoUpdateServiceProxy(string endpointConfigurationName, EndpointAddress remoteAddress)
            : base(endpointConfigurationName, remoteAddress)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="remoteAddress"></param>
        public RawAutoUpdateServiceProxy(System.ServiceModel.Channels.Binding binding, EndpointAddress remoteAddress)
            : base(binding, remoteAddress)
        { }
     
        public bool DownloadBackOfficePlugin(string backOfficeId, Uri autoUpdateUri, string autoUpdateProductId, string autoUpdateProductVersion,
            string autoUpdateComponentBaseName)
        {
            return base.Channel.DownloadBackOfficePlugin(backOfficeId, autoUpdateUri, autoUpdateProductId, autoUpdateProductVersion, autoUpdateComponentBaseName);
        }

        public bool CheckForUpdates()
        {
            return base.Channel.CheckForUpdates();
        }
    }
}
