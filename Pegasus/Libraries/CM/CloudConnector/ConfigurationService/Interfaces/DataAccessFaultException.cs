using System.Runtime.Serialization;

namespace Sage.Connector.ConfigurationService.Interfaces
{
    /// <summary>
    /// A data operation fault exception
    /// </summary>
    [DataContract]
    public class DataAccessFaultException
    {
        /// <summary>
        /// The attempted data operation
        /// </summary>
        [DataMember]
        public string Operation { get; set; }

        /// <summary>
        /// The reported problem
        /// </summary>
        [DataMember]
        public string Problem { get; set; }
    }
}
