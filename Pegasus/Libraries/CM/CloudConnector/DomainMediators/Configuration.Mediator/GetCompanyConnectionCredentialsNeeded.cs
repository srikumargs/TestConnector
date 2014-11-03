using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Resources;
using Newtonsoft.Json;
using Sage.Connector.Configuration.Contracts.BackOffice;
using Sage.Connector.Configuration.Contracts.Data;
using Sage.Connector.Configuration.Contracts.Data.Responses;
using Sage.Connector.DomainContracts.BackOffice;
using Sage.Connector.DomainContracts.Data.Metadata;
using Sage.Connector.DomainMediator.Core;

namespace Sage.Connector.Configuration.Mediator
{
    /// <summary>
    /// Get Company Connection Credentials Needed feature support
    /// </summary>
    [Export(typeof(IDomainFeatureRequest))]
    [FeatureMetadataExport(FeatureMessageTypes.GetCompanyConnectionCredentialsNeeded, typeof(FeatureDescriptions), "IVerifyCredentials")]
    public class GetCompanyConnectionCredentialsNeeded : AbstractDomainMediator
    {

        [ImportMany]
#pragma warning disable 649
        private System.Collections.Generic.IEnumerable<Lazy<IVerifyCredentials, IBackOfficeData>> _backOfficeHandlers;
#pragma warning restore 649


        /// <summary>
        /// Get Company Connection Credentials Needed
        /// </summary>
        /// <param name="processContext">The <see cref="IProcessContext"/></param>
        /// <param name="requestId">The Connector Request Id</param>
        /// <param name="tenantId">The tenant Id making the request.</param>
        /// <param name="backOfficeConfiguration">The back office company configuration <see cref="IBackOfficeCompanyConfiguration"/></param>
        /// <param name="payload">String representing the payload to process.</param>
        public override void FeatureRequest(IProcessContext processContext, Guid requestId, String tenantId, 
            IBackOfficeCompanyConfiguration backOfficeConfiguration, string payload)
        {
            CompanyConnectionCredentialsNeededResponse requestResponse = new CompanyConnectionCredentialsNeededResponse();
            try
            {
                CreateCompositionContainer(GetBackOfficePluginPartialPath(backOfficeConfiguration.BackOfficeId)); 
                //does not need/have storage

                string backOfficeId = backOfficeConfiguration.BackOfficeId;

                //Find the instance of the interface/featureProcessor to use.
                var featureProcessor = Common.FindFeatureProcessor(_backOfficeHandlers, backOfficeId);
                Common.ValidateFeatureProcessor(featureProcessor, backOfficeId);

                //IDictionary<string, string> cmCredentials = JsonConvert.DeserializeObject<IDictionary<string, string>>(payload);

                CompanyCredentials cmCredentials = JsonConvert.DeserializeObject<CompanyCredentials>(payload);

                processContext.TrackPluginInvoke();
                requestResponse = featureProcessor.GetCompanyConnectionCredentialsNeeded(processContext.GetSessionContext(), cmCredentials);
                processContext.TrackPluginComplete();

                Common.ValidateResponse(requestResponse, backOfficeId, 
                    new ResourceManager(typeof(FeatureDescriptions)).GetString(FeatureMessageTypes.GetCompanyConnectionCredentialsNeeded));

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
