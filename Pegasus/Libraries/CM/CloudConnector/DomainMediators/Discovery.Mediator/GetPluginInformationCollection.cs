using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Newtonsoft.Json;
using Sage.Connector.Discovery.Contracts.BackOffice;
using Sage.Connector.Discovery.Contracts.Data;
using Sage.Connector.DomainContracts.BackOffice;
using Sage.Connector.DomainContracts.Data.Metadata;
using Sage.Connector.DomainMediator.Core;

namespace Sage.Connector.Discovery.Mediator
{

    /// <summary>
    /// This class finds all plugins and returns a collection of their information
    /// </summary>
    [Export(typeof(IDomainFeatureRequest))]
    [FeatureMetadataExport(FeatureMessageTypes.GetPluginInformationCollection, typeof(FeatureDescriptions), "IDiscovery")]
    public class GetPluginInformationCollection: AbstractDomainMediator
    {

        [ImportMany]
#pragma warning disable 649
        private IEnumerable<Lazy<IDiscovery, IBackOfficeData>> _backOfficeHandlers;
#pragma warning restore 649

        /// <summary>
        /// For each of the plug ins get the plugin information
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

            var backOfficeDiscoveryProcessors = (from backOfficeHandler in _backOfficeHandlers
                                                    select backOfficeHandler.Value);

            var response = new List<PluginInformation>();
            
            processContext.TrackPluginInvoke();
            foreach (IDiscovery plugin in backOfficeDiscoveryProcessors)
            {
                response.Add(plugin.GetPluginInformation(processContext.GetSessionContext()));
            }
            processContext.TrackPluginComplete();

            String responsePayload = JsonConvert.SerializeObject(response);
            processContext.ResponseHandler.HandleResponse(requestId, responsePayload);
        }
    }
}
