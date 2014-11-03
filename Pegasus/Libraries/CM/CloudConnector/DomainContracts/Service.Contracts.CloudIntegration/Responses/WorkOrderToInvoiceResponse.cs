using System;
using Sage.Connector.DomainContracts.Responses;

namespace Sage.Connector.Service.Contracts.CloudIntegration.Responses
{
    /// <summary>
    /// Response of the Work Order to Invoice process
    /// </summary>
    public class WorkOrderToInvoiceResponse: Response
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public WorkOrderToInvoiceResponse()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="response"></param>
        public WorkOrderToInvoiceResponse(Response response)
        {
            if (response == null)
                return;

            Status = response.Status;
            Diagnoses = response.Diagnoses;
        }



        /// <summary>
        /// Response work order
        /// </summary>
        public WorkOrder WorkOrder { get; set; }
    }

    /// <summary>
    /// Work Order response with the limited set of information required by the cloud.
    /// </summary>
    public class WorkOrder
    {
        /// <summary>
        /// WorkOrder Id
        /// </summary>
        public String Id { get; set; }

        /// <summary>
        /// Document Reference
        /// </summary>
        public String DocumentReference { get; set; }
    }
}
