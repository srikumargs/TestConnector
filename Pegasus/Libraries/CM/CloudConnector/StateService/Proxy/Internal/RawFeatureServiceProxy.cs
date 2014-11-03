using Sage.Connector.StateService.Interfaces;
using Sage.Connector.StateService.Interfaces.DataContracts;
using System;
using System.ServiceModel;

namespace Sage.Connector.StateService.Proxy.Internal
{
    internal sealed class RawFeatureServiceProxy : ClientBase<IFeatureService>, IFeatureService
    {
        /// <summary>
        /// 
        /// </summary>
        public RawFeatureServiceProxy()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endpointConfigurationName"></param>
        public RawFeatureServiceProxy(string endpointConfigurationName)
            : base(endpointConfigurationName)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endpointConfigurationName"></param>
        /// <param name="remoteAddress"></param>
        public RawFeatureServiceProxy(string endpointConfigurationName, string remoteAddress)
            : base(endpointConfigurationName, remoteAddress)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="endpointConfigurationName"></param>
        /// <param name="remoteAddress"></param>
        public RawFeatureServiceProxy(string endpointConfigurationName, EndpointAddress remoteAddress)
            : base(endpointConfigurationName, remoteAddress)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binding"></param>
        /// <param name="remoteAddress"></param>
        public RawFeatureServiceProxy(System.ServiceModel.Channels.Binding binding, EndpointAddress remoteAddress)
            : base(binding, remoteAddress)
        { }

        #region IFeature Members

        /// <summary>
        /// Get the Feature Response
        /// </summary>
        /// <param name="backOfficeId"></param>
        /// <param name="backOfficeCredentials"></param>
        /// <param name="featureId"></param>
        /// <param name="tenantId"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        public FeatureResponse GetFeatureResponse(String backOfficeId, String backOfficeCredentials, String featureId, String tenantId, String payload)
        {
            return Channel.GetFeatureResponse(backOfficeId, backOfficeCredentials, featureId, tenantId, payload);
        }
        #endregion

 
    }
}
