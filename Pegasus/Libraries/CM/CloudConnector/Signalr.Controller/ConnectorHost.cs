using System;
using System.Text;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Hosting;
using Owin;
using Sage.Connector.Signalr.Common;
using Sage.Connector.Signalr.Interfaces;
using Sage.Connector.Cloud.Integration.Interfaces.Headers;
using Sage.Connector.Cloud.Integration.Interfaces.Utils;
using System.Collections.Concurrent;

namespace Sage.Connector.Signalr.Controller
{
    /// <summary>
    /// The SignalR host wrapper.
    /// </summary>
    public sealed class ConnectorHost : IConnectorHost
    {
        private IDisposable _owinApp = default(IDisposable);
        private IConnectorController _controller = default(IConnectorController);
        private readonly string _baseAddress;
        private bool _disposed;

        /// <summary>
        /// Middleware for authenticating signed requests. This class is called before the SignalR hubs 
        /// ever see the connection/request.
        /// </summary>
        internal class ConnectorMiddleware : OwinMiddleware
        {
            /// <summary>
            /// Internal class for storing last connector date nonce values
            /// </summary>
            internal class DateNonce
            {
                public DateTimeOffset RequestDate { get; set; }
                public Guid RequestNonce { get; set; }

                public DateNonce(DateTimeOffset requestDate, Guid requestNonce)
                {
                    RequestDate = requestDate;
                    RequestNonce = requestNonce;
                }
            }

            private static readonly ConcurrentDictionary<Guid, DateNonce> _dateNonceState = new ConcurrentDictionary<Guid, DateNonce>();
            public static GetConnectorKeyMethod GetConnectorKey { get; set; }

            public ConnectorMiddleware(OwinMiddleware next) : base(next) { }

            /// <summary>
            /// Determines if the request is a replay by checking that the request time is >= the last stored value and ensuring 
            /// that the client nonce is not a duplicate.
            /// </summary>
            /// <param name="connectorId">The connector id to validate.</param>
            /// <param name="dateRequest">The dateoffset for the current request.</param>
            /// <param name="nonce">The nonce guid for the current request.</param>
            /// <returns>Returns true if this is a replay, otherwise false.</returns>
            private static bool IsReplay(Guid connectorId, DateTimeOffset dateRequest, Guid nonce)
            {
                DateNonce dateNonce;

                if (!_dateNonceState.TryGetValue(connectorId, out dateNonce)) return false;

                return ((dateRequest < dateNonce.RequestDate) || dateNonce.RequestNonce.Equals(nonce));
            }

            /// <summary>
            /// Removes the nonce state for the connector id.
            /// </summary>
            /// <param name="connectorId">The connector id to remove the values for.</param>
            private static void RemoveState(Guid connectorId)
            {
                DateNonce dateNonce;

                _dateNonceState.TryRemove(connectorId, out dateNonce);
            }

            /// <summary>
            /// Updates the last date time offset and nonce values for the specified connector id.
            /// </summary>
            /// <param name="connectorId">The connector id to store the values for.</param>
            /// <param name="dateRequest">The date time offset of the last request.</param>
            /// <param name="nonce">The nonce value of the last request.</param>
            private static void UpdateState(Guid connectorId, DateTimeOffset dateRequest, Guid nonce)
            {
                var dateNonce = new DateNonce(dateRequest, nonce);

                _dateNonceState.AddOrUpdate(connectorId, dateNonce, (key, oldValue) => dateNonce);
            }

            /// <summary>
            /// Task invoke for the processing of web requests in the pipe.
            /// </summary>
            /// <param name="context">The OWIN context that contains the request and response objects.</param>
            /// <returns>The async task for request processing.</returns>
            public override async Task Invoke(IOwinContext context)
            {
                var connectorIdHeader = context.Request.Headers[HeaderCommon.ServiceHeaderKeys[ServiceHeaderKeyTypes.ConnectorId]];
                var hashHeader = context.Request.Headers[HeaderCommon.ServiceHeaderKeys[ServiceHeaderKeyTypes.MessageHash]];
                var cNonceHeader = context.Request.Headers[HeaderCommon.ServiceHeaderKeys[ServiceHeaderKeyTypes.CNonce]];
                var dateHeader = context.Request.Headers["Date"];
                var agentHeader = context.Request.Headers["User-Agent"];

                if ((GetConnectorKey == null) ||
                    String.IsNullOrEmpty(cNonceHeader) ||
                    String.IsNullOrEmpty(connectorIdHeader) || 
                    String.IsNullOrEmpty(hashHeader) || 
                    String.IsNullOrEmpty(dateHeader) || 
                    String.IsNullOrEmpty(agentHeader))
                {
                    context.Response.StatusCode = 403;
                    return;
                }

                Guid connectorId;

                if (!Guid.TryParse(connectorIdHeader, out connectorId))
                {
                    context.Response.StatusCode = 403;
                    return;
                }

                Guid cNonce;

                if (!Guid.TryParse(cNonceHeader, out cNonce))
                {
                    context.Response.StatusCode = 403;
                    return;
                }

                var connectorKey = GetConnectorKey(connectorId);

                if (String.IsNullOrEmpty(connectorKey))
                {
                    context.Response.StatusCode = 403;
                    return;
                }

                var dateServer = new DateTimeOffset(DateTime.UtcNow);
                DateTimeOffset dateRequest;

                if (!DateTimeOffset.TryParse(dateHeader, out dateRequest))
                {
                    context.Response.StatusCode = 403;
                    return;
                }

                if (!context.Request.IsSecure)
                {
                    var difference = (dateRequest > dateServer) ? dateRequest.Subtract(dateServer) : dateServer.Subtract(dateRequest);

                    if ((difference.Minutes >= 15) || IsReplay(connectorId, dateRequest, cNonce))
                    {
                        context.Response.StatusCode = 403;
                        return;
                    }
                }

                var sign = new StringBuilder();

                sign.Append(connectorId);
                sign.Append(context.Request.Method);
                sign.Append(dateRequest.ToString("r"));
                sign.Append(cNonce);
                sign.Append(HttpUtility.UrlDecode(context.Request.Uri.Query));
                sign.Append(agentHeader);

                var hashManager = new MessageHashManager(connectorKey);
                var messageHash = hashManager.ComputeMessageHash(sign.ToString());

                if (!messageHash.Equals(hashHeader))
                {
                    context.Response.StatusCode = 403;
                    return;
                }

                if (context.Request.IsSecure)
                {
                    RemoveState(connectorId);
                }
                else
                {
                    UpdateState(connectorId, dateRequest, cNonce);
                }

                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, connectorId.ToString())
                };
                                    
                var identity = new ClaimsIdentity(claims, "Basic");
 
                context.Request.User = new ClaimsPrincipal(identity);

                await Next.Invoke(context);
            }
        }

        /// <summary>
        /// Internal class for handling the OWIN startup.
        /// </summary>
        internal class Startup
        {
            public void Configuration(IAppBuilder app)
            {
                app.Use(typeof(ConnectorMiddleware));
                app.MapSignalR();
            }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="baseAddress">The base url to use for hosting the SignalR endpoint.</param>
        /// <param name="getConnectorKey">The delegate to use for obtaining the connector key.</param>
        public ConnectorHost(string baseAddress, GetConnectorKeyMethod getConnectorKey)
        {
            if (string.IsNullOrEmpty(baseAddress))
            {
                throw new ArgumentNullException("baseAddress");
            }

            _baseAddress = baseAddress;

            ConnectorMiddleware.GetConnectorKey = getConnectorKey;
            GlobalHost.Configuration.DefaultMessageBufferSize = 16384;

            try
            {
                _owinApp = WebApp.Start<Startup>(_baseAddress);
                _controller = ConnectorController.Instance;
            }
            catch (Exception)
            {
                Dispose(); 

                throw;
            }
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~ConnectorHost()
        {
            Dispose(false);
        }

        /// <summary>
        /// Public dispose method.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Private dispose that allows for cleanup of web app and controller.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                ConnectorMiddleware.GetConnectorKey = null;

                if (_controller != null)
                {
                    _controller.Dispose();
                    _controller = null;
                }
                if (_owinApp != null)
                {
                    _owinApp.Dispose();
                    _owinApp = null;
                }
            }

            _disposed = true;
        }

        /// <summary>
        /// The connector controller instance.
        /// </summary>
        public IConnectorController Controller
        {
            get
            {
                return _controller;
            }
        }

        /// <summary>
        /// The OWIN web app instance.
        /// </summary>
        public IDisposable OwinApp
        {
            get
            {
                return _owinApp;
            }
        }

        /// <summary>
        /// The base address being serviced by the SignalR connector controller.
        /// </summary>
        public string BaseAddress
        {
            get
            {
                return _baseAddress;
            }
        }

        /// <summary>
        /// The actual SignalR endpoint for the connector controller.
        /// </summary>
        public string EndpointAddress
        {
            get
            {
                return string.Format(ControllerCommon.SignalREndpoint, _baseAddress);
            }
        }
    }
}
