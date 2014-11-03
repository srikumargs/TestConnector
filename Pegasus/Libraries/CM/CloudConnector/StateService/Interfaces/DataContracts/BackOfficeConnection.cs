using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Sage.Connector.Common;
using Sage.Connector.LinkedSource;

namespace Sage.Connector.StateService.Interfaces.DataContracts
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Namespace = ServiceConstants.V1_SERVICE_NAMESPACE, Name = "BackOfficeConnectionContract")]
    public sealed class BackOfficeConnection : IExtensibleDataObject
    {
        /// <summary>
        /// 
        /// </summary>
        public BackOfficeConnection(String connectionInformation, String displayableConnectionInformation, String name)
        {
            ConnectionInformation = connectionInformation;
            DisplayableConnectionInformation = displayableConnectionInformation;
            Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the BackOfficeConnection class from an existing instance and a collection of propertyTuples
        /// </summary>
        /// <param name="source"></param>
        /// <param name="propertyTuples"></param>
        public BackOfficeConnection(BackOfficeConnection source, IEnumerable<PropertyTuple> propertyTuples)
        {
            ConnectionInformation = source.ConnectionInformation;
            DisplayableConnectionInformation = source.DisplayableConnectionInformation;
            Name = source.Name;
            ExtensionData = source.ExtensionData;

            var myPropertyTuples = propertyTuples.Where(x => x.Item1.DeclaringType == typeof(BackOfficeConnection));
            foreach (var tuple in myPropertyTuples)
            {
                tuple.Item1.SetValue(this, tuple.Item2, null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "ConnectionInformation", IsRequired = true, Order = 0)]
        public String ConnectionInformation { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "DisplayableConnectionInformation", IsRequired = true, Order = 1)]
        public String DisplayableConnectionInformation { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "Name", IsRequired = true, Order = 2)]
        public String Name { get; private set; }

        /// <summary>
        /// To support forward-compatible data contracts
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }
    }
}
