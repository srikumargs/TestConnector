using System.Runtime.Serialization;
using Sage.Connector.LinkedSource;

namespace Sage.Connector.StateService.Interfaces.DataContracts
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Namespace = ServiceConstants.V1_SERVICE_NAMESPACE, Name = "SubsystemContract")]
    public enum Subsystem
    {
        /// <summary>
        /// No Subsystem (default value automatically initialized by runtime)
        /// </summary>
        [EnumMember]
        None = 0,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        ConfigurationService,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        DispatchService,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Documents,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Entities,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Logging,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        MessagingService,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        MonitorService,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        NotificationService,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Queues,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        Reports,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        StateService,

        /// <summary>
        /// Special subsytem for hard database corruption detection
        /// </summary>
        [EnumMember]
        DatabaseHardCorruption,

        /// <summary>
        /// 
        /// </summary>
        [EnumMember]
        HealthCheck
    }
}
