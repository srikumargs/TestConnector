using System;
using System.Runtime.Serialization;
using Sage.Connector.LinkedSource;

namespace Sage.Connector.StateService.Interfaces.Faults
{
    /// <summary>
    /// Exception thrown when an invalid argument is specified.
    /// </summary>
    [DataContract(Namespace = ServiceConstants.V1_SERVICE_NAMESPACE, Name = "ArgumentFaultContract")]
    public class ArgumentFault : IExtensibleDataObject
    {
        /// <summary>
        /// Initializes a new instance of the InvalidArgumentFault class
        /// </summary>
        /// <param name="argumentName"></param>
        /// <param name="message"></param>
        public ArgumentFault(String argumentName, String message)
        {
            ArgumentName = argumentName;
            Message = message;
        }

        ///// <summary>
        ///// Initializes a new instance of the InvalidArgumentFault class from an existing instance and a collection of propertyTuples
        ///// </summary>
        ///// <param name="source"></param>
        ///// <param name="propertyTuples"></param>
        //public ArgumentFault(InvalidArgumentFault source, IEnumerable<PropertyTuple> propertyTuples)
        //{
        //    ArgumentName = source.ArgumentName;
        //    Message = source.Message;

        //    var myPropertyTuples = propertyTuples.Where(x => x.Item1.DeclaringType == typeof(ArgumentFault));
        //    foreach (var tuple in myPropertyTuples)
        //    {
        //        tuple.Item1.SetValue(this, tuple.Item2, null);
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "ArgumentName", IsRequired = true, Order = 0)]
        public String ArgumentName { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "Message", IsRequired = true, Order = 1)]
        public String Message { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>To support forward-compatible data contracts</remarks>
        public ExtensionDataObject ExtensionData { get; set; }
    }
}
