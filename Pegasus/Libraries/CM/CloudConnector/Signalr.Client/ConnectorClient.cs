using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.AspNet.SignalR.Client.Http;
using Sage.Connector.Signalr.Common;
using Sage.Connector.Signalr.Interfaces;
using Sage.Connector.Utilities.Platform.DotNet;
using Sage.Connector.Cloud.Integration.Interfaces.Headers;
using Sage.Connector.Cloud.Integration.Interfaces.Utils;

namespace Sage.Connector.Signalr.Client
{
    /// <summary>
    /// Client implemenation of IConnectorClient
    /// </summary>
    public class ConnectorClient : IConnectorClient
    {
        private readonly HashSet<Guid> _subscribed = new HashSet<Guid>();
        private readonly ThreadTaskQueue _eventQueue = new ThreadTaskQueue();
        private ClientState _state = ClientState.Disconnected;
        private HubConnection _connection;
        private IHubProxy _proxy;
        private readonly Object _lock = new Object();
        private readonly Guid _connectorId;
        private readonly IHttpClient _handler;
        private readonly string _baseAddress;
        private bool _disposed ;

        /// <summary>
        /// Internal class used for signing all web requests to the SignalR hub.
        /// </summary>
        internal class HttpConnectorClient : IHttpClient
        {
            private readonly IHttpClient _base =  new DefaultHttpClient();
            private readonly GetConnectorKeyMethod _getConnectorPremiseKey;
            private readonly Guid _connectorId;

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="connectorId">The connector id we will be signing requests for.</param>
            /// <param name="getConnectorPremiseKey">The delegate used to obtain the connector premise key.</param>
            public HttpConnectorClient(Guid connectorId, GetConnectorKeyMethod getConnectorPremiseKey)
            {
                if (connectorId.Equals(Guid.Empty))
                {
                    throw new ArgumentNullException("connectorId");
                }
                
                if (getConnectorPremiseKey == null)
                {
                    throw new ArgumentNullException("getConnectorPremiseKey");
                }

                _connectorId = connectorId;
                _getConnectorPremiseKey = getConnectorPremiseKey;
            }

            /// <summary>
            /// The action handler used to prepare and sign the requests.
            /// </summary>
            /// <param name="prepareRequest">The action which will be passing the message wrapper.</param>
            /// <returns>The new action which includes code for our message signing.</returns>
            Action<IRequest> HandleAction(Action<IRequest> prepareRequest)
            {
                return (req =>
                {
                    prepareRequest(req);

                    var requestField = typeof(HttpRequestMessageWrapper).GetField("_httpRequestMessage", BindingFlags.NonPublic | BindingFlags.Instance);

                    if ((requestField == null) || (_getConnectorPremiseKey == null)) return;

                    var hashKey = _getConnectorPremiseKey(_connectorId);
                        
                    var message = (HttpRequestMessage)requestField.GetValue(req);

                    if (message == null) return;
                            
                    var dateNow = new DateTimeOffset(DateTime.UtcNow);
                    var cNonce = Guid.NewGuid();

                    message.Headers.Date = dateNow;
                    message.Headers.Add(HeaderCommon.ServiceHeaderKeys[ServiceHeaderKeyTypes.ConnectorId], _connectorId.ToString());
                    message.Headers.Add(HeaderCommon.ServiceHeaderKeys[ServiceHeaderKeyTypes.CNonce], cNonce.ToString());

                    var hashManager = new MessageHashManager(hashKey);  
                            
                    var sign = new StringBuilder();

                    sign.Append(_connectorId);
                    sign.Append(message.Method.ToString());
                    sign.Append(dateNow.ToString("r"));
                    sign.Append(cNonce);
                    sign.Append(HttpUtility.UrlDecode(message.RequestUri.Query));
                    sign.Append(message.Headers.UserAgent);

                    var messageHash = hashManager.ComputeMessageHash(sign.ToString());

                    message.Headers.Add(HeaderCommon.ServiceHeaderKeys[ServiceHeaderKeyTypes.MessageHash], messageHash);
                });
            }

            /// <summary>
            /// Makes an async GET request to the specified url.
            /// </summary>
            /// <param name="url">The url to send the request to.</param>
            /// <param name="prepareRequest">A callback that initializes the request with default values.</param>
            /// <param name="isLongRunning">Indicates whether it is a long running request.</param>
            /// <returns>A Task{IResponse}.</returns>
            public Task<IResponse> Get(string url, Action<IRequest> prepareRequest, bool isLongRunning)
            {
                return _base.Get(url, HandleAction(prepareRequest), isLongRunning);
            }

            /// <summary>
            /// Initializes the http client with the specified connection.
            /// </summary>
            /// <param name="connection">The connection to use for initialization.</param>
            public void Initialize(IConnection connection)
            {
                _base.Initialize(connection);
            }

            /// <summary>
            /// Makes an async POST request to the specified url.
            /// </summary>
            /// <param name="url">The url to send the request to.</param>
            /// <param name="prepareRequest">A callback that initializes the request with default values.</param>
            /// <param name="postData">The data to post with the request.</param>
            /// <param name="isLongRunning">Indicates whether it is a long running request.</param>
            /// <returns>A Task{IResponse}.</returns>
            public Task<IResponse> Post(string url, Action<IRequest> prepareRequest, IDictionary<string, string> postData, bool isLongRunning)
            {
                return _base.Post(url, HandleAction(prepareRequest), postData, isLongRunning);
            }
         }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="connectorId">The connector id to use for push notifications.</param>
        /// <param name="getConnectorPremiseKey">The delegate to use for obtaining the connector premise key.</param>
        /// <param name="baseAddress">The base url to use for connecting to the SignalR endpoint.</param>
        public ConnectorClient(Guid connectorId, GetConnectorKeyMethod getConnectorPremiseKey, string baseAddress)
        {
            if (connectorId.Equals(Guid.Empty)) throw new ArgumentNullException("connectorId");
            if (getConnectorPremiseKey == null) throw new ArgumentNullException("getConnectorPremiseKey");
            if (String.IsNullOrEmpty(baseAddress)) throw new ArgumentNullException("baseAddress");
            
            _baseAddress = baseAddress;
            _connectorId = connectorId;

            _connection = new HubConnection(string.Format(ControllerCommon.SignalREndpoint, _baseAddress));
            _handler = new HttpConnectorClient(_connectorId, getConnectorPremiseKey);

            _connection.StateChanged += InternalStateChanged;
            _connection.Reconnected += InternalReconnect;

            _proxy = _connection.CreateHubProxy(ControllerCommon.HubName);
            _proxy.On("OnNotification", new Action<Guid, int>(InternalNotification));
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~ConnectorClient()
        {
            Dispose(false);
        }

        /// <summary>
        /// The event that gets fired when a SignalR notification is fired back to the client.
        /// </summary>
        public event NotifyHandler OnNotification;

        /// <summary>
        /// The event that gets fired when a SignalR connection state change occurs.
        /// </summary>
        public event StateHandler OnStateChange;

        /// <summary>
        /// Thread callback to allow for notifying the client of state changes. 
        /// </summary>
        /// <param name="state">The SignalR state associated with the state change event.</param>
        private void InternalStateChange(Object state)
        {
            if (OnStateChange != null)
            {
                OnStateChange(this, new ClientStateArgs(_state));
            }
        }

        /// <summary>
        /// Called when the connection state changes.
        /// </summary>
        /// <param name="state">Object that contains the old and new state of the connection.</param>
        private void InternalStateChanged(StateChange state)
        {
            switch (state.NewState)
            {
                case ConnectionState.Connected:
                    _state = ClientState.Connected;
                    break;

                case ConnectionState.Connecting:
                    _state = ClientState.Connecting;
                    break;

                case ConnectionState.Reconnecting:
                    _state = ClientState.Reconnecting;
                    break;

                case ConnectionState.Disconnected:
                    _state = (state.OldState == ConnectionState.Reconnecting) ? ClientState.UnexpectedDisconnect : ClientState.Disconnected;
                    lock (_lock)
                    {
                        _subscribed.Clear();
                    }
                    break;
            }

            _eventQueue.Queue((InternalStateChange), state);
        }

        /// <summary>
        /// Thread callback that occurs when a reconnect happens. This will ensure that the tenants are re-rgistered with the hub
        /// in case of (cloud) app domain recycle or restart.
        /// </summary>
        private void InternalReconnect()
        {
            Task.Run(async delegate
            {
                await Task.Delay(1000);

                try
                {
                    var unsubscribed = new HashSet<Guid>();
                    HashSet<Guid> subscribed;

                    lock (_lock)
                    {
                        subscribed = new HashSet<Guid>(_subscribed);
                    }

                    _proxy.Invoke("UnsubscribeAll", new object[] { }).Wait();

                    foreach (var tenantId in subscribed)
                    {
                        using (var resubscribed = _proxy.Invoke<bool>("TenantSubscribe", new object[] { tenantId }))
                        {
                            resubscribed.Wait();

                            if (!resubscribed.Result)
                            {
                                unsubscribed.Add(tenantId);
                            }
                        }
                    }

                    if (unsubscribed.Count > 0)
                    {
                        lock (_lock)
                        {
                            foreach (var tenantId in unsubscribed)
                            {
                                _subscribed.Remove(tenantId);
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    lock (_lock)
                    {
                        _subscribed.Clear();
                    }
                }
            });
        }

        /// <summary>
        /// Thread callback to allow for client notification, without suspending the SignalR threaad handling incoming data.
        /// </summary>
        /// <param name="state">Object that contains the client notify event args.</param>
        private void InternalNotify(Object state)
        {
            if (OnNotification != null)
            {
                OnNotification(this, (ClientNotifyArgs)state);
            }
        }

        /// <summary>
        /// Internal callback to handle notifications from SignalR.
        /// </summary>
        /// <param name="tenantId">The tenant id associated with the notification.</param>
        /// <param name="count">The count of notifications.</param>
        private void InternalNotification(Guid tenantId, int count)
        {
            if ((_proxy == null) || (_state != ClientState.Connected)) return;
                
            _proxy.Invoke("AcknowledgeNotification", new object[] { tenantId, count });

            if (OnNotification != null)
            {
                _eventQueue.Queue((InternalNotify), new ClientNotifyArgs(tenantId, count));
            }
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
        /// Protected dispose that allows for cleanup of managed and unmanaged resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                if (_connection != null)
                {
                    Shutdown();

                    _connection.Dispose();
                    _connection = null;
                }
            }

            _disposed = true;
        }

        /// <summary>
        /// Method to allow for connecting to the SignalR endpoint.
        /// </summary>
        public void Startup()
        {
            if (_disposed) return;

            try
            {
                if (_state == ClientState.Disconnected)
                {
                    _connection.Start(_handler).Wait();
                }
            }
            catch (Exception ex)
            {
                if ((ex is System.AggregateException) && (ex.InnerException is System.NullReferenceException))
                {
                    _connection.Dispose();
                    _connection = null;

                    _connection = new HubConnection(string.Format(ControllerCommon.SignalREndpoint, _baseAddress));
                    _connection.StateChanged += InternalStateChanged;
                    _connection.Reconnected += InternalReconnect;

                    _proxy = _connection.CreateHubProxy(ControllerCommon.HubName);
                    _proxy.On("OnNotification", new Action<Guid, int>(InternalNotification));

                    _connection.Start(_handler).Wait();

                    return;
                }
                throw;
            }
        }

        /// <summary>
        /// Method to allow for disconnecting from the SignalR endpoint.
        /// </summary>
        public void Shutdown()
        {
            if (_disposed) return;

            try
            {
                if (_state == ClientState.Connected)
                {
                    _proxy.Invoke("UnsubscribeAll", new object[] { });
                }
            }
            finally
            {
                _connection.Stop(TimeSpan.FromSeconds(5));
            }
        }

        /// <summary>
        /// Returns the current client state
        /// </summary>
        public ClientState State
        {
            get
            {
                return _state;
            }
        }

        /// <summary>
        /// The base url address used for connecting to the SignalR endpoint.
        /// </summary>
        public string BaseAddress
        {
            get
            {
                return _baseAddress;
            }
        }
    
        /// <summary>
        /// The connector id which identifies the client connector.
        /// </summary>
        public Guid ConnectorId
        {
            get
            {
                return _connectorId;
            }
        }

        /// <summary>
        /// Subscribes a SignalR client for tenant notifications.
        /// </summary>9
        /// <param name="tenantId">The Tenant id to recieve notifications for.</param>
        public bool TenantSubscribe(Guid tenantId)
        {
            if (_state != ClientState.Connected) return false;
                
            using (var subscribed = _proxy.Invoke<bool>("TenantSubscribe", new object[] { tenantId }))
            {
                subscribed.Wait();

                if (!subscribed.Result) return false;
                    
                lock (_lock)
                {
                    if (!_subscribed.Contains(tenantId))
                    {
                        _subscribed.Add(tenantId);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Unsubscribes a SignalR client from recieving tenant notifications.
        /// </summary>
        /// <param name="tenantId">The Tenant id to stop recieve notifications for.</param>
        /// <returns>True if the client successfully unsubscribed from the tenant id, otherwise false.</returns>
        public bool TenantUnsubscribe(Guid tenantId)
        {
            if (_state != ClientState.Connected) return false;

            using (var unsubsubscribed = _proxy.Invoke<bool>("TenantUnsubscribe", new object[] { tenantId }))
            {
                unsubsubscribed.Wait();

                if (!unsubsubscribed.Result) return false;

                lock (_lock)
                {
                    if (_subscribed.Contains(tenantId))
                    {
                        _subscribed.Remove(tenantId);
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Returns an enumerable list of tenant id's that the client is subscribed to.
        /// </summary>
        /// <returns>An enumerable list of tenant guids associated with the calling client.</returns>
        public IEnumerable<Guid> GetSubscribedTenants()
        {
            lock (_lock)
            {
                return new HashSet<Guid>(_subscribed);
            }
        }
    }
}
