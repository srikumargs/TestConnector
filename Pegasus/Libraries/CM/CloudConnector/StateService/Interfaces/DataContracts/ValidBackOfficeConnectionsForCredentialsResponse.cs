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
    [DataContract(Namespace = ServiceConstants.V1_SERVICE_NAMESPACE, Name = "BackOfficeConnectionsForCredentialsResponseContract")]
    public sealed class BackOfficeConnectionsForCredentialsResponse : IExtensibleDataObject
    {
        /// <summary>
        /// 
        /// </summary>
        public BackOfficeConnectionsForCredentialsResponse(IEnumerable<BackOfficeConnection> backOfficeConnections, IEnumerable<String> userFacingMessages, IEnumerable<String> rawErrorMessages)
        {
            if (backOfficeConnections == null)
            {
                backOfficeConnections = new BackOfficeConnection[] { };
            }

            if (userFacingMessages == null)
            {
                userFacingMessages = new String[] { };
            }

            if (rawErrorMessages == null)
            {
                rawErrorMessages = new String[] { };
            }

            BackOfficeConnections = backOfficeConnections.ToArray();
            UserFacingMessages = userFacingMessages.ToArray();
            RawErrorMessage = rawErrorMessages.ToArray();
        }

        /// <summary>
        /// Initializes a new instance of the BackOfficeConnectionsForCredentialsResponse class from an existing instance and a collection of propertyTuples
        /// </summary>
        /// <param name="source"></param>
        /// <param name="propertyTuples"></param>
        public BackOfficeConnectionsForCredentialsResponse(BackOfficeConnectionsForCredentialsResponse source, IEnumerable<PropertyTuple> propertyTuples)
        {
            BackOfficeConnections = source.BackOfficeConnections;
            UserFacingMessages = source.UserFacingMessages;
            RawErrorMessage = source.RawErrorMessage;
            ExtensionData = source.ExtensionData;

            var myPropertyTuples = propertyTuples.Where(x => x.Item1.DeclaringType == typeof(BackOfficeConnectionsForCredentialsResponse));
            foreach (var tuple in myPropertyTuples)
            {
                tuple.Item1.SetValue(this, tuple.Item2, null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "BackOfficeConnections", IsRequired = true, Order = 0)]
        public BackOfficeConnection[] BackOfficeConnections { get; private set; }

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
        /// To support forward-compatible data contracts
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }
    }
}