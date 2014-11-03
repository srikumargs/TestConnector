using System;
using System.ServiceModel;
using Sage.Connector.StateService.Proxy.Internal;
using Sage.CRE.HostingFramework.Proxy.Advanced;

namespace Sage.Connector.StateService.Proxy
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class DatabaseRepairerProxyFactoryParams :
        ICatalogedServiceWithAddressProxyFactoryParams<DatabaseRepairerServiceProxy>
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
        { get { return "DatabaseRepairerService"; } }

        /// <summary>
        /// Creates a new DatabaseRepairerServiceProxy with the given endpoint
        /// </summary>
        /// <param name="endpointAddress"></param>
        /// <param name="instanceContext"></param>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Factory method; rule does not apply")]
        public DatabaseRepairerServiceProxy Create(EndpointAddress endpointAddress, InstanceContext instanceContext)
        {
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.Transport, true);
            binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.SendTimeout = new TimeSpan(0, 2, 0);
            binding.ReliableSession.InactivityTimeout = new TimeSpan(0, 10, 0);
            binding.MaxReceivedMessageSize = 5000000;
            binding.MaxBufferPoolSize = 5000000;
            binding.MaxBufferSize = 5000000;
            binding.ReaderQuotas.MaxStringContentLength = 5000000;
            binding.ReaderQuotas.MaxArrayLength = 5000000;
            binding.ReaderQuotas.MaxBytesPerRead = 5000000;
            binding.ReaderQuotas.MaxDepth = 5000000;
            binding.ReaderQuotas.MaxNameTableCharCount = 5000000;
            return new DatabaseRepairerServiceProxy(delegate() { return new RawDatabaseRepairerServiceProxy(binding, endpointAddress); });
        }
    }

    /// <summary>
    /// A client convenience factory intended to facilitate creation of 
    /// An appropriately-configured DatabaseRepairerServiceProxy.
    /// </summary>
    public sealed class DatabaseRepairerServiceProxyFactory : CatalogedServiceProxyFactory<DatabaseRepairerServiceProxy, DatabaseRepairerProxyFactoryParams>
    { }
}
