using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using ConnectorServiceMonitor.ViewModel;
using IWshRuntimeLibrary;
using Sage.Connector.MonitorService.Interfaces.DataContracts;
using Sage.Connector.StateService.Interfaces.DataContracts;

namespace ConnectorServiceMonitor.Internal
{
    internal sealed class GeneralPageHtmlRenderer : HtmlRenderer
    {
        public GeneralPageHtmlRenderer(ImageManager imageManager, Control control)
            : base(imageManager, control)
        { }

        protected override void ComputeCol1(Graphics g, Font font)
        {
            _maxColumn1Width = Convert.ToUInt32(Math.Max(g.MeasureString(Strings.General_Col1_ConnectorMonitorStatus, font).Width, _maxColumn1Width));
            _maxColumn1Width = Convert.ToUInt32(Math.Max(g.MeasureString(Strings.General_Col1_ConnectorStatus, font).Width, _maxColumn1Width));
            _maxColumn1Width = Convert.ToUInt32(Math.Max(g.MeasureString(Strings.General_Col1_ConnectorUptime, font).Width, _maxColumn1Width));
            _maxColumn1Width = Convert.ToUInt32(Math.Max(g.MeasureString(Strings.General_Col1_Restart, font).Width, _maxColumn1Width));
            _maxColumn1Width = Convert.ToUInt32(Math.Max(g.MeasureString(Strings.General_Col1_ConnectorVersion, font).Width, _maxColumn1Width));
            _maxColumn1Width = Convert.ToUInt32(Math.Max(g.MeasureString(Strings.General_Col1_BackofficeVersion, font).Width, _maxColumn1Width));
            _maxColumn1Width = Convert.ToUInt32(Math.Max(g.MeasureString(Strings.General_Col1_CloudServiceStatus, font).Width, _maxColumn1Width));
            _maxColumn1Width = Convert.ToUInt32(Math.Max(g.MeasureString(Strings.General_Col1_AggregateStatus, font).Width, _maxColumn1Width));
            _maxColumn1Width = Convert.ToUInt32(Math.Max(g.MeasureString(Strings.General_Col1_ConnectionNumber, font).Width, _maxColumn1Width));
            _maxColumn1Width += Convert.ToUInt32(g.MeasureString("WW", font).Width);

            _maxRowHeight = Convert.ToUInt32(_imageManager.GreenLight.Height * 1.20);
            _maxRowHeight = Convert.ToUInt32(Math.Max(g.MeasureString(Strings.General_Col1_ConnectorMonitorStatus, font).Height * 1.20, _maxRowHeight));
            _maxRowHeight = Convert.ToUInt32(Math.Max(g.MeasureString(Strings.General_Col1_ConnectorStatus, font).Height * 1.20, _maxRowHeight));
            _maxRowHeight = Convert.ToUInt32(Math.Max(g.MeasureString(Strings.General_Col1_ConnectorUptime, font).Height * 1.20, _maxRowHeight));
            _maxRowHeight = Convert.ToUInt32(Math.Max(g.MeasureString(Strings.General_Col1_Restart, font).Height * 1.20, _maxRowHeight));
            _maxRowHeight = Convert.ToUInt32(Math.Max(g.MeasureString(Strings.General_Col1_ConnectorVersion, font).Height * 1.20, _maxRowHeight));
            _maxRowHeight = Convert.ToUInt32(Math.Max(g.MeasureString(Strings.General_Col1_BackofficeVersion, font).Height * 1.20, _maxRowHeight));
            _maxRowHeight = Convert.ToUInt32(Math.Max(g.MeasureString(Strings.General_Col1_CloudServiceStatus, font).Height * 1.20, _maxRowHeight));
            _maxRowHeight = Convert.ToUInt32(Math.Max(g.MeasureString(Strings.General_Col1_AggregateStatus, font).Height * 1.20, _maxRowHeight));
            _maxRowHeight = Convert.ToUInt32(Math.Max(g.MeasureString(Strings.General_Col1_ConnectionNumber, font).Height * 1.20, _maxRowHeight));
        }

        protected override void ComputeCol2(Graphics g, Font font)
        {
            _maxColumn2Width = Convert.ToUInt32(ComputeColWidth(Strings.General_AggregateCloudConnectionName, Convert.ToInt32(_maxColumn2Width)));
            _maxColumn2Width = Convert.ToUInt32(AddToColWidth("WW", Convert.ToInt32(_maxColumn2Width)));
        }

        public String Render(ServiceStatus status, ConnectorStateHelper stateHelper)
        {
            const int initialSize = 16384;
            var sb = new StringBuilder(Strings.General_HtmlDocumentMultiTable, initialSize);
            AppendNewTable(sb, String.Format("{0}", Strings.General_2ColTable_HtmlFragment));

            AppendNewBoldRow(sb, "Server", String.Format("{0} (port {1}) <a href='csm://changesettings' >[change]</a>", status.Host, status.Port));

            if (status.MonitorServiceStatus.HasValue)
            {
                RenderSystemStatus(sb, status);

                if (status.ConnectorServiceState.ConnectorState != null)
                {
                    var connectorState = status.ConnectorServiceState.ConnectorState;

                    DateTime upSince;
                    AppendNewBoldRow(sb, Strings.General_Col1_ConnectorUptime, Common.CreateElapsedSinceTimeString(DateTime.Now, connectorState.Uptime, out upSince));

                    RenderRestartInfo(sb, connectorState, upSince);
                    RenderSubsystemHealthMessages(sb, connectorState);
                    RenderConnectorUpdateStatus(sb, connectorState);
                    RenderBackOfficeUpdateStatus(sb, connectorState);
                    RenderCloudConnectivityStatus(sb, connectorState);

                    EndCurrentTable(sb); //Move to a 3 column table for status area
                    AppendNewTable(sb, Strings.General_3ColTable_HtmlFragment);
                    RenderAggregateConnectionState(sb, stateHelper);
                    RenderIntegratedConnectionStates(sb, stateHelper);
                    EndCurrentTable(sb);
                }
            }
            else
            {
                AppendNewBoldRow(sb, Strings.General_Col1_ConnectorMonitorStatus, "Offline");
            }

            ReplaceCommonFormatting(sb);

            return sb.ToString();
        }

        private void RenderSystemStatus(StringBuilder sb, ServiceStatus status)
        {
            string monitorStatus = string.Empty;
            string monitorImage = string.Empty;
            string connectorStatus = string.Empty;
            string connectorImage = string.Empty;

            switch (status.MonitorServiceStatus.Value)
            {
                case Sage.CRE.HostingFramework.Interfaces.Status.Ready:
                    monitorStatus = Strings.General_ConnectorMonitor_ReadyState;
                    monitorImage = _imageManager.OKBitmapFileName;
                    break;
                case Sage.CRE.HostingFramework.Interfaces.Status.Recycling:
                    monitorStatus = Strings.General_ConnectorMonitor_RecyclingState;
                    monitorImage = _imageManager.FixingBitmapFileName;
                    break;
                case Sage.CRE.HostingFramework.Interfaces.Status.SevicingRequest:
                    monitorStatus = Strings.General_ConnectorMonitor_ServicingRequestState;
                    monitorImage = _imageManager.RunningBitmapFileName;
                    break;
            }
            switch (status.ConnectorServiceStatus)
            {
                case ConnectorServiceConnectivityStatus.Connected:
                    connectorStatus = Strings.General_Connector_ConnectedState;
                    connectorImage = _imageManager.OKBitmapFileName;
                    break;
                case ConnectorServiceConnectivityStatus.ConnectivityError:
                    connectorStatus = Strings.General_Connector_ConnectivityErrorState;
                    connectorImage = _imageManager.SeriousBitmapFileName;
                    break;
                case ConnectorServiceConnectivityStatus.None:
                    connectorStatus = Strings.General_Connector_NoneState;
                    connectorImage = _imageManager.CriticalBitmapFileName;
                    break;
                case ConnectorServiceConnectivityStatus.ServiceNotReady:
                    connectorStatus = Strings.General_Connector_ServiceNotReadyState;
                    connectorImage = _imageManager.FixingBitmapFileName;
                    break;
                case ConnectorServiceConnectivityStatus.ServiceNotRegistered:
                    connectorStatus = Strings.General_Connector_ServiceNotRegisteredState;
                    connectorImage = _imageManager.CriticalBitmapFileName;
                    break;
                case ConnectorServiceConnectivityStatus.ServiceNotRunning:
                    connectorStatus = Strings.General_Connector_ServiceNotRunningState;
                    connectorImage = _imageManager.CriticalBitmapFileName;
                    break;
            }

            AppendNewBoldRowWithImage(sb
                , Strings.General_Col1_ConnectorMonitorStatus
                , monitorImage
                , String.Format("{0}", monitorStatus));


            if (status != null && status.ConnectorServiceState != null)
            {
                String monitorServiceFileVersion = status.ConnectorServiceState.MonitorServiceFileVersion;
                String myFileVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;

                if (monitorServiceFileVersion != myFileVersion)
                {
                    AppendNewBoldRowWithImage(sb
                        , String.Empty
                        , _imageManager.SeriousBitmapFileName
                        , String.Format("Monitor version ({0}) does not match Monitor service version ({1})", myFileVersion, monitorServiceFileVersion));
                }
            }

            AppendNewBoldRowWithImage(sb
                , Strings.General_Col1_ConnectorStatus
                , connectorImage
                , String.Format("{0}", connectorStatus));
        }

        private static void RenderRestartInfo(StringBuilder sb, ConnectorState connectorState, DateTime upSince)
        {
            switch (connectorState.RestartMode)
            {
                case RestartMode.None:
                    break;

                case RestartMode.RestartIntervalExceeded:
                    AppendNewBoldRow(sb, Strings.General_Col1_Restart, "Pending");
                    break;

                case RestartMode.RestartIntervalSpecified:
                    if (connectorState.MaxUptimeBeforeRestart.HasValue)
                    {
                        AppendNewBoldRow(sb, Strings.General_Col1_Restart, String.Format("after {0}", (upSince + connectorState.MaxUptimeBeforeRestart.Value).ToString("G", CultureInfo.CurrentCulture)));
                    }
                    break;
            }
        }

        private static uint SubsystemHealthIssueDisplayLimit
        {
            get
            {
                // This belongs to the main service rather than our monitor...
                //return ConnectorRegistryUtils.SubsystemHealthDisplayLimit;
                return 5;
            }
        }

        private static void RenderSubsystemHealthMessages(StringBuilder sb, ConnectorState connectorState)
        {
            Dictionary<Subsystem, uint> subSytemIssueCount = new Dictionary<Subsystem, uint>();

            // Iterate backwards to get the most current sub-system health first
            for (int index = connectorState.SubsystemHealthMessages.Length - 1;
                index >= 0;
                index--)
            {
                SubsystemHealthMessage message = connectorState.SubsystemHealthMessages[index];

                // Limit the number of displayed issues a sub system can raise
                if (!subSytemIssueCount.ContainsKey(message.Subsystem))
                {
                    subSytemIssueCount[message.Subsystem] = 0;
                }
                if (++subSytemIssueCount[message.Subsystem] <= SubsystemHealthIssueDisplayLimit)
                {
                    AppendNewBoldRow(
                        sb,
                        message.Subsystem.ToString(),
                        String.Format("{0} [{1}]",
                            message.UserFacingMessage,
                                message.TimestampUtc.ToLocalTime().ToString("G", CultureInfo.CurrentCulture)));
                }
            }
        }

        private void RenderConnectorUpdateStatus(StringBuilder sb, ConnectorState connectorState)
        {
            if (sb == null) return;
            if (connectorState == null) return;

            bool updateFileExists = false;
            if (connectorState.ConnectorUpdateInfo != null)
            {
                //check to see if the update file exists locally if so give option to apply the update.
                Uri updateFile = connectorState.ConnectorUpdateInfo.UpdateLinkUri;
                if (updateFile != null)
                {
                    string path = updateFile.LocalPath;
                    updateFileExists = System.IO.File.Exists(path);
                }
            }

            switch (connectorState.ConnectorUpdateStatus)
            {
                case ConnectorUpdateStatus.None:
                    AppendNewBoldRowWithImage(sb, Strings.General_Col1_ConnectorVersion, _imageManager.OKBitmapFileName,
                        String.Format(Strings.General_VersionIsCurrentMessageFormat, connectorState.ProductVersion));
                    break;
                case ConnectorUpdateStatus.UpdateAvailable:

                    AppendNewBoldRow(sb, Strings.General_Col1_ConnectorVersion, String.Format(Strings.General_VersionIsCurrentMessageFormat, connectorState.ProductVersion));

                    AppendNewBoldRowWithImage(sb, String.Empty, _imageManager.AlertBitmapFileName,
                        Strings.General_UpdateIsAvailableMessageFormatNoVersion);
                    
                    
                    //AppendNewBoldRowWithImage(sb, Strings.General_Col1_ConnectorVersion, _imageManager.AlertBitmapFileName,
                    //    String.Format(Strings.General_UpdateIsAvailableMessageFormatNoVersion,
                    //        connectorState.ConnectorUpdateInfo.PublicationDate.ToLocalTime().ToString("G", CultureInfo.CurrentCulture)));

                    //AppendNewBoldRow(sb, string.Empty,
                    //    string.Format(Strings.General_UpdateDescription, connectorState.ConnectorUpdateInfo.UpdateDescription));

                    //AppendNewBoldRow(sb, string.Empty,
                    //    String.Format(Strings.General_VersionIsUpdateMessageFormatAsLineItem, connectorState.ConnectorUpdateInfo.ProductVersion));

                    if (updateFileExists)
                    {
                        AppendNewBoldRow(sb, string.Empty, Strings.General_ApplyUpdateLink);
                    }
                    else
                    {
                        AppendNewBoldRow(sb, string.Empty, Strings.General_UpdateAvaibleOnConnectorMachine);
                    }
                    break;

                case ConnectorUpdateStatus.UpdateRequired:
                    AppendNewBoldRowWithImage(sb, Strings.General_Col1_ConnectorVersion, _imageManager.CriticalBitmapFileName,
                        String.Format(Strings.General_UpdateIsRequiredMessageFormatNoVersion,
                            connectorState.ConnectorUpdateInfo.PublicationDate.ToLocalTime().ToString("G", CultureInfo.CurrentCulture)));
                    AppendNewBoldRow(sb, string.Empty,
                        string.Format(Strings.General_UpdateDescription, connectorState.ConnectorUpdateInfo.UpdateDescription));

                    if (updateFileExists)
                    {
                        AppendNewBoldRow(sb, string.Empty, Strings.General_ApplyUpdateLink);
                    }
                    else
                    {
                        AppendNewBoldRow(sb, string.Empty, Strings.General_UpdateAvaibleOnConnectorMachine);
                    }

                    break;
            }
        }

        private void RenderBackOfficeUpdateStatus(StringBuilder sb, ConnectorState connectorState)
        {
            Boolean backOfficeUpdateRequired = false;
            if (connectorState != null && connectorState.IntegratedConnectionStates != null)
            {
                // this computes whether _any_ back office connection has indicated it is incompatible
                backOfficeUpdateRequired = connectorState.IntegratedConnectionStates.Where(x => x.BackOfficeConnectivityStatus == BackOfficeConnectivityStatus.Incompatible).Any();
            }

            // tell the user that one (or more) back office connections has indicated it is incompatible;  it would probably
            // be a better user experience to tell them specifically which 
            if (backOfficeUpdateRequired)
            {
                AppendNewBoldRowWithImage(sb
                    , Strings.General_Col1_BackofficeVersion
                    , _imageManager.CriticalBitmapFileName
                    , "Back office software is not current and must be updated");
            }
        }

        private void RenderCloudConnectivityStatus(StringBuilder sb, ConnectorState connectorState)
        {
            switch (connectorState.CloudConnectivityStatus)
            {
                case CloudConnectivityStatus.None:
                    AppendNewBoldRowWithImage(sb, Strings.General_Col1_CloudServiceStatus, _imageManager.BlankBitmapFileName, Strings.General_InternetConnectivityStatus_UnknownMessage);
                    break;
                case CloudConnectivityStatus.Blackout:
                    if (connectorState.TimeToBlackoutEnd.HasValue)
                    {
                        AppendNewBoldRowWithImage(sb, Strings.General_Col1_CloudServiceStatus, _imageManager.AlertBitmapFileName, String.Format(Strings.General_InternetConnectivityStatus_BlackoutMessageFormat, DateTime.Now + connectorState.TimeToBlackoutEnd.Value));
                    }
                    break;
                case CloudConnectivityStatus.LocalNetworkUnavailable:
                    AppendNewBoldRowWithImage(sb, Strings.General_Col1_CloudServiceStatus, _imageManager.CriticalBitmapFileName, Strings.General_InternetConnectivityStatus_LocalNetworkUnavailableMessage);
                    break;
                case CloudConnectivityStatus.InternetConnectionUnavailable:
                    AppendNewBoldRowWithImage(sb, Strings.General_Col1_CloudServiceStatus, _imageManager.CriticalBitmapFileName, Strings.General_InternetConnectivityStatus_InternetConnectionUnavailableMessage);
                    break;
                case CloudConnectivityStatus.CloudUnavailable:
                    AppendNewBoldRowWithImage(sb, Strings.General_Col1_CloudServiceStatus, _imageManager.CriticalBitmapFileName, Strings.General_InternetConnectivityStatus_CloudUnavailableMessage);
                    break;
                case CloudConnectivityStatus.GatewayServiceUnavailable:
                    AppendNewBoldRowWithImage(sb, Strings.General_Col1_CloudServiceStatus, _imageManager.CriticalBitmapFileName, Strings.General_InternetConnectivityStatus_GatewayServiceUnavailable);
                    break;
                case CloudConnectivityStatus.CommunicationFailure:
                    AppendNewBoldRowWithImage(sb, Strings.General_Col1_CloudServiceStatus, _imageManager.CriticalBitmapFileName, Strings.General_InternetConnectivityStatus_CommunicationFailure);
                    break;
                case CloudConnectivityStatus.Normal:
                    AppendNewBoldRowWithImage(sb, Strings.General_Col1_CloudServiceStatus, _imageManager.OKBitmapFileName, Strings.General_InternetConnectivityStatus_UpAndRunningMessage);
                    break;
            }
        }

        private void RenderAggregateConnectionState(StringBuilder sb, ConnectorStateHelper statusHelper)
        {
            if (statusHelper.Connections.Count > 1)
            {
                AppendNewBoldRow3Col(sb
                    , String.Format(Strings.General_Col1_AggregateStatus)
                    , "<u>Back Office Connection</u>"
                    , String.Format("{0} {1} {2} {3}"
                            , StatusImage(statusHelper.AggregateBackofficeStatus, Side.Left)
                            //Strings.General_ImageHtmlFragment.Replace("{BitmapSource}", _imageManager.ArrowFileName)
                             , StatusArrowImage(statusHelper.AggregateBackofficeStatus == statusHelper.AggregateTenantStatus, Side.Left)
                            , StatusImage(statusHelper.AggregateTenantStatus, Side.Right)
                            , String.Format("<u>{0}</u>", Strings.General_AggregateCloudConnectionName))
                    );
            }
        }

        private void RenderIntegratedConnectionStates(StringBuilder sb, ConnectorStateHelper statusHelper)
        {
            Int32 i = 1;
            foreach (Connection state in statusHelper.Connections)
            {
                String col2Text = String.Format("{0} ({1})", state.BackOfficeCompanyName, state.IntegratedConnectionState.BackOfficePluginInformation.ProductName);

                if (statusHelper.Connections.Count > 1)
                {
                    Int32 size = ComputeColWidth(col2Text + "WW", Convert.ToInt32(_maxColumn2Width));
                    _maxColumn2Width = Convert.ToUInt32(Math.Max(size, Convert.ToInt32(_maxColumn2Width)));

                    AppendNewBoldRow3Col(sb
                        , String.Format(Strings.General_Col1_ConnectionNumber, i++)
                        , col2Text
                        , String.Format("{0} {1} {2} {3}",
                                StatusImage(state.BackOfficeStatus, Side.Left),
                                StatusArrowImage(state.BackOfficeStatus == state.TenantStatus, Side.Left),
                                StatusImage(state.TenantStatus, Side.Right),
                                state.TenantName)
                        );
                }
                else
                {
                    AppendNewBoldRow(sb
                        , String.Format(Strings.General_Col1_ConnectionNoNumber, i++)
                        , String.Format("{0} {1} {2} {3} {4}",
                                col2Text,
                                StatusImage(state.BackOfficeStatus, Side.Left),
                                StatusArrowImage(state.BackOfficeStatus == state.TenantStatus, Side.Left),
                                StatusImage(state.TenantStatus, Side.Right),
                                state.TenantName)
                        );

                }
            }
        }

        private static void AppendNewBoldRow3Col(StringBuilder sb, String col1, String col2, String col3)
        {
            sb.Replace("{AdditionalHtml}", Strings.General_3ColRow_Bold_HtmlFragment);
            sb.Replace("{FieldName}", col1);
            sb.Replace("{ImageHtmlFragment}", String.Empty);
            sb.Replace("{FieldValue2}", col2);
            sb.Replace("{FieldValue3}", col3);
        }

        private static void AppendNewBoldRow(StringBuilder sb, String col1, String col2)
        {
            sb.Replace("{AdditionalHtml}", Strings.General_2ColRow_Bold_HtmlFragment);
            sb.Replace("{FieldName}", col1);
            sb.Replace("{ImageHtmlFragment}", String.Empty);
            sb.Replace("{FieldValue}", col2);
        }

        private static void AppendNewBoldRowWithImage(StringBuilder sb, String col1, String imageFileName, String col2)
        {
            sb.Replace("{AdditionalHtml}", Strings.General_2ColRow_Bold_HtmlFragment);
            sb.Replace("{FieldName}", col1);
            sb.Replace("{ImageHtmlFragment}", Strings.General_ImageHtmlFragment);
            sb.Replace("{BitmapSource}", imageFileName);
            sb.Replace("{FieldValue}", col2);
        }

    }
}
