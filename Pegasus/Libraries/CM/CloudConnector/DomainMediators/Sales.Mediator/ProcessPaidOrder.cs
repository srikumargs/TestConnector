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
using Sage.Connector.Sales.Contracts.BackOffice;
using Sage.Connector.Sales.Contracts.CloudIntegration.Responses;
using PaidOrder = Sage.Connector.Sales.Contracts.Data.PaidOrder;

namespace Sage.Connector.Sales.Mediator
{
    /// <summary>
    /// Process Paid Order Domain Mediator Feature Request
    /// </summary>
    [Export(typeof(IDomainFeatureRequest))]
    [FeatureMetadataExport(FeatureMessageTypes.ProcessPaidOrder, typeof(FeatureDescriptions), "IProcessPaidOrder")]
    public class ProcessPaidOrder : AbstractDomainMediator
    {
        [ImportMany]
#pragma warning disable 649
        private System.Collections.Generic.IEnumerable<Lazy<IProcessPaidOrder, IBackOfficeData>> _backOfficeHandlers;
#pragma warning restore 649

        /// <summary>
        /// Feature request for the ProcessPaidOrder sales domain implementation.
        /// </summary>
        /// <param name="processContext">The <see cref="IProcessContext"/></param>
        /// <param name="requestId">The Connector Request Id</param>
        /// <param name="tenantId">The tenant Id making the request.</param>
        /// <param name="backOfficeCompanyConfiguration">The back office company configuration <see cref="IBackOfficeCompanyConfiguration"/></param>
        /// <param name="payload">String representing the payload to process.</param>
        public override void FeatureRequest(IProcessContext processContext, Guid requestId, String tenantId,
            IBackOfficeCompanyConfiguration backOfficeCompanyConfiguration, String payload)
        {
            PaidOrderResponse paidOrderResponse = new PaidOrderResponse();
  
            try
            {
                if (String.IsNullOrWhiteSpace(payload))
                {
                    throw new ArgumentNullException(String.Format("{0} error: Missing request payload.",
                        FeatureMessageTypes.ProcessPaidOrder));
                }

                //Request Payload Deserialization
                PaidOrder paidOrder = JsonConvert.DeserializeObject<PaidOrder>(payload);
                var responsePaidOrder = new Contracts.CloudIntegration.Responses.PaidOrder
                {
                    Id = paidOrder.Id,
                };

                //Set as soon as we have an response object
                paidOrderResponse.PaidOrder = responsePaidOrder; 

                ExternalIdUtilities.ApplyExternalIdDecoding(paidOrder);

                CreateCompositionContainer(GetBackOfficePluginPartialPath(backOfficeCompanyConfiguration.BackOfficeId));
                SetupDataStoragePath(backOfficeCompanyConfiguration.DataStoragePath);
                
                IProcessPaidOrder backofficeProcessor = (from backOfficeHandler in _backOfficeHandlers
                                                         where backOfficeHandler.Metadata.BackOfficeId.Equals(backOfficeCompanyConfiguration.BackOfficeId, StringComparison.CurrentCultureIgnoreCase)
                                                         select backOfficeHandler.Value).FirstOrDefault();

                if (backofficeProcessor == null)
                {
                    Debug.Print("{0} BackOffice to Process the paid order was not found!", backOfficeCompanyConfiguration.BackOfficeId);
                    throw new ApplicationException(String.Format("{0} BackOffice to Process the paid order was not found!", backOfficeCompanyConfiguration.BackOfficeId));
                }
                var backOfficeSessionHandler = backofficeProcessor as IBackOfficeSessionHandler;

                try
                {
                    processContext.TrackPluginInvoke();
                    BeginBackOfficeSession(processContext.GetSessionContext(), backOfficeSessionHandler, backOfficeCompanyConfiguration, paidOrderResponse);
                    InitializeFeaturePropertyDefaults(backofficeProcessor, tenantId, paidOrderResponse);


                    Response response = backofficeProcessor.ProcessPaidOrder(paidOrder);

                    if (response == null || response.Status.Equals(Status.Indeterminate))
                    {
                        throw new ApplicationException(
                            String.Format("{0} Back Office failed to supply the proper paid order response Status.",
                                backOfficeCompanyConfiguration.BackOfficeId));
                    }

                    paidOrderResponse.Status = response.Status;
                    paidOrderResponse.Diagnoses = response.Diagnoses;

                    //Set response paid order values
                    responsePaidOrder.ExternalId = paidOrder.ExternalId;
                    responsePaidOrder.ExternalReference = paidOrder.ExternalReference;
                    responsePaidOrder.SubmittedDate = paidOrder.SubmittedDate;

                    ExternalIdUtilities.ApplyExternalIdEncoding(responsePaidOrder);
                }
                catch (Exception ex)
                {
                    ProcessException(paidOrderResponse, ex);
                }
                finally
                {
                    backOfficeSessionHandler.EndSession();
                    processContext.TrackPluginComplete();
                }
            }
            catch (Exception ex)
            {
                ProcessException(paidOrderResponse, ex);
            }

            String responsePayload = JsonConvert.SerializeObject(paidOrderResponse);
            processContext.ResponseHandler.HandleResponse(requestId, responsePayload);
        }
    }
}
