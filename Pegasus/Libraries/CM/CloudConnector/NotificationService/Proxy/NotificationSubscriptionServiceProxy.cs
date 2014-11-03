using System.ServiceModel;
using Sage.Connector.NotificationService.Interfaces;
using Sage.ServiceModel;
using System;

namespace Sage.Connector.NotificationService.Proxy
{
    /// <summary>
    /// 
    /// </summary>
    [CallbackBehavior(UseSynchronizationContext = false)]
    public sealed class NotificationSubscriptionServiceProxy : SubscriptionServiceProxy<INotificationSubscriptionService>, INotificationSubscriptionService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputInstance"></param>
        public NotificationSubscriptionServiceProxy(InstanceContext inputInstance)
            : base(inputInstance)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputInstance"></param>
        /// <param name="endpointConfigurationName"></param>
        public NotificationSubscriptionServiceProxy(InstanceContext inputInstance, string endpointConfigurationName)
            : base(inputInstance, endpointConfigurationName)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputInstance"></param>
        /// <param name="endpointConfigurationName"></param>
        /// <param name="remoteAddress"></param>
        public NotificationSubscriptionServiceProxy(InstanceContext inputInstance, string endpointConfigurationName, string remoteAddress)
            : base(inputInstance, endpointConfigurationName, remoteAddress)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputInstance"></param>
        /// <param name="endpointConfigurationName"></param>
        /// <param name="remoteAddress"></param>
        public NotificationSubscriptionServiceProxy(InstanceContext inputInstance, string endpointConfigurationName, EndpointAddress remoteAddress)
            : base(inputInstance, endpointConfigurationName, remoteAddress)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputInstance"></param>
        /// <param name="binding"></param>
        /// <param name="remoteAddress"></param>
        public NotificationSubscriptionServiceProxy(InstanceContext inputInstance, System.ServiceModel.Channels.Binding binding, EndpointAddress remoteAddress)
            : base(inputInstance, binding, remoteAddress)
        { }
    }
}

