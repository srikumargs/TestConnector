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
    [DataContract(Namespace = ServiceConstants.V1_SERVICE_NAMESPACE, Name = "ValidateBackOfficeConnectionResponseContract")]
    public sealed class ValidateBackOfficeConnectionResponse : IExtensibleDataObject
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="backOfficeConnectivityStatus"></param>
        /// <param name="connectionCredentials"></param>
        /// <param name="companyNameForDisplay"></param>
        /// <param name="companyUniqueIdentifier"></param>
        /// <param name="userFacingMessages"></param>
        /// <param name="rawErrorMessages"></param>
        public ValidateBackOfficeConnectionResponse(
            BackOfficeConnectivityStatus backOfficeConnectivityStatus,
            IDictionary<string, string> connectionCredentials,
            string companyNameForDisplay,
            string companyUniqueIdentifier,
            IEnumerable<String> userFacingMessages, 
            IEnumerable<String> rawErrorMessages)
        {
            BackOfficeConnectivityStatus = backOfficeConnectivityStatus;
            CompanyNameForDisplay = companyNameForDisplay??String.Empty;
            CompanyUniqueIndentifier = companyUniqueIdentifier ?? String.Empty;

            Dictionary<String,String> isolatedDictionary = null;
            if (connectionCredentials != null)
            {
                isolatedDictionary = connectionCredentials.ToDictionary(item => item.Key, item => item.Value);
            }
            else
            {
                isolatedDictionary = new Dictionary<string, string>();    
            }
            ConnectionCredentials = isolatedDictionary;

            if (userFacingMessages == null)
            {
                if (backOfficeConnectivityStatus == BackOfficeConnectivityStatus.Normal)
                {
                    userFacingMessages = new String[] { };
                }
                else
                {
                    throw new ArgumentException("userFacingMessages");
                }
            }

            if (rawErrorMessages == null)
            {
                if (backOfficeConnectivityStatus == BackOfficeConnectivityStatus.Normal)
                {
                    rawErrorMessages = new String[] { };
                }
                else
                {
                    throw new ArgumentException("rawErrorMessages");
                }
            }

            UserFacingMessages = userFacingMessages.ToArray();
            RawErrorMessage = rawErrorMessages.ToArray();
        }

        /// <summary>
        /// Initializes a new instance of the ValidateBackOfficeConnectionResponse class from an existing instance and a collection of propertyTuples
        /// </summary>
        /// <param name="source"></param>
        /// <param name="propertyTuples"></param>
        public ValidateBackOfficeConnectionResponse(ValidateBackOfficeConnectionResponse source, IEnumerable<PropertyTuple> propertyTuples)
        {
            BackOfficeConnectivityStatus = source.BackOfficeConnectivityStatus;
            UserFacingMessages = source.UserFacingMessages;
            RawErrorMessage = source.RawErrorMessage;
            CompanyNameForDisplay = source.CompanyNameForDisplay;
            ConnectionCredentials = source.ConnectionCredentials;
            CompanyUniqueIndentifier = source.CompanyUniqueIndentifier;
            
            ExtensionData = source.ExtensionData;


            var myPropertyTuples = propertyTuples.Where(x => x.Item1.DeclaringType == typeof(ValidateBackOfficeConnectionResponse));
            foreach (var tuple in myPropertyTuples)
            {
                tuple.Item1.SetValue(this, tuple.Item2, null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "BackOfficeConnectivityStatus", IsRequired = true, Order = 0)]
        public BackOfficeConnectivityStatus BackOfficeConnectivityStatus { get; private set; }

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
        /// 
        /// </summary>
        [DataMember(Name = "ConnectionCredentials", IsRequired = true, Order = 1)]
        public Dictionary<string, string> ConnectionCredentials { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "CompanyNameForDisplay", IsRequired = true, Order = 1)]
        public string CompanyNameForDisplay { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "CompanyUniqueIndentifier", IsRequired = true, Order = 1)]
        public string CompanyUniqueIndentifier { get; private set; }
        
        /// <summary>
        /// To support forward-compatible data contracts
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }
    }
}
