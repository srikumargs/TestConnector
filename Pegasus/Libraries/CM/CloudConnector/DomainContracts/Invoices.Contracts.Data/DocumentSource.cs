
namespace Sage.Connector.Invoices.Contracts.Data
{
    /// <summary>
    /// Document Sources
    /// </summary>
    public enum DocumentSource
    {
        /// <summary>
        /// Sales Order Source for Invoice
        /// </summary>
        SalesOrder = 0,                //Sales order - used my mobile sales and future mobile service integrated with sales order
       /// <summary>
       /// Accounts Receivable Source for Invoice
       /// </summary>
        ArTrx = 1,         //ar invoice - used by current mobile service.  Mobile Sales Mongo will use for Sales MTD, YTD, but will ignore for items purchase KPIs
       
        /// <summary>
        /// Other source for Invoice, such as Job Cost.
        /// </summary>
        Other = 2                  // JC or other unknow types of invoices.  Will not be sent by SB&P OR, however should be included in MTD, YTD sales KPI
    }
}
