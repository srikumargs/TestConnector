using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using Microsoft.Owin.Hosting;
using Sage.Connector.ConnectorServiceCommon;
using Sage.Connector.SageCloudService;
using Sage.Connector.Signalr.Controller;
using Sage.Connector.Cloud.Integration.Interfaces;
using Sage.Connector.LinkedSource;


namespace Sage.Connector.SageCloudService
{
    /// <summary>
    /// 
    /// </summary>
    public static class MockCloudServiceHost
    {
        private static ServiceHost _instance;
        private static ConnectorHost _signalRHost;
        private static IDisposable _webApp;
        private static String _notificationAddress;
        private static Uri _webAppBaseAddress;

        /// <summary>
        /// 
        /// </summary>
        public static void StartService()
        {            
            TraceUtils.WriteLine("MockCloudServiceHost.StartService: invoked");

            Uri serviceAddress = new Uri(String.Format("http://{0}:8002", Environment.MachineName));

            _notificationAddress = String.Format("http://{0}:8003/MockCloudService.svc",
                Environment.MachineName.ToLower());

            _signalRHost = new ConnectorHost(_notificationAddress, MockCloudService.GetConnectorPremiseKey);

            _webAppBaseAddress = new Uri(String.Format("http://{0}:8004/", Environment.MachineName));

            _webApp = WebApp.Start<ApiRouteConfiguration>(url: _webAppBaseAddress.ToString());

            var binding = CloudUtils.CreateCloudBinding(serviceAddress);
            _instance = new ServiceHost(typeof(MockCloudService), new[] { serviceAddress });
            _instance.AddServiceEndpoint(typeof(IGatewayService), binding, "MockCloudService.svc/Gateway");
            _instance.AddServiceEndpoint(typeof(IAdminService), binding, "MockCloudService.svc/Admin");
            _instance.AddServiceEndpoint(typeof(IRequestService), binding, "MockCloudService.svc/Request");
            _instance.AddServiceEndpoint(typeof(IResponseService), binding, "MockCloudService.svc/Response");
            _instance.AddServiceEndpoint(typeof(IUploadSessionService), binding, "MockCloudService.svc/UploadSession");
            _instance.AddServiceEndpoint(typeof(ICREMessageServiceInjection), binding, "MockCloudServiceInjection.svc");

            ServiceDebugBehavior debug = _instance.Description.Behaviors.Find<ServiceDebugBehavior>();

            // if not found - add behavior with setting turned on 
            if (debug == null)
            {
                _instance.Description.Behaviors.Add(
                     new ServiceDebugBehavior() { IncludeExceptionDetailInFaults = true });
            }
            else
            {
                // make sure setting is turned ON
                if (!debug.IncludeExceptionDetailInFaults)
                {
                    debug.IncludeExceptionDetailInFaults = true;
                }
            }

            _instance.Open();

            TraceUtils.WriteLine("MockCloudServiceHost.StartService: complete");
        }

        /// <summary>
        /// 
        /// </summary>
        public static void StopService()
        {
            TraceUtils.WriteLine("MockCloudServiceHost.StopService: invoked");

            try
            {
                if (_webApp != null)
                {
                    _webApp.Dispose();
                }
            }
            finally
            {
                _webApp = null;
            }

            try
            {
                if (_signalRHost != null)
                {
                    _signalRHost.Dispose();
                }
            }
            finally
            {
                _signalRHost = null;
            }

            try
            {
                MockCloudService.ResetMutableTenantState();

                if (_instance != null)
                {
                    _instance.Close();
                }
            }
            finally
            {
                if (_instance != null)
                {
                    _instance.Abort();
                }
                _instance = null;
            }

            TraceUtils.WriteLine("MockCloudServiceHost.StopService: complete");
        }

        /// <summary>
        /// 
        /// </summary>
        public static String SiteAddress
        {
            get { return WebAPIAddress; }
        }

        /// <summary>
        /// Returns the SignalR endpoint being used for notifications.
        /// </summary>
        public static String DisplayNotificationAddress
        { 
            get { return NotificationAddress; } 
        }

        /// <summary>
        /// Actual SignalR endpoint address
        /// </summary>
        public static String NotificationAddress
        {
            get
            {
                return _notificationAddress;
            }
        }

        /// <summary>
        /// Returns the WebAPI display address
        /// </summary>
        public static String DisplayWebAPIAddress
        {
            get { return WebAPIAddress; }
        }

        /// <summary>
        /// Actual WebAPI endpoint address
        /// </summary>
        public static String WebAPIAddress
        {
            get
            {
                return _webAppBaseAddress.ToString();
            }
        }
    }
}
