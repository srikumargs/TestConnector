using System;
using System.ServiceModel;
using Sage.Connector.DispatchService.Proxy.Internal;
using Sage.CRE.HostingFramework.Proxy.Advanced;

namespace Sage.Connector.DispatchService.Proxy
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class DispatchServiceProxyFactoryParams : ICatalogedServiceWithAddressProxyFactoryParams<DispatchServiceProxy>
    {
        /// <summary>
        /// 
        /// </summary>
        public String ServiceName
        { get { return "DispatchService"; } }


        /// <summary>
        /// Gets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public String Address
        { get { return "DispatchService"; } }

        /// <summary>
        /// Creates a new DispatchServiceProxy with the given endpoint
        /// </summary>
        /// <param name="endpointAddress"></param>
        /// <param name="instanceContext"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Factory method; rule does not apply")]
        public DispatchServiceProxy Create(EndpointAddress endpointAddress, InstanceContext instanceContext)
        {
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.Transport, true);
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.SendTimeout = new TimeSpan(0, 2, 0);
            binding.ReliableSession.InactivityTimeout = new TimeSpan(0, 10, 0);
            return new DispatchServiceProxy(delegate() { return new RawDispatchServiceProxy(binding, endpointAddress); });
        }
    }

    /// <summary>
    /// A client convenience factory intended to facilitate creation of an appropriately-configured DispatchServiceProxy.
    /// </summary>
    public sealed class DispatchServiceProxyFactory : CatalogedServiceProxyFactory<DispatchServiceProxy, DispatchServiceProxyFactoryParams>
    { }
}
