
using System;

namespace Sage.Connector.MockCloudHostApp
{    /// <summary>
    /// 
    /// </summary>
    public enum InvokeActionEnum
    {

        /// <summary>
        /// 
        /// </summary>
        None = 0,

        /// <summary>
        /// 
        /// </summary>
        Loopback,

        /// <summary>
        /// 
        /// </summary>
        UpdateSiteServiceInfo,

        /// <summary>
        /// 
        /// </summary>
        UpdateConfigParams,

        /// <summary>
        /// 
        /// </summary>
        UpdateCustomConfigParams,

        /// <summary>
        /// 
        /// </summary>
        DeleteTenant,

        /// <summary>
        /// 
        /// </summary>
        HealthCheck,


        /// <summary>
        /// Domain Mediation is a generic payload transport ProcessQuote request 
        /// where the domain mediator determines
        /// how and who to deliver the cloud request to the client. 
        /// This Request goes to the Sales domain
        /// </summary>
        ProcessQuote,
        /// <summary>
        /// Domain Mediation is a generic payload transport SyncCustomers request where the domain mediator determines
        /// how and who to deliver the cloud request to the client. 
        /// This Request goes to the Shared Sync domain
        /// </summary>
        SyncCustomers,

        /// <summary>
        /// Domain Mediation is a generic payload transport ValidateBackOfficeConnection
        ///  request where the domain mediator determines
        /// how and who to deliver the cloud request to the client. 
        /// This Request goes to the common core domain mediator
        /// </summary>
        /// <remarks>
        /// Replaced in plugin with calls that take property bags
        /// </remarks>
        [Obsolete]
        ValidateBackOfficeConnection,

        /// <summary>
        /// Domain Mediation is a generic payload transport the GetBackOfficeConfiguration request 
        /// where the domain mediator determines
        /// how and who to deliver the cloud request to the client. 
        /// This Request goes to the common core domain mediator
        /// </summary>
        GetBackOfficePluginConfiguration,

        /// <summary>
        /// Sync Tax Codes
        /// </summary>
        SyncTaxCodes,

        /// <summary>
        /// Sync Tax Schedules
        /// </summary>
        SyncTaxSchedules,

        /// <summary>
        /// Sync Inventory Items
        /// </summary>
        SyncInventoryItems,

        /// <summary>
        /// Process Quote To Order
        /// </summary>
        ProcessQuoteToOrder, 

        /// <summary>
        /// Process Paid dOrder
        /// </summary>
        ProcessPaidOrder,

        /// <summary>
        /// Process Payment
        /// </summary>
        ProcessPayment,

        /// <summary>
        /// Process Statements
        /// </summary>
        ProcessStatements,

        /// <summary>
        /// Sync Service Types
        /// </summary>
        SyncServiceTypes,

        /// <summary>
        /// Process WorkOrder To Invoice
        /// </summary>
        ServiceProcessWorkOrderToInvoice,

        /// <summary>
        /// Validate that the back office for the plug in is installed.
        /// </summary>
        ValidateBackOfficeIsInstalled,

        /// <summary>
        /// Get information from the plugin
        /// </summary>
        GetPluginInformation,

        /// <summary>
        /// Get collection of info for back offices
        /// </summary>
        GetPluginInformationCollection,
        
        /// <summary>
        /// Get collection for info for the installed back offices
        /// </summary>
        GetInstalledBackOfficePluginInformationCollection,

        /// <summary>
        /// 
        /// </summary>
        GetCompanyConnectionManagementCredentialsNeeded,
        /// <summary>
        /// 
        /// </summary>
        GetCompanyConnectionCredentialsNeeded,
        /// <summary>
        /// 
        /// </summary>
        ValidateCompanyConnectionCredentials,
        /// <summary>
        /// 
        /// </summary>
        ValidateCompanyConnectionManagementCredentials,

        /// <summary>
        /// Sync Invoices
        /// </summary>
        SyncInvoices,

        /// <summary>
        /// Sync Invoice Balances
        /// </summary>
        SyncInvoiceBalances,

        /// <summary>
        /// Sync Salespersons
        /// </summary>
        SyncSalespersons,

        /// <summary>
        /// Sync Salesperson Customers
        /// </summary>
        SyncSalespersonCustomers,

        /// <summary>
        /// Begin Scheduled Synchronization
        /// </summary>
        ScheduledSynchronization,
    }
}
