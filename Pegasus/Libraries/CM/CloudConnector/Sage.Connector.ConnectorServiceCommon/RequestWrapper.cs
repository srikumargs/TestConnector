using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Sage.Connector.Cloud.Integration.Interfaces.Utils;
using Sage.Connector.LinkedSource;

namespace Sage.Connector.ConnectorServiceCommon
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Namespace = ServiceConstants.V1_SERVICE_NAMESPACE, Name = "RequestWrapperContract")]
    public class RequestWrapper : IExtensibleDataObject
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the Request class
        /// </summary>
        /// <param name="activityTrackingContext"></param>
        /// <param name="requestPayload"></param>
        public RequestWrapper(ActivityTrackingContext activityTrackingContext, String requestPayload)
        {
            ActivityTrackingContext = activityTrackingContext;
            RequestPayload = requestPayload;
        }

        /// <summary>
        /// Initializes a new instance of the Request class from an existing instance and a collection of propertyTuples
        /// </summary>
        /// <param name="source"></param>
        /// <param name="propertyTuples"></param>
        public RequestWrapper(RequestWrapper source, IEnumerable<PropertyTuple> propertyTuples)
        {
            ActivityTrackingContext = source.ActivityTrackingContext;
            RequestPayload = source.RequestPayload;
            ExtensionData = source.ExtensionData;

            var myPropertyTuples = propertyTuples.Where(x => x.Item1.DeclaringType == typeof(RequestWrapper));
            foreach (var tuple in myPropertyTuples)
            {
                tuple.Item1.SetValue(this, tuple.Item2, null);
            }
        }
        #endregion


        #region Public properties
        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "ActivityTrackingContext", IsRequired = true, Order = 0)]
        public ActivityTrackingContext ActivityTrackingContext { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "RequestPayload", IsRequired = true, Order = 1)]
        public String RequestPayload { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// To support forward-compatible data contracts
        /// </remarks>
        public ExtensionDataObject ExtensionData { get; set; }
        #endregion
    }
}
