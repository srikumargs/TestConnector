using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Sage.Connector.Common;
using Sage.Connector.LinkedSource;

namespace Sage.Connector.StateService.Interfaces.DataContracts
{
    /// <summary>
    /// 
    /// </summary>
     [DataContract(Namespace = ServiceConstants.V1_SERVICE_NAMESPACE, Name = "ManagementCredentialsNeededResponse")]
    public class ManagementCredentialsNeededResponse : IExtensibleDataObject
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="descriptions"></param>
        /// <param name="currentValues"></param>
        /// <param name="userFacingMessages"></param>
        /// <param name="rawErrorMessages"></param>
        public ManagementCredentialsNeededResponse(string descriptions, Dictionary<string,string> currentValues , IEnumerable<String> userFacingMessages, IEnumerable<String> rawErrorMessages)
        {
            DescriptionsAsString = descriptions ?? string.Empty; ;
            CurrentValues = currentValues ?? new Dictionary<string, string>();
            
            userFacingMessages = userFacingMessages ?? new String[] { };
            UserFacingMessages = userFacingMessages.ToArray();
            
            rawErrorMessages = rawErrorMessages?? new String[] { };
            RawErrorMessage = rawErrorMessages.ToArray();
        }

        /// <summary>
        /// Initializes a new instance of the class from an existing instance and a collection of propertyTuples
        /// </summary>
        /// <param name="source"></param>
        /// <param name="propertyTuples"></param>
        public ManagementCredentialsNeededResponse(ManagementCredentialsNeededResponse source, IEnumerable<PropertyTuple> propertyTuples)
        {
            //Descriptions = source.Descriptions;
            DescriptionsAsString = source.DescriptionsAsString;
            CurrentValues = source.CurrentValues;
            UserFacingMessages = source.UserFacingMessages;
            RawErrorMessage = source.RawErrorMessage;
            ExtensionData = source.ExtensionData;

            var myPropertyTuples = propertyTuples.Where(x => x.Item1.DeclaringType == typeof(ManagementCredentialsNeededResponse));
            foreach (var tuple in myPropertyTuples)
            {
                tuple.Item1.SetValue(this, tuple.Item2, null);
            }
        }
        ///// <summary>
        ///// 
        ///// </summary>
        //[DataMember(Name = "Descriptions", IsRequired = true, Order = 1)]
        //public Dictionary<string, object> Descriptions { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "DescriptionsAsString", IsRequired = true, Order = 1)]
        public string DescriptionsAsString { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "CurrentValues", IsRequired = true, Order = 1)]
        public Dictionary<string, string> CurrentValues { get; set; }

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
