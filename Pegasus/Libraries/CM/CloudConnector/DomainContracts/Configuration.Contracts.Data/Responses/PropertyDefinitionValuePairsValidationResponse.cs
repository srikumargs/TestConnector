using System.Collections.Generic;
using Sage.Connector.DomainContracts.Responses;

namespace Sage.Connector.Configuration.Contracts.Data.Responses
{
    /// <summary>
    /// PropertyValue Pair response
    /// </summary>
    public class PropertyValuePairValidationResponse : Response
    {
        /// <summary>
        /// Property Value Pair 
        /// </summary>
        public KeyValuePair<string, object> PropertyValuePair { get; set; }

    }
}
