using System;
using System.AddIn.Hosting;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Sage.Connector.Cloud.Integration.Interfaces.DataContracts;
using Sage.Connector.Cloud.Integration.Interfaces.Requests;
using Sage.Connector.Cloud.Integration.Interfaces.Responses;
using Sage.Connector.Common;
using Sage.Connector.ConnectorServiceCommon;
using Sage.Connector.Data;
using Sage.Connector.Documents;
using Sage.Connector.Logging;
using Sage.Connector.ProcessExecution.Events;
using Sage.Connector.ProcessExecution.RequestActivator;
using Sage.Connector.Utilities;
using Sage.Diagnostics;

namespace Sage.Connector.Binding
{
    /// <summary>
    /// Binds posting request to back office posting
    /// </summary>
    public class DomainMediationBinder : IBindable
    {
        #region Private Members

        private const string DomainMediationError = "Domain Mediation Error";
        private static readonly String _myTypeName = typeof(DomainMediationBinder).FullName;
        private readonly ConcurrentDictionary<Guid, DomainMediatorResponseWrapper> _dmResponseWrappers = new ConcurrentDictionary<Guid, DomainMediatorResponseWrapper>();
        private readonly ConcurrentDictionary<Guid, ResponseWrapper> _responseWrappers = new ConcurrentDictionary<Guid, ResponseWrapper>();

        #endregion

        #region IBindable Members

        /// <summary>
        /// Indicates if this IBindable supports the provided request kind
        /// </summary>
        /// <param name="requestKind">The request kind to process.</param>
        /// <returns>True if the request is supported, otherwise false.</returns>
        public bool SupportRequestKind(string requestKind)
        {
            return requestKind.Equals(typeof(DomainMediationRequest).FullName);
        }

        /// <summary>
        /// The actual work of processing the request
        /// TODO: Multiple response, which requires more work than expected in the Dispatch. 
        /// TODO: Keeping the mechanism which events the multiple responses,however, in the initial wire-frame
        /// TODO: only eventing once with the one response.  So the module level variable should be set to be returned
        /// TODO: when the task finishes.
        /// </summary>
        /// <param name="requestWrapper"></param>
        /// <param name="cancellationTokenSource"></param>
        /// <param name="maxBlobSize"></param>
        /// <returns></returns>
        public ResponseWrapper InvokeWork(RequestWrapper requestWrapper, CancellationTokenSource cancellationTokenSource, long maxBlobSize)
        {
            ArgumentValidator.ValidateNonNullReference(requestWrapper, "requestWrapperMessage",
                _myTypeName + ".InvokeWork()");
            ArgumentValidator.ValidateNonNullReference(requestWrapper.ActivityTrackingContext, "requestWrapperMessage.ActivityTrackingContext",
                _myTypeName + ".InvokeWork()");
            if (!SupportRequestKind(requestWrapper.ActivityTrackingContext.RequestType))
            {
                throw new ArgumentException(String.Format("Unsupported request type: {0}",
                    requestWrapper.ActivityTrackingContext.RequestType));
            }

            var id = requestWrapper.ActivityTrackingContext.RequestId;
            ResponseWrapper response;
            uint retryCount;

            try
            {
                string tenantId = requestWrapper.ActivityTrackingContext.TenantId;
                // If the tenant doesn't exist then create an error response with a delete action
                PremiseConfigurationRecord iConfiguration =
                    ConfigurationSettingFactory.RetrieveConfiguration(tenantId);
                if (iConfiguration == null)
                {
                    ErrorResponse configurationMissingErrorResponse = ErrorResponseUtils.GenerateErrorResponse(
                        requestWrapper.ActivityTrackingContext.RequestId, ConfigurationSettingFactory.TENANT_CONFIG_NOT_FOUND, string.Empty);

                    return new ResponseWrapper(requestWrapper, configurationMissingErrorResponse, null);
                }

                DomainMediationRequest domainMediationRequest =
                    Utils.JsonDeserialize<DomainMediationRequest>(requestWrapper.RequestPayload);
                retryCount = domainMediationRequest.RetryCount;

                if (!_dmResponseWrappers.TryAdd(id, new DomainMediatorResponseWrapper(requestWrapper, domainMediationRequest.DomainMediationEntry, maxBlobSize)))
                {
                    throw new ArgumentException(@"Failed to add the domain response wrapper", id.ToString());
                }

                using (var lm = new LogManager())
                {
                    lm.SetActivityInnerType(this, requestWrapper.ActivityTrackingContext, domainMediationRequest.DomainMediationEntry.DomainFeatureRequest);
                }

                // Here we want to call the correct platform process execution shim to handle the request
                //TODO: DomainMediation Contract for back office needs to know the platform type 
                //TODO: It is probable that some back office plug ins are x86 only, such as Sage 300 ERP 
                //TODO KMS: RESEARCH: Is there a way to get a catalog and metadata of feature for the back office before we call into 
                //          the dll to determine which platform?   

                //TODO KMS: RESEARCH how to create factory for hot/warm/colde process feature. 
                var processRequestActivation = new ProcessRequestActivation(true, Platform.X86);

                //get the path for data storage for this tenant
                var dataStoragePath = DocumentManager.GetTenantDataStorageFolder(tenantId);

                var boCompanyConfig = new BackOfficeCompanyConfigurationObject
                    {
                        BackOfficeId = iConfiguration.ConnectorPluginId,
                        ConnectionCredentials = iConfiguration.BackOfficeConnectionCredentials,
                        DataStoragePath = dataStoragePath
                    };

                processRequestActivation.ExecuteProcessRequest(
                    Request_ProcessResponse,
                    requestWrapper.ActivityTrackingContext,
                    boCompanyConfig,
                    domainMediationRequest.DomainMediationEntry.DomainFeatureRequest,
                    domainMediationRequest.DomainMediationEntry.Payload,
                    cancellationTokenSource);


                _responseWrappers.TryRemove(id, out response);
            }
            catch (Exception ex)
            {
                using (var lm = new LogManager())
                {
                    lm.WriteErrorForRequest(this, requestWrapper.ActivityTrackingContext, ex.ExceptionAsString());
                }
                throw;
            }
            finally
            {
                DomainMediatorResponseWrapper dmResponse;

                if (_dmResponseWrappers.TryRemove(id, out dmResponse))
                {
                    dmResponse.Dispose();
                }
            }


            if (response == null)
            {
                // No response created, default to a retry error response
                ErrorResponse genericErrorResponse =
                    ErrorResponseUtils.GenerateRetryErrorResponse(requestWrapper.ActivityTrackingContext.RequestId,
                        DomainMediationError, string.Empty, retryCount, null);
                response = new ResponseWrapper(requestWrapper, genericErrorResponse, null);
            }

            return response;
        }

        #endregion

        private static bool NoBlob()
        {
            String noBlob = Environment.GetEnvironmentVariable("SAGE_CONNECTOR_NOBLOB", EnvironmentVariableTarget.Machine);
            return !String.IsNullOrEmpty(noBlob) && noBlob == "1";
        }


        private void Request_ProcessResponse(object sender, ResponseEventArgs e)
        {
            Debug.Print("Host: Work progressing: {0}", e.Payload);

            //We're going to stop the add-in if it ever reports progress > 50%
            Debug.Print("Host: Request Response processing: {0}", e.Payload);

            DomainMediatorResponseWrapper dmResponseWrapper;

            if (!_dmResponseWrappers.TryGetValue(e.RequestId, out dmResponseWrapper))
            {
                throw new ApplicationException("Missing Request Information for response");
            }

            // Currently _always_ allocate a content writer as ALL cloud content will be
            // via BLOB, eventually this needs to be conditional on a configuration parameter
            if (dmResponseWrapper.ContentWriter == null)
            {
                if (!e.Completed || !NoBlob())
                    dmResponseWrapper.AllocContentWriter();
            }


            if (!e.Completed)
            {
                dmResponseWrapper.ContentWriter.Write(e.Payload);
                return;
            }

            DomainMediation dmResponseEntry;

            if (dmResponseWrapper.ContentWriter != null)
            {
                dmResponseWrapper.ContentWriter.Write(e.Payload);
                dmResponseWrapper.ContentWriter.Close();

                dmResponseEntry = new DomainMediation(dmResponseWrapper.DomainMediationEntry.UniqueIdentifier,
                    dmResponseWrapper.DomainMediationEntry.DomainFeatureRequest,
                    String.Empty, "blob");
                // TODO: Inject payload version?
            }
            else
            {
                dmResponseEntry = new DomainMediation(dmResponseWrapper.DomainMediationEntry.UniqueIdentifier,
                    dmResponseWrapper.DomainMediationEntry.DomainFeatureRequest,
                    e.Payload, dmResponseWrapper.DomainMediationEntry.PayloadType);
                // TODO: Inject payload version?
            }

            // Not exactly sure what the id is for the domain mediator.
            // Create the successful post dm response
            var innerResponse = new DomainMediationRequestResponse(e.RequestId, Guid.NewGuid(), DateTime.UtcNow, dmResponseEntry);

            ResponseWrapper response;

            if (dmResponseWrapper.ContentWriter != null)
            {
                var uploads = dmResponseWrapper.ContentWriter.Select(file => new ResponseWrapperUploadSpecification("Uploads", file)).ToList();
                
                response = new ResponseWrapper(dmResponseWrapper.RequestWrapper, innerResponse, uploads.ToArray());
            }
            else
            {
                response = new ResponseWrapper(dmResponseWrapper.RequestWrapper, innerResponse, null);
            }

            _responseWrappers.TryAdd(e.RequestId, response);

            using (var lm = new LogManager())
            {
                ActivityTrackingContext atc = new ActivityTrackingContext(e.TrackingId, e.TenantId, e.RequestId, string.Empty);
                lm.AdvanceActivityState(null, atc, ActivityState.State12_ProcessExecutionComplete, ActivityEntryStatus.InProgress);
                //lm.WriteInfoForRequest(null, rw.ActivityTrackingContext, "Invoking " + ib.GetType().FullName + " on request '" + rw.ActivityTrackingContext.RequestId + "'");

            }
        }
    }
}
