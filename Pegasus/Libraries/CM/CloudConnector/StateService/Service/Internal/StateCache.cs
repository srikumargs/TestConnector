using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Sage.Connector.Common;
using Sage.Connector.Data;
using Sage.Connector.Logging;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.Connector.Utilities;
using Sage.Connector.Cloud.Integration.Interfaces;
using Sage.CRE.CloudConnector.Integration.Interfaces;

namespace Sage.Connector.StateService.Internal
{
    internal sealed class StateCache
    {

        private readonly String _fileVersion = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
  

// ReSharper disable ConvertToConstant.Local
        //TODO: Determine how we can do this in the future without using the registry
        private readonly String _productCode = "168CF325-99E5-449B-B7A1-4859B8F223A3"; //.Replace("{", "").Replace("}", "");  // force string to not have what looks like a String.Format() specification in it
        private readonly String _productName = "Sage Connector";
        
        //move to actual product version
        //private readonly String _productVersion = "1.5.225.1";
        private readonly String _productVersion = FileVersionInfo.GetVersionInfo(typeof(StateCache).Assembly.Location).ProductVersion;

        private readonly String _cloudInterfaceVersion = FileVersionInfo.GetVersionInfo(typeof(IAdminService).Assembly.Location).FileVersion;
        private readonly String _connectorBackOfficeIntegrationInterfaceVersion = FileVersionInfo.GetVersionInfo(typeof(IVersionInfo).Assembly.Location).FileVersion;

        private readonly String _connectorMinimumBackOfficeIntegrationInterfaceVersion = "1.4.107.2";
// ReSharper restore ConvertToConstant.Local
        private readonly Dictionary<Subsystem, List<SubsystemHealthMessage>> _subsystemHealthMessages = new Dictionary<Subsystem, List<SubsystemHealthMessage>>();
        private ConnectorUpdateStatus _connectorUpdateStatus;
        private UpdateInfo _connectorUpdateInfo;
        private TimeSpan? _maxUptimeBeforeRestart;
        private RestartMode _restartMode = RestartMode.None;
        private TimeSpan? _blackoutEnd;
        private DateTime? _startTimeUtc;
        private List<IntegratedConnectionState> _integratedConnectionStates = new List<IntegratedConnectionState>();
        private readonly Dictionary<String, Int32> _integratedConnectionStatesIndexes = new Dictionary<String, Int32>();
        private static Dictionary<String, String> _productVersionByAssemblyLocation = new Dictionary<String, String>();
        private static Dictionary<String, String> _interfaceVersionByAssemblyLocation = new Dictionary<String, String>();
        private static readonly Dictionary<String, String> _fileVersionByAssemblyLocation = new Dictionary<String, String>();
        private static readonly Dictionary<String, BackOfficePluginInformation> _backOfficePluginInformation = new Dictionary<String, BackOfficePluginInformation>();

        public ConnectorState GetConnectorState()
        {
            return new ConnectorState(
                _fileVersion,
                _productCode,
                _productName,
                _productVersion,
                _cloudInterfaceVersion,
                _connectorBackOfficeIntegrationInterfaceVersion,
                _connectorMinimumBackOfficeIntegrationInterfaceVersion,
                DateTime.UtcNow,
                GetUptime(),
                GetMaxUptimeBeforeRestart(),
                GetRestartMode(),
                GetTimeToBlackoutEnd(),
                _connectorUpdateStatus,
                _connectorUpdateInfo,
                GetSubsystemHealthMessages(),
                GetIntegratedConnectionStates());
        }

        public void Start()
        {
            _startTimeUtc = DateTime.UtcNow;

            // prime the state cache with values from the ConfigurationStore
            using (var lm = new SimpleTraceLogger())
            {
                PrimeIntegratedConnectionStatesFromStore(lm);
            }
        }

        private void PrimeIntegratedConnectionStatesFromStore(SimpleTraceLogger lm)
        {
            ActivityEntryRecord[] activityEntries = new ActivityEntryRecord[] { };
            try
            {
                var activityEntryRecordFactory = ActivityEntryRecordFactory.Create(lm);
                activityEntries = activityEntryRecordFactory.GetEntries();
            }
            catch { } // best attempt to get entries only

            PremiseConfigurationRecord[] pcrs = new PremiseConfigurationRecord[] { };
            try
            {
                var pcrFactory = PremiseConfigurationRecordFactory.Create(lm);
                pcrs = pcrFactory.GetEntries();
            }
            catch { } // best attempt to get entries only

            foreach (var pcr in pcrs)
            {
                var entriesForTenant = activityEntries.Where(x => (x.CloudTenantId == pcr.CloudTenantId) && (x.IsSystemRequest == false));
                var completedSuccessEntriesForTenant = entriesForTenant.Where(x => x.Status == ActivityEntryStatus.CompletedWithSuccessResponse);
                var completedErrorEntriesForTenant = entriesForTenant.Where(x => x.Status == ActivityEntryStatus.CompletedWithErrorResponse);
                var completedCancelEntriesForTenant = entriesForTenant.Where(x => x.Status == ActivityEntryStatus.CompletedWithCancelResponse);

                IntegrationEnabledStatus integrationEnabledStatus = IntegrationEnabledStatus.None;
                if (pcr.CloudConnectionEnabledToReceive)
                {
                    integrationEnabledStatus |= IntegrationEnabledStatus.CloudGetRequests;
                }

                if (pcr.CloudConnectionEnabledToSend)
                {
                    integrationEnabledStatus |= IntegrationEnabledStatus.CloudPutResponses;
                }

                if (pcr.BackOfficeConnectionEnabledToReceive)
                {
                    integrationEnabledStatus |= IntegrationEnabledStatus.BackOfficeProcessing;
                }

                UInt32 requestsReceivedCount = Convert.ToUInt32(entriesForTenant.Count());
                UInt32 completedSuccessCount = Convert.ToUInt32(completedSuccessEntriesForTenant.Count());
                UInt32 completedErrorCount = Convert.ToUInt32(completedErrorEntriesForTenant.Count() + completedCancelEntriesForTenant.Count());
                var pluginInformation = GetBackOfficePluginInformation(pcr.ConnectorPluginId, pcr.BackOfficeProductName);
                _integratedConnectionStates.Add(
                    new IntegratedConnectionState(pcr.CloudTenantId,
                        pcr.CloudCompanyName,
                        !String.IsNullOrEmpty(pcr.CloudCompanyUrl) ? new Uri(pcr.CloudCompanyUrl) : null,
                        integrationEnabledStatus,
                        TenantConnectivityStatus.None,
                        BackOfficeConnectivityStatus.None,
                        DateTime.MinValue,
                        DateTime.MinValue,
                        DateTime.MinValue,
                        requestsReceivedCount,
                        completedSuccessCount,
                        completedErrorCount,
                        Math.Max(0, requestsReceivedCount - (completedSuccessCount + completedErrorCount)),
                        pcr.BackOfficeCompanyName,
                        pluginInformation));
            }

            RebuildIntegratedConnectionStateIndexes();
        }

        public void Stop()
        { _startTimeUtc = null; }

        public void AppendSubsystemHealthMessage(SubsystemHealthMessage message)
        {
            List<SubsystemHealthMessage> list = null;
            if (!_subsystemHealthMessages.TryGetValue(message.Subsystem, out list))
            {
                list = new List<SubsystemHealthMessage>();
                list.Add(message);
                _subsystemHealthMessages.Add(message.Subsystem, list);
            }
            else
            {
                list.Add(message);
            }
        }

        public void ClearSubsystemHealthMessages(Subsystem subsystem)
        {
            List<SubsystemHealthMessage> list = null;
            if (_subsystemHealthMessages.TryGetValue(subsystem, out list))
            {
                _subsystemHealthMessages.Remove(subsystem);
            }
        }

        public void SetUpdateInfo(ConnectorUpdateStatus status, UpdateInfo info)
        {
            _connectorUpdateStatus = status;
            if (status != ConnectorUpdateStatus.None)
            {
                _connectorUpdateInfo = info;
            }
            else
            {
                _connectorUpdateInfo = null;
            }
        }

        private TimeSpan GetUptime()
        {
            var result = new TimeSpan(0);

            if (_startTimeUtc.HasValue)
            {
                result = DateTime.UtcNow.Subtract(_startTimeUtc.Value);
            }

            return result;
        }

        public void SetMaxUptimeBeforeRestart(TimeSpan? uptime)
        { _maxUptimeBeforeRestart = uptime; }

        private TimeSpan? GetMaxUptimeBeforeRestart()
        { return _maxUptimeBeforeRestart; }

        public void SetRestartMode(RestartMode mode)
        { _restartMode = mode; }

        private RestartMode GetRestartMode()
        { return _restartMode; }

        public void SetTimeToBlackoutEnd(TimeSpan? blackout)
        { _blackoutEnd = blackout; }

        private TimeSpan? GetTimeToBlackoutEnd()
        { return _blackoutEnd; }

        private SubsystemHealthMessage[] GetSubsystemHealthMessages()
        {
            List<SubsystemHealthMessage> result = new List<SubsystemHealthMessage>();

            foreach (var subsystemElement in _subsystemHealthMessages)
            {
                result.AddRange(subsystemElement.Value);
            }

            return result.OrderBy((x) => x.TimestampUtc).ToArray();
        }

        private void SetIntegratedConnectionState(IntegratedConnectionState updatedState)
        {
            if (_integratedConnectionStatesIndexes.ContainsKey(updatedState.TenantId))
            {
                // simple update
                _integratedConnectionStates[_integratedConnectionStatesIndexes[updatedState.TenantId]] = updatedState;
            }
            else
            {
                // insert, but be certain to keep records in the same order as they are in the ConfigurationStore
                // the reason why this matters is we want GetIntegratedConnectionStates() to be very quick
                // and we need it to be in the same order so the Monitor shows things the same way as the Connector UI

                // NOTE: purposefully going directly to the ConfigStore as opposed to using the ConfigurationService. 
                // This is in order to prevent blowing the stack when the ConfigurationService.GetConfigurations()
                // calls ClearSubsystemHealthIssues
                using (var lm = new SimpleTraceLogger())
                {
                    PremiseConfigurationRecordFactory factory = PremiseConfigurationRecordFactory.Create(lm);
                    var tenantIdsFromConfigStore = factory.GetEntries().Select(x => x.CloudTenantId);

                    var updatedStateAdded = false;
                    var newList = new List<IntegratedConnectionState>();
                    foreach (var tenantId in tenantIdsFromConfigStore)
                    {
                        // if we already have some state for this tenant, then add it in to the newList
                        if (_integratedConnectionStatesIndexes.ContainsKey(tenantId))
                        {
                            newList.Add(_integratedConnectionStates[_integratedConnectionStatesIndexes[tenantId]]);
                        }
                        else
                        {
                            // if don't yet have state for this tenant, and it is the one we are are trying to update, then append it now
                            if (tenantId == updatedState.TenantId)
                            {
                                newList.Add(updatedState);
                                updatedStateAdded = true;
                            }
                            else
                            {
                                // otherwise skip a tenant for which we have no state for yet
                            }
                        }
                    }

                    // if we haven't yet added the state we are trying to update (because it is brand new), then do it now
                    if (!updatedStateAdded)
                    {
                        // don't add the state unless it is a real the PCR!
                        //newList.Add(updatedState);
                    }

                    // replace the  _integratedConnectionStates now with the newList
                    _integratedConnectionStates = newList;

                    // rebuild the indexes for faster lookups
                    RebuildIntegratedConnectionStateIndexes();
                }
            }
        }

        private void RebuildIntegratedConnectionStateIndexes()
        {
            Int32 i = 0;
            _integratedConnectionStatesIndexes.Clear();
            foreach (var state in _integratedConnectionStates)
            {
                _integratedConnectionStatesIndexes[state.TenantId] = i++;
            }
        }

        private IntegratedConnectionState GetIntegratedConnectionState(string tenantId)
        {
            IntegratedConnectionState result = null;

            if (_integratedConnectionStatesIndexes.ContainsKey(tenantId))
            {
                result = _integratedConnectionStates[_integratedConnectionStatesIndexes[tenantId]];
            }
            else
            {
                result = new IntegratedConnectionState(
                    tenantId,
                    String.Empty,
                    null,
                    IntegrationEnabledStatus.None,
                    TenantConnectivityStatus.None,
                    BackOfficeConnectivityStatus.None,
                    DateTime.MinValue,
                    DateTime.MinValue,
                    DateTime.MinValue,
                    0,
                    0,
                    0,
                    0,
                    String.Empty, // back office company name
                    null); // plugin product version
                SetIntegratedConnectionState(result);
            }

            return result;
        }

        public void UpdateIntegratedConnectionState(String tenantId, String tenantName, String backOfficeCompanyName, Uri tenantUri, IntegrationEnabledStatus integrationEnabledStatus, BackOfficePluginInformation backOfficePluginInformation)
        {
            var oldState = GetIntegratedConnectionState(tenantId);
            var changes = new List<PropertyTuple>();
            changes.Add(new PropertyTuple(oldState.PropertyInfo(x => x.TenantName), tenantName));
            changes.Add(new PropertyTuple(oldState.PropertyInfo(x => x.BackOfficeCompanyName), backOfficeCompanyName));
            changes.Add(new PropertyTuple(oldState.PropertyInfo(x => x.TenantUri), tenantUri));
            changes.Add(new PropertyTuple(oldState.PropertyInfo(x => x.IntegrationEnabledStatus), integrationEnabledStatus));
            changes.Add(new PropertyTuple(oldState.PropertyInfo(x => x.BackOfficePluginInformation), backOfficePluginInformation));
            var newState = new IntegratedConnectionState(oldState, changes);
            SetIntegratedConnectionState(newState);
        }

        private IntegratedConnectionState[] GetIntegratedConnectionStates()
        { return _integratedConnectionStates.ToArray(); }

        public void RemoveIntegratedConnectionState(string tenantId)
        {
            if (_integratedConnectionStatesIndexes.ContainsKey(tenantId))
            {
                _integratedConnectionStates.RemoveAt(_integratedConnectionStatesIndexes[tenantId]);
                RebuildIntegratedConnectionStateIndexes();
            }
        }

        public void UpdateLastAttemptedCommunicationWithCloud(string tenantId, DateTime commTime)
        {
            var oldState = GetIntegratedConnectionState(tenantId);
            if ((null == oldState.LastAttemptedCommunicationWithCloud) ||
                (commTime > oldState.LastAttemptedCommunicationWithCloud))
            {
                var changes = new List<PropertyTuple>();
                changes.Add(new PropertyTuple(oldState.PropertyInfo(x => x.LastAttemptedCommunicationWithCloud), commTime));
                var newState = new IntegratedConnectionState(oldState, changes);
                SetIntegratedConnectionState(newState);
            }
        }

        public void UpdateLastSuccessfulCommunicationWithCloud(string tenantId, DateTime commTime)
        {
            var oldState = GetIntegratedConnectionState(tenantId);
            if ((null == oldState.LastSuccessfulCommunicationWithCloud) ||
                (commTime > oldState.LastSuccessfulCommunicationWithCloud))
            {
                var changes = new List<PropertyTuple>();
                changes.Add(new PropertyTuple(oldState.PropertyInfo(x => x.LastSuccessfulCommunicationWithCloud), commTime));
                var newState = new IntegratedConnectionState(oldState, changes);
                SetIntegratedConnectionState(newState);
            }
        }

        public void UpdateNextScheduledCommunicationWithCloud(string tenantId, DateTime commTime)
        {
            var oldState = GetIntegratedConnectionState(tenantId);
            if ((null == oldState.NextScheduledCommunicationWithCloud) ||
                oldState.NextScheduledCommunicationWithCloud == DateTime.MinValue ||
                oldState.NextScheduledCommunicationWithCloud < DateTime.UtcNow ||
                (commTime < oldState.NextScheduledCommunicationWithCloud))
            {
                var changes = new List<PropertyTuple>();
                changes.Add(new PropertyTuple(oldState.PropertyInfo(x => x.NextScheduledCommunicationWithCloud), commTime));
                var newState = new IntegratedConnectionState(oldState, changes);
                SetIntegratedConnectionState(newState);
            }
        }

        public void IncrementRequestsReceivedCount(string tenantId, uint count)
        {
            var oldState = GetIntegratedConnectionState(tenantId);
            var changes = new List<PropertyTuple>();
            changes.Add(new PropertyTuple(oldState.PropertyInfo(x => x.RequestsReceivedCount), oldState.RequestsReceivedCount + count));
            var newState = new IntegratedConnectionState(oldState, changes);
            SetIntegratedConnectionState(newState);
        }

        public void UpdateBackOfficeConnectivityStatus(string tenantId, BackOfficeConnectivityStatus status)
        {
            var oldState = GetIntegratedConnectionState(tenantId);
            var changes = new List<PropertyTuple>();
            changes.Add(new PropertyTuple(oldState.PropertyInfo(x => x.BackOfficeConnectivityStatus), status));
            var newState = new IntegratedConnectionState(oldState, changes);
            SetIntegratedConnectionState(newState);
        }

        public void IncrementNonErrorResponsesSentCount(string tenantId, uint count)
        {
            var oldState = GetIntegratedConnectionState(tenantId);
            var changes = new List<PropertyTuple>();
            changes.Add(new PropertyTuple(oldState.PropertyInfo(x => x.NonErrorResponsesSentCount), oldState.NonErrorResponsesSentCount + count));
            var newState = new IntegratedConnectionState(oldState, changes);
            SetIntegratedConnectionState(newState);
        }

        public void IncrementErrorResponsesSentCount(string tenantId, uint count)
        {
            var oldState = GetIntegratedConnectionState(tenantId);
            var changes = new List<PropertyTuple>();
            changes.Add(new PropertyTuple(oldState.PropertyInfo(x => x.ErrorResponsesSentCount), oldState.ErrorResponsesSentCount + count));
            var newState = new IntegratedConnectionState(oldState, changes);
            SetIntegratedConnectionState(newState);
        }

        public void AdjustRequestsInProgressCount(string tenantId, int adjustment)
        {
            if (adjustment != 0)
            {
                var oldState = GetIntegratedConnectionState(tenantId);

                Int64 newCount = oldState.RequestsInProgressCount + adjustment;
                newCount = Math.Max(0, newCount);

                var changes = new List<PropertyTuple>();
                changes.Add(new PropertyTuple(oldState.PropertyInfo(x => x.RequestsInProgressCount), System.Convert.ToUInt32(newCount)));
                var newState = new IntegratedConnectionState(oldState, changes);
                SetIntegratedConnectionState(newState);
            }
        }

        public BackOfficePluginInformation GetBackOfficePluginInformation(String pluginId, String backOfficeProductName)
        {
            BackOfficePluginInformation result = null;

            if (!_backOfficePluginInformation.ContainsKey(pluginId))
            {
                //TODO: this path should not be called in theory. But its a timing thing. 
                //We now populate this data when the first discovery request is made.
                //This will happen as part of the auto update process as it needs this data to figure out if 
                //we need to check for specific plugin updates as part of the service start update checks.
                //Current code in the state service will initialize the cacheed values with all of the installed plugins
                var backOfficeInterfaceVersion = GetBackOfficeInterfaceVersion(pluginId);

                //TODO KMS:  with the new implementation, there could be more than one dll supporting the multiple features
                //TODO KMS:  therefore, we either pick one, like the main plugin dll (one that supports GetPluginId)
                //TODO KMS:  However, it would be a call into the plugin to get the information or it could be
               
                //var backOfficeProductPluginFileVersion = GetFileVersion(backOfficeProductPluginPath);
                var backOfficeProductVersion = GetBackOfficeProductVersion(pluginId);
                _backOfficePluginInformation.Add(pluginId,
                    new BackOfficePluginInformation(
                        pluginId, 
                        backOfficeProductName, 
                        backOfficeInterfaceVersion,
                        "Not Yet Populated: PluginFileVersion", 
                        backOfficeProductVersion,
                        "Not Yet Populated: autoUpdateProductId",
                        "Not Yet Populated: autoUpdateProductVersion",
                        "Not Yet Populated: autoUpdateComponentBaseName",
                        false,
                        "Not Yet Populated: platform"
                        ));
            }

            result = _backOfficePluginInformation[pluginId];

            return result;
        }

        /// <summary>
        /// Sets the back office plugin information.
        /// </summary>
        /// <param name="pluginId">The plugin identifier.</param>
        /// <param name="backOfficePluginInformation">The back office plugin information.</param>
        public void SetBackOfficePluginInformation(string pluginId, BackOfficePluginInformation backOfficePluginInformation)
        {

            if (pluginId != null && backOfficePluginInformation != null)
            {
                //add or update as needed
                _backOfficePluginInformation[pluginId] = backOfficePluginInformation;
            }
        }

        public void UpdateTenantConnectivityStatus(String tenantId, TenantConnectivityStatus status)
        {
            var oldState = GetIntegratedConnectionState(tenantId);

            var changes = new List<PropertyTuple>();
            changes.Add(new PropertyTuple(oldState.PropertyInfo(x => x.TenantConnectivityStatus), status));
            var newState = new IntegratedConnectionState(oldState, changes);
            SetIntegratedConnectionState(newState);
        }

        private static String GetBackOfficeProductVersion(String pluginId)
        {

            if (String.IsNullOrEmpty(pluginId))
                return String.Empty;

// ReSharper disable once RedundantAssignment
            var result = String.Empty;
            


            //using (var lm = new LogManager())
            //{
            //    //call into the process execution manager for that domain mediation request. 

                //if ((response != null) && (response.Succeeded) && (!String.IsNullOrEmpty(response.ProductVersion)))
                //{
                //    result = response.ProductVersion;
                //    _productVersionByAssemblyLocation.Add(backOfficeProductPluginPath, result);
                //}
                //else
                //{
                //    string sMsg = "The state service was unable to retrieve back office product version";
                //    if (response != null)
                //    {
                //        if (response.Errors.Any())
                //        {
                //            sMsg += ": ";
                //            sMsg += response.Errors.First().RawErrorMessage;
                //        }
                //    }
                //    lm.WriteWarning(
                //        null,
                //        sMsg,
                //        null);
                //}
            //}

            return result;
        }

        private static String GetBackOfficeInterfaceVersion(String backOfficePluginId)
        {
            var result = String.Empty;

            result = "TODO: Interface Versions";
            //TODO: JSB Revise
            //if (!String.IsNullOrEmpty(backOfficeProductPluginPath) && !String.IsNullOrEmpty(backOfficeProductPluginShimExePath))
            //{
            //    // cache the product version in a static dictionary:  careful, this means the cache _could_ get
            //    // stale if the plugin was updated without restarting the HostingFx service
            //    if (!_interfaceVersionByAssemblyLocation.TryGetValue(backOfficeProductPluginPath, out result))
            //    {
            //        using (var lm = new LogManager())
            //        {
            //            var shim = new VersionInfoPluginShim(backOfficeProductPluginPath, backOfficeProductPluginShimExePath, lm, null);
            //            var response = shim.GetInterfaceVersion();
            //            if ((response != null) && (response.Succeeded) && (!String.IsNullOrEmpty(response.InterfaceVersion)))
            //            {
            //                result = response.InterfaceVersion;
            //                _interfaceVersionByAssemblyLocation.Add(backOfficeProductPluginPath, result);
            //            }
            //            else
            //            {
            //                string sMsg = "The state service was unable to retrieve back office interface version";
            //                if (response != null)
            //                {
            //                    if (response.Errors.Any())
            //                    {
            //                        sMsg += ": ";
            //                        sMsg += response.Errors.First().RawErrorMessage;
            //                    }
            //                }
            //                lm.WriteWarning(
            //                    null,
            //                    sMsg,
            //                    null);
            //            }
            //        }
            //    }
            //}

            return result;
        }

        private static String GetFileVersion(String filePath)
        {
            String result = String.Empty;

            if (File.Exists(filePath))
            {
                // cache the file version in a static dictionary:  careful, this means the cache _could_ get
                // stale if the plugin was updated without restarting the HostingFx service
                if (!_fileVersionByAssemblyLocation.TryGetValue(filePath, out result))
                {
                    if (File.Exists(filePath))
                    {
                        FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(filePath);
                        result = fvi.FileVersion;
                    }

                    _fileVersionByAssemblyLocation.Add(filePath, result);
                }
            }

            return result;
        }

    }
}
