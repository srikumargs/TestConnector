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
    [DataContract(Namespace = ServiceConstants.V1_SERVICE_NAMESPACE, Name = "SubsystemHealthMessageContract")]
    public sealed class SubsystemHealthMessage : IExtensibleDataObject
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="subsystem"></param>
        /// <param name="rawMessage"></param>
        /// <param name="userFacingMessage"></param>
        /// <param name="timestampUtc"></param>
        /// <param name="helpTopicId"></param>
        public SubsystemHealthMessage(Subsystem subsystem, String rawMessage, String userFacingMessage, DateTime timestampUtc, Int32? helpTopicId)
        {
            Subsystem = subsystem;
            RawMessage = rawMessage;
            UserFacingMessage = userFacingMessage;
            TimestampUtc = timestampUtc;
            HelpTopicId = helpTopicId;
        }

        /// <summary>
        /// Initializes a new instance of the UnhealthySubsystemMessage class from an existing instance and a collection of propertyTuples
        /// </summary>
        /// <param name="source"></param>
        /// <param name="propertyTuples"></param>
        public SubsystemHealthMessage(SubsystemHealthMessage source, IEnumerable<PropertyTuple> propertyTuples)
        {
            Subsystem = source.Subsystem;
            RawMessage = source.RawMessage;
            UserFacingMessage = source.UserFacingMessage;
            TimestampUtc = source.TimestampUtc;
            HelpTopicId = source.HelpTopicId;
            ExtensionData = source.ExtensionData;

            var myPropertyTuples = propertyTuples.Where(x => x.Item1.DeclaringType == typeof(SubsystemHealthMessage));
            foreach (var tuple in myPropertyTuples)
            {
                tuple.Item1.SetValue(this, tuple.Item2, null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "Subsystem", IsRequired = true, Order = 0)]
        public Subsystem Subsystem { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "RawMessage", IsRequired = true, Order = 1)]
        public String RawMessage { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "UserFacingMessage", IsRequired = true, Order = 2)]
        public String UserFacingMessage { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "TimestampUtc", IsRequired = true, Order = 3)]
        public DateTime TimestampUtc { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "HelpLink", IsRequired = true, Order = 4)]
        public Int32? HelpTopicId { get; private set; }

        /// <summary>
        /// To support forward-compatible data contracts
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }

    }
}
