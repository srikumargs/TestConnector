using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Newtonsoft.Json;
using Sage.Connector.Configuration.Contracts.BackOffice;
using Sage.Connector.Configuration.Contracts.Data.Responses;
using Sage.Connector.DomainContracts.BackOffice;
using Sage.Connector.DomainContracts.Data.Metadata;
using Sage.Connector.DomainMediator.Core;

namespace Sage.Connector.Configuration.Mediator
{
    /// <summary>
    ///  Validate the back office connection request feature support
    /// </summary>
    [Export(typeof(IDomainFeatureRequest))]
    [FeatureMetadataExport(FeatureMessageTypes.ValidateCompanyConnectionManagementCredentials, typeof(FeatureDescriptions), "IVerifyCredentials")]
    public class ValidateCompanyConnectionManagementCredentials : AbstractDomainMediator
    {

        [ImportMany]
#pragma warning disable 649
        private IEnumerable<Lazy<IVerifyCredentials, IBackOfficeData>> _backOfficeHandlers;
#pragma warning restore 649


        /// <summary>
        /// Validate the back office connection request 
        /// </summary>
        /// <param name="processContext">The <see cref="IProcessContext"/></param>
        /// <param name="requestId">The Connector Request Id</param>
        /// <param name="tenantId">The tenant Id making the request.</param>
        /// <param name="backOfficeConfiguration">The back office company configuration <see cref="IBackOfficeCompanyConfiguration"/></param>
        /// <param name="payload">String representing the payload to process.</param>
        public override void FeatureRequest(IProcessContext processContext, Guid requestId, String tenantId,
            IBackOfficeCompanyConfiguration backOfficeConfiguration, string payload)
        {
            ValidateCompanyConnectionManagementCredentialsResponse requestResponse = new ValidateCompanyConnectionManagementCredentialsResponse();
            try
            {
                CreateCompositionContainer(GetBackOfficePluginPartialPath(backOfficeConfiguration.BackOfficeId));
                //does not need/have storage

                string backOfficeId = backOfficeConfiguration.BackOfficeId;

                //Find the instance of the interface/featureProcessor to use.
                var featureProcessor = Common.FindFeatureProcessor(_backOfficeHandlers, backOfficeId);
                Common.ValidateFeatureProcessor(featureProcessor, backOfficeId);

                IDictionary<string, string> cmCredentials = JsonConvert.DeserializeObject<IDictionary<string, string>>(payload);

                processContext.TrackPluginInvoke();
                requestResponse = featureProcessor.ValidateCompanyConnectionManagementCredentials(processContext.GetSessionContext(),cmCredentials);
                processContext.TrackPluginComplete();

                Common.ValidateResponse(requestResponse, backOfficeId, FeatureMessageTypes.ValidateCompanyConnectionManagementCredentials);
            }
            catch (Exception ex)
            {
                //note fills in the response structure
                ProcessException(requestResponse, ex);

            }

            String responsePayload = JsonConvert.SerializeObject(requestResponse);
            processContext.ResponseHandler.HandleResponse(requestId, responsePayload);
        }
    }
}
