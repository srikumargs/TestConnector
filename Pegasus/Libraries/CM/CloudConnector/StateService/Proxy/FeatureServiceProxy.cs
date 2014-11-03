using System;
using System.Collections.Generic;
using Sage.Connector.StateService.Interfaces;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.ServiceModel;

namespace Sage.Connector.StateService.Proxy
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class FeatureServiceProxy : RetryClientBase<IFeatureService>, IFeatureService
    {
        /// <summary>
        /// 
        /// </summary>
        public FeatureServiceProxy(CreationFunction rawProxyCreationFunction)
            : base(rawProxyCreationFunction)
        { }

        #region IFeatureService Members

        /// <summary>
        /// Get the FeatureReesponse
        /// </summary>
        /// <param name="backOfficeId"></param>
        /// <param name="backOfficeCredentials"></param>
        /// <param name="featureId"></param>
        /// <param name="tenantId"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        public FeatureResponse GetFeatureResponse(string backOfficeId, string backOfficeCredentials, string featureId, String tenantId, string payload)
        {
            return (FeatureResponse)RetvalCallRawProxy((RetvalMethodInvoker)(() =>
                RawProxy.GetFeatureResponse(backOfficeId, backOfficeCredentials, featureId, tenantId, payload)));
        }

        #endregion
    }
}
