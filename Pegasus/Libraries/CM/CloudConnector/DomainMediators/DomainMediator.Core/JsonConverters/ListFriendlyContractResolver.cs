using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Serialization;

namespace Sage.Connector.DomainMediator.Core.JsonConverters
{
    /// <summary>
    /// List resolver handles generic types
    /// TODO KMS: verify that this is needed.
    /// </summary>
    public class ListFriendlyContractResolver : DefaultContractResolver
    {
        /// <summary>
        /// Create the contract for the specified list type
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        protected override JsonContract CreateContract(Type objectType)
        {
            if (objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(IList<>))
                return new JsonArrayContract(objectType);
            if (objectType.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IList<>)))
                return new JsonArrayContract(objectType);
            return base.CreateContract(objectType);
        }

    }
}
