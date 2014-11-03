using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sage.Connector.DomainContracts.Data;
using Sage.Connector.DomainContracts.Responses;

namespace Sage.Connector.Sales.Contracts.CloudIntegration.Responses
{
    /// <summary>
    /// QuoteToOrder Response
    /// </summary>
    public class QuoteToOrderResponse : Response
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public QuoteToOrderResponse()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="response"></param>
        public QuoteToOrderResponse(Response response)
        {
            if (response == null)
                return;

            Status = response.Status;
            Diagnoses = response.Diagnoses;
        }

        /// <summary>
        /// Quote to Order for response
        /// </summary>
        public QuoteToOrder QuoteToOrder { get; set; }
    }

    /// <summary>
    /// Quote to Order for <see cref="QuoteToOrderResponse"/>
    /// </summary>
    public class QuoteToOrder : AbstractEntityInformation
    {
        private ICollection<QuoteToOrderDetail> _details;

        /// <summary>
        /// Cloud Id to be set from original payload request
        /// </summary>
        public String Id { get; set; }

        /// <summary>
        /// Date quote was submitted 
        /// </summary>
        public DateTime? SubmittedDate { get; set; } //get //set (by 100 if null will set to order date (today)

        /// <summary>
        /// Quote to Order details collection
        /// </summary>
        public ICollection<QuoteToOrderDetail> Details
        {
            get { return _details ?? (_details = new Collection<QuoteToOrderDetail>()); }
            set { _details = value; }
        }
    }

    /// <summary>
    /// Quote to Order Detail 
    /// </summary>
    public class QuoteToOrderDetail : AbstractEntityInformation
    {
        /// <summary>
        /// Cloud Id to be set from original payload request
        /// </summary>
        public String Id { get; set; }
    }
}
