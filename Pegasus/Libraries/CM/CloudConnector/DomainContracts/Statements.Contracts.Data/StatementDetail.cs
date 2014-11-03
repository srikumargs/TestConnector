using System;
using Sage.Connector.DomainContracts.Data;
using Sage.Connector.DomainContracts.Data.Attributes;

namespace Sage.Connector.Statements.Contracts.Data
{
    /// <summary>
    /// Statement Detail
    /// </summary>
    public  class StatementDetail: AbstractEntityInformation
    {
        /// <summary>
        /// Transaction Sequence
        /// </summary>
        public int TransactionSequence { get; set; }

        /// <summary>
        /// Transaction Reference
        /// </summary>
        public string TransactionReference { get; set; }

        /// <summary>
        /// Transaction Type from the back office
        /// 
        /// </summary>
        public string TransactionType { get; set; }

        /// <summary>
        /// Invoice External ID
        /// </summary>
        [ExternalIdReference]
        public String Invoice { get; set; }                  //Up in cloud, worker role will need to obtain InvoiceId from ExternalId + tenantId
        
        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Transaction Date
        /// </summary>
        public DateTime? TransactionDate { get; set; }

        /// <summary>
        /// Due Date
        /// </summary>
        public DateTime? DueDate { get; set; }

        /// <summary>
        /// Transaction Amount
        /// </summary>
        public decimal TransactionAmt { get; set; }

        /// <summary>
        /// Balance
        /// </summary>
        public decimal Balance { get; set; }


    }
}
