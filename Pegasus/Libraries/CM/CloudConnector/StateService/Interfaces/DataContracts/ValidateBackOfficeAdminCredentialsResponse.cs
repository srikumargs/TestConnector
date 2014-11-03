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
    [DataContract(Namespace = ServiceConstants.V1_SERVICE_NAMESPACE, Name = "ValidateBackOfficeAdminCredentialsResponseContract")]
    public sealed class ValidateBackOfficeAdminCredentialsResponse : IExtensibleDataObject
    {
        /// <summary>
        /// 
        /// </summary>
        public ValidateBackOfficeAdminCredentialsResponse(Boolean isValid, IEnumerable<String> userFacingMessages, IEnumerable<String> rawErrorMessages)
        {
            IsValid = isValid;

            if (userFacingMessages == null)
            {
                if (isValid)
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
                if (isValid)
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
        public ValidateBackOfficeAdminCredentialsResponse(ValidateBackOfficeAdminCredentialsResponse source, IEnumerable<PropertyTuple> propertyTuples)
        {
            IsValid = source.IsValid;
            UserFacingMessages = source.UserFacingMessages;
            RawErrorMessage = source.RawErrorMessage;
            ExtensionData = source.ExtensionData;

            var myPropertyTuples = propertyTuples.Where(x => x.Item1.DeclaringType == typeof(ValidateBackOfficeAdminCredentialsResponse));
            foreach (var tuple in myPropertyTuples)
            {
                tuple.Item1.SetValue(this, tuple.Item2, null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "IsValid", IsRequired = true, Order = 0)]
        public Boolean IsValid { get; private set; }

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
