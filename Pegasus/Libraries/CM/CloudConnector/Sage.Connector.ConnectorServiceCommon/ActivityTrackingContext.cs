using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Sage.Connector.LinkedSource;
using Sage.CRE.CloudConnector.Integration.Interfaces.Utils;
using Sage.Diagnostics;

namespace Sage.Connector.ConnectorServiceCommon
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Namespace = ServiceConstants.V1_SERVICE_NAMESPACE, Name = "TrackingContextContract")]
    public sealed class ActivityTrackingContext
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="tenantId"></param>
        /// <param name="requestId"></param>
        /// <param name="requestType"></param>
        public ActivityTrackingContext(Guid id, String tenantId, Guid requestId, String requestType)
        {
            ArgumentValidator.ValidateNonNullReference(tenantId, "tenantId", _myTypeName + ".ctor()");

            Id = id;
            TenantId = tenantId;
            RequestId = requestId;
            RequestType = requestType;

        }

        /// <summary>
        /// Initializes a new instance of the TrackingContext class from an existing instance and a collection of propertyTuples
        /// </summary>
        /// <param name="source"></param>
        /// <param name="propertyTuples"></param>
        public ActivityTrackingContext(ActivityTrackingContext source, IEnumerable<PropertyTuple> propertyTuples)
        {
            Id = source.Id;
            TenantId = source.TenantId;
            RequestId = source.RequestId;
            RequestType = source.RequestType;

            var myPropertyTuples = propertyTuples.Where(x => x.Item1.DeclaringType == typeof(ActivityTrackingContext));
            foreach (var tuple in myPropertyTuples)
            {
                tuple.Item1.SetValue(this, tuple.Item2, null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "Id", IsRequired = true, Order = 0)]
        public Guid Id
        { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "TenantId", IsRequired = true, Order = 1)]
        public String TenantId
        { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "RequestId", IsRequired = true, Order = 2)]
        public Guid RequestId
        { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name="RequestType", IsRequired = true, Order = 3)]
        public String RequestType { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public static ActivityTrackingContext Empty
        { get { return _empty; } }

        private readonly static ActivityTrackingContext _empty = new ActivityTrackingContext(Guid.Empty, String.Empty, Guid.Empty, String.Empty);

        private static readonly String _myTypeName = typeof(ActivityTrackingContext).FullName;
    }
}
