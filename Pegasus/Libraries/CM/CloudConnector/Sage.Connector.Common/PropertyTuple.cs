using System;
using System.Reflection;

namespace Sage.Connector.Common
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class PropertyTuple
    {
        /// <summary>
        /// Initializes a new instance of the PropertyTuple class
        /// </summary>
        /// <param name="propertyInfo"></param>
        /// <param name="value"></param>
        /// <remarks>IEnumerable type must be passed in property storage type.</remarks>
        public PropertyTuple(PropertyInfo propertyInfo, Object value)
        {
            Item1 = propertyInfo;
            Item2 = value;
        }

        /// <summary>
        /// 
        /// </summary>
        public PropertyInfo Item1
        { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Object Item2
        { get; set; }
    }
}
