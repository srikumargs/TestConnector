using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sage.Connector.DomainContracts.BackOffice;
using Sage.Connector.DomainContracts.Responses;
using Sage.Connector.DomainMediator.Core;
using Sage.Connector.DomainMediator.Core.JsonConverters;

namespace Sage.Connector.Configuration.Mediator
{
    /// <summary>
    /// Sets up the Back Office Feature Configuration for Entry
    /// </summary>
    [Export(typeof(IDomainFeatureRequest))]
    [FeatureMetadataExport(FeatureMessageTypes.SaveCompanyFeaturePropertyValuePairs, typeof(FeatureDescriptions), "IManageFeatureConfiguration")]
    public class SaveCompanyFeaturePropertyValuePairs : AbstractDomainMediator
    {

        /// <summary>
        /// Feature Request implementation
        /// </summary>
        /// <param name="processContext">The <see cref="IProcessContext"/></param>
        /// <param name="requestId">The Connector Request Id</param>
        /// <param name="tenantId">The tenant Id making the request.</param>
        /// <param name="backOfficeConfiguration">The back office company configuration <see cref="IBackOfficeCompanyConfiguration"/></param>
        /// <param name="payload">String representing the payload to process.</param>
        public override void FeatureRequest(IProcessContext processContext, Guid requestId, string tenantId,
            IBackOfficeCompanyConfiguration backOfficeConfiguration, string payload)
        {
            CreateCompositionContainer(GetBackOfficePluginPartialPath(backOfficeConfiguration.BackOfficeId));
            SetupDataStoragePath(backOfficeConfiguration.DataStoragePath);

            var saveResponse = new Response();
            
            var cfg = new DomainMediatorJsonSerializerSettings
            {
                ContractResolver = new DictionaryFriendlyContractResolver()
            };
            cfg.Converters.Add(new KeyValuePairConverter());

            Dictionary<String, Dictionary<String, Object>> featurePropertyValuePairs = (String.IsNullOrWhiteSpace(payload))
                ? new Dictionary<String, Dictionary<String, Object>>()
                : JsonConvert.DeserializeObject<IList<KeyValuePair<String, IList<KeyValuePair<String, Object>>>>>(payload, cfg)
                    .ToDictionary(x => x.Key, x => x.Value.ToDictionary(y => y.Key, y => y.Value));

            if (featurePropertyValuePairs.Any())
            {
                foreach (var feature in featurePropertyValuePairs)
                {

                    //save the features in the property bag. 
                    using (var storageDictionary = GetDefaultPropertyValuesDictionary(tenantId, feature.Key, StorageMode.ReadWrite))
                    {
                        foreach (var propPair in feature.Value)
                        {
                            // Calling Add on a normal Dictionary implementation will throw an exception if the key already exists. 
                            // In the Storage Dictionary implementation, we perform an update on the value data (vs a record insert).

                            storageDictionary.Add(propPair.Key, propPair.Value);

                        }
                    }
                }
            }
            saveResponse.Status = Status.Success;

            var responsePayload = JsonConvert.SerializeObject(saveResponse, new DomainMediatorJsonSerializerSettings());
            processContext.ResponseHandler.HandleResponse(requestId, responsePayload);
        }
    }
}
