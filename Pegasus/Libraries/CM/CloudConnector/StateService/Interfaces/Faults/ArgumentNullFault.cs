using System;
using System.Runtime.Serialization;
using Sage.Connector.LinkedSource;

namespace Sage.Connector.StateService.Interfaces.Faults
{
    /// <summary>
    /// Exception thrown when an invalid argument is specified.
    /// </summary>
    [DataContract(Namespace = ServiceConstants.V1_SERVICE_NAMESPACE, Name = "ArgumentNullFaultContract")]
    public class ArgumentNullFault : ArgumentFault
    {
        /// <summary>
        /// Initializes a new instance of the InvalidArgumentFault class
        /// </summary>
        /// <param name="argumentName"></param>
        /// <param name="message"></param>
        public ArgumentNullFault(String argumentName, String message)
            :base(argumentName, message)
        { }

        ///// <summary>
        ///// Initializes a new instance of the InvalidArgumentFault class from an existing instance and a collection of propertyTuples
        ///// </summary>
        ///// <param name="source"></param>
        ///// <param name="propertyTuples"></param>
        //public ArgumentNullFault(InvalidArgumentFault source, IEnumerable<PropertyTuple> propertyTuples)
        //{
        //    ArgumentName = source.ArgumentName;
        //    Message = source.Message;

        //    var myPropertyTuples = propertyTuples.Where(x => x.Item1.DeclaringType == typeof(ArgumentNullFault));
        //    foreach (var tuple in myPropertyTuples)
        //    {
        //        tuple.Item1.SetValue(this, tuple.Item2, null);
        //    }
        //}
    }
}
