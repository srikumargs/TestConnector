using System;
using System.Collections.Generic;
using Sage.Connector.ConnectorServiceCommon;
using Sage.Connector.DispatchService.Interfaces;
using Sage.ServiceModel;

namespace Sage.Connector.DispatchService.Proxy
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class DispatchServiceProxy : RetryClientBase<IDispatchService>, IDispatchService
    {
        /// <summary>
        /// 
        /// </summary>
        public DispatchServiceProxy(RetryClientBase<IDispatchService>.CreationFunction rawProxyCreationFunction)
            : base(rawProxyCreationFunction)
        { }

        #region IDispatchService Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        public void CancelInProgressWork(String tenantId)
        {
            VoidCallRawProxy((VoidMethodInvoker)delegate() 
                { RawProxy.CancelInProgressWork(tenantId); });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="activityTrackingContextId"></param>
        public void CancelWork(string tenantId, string activityTrackingContextId)
        {
            VoidCallRawProxy((VoidMethodInvoker) delegate() 
                { RawProxy.CancelWork(tenantId, activityTrackingContextId); });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public IEnumerable<RequestWrapper> InProgressWork(string tenantId)
        {
            return (IEnumerable<RequestWrapper>)RetvalCallRawProxy(
                ((RetvalMethodInvoker) delegate() { return RawProxy.InProgressWork(tenantId); }));
        }

        #endregion
    }
}
