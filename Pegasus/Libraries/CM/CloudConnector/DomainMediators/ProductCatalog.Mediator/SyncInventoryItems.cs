using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using ProductCatalog.Contracts.CloudIntegration;
using Sage.Connector.DomainContracts.BackOffice;
using Sage.Connector.DomainContracts.Data.Metadata;
using Sage.Connector.DomainContracts.Responses;
using Sage.Connector.DomainMediator.Core;
using Sage.Connector.DomainMediator.Core.JsonConverters;
using Sage.Connector.DomainMediator.Core.Utilities;
using Sage.Connector.ProductCatalog.Contracts.BackOffice;
using Sage.Connector.ProductCatalog.Contracts.Data;
using Sage.Connector.Sync;
using Sage.Connector.Sync.Contracts.CloudIntegration;
using Sage.Connector.Sync.Contracts.CloudIntegration.Requests;
using Sage.Connector.Sync.Contracts.Data;
using Sage.Connector.Sync.Interfaces;

namespace Sage.Connector.ProductCatalog.Mediator
{
    /// <summary>
    /// The SyncInventoryItems Feature implementation.
    /// </summary>
    [Export(typeof(IDomainFeatureRequest))]
    [FeatureMetadataExport(FeatureMessageTypes.SyncInventoryItems, typeof(FeatureDescriptions), "ISyncInventoryItems")]
    public class SyncInventoryItems : AbstractSyncDomainMediator
    {
        [ImportMany]
#pragma warning disable 649
        private IEnumerable<Lazy<ISyncInventoryItems, IBackOfficeData>> _backOfficeHandlers;
#pragma warning restore 649
        /// <summary>
        /// The SyncInventoryItems SyncShared Feature Request Implementation. 
        /// </summary>
        /// <param name="processContext">The <see cref="IProcessContext"/></param>
        /// <param name="requestId">The Connector Request Id</param>
        /// <param name="tenantId">The tenant Id making the request.</param>
        /// <param name="backOfficeCompanyConfiguration">The back office company configuration <see cref="IBackOfficeCompanyConfiguration"/></param>
        /// <param name="payload">String representing the payload to process.</param>
        public override void FeatureRequest(IProcessContext processContext, Guid requestId, String tenantId,
            IBackOfficeCompanyConfiguration backOfficeCompanyConfiguration, string payload)
        {
            var syncInventoryItemsResponse = new SyncInventoryItemsResponse();
            try
            {
                if (backOfficeCompanyConfiguration == null)
                    throw new NoNullAllowedException("Back Office Company Configuration must exist to process feature request.");
                
                CreateCompositionContainer(GetBackOfficePluginPartialPath(backOfficeCompanyConfiguration.BackOfficeId));
                SetupDataStoragePath(backOfficeCompanyConfiguration.DataStoragePath);

                ISyncInventoryItems backofficeFeatureProcessor = (from backOfficeHandler in _backOfficeHandlers
                                                                  where backOfficeHandler.Metadata.BackOfficeId.Equals(backOfficeCompanyConfiguration.BackOfficeId,
                                                                  StringComparison.CurrentCultureIgnoreCase)
                                                                  select backOfficeHandler.Value).FirstOrDefault();

                if (backofficeFeatureProcessor == null)
                {
                    Debug.Print("{0} BackOffice to SyncInventoryItems was not found!", backOfficeCompanyConfiguration.BackOfficeId);
                    throw new ApplicationException(String.Format("{0} BackOffice to SyncInventoryItems was not found!", backOfficeCompanyConfiguration.BackOfficeId));
                }

                if (String.IsNullOrWhiteSpace(payload))
                {
                    throw new ArgumentNullException(String.Format("{0} error: Missing request payload.",
                        FeatureMessageTypes.SyncInventoryItems));
                }
                //Request Payload Deserialization
                var payloadSettings = new DomainMediatorJsonSerializerSettings();
                var syncRequest = JsonConvert.DeserializeObject<SyncRequest>(payload, payloadSettings);

                var inventoryItems = new Collection<InventoryItem>();

                //Initialize Response
                syncInventoryItemsResponse.SyncDigest = new SyncDigest
                {
                    EndpointId = SyncConstants.BackOfficeEndpointId,
                    ResourceKindName = syncRequest.ResourceKindName,
                    EndpointTick = syncRequest.CloudTick
                };
                syncInventoryItemsResponse.InventoryItems = inventoryItems;

                var backOfficeSessionHandler = backofficeFeatureProcessor as IBackOfficeSessionHandler;

                try
                {
                    processContext.TrackPluginInvoke();
                    BeginBackOfficeSession(processContext.GetSessionContext(), backOfficeSessionHandler, backOfficeCompanyConfiguration, syncInventoryItemsResponse);
                    InitializeFeaturePropertyDefaults(backofficeFeatureProcessor, tenantId, syncInventoryItemsResponse);
                    CheckResponse(backofficeFeatureProcessor.InitializeSyncInventoryItems(), syncInventoryItemsResponse);

                    InventoryItem inventoryItem = backofficeFeatureProcessor.GetNextSyncInventoryItem();

                    if (inventoryItem != null)
                    {
                        //Verify an ExternalId Attribute exists 
                        string idPropertyName = GetExternalIdPropertyName(inventoryItem.GetType());

                        //Sync Inventory Items calls to back office
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
                                    syncInventoryItemsResponse.SyncDigest.EndpointTick = client.Tick;
                                    while (inventoryItem != null)
                                    {
                                        if (!client.SyncEntity(inventoryItem, idPropertyName).Equals(SyncEntityState.Unchanged))
                                        {
                                            ExternalIdUtilities.ApplyExternalIdEncoding(inventoryItem);
                                            inventoryItems.Add(inventoryItem);

                                            if (inventoryItems.Count == processContext.EntityChunkCount)
                                            {
                                                try
                                                {
                                                    syncInventoryItemsResponse.Status = Status.Success;
                                                    processContext.ResponseHandler.HandleResponse(requestId, JsonConvert.SerializeObject(syncInventoryItemsResponse, new DomainMediatorJsonSerializerSettings()), false);
                                                    inventoryItems.Clear();
                                                }
                                                finally
                                                {
                                                    syncInventoryItemsResponse.Status = Status.Indeterminate;
                                                }
                                            }
                                        }
                                        inventoryItem = backofficeFeatureProcessor.GetNextSyncInventoryItem();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ProcessException(syncInventoryItemsResponse, ex);
                                }
                                finally
                                {
                                    client.EndSync(true);
                                    ProcessDeletedEntities(client, syncInventoryItemsResponse);
                                }
                            }
                            catch (Exception ex)
                            {
                                ProcessException(syncInventoryItemsResponse, ex);
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
                    ProcessException(syncInventoryItemsResponse, ex);
                }
                finally
                {
                    backOfficeSessionHandler.EndSession();
                    processContext.TrackPluginComplete();
                }

            }
            catch (Exception ex)
            {
                ProcessException(syncInventoryItemsResponse, ex);
            }


            if (!syncInventoryItemsResponse.Status.Equals(Status.Failure))
            {
                syncInventoryItemsResponse.Status = Status.Success;
            }

            String responsePayload = JsonConvert.SerializeObject(syncInventoryItemsResponse, new DomainMediatorJsonSerializerSettings());
            processContext.ResponseHandler.HandleResponse(requestId, responsePayload);
        }

    }
}
