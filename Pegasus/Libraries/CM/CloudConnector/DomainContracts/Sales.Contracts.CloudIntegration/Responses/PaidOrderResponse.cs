using System;
using Sage.Connector.DomainContracts.Data;
using Sage.Connector.DomainContracts.Responses;

namespace Sage.Connector.Sales.Contracts.CloudIntegration.Responses
{
    /// <summary>
    /// Paid Order Reponse
    /// </summary>
    public class PaidOrderResponse : Response
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public PaidOrderResponse()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="response"></param>
        public PaidOrderResponse(Response response)
        {
            if (response == null)
                return;

            Status = response.Status;
            Diagnoses = response.Diagnoses;
        }

        /// <summary>
        /// Paid Order
        /// </summary>
        public PaidOrder PaidOrder { get; set; }
    }

    /// <summary>
    /// Paid Order for Response
    /// </summary>
    public class PaidOrder : AbstractEntityInformation
    {
        /// <summary>
        /// Cloud Id
        /// </summary>
        public String Id { get; set; }

        /// <summary>
        /// Date paid order was submitted 
        /// </summary>
        public DateTime? SubmittedDate { get; set; } 

    }

}
