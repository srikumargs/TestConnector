using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sage.Connector.Common.DataContracts;
using Sage.Connector.StateService.Interfaces;
using Sage.Connector.StateService.Interfaces.DataContracts;

namespace ConnectorServiceMonitor.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    public class ConnectorStateHelper
    {
        private ConnectorState _connectorState;
        private RequestState[] _requests;
        /// <summary>
        /// 
        /// </summary>
        public ConnectorStateHelper(ConnectorState state, RequestState[] requests)
        {
            _connectorState = state;
            _requests = requests;
            if (_connectorState != null)
            {
                ComputeAggregateConnectorState();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string CurrentInterfaceVersion
        {
            get
            { return _connectorState.ConnectorBackOfficeIntegrationInterfaceVersion; }
        }
        /// <summary>
        /// 
        /// </summary>
        public ConnectorState ConnectorState
        {
            get { return _connectorState; }
        }

        /// <summary>
        /// 
        /// </summary>
        public UpdateInfo UpdateInfo
        {
            get { return _connectorState.ConnectorUpdateInfo; }
        }
        /// <summary>
        /// 
        /// </summary>
        public ConnectorStatusEnum AggregateBackofficeStatus
        {
            get{ return _aggregateBackofficeStatus;}
        }
        ConnectorStatusEnum _aggregateBackofficeStatus;

        /// <summary>
        /// 
        /// </summary>
        public ConnectorStatusEnum AggregateTenantStatus
        {
            get { return _aggregateTenantStatus; }
        }
        ConnectorStatusEnum _aggregateTenantStatus;

        /// <summary>
        /// 
        /// </summary>
        public List<Connection> Connections
        {
            get
            {
                if (_connections == null)
                    _connections = new List<Connection>();
                return _connections;
            }
        }
        List<Connection> _connections = null;

        /// <summary>
        /// 
        /// </summary>
        public void ComputeAggregateConnectorState()
        {
            if (_connectorState != null && _connectorState.IntegratedConnectionStates != null)
            {
                int count = _connectorState.IntegratedConnectionStates.Length;
                Connection newConnection;

                foreach (var connection in _connectorState.IntegratedConnectionStates)
                {
                    // Only include requests for this tenant!
                    RequestState[] tenantRequests = null;
                    if (_requests != null)
                    {
                        IEnumerable<RequestState> requestsList =
                            _requests.Where(request => request.CloudTenantId == connection.TenantId);
                        if (requestsList.Count() > 0)
                        {
                            tenantRequests = requestsList.ToArray();
                        }
                    }

                    newConnection = new Connection(_connectorState, connection, tenantRequests);
                    Connections.Add(newConnection);
                }

                _aggregateBackofficeStatus = ConnectorStatusEnum.None;
                if (Connections.Find(x => x.BackOfficeStatus == ConnectorStatusEnum.BrokenAndProcessing) != null)
                    _aggregateBackofficeStatus = ConnectorStatusEnum.BrokenAndProcessing;
                else if (Connections.Find(x => x.BackOfficeStatus == ConnectorStatusEnum.Broken) != null)
                    _aggregateBackofficeStatus = ConnectorStatusEnum.Broken;
                else if (Connections.Find(x => x.BackOfficeStatus == ConnectorStatusEnum.DisabledAndProcessing) != null)
                    _aggregateBackofficeStatus = ConnectorStatusEnum.DisabledAndProcessing;
                else if (Connections.Find(x => x.BackOfficeStatus == ConnectorStatusEnum.Disabled) != null)
                    _aggregateBackofficeStatus = ConnectorStatusEnum.Disabled;
                else if (Connections.Find(x => x.BackOfficeStatus == ConnectorStatusEnum.OkAndProcessing) != null)
                    _aggregateBackofficeStatus = ConnectorStatusEnum.OkAndProcessing;
                else if (Connections.Find(x => x.BackOfficeStatus == ConnectorStatusEnum.Ok) != null)
                    _aggregateBackofficeStatus = ConnectorStatusEnum.Ok;

                //Overall Cloud status
                _aggregateTenantStatus = ConnectorStatusEnum.None;
                if (Connections.Find(x => x.TenantStatus == ConnectorStatusEnum.BrokenAndProcessing) != null)
                    _aggregateTenantStatus = ConnectorStatusEnum.BrokenAndProcessing;
                else if (Connections.Find(x => x.TenantStatus == ConnectorStatusEnum.Broken) != null)
                    _aggregateTenantStatus = ConnectorStatusEnum.Broken;
                else if (Connections.Find(x => x.TenantStatus == ConnectorStatusEnum.DisabledAndProcessing) != null)
                    _aggregateTenantStatus = ConnectorStatusEnum.DisabledAndProcessing;
                else if (Connections.Find(x => x.TenantStatus == ConnectorStatusEnum.Disabled) != null)
                    _aggregateTenantStatus = ConnectorStatusEnum.Disabled;
                else if (Connections.Find(x => x.TenantStatus == ConnectorStatusEnum.OkAndProcessing) != null)
                    _aggregateTenantStatus = ConnectorStatusEnum.OkAndProcessing;
                else if (Connections.Find(x => x.TenantStatus == ConnectorStatusEnum.Ok) != null)
                    _aggregateTenantStatus = ConnectorStatusEnum.Ok;
            }

        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Connection
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectorState"></param>
        /// <param name="integratedConnectionState"></param>
        /// <param name="requests"></param>
        public Connection(ConnectorState connectorState, IntegratedConnectionState integratedConnectionState, RequestState[] requests)
        {
            ConnectorState = connectorState;
            Requests = requests;
            IntegratedConnectionState = integratedConnectionState;
            SetInProgressValues();
            ComputeCurrentBackOfficeStatus();
            ComputeCurrentTenantStatus();
        }
        /// <summary>
        /// 
        /// </summary>
        public IntegratedConnectionState IntegratedConnectionState
        { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ConnectorState ConnectorState
        { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public RequestState[] Requests
        { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string TenantName { get { return IntegratedConnectionState.TenantName; } }
        /// <summary>
        /// 
        /// </summary>
        public string BackOfficeCompanyName { get{ return IntegratedConnectionState.BackOfficeCompanyName; } }
        /// <summary>
        /// 
        /// </summary>
        public ConnectorStatusEnum TenantStatus 
        { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public uint TenantRequestsInProgress
        { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        public ConnectorStatusEnum BackOfficeStatus
        { get; set; }

        private void ComputeCurrentTenantStatus()
        {
            // Get the raw active status
            bool cloudConnectionEnabledToReceive, cloudConnectionEnabledToSend, connectorUpdateRequired;
            ConnectionActiveStatus connectionActiveStatus = ConnectionActiveStatusHelper.GetCloudConnectivityStatus(
                ConnectorState,
                IntegratedConnectionState,
                out cloudConnectionEnabledToReceive,
                out cloudConnectionEnabledToSend,
                out connectorUpdateRequired);

            // Convert to the appropriate monitor connection status
            TenantStatus = ConvertToConnectorStatusEnum(connectionActiveStatus);
        }

        private void ComputeCurrentBackOfficeStatus()
        {
            // Get the raw active status
            bool backOfficeConnectionEnabledToReceive, backOfficeUpdateRequired;
            ConnectionActiveStatus connectionActiveStatus = ConnectionActiveStatusHelper.GetBackOfficeConnectionStatus(
                IntegratedConnectionState,
                out backOfficeConnectionEnabledToReceive,
                out backOfficeUpdateRequired);

            // Convert to the appropriate monitor connection status
            BackOfficeStatus = ConvertToConnectorStatusEnum(connectionActiveStatus);
        }

        private ConnectorStatusEnum ConvertToConnectorStatusEnum(ConnectionActiveStatus connectionActiveStatus)
        {
            ConnectorStatusEnum result = ConnectorStatusEnum.None;
            switch (connectionActiveStatus)
            {
                case ConnectionActiveStatus.Inactive:
                    result = TenantRequestsInProgress > 0 ? ConnectorStatusEnum.DisabledAndProcessing : ConnectorStatusEnum.Disabled;
                    break;
                case ConnectionActiveStatus.Active:
                    result = TenantRequestsInProgress > 0 ? ConnectorStatusEnum.OkAndProcessing : ConnectorStatusEnum.Ok;
                    break;
                case ConnectionActiveStatus.Broken:
                    result = TenantRequestsInProgress > 0 ? ConnectorStatusEnum.BrokenAndProcessing : ConnectorStatusEnum.Broken;
                    break;
                case ConnectionActiveStatus.None:
                default:
                    result = ConnectorStatusEnum.None;
                    break;
            }

            return result;
        }

        /// <summary>
        /// If we have requests, compute the 'in process' count from
        /// the available activities;
        /// if not, use the old style 'requests - responses' counts.
        /// </summary>
        private void SetInProgressValues()
        {
            if (null != Requests)
            {
                TenantRequestsInProgress =
                    Convert.ToUInt16(
                        Requests.Count(
                            request =>
                                (request.RequestStatus == RequestStatus.InProgress) ||
                                (request.RequestStatus == RequestStatus.InProgressBindableWorkProcessing)));
            }
            else
            {
                uint responsesSent = IntegratedConnectionState.NonErrorResponsesSentCount + IntegratedConnectionState.ErrorResponsesSentCount;
                if (responsesSent > IntegratedConnectionState.RequestsReceivedCount)
                {
                    TenantRequestsInProgress = 0;
                }
                else
                {
                    TenantRequestsInProgress = IntegratedConnectionState.RequestsReceivedCount - responsesSent;
                }

            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum ConnectorStatusEnum
    {
        /// <summary>
        /// 
        /// </summary>
        None = 0,
        /// <summary>
        /// 
        /// </summary>
        Ok = 1,
        /// <summary>
        /// 
        /// </summary>
        OkAndProcessing = 2,
        /// <summary>
        /// 
        /// </summary>
        Disabled = 3,
        /// <summary>
        /// 
        /// </summary>
        DisabledAndProcessing = 4,
        /// <summary>
        /// 
        /// </summary>
        Broken = 5,
        /// <summary>
        /// 
        /// </summary>
        BrokenAndProcessing = 6
    }

}
