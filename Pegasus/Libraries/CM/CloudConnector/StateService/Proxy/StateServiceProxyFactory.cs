using System;
using System.ServiceModel;
using Sage.Connector.StateService.Proxy.Internal;
using Sage.CRE.HostingFramework.Proxy.Advanced;

namespace Sage.Connector.StateService.Proxy
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class StateServiceProxyFactoryParams : ICatalogedServiceWithAddressProxyFactoryParams<StateServiceProxy>
    {
        /// <summary>
        /// 
        /// </summary>
        public String ServiceName
        { get { return "StateService"; } }

        /// <summary>
        /// 
        /// </summary>
        public String Address
        { get { return "StateService"; } }

        /// <summary>
        /// Creates a new StateServiceProxy with the given endpoint
        /// </summary>
        /// <param name="endpointAddress"></param>
        /// <param name="instanceContext"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Factory method; rule does not apply")]
        public StateServiceProxy Create(EndpointAddress endpointAddress, InstanceContext instanceContext)
        {
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.Transport, true);
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.SendTimeout = new TimeSpan(0, 2, 0);
            binding.ReliableSession.InactivityTimeout = new TimeSpan(0, 10, 0);
            const int mediumThreshhold = 5 * 1000 * 1000;
            const int largeThreshhold = 200 * 1000 * 1000;
            binding.MaxReceivedMessageSize = largeThreshhold;
            binding.MaxBufferPoolSize = largeThreshhold;
            binding.MaxBufferSize = largeThreshhold;
            binding.ReaderQuotas.MaxStringContentLength = mediumThreshhold;
            binding.ReaderQuotas.MaxArrayLength = mediumThreshhold;
            binding.ReaderQuotas.MaxBytesPerRead = largeThreshhold;
            binding.ReaderQuotas.MaxDepth = mediumThreshhold;
            binding.ReaderQuotas.MaxNameTableCharCount = mediumThreshhold;
            return new StateServiceProxy(delegate() { return new RawStateServiceProxy(binding, endpointAddress); });
        }
    }

    /// <summary>
    /// A client convenience factory intended to facilitate creation of an appropriately-configured StateServiceProxy.
    /// </summary>
    public sealed class StateServiceProxyFactory : CatalogedServiceProxyFactory<StateServiceProxy, StateServiceProxyFactoryParams>
    { }
}
