using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using Newtonsoft.Json;
using Sage.Connector.AutoUpdate;
using Sage.Connector.Common;
using Sage.Connector.Common.DataContracts;
using Sage.Connector.Configuration.Contracts.Data;
using Sage.Connector.Configuration.Contracts.Data.Responses;
using Sage.Connector.ConnectorServiceCommon;
using Sage.Connector.Data;
using Sage.Connector.Documents;
using Sage.Connector.Logging;
using Sage.Connector.MessagingService.Proxy;
using Sage.Connector.ProcessExecution.Events;
using Sage.Connector.ProcessExecution.RequestActivator;
using Sage.Connector.StateService.Interfaces;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.Connector.StateService.Internal;
using Sage.Connector.StateService.JsonConverters;
using Sage.Connector.Utilities;
using Sage.CRE.HostingFramework.Interfaces;
using Status = Sage.Connector.DomainContracts.Responses.Status;

namespace Sage.Connector.StateService
{
    /// <summary>
    /// A configuration service for storing and retrieving premise-cloud connectivity attributes
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
        ConcurrencyMode = ConcurrencyMode.Reentrant, // must be reentrant because StateService calls MessagingService, which could call StateService
        ConfigurationName = "StateService")]
    [SingletonServiceControl(StartMethod = "Startup", StopMethod = "Shutdown")]
    public sealed class StateService : IStateService, IDatabaseRepairerService, IBackOfficeValidationService, ITenantValidationService, IFeatureService
    {


        #region IStateService Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ConnectorState GetConnectorState()
        {
            ConnectorState result = null;

            try
            {
                lock (_syncObject)
                {
                    result = _stateCache.GetConnectorState();
                }
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pluginId"></param>
        /// <param name="backOfficeProductName"></param>
        public BackOfficePluginInformation GetBackOfficePluginInformation(String pluginId, String backOfficeProductName)
        {
            BackOfficePluginInformation result = null;

            try
            {
                lock (_syncObject)
                {
                    result = _stateCache.GetBackOfficePluginInformation(pluginId, backOfficeProductName);
                }
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }
            }

            return result;
        }


        /// <summary>
        /// Retrieves the recently created request entries as well as all currently in-progress ones
        /// </summary>
        /// <param name="recentEntriesThreshold"></param>
        /// <returns></returns>
        public RequestState[] GetRecentAndInProgressRequestsState(TimeSpan recentEntriesThreshold)
        {
            RequestState[] result = null;

            try
            {
                using (var logger = new LogManager())
                {
                    var factory = ActivityEntryRecordFactory.Create(logger);
                    result = factory.GetRecentAndInProgressEntriesAsRequestState(recentEntriesThreshold);
                }
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }
            }
            return result;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        public void RaiseSubsystemHealthIssue(SubsystemHealthMessage message)
        {
            try
            {
                lock (_syncObject)
                {
                    _stateCache.AppendSubsystemHealthMessage(message);
                }
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subsystem"></param>
        public void ClearSubsystemHealthIssues(Subsystem subsystem)
        {
            try
            {
                lock (_syncObject)
                {
                    _stateCache.ClearSubsystemHealthMessages(subsystem);
                }
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxUptime"></param>
        public void SetMaxUptimeBeforeRestart(TimeSpan? maxUptime)
        {
            try
            {
                lock (_syncObject)
                {
                    _stateCache.SetMaxUptimeBeforeRestart(maxUptime);
                }
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mode"></param>
        public void SetRestartMode(RestartMode mode)
        {
            try
            {
                lock (_syncObject)
                {
                    _stateCache.SetRestartMode(mode);
                }
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="blackoutEnd"></param>
        public void SetTimeToBlackoutEnd(TimeSpan? blackoutEnd)
        {
            try
            {
                lock (_syncObject)
                {
                    _stateCache.SetTimeToBlackoutEnd(blackoutEnd);
                }
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        /// <param name="info"></param>
        public void SetUpdateInfo(ConnectorUpdateStatus status, UpdateInfo info)
        {
            try
            {
                lock (_syncObject)
                {
                    _stateCache.SetUpdateInfo(status, info);
                }
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="tenantName"></param>
        /// <param name="backOfficeCompanyName"></param>
        /// <param name="tenantUri"></param>
        /// <param name="integrationEnabledStatus"></param>
        /// <param name="backOfficePluginInformation"></param>
        public void UpdateIntegratedConnectionState(String tenantId, String tenantName, String backOfficeCompanyName, Uri tenantUri, IntegrationEnabledStatus integrationEnabledStatus, BackOfficePluginInformation backOfficePluginInformation)
        {
            try
            {
                lock (_syncObject)
                {
                    _stateCache.UpdateIntegratedConnectionState(tenantId, tenantName, backOfficeCompanyName, tenantUri, integrationEnabledStatus, backOfficePluginInformation);
                }
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }
            }
        }

        /// <summary>
        /// Removes the tenant state for the given tenant identifier
        /// </summary>
        /// <param name="tenantId"></param>
        public void RemoveIntegratedConnectionState(string tenantId)
        {
            try
            {
                lock (_syncObject)
                {
                    _stateCache.RemoveIntegratedConnectionState(tenantId);
                }
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }
            }
        }

        /// <summary>
        /// Updates the last attempted communication with the cloud for the tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="commTime"></param>
        public void UpdateLastAttemptedCommunicationWithCloud(string tenantId, DateTime commTime)
        {
            try
            {
                lock (_syncObject)
                {
                    _stateCache.UpdateLastAttemptedCommunicationWithCloud(tenantId, commTime);
                }
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }
            }
        }

        /// <summary>
        /// Update the last successful communication with the cloud for the tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="commTime"></param>
        public void UpdateLastSuccessfulCommunicationWithCloud(string tenantId, DateTime commTime)
        {
            try
            {
                lock (_syncObject)
                {
                    _stateCache.UpdateLastSuccessfulCommunicationWithCloud(tenantId, commTime);
                }
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }
            }
        }

        /// <summary>
        /// Updates the next scheduled communication with the cloud for the tenant
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="commTime"></param>
        public void UpdateNextScheduledCommunicationWithCloud(string tenantId, DateTime commTime)
        {
            try
            {
                lock (_syncObject)
                {
                    _stateCache.UpdateNextScheduledCommunicationWithCloud(tenantId, commTime);
                }
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }
            }
        }

        /// <summary>
        /// Increments the requests received count
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="count"></param>
        public void IncrementRequestsReceivedCount(string tenantId, uint count)
        {
            try
            {
                lock (_syncObject)
                {
                    _stateCache.IncrementRequestsReceivedCount(tenantId, count);
                }
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }
            }
        }

        /// <summary>
        /// Increments the non-error responses sent count
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="count"></param>
        public void IncrementNonErrorResponsesSentCount(string tenantId, uint count)
        {
            try
            {
                lock (_syncObject)
                {
                    _stateCache.IncrementNonErrorResponsesSentCount(tenantId, count);
                }
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }
            }
        }

        /// <summary>
        /// Increments the error responses sent count
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="count"></param>
        public void IncrementErrorResponsesSentCount(string tenantId, uint count)
        {
            try
            {
                lock (_syncObject)
                {
                    _stateCache.IncrementErrorResponsesSentCount(tenantId, count);
                }
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }
            }
        }

        /// <summary>
        /// Adjusts the number of requests in process
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="count"></param>
        public void AdjustRequestsInProgressCount(string tenantId, int count)
        {
            try
            {
                lock (_syncObject)
                {
                    _stateCache.AdjustRequestsInProgressCount(tenantId, count);
                }
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="status"></param>
        public void UpdateTenantConnectivityStatus(String tenantId, TenantConnectivityStatus status)
        {
            try
            {
                lock (_syncObject)
                {
                    _stateCache.UpdateTenantConnectivityStatus(tenantId, status);
                }
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }
            }
        }


        #endregion

        #region IBackOfficeValidationService

        /// <summary>
        ///
        /// </summary>
        /// <param name="backOfficeId"></param>
        /// <param name="credentials"></param>
        /// <returns></returns>

        public ValidateBackOfficeConnectionResponse ValidateBackOfficeConnection(
        String backOfficeId,
        IDictionary<string, string> credentials)
        {
            string payload = JsonConvert.SerializeObject(credentials);

            return ValidateBackOfficeConnectionCredentialsAsString(backOfficeId, payload);
        }

        /// <summary>
        ///  TODO KMS: Modify ValidateBackOfficeConnectionResponse
        ///  TODO KMS: to use the response status and diagnoses  
        /// </summary>
        /// <param name="backOfficeId"></param>
        /// <param name="connectionCredentials"></param>
        /// <returns></returns>
        public ValidateBackOfficeConnectionResponse ValidateBackOfficeConnectionCredentialsAsString(
            string backOfficeId,
            string connectionCredentials
        )
        {
            //TODO JSB: Revisit this and the response details.
            BackOfficeConnectivityStatus bocs = BackOfficeConnectivityStatus.None;
            ValidateBackOfficeConnectionResponse result = new ValidateBackOfficeConnectionResponse(
                bocs, null, null, null, new String[] { }, new String[] { });

            try
            {
                List<String> userFacingMessages = new List<String>();
                List<String> rawErrorMessages = new List<String>();

                Guid requestId = Guid.NewGuid();
                var backOfficeConfig = new BackOfficeCompanyConfigurationObject();
                backOfficeConfig.BackOfficeId = backOfficeId;

                String responsePayload = MakeBackOfficeRequest("ValidateCompanyConnectionCredentials", requestId, null, backOfficeConfig, connectionCredentials);

                var validationInfo = DeserializeJson<ValidateCompanyConnectionCredentialsResponse>(responsePayload);

                if (validationInfo.Status == Status.Success)
                {
                    bocs = BackOfficeConnectivityStatus.Normal;
                }
                if (validationInfo.Diagnoses != null)
                {
                    foreach (var diagnosis in validationInfo.Diagnoses)
                    {
                        userFacingMessages.Add(diagnosis.UserFacingMessage);
                        rawErrorMessages.Add(diagnosis.RawMessage);
                    }
                }

                //using (var logger = new LogManager())
                //{
                //    // Pass in an empty tenant ID because we don't want to update the state service here
                //    // We want the caller to do that
                //    var v = new VerifyCredentialsPluginShim(pluginAssemblyPath, pluginShimExePath, logger, null);
                //    var response = v.ValidateConnection(
                //        backOfficeUserName, 
                //        backOfficeUserPassword.ToSecureString(), 
                //        backOfficeConnectionInformation,
                //        out bocs);

                //    if (response != null)
                //    {
                //        if (response.Succeeded)
                //        {
                //            if ((null != response.ValidationMessages) &&
                //                (response.ValidationMessages.Count() > 0))
                //            {
                //                userFacingMessages.AddRange(response.ValidationMessages);
                //            }
                //        }

                //        if (null != response.Errors)
                //        {
                //            foreach (PremiseContracts.ErrorInformation ei in response.Errors)
                //            {
                //                if (!string.IsNullOrEmpty(ei.UserFacingErrorMessage))
                //                {
                //                    userFacingMessages.Add(ei.UserFacingErrorMessage);
                //                }

                //                if (!string.IsNullOrEmpty(ei.RawErrorMessage))
                //                {
                //                    rawErrorMessages.Add(ei.RawErrorMessage);
                //                }
                //            }
                //        }
                //    }

                result = new ValidateBackOfficeConnectionResponse(
                    bocs,
                    validationInfo.Credentials,
                    validationInfo.CompanyNameForDisplay,
                    validationInfo.CompanyUnqiueIdentifier,
                    userFacingMessages,
                    rawErrorMessages);
                //}
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="status"></param>
        public void UpdateBackOfficeConnectivityStatus(String tenantId, BackOfficeConnectivityStatus status)
        {
            try
            {
                lock (_syncObject)
                {
                    _stateCache.UpdateBackOfficeConnectivityStatus(tenantId, status);
                }
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public BackOfficePluginsResponse GetBackOfficePlugins()
        {
            BackOfficePluginsResponse result = new BackOfficePluginsResponse(new BackOfficePlugin[] { }, new String[] { }, new String[] { });

            try
            {
                List<String> userFacingMessages = new List<String>();
                List<String> rawErrorMessages = new List<String>();

                Guid requestId = Guid.NewGuid();
                var backOfficeConfig = new BackOfficeCompanyConfigurationObject();
                //TODO: JSB review if we want to be more explicit for back office config and cases where its not relevant.
                //backOfficeConfig.BackOfficeId = backOfficeId; No back office ID to set as we do not have a back office yet.
                
                IList<BackOfficePlugin> boConns = new List<BackOfficePlugin>();

                //String responsePayload = MakeBackOfficeRequest("GetInstalledBackOfficePluginInformationCollection", requestId, backOfficeConfig);
                String responsePayload = MakeBackOfficeRequest("GetPluginInformationCollection", requestId, backOfficeConfig);
                List<Dictionary<String, String>> backOfficePlugins = DeserializeJson<List<Dictionary<String, String>>>(responsePayload);

                foreach (var backOfficePlugin in backOfficePlugins)
                {
                    if (backOfficePlugin == null)
                    {
                        //todo: log something useful here if possible. Back office plugin info is not there. 
                        //Example an early sage100Erp running on a system without a sage 100 erp...
                        continue;
                    }
                    //TODO: extract if this stays?
                    ApplicationSecurityMode applicationSecurityModeValue;
                    Enum.TryParse(ValueOrEmpty(backOfficePlugin, "ApplicationSecurityMode"), true, out applicationSecurityModeValue);
                    if (applicationSecurityModeValue == ApplicationSecurityMode.None)
                    {
                        applicationSecurityModeValue =
                            ApplicationSecurityMode.UsesGlobalApplicationSecurityAdministrator;
                    }

                    Boolean loginAdministratorAccountNameReadOnlyValue;
                    Boolean.TryParse(ValueOrEmpty(backOfficePlugin, "LoginAdministratorAccountNameReadOnly"), out loginAdministratorAccountNameReadOnlyValue);

                    string backOfficeId = ValueOrEmpty(backOfficePlugin, "BackOfficeId");
                        
                        
                    string auProductId = ValueOrEmpty(backOfficePlugin, "AutoUpdateProductId");
                    auProductId = (string.IsNullOrWhiteSpace(auProductId)? ValueOrEmpty(backOfficePlugin, "BackOfficePluginProductId") : auProductId);

                    string auProductVersion = ValueOrEmpty(backOfficePlugin, "AutoUpdateProductVersion");
                    string auComponentBaseName = ValueOrEmpty(backOfficePlugin, "AutoUpdateComponentBaseName");

                    string pluginVersion = ValueOrEmpty(backOfficePlugin, "PluginVersion");

                    Boolean runAsUserIsRequired;
                    Boolean.TryParse(ValueOrEmpty(backOfficePlugin, "RunAsUserIsRequired"), out runAsUserIsRequired);

                    //add missing trailing period on auComponentBaseName if needed.
                    if (!String.IsNullOrEmpty(auComponentBaseName))
                    {
                        const string separator = ".";
                        if (!auComponentBaseName.EndsWith(separator))
                        {
                            auComponentBaseName += separator;
                        }
                    }

                    AutoUpdateManager.AdjustBackOfficePluginAutoUpdateValues(backOfficeId, ref auProductId, ref auProductVersion, ref auComponentBaseName);
                        
                    //TODO: JSB - Consider adding validation for incoming values. Turns out under some error cases
                    // the back office can return a valid structure but return null for the value of the properties.
                    // it would be useful to log that for correction.

                    //TODO JSB - Revise BackOfficePlugin in light of property bag system
                    var boPlugin = new BackOfficePlugin(
                                        backOfficeId,
                                        ValueOrEmpty(backOfficePlugin, "BackOfficeName"),
                                        ValueOrEmpty(backOfficePlugin, "Platform"),
                                        ValueOrEmpty(backOfficePlugin, "HelpUri"),
                                        ValueOrEmpty(backOfficePlugin, "BackOfficeVersion"),
                                        ValueOrEmpty(backOfficePlugin, "LoginAdministratorTerm"),
                                        applicationSecurityModeValue,
                                        auProductId,
                                        auProductVersion,
                                        auComponentBaseName,
                                        pluginVersion,
                                        runAsUserIsRequired
                                        );
                    boConns.Add(boPlugin);



                    //For reference until last dregs of old system are expunged
                    //For an experience more in keeping with Sage 100 Contractor values used to
                    //pi.BackOfficeId = "Sage100Contractor";
                    //pi.BackOfficeName = "Sage 100 Contractor";
                    //pi.ApplicationSecurityMode = "UsesPerCompanyAdministrator";
                    //pi.LoginAdministratorTerm = "Supervisor";
                    //pi.LoginAdministratorAccountNamePrefill = "Supervisor";
                    //pi.LoginAdministratorAccountNameReadOnly = true;
                    //pi.HelpUri = "http://help.sageconstructionanywhere.com/2-0/sage100contractor/help/";
                }

                UpdateStateCacheForBackOfficePlugins(boConns);

                IEnumerable<BackOfficePlugin> backofficePlugins = boConns;

                //using (var logger = new LogManager())
                //{
                //    var v = new VerifyCredentialsPluginShim(pluginAssemblyPath, pluginShimExePath, logger, null);
                //    var response = v.ValidConnectionsForCredentials(backOfficeUserName, backOfficeUserPassword.ToSecureString());
                //    if (response != null && (response.Succeeded) && (response.Connections != null))
                //    {
                //        backofficeConnections = response.Connections.Select(x => new BackOfficeConnection(x.ConnectionInformation, x.DisplayableConnectionInformation, x.Name));
                //    }
                //    else
                //    {
                //        if (null != response.Errors)
                //        {
                //            foreach (PremiseContracts.ErrorInformation ei in response.Errors)
                //            {
                //                if (!string.IsNullOrEmpty(ei.UserFacingErrorMessage))
                //                {
                //                    userFacingMessages.Add(ei.UserFacingErrorMessage);
                //                }

                //                if (!string.IsNullOrEmpty(ei.RawErrorMessage))
                //                {
                //                    rawErrorMessages.Add(ei.RawErrorMessage);
                //                }
                //            }
                //        }
                //    }

                result = new BackOfficePluginsResponse(backofficePlugins, userFacingMessages, rawErrorMessages);
                //}
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }
            }

            return result;
        }

        private void UpdateStateCacheForBackOfficePlugins(IList<BackOfficePlugin> backOfficePlugins)
        {

            foreach (BackOfficePlugin p in backOfficePlugins)
            {
                var pi = new BackOfficePluginInformation(
                    p.PluginId,
                    p.BackofficeProductName,
                    "TODO: integration interface version",
                    p.PluginVersion,
                    p.BackOfficeVersion,
                    p.BackOfficePluginAutoUpdateProductId,
                    p.BackOfficePluginAutoUpdateProductVersion,
                    p.BackOfficePluginAutoUpdateComponentBaseName,
                    p.RunAsUserIsRequried,
                    p.Platform
                    );
                _stateCache.SetBackOfficePluginInformation(p.PluginId, pi);
            }
        }
      

        /// <summary>
        /// Get that value of an item in a string string dictionary,
        /// If the item is not in the dictionary, return empty string, rather then throw exception
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string ValueOrEmpty(IDictionary<string, string> dictionary, string key)
        {
            string outValue;
            if (!dictionary.TryGetValue(key, out outValue))
            {
                //did not find the key in the dictionary so set to empty string
                outValue = string.Empty;
            }

            //found the key but the value of key is null. Users of this method no longer expect null.
            //Thus turn the null into an empty.
            //users of this function should consider a adding a validation method to check for this before hand.
            //and reporting if unexpected nulls are found.
            if (outValue == null)
                outValue = string.Empty;

            return outValue;
        }


        /// <summary>
        /// ValidateBackOfficeAdminCredentialsResponse
        /// TODO KMS: modify ValidateBackOfficeAdminCredentialsResponse such that 
        /// TODO KMS: it contains the Response Status Diagnoses information
        /// </summary>
        /// <param name="backOfficeId"></param>
        /// <param name="credentials"></param>
        /// <returns></returns>
        public ValidateBackOfficeAdminCredentialsResponse ValidateBackOfficeAdminCredentials(String backOfficeId, IDictionary<string, string> credentials)
        {
            ValidateBackOfficeAdminCredentialsResponse result = new ValidateBackOfficeAdminCredentialsResponse(false, new String[] { }, new String[] { });

            try
            {
                List<String> userFacingMessages = new List<String>();
                List<String> rawErrorMessages = new List<String>();

                Guid requestId = Guid.NewGuid();
                var backOfficeConfig = new BackOfficeCompanyConfigurationObject();
                backOfficeConfig.BackOfficeId = backOfficeId;

                string payload = JsonConvert.SerializeObject(credentials);

                String responsePayload = MakeBackOfficeRequest("ValidateCompanyConnectionManagementCredentials", requestId, null, backOfficeConfig, payload);
                var validationInfo = DeserializeJson<ValidateCompanyConnectionManagementCredentialsResponse>(responsePayload);

                if (validationInfo.Diagnoses != null)
                {
                    foreach (var diagnosis in validationInfo.Diagnoses)
                    {
                        userFacingMessages.Add(diagnosis.UserFacingMessage);
                        rawErrorMessages.Add(diagnosis.RawMessage);
                    }
                }

                bool isValid = (validationInfo.Status == Status.Success);
                result = new ValidateBackOfficeAdminCredentialsResponse(isValid, userFacingMessages, rawErrorMessages);
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }
            }

            return result;
        }

        #endregion

        /// <summary>
        /// If a SYSTEM environment variable SAGE_CONNECTOR_STATE_SERVICE_BREAK is set to 1,, then 
        /// fire a debugger breakpoint.  This allows us to decide to break into the HostingFramework
        /// startup code without changing any code.
        /// 
        /// Can use the RUNME-ClearMessagingServiceBreak and RUNME-SetMessagingServiceBreak shortcuts
        /// to facilitate changing this variable.
        /// </summary>
        [Conditional("DEBUG")]
        private static void ConditionalDebugBreak()
        {
            String doBreak = Environment.GetEnvironmentVariable("SAGE_CONNECTOR_STATE_SERVICE_BREAK", EnvironmentVariableTarget.Machine);
            if (!String.IsNullOrEmpty(doBreak) && doBreak == "1")
            {
                Debugger.Break();
            }
        }

        /// <summary>
        /// Called by the HostingFx whenever the service should start work (e.g., service started or continued after paused)
        /// </summary>
        public void Startup()
        {
            ConditionalDebugBreak();

            using (new StackTraceContext(this))
            {
                InitRetryManagerDatabaseRepairMethod();

                lock (_syncObject)
                {
                    _stateCache.Start();
                }
            }
        }

        /// <summary>
        /// Called by the HostingFx whenever the service should cease work (e.g., service stopped or paused)
        /// </summary>
        public void Shutdown()
        {
            using (new StackTraceContext(this))
            {
                lock (_syncObject)
                {
                    _stateCache.Stop();
                }
            }
        }


        #region IDatabaseRepairerService Members

        /// <summary>
        /// Handler for recovering a database
        /// Will re-verify the database first, then back up the existing one
        /// Then attempt a repair, and finally perform a second verification
        /// To make sure the repair attempt was successful
        /// </summary>
        /// <param name="databaseFilename"></param>
        /// <param name="storagePath"></param>
        /// <returns>Whether or not the repair succeeded</returns>
        public bool RepairDatabase(string databaseFilename, string storagePath)
        {
            // Don't do anything if the file does not exist
            if (String.IsNullOrEmpty(databaseFilename) ||
                !File.Exists(Path.Combine(storagePath, databaseFilename)))
            {
                return false;
            }
            // Hold a local lock for the entirety of this method for the specific database
            // We do not want two different paths attempting to recover the same database at the same time
            lock (GetDatabaseRepairLockObject(databaseFilename))
            {
                // Check for existing hard corruption detected
                // If we already found one, then there is no point in 
                // Re-attempting the verification or recovery
                if (HasExistingHardCorruption(databaseFilename))
                {
                    return false;
                }

                // Setup file names and paths for potential SDF file backup
                string sdfFullPath = Path.Combine(storagePath, databaseFilename);
                string sdfBackupDestinationPath = Path.Combine(storagePath, ConnectorRegistryUtils.DatabaseRecoveryBackupDirectory);
                sdfBackupDestinationPath = Path.Combine(sdfBackupDestinationPath, String.Format("{0}_{1}_Recover", DateTime.Now.ToString("yyyyMMdd.HHmmss"), ConnectorRegistryUtils.ProductVersion));
                string sdfBackupDestinationFullPath = Path.Combine(sdfBackupDestinationPath, databaseFilename);

                try
                {
                    // Common code to execute the retry
                    using (var logger = new SimpleTraceLogger())
                    {
                        return DatabaseRepairUtils.RepairDatabase(
                            sdfFullPath,
                            sdfBackupDestinationFullPath,
                            logger);
                    }
                }
                catch (Exception ex)
                {
                    // Any exception (e.g. above retries all failed) will be considered a hard corruption
                    // Notify the state service of the hard corruption.  
                    HandleHardDatabaseCorruption(databaseFilename, ex);
                }
            }

            // Did not succeed
            return false;
        }

        /// <summary>
        /// Provide a notification that we have encountered a hard database corruption
        /// </summary>
        /// <param name="databaseFilename"></param>
        /// <param name="ex"></param>
        public void HandleHardDatabaseCorruption(string databaseFilename, Exception ex)
        {
            lock (_syncObject)
            {
                // Check that this is the first hard corruption 
                if (HasExistingHardCorruption(databaseFilename))
                {
                    return;
                }

                // Create the error messages that we'll need
                string detailedErrorMessage = String.Format(_hardCorruptDetailedErrorMessageTemplate, (ex == null) ? "null" : ex.ToString());
                string userFacingErrorMessage = String.Format(_hardCorruptUserFacingErrorMessage, databaseFilename);

                // Add a windows event log entry for hard corruption detection
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteCriticalWithEventLogging(
                        this, "CorruptDatabaseRecovery", detailedErrorMessage);
                }

                // Perform notification
                NotifyOfHardDatabaseCorruption(
                    detailedErrorMessage, userFacingErrorMessage, _hardCorruptHelpLinkId, DateTime.UtcNow);

                // Store the fact that a hard corruption was detected
                // Do this last so if there are any failures along the way, we will
                // Retry the database repair/hard corruption notification to state service
                _hardCorruptionDetected[databaseFilename] = true;
            }
        }

        #endregion


        #region Database Repair Service Helpers

        /// <summary>
        /// Init the recovery manager externally
        /// Just used to set the database recovery handler
        /// </summary>
        private void InitRetryManagerDatabaseRepairMethod()
        {
            RetryPolicyManager.SetDatabaseRepairMethod(RepairDatabaseCaller);
        }

        /// <summary>
        /// Wraps up the call to the database repair method, which is behind the database
        /// Repairer interface.  The caller of the repair method has to handle initial verification
        /// That we are in a corrupt DB situation, and verification after a repair attempt is made
        /// That the final database is no longer corrupt. Since the call to SqlCeEngine.Verify() is 
        /// Completely useless (i.e returned false positives and negatives), we do this by retrying 
        /// The original failed action.
        /// </summary>
        /// <param name="databaseFilename"></param>
        /// <param name="storePath"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        private bool RepairDatabaseCaller(string databaseFilename, string storePath, Action action)
        {
            // STEP 1: Verify that the database is in fact corrupt
            // By re-executing the action that detected the corruption
            if (!DatabaseCorruptionHelper.VerifyDatabaseCorruption(action))
            {
                // Database not corrupt, we're done
                return true;
            }

            // STEP 2: Database was corrupt, attempt to repair it
            bool dbRepairSuccessful = RepairDatabase(databaseFilename, storePath);

            if (dbRepairSuccessful)
            {
                // STEP 3: Database repair attempt succeeded
                // Re-attempt the initial action to make sure things worked
                dbRepairSuccessful = DatabaseCorruptionHelper.VerifyDatabaseAfterRepair(action);
            }

            if (!dbRepairSuccessful)
            {
                // STEP 4: Either the repair failed, or re-verification after the repair failed
                // This is a hard database corruption, so handle that
                HandleHardDatabaseCorruption(databaseFilename,
                    new Exception("Attempt to repair corrupt database failed"));
            }

            // Return whether or not we repaired the database
            return dbRepairSuccessful;
        }

        /// <summary>
        /// Get the lock object for this specific database file
        /// Create a new lock object and store it if one does not already exist
        /// </summary>
        /// <param name="databaseFilename"></param>
        /// <returns></returns>
        private Object GetDatabaseRepairLockObject(string databaseFilename)
        {
            lock (_syncObject)
            {
                Object lockObject;
                if (!_databaseRepairLocks.TryGetValue(databaseFilename, out lockObject))
                {
                    // Object does not exist yet, so add it
                    lockObject = new Object();
                    _databaseRepairLocks[databaseFilename] = lockObject;
                }

                // Return either retrieved or newly created lock object
                return lockObject;
            }
        }

        /// <summary>
        /// Determine if the existing db file has already seen a hard corruption
        /// </summary>
        /// <param name="databaseFilename"></param>
        /// <returns></returns>
        private bool HasExistingHardCorruption(string databaseFilename)
        {
            lock (_syncObject)
            {
                bool hasExistingHardCorruption;
                _hardCorruptionDetected.TryGetValue(databaseFilename, out hasExistingHardCorruption);
                return hasExistingHardCorruption;
            }
        }

        /// <summary>
        /// The actual action for hard database corruption notification
        /// Just sends a subsystem health issue message to the state service
        /// </summary>
        /// <param name="rawMessage"></param>
        /// <param name="userFacingMessage"></param>
        /// <param name="helpLinkId"></param>
        /// <param name="timestampUtc"></param>
        private void NotifyOfHardDatabaseCorruption(
            string rawMessage,
            string userFacingMessage,
            Int32 helpLinkId,
            DateTime timestampUtc)
        {
            // Create the message
            SubsystemHealthMessage message = new SubsystemHealthMessage(
                Subsystem.DatabaseHardCorruption,
                rawMessage,
                userFacingMessage,
                timestampUtc,
                helpLinkId);

            // Raise it
            RaiseSubsystemHealthIssue(message);
        }

        #endregion


        #region Private Members

        // ReSharper disable once UnusedMember.Local
        private static readonly String _myTypeName = typeof(StateService).FullName;
        private static readonly Object _syncObject = new Object();
        private static readonly StateCache _stateCache = new StateCache();

        /// <summary>
        /// Dictionary of database name to flags storing whether or not
        /// A hard corruption has been detected.  If one has, we do not want to 
        /// Keep trying to repair that database.
        /// </summary>
        private static readonly Dictionary<string, bool> _hardCorruptionDetected =
            new Dictionary<string, bool>();

        /// <summary>
        /// Provide locks per database so we could technically restore two different dbs at once
        /// For use only with database repair scenarios
        /// </summary>
        private static readonly Dictionary<string, Object> _databaseRepairLocks =
            new Dictionary<string, object>();

        private static readonly string _hardCorruptDetailedErrorMessageTemplate = "Could not repair corrupt database, exception: {0}";
        private static readonly string _hardCorruptUserFacingErrorMessage = "The '{0}' database has become corrupt and cannot be used or repaired. \n\nTo recreate a new, clean database, follow the instructions in the Help topic.";
        private static readonly Int32 _hardCorruptHelpLinkId = 10035;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteAddress"></param>
        /// <param name="tenantId"></param>
        /// <param name="premiseId"></param>
        /// <param name="wireClaim"></param>
        /// <returns></returns>
        public ValidateTenantConnectionResponse ValidateTenantConnection(string siteAddress, string tenantId, string premiseId, string wireClaim)
        {
            var result = new ValidateTenantConnectionResponse(String.Empty, null, TenantConnectivityStatus.None);

            try
            {
                using (var proxy = MessagingServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                {
                    result = proxy.ValidateTenantConnection(siteAddress, tenantId, premiseId, wireClaim);
                }
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }
            }

            return result;
        }


        private String MakeBackOfficeRequest(string requestName, Guid requestId,
            BackOfficeCompanyConfigurationObject backOfficeConfiguration)
        {
            return MakeBackOfficeRequest(requestName, requestId, null, backOfficeConfiguration, null);
        }

        private String MakeBackOfficeRequest(string requestName, Guid requestId, string tenantId,
            BackOfficeCompanyConfigurationObject backOfficeConfiguration, string payload)
        {
            var processRequestActivation = new ProcessRequestActivation();

            ActivityTrackingContext atc;
            using (var lm = new LogManager())
            {
                string requestType = "System DomainMediation";
                string useTenantId = (string.IsNullOrWhiteSpace(tenantId) ? Guid.Empty.ToString() : tenantId);
                //tracking or system requests is a bit funky. We set state1 and bindable work start so we have numbers to compute duration
                atc = lm.BeginActivityTrackingInternalSystemRequest(this, useTenantId, requestId, requestType, requestName);

                String envRequestResponse = Environment.GetEnvironmentVariable("SAGE_CONNECTOR_LOG_REQUEST_RESPONSE", EnvironmentVariableTarget.Machine);
                Boolean logRequestResponse = (!String.IsNullOrEmpty(envRequestResponse) && envRequestResponse == "1");

                if (logRequestResponse)
                {
                    lm.WriteInfoForRequest(this, atc, "System request: {0}, with payload: {1}", requestName, payload ?? "Null Payload");
                }
            }

            using (CancellationTokenSource cancelTokenSource = new CancellationTokenSource())
            {
                processRequestActivation.ExecuteProcessRequest(Request_ProcessResponse,
                    atc,
                    backOfficeConfiguration,
                    requestName,
                    payload,
                    cancelTokenSource);
            }

            if (!_responsePayload.ContainsKey(requestId))
            {
                //TODO: do we really want to throw here? How does upstream deal?
                // we must have error'd, return null or throw exception
                string msg = string.Format("No response event handling for {0}", requestName);
                throw new ApplicationException(msg);
            }

            IList<String> payloadList = _responsePayload[requestId];
            _responsePayload.Remove(requestId);
            string retval = payloadList[0];

            using (var lm = new LogManager())
            {
                //TODO: consider adding conditional logging of response

                //now that we have a response add the closing parts of the tracking and close the trace. The upstream systems currently
                //have no idea of the tracking system and its out of scope to make them aware assuming they even should be.
                lm.AdvanceActivityState(this,atc, ActivityState.State13_BindableWorkComplete, ActivityEntryStatus.InProgress);
                
                //Note: for system messages we "lie" and say they always were a success. 
                //calling locations do not currently know about the activity tracking system and are the only ones that know if
                //the request was a success or not.
                lm.AdvanceActivityState(this, atc, ActivityState.State17_ResponseSentToCloud, ActivityEntryStatus.CompletedWithSuccessResponse);
            }

            return retval;
        }

        /// <summary>
        /// Process the response for the request. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Request_ProcessResponse(object sender, ResponseEventArgs e)
        {
            Debug.Print("Host: Work progressing: {0}", e.Payload);

            //We're going to stop the add-in if it ever reports progress > 50%
            Debug.Print("Host: Request Response processing: {0}", e.Payload);

            

            //Need to concern ourselves withe the possibility that this response could be called multiple times before it is done. 
            //For this state calls, we are forcing them to be synchronous. so gathering up all the payloads and adding them 
            //to the list before we pull them off and process them. 
            IList<String> payloadList;
            if (!_responsePayload.TryGetValue(e.RequestId, out payloadList))
            {
                payloadList = new List<string>();
                _responsePayload.Add(e.RequestId, payloadList);
            }
            string payload = e.Payload;
            payloadList.Add(payload);

            using (var lm = new LogManager())
            {
                String envRequestResponse = Environment.GetEnvironmentVariable("SAGE_CONNECTOR_LOG_REQUEST_RESPONSE", EnvironmentVariableTarget.Machine);
                Boolean logRequestResponse = (!String.IsNullOrEmpty(envRequestResponse) && envRequestResponse == "1");

                ActivityTrackingContext atc = new ActivityTrackingContext(e.TrackingId, e.TenantId, e.RequestId, string.Empty);
                
                if (logRequestResponse)
                    lm.WriteInfoForRequest(this, atc, "Request Response:{0}",payload);

                lm.AdvanceActivityState(this, atc, ActivityState.State12_ProcessExecutionComplete, ActivityEntryStatus.InProgress);
                //lm.WriteInfoForRequest(null, rw.ActivityTrackingContext, "Invoking " + ib.GetType().FullName + " on request '" + rw.ActivityTrackingContext.RequestId + "'");
            }
        }

        private readonly IDictionary<Guid, IList<String>> _responsePayload = new Dictionary<Guid, IList<String>>();


        private T DeserializeJson<T>(string payload)
        {
            T retval = JsonConvert.DeserializeObject<T>(payload, new DomainMediatorJsonSerializerSettings());
            return retval;
        }

        private T DeserializeJson<T>(string payload, JsonSerializerSettings settings)
        {
            T retval = JsonConvert.DeserializeObject<T>(payload, settings);
            return retval;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ManagementCredentialsNeededResponse GetManagementCredentialsNeeded(string backOfficeId)
        {
            //TODO: value up error responses here maybe?
            ManagementCredentialsNeededResponse result = new ManagementCredentialsNeededResponse(null, null, null, null);

            try
            {
                List<String> userFacingMessages = new List<String>();
                List<String> rawErrorMessages = new List<String>();

                Guid requestId = Guid.NewGuid();
                var backOfficeConfig = new BackOfficeCompanyConfigurationObject();
                backOfficeConfig.BackOfficeId = backOfficeId;
                String responsePayload = MakeBackOfficeRequest("GetCompanyConnectionManagementCredentialsNeeded", requestId, backOfficeConfig);

                Dictionary<string, string> values = new Dictionary<string, string>();
                string descriptionsAsString = string.Empty;
                
                CompanyManagementCredentialsNeededResponse response = DeserializeJson<CompanyManagementCredentialsNeededResponse>(responsePayload);
                if (response != null)
                {
                    values = response.CurrentValues as Dictionary<string, string>;
                    Dictionary<string, object> descriptions = response.Descriptions as Dictionary<string, object>;
                    descriptionsAsString = JsonConvert.SerializeObject(descriptions);

                    if (response.Diagnoses != null)
                    {
                        foreach (var diagnosis in response.Diagnoses)
                        {
                            userFacingMessages.Add(diagnosis.UserFacingMessage);
                            rawErrorMessages.Add(diagnosis.RawMessage);
                        }
                    }
                }

                result = new ManagementCredentialsNeededResponse(descriptionsAsString, values, userFacingMessages, rawErrorMessages);
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="backOfficeId"></param>
        /// <param name="companyManagementCredentials"></param>
        /// <param name="companyConnectionCredentials"></param>
        /// <returns></returns>
        public ConnectionCredentialsNeededResponse GetConnectionCredentialsNeeded(
            string backOfficeId,
            IDictionary<string, string> companyManagementCredentials,
            IDictionary<string, string> companyConnectionCredentials)
        {
            //TODO: value up error responses here maybe?
            ConnectionCredentialsNeededResponse result = new ConnectionCredentialsNeededResponse(null, null, null, null);

            try
            {
                List<String> userFacingMessages = new List<String>();
                List<String> rawErrorMessages = new List<String>();

                Guid requestId = Guid.NewGuid();
                var backOfficeConfig = new BackOfficeCompanyConfigurationObject();
                backOfficeConfig.BackOfficeId = backOfficeId;

                var credentials = new CompanyCredentials
                {
                    CompanyManagementCredentials = companyManagementCredentials,
                    CompanyConnectionCredentials = companyConnectionCredentials
                };
                string requestPayload = JsonConvert.SerializeObject(credentials);
                String responsePayload = MakeBackOfficeRequest("GetCompanyConnectionCredentialsNeeded", requestId, null, backOfficeConfig, requestPayload);

                Dictionary<string, string> values = new Dictionary<string, string>();
                string responseDescriptions = string.Empty;
                
                CompanyConnectionCredentialsNeededResponse response = DeserializeJson<CompanyConnectionCredentialsNeededResponse>(responsePayload);
                if (response != null)
                {
                    values = response.CurrentValues as Dictionary<string, string>;
                    Dictionary<string, object> descriptions = response.Descriptions as Dictionary<string, object>;
                    if (descriptions != null &&  descriptions.Count > 0)
                    {
                        responseDescriptions = JsonConvert.SerializeObject(descriptions);
                    }
                        

                    if (response.Diagnoses != null)
                    {
                        foreach (var diagnosis in response.Diagnoses)
                        {
                            userFacingMessages.Add(diagnosis.UserFacingMessage);
                            rawErrorMessages.Add(diagnosis.RawMessage);
                        }
                    }
                }

                result = new ConnectionCredentialsNeededResponse(responseDescriptions, values, userFacingMessages, rawErrorMessages);
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }
            }

            return result;
        }



        #region Feature Service

        /// <summary>
        /// 
        /// </summary>
        /// <param name="backOfficeId"></param>
        /// <param name="backOfficeCredentials"></param>
        /// <param name="featureId"></param>
        /// <param name="tenantId"></param>
        /// <param name="requestPayload"></param>
        /// <returns></returns>
        public FeatureResponse GetFeatureResponse(string backOfficeId, string backOfficeCredentials, string featureId, string tenantId, string requestPayload)
        {
            //TODO: value up error responses here maybe?
            var result = new FeatureResponse(null, new String[] { }, new String[] { });
            try
            {
                Guid requestId = Guid.NewGuid();
                
                //need a storage path since features need to store things.
                var dataStoragePath = DocumentManager.GetTenantDataStorageFolder(tenantId);
                var backOfficeConfig = new BackOfficeCompanyConfigurationObject
                {
                    BackOfficeId = backOfficeId,
                    ConnectionCredentials = backOfficeCredentials,
                    DataStoragePath = dataStoragePath
                };

                String responsePayload = MakeBackOfficeRequest(featureId, requestId, tenantId, backOfficeConfig, requestPayload);
                if (!String.IsNullOrWhiteSpace(responsePayload))
                {
                    result.Payload = responsePayload;
                }
            }
            catch (Exception ex)
            {
                // don't let any unintended exception escape the WCF service boundary; should use explicit FaultContract
                // to send error to client
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }
            }

            return result;
        }
        #endregion
    }
}

