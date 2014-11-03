
namespace Sage.Connector.Invoices.Contracts.Data
{

    /// <summary>
    /// Document Types
    /// </summary>
    public enum DocumentType
    {
        /// <summary>
        /// Invoice document
        /// </summary>
        Invoice = 0,

        /// <summary>
        /// Credit Memo document
        /// </summary>
        CreditMemo = 1,

        /// <summary>
        /// Debit Memo document
        /// </summary>
        DebitMemo = 2,

        /// <summary>
        /// Other document, for example cash type invoices that need to be included for MTD, YTD sales amounts
        /// </summary>
        Other = 3                 
    }

}
