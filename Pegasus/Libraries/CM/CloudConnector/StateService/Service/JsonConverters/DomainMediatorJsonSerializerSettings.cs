using System.Runtime.Serialization.Formatters;
using Newtonsoft.Json;

namespace Sage.Connector.StateService.JsonConverters
{
    /// <summary>
    /// Default set of serializtion settings used by the Domain Mediators
    /// </summary>
    internal class DomainMediatorJsonSerializerSettings : JsonSerializerSettings
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
