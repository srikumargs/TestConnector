using System.Runtime.Serialization;
using Sage.Connector.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sage.Connector.StateService.Interfaces.DataContracts
{
    /// <summary>
    /// 
    /// </summary>
    public class FeatureRequest
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pluginId"></param>
        /// <param name="featureId"></param>
        /// <param name="tenantId"></param>
        /// <param name="payload"></param>
        public FeatureRequest(String pluginId, string featureId, string tenantId, string payload)
        {
            PluginId = pluginId;
            FeatureId = featureId;
            TenantId = tenantId;
            Payload = payload;
        }

        /// <summary>
        /// Initializes a new instance of the BackOfficeConnection class from an existing instance and a collection of propertyTuples
        /// </summary>
        /// <param name="source"></param>
        /// <param name="propertyTuples"></param>
        public FeatureRequest(FeatureRequest source, IEnumerable<PropertyTuple> propertyTuples)
        {
            PluginId = source.PluginId;
            FeatureId = source.FeatureId;
            Payload = source.Payload;
            TenantId = source.TenantId;

            var myPropertyTuples = propertyTuples.Where(x => x.Item1.DeclaringType == typeof(FeatureRequest));
            foreach (var tuple in myPropertyTuples)
            {
                tuple.Item1.SetValue(this, tuple.Item2, null);
            }

        }


        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "PluginId", IsRequired = true, Order = 0)]

        public string PluginId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "FeatureId", IsRequired = true, Order = 1)]
        public string FeatureId { get; set; }

        /// <summary>
        /// TenantId
        /// </summary>
        [DataMember(Name = "Tenant", IsRequired = true, Order = 2)]
        public string TenantId { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "Payload", IsRequired = false, Order = 3)]
        public string Payload { get; set; }
    }
}
