using Sage.Connector.DomainContracts.Data;
using Sage.Connector.DomainContracts.Responses;
using System;

namespace Sage.Connector.Payments.Contracts.CloudIntegration.Responses
{

    /// <summary>
    /// Process Paymenet Reponse
    /// </summary>
    public class PaymentResponse : Response
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public PaymentResponse()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="response"></param>
        public PaymentResponse(Response response)
        {
            if (response == null)
                return;

            Status = response.Status;
            Diagnoses = response.Diagnoses;
        }

        /// <summary>
        /// Payment
        /// </summary>
        public Payment Payment { get; set; }
    }

    /// <summary>
    /// Payment for PaymentResponse
    /// </summary>
    public class Payment : AbstractEntityInformation
    {
        /// <summary>
        /// Cloud Id
        /// </summary>
        public String Id { get; set; }
   
    }

}
