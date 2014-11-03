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
    /// The SyncInvoiceBalances SyncShared Domain Feature implementation.
    /// </summary>
    [Export(typeof(IDomainFeatureRequest))]
    [FeatureMetadataExport(FeatureMessageTypes.SyncInvoiceBalances, typeof(FeatureDescriptions), "ISyncInvoiceBalances")]
    public class SyncInvoiceBalances : AbstractSyncDomainMediator
    {
        [ImportMany]
#pragma warning disable 649
        private IEnumerable<Lazy<ISyncInvoiceBalances, IBackOfficeData>> _backOfficeHandlers;
#pragma warning restore 649

        /// <summary>
        /// The SyncInvoiceBalances SyncShared Feature Request Implementation. 
        /// </summary>
        /// <param name="processContext">The <see cref="IProcessContext"/></param>
        /// <param name="requestId">The Connector Request Id</param>
        /// <param name="tenantId">The tenant Id making the request.</param>
        /// <param name="backOfficeCompanyConfiguration">The back office company configuration <see cref="IBackOfficeCompanyConfiguration"/></param>
        /// <param name="payload">String representing the payload to process.</param>
        public override void FeatureRequest(IProcessContext processContext, Guid requestId, string tenantId,
          IBackOfficeCompanyConfiguration backOfficeCompanyConfiguration, string payload)
        {
            var syncInvoiceBalancesResponse = new SyncInvoiceBalancesResponse();

            try
            {
                if (backOfficeCompanyConfiguration == null)
                    throw new NoNullAllowedException("Back Office Company Configuration must exist to process feature request.");
                
                CreateCompositionContainer(GetBackOfficePluginPartialPath(backOfficeCompanyConfiguration.BackOfficeId));
                SetupDataStoragePath(backOfficeCompanyConfiguration.DataStoragePath);

                ISyncInvoiceBalances backofficeFeatureProcessor = (from backOfficeHandler in _backOfficeHandlers
                                                                   where backOfficeHandler.Metadata.BackOfficeId.Equals(backOfficeCompanyConfiguration.BackOfficeId,
                                                                   StringComparison.CurrentCultureIgnoreCase)
                                                                   select backOfficeHandler.Value).FirstOrDefault();

                if (backofficeFeatureProcessor == null)
                {
                    Debug.Print("{0} BackOffice to SyncInvoiceBalances was not found!", backOfficeCompanyConfiguration.BackOfficeId);
                    throw new ApplicationException(String.Format("{0} BackOffice to SyncInvoiceBalances was not found!", backOfficeCompanyConfiguration.BackOfficeId));
                }

                if (String.IsNullOrWhiteSpace(payload))
                {
                    throw new ArgumentNullException(String.Format("{0} error: Missing request payload.",
                        FeatureMessageTypes.SyncInvoiceBalances));
                }

                //Request Payload Deserialization
                var payloadSettings = new DomainMediatorJsonSerializerSettings();
                var syncRequest = JsonConvert.DeserializeObject<SyncRequest>(payload, payloadSettings);

                //Initialize response
                var invoiceBalances = new Collection<InvoiceBalance>();
                syncInvoiceBalancesResponse.SyncDigest = new SyncDigest
                {
                    EndpointId = SyncConstants.BackOfficeEndpointId,
                    ResourceKindName = syncRequest.ResourceKindName,
                    EndpointTick = syncRequest.CloudTick
                };
                syncInvoiceBalancesResponse.InvoiceBalances = invoiceBalances;

                // ReSharper disable once SuspiciousTypeConversion.Global
                var backOfficeSessionHandler = backofficeFeatureProcessor as IBackOfficeSessionHandler;

                try
                {
                    processContext.TrackPluginInvoke();
                    BeginBackOfficeSession(processContext.GetSessionContext(), backOfficeSessionHandler, backOfficeCompanyConfiguration, syncInvoiceBalancesResponse);
                    InitializeFeaturePropertyDefaults(backofficeFeatureProcessor, tenantId, syncInvoiceBalancesResponse);
                    CheckResponse(backofficeFeatureProcessor.InitializeSyncInvoiceBalances(), syncInvoiceBalancesResponse);

                    //Loop  through sync logic to get the set of InvoiceBalance payload. 
                    InvoiceBalance invoiceBalance = backofficeFeatureProcessor.GetNextSyncInvoiceBalance();

                    if (invoiceBalance != null)
                    {
                        //because attribute cannot be inherited, we require this check 
                        string idPropertyName = invoiceBalance.GetType().GetExternalIdPropertyName();

                        if (idPropertyName == null)
                        {
                            Debug.Print("Missing External ID attribute");
                            throw new ApplicationException("Missing External ID attribute");
                        }

                        using (var client = new SyncClient(tenantId, GetSyncDataStoragePath()))
                        {
                            try
                            {
                                //Open the sync metadata database
                                //Begin sync for the InvoiceBalances resource kind based on cloud tick
                                client.OpenDatabase();
                                client.BeginSession(syncRequest.ResourceKindName, syncRequest.CloudTick);
                                try
                                {
                                    //Begin and sync
                                    //TODO KMS: Sync type needs to be part of the plugin configuration for the resource.
                                    client.BeginSync(SyncType.Internal);

                                    //Set the response tick to the new tick for this resource endpoint. 
                                    syncInvoiceBalancesResponse.SyncDigest.EndpointTick = client.Tick;
                                    while (invoiceBalance != null)
                                    {
                                        if (!client.SyncEntity(invoiceBalance, idPropertyName).Equals(SyncEntityState.Unchanged))
                                        {
                                            ExternalIdUtilities.ApplyExternalIdEncoding(invoiceBalance);
                                            invoiceBalances.Add(invoiceBalance);

                                            if (invoiceBalances.Count == processContext.EntityChunkCount)
                                            {
                                                try
                                                {
                                                    syncInvoiceBalancesResponse.Status = Status.Success;
                                                    processContext.ResponseHandler.HandleResponse(requestId, JsonConvert.SerializeObject(syncInvoiceBalancesResponse, new DomainMediatorJsonSerializerSettings()), false);
                                                    invoiceBalances.Clear();
                                                }
                                                finally
                                                {
                                                    syncInvoiceBalancesResponse.Status = Status.Indeterminate;
                                                }
                                            }
                                        }
                                        invoiceBalance = backofficeFeatureProcessor.GetNextSyncInvoiceBalance();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ProcessException(syncInvoiceBalancesResponse, ex);
                                }
                                finally
                                {
                                    //end the sync
                                    client.EndSync(true);
                                    ProcessDeletedEntities(client, syncInvoiceBalancesResponse);
                                    
                                }
                            }
                            catch (Exception ex)
                            {
                                ProcessException(syncInvoiceBalancesResponse, ex);
                            }
                            finally
                            {
                                //end the sync session
                                client.EndSession();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    ProcessException(syncInvoiceBalancesResponse, ex);
                }
                finally
                {
                    backOfficeSessionHandler.EndSession();
                    processContext.TrackPluginComplete();
                }
            }
            catch (Exception ex)
            {
                ProcessException(syncInvoiceBalancesResponse, ex);
            }

            if (!syncInvoiceBalancesResponse.Status.Equals(Status.Failure))
            {
                syncInvoiceBalancesResponse.Status = Status.Success;
            }

            //Serialize the payload and send the reponse to the calling application
            String responsePayload = JsonConvert.SerializeObject(syncInvoiceBalancesResponse, new DomainMediatorJsonSerializerSettings());
            processContext.ResponseHandler.HandleResponse(requestId, responsePayload);

        }


    }
}
