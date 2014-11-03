using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sage.Connector.DomainContracts.Data;

namespace Sage.Connector.Statements.Contracts.Data.Requests
{
    /// <summary>
    /// Statements Request
    /// </summary>
    public class StatementsRequest
    {
        private ICollection<ExternalReference> _customerReferences;

        /// <summary>
        /// Statement Processing Date
        /// </summary>
        public DateTime StatementDate { get; set; }

        /// <summary>
        /// Collection of Customer External References
        /// </summary>
        public ICollection<ExternalReference> CustomerReferences
        {
            get { return _customerReferences ?? (_customerReferences = new Collection<ExternalReference>()); }
            set { _customerReferences = value; }
        }
    }
}
