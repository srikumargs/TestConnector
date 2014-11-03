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
    /// The SyncTaxCodes Feature implementation.
    /// </summary>
    [Export(typeof(IDomainFeatureRequest))]
    [FeatureMetadataExport(FeatureMessageTypes.SyncTaxCodes, typeof(FeatureDescriptions), "ISyncTaxCodes")]
    public class SyncTaxCodes : AbstractSyncDomainMediator
    {
        [ImportMany]
#pragma warning disable 649
        private IEnumerable<Lazy<ISyncTaxCodes, IBackOfficeData>> _backOfficeHandlers;
#pragma warning restore 649
        /// <summary>
        /// The SyncTaxCodes Feature Request Implementation. 
        /// </summary>
        /// <param name="processContext">The <see cref="IProcessContext"/></param>
        /// <param name="requestId">The Connector Request Id</param>
        /// <param name="tenantId">The tenant Id making the request.</param>
        /// <param name="backOfficeCompanyConfiguration">The back office company configuration <see cref="IBackOfficeCompanyConfiguration"/></param>
        /// <param name="payload">String representing the payload to process.</param>
        public override void FeatureRequest(IProcessContext processContext, Guid requestId, String tenantId,
            IBackOfficeCompanyConfiguration backOfficeCompanyConfiguration, string payload)
        {
            var syncTaxCodesResponse = new SyncTaxCodesResponse();

            try
            {
                if (backOfficeCompanyConfiguration == null)
                    throw new NoNullAllowedException("Back Office Company Configuration must exist to process feature request.");

                CreateCompositionContainer(GetBackOfficePluginPartialPath(backOfficeCompanyConfiguration.BackOfficeId));
                SetupDataStoragePath(backOfficeCompanyConfiguration.DataStoragePath);
                
                ISyncTaxCodes backofficeFeatureProcessor = (from backOfficeHandler in _backOfficeHandlers
                                                            where backOfficeHandler.Metadata.BackOfficeId.Equals(backOfficeCompanyConfiguration.BackOfficeId,
                                                            StringComparison.CurrentCultureIgnoreCase)
                                                            select backOfficeHandler.Value).FirstOrDefault();

                if (backofficeFeatureProcessor == null)
                {
                    Debug.Print("{0} BackOffice to SyncTaxCodes was not found!", backOfficeCompanyConfiguration.BackOfficeId);
                    throw new ApplicationException(String.Format("{0} BackOffice to SyncTaxCodes was not found!", backOfficeCompanyConfiguration.BackOfficeId));
                }

                if (String.IsNullOrWhiteSpace(payload))
                {
                    throw new ArgumentNullException(String.Format("{0} error: Missing request payload.",
                        FeatureMessageTypes.SyncTaxCodes));
                }

                //SyncRequest Deserialization
                var payloadSettings = new DomainMediatorJsonSerializerSettings();
                var syncRequest = JsonConvert.DeserializeObject<SyncRequest>(payload, payloadSettings);

                //Initialize syncTaxCodesResponse
                var taxCodes = new Collection<TaxCode>();
                syncTaxCodesResponse.SyncDigest = new SyncDigest
                {
                    EndpointId = SyncConstants.BackOfficeEndpointId,
                    ResourceKindName = syncRequest.ResourceKindName,
                    EndpointTick = syncRequest.CloudTick
                };
                syncTaxCodesResponse.TaxCodes = taxCodes;

                // ReSharper disable once SuspiciousTypeConversion.Global
                var backOfficeSessionHandler = backofficeFeatureProcessor as IBackOfficeSessionHandler;

                try
                {
                    processContext.TrackPluginInvoke();
                    BeginBackOfficeSession(processContext.GetSessionContext(), backOfficeSessionHandler, backOfficeCompanyConfiguration, syncTaxCodesResponse);
                    InitializeFeaturePropertyDefaults(backofficeFeatureProcessor, tenantId, syncTaxCodesResponse);
                    CheckResponse(backofficeFeatureProcessor.InitializeSyncTaxCodes(), syncTaxCodesResponse);

                    //Continue with loop to sync tax codes only when not in failure mode.
                    TaxCode taxCode = backofficeFeatureProcessor.GetNextSyncTaxCode();

                    if (taxCode != null)
                    {
                        //because attribute cannot be inherited, we require this check 
                        string idPropertyName = GetExternalIdPropertyName(taxCode.GetType());

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
                                    syncTaxCodesResponse.SyncDigest.EndpointTick = client.Tick;
                                    while (taxCode != null)
                                    {
                                        if (!client.SyncEntity(taxCode, idPropertyName).Equals(SyncEntityState.Unchanged))
                                        {
                                            ExternalIdUtilities.ApplyExternalIdEncoding(taxCode);
                                            taxCodes.Add(taxCode);

                                            if (taxCodes.Count == processContext.EntityChunkCount)
                                            {
                                                try
                                                {
                                                    syncTaxCodesResponse.Status = Status.Success;
                                                    processContext.ResponseHandler.HandleResponse(requestId, JsonConvert.SerializeObject(syncTaxCodesResponse, new DomainMediatorJsonSerializerSettings()), false);
                                                    taxCodes.Clear();
                                                }
                                                finally
                                                {
                                                    syncTaxCodesResponse.Status = Status.Indeterminate;
                                                }
                                            }
                                        }
                                        taxCode = backofficeFeatureProcessor.GetNextSyncTaxCode();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ProcessException(syncTaxCodesResponse, ex);
                                }
                                finally
                                {
                                    client.EndSync(true);
                                    ProcessDeletedEntities(client, syncTaxCodesResponse);
                                }
                            }
                            catch (Exception ex)
                            {
                                ProcessException(syncTaxCodesResponse, ex);
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
                    ProcessException(syncTaxCodesResponse, ex);
                }
                finally
                {
                    backOfficeSessionHandler.EndSession();
                    processContext.TrackPluginComplete();
                }

            }
            catch (Exception ex)
            {
                ProcessException(syncTaxCodesResponse, ex);
            }

            if (!syncTaxCodesResponse.Status.Equals(Status.Failure))
            {
                syncTaxCodesResponse.Status = Status.Success;
            }

            String responsePayload = JsonConvert.SerializeObject(syncTaxCodesResponse, new DomainMediatorJsonSerializerSettings());
            processContext.ResponseHandler.HandleResponse(requestId, responsePayload);
        }



    }
}
