using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sage.Connector.Configuration.Contracts.BackOffice;
using Sage.Connector.Configuration.Contracts.Data.Responses;
using Sage.Connector.Configuration.Mediator.JsonConverters;
using Sage.Connector.DomainContracts.BackOffice;
using Sage.Connector.DomainContracts.Data.Metadata;
using Sage.Connector.DomainContracts.Responses;
using Sage.Connector.DomainMediator.Core;
using Sage.Connector.DomainMediator.Core.JsonConverters;

namespace Sage.Connector.Configuration.Mediator
{
    /// <summary>
    /// Validates the Back Office Feature Configuration
    /// </summary>
    [Export(typeof(IDomainFeatureRequest))]
    [FeatureMetadataExport(FeatureMessageTypes.ValidateCompanyFeaturePropertyValuePairs, typeof(FeatureDescriptions), "IManageFeatureConfiguration")]
    public class ValidateBackOfficeFeatureConfigurations : AbstractDomainMediator
    {

        [ImportMany]
#pragma warning disable 649
        private IEnumerable<Lazy<IManageFeatureConfiguration, IBackOfficeData>> _backOfficeHandlers;
        [ImportMany]
        private IEnumerable<Lazy<IDomainFeatureRequest, IFeatureMetaData>> _installedFeatures;
#pragma warning restore 649


        /// <summary>
        /// Validate the back office feature configuration
        /// </summary>
        /// <param name="processContext">The <see cref="IProcessContext"/></param>
        /// <param name="requestId">The Connector Request Id</param>
        /// <param name="tenantId">The tenant Id making the request.</param>
        /// <param name="backOfficeConfiguration">The back office company configuration <see cref="IBackOfficeCompanyConfiguration"/></param>
        /// <param name="requestPayload">The payload is Dictionary list of all the features for all 
        /// the back office implementations for this company which require validation.</param>
        public override void FeatureRequest(IProcessContext processContext, Guid requestId, String tenantId,
            IBackOfficeCompanyConfiguration backOfficeConfiguration, string requestPayload)
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

            var featureValidationResponses = new Dictionary<string, ICollection<PropertyValuePairValidationResponse>>();

            var processors = (from backOfficeHandler in _backOfficeHandlers
                              where backOfficeHandler.Metadata.BackOfficeId.Equals(backOfficeConfiguration.BackOfficeId)
                              select backOfficeHandler.Value).DefaultIfEmpty();

            var cfg = new DomainMediatorJsonSerializerSettings
            {
                ContractResolver = new DictionaryFriendlyContractResolver()
            };
            cfg.Converters.Add(new KeyValuePairConverter());
            cfg.Converters.Add(new AbstractDataTypeConverter());

            Dictionary<String, Dictionary<String, Object>> featurePropertyValuePairs = (String.IsNullOrWhiteSpace(requestPayload))
                ? new Dictionary<String, Dictionary<String, Object>>()
                : JsonConvert.DeserializeObject<IList<KeyValuePair<String, IList<KeyValuePair<String, Object>>>>>(requestPayload, cfg)
                    .ToDictionary(x => x.Key, x => x.Value.ToDictionary(y => y.Key, y => y.Value));


            var installedFeatures = (from installedFeatureHandler in _installedFeatures
                                     where !installedFeatureHandler.Metadata.InterfaceName.Equals("IManageFeatureConfiguration")
                                     select installedFeatureHandler.Metadata);

            IEnumerable<IFeatureMetaData> featureMetaDatas = installedFeatures as IFeatureMetaData[] ?? installedFeatures.ToArray();



            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (processors != null && featurePropertyValuePairs.Any())
            {

                foreach (IManageFeatureConfiguration processor in processors)
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
                    string featureName = featureMetaData.Name;
                    if (featureName == null)
                    {
                        EventLog.WriteEntry("Sage Connector", String.Format("Missing feature name for '{0}' interface.", featureMetaData.InterfaceName));
                        continue;

                    }
                    if (!featurePropertyValuePairs.ContainsKey(featureName))
                    {
                        continue;
                    }

                    var propertyValuePairs = featurePropertyValuePairs[featureName];


                    // ReSharper disable once SuspiciousTypeConversion.Global
                    var backOfficeSessionHandler = processor as IBackOfficeSessionHandler;
                    var response = new Response();
                    processContext.TrackPluginInvoke();
                    BeginBackOfficeSession(processContext.GetSessionContext(), backOfficeSessionHandler, backOfficeConfiguration, response);
                    try
                    {
                        var validation = processor.ValidateFeatureConfigurationValues(propertyValuePairs);
                        featureValidationResponses.Add(featureName, validation);
                    }
                    finally
                    {
                        if (backOfficeSessionHandler != null)
                        {
                            backOfficeSessionHandler.EndSession();
                        }
                        processContext.TrackPluginComplete();
                    }
                }
            }

            String responsePayload = JsonConvert.SerializeObject(featureValidationResponses.ToList(), cfg);
            processContext.ResponseHandler.HandleResponse(requestId, responsePayload);
        }
    }
}
