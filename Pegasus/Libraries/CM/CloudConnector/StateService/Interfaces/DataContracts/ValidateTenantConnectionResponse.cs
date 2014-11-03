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
    [DataContract(Namespace = ServiceConstants.V1_SERVICE_NAMESPACE, Name = "ValidateTenantConnectionResponseContract")]
    public sealed class ValidateTenantConnectionResponse : IExtensibleDataObject
    {
        /// <summary>
        /// 
        /// </summary>
        public ValidateTenantConnectionResponse(String name, Uri siteAddress, TenantConnectivityStatus tenantConnectivityStatus)
        {
            Name = name;
            SiteAddress = siteAddress;
            TenantConnectivityStatus = tenantConnectivityStatus;
        }

        /// <summary>
        /// Initializes a new instance of the ValidateTenantConnectionResponse class from an existing instance and a collection of propertyTuples
        /// </summary>
        /// <param name="source"></param>
        /// <param name="propertyTuples"></param>
        public ValidateTenantConnectionResponse(ValidateTenantConnectionResponse source, IEnumerable<PropertyTuple> propertyTuples)
        {
            Name = source.Name;
            SiteAddress = source.SiteAddress;
            TenantConnectivityStatus = source.TenantConnectivityStatus;
            ExtensionData = source.ExtensionData;

            var myPropertyTuples = propertyTuples.Where(x => x.Item1.DeclaringType == typeof(ValidateTenantConnectionResponse));
            foreach (var tuple in myPropertyTuples)
            {
                tuple.Item1.SetValue(this, tuple.Item2, null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "Name", IsRequired = true, Order = 0)]
        public String Name { get; private set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "SiteAddress", IsRequired = true, Order = 1)]
        public Uri SiteAddress { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "TenantConnectivityStatus", IsRequired = true, Order = 2)]
        public TenantConnectivityStatus TenantConnectivityStatus { get; private set; }

        /// <summary>
        /// To support forward-compatible data contracts
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }
    }
}
