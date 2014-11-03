using System.Runtime.Serialization.Formatters;
using Newtonsoft.Json;

namespace Sage.Connector.DomainMediator.Core.JsonConverters
{
    /// <summary>
    /// Default set of serializtion settings used by the Domain Mediators
    /// </summary>
    public class DomainMediatorJsonSerializerSettings : JsonSerializerSettings
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public DomainMediatorJsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.None;
            TypeNameAssemblyFormat = FormatterAssemblyStyle.Simple;
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;
            ObjectCreationHandling = ObjectCreationHandling.Auto;

        }
    }
}