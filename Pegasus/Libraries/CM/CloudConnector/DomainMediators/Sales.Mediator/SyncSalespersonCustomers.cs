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
using Sage.Connector.Sales.Contracts.BackOffice;
using Sage.Connector.Sales.Contracts.CloudIntegration.Responses;
using Sage.Connector.Sales.Contracts.Data;
using Sage.Connector.Sync;
using Sage.Connector.Sync.Contracts.CloudIntegration;
using Sage.Connector.Sync.Contracts.CloudIntegration.Requests;
using Sage.Connector.Sync.Contracts.Data;
using Sage.Connector.Sync.Interfaces;

namespace Sage.Connector.Sales.Mediator
{
    /// <summary>
    /// The SyncSalespersons SyncShared Domain Feature implementation.
    /// </summary>
    [Export(typeof(IDomainFeatureRequest))]
    [FeatureMetadataExport(FeatureMessageTypes.SyncSalespersonCustomers, typeof(FeatureDescriptions), "ISyncSalespersonCustomers")]
    public class SyncSalespersonCustomers : AbstractSyncDomainMediator
    {
        [ImportMany]
#pragma warning disable 649
        private IEnumerable<Lazy<ISyncSalespersonCustomers, IBackOfficeData>> _backOfficeHandlers;
#pragma warning restore 649

        /// <summary>
        /// The SyncSalespersons SyncShared Feature Request Implementation. 
        /// </summary>
        /// <param name="processContext">The <see cref="IProcessContext"/></param>
        /// <param name="requestId">The Connector Request Id</param>
        /// <param name="tenantId">The tenant Id making the request.</param>
        /// <param name="backOfficeCompanyConfiguration">The back office company configuration <see cref="IBackOfficeCompanyConfiguration"/></param>
        /// <param name="payload">String representing the payload to process.</param>
        public override void FeatureRequest(IProcessContext processContext, Guid requestId, string tenantId,
          IBackOfficeCompanyConfiguration backOfficeCompanyConfiguration, string payload)
        {
            var syncSalespersonCustomersResponse = new SyncSalespersonCustomersResponse();

            try
            {
                if (backOfficeCompanyConfiguration == null)
                    throw new NoNullAllowedException("Back Office Company Configuration must exist to process feature request.");

                CreateCompositionContainer(GetBackOfficePluginPartialPath(backOfficeCompanyConfiguration.BackOfficeId));
                SetupDataStoragePath(backOfficeCompanyConfiguration.DataStoragePath);

                ISyncSalespersonCustomers backofficeFeatureProcessor = (from backOfficeHandler in _backOfficeHandlers
                                                                        where backOfficeHandler.Metadata.BackOfficeId.Equals(backOfficeCompanyConfiguration.BackOfficeId,
                                                                        StringComparison.CurrentCultureIgnoreCase)
                                                                        select backOfficeHandler.Value).FirstOrDefault();

                if (backofficeFeatureProcessor == null)
                {
                    Debug.Print("{0} BackOffice to SyncSalespersons was not found!", backOfficeCompanyConfiguration.BackOfficeId);
                    throw new ApplicationException(String.Format("{0} BackOffice to SyncSalespersons was not found!", backOfficeCompanyConfiguration.BackOfficeId));
                }

                if (String.IsNullOrWhiteSpace(payload))
                {
                    throw new ArgumentNullException(String.Format("{0} error: Missing request payload.",
                        FeatureMessageTypes.SyncSalespersons));
                }

                //Request Payload Deserialization
                var payloadSettings = new DomainMediatorJsonSerializerSettings();
                var syncRequest = JsonConvert.DeserializeObject<SyncRequest>(payload, payloadSettings);

                //Initialize response
                var salespersonCustomers = new Collection<SalespersonCustomer>();
                syncSalespersonCustomersResponse.SyncDigest = new SyncDigest
                {
                    EndpointId = SyncConstants.BackOfficeEndpointId,
                    ResourceKindName = syncRequest.ResourceKindName,
                    EndpointTick = syncRequest.CloudTick
                };
                syncSalespersonCustomersResponse.SalespersonCustomers = salespersonCustomers;

                // ReSharper disable once SuspiciousTypeConversion.Global
                var backOfficeSessionHandler = backofficeFeatureProcessor as IBackOfficeSessionHandler;

                try
                {
                    processContext.TrackPluginInvoke();
                    BeginBackOfficeSession(processContext.GetSessionContext(), backOfficeSessionHandler, backOfficeCompanyConfiguration, syncSalespersonCustomersResponse);
                    InitializeFeaturePropertyDefaults(backofficeFeatureProcessor, tenantId, syncSalespersonCustomersResponse);
                    CheckResponse(backofficeFeatureProcessor.InitializeSyncSalespersonCustomers(), syncSalespersonCustomersResponse);


                    //Loop  through sync logic to get the set of Salesperson payload. 
                    SalespersonCustomer salespersonCustomer = backofficeFeatureProcessor.GetNextSyncSalespersonCustomer();

                    if (salespersonCustomer != null)
                    {
                        //because attribute cannot be inherited, we require this check 
                        string idPropertyName = GetExternalIdPropertyName(salespersonCustomer.GetType());

                        using (var client = new SyncClient(tenantId, GetSyncDataStoragePath()))
                        {
                            try
                            {
                                //Open the sync metadata database
                                //Begin sync for the Salespersons resource kind based on cloud tick
                                client.OpenDatabase();
                                client.BeginSession(syncRequest.ResourceKindName, syncRequest.CloudTick);
                                try
                                {
                                    //Begin and sync
                                    //TODO KMS: Sync type needs to be part of the plugin configuration for the resource.
                                    client.BeginSync(SyncType.Internal);

                                    //Set the response tick to the new tick for this resource endpoint. 
                                    syncSalespersonCustomersResponse.SyncDigest.EndpointTick = client.Tick;
                                    while (salespersonCustomer != null)
                                    {
                                        if (!client.SyncEntity(salespersonCustomer, idPropertyName).Equals(SyncEntityState.Unchanged))
                                        {
                                            ExternalIdUtilities.ApplyExternalIdEncoding(salespersonCustomer);
                                            salespersonCustomers.Add(salespersonCustomer);

                                            if (salespersonCustomers.Count == processContext.EntityChunkCount)
                                            {
                                                try
                                                {
                                                    syncSalespersonCustomersResponse.Status = Status.Success;
                                                    processContext.ResponseHandler.HandleResponse(requestId, JsonConvert.SerializeObject(syncSalespersonCustomersResponse, new DomainMediatorJsonSerializerSettings()), false);
                                                    salespersonCustomers.Clear();
                                                }
                                                finally
                                                {
                                                    syncSalespersonCustomersResponse.Status = Status.Indeterminate;
                                                }
                                            }
                                        }
                                        salespersonCustomer = backofficeFeatureProcessor.GetNextSyncSalespersonCustomer();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ProcessException(syncSalespersonCustomersResponse, ex);
                                }
                                finally
                                {
                                    //end the sync
                                    client.EndSync(true);
                                    ProcessDeletedEntities(client, syncSalespersonCustomersResponse);
                                }
                            }
                            catch (Exception ex)
                            {
                                ProcessException(syncSalespersonCustomersResponse, ex);
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
                    ProcessException(syncSalespersonCustomersResponse, ex);
                }
                finally
                {
                    backOfficeSessionHandler.EndSession();
                    processContext.TrackPluginComplete();
                }
            }
            catch (Exception ex)
            {
                ProcessException(syncSalespersonCustomersResponse, ex);
            }

            if (!syncSalespersonCustomersResponse.Status.Equals(Status.Failure))
            {
                syncSalespersonCustomersResponse.Status = Status.Success;
            }

            //Serialize the payload and send the reponse to the calling application
            String responsePayload = JsonConvert.SerializeObject(syncSalespersonCustomersResponse, new DomainMediatorJsonSerializerSettings());
            processContext.ResponseHandler.HandleResponse(requestId, responsePayload);

        }


    }
}
