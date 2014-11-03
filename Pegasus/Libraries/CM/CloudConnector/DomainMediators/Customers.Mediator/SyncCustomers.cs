using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using Sage.Connector.Customers.Contracts.BackOffice;
using Sage.Connector.Customers.Contracts.CloudIntegration.Responses;
using Sage.Connector.Customers.Contracts.Data;
using Sage.Connector.DomainContracts.BackOffice;
using Sage.Connector.DomainContracts.Data.Metadata;
using Sage.Connector.DomainContracts.Responses;
using Sage.Connector.DomainMediator.Core;
using Sage.Connector.DomainMediator.Core.JsonConverters;
using Sage.Connector.DomainMediator.Core.Utilities;
using Sage.Connector.Sync;
using Sage.Connector.Sync.Contracts.CloudIntegration;
using Sage.Connector.Sync.Contracts.CloudIntegration.Requests;
using Sage.Connector.Sync.Contracts.CloudIntegration.Responses;
using Sage.Connector.Sync.Contracts.Data;
using Sage.Connector.Sync.Interfaces;

namespace Sage.Connector.Customers.Mediator
{
    /// <summary>
    /// The SyncCustomers SyncShared Domain Feature implementation.
    /// </summary>
    [Export(typeof(IDomainFeatureRequest))]
    [FeatureMetadataExport(FeatureMessageTypes.SyncCustomers, typeof(FeatureDescriptions), "ISyncCustomers")]
    public class SyncCustomers : AbstractSyncDomainMediator
    {
        [ImportMany]
#pragma warning disable 649
        private IEnumerable<Lazy<ISyncCustomers, IBackOfficeData>> _backOfficeHandlers;
#pragma warning restore 649

        /// <summary>
        /// The SyncCustomers SyncShared Feature Request Implementation. 
        /// </summary>
        /// <param name="processContext">The <see cref="IProcessContext"/></param>
        /// <param name="requestId">The Connector Request Id</param>
        /// <param name="tenantId">The tenant Id making the request.</param>
        /// <param name="backOfficeCompanyConfiguration">The back office company configuration <see cref="IBackOfficeCompanyConfiguration"/></param>
        /// <param name="payload">String representing the payload to process.</param>
        public override void FeatureRequest(IProcessContext processContext, Guid requestId, string tenantId,
          IBackOfficeCompanyConfiguration backOfficeCompanyConfiguration, string payload)
        {
            var syncCustomersResponse = new SyncCustomersResponse();

            try
            {
                if (backOfficeCompanyConfiguration == null)
                    throw new NoNullAllowedException("Back Office Company Configuration must exist to process feature request.");

                CreateCompositionContainer(GetBackOfficePluginPartialPath(backOfficeCompanyConfiguration.BackOfficeId));
                SetupDataStoragePath(backOfficeCompanyConfiguration.DataStoragePath);

                ISyncCustomers backofficeFeatureProcessor = (from backOfficeHandler in _backOfficeHandlers
                                                             where backOfficeHandler.Metadata.BackOfficeId.Equals(backOfficeCompanyConfiguration.BackOfficeId,
                                                             StringComparison.CurrentCultureIgnoreCase)
                                                             select backOfficeHandler.Value).FirstOrDefault();

                if (backofficeFeatureProcessor == null)
                {
                    Debug.Print("{0} BackOffice to SyncCustomers was not found!", backOfficeCompanyConfiguration.BackOfficeId);
                    throw new ApplicationException(String.Format("{0} BackOffice to SyncCustomers was not found!", backOfficeCompanyConfiguration.BackOfficeId));
                }

                if (String.IsNullOrWhiteSpace(payload))
                {
                    throw new ArgumentNullException(String.Format("{0} error: Missing request payload.",
                        FeatureMessageTypes.SyncCustomers));
                }

                //Request Payload Deserialization
                var payloadSettings = new DomainMediatorJsonSerializerSettings();
                var syncRequest = JsonConvert.DeserializeObject<SyncRequest>(payload, payloadSettings);

                //Initialize response
                var customers = new Collection<Customer>();
                syncCustomersResponse.SyncDigest = new SyncDigest
                {
                    EndpointId = SyncConstants.BackOfficeEndpointId,
                    ResourceKindName = syncRequest.ResourceKindName,
                    EndpointTick = syncRequest.CloudTick
                };
                syncCustomersResponse.Customers = customers;

                // ReSharper disable once SuspiciousTypeConversion.Global
                var backOfficeSessionHandler = backofficeFeatureProcessor as IBackOfficeSessionHandler;

                try
                {
                    processContext.TrackPluginInvoke();
                    BeginBackOfficeSession(processContext.GetSessionContext(), backOfficeSessionHandler, backOfficeCompanyConfiguration, syncCustomersResponse);
                    InitializeFeaturePropertyDefaults(backofficeFeatureProcessor, tenantId, syncCustomersResponse);
                    CheckResponse(backofficeFeatureProcessor.InitializeSyncCustomers(), syncCustomersResponse);

                        //Loop  through sync logic to get the set of customer payload. 
                        Customer customer = backofficeFeatureProcessor.GetNextSyncCustomer();

                        if (customer != null)
                        {
                            //because attribute cannot be inherited, we require this check 
                            string idPropertyName = GetExternalIdPropertyName(customer.GetType());

                            using (var client = new SyncClient(tenantId, GetSyncDataStoragePath()))
                            {
                                try
                                {
                                    //Open the sync metadata database
                                    //Begin sync for the customers resource kind based on cloud tick
                                    client.OpenDatabase();
                                    client.BeginSession(syncRequest.ResourceKindName, syncRequest.CloudTick);
                                    try
                                    {
                                        //Begin and sync
                                        //TODO KMS: Sync type needs to be part of the plugin configuration for the resource.
                                        client.BeginSync(SyncType.Internal);

                                        //Set the response tick to the new tick for this resource endpoint. 
                                        syncCustomersResponse.SyncDigest.EndpointTick = client.Tick;
                                        while (customer != null)
                                        {
                                            if (!client.SyncEntity(customer, idPropertyName).Equals(SyncEntityState.Unchanged))
                                            {
                                                ExternalIdUtilities.ApplyExternalIdEncoding(customer);
                                                customers.Add(customer);

                                                if (customers.Count == processContext.EntityChunkCount)
                                                {
                                                    try
                                                    {
                                                        syncCustomersResponse.Status = Status.Success;
                                                        processContext.ResponseHandler.HandleResponse(requestId, JsonConvert.SerializeObject(syncCustomersResponse, new DomainMediatorJsonSerializerSettings()), false);
                                                        customers.Clear();
                                                    }
                                                    finally
                                                    {
                                                        syncCustomersResponse.Status = Status.Indeterminate;
                                                    }
                                                }

                                            }
                                            customer = backofficeFeatureProcessor.GetNextSyncCustomer();
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        ProcessException(syncCustomersResponse, ex);
                                    }
                                    finally
                                    {
                                        //end the sync
                                        client.EndSync(true);
                                        ProcessDeletedEntities(client, syncCustomersResponse);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ProcessException(syncCustomersResponse, ex);
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
                    ProcessException(syncCustomersResponse, ex);
                }
                finally
                {
                    backOfficeSessionHandler.EndSession();
                    processContext.TrackPluginComplete();
                }
            }
            catch (Exception ex)
            {
                ProcessException(syncCustomersResponse, ex);
            }

            if (!syncCustomersResponse.Status.Equals(Status.Failure))
            {
                syncCustomersResponse.Status = Status.Success;
            }

            //Serialize the payload and send the reponse to the calling application
            String responsePayload = JsonConvert.SerializeObject(syncCustomersResponse, new DomainMediatorJsonSerializerSettings());
            processContext.ResponseHandler.HandleResponse(requestId, responsePayload);

        }



    }
}
