using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sage.Connector.DomainContracts.Data;
using Sage.Connector.DomainContracts.Responses;

namespace Sage.Connector.Sales.Contracts.CloudIntegration.Responses
{
    /// <summary>
    /// Quote Response to send up to cloud
    /// </summary>
    public class QuoteResponse : Response
    {

        /// <summary>
        /// Constructor
        /// </summary>
        public QuoteResponse()
        {
        }
        /// <summary>
        /// Quote Response Constructor
        /// </summary>
        /// <param name="response"></param>
        public QuoteResponse(Response response)
        {
            if (response == null)
                return;

            Status = response.Status;
            Diagnoses = response.Diagnoses;
        }

        /// <summary>
        /// Quote
        /// </summary>
        public Quote Quote { get; set; }
    }

    /// <summary>
    /// Quote to use in the Process Quote Response
    /// </summary>
    public class Quote: AbstractEntityInformation
    {
        private ICollection<QuoteDetail> _details;

        /// <summary>
        /// Cloud Id to be set from original payload request
        /// </summary>
        public String Id { get; set; }

        /// <summary>
        /// Shipping and Handling
        /// </summary>
        public decimal? SandH { get; set; }

        /// <summary>
        /// Discount Percent
        /// TODO KMS: Range Required so the back offices know how to set 
        /// </summary>
        public decimal? DiscountPercent { get; set; }
       
        /// <summary>
        /// Tax
        /// </summary>
        public decimal? Tax { get; set; }
       
        /// <summary>
        /// Document Total
        /// </summary>
        public decimal DocumentTotal { get; set; }                         
        
        /// <summary>
        /// SubTotal
        /// </summary>
        public decimal SubTotal { get; set; }                          
       
        /// <summary>
        /// Expiry Date
        /// </summary>
        public DateTime? ExpiryDate { get; set; }       //get //set (by 100 if null will set to order date (today)

        /// <summary>
        /// Submitted Date
        /// </summary>
        public DateTime? SubmittedDate { get; set; } //get //set (by 100 if null will set to order date (today)

        /// <summary>
        /// Quote Detail 
        /// </summary>
        public ICollection<QuoteDetail> Details
        {
            get { return _details ?? (_details = new Collection<QuoteDetail>()); }
            set { _details = value; }
        }
    }

    /// <summary>
    /// Quote Detail for Quote Response
    /// </summary>
     public class QuoteDetail: AbstractEntityInformation
    {
         /// <summary>
         /// Cloud Id to be set from original payload request
         /// </summary>
        public String Id { get; set; }

        /// <summary>
        /// Price
        /// </summary>
        public decimal Price { get; set; }                              
    }
}