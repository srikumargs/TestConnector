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
using Sage.Connector.Invoices.Contracts.BackOffice;
using Sage.Connector.Invoices.Contracts.CloudIntegration.Responses;
using Sage.Connector.Invoices.Contracts.Data;
using Sage.Connector.Sync;
using Sage.Connector.Sync.Contracts.CloudIntegration;
using Sage.Connector.Sync.Contracts.CloudIntegration.Requests;
using Sage.Connector.Sync.Contracts.Data;
using Sage.Connector.Sync.Interfaces;

namespace Sage.Connector.Invoices.Mediator
{
    /// <summary>
    /// The SyncInvoices SyncShared Domain Feature implementation.
    /// </summary>
    [Export(typeof(IDomainFeatureRequest))]
    [FeatureMetadataExport(FeatureMessageTypes.SyncInvoices, typeof(FeatureDescriptions), "ISyncInvoices")]

    /*NOTE:  
     * Because Invoices is Historical, we are not processing the deleted invoices.  
     * This means there is no compelling reason to derive from the AbstrctSyncDomainMediator
    */
    public class SyncInvoices : AbstractDomainMediator
    {
        [ImportMany]
#pragma warning disable 649
        private IEnumerable<Lazy<ISyncInvoices, IBackOfficeData>> _backOfficeHandlers;
#pragma warning restore 649

        /// <summary>
        /// The SyncInvoices SyncShared Feature Request Implementation. 
        /// </summary>
        /// <param name="processContext">The <see cref="IProcessContext"/></param>
        /// <param name="requestId">The Connector Request Id</param>
        /// <param name="tenantId">The tenant Id making the request.</param>
        /// <param name="backOfficeCompanyConfiguration">The back office company configuration <see cref="IBackOfficeCompanyConfiguration"/></param>
        /// <param name="payload">String representing the payload to process.</param>
        public override void FeatureRequest(IProcessContext processContext, Guid requestId, string tenantId,
          IBackOfficeCompanyConfiguration backOfficeCompanyConfiguration, string payload)
        {
            var syncInvoicesResponse = new SyncInvoicesResponse();

            try
            {
                if (backOfficeCompanyConfiguration == null)
                    throw new NoNullAllowedException(
                        "Back Office Company Configuration must exist to process feature request.");

                CreateCompositionContainer(GetBackOfficePluginPartialPath(backOfficeCompanyConfiguration.BackOfficeId));
                SetupDataStoragePath(backOfficeCompanyConfiguration.DataStoragePath);

                ISyncInvoices backofficeFeatureProcessor = (from backOfficeHandler in _backOfficeHandlers
                                                            where backOfficeHandler.Metadata.BackOfficeId.Equals(backOfficeCompanyConfiguration.BackOfficeId,
                                                            StringComparison.CurrentCultureIgnoreCase)
                                                            select backOfficeHandler.Value).FirstOrDefault();

                if (backofficeFeatureProcessor == null)
                {
                    Debug.Print("{0} BackOffice to SyncInvoices was not found!",
                        backOfficeCompanyConfiguration.BackOfficeId);
                    throw new ApplicationException(String.Format("{0} BackOffice to SyncInvoices was not found!",
                        backOfficeCompanyConfiguration.BackOfficeId));
                }

                if (String.IsNullOrWhiteSpace(payload))
                {
                    throw new ArgumentNullException(String.Format("{0} error: Missing request payload.",
                        FeatureMessageTypes.SyncInvoices));
                }

                //Request Payload Deserialization
                var payloadSettings = new DomainMediatorJsonSerializerSettings();
                var syncRequest = JsonConvert.DeserializeObject<SyncRequest>(payload, payloadSettings);

                //Initialize response
                var invoices = new Collection<Invoice>();
                syncInvoicesResponse.SyncDigest = new SyncDigest
                {
                    EndpointId = SyncConstants.BackOfficeEndpointId,
                    ResourceKindName = syncRequest.ResourceKindName,
                    EndpointTick = syncRequest.CloudTick
                };
                syncInvoicesResponse.Invoices = invoices;

                // ReSharper disable once SuspiciousTypeConversion.Global
                var backOfficeSessionHandler = backofficeFeatureProcessor as IBackOfficeSessionHandler;

                try
                {
                    processContext.TrackPluginInvoke();
                    BeginBackOfficeSession(processContext.GetSessionContext(), backOfficeSessionHandler, backOfficeCompanyConfiguration, syncInvoicesResponse);
                    InitializeFeaturePropertyDefaults(backofficeFeatureProcessor, tenantId, syncInvoicesResponse);
                    //Because of historical sales history processing allows for a property bag that can maintain state
                    //the call to the initialize for sync invoices is performed after we know the tick values and 
                    //whether the last response was successful.

                    using (var client = new SyncClient(tenantId, GetSyncDataStoragePath()))
                    {
                        try
                        {
                            //Open the sync metadata database
                            //Begin sync for the Invoices resource kind based on cloud tick
                            client.OpenDatabase();

                            using (var processingPropBag = GetSyncProcessingPropBag(client.Tick, syncRequest.CloudTick, tenantId))
                            {
                                CheckResponse(backofficeFeatureProcessor.InitializeSyncInvoices(processingPropBag), syncInvoicesResponse);

                                client.BeginSession(syncRequest.ResourceKindName, syncRequest.CloudTick);
                                try
                                {
                                    //Begin and sync
                                    //TODO KMS: Sync type needs to be part of the plugin configuration for the resource.
                                    client.BeginSync(SyncType.Internal);

                                    //Set the response tick to the new tick for this resource endpoint. 
                                    syncInvoicesResponse.SyncDigest.EndpointTick = client.Tick;

                                    //Loop  through sync logic to get the set of Invoice payload. 
                                    Invoice invoice = backofficeFeatureProcessor.GetNextSyncInvoice();
                                    string idPropertyName = null;
                                    if (invoice != null)
                                    {
                                        //because attribute cannot be inherited, we require this check 
                                        idPropertyName = GetExternalIdPropertyName(invoice.GetType());

                                    }
                                    while (invoice != null)
                                    {
                                        if (!client.SyncEntity(invoice, idPropertyName).Equals(SyncEntityState.Unchanged))
                                        {
                                            ExternalIdUtilities.ApplyExternalIdEncoding(invoice);
                                            invoices.Add(invoice);

                                            if (invoices.Count == processContext.EntityChunkCount)
                                            {
                                                try
                                                {
                                                    syncInvoicesResponse.Status = Status.Success;
                                                    processContext.ResponseHandler.HandleResponse(requestId,
                                                        JsonConvert.SerializeObject(syncInvoicesResponse,
                                                            new DomainMediatorJsonSerializerSettings()), false);
                                                    invoices.Clear();
                                                }
                                                finally
                                                {
                                                    syncInvoicesResponse.Status = Status.Indeterminate;
                                                }
                                            }
                                        }
                                        invoice = backofficeFeatureProcessor.GetNextSyncInvoice();
                                    }


                                }
                                catch (Exception ex)
                                {
                                    ProcessException(syncInvoicesResponse, ex);
                                }
                                finally
                                {
                                    //end the sync
                                    client.EndSync(true);

                                    /*NOTE:  
                                     * Because Invoices is Historical, we are not processing the deleted invoices.
                                     * Ignore the client.DeletedEntities.
                                    */

                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ProcessException(syncInvoicesResponse, ex);
                        }
                        finally
                        {
                            //end the sync session
                            client.EndSession();
                        }
                    }
                }

                catch (Exception ex)
                {
                    ProcessException(syncInvoicesResponse, ex);
                }
                finally
                {
                    backOfficeSessionHandler.EndSession();
                    processContext.TrackPluginComplete();
                }
            }
            catch
                (Exception ex)
            {
                ProcessException(syncInvoicesResponse, ex);
            }

            if (!syncInvoicesResponse.Status.Equals(Status.Failure))
            {
                syncInvoicesResponse.Status = Status.Success;
            }


            //Serialize the payload and send the reponse to the calling application
            String responsePayload = JsonConvert.SerializeObject(syncInvoicesResponse, new DomainMediatorJsonSerializerSettings());
            processContext.ResponseHandler.HandleResponse(requestId, responsePayload);

        }


        /// <summary>
        /// Get the storage id for the Processing Property Bag
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        private string GetProcessingPropBagId(string tenantId)
        {

            return String.Format("{0}_{1}_PropBag", tenantId, GetFeatureName());

        }

        /// <summary>
        /// Get the storage id used for the processing property bag containing previous run property values. 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        private string GetProcessingPropBagPreviousId(string tenantId)
        {
            return String.Format("{0}_{1}_PropBagPrevious", tenantId, GetFeatureName());

        }
        private StorageDictionary<string, Object> GetSyncProcessingPropBag(int localTick, int cloudTick, String tenantId)
        {
            string currentPropBagId = GetProcessingPropBagId(tenantId);
            string previousPropBagId = GetProcessingPropBagPreviousId(tenantId);

            var processingPropBag =
                new StorageDictionary<string, object>(GetFeatureDataStoragePath(), currentPropBagId);

            if (localTick > cloudTick)
            {
                //use previous values in this run
                processingPropBag.Clear();

                if (StorageDictionary<String, Object>.Exists(GetFeatureDataStoragePath(),
                    previousPropBagId))
                {
                    using (var processingPropBagPrevious =
                        new StorageDictionary<string, object>(GetFeatureDataStoragePath(),
                            previousPropBagId))
                    {
                        foreach (var prop in processingPropBagPrevious)
                        {
                            processingPropBag.Add(prop.Key, prop.Value);
                        }
                    }
                }
            }
            else
            {
                //Copy values into previous propbag for backup if next sync tick counts don't match.
                using (
                    var processingPropBagPrevious =
                        new StorageDictionary<string, object>(GetFeatureDataStoragePath(),
                            GetProcessingPropBagPreviousId(tenantId)))
                {
                    processingPropBagPrevious.Clear();
                    foreach (var prop in processingPropBag)
                    {
                        processingPropBagPrevious.Add(prop.Key, prop.Value);
                    }
                }
            }
            return processingPropBag;
        }




    }
}
