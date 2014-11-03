using System;
using System.ServiceModel;
using Sage.Connector.MessagingService.Proxy.Internal;
using Sage.CRE.HostingFramework.Proxy.Advanced;

namespace Sage.Connector.MessagingService.Proxy
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class MessagingServiceProxyFactoryParams : ICatalogedServiceProxyFactoryParams<MessagingServiceProxy>
    {
        /// <summary>
        /// 
        /// </summary>
        public String ServiceName
        { get { return "MessagingService"; } }

        /// <summary>
        /// Creates a new MessagingServiceProxy with the given endpoint
        /// </summary>
        /// <param name="endpointAddress"></param>
        /// <param name="instanceContext"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Factory method; rule does not apply")]
        public MessagingServiceProxy Create(EndpointAddress endpointAddress, InstanceContext instanceContext)
        {
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.Transport, true);
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.SendTimeout = new TimeSpan(0, 2, 0);
            binding.ReliableSession.InactivityTimeout = new TimeSpan(0, 10, 0);
            return new MessagingServiceProxy(delegate() { return new RawMessagingServiceProxy(binding, endpointAddress); });
        }
    }

    /// <summary>
    /// A client convenience factory intended to facilitate creation of an appropriately-configured MessagingServiceProxy.
    /// </summary>
    public sealed class MessagingServiceProxyFactory : CatalogedServiceProxyFactory<MessagingServiceProxy, MessagingServiceProxyFactoryParams>
    { }
}
