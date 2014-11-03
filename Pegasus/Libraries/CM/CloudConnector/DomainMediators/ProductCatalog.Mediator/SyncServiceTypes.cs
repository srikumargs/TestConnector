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
    /// The SyncServiceTypes Feature implementation.
    /// </summary>
    [Export(typeof(IDomainFeatureRequest))]
    [FeatureMetadataExport(FeatureMessageTypes.SyncServiceTypes, typeof(FeatureDescriptions), "ISyncServiceTypes")]
    public class SyncServiceTypes : AbstractSyncDomainMediator
    {
        [ImportMany]
#pragma warning disable 649
        private IEnumerable<Lazy<ISyncServiceTypes, IBackOfficeData>> _backOfficeHandlers;
#pragma warning restore 649
        /// <summary>
        /// The SyncServiceTypes SyncShared Feature Request Implementation. 
        /// </summary>
        /// <param name="processContext">The <see cref="IProcessContext"/></param>
        /// <param name="requestId">The Connector Request Id</param>
        /// <param name="tenantId">The tenant Id making the request.</param>
        /// <param name="backOfficeCompanyConfiguration">The back office company configuration <see cref="IBackOfficeCompanyConfiguration"/></param>
        /// <param name="payload">String representing the payload to process.</param>
        public override void FeatureRequest(IProcessContext processContext, Guid requestId, String tenantId,
            IBackOfficeCompanyConfiguration backOfficeCompanyConfiguration, string payload)
        {
            var syncServiceTypesResponse = new SyncServiceTypesResponse();

            try
            {
                if (backOfficeCompanyConfiguration == null)
                    throw new NoNullAllowedException(
                        "Back Office Company Configuration must exist to process feature request.");

                CreateCompositionContainer(GetBackOfficePluginPartialPath(backOfficeCompanyConfiguration.BackOfficeId));
                SetupDataStoragePath(backOfficeCompanyConfiguration.DataStoragePath);

                ISyncServiceTypes backofficeFeatureProcessor = (from backOfficeHandler in _backOfficeHandlers
                                                                where backOfficeHandler.Metadata.BackOfficeId.Equals(backOfficeCompanyConfiguration.BackOfficeId,
                                                                    StringComparison.CurrentCultureIgnoreCase)
                                                                select backOfficeHandler.Value).FirstOrDefault();

                if (backofficeFeatureProcessor == null)
                {
                    Debug.Print("{0} BackOffice to SyncServiceTypes was not found!",
                        backOfficeCompanyConfiguration.BackOfficeId);
                    throw new ApplicationException(String.Format("{0} BackOffice to SyncServiceTypes was not found!",
                        backOfficeCompanyConfiguration.BackOfficeId));
                }

                if (String.IsNullOrWhiteSpace(payload))
                {
                    throw new ArgumentNullException(String.Format("{0} error: Missing request payload.",
                        FeatureMessageTypes.SyncServiceTypes));
                }

                //SyncRequest Payload Deserialization
                var payloadSettings = new DomainMediatorJsonSerializerSettings();
                var syncRequest = JsonConvert.DeserializeObject<SyncRequest>(payload, payloadSettings);

                var serviceTypes = new Collection<ServiceType>();
                syncServiceTypesResponse.SyncDigest = new SyncDigest
                {
                    EndpointId = SyncConstants.BackOfficeEndpointId,
                    ResourceKindName = syncRequest.ResourceKindName,
                    EndpointTick = syncRequest.CloudTick
                };
                syncServiceTypesResponse.ServiceTypes = serviceTypes;


                var backOfficeSessionHandler = backofficeFeatureProcessor as IBackOfficeSessionHandler;

                try
                {
                    processContext.TrackPluginInvoke();
                    BeginBackOfficeSession(processContext.GetSessionContext(), backOfficeSessionHandler, backOfficeCompanyConfiguration, syncServiceTypesResponse);
                    InitializeFeaturePropertyDefaults(backofficeFeatureProcessor, tenantId, syncServiceTypesResponse);
                    CheckResponse(backofficeFeatureProcessor.InitializeSyncServiceTypes(), syncServiceTypesResponse);

                    ServiceType serviceType = backofficeFeatureProcessor.GetNextSyncServiceType();

                    if (serviceType != null)
                    {
                        //because attribute cannot be inherited, we require this check 
                        string idPropertyName = GetExternalIdPropertyName(serviceType.GetType());

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
                                    syncServiceTypesResponse.SyncDigest.EndpointTick = client.Tick;
                                    while (serviceType != null)
                                    {
                                        if (!client.SyncEntity(serviceType, idPropertyName).Equals(SyncEntityState.Unchanged))
                                        {
                                            ExternalIdUtilities.ApplyExternalIdEncoding(serviceType);
                                            serviceTypes.Add(serviceType);

                                            if (serviceTypes.Count == processContext.EntityChunkCount)
                                            {
                                                try
                                                {
                                                    syncServiceTypesResponse.Status = Status.Success;
                                                    processContext.ResponseHandler.HandleResponse(requestId, JsonConvert.SerializeObject(syncServiceTypesResponse, new DomainMediatorJsonSerializerSettings()), false);
                                                    serviceTypes.Clear();
                                                }
                                                finally
                                                {
                                                    syncServiceTypesResponse.Status = Status.Indeterminate;
                                                }
                                            }

                                        }
                                        serviceType = backofficeFeatureProcessor.GetNextSyncServiceType();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ProcessException(syncServiceTypesResponse, ex);
                                }
                                finally
                                {
                                    client.EndSync(true);
                                    ProcessDeletedEntities(client, syncServiceTypesResponse);
                                }
                            }
                            catch (Exception ex)
                            {
                                ProcessException(syncServiceTypesResponse, ex);
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
                    ProcessException(syncServiceTypesResponse, ex);
                }
                finally
                {
                    backOfficeSessionHandler.EndSession();
                    processContext.TrackPluginComplete();
                }

            }
            catch (Exception ex)
            {
                ProcessException(syncServiceTypesResponse, ex);
            }

            if (!syncServiceTypesResponse.Status.Equals(Status.Failure))
            {
                syncServiceTypesResponse.Status = Status.Success;
            }

            String responsePayload = JsonConvert.SerializeObject(syncServiceTypesResponse, new DomainMediatorJsonSerializerSettings());
            processContext.ResponseHandler.HandleResponse(requestId, responsePayload);
        }



    }
}
