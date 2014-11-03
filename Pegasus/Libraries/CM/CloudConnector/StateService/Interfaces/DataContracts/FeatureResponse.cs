using System.Linq;
using Sage.Connector.Common;
using Sage.Connector.LinkedSource;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Sage.Connector.StateService.Interfaces.DataContracts
{

    /// <summary>
    /// 
    /// </summary>
    [DataContract(Namespace = ServiceConstants.V1_SERVICE_NAMESPACE, Name = "FeatureResponse")]
    public sealed class FeatureResponse : IExtensibleDataObject
    {
        /// <summary>
        /// 
        /// </summary>
        public FeatureResponse(String payload, IEnumerable<String> userFacingMessages, IEnumerable<String> rawErrorMessages)
        {


            if (userFacingMessages == null)
            {
                userFacingMessages = new String[] { };
            }

            if (rawErrorMessages == null)
            {
                rawErrorMessages = new String[] { };
            }

            Payload = payload;
            UserFacingMessages = userFacingMessages.ToArray();
            RawErrorMessage = rawErrorMessages.ToArray();
        }

        /// <summary>
        /// Initializes a new instance of the FeatureResponse class from an existing instance and a collection of propertyTuples
        /// </summary>
        /// <param name="source"></param>
        /// <param name="propertyTuples"></param>
        public FeatureResponse(FeatureResponse source,
            IEnumerable<PropertyTuple> propertyTuples)
        {
            Payload = source.Payload;

            var myPropertyTuples = propertyTuples.Where(x => x.Item1.DeclaringType == typeof(FeatureResponse));
            foreach (var tuple in myPropertyTuples)
            {
                tuple.Item1.SetValue(this, tuple.Item2, null);
            }
        }

        /// <summary>
        /// Payload response
        /// </summary>
        [DataMember(Name = "Payload", IsRequired = true, Order = 0)]
        public string Payload { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "UserFacingMessages", IsRequired = true, Order = 1)]
        public String[] UserFacingMessages { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "RawErrorMessage", IsRequired = true, Order = 1)]
        public String[] RawErrorMessage { get; private set; }
        /// <summary>
        /// Gets or sets the structure that contains extra data.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Runtime.Serialization.ExtensionDataObject"/> that contains data that is not recognized as belonging to the data contract.
        /// </returns>
        public ExtensionDataObject ExtensionData { get; set; }
    }


}


