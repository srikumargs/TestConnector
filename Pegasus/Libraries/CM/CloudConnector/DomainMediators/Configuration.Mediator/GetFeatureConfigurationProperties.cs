using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Newtonsoft.Json;
using Sage.Connector.Configuration.Contracts.BackOffice;
using Sage.Connector.Configuration.Contracts.Data;
using Sage.Connector.DomainContracts.BackOffice;
using Sage.Connector.DomainContracts.Data.Metadata;
using Sage.Connector.DomainMediator.Core;
using Sage.Connector.DomainMediator.Core.JsonConverters;

namespace Sage.Connector.Configuration.Mediator
{
    /// <summary>
    /// Get Feature Configuration Properties
    /// </summary>
    [Export(typeof(IDomainFeatureRequest))]
    [FeatureMetadataExport(FeatureMessageTypes.GetFeatureConfigurationProperties, typeof(FeatureDescriptions), "IManageFeatureConfiguration")]
    public class GetFeatureConfigurationProperties : AbstractDomainMediator
    {

#pragma warning disable 649
        [ImportMany]
        private IEnumerable<Lazy<IManageFeatureConfiguration, IBackOfficeData>> _backOfficeHandlers;
        [ImportMany]
        private IEnumerable<Lazy<IDomainFeatureRequest, IFeatureMetaData>> _installedFeatures;

        private const String BackOfficeIdFeatureIdentifierFormat = "{0}_{1}";

#pragma warning restore 649

        /// <summary>
        /// Get Feature Configuration Properties for all features of this back office. 
        /// </summary>
        /// <param name="processContext">The <see cref="IProcessContext"/></param>
        /// <param name="requestId">The Connector Request Id</param>
        /// <param name="tenantId">The tenant Id making the request.</param>
        /// <param name="backOfficeConfiguration">The back office company configuration <see cref="IBackOfficeCompanyConfiguration"/></param>
        /// <param name="requestPayload">String representing the payload to process.</param>
        public override void FeatureRequest(IProcessContext processContext, Guid requestId, String tenantId, IBackOfficeCompanyConfiguration backOfficeConfiguration, string requestPayload)
        {
            //this uses both 'phases' of mef resolution to process. 
            // so need to add paths for both process execution and plugins
            List<string> paths = new List<string>()
            {
                GetBasePartialPath(),
                GetBackOfficePluginPartialPath(backOfficeConfiguration.BackOfficeId)
            };
            CreateCompositionContainer(paths);
            SetupDataStoragePath(backOfficeConfiguration.DataStoragePath);

            var installedFeatures = (from installedFeatureHandler in _installedFeatures
                                     where !installedFeatureHandler.Metadata.InterfaceName.Equals("IManageFeatureConfiguration")
                                     select installedFeatureHandler.Metadata);

            IEnumerable<IFeatureMetaData> featureMetaDatas = installedFeatures as IFeatureMetaData[] ?? installedFeatures.ToArray();

            var processors = (from backOfficeHandler in _backOfficeHandlers
                              where backOfficeHandler.Metadata.BackOfficeId.Equals(backOfficeConfiguration.BackOfficeId)
                              select backOfficeHandler.Value);

            IDictionary<IFeatureMetaData, ICollection<PropertyDefinition>> featureMetaPropertyLists = new Dictionary<IFeatureMetaData, ICollection<PropertyDefinition>>();
            processContext.TrackPluginInvoke();
            foreach (var processor in processors)
            {
                IFeatureMetaData featureMetaData;
                try
                {
                    featureMetaData = (from interfaceType in processor.GetType().GetInterfaces()
                                                        join inst in featureMetaDatas on interfaceType.Name equals inst.InterfaceName
                                                        select inst).FirstOrDefault();
                }
                catch (Exception)
                {
                    //ignore bad implementation from the back office. 
                    continue;
                }

                if (featureMetaData == null)
                {
                    //no feature configurations for this processor
                    continue;
                }

                ICollection<PropertyDefinition> propertyDefinitions = processor.GetFeatureProperties();

                var storageId = String.Format(BackOfficeIdFeatureIdentifierFormat, backOfficeConfiguration.BackOfficeId,
                    featureMetaData.Name);

                //The list should have then entire Feature meta data for display in the configuration page. 
                if (propertyDefinitions != null)
                {
                    using (var featurePropertyStorage = new StorageDictionary<String, String>(GetFeatureDataStoragePath(),
                            storageId))
                    {
                        foreach (var prop in propertyDefinitions)
                        {
                            featurePropertyStorage.Add(prop.PropertyName, JsonConvert.SerializeObject(prop,
                                new DomainMediatorJsonSerializerSettings()));
                        }
                    }
                    featureMetaPropertyLists.Add(featureMetaData, propertyDefinitions);
                }
                else
                {
                    //remove property definition
                    RemoveStorageDictionary(GetFeatureDataStoragePath(), storageId);
                }
            }
            processContext.TrackPluginComplete();

            var cfg = new DomainMediatorJsonSerializerSettings();


            var response = featureMetaPropertyLists.ToList();
            String responsePayload = JsonConvert.SerializeObject(response, cfg);
            processContext.ResponseHandler.HandleResponse(requestId, responsePayload);
        }
    }
}
