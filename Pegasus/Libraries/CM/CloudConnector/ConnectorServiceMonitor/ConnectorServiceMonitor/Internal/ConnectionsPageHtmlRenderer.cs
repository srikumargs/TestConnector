using System;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using ConnectorServiceMonitor.ViewModel;
using System.Text;
using Sage.Connector.StateService.Interfaces.DataContracts;

namespace ConnectorServiceMonitor.Internal
{
    internal sealed class ConnectionsPageHtmlRenderer : HtmlRenderer
    {
        public ConnectionsPageHtmlRenderer(ImageManager imageManager, Control control)
            : base(imageManager, control)
        { }

        protected override void ComputeCol1(Graphics g, Font font)
        {
            _maxColumn1Width = Convert.ToUInt32(Math.Max(g.MeasureString(Strings.Connections_Col1_LastCommunicationAttempt, font).Width, _maxColumn1Width));
            _maxColumn1Width = Convert.ToUInt32(Math.Max(g.MeasureString(Strings.Connections_Col1_LastSuccessfulCommunication, font).Width, _maxColumn1Width));
            //_maxColumn1Width = Convert.ToUInt32(Math.Max(g.MeasureString(Strings.Connections_Col1_NextScheduledCommunication, font).Width, _maxColumn1Width));
            //_maxColumn1Width += Convert.ToUInt32(g.MeasureString("WWW", font).Width);

            _maxRowHeight = Convert.ToUInt32(_imageManager.Blank.Height);
            _maxRowHeight = Convert.ToUInt32(Math.Max(g.MeasureString(Strings.Connections_Col1_LastCommunicationAttempt, font).Height * 1.20, _maxRowHeight));
            _maxRowHeight = Convert.ToUInt32(Math.Max(g.MeasureString(Strings.Connections_Col1_LastSuccessfulCommunication, font).Height * 1.20, _maxRowHeight));
            //_maxRowHeight = Convert.ToUInt32(Math.Max(g.MeasureString(Strings.Connections_Col1_NextScheduledCommunication, font).Height * 1.20, _maxRowHeight));
        }

        public String Render(ServiceStatus status, ConnectorStateHelper stateServiceHelper)
        {
            const int initialSize = 16384;
            var sb = new StringBuilder(Strings.General_HtmlDocument, initialSize);

            if (status.MonitorServiceStatus.HasValue)
            {
                if (status.ConnectorServiceState.ConnectorState != null)
                {
                    var connectorState = status.ConnectorServiceState.ConnectorState;
                    RenderIntegratedConnectionStates(sb, stateServiceHelper);
                }
            }

            ReplaceCommonFormatting(sb);

            return sb.ToString();
        }

        private void RenderIntegratedConnectionStates(StringBuilder sb, ConnectorStateHelper connectorState)
        {
            Int32 i = 1;
            Int32 connectionCount = connectorState.Connections.Count;
            foreach (var state in connectorState.Connections)
            {
                if (connectionCount > 1)
                {
                    AppendNewBoldRow(sb
                        , String.Format(Strings.General_Col1_ConnectionNumber, i++)
                        , String.Format("{0}  {1} {2} {3}  {4}"
                                , state.BackOfficeCompanyName
                                , StatusImage(state.BackOfficeStatus, Side.Left)
                                , StatusArrowImage(state.BackOfficeStatus ==  state.TenantStatus, Side.Left)
                                , StatusImage(state.TenantStatus, Side.Right)
                                , state.TenantName)
                        );
                }
                else
                {
                    AppendNewBoldRow(sb
                        , "Connection"
                        , String.Format("{0}  {1} {2} {3}  {4}"
                                , state.BackOfficeCompanyName
                                , StatusImage(state.BackOfficeStatus, Side.Left)
                                , StatusArrowImage(state.BackOfficeStatus == state.TenantStatus, Side.Left)
                                , StatusImage(state.TenantStatus, Side.Right)
                                , state.TenantName)
                        );
                }

                if (state.IntegratedConnectionState.BackOfficeConnectivityStatus != BackOfficeConnectivityStatus.Normal && state.IntegratedConnectionState.BackOfficeConnectivityStatus != BackOfficeConnectivityStatus.None)
                {
                    AppendNewNormalRow(sb, String.Empty, FormatBackOfficeConnectivityStatus(state.IntegratedConnectionState.BackOfficeConnectivityStatus));
                }
                if (state.IntegratedConnectionState.TenantConnectivityStatus != TenantConnectivityStatus.Normal && state.IntegratedConnectionState.TenantConnectivityStatus != TenantConnectivityStatus.None)
                {
                    AppendNewNormalRow(sb, String.Empty, FormatTenantConnectivityStatus(state.IntegratedConnectionState.TenantConnectivityStatus));
                }
                AppendNewNormalRow(sb, Strings.Connections_Col1_IntegrationModes, FormatIntegrationModes(state.IntegratedConnectionState.IntegrationEnabledStatus));
                var now = DateTime.Now;
                AppendNewNormalRow(sb, Strings.Connections_Col1_LastCommunicationAttempt, Common.CreateTimeTillOccurrenceTimeString(now, state.IntegratedConnectionState.LastAttemptedCommunicationWithCloud.ToLocalTime()));
                AppendNewNormalRow(sb, Strings.Connections_Col1_LastSuccessfulCommunication, Common.CreateTimeTillOccurrenceTimeString(now, state.IntegratedConnectionState.LastSuccessfulCommunicationWithCloud.ToLocalTime()));
                //AppendNewNormalRow(sb, Strings.Connections_Col1_NextScheduledCommunication, Common.CreateTimeTillOccurrenceTimeString(now, state.IntegratedConnectionState.NextScheduledCommunicationWithCloud.ToLocalTime()));
                AppendNewNormalRow(sb, Strings.Connections_Col1_RequestsRecieved, state.IntegratedConnectionState.RequestsReceivedCount.ToString());
                AppendNewNormalRow(sb, Strings.Connections_Col1_RequestsInProgress, state.IntegratedConnectionState.RequestsInProgressCount.ToString());
                AppendNewNormalRow(sb, Strings.Connections_Col1_ResponsesSent, String.Format("{0} (with {1} error responses)", state.IntegratedConnectionState.NonErrorResponsesSentCount + state.IntegratedConnectionState.ErrorResponsesSentCount, state.IntegratedConnectionState.ErrorResponsesSentCount));
                AppendNewNormalRow(sb, String.Empty, String.Empty);
            }
        }

        private string FormatBackOfficeConnectivityStatus(BackOfficeConnectivityStatus status)
        {
            String result = String.Empty;
            switch (status)
            {
                case BackOfficeConnectivityStatus.ConnectivityBroken:
                    result = Strings.Connection_BackOfficeConnState_ConnectivityBroken;
                    break;
                case BackOfficeConnectivityStatus.Incompatible:
                    result = Strings.Connection_BackOfficeConnState_Incompatible;
                    break;
                case BackOfficeConnectivityStatus.None:
                    result = Strings.Connection_BackOfficeConnState_None;
                    break;
                case BackOfficeConnectivityStatus.Normal:
                    result = Strings.Connection_BackOfficeConnState_Normal;
                    break;
                default:
                    result = Strings.Connection_BackOfficeConnState_None;
                    break;
            }

            return result;
        }

        private string FormatTenantConnectivityStatus(TenantConnectivityStatus status)
        {
            String result = String.Empty;
            switch(status)
            {
                case TenantConnectivityStatus.Normal:
                    result = Strings.Connection_TenantConnState_Normal;
                    break;
                case TenantConnectivityStatus.CloudUnavailable:
                    result = Strings.Connection_TenantConnState_CloudUnavailable;
                    break;
                case TenantConnectivityStatus.CommunicationFailure:
                    result = Strings.Connection_TenantConnState_CommunicationFailure;
                    break;
                case TenantConnectivityStatus.GatewayServiceUnavailable:
                    result = Strings.Connection_TenantConnState_GatewayServiceUnavailable;
                    break;
                case TenantConnectivityStatus.IncompatibleClient:
                    result = Strings.Connection_TenantConnState_IncompatibleClient;
                    break;
                case TenantConnectivityStatus.InternetConnectionUnavailable:
                    result = Strings.Connection_TenantConnState_InternetConnUnavailable;
                    break;
                case TenantConnectivityStatus.InvalidConnectionInformation:
                    result = Strings.Connection_TenantConnState_InvalidConnectionInfo;
                    break;
                case TenantConnectivityStatus.LocalNetworkUnavailable:
                    result = Strings.Connection_TenantConnState_LocalNetworkUnavailable;
                    break;
                case TenantConnectivityStatus.Reconfigure:
                    result = Strings.Connection_TenantConnState_Reconfigure;
                    break;
                case TenantConnectivityStatus.TenantDisabled:
                    result = Strings.Connection_TenantConnState_TenantDisabled;
                    break;
                default:
                    result = Strings.Connection_TenantConnState_None;
                    break;
            }

            return result;
        }

        private string FormatIntegrationModes(IntegrationEnabledStatus status)
        {
            string returnString = string.Empty;
            bool currentStateShowing = false;

            if (status  == IntegrationEnabledStatus.None)
            {
                returnString += string.Format("{0} {1}"
                    , Strings.General_ImageHtmlFragment.Replace("{BitmapSource}", _imageManager.CriticalBitmapFileName)
                    , Strings.Connections_IntegrationDisabled
                    );
                return returnString;
            }

            if (((status & IntegrationEnabledStatus.BackOfficeProcessing) == IntegrationEnabledStatus.BackOfficeProcessing))
            {
                returnString += string.Format("{0} {1}"
                    , Strings.General_ImageHtmlFragment.Replace("{BitmapSource}", _imageManager.OKBitmapFileName)
                    , Strings.Connections_IntegrationProcessing
                    );
            }
            else
            {
                returnString += string.Format("{0} {1}"
                    , Strings.General_ImageHtmlFragment.Replace("{BitmapSource}", _imageManager.SeriousBitmapFileName)
                    , Strings.Connections_IntegrationNotProcessing
                    );
            }

            if ((status & IntegrationEnabledStatus.CloudGetRequests) != IntegrationEnabledStatus.CloudGetRequests)
            {
                returnString += string.Format(" {0}{1}"
                    , currentStateShowing ? "/ " : "("
                    , Strings.Connections_Integration_GetDisabled
                    );
                currentStateShowing = true;
            }

            if ((status & IntegrationEnabledStatus.CloudPutResponses) != IntegrationEnabledStatus.CloudPutResponses)
            {
                returnString += string.Format(" {0}{1}"
                    , currentStateShowing ? "/ " : "("
                    , Strings.Connections_Integration_PutDisabled
                    );
                currentStateShowing = true;
            }
            returnString += currentStateShowing ? ")" : string.Empty;

            return returnString;
        }

        private static void AppendNewBoldRow(StringBuilder sb, String col1, String col2)
        {
            sb.Replace("{AdditionalHtml}", Strings.General_2ColRow_Bold_HtmlFragment);
            sb.Replace("{FieldName}", col1);
            sb.Replace("{ImageHtmlFragment}", String.Empty);
            sb.Replace("{FieldValue}", col2);
        }

        private static void AppendNewNormalRow(StringBuilder sb, String col1, String col2)
        {
            sb.Replace("{AdditionalHtml}", Strings.General_2ColRow_HtmlFragment);
            sb.Replace("{FieldName}", col1);
            sb.Replace("{ImageHtmlFragment}", String.Empty);
            sb.Replace("{FieldValue}", col2);
        }
    }
}
