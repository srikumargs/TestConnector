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
    /// Validate the company connection credentials feature support
    /// </summary>
    [Export(typeof(IDomainFeatureRequest))]
    [FeatureMetadataExport(FeatureMessageTypes.ValidateCompanyConnectionCredentials, typeof(FeatureDescriptions), "IVerifyCredentials")]
    public class ValidateCompanyConnectionCredentials : AbstractDomainMediator
    {

        [ImportMany]
#pragma warning disable 649
        private IEnumerable<Lazy<IVerifyCredentials, IBackOfficeData>> _backOfficeHandlers;
#pragma warning restore 649


        /// <summary>
        /// Validate the company connection credentials 
        /// </summary>
        /// <param name="processContext">The <see cref="IProcessContext"/></param>
        /// <param name="requestId">The Connector Request Id</param>
        /// <param name="tenantId">The tenant Id making the request.</param>
        /// <param name="backOfficeConfiguration">The back office company configuration <see cref="IBackOfficeCompanyConfiguration"/></param>
        /// <param name="payload">String representing the payload to process.</param>
        public override void FeatureRequest(IProcessContext processContext, Guid requestId, String tenantId,
            IBackOfficeCompanyConfiguration backOfficeConfiguration, string payload)
        {
            ValidateCompanyConnectionCredentialsResponse requestResponse = new ValidateCompanyConnectionCredentialsResponse();
            try
            {
                CreateCompositionContainer(GetBackOfficePluginPartialPath(backOfficeConfiguration.BackOfficeId));
                //does not need/have storage

                string backOfficeId = backOfficeConfiguration.BackOfficeId;

                //Find the instance of the interface/featureProcessor to use.
                var featureProcessor = Common.FindFeatureProcessor(_backOfficeHandlers, backOfficeId);
                Common.ValidateFeatureProcessor(featureProcessor, backOfficeId);

                IDictionary<string, string> ccCredentials = JsonConvert.DeserializeObject<IDictionary<string, string>>(payload);

                processContext.TrackPluginInvoke();
                requestResponse = featureProcessor.ValidateCompanyConnectionCredentials(processContext.GetSessionContext(),ccCredentials);
                processContext.TrackPluginComplete();

                Common.ValidateResponse(requestResponse, backOfficeId, FeatureMessageTypes.ValidateCompanyConnectionCredentials);
            }
            catch (Exception ex)
            {
                //note fills in the response structure
                ProcessException(requestResponse, ex);

            }

            String responsePayload = JsonConvert.SerializeObject(requestResponse);
            ////Package up the response
            //var cfg = new DomainMediatorJsonSerializerSettings {ContractResolver = new ListFriendlyContractResolver()};

            //String responsePayload = JsonConvert.SerializeObject(response, cfg);
            processContext.ResponseHandler.HandleResponse(requestId, responsePayload);
        }
    }
}
