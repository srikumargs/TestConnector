using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using Sage.Connector.DomainContracts.BackOffice;
using Sage.Connector.DomainContracts.Data.Metadata;
using Sage.Connector.DomainContracts.Responses;
using Sage.Connector.DomainMediator.Core;
using Sage.Connector.DomainMediator.Core.JsonConverters;
using Sage.Connector.DomainMediator.Core.Utilities;
using Sage.Connector.Sync;
using Sage.Connector.Sync.Contracts.CloudIntegration;
using Sage.Connector.Sync.Contracts.CloudIntegration.Requests;
using Sage.Connector.Sync.Contracts.Data;
using Sage.Connector.Sync.Interfaces;
using Sage.Connector.Sync.Mediator.Features;
using Sage.Connector.Taxes.Contracts.BackOffice;
using Sage.Connector.Taxes.Contracts.CloudIntegration;
using Sage.Connector.Taxes.Contracts.Data;

namespace Sage.Connector.Taxes.Mediator
{
    /// <summary>
    /// The SyncTaxSchedules Domain Feature implementation.
    /// </summary>
    [Export(typeof(IDomainFeatureRequest))]
    [FeatureMetadataExport(FeatureMessageTypes.SyncTaxSchedules, typeof(FeatureDescriptions), "ISyncTaxSchedules")]
    public class SyncTaxSchedules : AbstractSyncDomainMediator
    {
        [ImportMany]
#pragma warning disable 649
        private IEnumerable<Lazy<ISyncTaxSchedules, IBackOfficeData>> _backOfficeHandlers;
#pragma warning restore 649
        /// <summary>
        /// The SyncTaxSchedules Feature Request Implementation.
        /// </summary>
        /// <param name="processContext">The <see cref="IProcessContext" /></param>
        /// <param name="requestId">The Connector Request Id</param>
        /// <param name="tenantId">The tenant Id making the request.</param>
        /// <param name="backOfficeCompanyConfiguration">The back office configuration.</param>
        /// <param name="payload">String representing the payload to process.</param>
        /// <exception cref="System.Data.NoNullAllowedException">Back Office Company Configuration must exist to process feature request.</exception>
        /// <exception cref="System.ApplicationException"></exception>
        /// <exception cref="System.ArgumentNullException"></exception>
        public override void FeatureRequest(IProcessContext processContext, Guid requestId, String tenantId,
            IBackOfficeCompanyConfiguration backOfficeCompanyConfiguration, string payload)
        {
            var syncTaxSchedulesResponse = new SyncTaxSchedulesResponse();
            try
            {

                if (backOfficeCompanyConfiguration == null)
                    throw new NoNullAllowedException("Back Office Company Configuration must exist to process feature request.");

                CreateCompositionContainer(GetBackOfficePluginPartialPath(backOfficeCompanyConfiguration.BackOfficeId));
                SetupDataStoragePath(backOfficeCompanyConfiguration.DataStoragePath);

                ISyncTaxSchedules backofficeFeatureProcessor = (from backOfficeHandler in _backOfficeHandlers
                                                                where backOfficeHandler.Metadata.BackOfficeId.Equals(backOfficeCompanyConfiguration.BackOfficeId,
                                                                StringComparison.CurrentCultureIgnoreCase)
                                                                select backOfficeHandler.Value).FirstOrDefault();

                if (backofficeFeatureProcessor == null)
                {
                    Debug.Print("{0} BackOffice to SyncTaxSchedules was not found!", backOfficeCompanyConfiguration.BackOfficeId);
                    throw new ApplicationException(String.Format("{0} BackOffice to {1} was not found!",
                        backOfficeCompanyConfiguration.BackOfficeId, FeatureMessageTypes.SyncTaxSchedules));
                }

                if (String.IsNullOrWhiteSpace(payload))
                {
                    throw new ArgumentNullException(String.Format("{0} error: Missing request payload.",
                        FeatureMessageTypes.SyncTaxSchedules));
                }

                //SyncRequest Deserialization
                var payloadSettings = new DomainMediatorJsonSerializerSettings();
                var syncRequest = JsonConvert.DeserializeObject<SyncRequest>(payload, payloadSettings);

                var taxSchedules = new Collection<TaxSchedule>();

                //Setup response. 
                syncTaxSchedulesResponse.SyncDigest = new SyncDigest
                {
                    EndpointId = SyncConstants.BackOfficeEndpointId,
                    ResourceKindName = syncRequest.ResourceKindName,
                    EndpointTick = syncRequest.CloudTick
                };
                syncTaxSchedulesResponse.TaxSchedules = taxSchedules;

                // ReSharper disable once SuspiciousTypeConversion.Global
                var backOfficeSessionHandler = backofficeFeatureProcessor as IBackOfficeSessionHandler;

                try
                {
                    processContext.TrackPluginInvoke();
                    BeginBackOfficeSession(processContext.GetSessionContext(), backOfficeSessionHandler, backOfficeCompanyConfiguration, syncTaxSchedulesResponse);
                    InitializeFeaturePropertyDefaults(backofficeFeatureProcessor, tenantId, syncTaxSchedulesResponse);
                    CheckResponse(backofficeFeatureProcessor.InitializeSyncTaxSchedules(), syncTaxSchedulesResponse);

                    //Continue with loop to sync tax schedule only when not in failure mode.

                    TaxSchedule taxSchedule = backofficeFeatureProcessor.GetNextSyncTaxSchedule();
                    if (taxSchedule != null)
                    {
                        //because attribute cannot be inherited, we require this check 
                        string idPropertyName = GetExternalIdPropertyName(taxSchedule.GetType());

                        using (var client = new SyncClient(tenantId, GetSyncDataStoragePath()))
                        {
                            try
                            {
                                client.OpenDatabase();
                                client.BeginSession(syncRequest.ResourceKindName, syncRequest.CloudTick);

                                try
                                {
                                    //TODO KMS: Sync type needs to be part of the plugin configuration for the resource.
                                    client.BeginSync(SyncType.Internal);
                                    syncTaxSchedulesResponse.SyncDigest.EndpointTick = client.Tick;
                                    while (taxSchedule != null)
                                    {
                                        if (!client.SyncEntity(taxSchedule, idPropertyName).Equals(SyncEntityState.Unchanged))
                                        {
                                            ExternalIdUtilities.ApplyExternalIdEncoding(taxSchedule);
                                            taxSchedules.Add(taxSchedule);

                                            if (taxSchedules.Count == processContext.EntityChunkCount)
                                            {
                                                try
                                                {
                                                    syncTaxSchedulesResponse.Status = Status.Success;
                                                    processContext.ResponseHandler.HandleResponse(requestId, JsonConvert.SerializeObject(syncTaxSchedulesResponse, new DomainMediatorJsonSerializerSettings()), false);
                                                    taxSchedules.Clear();
                                                }
                                                finally
                                                {
                                                    syncTaxSchedulesResponse.Status = Status.Indeterminate;
                                                }
                                            }
                                        }
                                        taxSchedule = backofficeFeatureProcessor.GetNextSyncTaxSchedule();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ProcessException(syncTaxSchedulesResponse, ex);
                                }
                                finally
                                {
                                    client.EndSync(true);
                                    ProcessDeletedEntities(client, syncTaxSchedulesResponse);
                                }
                            }
                            catch (Exception ex)
                            {
                                ProcessException(syncTaxSchedulesResponse, ex);
                            }
                            finally
                            {
                                client.EndSession();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ProcessException(syncTaxSchedulesResponse, ex);
                }
                finally
                {
                    backOfficeSessionHandler.EndSession();
                    processContext.TrackPluginComplete();
                }

            }
            catch (Exception ex)
            {

                ProcessException(syncTaxSchedulesResponse, ex);
            }

            if (!syncTaxSchedulesResponse.Status.Equals(Status.Failure))
            {
                syncTaxSchedulesResponse.Status = Status.Success;
            }

            String responsePayload = JsonConvert.SerializeObject(syncTaxSchedulesResponse, new DomainMediatorJsonSerializerSettings());
            processContext.ResponseHandler.HandleResponse(requestId, responsePayload);
        }



    }
}
