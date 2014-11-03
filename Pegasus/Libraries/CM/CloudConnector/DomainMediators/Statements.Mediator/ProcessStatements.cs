using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using Sage.Connector.BillingAndPayments.Mediator.Features;
using Sage.Connector.DomainContracts.BackOffice;
using Sage.Connector.DomainContracts.Data.Metadata;
using Sage.Connector.DomainContracts.Responses;
using Sage.Connector.DomainMediator.Core;
using Sage.Connector.DomainMediator.Core.JsonConverters;
using Sage.Connector.DomainMediator.Core.Utilities;
using Sage.Connector.Statements.Contracts.BackOffice;
using Sage.Connector.Statements.Contracts.CloudIntegration.Responses;
using Sage.Connector.Statements.Contracts.Data.Requests;
using Sage.Connector.Statements.Contracts.Data.Responses;

namespace Sage.Connector.Statements.Mediator
{
    /// <summary>
    /// The SyncCustomers SyncShared Domain Feature implementation.
    /// </summary>
    [Export(typeof(IDomainFeatureRequest))]
    [FeatureMetadataExport(FeatureMessageTypes.ProcessStatements, typeof(FeatureDescriptions), "IProcessStatements")]
    public class ProcessStatements : AbstractDomainMediator
    {
        [ImportMany]
#pragma warning disable 649
        private IEnumerable<Lazy<IProcessStatements, IBackOfficeData>> _backOfficeHandlers;
#pragma warning restore 649
        /// <summary>
        /// The ProcessStatements Feature Request Implementation. 
        /// </summary>
        /// <param name="processContext">The <see cref="IProcessContext"/></param>
        /// <param name="requestId">The Connector Request Id</param>
        /// <param name="tenantId">The tenant Id making the request.</param>
        /// <param name="backOfficeCompanyConfiguration">The back office company configuration <see cref="IBackOfficeCompanyConfiguration"/></param>
        /// <param name="payload">String representing the payload to process.</param>
        public override void FeatureRequest(IProcessContext processContext, Guid requestId, String tenantId,
            IBackOfficeCompanyConfiguration backOfficeCompanyConfiguration, string payload)
        {
            var statementsResponse = new StatementsResponse();
            try
            {
                if (backOfficeCompanyConfiguration == null)
                    throw new NoNullAllowedException("Back Office Company Configuration must exist to process feature request.");
                
                CreateCompositionContainer(GetBackOfficePluginPartialPath(backOfficeCompanyConfiguration.BackOfficeId));
                SetupDataStoragePath(backOfficeCompanyConfiguration.DataStoragePath);
                
                IProcessStatements backofficeFeatureProcessor = (from backOfficeHandler in _backOfficeHandlers
                                                                 where backOfficeHandler.Metadata.BackOfficeId.Equals(backOfficeCompanyConfiguration.BackOfficeId,
                                                                 StringComparison.CurrentCultureIgnoreCase)
                                                                 select backOfficeHandler.Value).FirstOrDefault();


                if (backofficeFeatureProcessor == null)
                {
                    Debug.Print("{0} BackOffice to Process Statements was not found!", backOfficeCompanyConfiguration.BackOfficeId);
                    throw new ApplicationException(String.Format("{0} BackOffice to Process Statements was not found!", backOfficeCompanyConfiguration.BackOfficeId));
                }

                if (String.IsNullOrWhiteSpace(payload))
                {
                    throw new ArgumentNullException(String.Format("{0} error: Missing request payload.",
                        FeatureMessageTypes.ProcessStatements));
                }

                //Request Payload Deserialization
                Debug.Print("Process Statements Request Payload {0}", payload);

                var payloadSettings = new DomainMediatorJsonSerializerSettings();
                var statementsRequest = JsonConvert.DeserializeObject<StatementsRequest>(payload, payloadSettings);
                ExternalIdUtilities.ApplyExternalIdDecoding(statementsRequest);

                //Initialize response
                var statementResponses = new Collection<StatementResponse>();
                statementsResponse.StatementResponses = statementResponses;

                // ReSharper disable once SuspiciousTypeConversion.Global
                var backOfficeSessionHandler = backofficeFeatureProcessor as IBackOfficeSessionHandler;

                try
                {
                    processContext.TrackPluginInvoke();
                    BeginBackOfficeSession(processContext.GetSessionContext(), backOfficeSessionHandler, backOfficeCompanyConfiguration, statementsResponse);
                    InitializeFeaturePropertyDefaults(backofficeFeatureProcessor, tenantId, statementsResponse);
                    CheckResponse(backofficeFeatureProcessor.InitializeProcessStatements(statementsRequest), statementsResponse);

                    int statementCount = statementsRequest.CustomerReferences.Count;
                    for (int i = 0; i < statementCount; i++)
                    {
                        StatementResponse statementResponse;
                        try
                        {
                            statementResponse = backofficeFeatureProcessor.GetNextStatement();

                            if (statementResponse == null)
                                continue;

                            ExternalIdUtilities.ApplyExternalIdEncoding(statementResponse);
                            statementResponses.Add(statementResponse);

                        }
                        catch (Exception ex)
                        {
                            statementResponse = new StatementResponse();
                            ProcessException(statementResponse, ex);
                            statementResponses.Add(statementResponse);
                        }
                    }

                }
                catch (Exception ex)
                {
                    ProcessException(statementsResponse, ex);
                }
                finally
                {
                    backOfficeSessionHandler.EndSession();
                    processContext.TrackPluginComplete();
                }
            }
            catch (Exception ex)
            {
                ProcessException(statementsResponse, ex);
            }

            if (!statementsResponse.Status.Equals(Status.Failure))
            {
                statementsResponse.Status = Status.Success;
            }

            String responsePayload = JsonConvert.SerializeObject(statementsResponse, new DomainMediatorJsonSerializerSettings());
            processContext.ResponseHandler.HandleResponse(requestId, responsePayload);
        }
    }
}
