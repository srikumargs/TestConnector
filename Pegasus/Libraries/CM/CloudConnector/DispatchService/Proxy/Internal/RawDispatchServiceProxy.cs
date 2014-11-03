using System;
using System.Collections.Generic;
using System.ServiceModel;
using Sage.Connector.ConnectorServiceCommon;
using Sage.Connector.DispatchService.Interfaces;

namespace Sage.Connector.DispatchService.Proxy.Internal
{
    internal sealed class RawDispatchServiceProxy : ClientBase<IDispatchService>, IDispatchService
    {
        /// <summary>
        /// 
        /// </summary>
        public RawDispatchServiceProxy()
            : base()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endpointConfigurationName"></param>
        public RawDispatchServiceProxy(string endpointConfigurationName)
            : base(endpointConfigurationName)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endpointConfigurationName"></param>
        /// <param name="remoteAddress"></param>
        public RawDispatchServiceProxy(string endpointConfigurationName, string remoteAddress)
            : base(endpointConfigurationName, remoteAddress)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endpointConfigurationName"></param>
        /// <param name="remoteAddress"></param>
        public RawDispatchServiceProxy(string endpointConfigurationName, EndpointAddress remoteAddress)
            : base(endpointConfigurationName, remoteAddress)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="remoteAddress"></param>
        public RawDispatchServiceProxy(System.ServiceModel.Channels.Binding binding, EndpointAddress remoteAddress)
            : base(binding, remoteAddress)
        { }

        #region IDispatchService Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        public void CancelInProgressWork(String tenantId)
        {
            base.Channel.CancelInProgressWork(tenantId);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="activityTrackingContextId"></param>
        public void CancelWork(string tenantId, string activityTrackingContextId)
        {
            base.Channel.CancelWork(tenantId, activityTrackingContextId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public IEnumerable<RequestWrapper> InProgressWork(string tenantId)
        {
            return base.Channel.InProgressWork(tenantId);
        }

        #endregion
    }
}
