using System;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using Sage.Connector.Discovery.Contracts.BackOffice;
using Sage.Connector.DomainContracts.BackOffice;
using Sage.Connector.DomainContracts.Data.Metadata;
using Sage.Connector.DomainContracts.Responses;
using Sage.Connector.DomainMediator.Core;

namespace Sage.Connector.Discovery.Mediator
{
    /// <summary>
    /// Validate Back Office is installed
    /// </summary>
    [Export(typeof(IDomainFeatureRequest))]
    [FeatureMetadataExport(FeatureMessageTypes.ValidateBackOfficeIsInstalled, typeof(FeatureDescriptions), "IDiscovery")]
    public class ValidateBackOfficeIsInstalled : AbstractDomainMediator
    {

        [ImportMany]
#pragma warning disable 649
        private System.Collections.Generic.IEnumerable<Lazy<IDiscovery, IBackOfficeData>> _backOfficeHandlers;
#pragma warning restore 649


        /// <summary>
        /// Report if the back office is installed 
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

            var validateConnectionProcessor = (from backOfficeHandler in _backOfficeHandlers 
                                               where backOfficeHandler.Metadata.BackOfficeId.Equals(backOfficeConfiguration.BackOfficeId, StringComparison.CurrentCultureIgnoreCase) 
                                               select backOfficeHandler.Value).FirstOrDefault();

            
            //TODO: update strings look at general system for these so every request type is not special.
            if (validateConnectionProcessor == null)
            {
                Debug.Print("{0} BackOffice to Process the connection validation was not found Not Found!", backOfficeConfiguration.BackOfficeId);
                throw new ApplicationException(String.Format("{0} BackOffice to Process the connection validation  was not found Not Found!", backOfficeConfiguration.BackOfficeId));
            }

            processContext.TrackPluginInvoke();
            bool isInstalled = validateConnectionProcessor.IsBackOfficeInstalled(processContext.GetSessionContext());
            processContext.TrackPluginComplete();

            var response = new Response
            {
                Status = (isInstalled ? Status.Success : Status.Failure)
            };
            String responsePayload = JsonConvert.SerializeObject(response);
            processContext.ResponseHandler.HandleResponse(requestId, responsePayload);
        }
    }
}