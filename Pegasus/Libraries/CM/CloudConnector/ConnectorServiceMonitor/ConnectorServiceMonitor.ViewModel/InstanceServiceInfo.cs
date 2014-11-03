using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HostingFxInterfaces = Sage.CRE.HostingFramework.Interfaces;

namespace ConnectorServiceMonitor.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class InstanceServiceInfo
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="serviceInfo"></param>
        public InstanceServiceInfo(String instance, HostingFxInterfaces.ServiceInfo serviceInfo)
        {
            Instance = instance;
            ServiceInfo = serviceInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        public String Instance { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public HostingFxInterfaces.ServiceInfo ServiceInfo { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public String FullName
        { get { return String.Format("{0}\\{1}", Instance, ServiceInfo.ConfigurationName); } }
    }
}
