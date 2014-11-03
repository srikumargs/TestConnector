using System;
using System.ServiceModel;
using System.Threading;

namespace Sage.Connector.ConnectorServiceCommon
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBindable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestKind"></param>
        /// <returns></returns>
        [OperationContract]
        bool SupportRequestKind(String requestKind);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestWrapper"></param>
        /// <param name="cancellationTokenSource"></param>
        /// <param name="maxBlobSize"></param>
        /// <returns></returns>
        [OperationContract]
        ResponseWrapper InvokeWork(RequestWrapper requestWrapper, CancellationTokenSource cancellationTokenSource, long maxBlobSize);
    }
}
