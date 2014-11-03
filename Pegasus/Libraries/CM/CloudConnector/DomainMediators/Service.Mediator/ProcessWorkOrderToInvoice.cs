using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using Sage.Connector.DomainContracts.BackOffice;
using Sage.Connector.DomainContracts.Data.Metadata;
using Sage.Connector.DomainContracts.Responses;
using Sage.Connector.DomainMediator.Core;
using Sage.Connector.DomainMediator.Core.Utilities;
using Sage.Connector.Service.Contracts.BackOffice;
using Sage.Connector.Service.Contracts.CloudIntegration.Responses;

namespace Sage.Connector.Service.Mediator
{
    /// <summary>
    /// Process Work Order to Invoice Service Domain Mediator Feature Request
    /// </summary>
    [Export(typeof(IDomainFeatureRequest))]
    [FeatureMetadataExport(FeatureMessageTypes.ProcessWorkOrderToInvoice, typeof(FeatureDescriptions), "IProcessWorkOrderToInvoice"
        )]
    public class ProcessQuoteToOrder : AbstractDomainMediator
    {

        [ImportMany]
#pragma warning disable 649
        private System.Collections.Generic.IEnumerable<Lazy<IProcessWorkOrderToInvoice, IBackOfficeData>>
        _backOfficeHandlers;
#pragma warning restore 649


        /// <summary>
        /// Feature request for the ProcessWorkOrderToInvoice service domain implementation.
        /// </summary>
        /// <param name="processContext">The <see cref="IProcessContext"/></param>
        /// <param name="requestId">The Connector Request Id</param>
        /// <param name="tenantId">The tenant Id making the request.</param>
        /// <param name="backOfficeCompanyConfiguration">The back office company configuration <see cref="IBackOfficeCompanyConfiguration"/></param>
        /// <param name="payload">String representing the payload to process.</param>
        public override void FeatureRequest(IProcessContext processContext, Guid requestId, String tenantId,
            IBackOfficeCompanyConfiguration backOfficeCompanyConfiguration, string payload)
        {
            WorkOrderToInvoiceResponse workOrderToInvoiceResponse = new WorkOrderToInvoiceResponse();
            try
            {
                if (String.IsNullOrWhiteSpace(payload))
                {
                    throw new ArgumentNullException(String.Format("{0} error: Missing request payload.",
                        FeatureMessageTypes.ProcessWorkOrderToInvoice));
                }

                //Request Payload Deserialization
                Contracts.Data.WorkOrder workOrder = JsonConvert.DeserializeObject<Contracts.Data.WorkOrder>(payload);

                var responseWorkOrder = new WorkOrder
                {
                    Id = workOrder.Id,
                };
                workOrderToInvoiceResponse.WorkOrder = responseWorkOrder; 

                CreateCompositionContainer(GetBackOfficePluginPartialPath(backOfficeCompanyConfiguration.BackOfficeId));
                SetupDataStoragePath(backOfficeCompanyConfiguration.DataStoragePath);

                IProcessWorkOrderToInvoice backofficeProcessor = (from backOfficeHandler in _backOfficeHandlers
                                                                  where
                                                                      backOfficeHandler.Metadata.BackOfficeId.Equals(backOfficeCompanyConfiguration.BackOfficeId,
                                                                          StringComparison.CurrentCultureIgnoreCase)
                                                                  select backOfficeHandler.Value).FirstOrDefault();

                if (backofficeProcessor == null)
                {
                    Debug.Print("{0} BackOffice to Process the work order to invoice was not found!",
                        backOfficeCompanyConfiguration.BackOfficeId);
                    throw new ApplicationException(
                        String.Format("{0} BackOffice to Process the work order to invoice was not found!",
                            backOfficeCompanyConfiguration.BackOfficeId));
                }

                // ReSharper disable once SuspiciousTypeConversion.Global
                var backOfficeSessionHandler = backofficeProcessor as IBackOfficeSessionHandler;

                try
                {
                    processContext.TrackPluginInvoke();
                    BeginBackOfficeSession(processContext.GetSessionContext(), backOfficeSessionHandler, backOfficeCompanyConfiguration, workOrderToInvoiceResponse);
                    InitializeFeaturePropertyDefaults(backofficeProcessor, tenantId, workOrderToInvoiceResponse);

                    ExternalIdUtilities.ApplyExternalIdDecoding(workOrder);

                    var processWorkOrderResponse = backofficeProcessor.ProcessWorkOrder(workOrder);
                    if (processWorkOrderResponse == null || processWorkOrderResponse.Status.Equals(Status.Indeterminate))
                    {
                        throw new ApplicationException(
                            String.Format(
                                "{0} Back Office failed to supply the proper work order to invoice response Status.",
                                backOfficeCompanyConfiguration.BackOfficeId));
                    }

                    workOrderToInvoiceResponse.Status = processWorkOrderResponse.Status;
                    workOrderToInvoiceResponse.Diagnoses = processWorkOrderResponse.Diagnoses;
                    responseWorkOrder.DocumentReference = processWorkOrderResponse.DocumentReference;

                    ExternalIdUtilities.ApplyExternalIdEncoding(responseWorkOrder);

                }
                catch (Exception ex)
                {
                    ProcessException(workOrderToInvoiceResponse, ex);
                }
                finally
                {
                    backOfficeSessionHandler.EndSession();
                    processContext.TrackPluginComplete();
                }
            }
            catch (Exception ex)
            {
                ProcessException(workOrderToInvoiceResponse, ex);
            }

            String responsePayload = JsonConvert.SerializeObject(workOrderToInvoiceResponse);
            processContext.ResponseHandler.HandleResponse(requestId, responsePayload);

        }
    }
}
