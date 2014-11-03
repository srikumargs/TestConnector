using System;
using System.Collections.Generic;
using Sage.Connector.SageCloudService;
using System.Windows.Forms;
using Sage.Connector.Cloud.Integration.Interfaces;
using Sage.Connector.Cloud.Integration.Interfaces.DataContracts;

namespace Sage.Connector.MockCloudHostApp
{
    /// <summary>
    /// 
    /// </summary>
    public class HostAppAutomationWorker
    {
        private List<ConfigurationRequest> _continuedRequests;
        private List<ConfigurationRequest> _initialRequests;
        private readonly IEnumerator<ConfigurationRequest> _workItem;

        /// <summary>
        /// Tenant Id
        /// </summary>
        public string TenantId
        { get; set; }

        /// <summary>
        /// Premise Key
        /// </summary>
        public string PremiseKey
        { get; set; }

        /// <summary>
        /// Time, in minutes, the tenant will run ContinuedRequests
        /// </summary>
        public double TestTime
        { get; set; }

        /// <summary>
        /// Delay, in seconds, between ContinuedRequests
        /// </summary>
        public double RequestDelaySeconds
        { get; set; }

        /// <summary>
        /// Requests that will be queued up immediately when starting up tenant
        /// </summary>
        public ConfigurationRequest[] InitialRequests
        {
            get { return _initialRequests.ToArray(); }
            set { _initialRequests = new List<ConfigurationRequest>(value); }
        }

        /// <summary>
        /// Requests that will be sent in round-robin fashion
        /// </summary>
        public ConfigurationRequest[] ContinuedRequests
        {
            get { return _continuedRequests.ToArray(); }
            set { _continuedRequests = new List<ConfigurationRequest>(value); }
        }

        /// <summary>
        ///Time to stop run for this tenant
        /// </summary>
        /// 
        public DateTime StopTime
        { get; set; }

        /// <summary>
        /// Number of Automated continued requests
        /// </summary>
        public int ContinuedRequestCount
        {   get{ return _continuedRequests.Count;}}

        /// <summary>
        /// Number of Automated initial requests
        /// </summary>
        public int InitialRequestCount
        { get { return _initialRequests.Count; } }

        /// <summary>
        /// Whether there are any initial requests to run for tenant.
        /// </summary>
        public bool InitialRequestsToRun
        {get {return InitialRequestCount > 0 ? true : false; } }
        
        /// <summary>
        /// Whether automation will be run for this tenant or not
        /// </summary>
        public bool SetupForAutomation
        { get { return ContinuedRequestCount > 0 ? true : false; } }

        /// <summary>
        /// The tenant endpoint address
        /// </summary>
        public string TenantEndpointAddress
        { get; set; }

        /// <summary>
        /// The config params for this tenant
        /// </summary>
        public Sage.Connector.Cloud.Integration.Interfaces.WebAPI.Configuration ConfigParams
        { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Sage.Connector.Cloud.Integration.Interfaces.WebAPI.Configuration ConfigParamsCustomUpdate
        { get; set; }

        /// <summary>
        /// The gateway service info for this tenant
        /// </summary>
        public ServiceInfo GatewayServiceInfo
        { get; set; }

        /// <summary>
        /// Full Constructor
        /// </summary>
        /// <param name="endpointAddress"></param>
        /// <param name="tenantId"></param>
        /// <param name="premiseKey"></param>
        /// <param name="testTime"></param>
        /// <param name="requestDelay"></param>
        /// <param name="initRequests"></param>
        /// <param name="contRequests"></param>
        /// <param name="configParams"></param>
        /// <param name="gatewayServiceInfos"></param>
        /// <param name="configParamsCustom"></param>
        public HostAppAutomationWorker(
            string endpointAddress, string tenantId, string premiseKey, double testTime, double requestDelay,
            ConfigurationRequest[] initRequests, ConfigurationRequest[] contRequests,
            Sage.Connector.Cloud.Integration.Interfaces.WebAPI.Configuration configParams,
            Sage.Connector.Cloud.Integration.Interfaces.WebAPI.Configuration configParamsCustom,
            ServiceInfo gatewayServiceInfos)
        {
            TenantEndpointAddress = endpointAddress;
            TenantId = tenantId;
            PremiseKey = premiseKey;
            TestTime = testTime;
            RequestDelaySeconds = requestDelay;
            InitialRequests = initRequests ?? new ConfigurationRequest[] { };
            _continuedRequests = contRequests == null ? new List<ConfigurationRequest>() : new List<ConfigurationRequest>(contRequests);
            _workItem = _continuedRequests.GetEnumerator();
            ConfigParams = configParams;
            ConfigParamsCustomUpdate = configParamsCustom;
            GatewayServiceInfo = gatewayServiceInfos;
        }

        /// <summary>
        /// Retrieves next work item from Continued Requests
        /// </summary>
        /// <returns></returns>
        public ConfigurationRequest GetNextWorkItem()
        {
            ConfigurationRequest retVal = new ConfigurationRequest();

            try
            {
                if (_workItem.MoveNext())
                {
                    retVal = _workItem.Current;
                }
                else
                {
                    _workItem.Reset();
                    _workItem.MoveNext();
                    retVal = _workItem.Current;
                }
            }
            catch (InvalidOperationException)
            {
                retVal = new ConfigurationRequest();
                retVal.RequestTypeName = InvokeActionEnum.None.ToString();
            }

            return retVal;
        }
    }
}
