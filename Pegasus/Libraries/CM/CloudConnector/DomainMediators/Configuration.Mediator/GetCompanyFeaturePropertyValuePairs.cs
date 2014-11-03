using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sage.Connector.DomainContracts.BackOffice;
using Sage.Connector.DomainMediator.Core;
using Sage.Connector.DomainMediator.Core.JsonConverters;

namespace Sage.Connector.Configuration.Mediator
{
    /// <summary>
    /// Sets up the Back Office Feature Configuration for Entry
    /// </summary>
    [Export(typeof(IDomainFeatureRequest))]
    [FeatureMetadataExport(FeatureMessageTypes.GetCompanyFeaturePropertyValuePairs, typeof(FeatureDescriptions), "IManageFeatureConfiguration")]
    public class GetCompanyFeaturePropertyValuePairs : AbstractDomainMediator
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

            var cfg = new DomainMediatorJsonSerializerSettings
            {
                ContractResolver = new DictionaryFriendlyContractResolver()
            };
            cfg.Converters.Add(new KeyValuePairConverter());

            var featureList = JsonConvert.DeserializeObject<IList<string>>(payload, cfg);

            Dictionary<String, IDictionary<String, Object>> featurePropertyValuePairs = new Dictionary<String, IDictionary<String, Object>>();
            foreach (var feature in featureList)
            {
                //Get the features in the property bag. 
                using (var storageDictionary = GetDefaultPropertyValuesDictionary(tenantId, feature, StorageMode.ReadOnly))
                {
                    KeyValuePair<String, Object>[] storageCopy = new KeyValuePair<string, object>[storageDictionary.Count];

                    storageDictionary.CopyTo(storageCopy, 0);
                    var featurePairs = storageCopy.ToDictionary(x => x.Key, y => y.Value);
                    featurePropertyValuePairs.Add(feature, featurePairs);

                }
            }


            var responsePayload = JsonConvert.SerializeObject(featurePropertyValuePairs, cfg);
            processContext.ResponseHandler.HandleResponse(requestId, responsePayload);
        }
    }
}
