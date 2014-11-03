using Sage.Connector.DomainContracts.BackOffice;
using Sage.Connector.DomainContracts.Responses;
using Sage.Connector.Invoices.Contracts.Data;
using System;
using System.Collections.Generic;

namespace Sage.Connector.Invoices.Contracts.BackOffice
{
    /// <summary>
    /// Get Invoices
    /// </summary>
    public interface ISyncInvoices : IBackOfficeSessionHandler
    {
        /// <summary>
        /// Initialize the GetInvoices to get ready to start processing the selection.
        /// This could be something like loading the invoice business object to get 
        /// ready to process.  
        /// </summary>
        /// <param name="processingPropBag">Processing Property Bag is a writable dictionary to persist property value pairs 
        /// on a local machine store for the back office company for this feature. 
        /// Because this is historical data (ex 2 years worth), these values can help with knowing which invoices to get on 
        /// the next run, which can avoid upload of the same records more than once.  If for some reason a duplicate is sent, 
        /// the sync mechanism will exclude it. 
        /// Initially, the property bag is empty.  It is up to the process to set property information for the last 
        /// invoice information processed to be used on the next run.    
        /// As many property value pairs as desired can be used, the amount of data should not be large, meaning don't store
        /// the set of 2 year history invoices in the property bag. </param>
        /// <returns><see cref="Response"/> A response status of failure will end the sync feature immediately.  </returns>
        Response InitializeSyncInvoices(IDictionary<String, Object> processingPropBag);

        /// <summary>
        /// Get the next invoice to sync up from the backoffice
        /// </summary>
        /// <returns>The <see cref="Invoice"/></returns>
        Invoice GetNextSyncInvoice();
    }
}
