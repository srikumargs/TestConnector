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
using Sage.Connector.Payments.Contracts.BackOffice;
using Sage.Connector.Payments.Contracts.CloudIntegration.Responses;
using Payment = Sage.Connector.Payments.Contracts.Data.Payment;

namespace Sage.Connector.Payments.Mediator
{
    /// <summary>
    /// Process payment Domain Mediator Feature Request
    /// </summary>
    [Export(typeof(IDomainFeatureRequest))]
    [FeatureMetadataExport(FeatureMessageTypes.ProcessPayment, typeof(FeatureDescriptions), "IProcessPayment")]
    public class ProcessPayment : AbstractDomainMediator
    {
        [ImportMany]
#pragma warning disable 649
        private System.Collections.Generic.IEnumerable<Lazy<IProcessPayment, IBackOfficeData>> _backOfficeHandlers;
#pragma warning restore 649

        /// <summary>
        /// Feature request for the ProcessPayment sales domain implementation.
        /// </summary>
        /// <param name="processContext">The <see cref="IProcessContext"/></param>
        /// <param name="requestId">The Connector Request Id</param>
        /// <param name="tenantId">The tenant Id making the request.</param>
        /// <param name="backOfficeCompanyConfiguration">The back office company configuration <see cref="IBackOfficeCompanyConfiguration"/></param>
        /// <param name="payload">String representing the payload to process.</param>
        public override void FeatureRequest(IProcessContext processContext, Guid requestId, String tenantId,
            IBackOfficeCompanyConfiguration backOfficeCompanyConfiguration, String payload)
        {
            PaymentResponse paymentResponse = new PaymentResponse();
            try
            {

                if (String.IsNullOrWhiteSpace(payload))
                {
                    throw new ArgumentNullException(String.Format("{0} error: Missing request payload.",
                        FeatureMessageTypes.ProcessPayment));
                }

                //Request Payload Deserialization
                var payment = JsonConvert.DeserializeObject<Payment>(payload);

                var responsePayment = new Contracts.CloudIntegration.Responses.Payment
                {
                    Id = payment.Id,
                };

                paymentResponse.Payment = responsePayment;

                ExternalIdUtilities.ApplyExternalIdDecoding(responsePayment);

                CreateCompositionContainer(GetBackOfficePluginPartialPath(backOfficeCompanyConfiguration.BackOfficeId));
                SetupDataStoragePath(backOfficeCompanyConfiguration.DataStoragePath);

                IProcessPayment backofficeProcessor = (from backOfficeHandler in _backOfficeHandlers
                    where
                        backOfficeHandler.Metadata.BackOfficeId.Equals(backOfficeCompanyConfiguration.BackOfficeId,
                            StringComparison.CurrentCultureIgnoreCase)
                    select backOfficeHandler.Value).FirstOrDefault();

                if (backofficeProcessor == null)
                {
                    Debug.Print("{0} BackOffice to Process the payment was not found!",
                        backOfficeCompanyConfiguration.BackOfficeId);
                    throw new ApplicationException(String.Format(
                        "{0} BackOffice to Process the payment was not found!",
                        backOfficeCompanyConfiguration.BackOfficeId));
                }

                var backOfficeSessionHandler = backofficeProcessor as IBackOfficeSessionHandler;

                try
                {
                    processContext.TrackPluginInvoke();
                    BeginBackOfficeSession(processContext.GetSessionContext(), backOfficeSessionHandler,
                        backOfficeCompanyConfiguration, paymentResponse);
                    InitializeFeaturePropertyDefaults(backofficeProcessor, tenantId, paymentResponse);

                    Response response = backofficeProcessor.ProcessPayment(payment);

                    if (response == null || response.Status.Equals(Status.Indeterminate))
                    {
                        throw new ApplicationException(
                            String.Format("{0} Back Office failed to supply the proper payment response Status.",
                                backOfficeCompanyConfiguration.BackOfficeId));
                    }

                    paymentResponse.Status = response.Status;
                    paymentResponse.Diagnoses = response.Diagnoses;

                    responsePayment.ExternalId = payment.ExternalId;
                    responsePayment.EntityStatus = payment.EntityStatus;
                    responsePayment.ExternalReference = payment.ExternalReference;

                    ExternalIdUtilities.ApplyExternalIdEncoding(responsePayment);

                }
                catch (Exception ex)
                {
                    ProcessException(paymentResponse, ex);
                }
                finally
                {
                    backOfficeSessionHandler.EndSession();
                    processContext.TrackPluginComplete();
                }
            }
            catch (Exception ex)
            {
                ProcessException(paymentResponse, ex);
            }


            String responsePayload = JsonConvert.SerializeObject(paymentResponse);
            processContext.ResponseHandler.HandleResponse(requestId, responsePayload);
        }
    }
}
