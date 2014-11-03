using System;
using System.ComponentModel.Composition;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sage.Connector.Discovery.Contracts.BackOffice;
using Sage.Connector.Discovery.Contracts.Data.Metadata;
using Sage.Connector.Discovery.Contracts.Integration.Responses;
using Sage.Connector.DomainContracts.BackOffice;
using Sage.Connector.DomainMediator.Core;
using Sage.Connector.DomainMediator.Core.JsonConverters;

namespace Sage.Connector.Discovery.Mediator
{
    /// <summary>
    /// This class uses the back office metadata to find all the plugins installed. 
    /// May not be needed many more but allows for meta data only resolution of meta data so preserved for now.
    /// Not currently use in from core connector
    /// </summary>
    [Export(typeof(IDomainFeatureRequest))]
    [FeatureMetadataExport(FeatureMessageTypes.GetBackOfficeConfiguration, typeof(FeatureDescriptions), "IDiscovery")]
    public class GetBackOfficeConfiguration : AbstractDomainMediator
    {

        [ImportMany]
#pragma warning disable 649
        private System.Collections.Generic.IEnumerable<Lazy<IDiscovery, IBackOfficeConfigMetadata>> _backOfficeHandlers;
#pragma warning restore 649


        /// <summary>
        /// Find plugs installed via their meta data and return the meta data information
        /// </summary>
        /// <param name="processContext">The <see cref="IProcessContext"/></param>
        /// <param name="requestId">The Connector Request Id</param>
        /// <param name="tenantId">The tenant Id making the request.</param>
        /// <param name="backOfficeConfiguration">The back office company configuration <see cref="IBackOfficeCompanyConfiguration"/></param>
        /// <param name="requestPayload">String representing the payload to process.</param>
        public override void FeatureRequest(IProcessContext processContext, Guid requestId, String tenantId, IBackOfficeCompanyConfiguration backOfficeConfiguration, string requestPayload)
        {
            CreateCompositionContainer(GetDiscoveryPluginsPartialPath());
            //does not need/have storage

            var backOfficeConfigProcessor = (from backOfficeHandler in _backOfficeHandlers
                                                select backOfficeHandler.Metadata);

            //no real plugin traffic here but to keep the queries sane adding plugin turnstiles
            processContext.TrackPluginInvoke();
            //If there aren't any then the response is empty
            var response = backOfficeConfigProcessor.ToList();
            processContext.TrackPluginComplete();

            var cfg = new DomainMediatorJsonSerializerSettings {ContractResolver = new ListFriendlyContractResolver()};
            cfg.Converters.Add(new BackOfficeConfigurationConverter());

            String responsePayload = JsonConvert.SerializeObject(response, cfg);
            processContext.ResponseHandler.HandleResponse(requestId, responsePayload);
        }


        internal class BackOfficeConfigurationConverter : CustomCreationConverter<IBackOfficeConfigMetadata>
        {
            public override IBackOfficeConfigMetadata Create(Type objectType)
            {
                return new BackOfficeConfig();
            }
        }


    }
}
