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
using Quote = Sage.Connector.Sales.Contracts.Data.Quote;

namespace Sage.Connector.Sales.Mediator
{
    /// <summary>
    /// Process Quote Domain Mediator Feature Request
    /// </summary>
    [Export(typeof(IDomainFeatureRequest))]
    [FeatureMetadataExport(FeatureMessageTypes.ProcessQuote, typeof(FeatureDescriptions), "IProcessQuote")]
    public class ProcessQuote : AbstractDomainMediator
    {

        [ImportMany]
#pragma warning disable 649
        private System.Collections.Generic.IEnumerable<Lazy<IProcessQuote, IBackOfficeData>> _backOfficeHandlers;
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
            QuoteResponse quoteResponse = new QuoteResponse();
            try
            {

                if (String.IsNullOrWhiteSpace(payload))
                {
                    throw new ArgumentNullException(String.Format("{0} error: Missing request payload.",
                        FeatureMessageTypes.ProcessQuote));
                }

                //Request Payload Deserialization
                Quote quote = JsonConvert.DeserializeObject<Quote>(payload);

                //Set the response quote with id as soon as payload is deserialized.
                //This way, in case of error below, the quote with the problem processing can be identified and not stuck in submitted status
                var responseQuote = new Contracts.CloudIntegration.Responses.Quote
                {
                    Id = quote.Id,
                };
                quoteResponse.Quote = responseQuote;

                ExternalIdUtilities.ApplyExternalIdDecoding(quote);

                CreateCompositionContainer(GetBackOfficePluginPartialPath(backOfficeCompanyConfiguration.BackOfficeId));
                SetupDataStoragePath(backOfficeCompanyConfiguration.DataStoragePath);

                IProcessQuote backofficeProcessor = (from backOfficeHandler in _backOfficeHandlers
                                                     where backOfficeHandler.Metadata.BackOfficeId.Equals(backOfficeCompanyConfiguration.BackOfficeId,
                                                     StringComparison.CurrentCultureIgnoreCase)
                                                     select backOfficeHandler.Value).FirstOrDefault();

                if (backofficeProcessor == null)
                {
                    Debug.Print("{0} BackOffice to Process the quote was not found!",
                        backOfficeCompanyConfiguration.BackOfficeId);
                    throw new ApplicationException(String.Format("{0} BackOffice to Process the quote was not found!",
                        backOfficeCompanyConfiguration.BackOfficeId));
                }

                var backOfficeSessionHandler = backofficeProcessor as IBackOfficeSessionHandler;

                try
                {
                    processContext.TrackPluginInvoke();
                    BeginBackOfficeSession(processContext.GetSessionContext(), backOfficeSessionHandler, backOfficeCompanyConfiguration, quoteResponse);
                    InitializeFeaturePropertyDefaults(backofficeProcessor, tenantId, quoteResponse);

                    Response response = backofficeProcessor.ProcessQuote(quote);

                    if (response == null || response.Status.Equals(Status.Indeterminate))
                    {
                        throw new ApplicationException(String.Format("{0} Back Office failed to supply the proper quote response Status.", backOfficeCompanyConfiguration.BackOfficeId));
                    }

                    quoteResponse.Status = response.Status;
                    quoteResponse.Diagnoses = response.Diagnoses; 

                    if (response.Status.Equals(Status.Success))
                    {
                        responseQuote.ExternalId = quote.ExternalId;
                        responseQuote.ExternalReference = quote.ExternalReference;
                        responseQuote.DiscountPercent = quote.DiscountPercent;
                        responseQuote.DocumentTotal = quote.DocumentTotal;
                        responseQuote.ExpiryDate = quote.ExpiryDate;
                        responseQuote.SandH = quote.SandH;
                        responseQuote.SubTotal = quote.SubTotal;
                        responseQuote.SubmittedDate = quote.SubmittedDate;
                        responseQuote.Tax = quote.Tax;
                       

                        foreach (var quoteDetail in quote.Details)
                        {
                            responseQuote.Details.Add(new QuoteDetail
                            {
                                Id = quoteDetail.Id,
                                ExternalId = quoteDetail.ExternalId,
                                ExternalReference = quoteDetail.ExternalReference,
                                EntityStatus = quoteDetail.EntityStatus,
                                Price = quoteDetail.Price
                            });
                        }
                        ExternalIdUtilities.ApplyExternalIdEncoding(responseQuote);
                    }
                }
                catch (Exception ex)
                {
                    ProcessException(quoteResponse, ex);
                }
                finally
                {
                    backOfficeSessionHandler.EndSession();
                    processContext.TrackPluginComplete();
                }
            }
            catch (Exception ex)
            {
                ProcessException(quoteResponse, ex);
            }

            String responsePayload = JsonConvert.SerializeObject(quoteResponse);
            processContext.ResponseHandler.HandleResponse(requestId, responsePayload);
        }



    }
}
