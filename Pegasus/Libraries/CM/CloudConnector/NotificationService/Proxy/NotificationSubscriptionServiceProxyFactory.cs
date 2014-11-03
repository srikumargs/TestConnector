using System;
using System.ServiceModel;
using Sage.CRE.HostingFramework.Proxy.Advanced;

namespace Sage.Connector.NotificationService.Proxy
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class NotificationSubscriptionServiceProxyFactoryParams : ICatalogedServiceProxyFactoryParams<NotificationSubscriptionServiceProxy>
    {
        /// <summary>
        /// The name of the service as it appears in the catalog
        /// </summary>
        public String ServiceName
        { get { return "NotificationSubscriptionService"; } }

        /// <summary>
        /// Create the service proxy at the specified endpoint address
        /// </summary>
        /// <param name="endpointAddress">The endpoint for the service</param>
        /// <param name="instanceContext">An optional parameter used to supply an instance context for callback interfaces (can be null)</param>
        /// <returns></returns>
        public NotificationSubscriptionServiceProxy Create(EndpointAddress endpointAddress, InstanceContext instanceContext)
        {
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.Transport, true);
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            return new NotificationSubscriptionServiceProxy(instanceContext, binding, endpointAddress);
        }
    }

    /// <summary>
    /// A client convenience factory intended to facilitate creation of an appropriately-configured ChatSubscriptionServiceProxy.
    /// </summary>
    public sealed class NotificationSubscriptionServiceProxyFactory : CatalogedServiceProxyFactory<NotificationSubscriptionServiceProxy, NotificationSubscriptionServiceProxyFactoryParams>
    { }
}
