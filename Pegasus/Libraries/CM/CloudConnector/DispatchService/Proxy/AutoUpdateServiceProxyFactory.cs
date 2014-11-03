using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Sage.Connector.DispatchService.Proxy.Internal;
using Sage.CRE.HostingFramework.Proxy.Advanced;

namespace Sage.Connector.DispatchService.Proxy
{
         /// <summary>
        /// 
        /// </summary>
    public sealed class AutoUpdateServiceProxyFactoryParams : ICatalogedServiceWithAddressProxyFactoryParams<AutoUpdateServiceProxy>
        {
            /// <summary>
            /// 
            /// </summary>
            public String ServiceName
            {
                get { return "DispatchService"; }
            }

            /// <summary>
            /// 
            /// </summary>
            public String Address
            {
                get { return "AutoUpdateService"; }
            }

            /// <summary>
            /// Creates a new DispatchServiceProxy with the given endpoint
            /// </summary>
            /// <param name="endpointAddress"></param>
            /// <param name="instanceContext"></param>
            /// <returns></returns>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope",
                Justification = "Factory method; rule does not apply")]
            public AutoUpdateServiceProxy Create(EndpointAddress endpointAddress, InstanceContext instanceContext)
            {
                NetTcpBinding binding = new NetTcpBinding(SecurityMode.Transport, true);
                binding.Security.Transport.ProtectionLevel = System.Net.Security.ProtectionLevel.EncryptAndSign;
                binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
                binding.SendTimeout = new TimeSpan(0, 2, 0);
                binding.ReliableSession.InactivityTimeout = new TimeSpan(0, 10, 0);
                return new AutoUpdateServiceProxy(delegate() { return new RawAutoUpdateServiceProxy(binding, endpointAddress); });
            }
        }
    

    /// <summary>
    /// A client convenience factory intended to facilitate creation of an appropriately-configured DispatchServiceProxy.
    /// </summary>
    public sealed class AutoUpdateServiceProxyFactory : CatalogedServiceProxyFactory<AutoUpdateServiceProxy, Proxy.AutoUpdateServiceProxyFactoryParams>
    { }
}
    

