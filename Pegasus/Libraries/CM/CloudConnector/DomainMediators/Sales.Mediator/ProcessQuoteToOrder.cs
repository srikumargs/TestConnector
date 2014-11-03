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
using QuoteToOrder = Sage.Connector.Sales.Contracts.Data.QuoteToOrder;

namespace Sage.Connector.Sales.Mediator
{
    /// <summary>
    /// Process Quote To Order Domain Mediator Feature Request
    /// </summary>
    [Export(typeof(IDomainFeatureRequest))]
    [FeatureMetadataExport(FeatureMessageTypes.ProcessQuoteToOrder, typeof(FeatureDescriptions), "IProcessQuoteToOrder"
        )]
    public class ProcessQuoteToOrder : AbstractDomainMediator
    {

        [ImportMany]
#pragma warning disable 649
        private System.Collections.Generic.IEnumerable<Lazy<IProcessQuoteToOrder, IBackOfficeData>>
        _backOfficeHandlers;
#pragma warning restore 649


        /// <summary>
        /// Feature request for the ProcessQuote sales domain implementation.
        /// </summary>
        /// <param name="processContext">The <see cref="IProcessContext"/></param>
        /// <param name="requestId">The Connector Request Id</param>
        /// <param name="tenantId">The tenant Id making the request.</param>
        /// <param name="backOfficeCompanyConfiguration">The back office company configuration <see cref="IBackOfficeCompanyConfiguration"/></param>
        /// <param name="payload">String representing the payload to process.</param>
        public override void FeatureRequest(IProcessContext processContext, Guid requestId, String tenantId,
            IBackOfficeCompanyConfiguration backOfficeCompanyConfiguration, string payload)
        {
            QuoteToOrderResponse quoteToOrderResponse = new QuoteToOrderResponse();
            try
            {
                if (String.IsNullOrWhiteSpace(payload))
                {
                    throw new ArgumentNullException(String.Format("{0} error: Missing request payload.",
                        FeatureMessageTypes.ProcessQuoteToOrder));
                }

                //Request Payload Deserialization
                QuoteToOrder quoteToOrder = JsonConvert.DeserializeObject<QuoteToOrder>(payload);

                //Set the response quoteToOrder with id as soon as payload is deserialized.
                //This way, in case of error below, the quote with the problem processing can be identified and not stuck in submitted status
                var responseQuoteToOrder = new Contracts.CloudIntegration.Responses.QuoteToOrder
                {
                    Id = quoteToOrder.Id
                };
                quoteToOrderResponse.QuoteToOrder = responseQuoteToOrder;

                ExternalIdUtilities.ApplyExternalIdDecoding(quoteToOrder);

                CreateCompositionContainer(GetBackOfficePluginPartialPath(backOfficeCompanyConfiguration.BackOfficeId));
                SetupDataStoragePath(backOfficeCompanyConfiguration.DataStoragePath);

                IProcessQuoteToOrder backofficeProcessor = (from backOfficeHandler in _backOfficeHandlers
                                                            where
                                                                backOfficeHandler.Metadata.BackOfficeId.Equals(backOfficeCompanyConfiguration.BackOfficeId,
                                                                    StringComparison.CurrentCultureIgnoreCase)
                                                            select backOfficeHandler.Value).FirstOrDefault();

                if (backofficeProcessor == null)
                {
                    Debug.Print("{0} BackOffice to Process the quote to order was not found!",
                        backOfficeCompanyConfiguration.BackOfficeId);
                    throw new ApplicationException(
                        String.Format("{0} BackOffice to Process the quote to order was not found!",
                            backOfficeCompanyConfiguration.BackOfficeId));
                }

                var backOfficeSessionHandler = backofficeProcessor as IBackOfficeSessionHandler;

                try
                {
                    processContext.TrackPluginInvoke();
                    BeginBackOfficeSession(processContext.GetSessionContext(), backOfficeSessionHandler, backOfficeCompanyConfiguration, quoteToOrderResponse);
                    InitializeFeaturePropertyDefaults(backofficeProcessor, tenantId, quoteToOrderResponse);

                    Response response = backofficeProcessor.ProcessQuoteToOrder(quoteToOrder);
                    if (response == null || response.Status.Equals(Status.Indeterminate))
                    {
                        throw new ApplicationException(String.Format("{0} Back Office failed to supply the proper quote response Status.", backOfficeCompanyConfiguration.BackOfficeId));
                    }

                    quoteToOrderResponse.Status = response.Status;
                    quoteToOrderResponse.Diagnoses = response.Diagnoses;

                    responseQuoteToOrder.ExternalId = quoteToOrder.ExternalId;
                    responseQuoteToOrder.ExternalReference = quoteToOrder.ExternalReference;
                    responseQuoteToOrder.EntityStatus = quoteToOrder.EntityStatus;
                    responseQuoteToOrder.SubmittedDate = quoteToOrder.SubmittedDate;


                    foreach (var quoteToOrderDetail in quoteToOrder.Details)
                    {
                        responseQuoteToOrder.Details.Add(new QuoteToOrderDetail
                        {
                            Id = quoteToOrderDetail.Id,
                            ExternalId = quoteToOrderDetail.ExternalId,
                            ExternalReference = quoteToOrderDetail.ExternalReference,
                            EntityStatus = quoteToOrderDetail.EntityStatus
                        });
                    }

                    ExternalIdUtilities.ApplyExternalIdEncoding(responseQuoteToOrder);
                }
                catch (Exception ex)
                {
                    ProcessException(quoteToOrderResponse, ex);
                }
                finally
                {
                    backOfficeSessionHandler.EndSession();
                    processContext.TrackPluginComplete();
                }
            }
            catch (Exception ex)
            {
                ProcessException(quoteToOrderResponse, ex);
            }

            String responsePayload = JsonConvert.SerializeObject(quoteToOrderResponse);
            processContext.ResponseHandler.HandleResponse(requestId, responsePayload);

        }
    }
}
