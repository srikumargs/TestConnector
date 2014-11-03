using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Sage.Connector.LinkedSource;

namespace Sage.Connector.Common.DataContracts
{
    /// <summary>
    /// TODO:  v1ok - evaluate consolidation of ActivitEntryRecord (an internal implementation detail of the Connector service) with the RequestState 
    /// parallel to it located in Sage.Connector.Commmon (which is used to surface request status information to the Monitor tray app)
    /// </summary>
    [DataContract(Namespace = ServiceConstants.V1_SERVICE_NAMESPACE, Name = "RequestStateContract")]
    public sealed class RequestState : IExtensibleDataObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestState" /> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cloudTenantId">The cloud tenant identifier.</param>
        /// <param name="cloudRequestId">The cloud request identifier.</param>
        /// <param name="cloudRequestCreatedTimestampUtc">The cloud request created timestamp UTC.</param>
        /// <param name="cloudRequestRetryCount">The cloud request retry count.</param>
        /// <param name="cloudRequestType">Type of the cloud request.</param>
        /// <param name="cloudRequestInnerType">Type of the cloud request secondary.</param>
        /// <param name="cloudRequestRequestingUser">The cloud request requesting user.</param>
        /// <param name="isSystemRequest">The is system request.</param>
        /// <param name="state1DateTimeUtc">The state1 date time UTC.</param>
        /// <param name="state2DateTimeUtc">The state2 date time UTC.</param>
        /// <param name="state3DateTimeUtc">The state3 date time UTC.</param>
        /// <param name="state4DateTimeUtc">The state4 date time UTC.</param>
        /// <param name="state5DateTimeUtc">The state5 date time UTC.</param>
        /// <param name="state6DateTimeUtc">The state6 date time UTC.</param>
        /// <param name="state7DateTimeUtc">The state7 date time UTC.</param>
        /// <param name="state8DateTimeUtc">The state8 date time UTC.</param>
        /// <param name="state9DateTimeUtc">The state9 date time UTC.</param>
        /// <param name="state10DateTimeUtc">The state10 date time UTC.</param>
        /// <param name="state11DateTimeUtc">The state11 date time UTC.</param>
        /// <param name="state12DateTimeUtc">The state12 date time UTC.</param>
        /// <param name="state13DateTimeUtc">The state13 date time UTC.</param>
        /// <param name="state14DateTimeUtc">The state14 date time UTC.</param>
        /// <param name="state15DateTimeUtc">The state15 date time UTC.</param>
        /// <param name="state16DateTimeUtc">The state16 date time UTC.</param>
        /// <param name="state17DateTimeUtc">The state17 date time UTC.</param>
        /// <param name="dateTimeUtc">The date time UTC.</param>
        /// <param name="requestStatus">The request status.</param>
        /// <param name="tenantName">Name of the tenant.</param>
        /// <param name="backOfficeCompanyName">Name of the back office company.</param>
        /// <param name="cloudProjectName">Name of the cloud project.</param>
        /// <param name="cloudRequestSummary">The cloud request summary.</param>
        public RequestState(
            Guid id,
            String cloudTenantId,
            Guid cloudRequestId,
            DateTime cloudRequestCreatedTimestampUtc,
            Int64 cloudRequestRetryCount,
            String cloudRequestType,
            String cloudRequestInnerType,
            String cloudRequestRequestingUser,
            Boolean isSystemRequest,
            DateTime? state1DateTimeUtc,
            DateTime? state2DateTimeUtc,
            DateTime? state3DateTimeUtc,
            DateTime? state4DateTimeUtc,
            DateTime? state5DateTimeUtc,
            DateTime? state6DateTimeUtc,
            DateTime? state7DateTimeUtc,
            DateTime? state8DateTimeUtc,
            DateTime? state9DateTimeUtc,
            DateTime? state10DateTimeUtc,
            DateTime? state11DateTimeUtc,
            DateTime? state12DateTimeUtc,
            DateTime? state13DateTimeUtc,
            DateTime? state14DateTimeUtc,
            DateTime? state15DateTimeUtc,
            DateTime? state16DateTimeUtc,
            DateTime? state17DateTimeUtc,
            DateTime dateTimeUtc,
            RequestStatus requestStatus,
            String tenantName,
            String backOfficeCompanyName,
            String cloudProjectName,
            String cloudRequestSummary)
        {
            Id = id;
            CloudTenantId = cloudTenantId;
            CloudRequestId = cloudRequestId;
            CloudRequestCreatedTimestampUtc = cloudRequestCreatedTimestampUtc;
            CloudRequestRetryCount = cloudRequestRetryCount;
            CloudRequestType = cloudRequestType;
            CloudRequestInnerType = cloudRequestInnerType;
            IsSystemRequest = isSystemRequest;
            CloudRequestRequestingUser = cloudRequestRequestingUser;
            State1DateTimeUtc = state1DateTimeUtc;
            State2DateTimeUtc = state2DateTimeUtc;
            State3DateTimeUtc = state3DateTimeUtc;
            State4DateTimeUtc = state4DateTimeUtc;
            State5DateTimeUtc = state5DateTimeUtc;
            State6DateTimeUtc = state6DateTimeUtc;
            State7DateTimeUtc = state7DateTimeUtc;
            State8DateTimeUtc = state8DateTimeUtc;
            State9DateTimeUtc = state9DateTimeUtc;
            State10DateTimeUtc = state10DateTimeUtc;
            State11DateTimeUtc = state11DateTimeUtc;
            State12DateTimeUtc = state12DateTimeUtc;
            State13DateTimeUtc = state13DateTimeUtc;
            State14DateTimeUtc = state14DateTimeUtc;
            State15DateTimeUtc = state15DateTimeUtc;
            State16DateTimeUtc = state16DateTimeUtc;
            State17DateTimeUtc = state17DateTimeUtc;
            DateTimeUtc = dateTimeUtc;
            RequestStatus = requestStatus;
            TenantName = tenantName;
            BackOfficeCompanyName = backOfficeCompanyName;
            CloudProjectName = cloudProjectName;
            CloudRequestSummary = cloudRequestSummary;
        }

        /// <summary>
        /// Initializes a new instance of the RequestState class from an existing instance and a collection of propertyTuples
        /// </summary>
        /// <param name="source"></param>
        /// <param name="propertyTuples"></param>
        public RequestState(RequestState source, IEnumerable<PropertyTuple> propertyTuples)
        {
            Id = source.Id;
            CloudTenantId = source.CloudTenantId;
            CloudRequestId = source.CloudRequestId;
            CloudRequestCreatedTimestampUtc = source.CloudRequestCreatedTimestampUtc;
            CloudRequestRetryCount = source.CloudRequestRetryCount;
            CloudRequestType = source.CloudRequestType;
            CloudRequestInnerType = source.CloudRequestInnerType;
            IsSystemRequest = source.IsSystemRequest;
            CloudRequestRequestingUser = source.CloudRequestRequestingUser;
            State1DateTimeUtc = source.State1DateTimeUtc;
            State2DateTimeUtc = source.State2DateTimeUtc;
            State3DateTimeUtc = source.State3DateTimeUtc;
            State4DateTimeUtc = source.State4DateTimeUtc;
            State5DateTimeUtc = source.State5DateTimeUtc;
            State6DateTimeUtc = source.State6DateTimeUtc;
            State7DateTimeUtc = source.State7DateTimeUtc;
            State8DateTimeUtc = source.State8DateTimeUtc;
            State9DateTimeUtc = source.State9DateTimeUtc;
            State10DateTimeUtc = source.State10DateTimeUtc;
            State11DateTimeUtc = source.State11DateTimeUtc;
            State12DateTimeUtc = source.State12DateTimeUtc;
            State13DateTimeUtc = source.State13DateTimeUtc;
            State14DateTimeUtc = source.State14DateTimeUtc;
            State15DateTimeUtc = source.State15DateTimeUtc;
            State16DateTimeUtc = source.State16DateTimeUtc;
            State17DateTimeUtc = source.State17DateTimeUtc;
            DateTimeUtc = source.DateTimeUtc;
            RequestStatus = source.RequestStatus;
            TenantName = source.TenantName;
            BackOfficeCompanyName = source.BackOfficeCompanyName;
            CloudProjectName = source.CloudProjectName;
            CloudRequestSummary = source.CloudRequestSummary;
            ExtensionData = source.ExtensionData;

            var myPropertyTuples = propertyTuples.Where(x => x.Item1.DeclaringType == typeof(RequestState));
            foreach (var tuple in myPropertyTuples)
            {
                tuple.Item1.SetValue(this, tuple.Item2, null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "Id", IsRequired = true, Order = 0)]
        public Guid Id { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "CloudTenantId", IsRequired = true, Order = 1)]
        public String CloudTenantId { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "CloudRequestId", IsRequired = true, Order = 2)]
        public Guid CloudRequestId { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "CloudRequestCreatedTimestampUtc", IsRequired = true, Order = 3)]
        public DateTime CloudRequestCreatedTimestampUtc { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "CloudRequestRetryCount", IsRequired = true, Order = 4)]
        public Int64 CloudRequestRetryCount { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "CloudRequestType", IsRequired = true, Order = 5)]
        public String CloudRequestType { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "CloudRequestInnerType", IsRequired = true, Order = 6)]
        public String CloudRequestInnerType { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "CloudRequestRequestingUser", IsRequired = true, Order = 7)]
        public String CloudRequestRequestingUser { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is system request.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is system request; otherwise, <c>false</c>.
        /// </value>
        [DataMember(Name = "IsSystemRequest", IsRequired = true, Order = 8)]
        public bool IsSystemRequest { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "State1DateTimeUtc", IsRequired = true, Order = 9)]
        public DateTime? State1DateTimeUtc { get; private set; }

        /// <summary>        
        /// 
        /// </summary>
        [DataMember(Name = "State2DateTimeUtc", IsRequired = true, Order = 10)]
        public DateTime? State2DateTimeUtc { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "State3DateTimeUtc", IsRequired = true, Order = 11)]
        public DateTime? State3DateTimeUtc { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "State4DateTimeUtc", IsRequired = true, Order = 12)]
        public DateTime? State4DateTimeUtc { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "State5DateTimeUtc", IsRequired = true, Order = 13)]
        public DateTime? State5DateTimeUtc { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "State6DateTimeUtc", IsRequired = true, Order = 14)]
        public DateTime? State6DateTimeUtc { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "State7DateTimeUtc", IsRequired = true, Order = 15)]
        public DateTime? State7DateTimeUtc { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "State8DateTimeUtc", IsRequired = true, Order = 16)]
        public DateTime? State8DateTimeUtc { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "State9DateTimeUtc", IsRequired = true, Order = 17)]
        public DateTime? State9DateTimeUtc { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "State10DateTimeUtc", IsRequired = true, Order = 18)]
        public DateTime? State10DateTimeUtc { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "State11DateTimeUtc", IsRequired = true, Order = 19)]
        public DateTime? State11DateTimeUtc { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "State12DateTimeUtc", IsRequired = true, Order = 20)]
        public DateTime? State12DateTimeUtc { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "State13DateTimeUtc", IsRequired = true, Order = 21)]
        public DateTime? State13DateTimeUtc { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "State14DateTimeUtc", IsRequired = true, Order = 22)]
        public DateTime? State14DateTimeUtc { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "State15DateTimeUtc", IsRequired = true, Order = 23)]
        public DateTime? State15DateTimeUtc { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "State16DateTimeUtc", IsRequired = true, Order = 24)]
        public DateTime? State16DateTimeUtc { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "State17DateTimeUtc", IsRequired = true, Order = 25)]
        public DateTime? State17DateTimeUtc { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "DateTimeUtc", IsRequired = true, Order = 26)]
        public DateTime DateTimeUtc { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "RequestStatus", IsRequired = true, Order = 27)]
        public RequestStatus RequestStatus { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "TenantName", IsRequired = true, Order = 28)]
        public String TenantName { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "BackOfficeCompanyName", IsRequired = true, Order = 29)]
        public String BackOfficeCompanyName { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "CloudProjectName", IsRequired = true, Order = 30)]
        public String CloudProjectName { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "CloudRequestSummary", IsRequired = true, Order = 31)]
        public String CloudRequestSummary { get; private set; }

        /// <summary>
        /// To support forward-compatible data contracts
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }
    }
}
