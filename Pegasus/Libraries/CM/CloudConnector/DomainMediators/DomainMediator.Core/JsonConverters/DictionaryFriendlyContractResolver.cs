using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Serialization;

namespace Sage.Connector.DomainMediator.Core.JsonConverters
{
    /// <summary>
    /// Converter for the Dictionary Friendly Contract
    /// </summary>
    public class DictionaryFriendlyContractResolver : DefaultContractResolver
    {
        /// <summary>
        /// Determines which contract type is created for the given type.
        /// </summary>
        /// <param name="objectType">Type of the object.</param>
        /// <returns>
        /// A <see cref="T:Newtonsoft.Json.Serialization.JsonContract"/> for the given type.
        /// </returns>
        protected override JsonContract CreateContract(Type objectType)
        {
            if (objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                return new JsonArrayContract(objectType);
            if (objectType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDictionary<,>)))
                return new JsonArrayContract(objectType);
            return base.CreateContract(objectType);
        }

    }
}
