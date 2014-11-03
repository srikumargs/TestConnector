using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using Microsoft.WindowsAzure.Storage;
using Sage.Connector.Common;
using Sage.Connector.ConnectorServiceCommon;
using Sage.Connector.Data;
using Sage.Connector.Documents;
using Sage.Connector.Logging;
using Sage.Connector.MessagingService.ClientProxies;
using Sage.Connector.NotificationService.Proxy;
using Sage.Connector.Queues;
using Sage.Connector.Utilities;
using Sage.Connector.Cloud.Integration.Interfaces.Responses;
using Sage.Diagnostics;
using CloudContracts = Sage.Connector.Cloud.Integration.Interfaces.DataContracts;
using CloudInterfaceUtils = Sage.Connector.Cloud.Integration.Interfaces.Utils;
using CloudProxy = Sage.Connector.Cloud.Integration.Proxy;

namespace Sage.Connector.MessagingService.Internal
{
    /// <summary>
    /// The PutResponses worker unit.
    /// </summary>
    internal sealed class PutResponsesWorker
    {
        #region Private fields

        private static readonly String _myTypeName = typeof(PutResponsesWorker).FullName;
        private readonly Semaphore _limitCount;
        private APIResponseServiceProxy _responseService;
        private APIBeginUploadSessionServiceProxy _beginUploadSessionService;
        private APIConcludeUploadSessionServiceProxy _concludeUploadSessionService;
        private readonly Uri _responseEndpointAddress;
        private readonly String _tenantId;
        private readonly PutResponsesPolicy _policy;

        #endregion

        #region Private methods

        /// <summary>
        /// Cleanup routine to dispose of all owned IDisposable objects.
        /// </summary>
        private void CleanupDisposables()
        {
            if (_responseService != null)
            {
                _responseService.Dispose();
                _responseService = null;
            }

            if (_beginUploadSessionService != null)
            {
                _beginUploadSessionService.Dispose();
                _beginUploadSessionService = null;
            }

            if (_concludeUploadSessionService != null)
            {
                _concludeUploadSessionService.Dispose();
                _concludeUploadSessionService = null;
            }
        }

        /// <summary>
        /// Threaded work for sending the response message to the cloud.
        /// </summary>
        /// <param name="responseAsMessage">The response message.</param>
        /// <param name="cancel">The cancellation token.</param>
        private void DoInnerWork(StorageQueueMessage responseAsMessage, CancellationToken cancel)
        {
            try
            {
                using (new StackTraceContext(this, "_tenantId={0}", _tenantId))
                {
                    IEnumerable<String> uploadFiles;
                    var responseWrap = ProcessResponseMessage(responseAsMessage, out uploadFiles);

                    if (responseWrap == null ||
                        cancel.IsCancellationRequested)
                    {
                        PutResponseBackOnOutputQueue(responseAsMessage);
                        return;
                    }

                    using (var lm = new LogManager())
                    {
                        lm.WriteInfo(this, "READ response from outbound queue: ID={0}, RequestId={1}, MessageKind={2}", responseWrap.ResponseId, responseWrap.ActivityTrackingContext.RequestId, responseWrap.ResponseType);
                    }

                    var responseIsErrorResponse = (responseWrap.ResponseType == typeof(ErrorResponse).FullName);
                    var responseIsCancelResponse = false;

                    if (responseIsErrorResponse)
                    {
                        var resp = Utils.JsonDeserialize<ErrorResponse>(responseWrap.ResponsePayload);
                        responseIsCancelResponse = (null != resp && resp.ErrorResponseAction == ErrorResponseAction.Cancel);
                    }

                    if (!CloudPutResponse(responseWrap))
                    {
                        // Failed but retryable, try to put the response back on the output queue
                        // So that we will retry to put the response to the cloud.
                        // We do this here, because of the possibility of a transient cloud
                        // Side error, e.g. the service being down
                        PutResponseBackOnOutputQueue(responseAsMessage);
                        return;
                    }

                    using (var lm = new LogManager())
                    {
                        lm.AdvanceActivityState(this, responseWrap.ActivityTrackingContext, ActivityState.State17_ResponseSentToCloud, responseIsErrorResponse
                                    ? (responseIsCancelResponse ? ActivityEntryStatus.CompletedWithCancelResponse
                                        : ActivityEntryStatus.CompletedWithErrorResponse)
                                    : ActivityEntryStatus.CompletedWithSuccessResponse);
                    }

                    RemoveResponseFromOutputQueue(responseAsMessage, uploadFiles);
                    CloudConnectivityStateMonitorHelper.UpdateResponseCount(_tenantId, responseIsErrorResponse);
                }
            }
            finally
            {
                try
                {
                    _limitCount.Release();
                }
                catch (ObjectDisposedException)
                {
                }
                CleanupDisposables();
            }
        }

        /// <summary>
        /// Processes the repsonse message.
        /// </summary>
        /// <param name="responseAsMessage">The response to process.</param>
        /// <param name="uploadFiles"></param>
        /// <returns>The finalized response.</returns>
        private ResponseWrapper ProcessResponseMessage(StorageQueueMessage responseAsMessage, out IEnumerable<String> uploadFiles)
        {
            ResponseWrapper result = null;
            uploadFiles = new List<String>();

            using (var stc = new StackTraceContext(this, "_tenantId={0}", _tenantId))
            {
                if (null != responseAsMessage)
                {
                    var responseAsString = responseAsMessage.Payload;

                    using (var lm = new LogManager())
                    {
                        lm.WriteInfo(this, "Response read from output queue: {0}", responseAsString.Substring(0, Math.Min(512, responseAsString.Length)));
                    }

                    var responseWrapper = Utils.JsonDeserialize<ResponseWrapper>(responseAsString);

                    if (responseWrapper != null)
                    {
                        using (var lm = new LogManager())
                        {
                            lm.AdvanceActivityState(this, responseWrapper.ActivityTrackingContext, ActivityState.State15_DequeueTenantOutboxResponse, ActivityEntryStatus.InProgress);
                            result = FinalizeResponse(responseWrapper, out uploadFiles);

                            if (result != null)
                            {
                                lm.AdvanceActivityState(this, responseWrapper.ActivityTrackingContext, ActivityState.State16_UploadsCompletedAndResponseFinalized, ActivityEntryStatus.InProgress);
                                lm.WriteInfoForRequest(this, responseWrapper.ActivityTrackingContext, "Response: Id={0}, RequestId={1}, MessageKind={2}", result.ResponseId, result.ActivityTrackingContext.RequestId, result.ResponseType);
                            }
                            else
                            {
                                lm.WriteErrorForRequest(this, responseWrapper.ActivityTrackingContext, "Non-empty response failed to finalize");
                            }
                        }
                    }
                    else
                    {
                        using (var lm = new LogManager())
                        {
                            lm.WriteError(this, "Non-empty response failed to deserialize");
                        }
                    }
                }

                stc.SetResult(result);
            }

            return result;
        }

        /// <summary>
        /// Restores the queue message to allow for reprocessing. 
        /// </summary>
        /// <param name="responseAsMessage">The outbox message to restore.</param>
        private void PutResponseBackOnOutputQueue(StorageQueueMessage responseAsMessage)
        {
            if (null != responseAsMessage)
            {
                using (var qm = new QueueManager())
                {
                    try
                    {
                        qm.RestoreQueueMessage(responseAsMessage.Id);
                        SubsystemHealthHelper.ClearSubsystemHealthIssues(StateService.Interfaces.DataContracts.Subsystem.Queues);
                    }
                    catch (Exception ex)
                    {
                        using (var lm = new LogManager())
                        {
                            lm.WriteCriticalWithEventLogging(this, "Messaging Service", "Failed to put a failed response back on the output queue; exception: {0}", ex.ExceptionAsString());
                        }
                        SubsystemHealthHelper.RaiseSubsystemHealthIssue(StateService.Interfaces.DataContracts.Subsystem.Queues, ex.ExceptionAsString(), "Error restoring a message back to the tenant's outbox: " + ex.Message);

                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Removes the message from the outbox queue.
        /// </summary>
        /// <param name="responseAsMessage">The message to remove from the outbox queue.</param>
        /// <param name="uploadFiles"></param>
        private void RemoveResponseFromOutputQueue(StorageQueueMessage responseAsMessage, IEnumerable<String> uploadFiles)
        {
            if (responseAsMessage == null) return;

            using (var qm = new QueueManager())
            {
                try
                {
                    if (!qm.RemoveSpecificMessage(responseAsMessage.Id))
                    {
                        throw new Exception("Unexpected queue deletion failure for '" + responseAsMessage.Id + "'");
                    }
                    MarkFilesAsSent(uploadFiles);
                    SubsystemHealthHelper.ClearSubsystemHealthIssues(StateService.Interfaces.DataContracts.Subsystem.Queues);
                }
                catch (Exception ex)
                {
                    using (var lm = new LogManager())
                    {
                        lm.WriteCriticalWithEventLogging(this, "Messaging Service", "Failed to remove the response from the output queue; exception: {0}", ex.ExceptionAsString());
                    }
                    SubsystemHealthHelper.RaiseSubsystemHealthIssue(StateService.Interfaces.DataContracts.Subsystem.Queues, ex.ExceptionAsString(), "Failed to delete a message from the tenant's outbox: " + ex.Message);

                    throw;
                }
            }
        }

        /// <summary>
        /// Provide any additional work needed before the response is put on the outbound queue.
        /// </summary>
        /// <param name="responseWrapper">The response wrapper to work on.</param>
        /// <param name="uploadFiles"></param>
        /// <returns>The finalized response.</returns>
        private ResponseWrapper FinalizeResponse(ResponseWrapper responseWrapper, out IEnumerable<String> uploadFiles)
        {
            uploadFiles = new List<String>();
            if ((responseWrapper == null) || (responseWrapper.ActivityTrackingContext == null)) return null;

            if (responseWrapper.ResponseType != typeof(ErrorResponse).FullName)
            {
                return ExecuteResponseSpecificFileUploads(responseWrapper, out uploadFiles);
            }

            // Cloud does not currently properly process response payload
            // via blob.  Relying on file-based uploads to already to proper
            // shrinkage.
            return responseWrapper;
            //return CreateResponseIndirectPayload(responseWrapper);
        }

        /// <summary>
        /// Perform any response specific file uploads. For example, uploading a report file.
        /// </summary>
        /// <param name="responseWrapper">The response wrapper to work on.</param>
        /// <param name="uploadFiles"></param>
        /// <remarks>
        /// Note: possible to handle multiple file uploads here, and if so we consider a failure of one
        /// Upload to fail the whole response.
        /// </remarks>
        private ResponseWrapper ExecuteResponseSpecificFileUploads(ResponseWrapper responseWrapper, out IEnumerable<String> uploadFiles)
        {
            List<CloudContracts.UploadSessionInfo> fileUploads = new List<CloudContracts.UploadSessionInfo>();
            uploadFiles = new List<String>();

            if ((responseWrapper.Uploads == null) || (responseWrapper.ActivityTrackingContext == null)) return responseWrapper;

            uploadFiles = responseWrapper.Uploads.Select(uploadSpec => uploadSpec.FileName).ToList();

            foreach (var uploadSpec in responseWrapper.Uploads)
            {
                try
                {
                    CloudContracts.UploadSessionInfo fileUpload = null;
                    if (responseWrapper.ActivityTrackingContext != null)
                    {
                        fileUpload = UploadIndirectPayloadFile(responseWrapper.ActivityTrackingContext.RequestId, uploadSpec.FileName);
                    }
                    fileUploads.Add(fileUpload);
                }
                catch (Exception ex)
                {
                    FaultExceptionType feType = FaultHelper.ProcessFaultException(ex, this, _tenantId, null,
                        @"Messaging Service: exception encountered during file upload");
                    return null; // Keep files for upload retries
                }
            }
            responseWrapper.UploadSessions = fileUploads.ToArray();
            return responseWrapper;
        }

        /// <summary>
        /// Mark all files as sent so the document manager can perform cleanup.
        /// </summary>
        /// <param name="fileNames">The enumerable list of filenames.</param>
        private void MarkFilesAsSent(IEnumerable<string> fileNames)
        {
            var dm = new DocumentManager();

            using (var lm = new LogManager())
            {
                foreach (string fileName in fileNames)
                {
                    dm.FileSent(_tenantId, lm, _policy.Configuration, fileName);
                }
            }
        }

        /// <summary>
        /// Iterates through a list of property names to obtain the final end result property info.
        /// </summary>
        /// <param name="obj">The object to get the property info from.</param>
        /// <param name="names">The list of property names.</param>
        /// <returns>The property info for the final property name.</returns>
        private PropertyInfo GetPropertyInfo(object obj, string[] names)
        {
            if ((names == null) || (names.Length == 0)) throw new ArgumentNullException("names");

            PropertyInfo property = null;

            if (obj != null)
            {
                var currentObj = obj;

                foreach (var name in names)
                {
                    property = currentObj.GetType().GetProperty(name);
                    currentObj = property.GetValue(currentObj);
                }
            }

            return property;
        }

        /// <summary>
        /// Try best effort to shrink the response if necessary using an indirect payload.
        /// </summary>
        /// <param name="responseWrapper">The response wrapper.</param>
        private ResponseWrapper CreateResponseIndirectPayload(ResponseWrapper responseWrapper)
        {
            try
            {
                bool isWithinThreshold = (responseWrapper.ResponsePayload.LongCount() <=
                                          _policy.GetCurrentLargeResponseSizeThreshold());
                if (isWithinThreshold)
                    return responseWrapper;

                var uploadReference = UploadIndirectPayloadResponse(
                    responseWrapper.ActivityTrackingContext.RequestId,
                    responseWrapper.ResponsePayload);

                // Add our upload reference to the response wrapper upload sessions
                List<CloudContracts.UploadSessionInfo> uploadSessions = new List<CloudContracts.UploadSessionInfo>();
                if ((null != responseWrapper.UploadSessions) &&
                    (responseWrapper.UploadSessions.Any()))
                    uploadSessions.AddRange(responseWrapper.UploadSessions);
                uploadSessions.Add(uploadReference);

                // Construct the redirect response
                Response shrunkResponse = new Response(
                    responseWrapper.ActivityTrackingContext.RequestId,
                    responseWrapper.ResponseId,
                    DateTime.UtcNow,
                    uploadReference.DestinationName);
        
                // Construct the stripped shrunk response
                ResponseWrapper shrunkWrapper = new ResponseWrapper(
                    responseWrapper.ActivityTrackingContext,
                    responseWrapper.OriginalRequestPayload,
                    responseWrapper.ResponseId,
                    shrunkResponse.GetType().FullName,
                    Utils.JsonSerialize(shrunkResponse),
                    responseWrapper.Uploads)
                {
                    UploadSessions = uploadSessions.ToArray()
                };
                return shrunkWrapper;
            }
            catch (Exception ex)
            {
                // Send to the common fault processing logic
                FaultHelper.ProcessFaultException(
                    ex, this, _tenantId, null, @"Failure during creation of response indirect payload",
                    true, responseWrapper.ActivityTrackingContext);
                return responseWrapper;
            }
        }

        /// <summary>
        /// Pushes the finalized response message up to the cloud.
        /// </summary>
        /// <param name="responseWrap">The response message.</param>
        /// <returns>True if the response was successfully sent, otherwise false.</returns>
        private bool CloudPutResponse(ResponseWrapper responseWrap)
        {
            bool result = false;

            using (var stc = new StackTraceContext(this, "_tenantId={0}", _tenantId))
            {
                try
                {
                    DateTime timeToUpdate = DateTime.UtcNow;
                    CloudConnectivityStateMonitorHelper.UpdateLastCommunicationAttempt(_tenantId, timeToUpdate);

                    // Put the response to the cloud
                    // In case of fault, log the exception 
                    // In case of exception, allow to bubble up

                    // Cloud is currently tracking blob list as we request them,
                    // if we change strategy, we will need a mechanism to supply
                    // the cloud our responseWrap.UploadSessions
                    _responseService.PutResponse(
                        responseWrap.ActivityTrackingContext.RequestId,
                        responseWrap.ResponseType,
                        responseWrap.ResponsePayload,
                        null);
                    using (var lm = new LogManager())
                    {
                        lm.WriteInfo(this, "SENT response to cloud: Id={0}, RequestId={1}, MessageKind={2}", responseWrap.ResponseId, responseWrap.ActivityTrackingContext.RequestId, responseWrap.ResponseType);
                    }

                    // Succeeded
                    result = true;

                    timeToUpdate = DateTime.UtcNow;
                    CloudConnectivityStateMonitorHelper.UpdateLastSuccessfulCommunication(_tenantId, timeToUpdate);
                }
                catch (Exception ex)
                {
                    // Send to the common fault processing logic
                    FaultExceptionType feType = FaultHelper.ProcessFaultException(
                        ex, this, _tenantId, _responseEndpointAddress,
                        @"Messaging Service: An exception was encountered putting a response to the cloud");

                    // Fault exception type specific handling for this method
                    // TODO: want to add more types here? e.g. Retired Endpoint?
                    if (feType == FaultExceptionType.InvalidResponse ||
                        feType == FaultExceptionType.Serialization)
                    {
                        // Did not pass, but do not want to retry as this will never pass
                        result = true;
                    }
                }

                stc.SetResult(result);
            }

            return result;
        }

        /// <summary>
        /// Upload a local file to the cloud as an indirect payload.
        /// </summary>
        /// <param name="requestGuid">The identifier for the request message.</param>
        /// <param name="filename">The name of the file to upload.</param>
        /// <returns>Upload session information</returns>
        private CloudContracts.UploadSessionInfo UploadIndirectPayloadFile(Guid requestGuid, string filename)
        {
            using (var stc = new StackTraceContext(this, "filename={0}", filename))
            {
                ArgumentValidator.ValidateNonEmptyString(filename, "filename", _myTypeName + ".UploadIndirectPayloadFile()");

                var result = UploadIndirectPayload(requestGuid, filename, true);
                stc.SetResult(result);

                return result;
            }
        }

        /// <summary>
        /// Upload text version of the full original response.
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="originalResponse">The original response message.</param>
        /// <returns>Upload session information</returns>
        private CloudContracts.UploadSessionInfo UploadIndirectPayloadResponse(Guid requestId, String originalResponse)
        {
            using (var stc = new StackTraceContext(this, "originalResponse={0}", originalResponse))
            {
                ArgumentValidator.ValidateNonNullReference(originalResponse, "originalResponse", _myTypeName + ".UploadIndirectPayloadRequest()");

                var result = UploadIndirectPayload(requestId, originalResponse, false);
                stc.SetResult(result);

                return result;
            }
        }

        /// <summary>
        /// Gets a shared access key from the cloud, then uploads the content to that location and 
        /// returns the name of the file uploaded as the id.
        /// </summary>
        /// <param name="requestGuid">The guid for request.</param>
        /// <param name="content">Either the file name or actual data content to transfer to blob storage.</param>
        /// <param name="isFilename">True if content represents a filename, otherwise false.</param>
        /// <returns>Upload session information</returns>
        private CloudContracts.UploadSessionInfo UploadIndirectPayload(Guid requestGuid, string content, bool isFilename)
        {
            Guid uploadGuid = Guid.NewGuid();

            var uploadSessionInfo = _beginUploadSessionService.CreateAndBeginUploadSession(requestGuid, uploadGuid, String.Empty, String.Empty, -1);
            if (uploadSessionInfo == null) throw new InvalidOperationException("Could not create an upload session.");
            var destinationName = uploadSessionInfo.DestinationName;

            if (isFilename)
            {
                using (var fileStream = File.OpenRead(content))
                {
                    destinationName = PutBlob(fileStream, uploadSessionInfo.ContainerUri, destinationName);
                    //PutChunkedBlocks(fileStream, uploadSessionInfo.ContainerUri, uploadSessionInfo.ChunkSizeInBytes);
                }
            }
            else
            {
                using (var memoryStream = new MemoryStream(Encoding.Default.GetBytes(content)))
                {
                    destinationName = PutBlob(memoryStream, uploadSessionInfo.ContainerUri, destinationName);
                    //PutChunkedBlocks(memoryStream, uploadSessionInfo.ContainerUri, uploadSessionInfo.ChunkSizeInBytes);
                }
            }
            uploadSessionInfo.DestinationName = destinationName;

            _concludeUploadSessionService.ConcludeUploadSession(requestGuid, uploadGuid);

            return uploadSessionInfo;
        }

        /// <summary>
        /// Writes a stream to the webrequest
        /// </summary>
        /// <param name="fullStream"></param>
        /// <param name="request"></param>
        private void WriteStreamToRequest(Stream fullStream, WebRequest request)
        {
            var bufferSize = System.Convert.ToInt32(fullStream.Length);
            var buffer = new byte[bufferSize];
            fullStream.Read(buffer, 0, bufferSize);

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(buffer, 0, bufferSize);
            }
        }

        /// <summary>
        /// Puts the entire stream into the blob
        /// </summary>
        /// <param name="fullStream"></param>
        /// <param name="blobUri"></param>
        /// <param name="blobName"></param>
        private string PutBlob(Stream fullStream, Uri blobUri, String blobName)
        {
            if (Utils.MockUploadSessionInfo.ContainerUri.Equals(blobUri))
            {
                // Best attempt to upload to development storage mock container
                try
                {
                    blobName = Guid.NewGuid().ToString();
                    var blobClient = CloudStorageAccount.DevelopmentStorageAccount.CreateCloudBlobClient();
                    var container = blobClient.GetContainerReference("mockcontainer");
                    container.CreateIfNotExists();
                    var blockBlob = container.GetBlockBlobReference(blobName);
                    blockBlob.UploadFromStream(fullStream);
                }
                catch (Exception)
                {
                    blobName = String.Empty;
                }
                return blobName;
            }

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(blobUri);
            request.Method = "PUT";
            request.Headers.Add("x-ms-blob-type", "BlockBlob");
            WriteStreamToRequest(fullStream, request);
            using (HttpWebResponse resp = (HttpWebResponse)request.GetResponse())
            { }

            return blobName;
        }

        /// <summary>
        /// Uploads a file to blob storage using block access.
        /// </summary>
        /// <param name="fullStream">The complete file / memory stream to upload to blob storage.</param>
        /// <param name="blobUri">The azure block blob reference.</param>
        /// <param name="numBytesPerChunk">The number of bytes to use for each block.</param>
        private void PutChunkedBlocks(Stream fullStream, Uri blobUri, int numBytesPerChunk)
        {
            var blockIdList = new List<String>();
            var blockId = 0;

            while (fullStream.Position < fullStream.Length)
            {
                var bufferSize = (numBytesPerChunk < (fullStream.Length - fullStream.Position)) ? numBytesPerChunk : fullStream.Length - fullStream.Position;
                var buffer = new byte[bufferSize];

                fullStream.Read(buffer, 0, buffer.Length);

                using (var stream = new MemoryStream(buffer))
                {
                    stream.Position = 0;
                    var blockIdBase64 = System.Convert.ToBase64String(Encoding.UTF8.GetBytes(blockId.ToString("d6", CultureInfo.InvariantCulture)));
                    PutBlock(blobUri, blockIdBase64, stream);
                    blockIdList.Add(blockIdBase64);
                    blockId++;
                }
            }

            PutBlockList(blobUri, blockIdList);
        }

        private string RetrieveAuthorizationFromSessionKey(String sessionKey)
        {
            var parameters = HttpUtility.ParseQueryString(sessionKey);
            return "SharedKeyLite devstoreaccount1:" + parameters["sig"];
        }

        /// <summary>
        /// Stream a block to the blob block
        /// </summary>
        /// <param name="blobUri"></param>
        /// <param name="blockId"></param>
        /// <param name="streamToUpload"></param>
        private void PutBlock(Uri blobUri, String blockId, Stream streamToUpload)
        {
            if (Utils.MockUploadSessionInfo.ContainerUri.Equals(blobUri))
            {
                // Best attempt to upload to development storage mock container
                try
                {
                    var blobClient = CloudStorageAccount.DevelopmentStorageAccount.CreateCloudBlobClient();
                    var container = blobClient.GetContainerReference("mockcontainer");
                    container.CreateIfNotExists();
                    var blockBlob = container.GetBlockBlobReference(Guid.NewGuid().ToString());
                    blockBlob.PutBlock(blockId, streamToUpload, null);
                }
                catch (Exception)
                { }
                return;
            }

            Uri blockUri = new Uri(blobUri, "?comp=block&blockId="+blockId);
            var request = WebRequest.Create(blockUri);
            request.Method = "PUT";
            request.Headers.Add("x-ms-version", "2013-08-15");
            request.Headers.Add("x-ms-date", DateTime.UtcNow.ToLongTimeString());
            request.Headers.Add("Authorization", RetrieveAuthorizationFromSessionKey(blobUri.ToString()));

            WriteStreamToRequest(streamToUpload, request);

            using (var response = request.GetResponse())
            {
            }
        }

        /// <summary>
        /// Transforms the block list to an XML body of blocks
        /// </summary>
        /// <param name="blockIdList"></param>
        /// <returns></returns>
        private String BlockListToXMLStream(IEnumerable<String> blockIdList)
        {
            var xmlDoc = new XmlDocument();
            var blockList = xmlDoc.CreateNode(XmlNodeType.Element, "BlockList", String.Empty);
            foreach (var blockId in blockIdList)
            {
                var newBlockIdElement = xmlDoc.CreateNode(XmlNodeType.Element, "Latest", String.Empty);
                newBlockIdElement.InnerText = blockId;
                blockList.AppendChild(newBlockIdElement);
            }

            return xmlDoc.ToString();
        }

        /// <summary>
        /// Sends the block list to the cloud
        /// </summary>
        /// <param name="blobUri"></param>
        /// <param name="blockIdList"></param>
        private void PutBlockList(Uri blobUri, IEnumerable<string> blockIdList)
        {
            if (Utils.MockUploadSessionInfo.ContainerUri.Equals(blobUri))
            {
                // Best attempt to upload to development storage mock container
                try
                {
                    var blobClient = CloudStorageAccount.DevelopmentStorageAccount.CreateCloudBlobClient();
                    var container = blobClient.GetContainerReference("mockcontainer");
                    container.CreateIfNotExists();
                    var blockBlob = container.GetBlockBlobReference(Guid.NewGuid().ToString());
                    blockBlob.PutBlockList(blockIdList);
                }
                catch (Exception)
                { }
                return;
            }

            Uri blockListUri = new Uri(blobUri, "?comp=blocklist");
            var request = WebRequest.Create(blockListUri);
            request.Method = "PUT";
            request.Headers.Add("x-ms-version", "2013-08-15");
            request.Headers.Add("x-ms-date", DateTime.UtcNow.ToLongTimeString());
            request.Headers.Add("Authorization", RetrieveAuthorizationFromSessionKey(blobUri.ToString()));

            var xmlBlockList = BlockListToXMLStream(blockIdList);
            using (var blockListStream = new MemoryStream(Encoding.UTF8.GetBytes(xmlBlockList)))
            {
                WriteStreamToRequest(blockListStream, request);
                using (var response = request.GetResponse())
                {
                }
            }
        }

        

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the PutReponsesWorker class
        /// </summary>
        /// <param name="responseEndpointAddress"></param>
        /// <param name="responseService"></param>
        /// <param name="beginUploadSessionService"></param>
        /// <param name="concludeUploadSessionService"></param>
        /// <param name="tenantId"></param>
        /// <param name="policy"></param>
        /// <param name="limiter"></param>
        public PutResponsesWorker(Uri responseEndpointAddress, APIResponseServiceProxy responseService, APIBeginUploadSessionServiceProxy beginUploadSessionService,
            APIConcludeUploadSessionServiceProxy concludeUploadSessionService, String tenantId, PutResponsesPolicy policy, Semaphore limiter)
        {
            ArgumentValidator.ValidateNonNullReference(responseEndpointAddress, "responseEndpointAddress", _myTypeName + ".ctor()");
            ArgumentValidator.ValidateNonNullReference(responseService, "responseService", _myTypeName + ".ctor()");
            ArgumentValidator.ValidateNonNullReference(beginUploadSessionService, "beginUploadSessionService", _myTypeName + ".ctor()");
            ArgumentValidator.ValidateNonNullReference(concludeUploadSessionService, "concludeUploadSessionService", _myTypeName + ".ctor()");
            ArgumentValidator.ValidateNonEmptyString(tenantId, "tenantId", _myTypeName + ".ctor()");
            ArgumentValidator.ValidateNonNullReference(policy, "policy", _myTypeName + ".ctor()");

            _responseEndpointAddress = responseEndpointAddress;
            _responseService = responseService;
            _beginUploadSessionService = beginUploadSessionService;
            _concludeUploadSessionService = concludeUploadSessionService;
            _tenantId = tenantId;
            _limitCount = limiter;
            _policy = policy;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Retreives a single back-office response message from the the output queue and pushes them to the cloud.
        /// </summary>
        /// <returns>True or False depending on if there were any responses messages ready to be sent to the cloud.</returns>
        public Boolean DoWork(CancellationToken cancellationToken)
        {
            using (var stc = new StackTraceContext(this, "_tenantId={0}", _tenantId))
            {
                using (var queueManager = new QueueManager())
                {
                    StorageQueueMessage message = null;

                    try
                    {
                        message = queueManager.GetMessageFromOutput(_tenantId);
                        SubsystemHealthHelper.ClearSubsystemHealthIssues(StateService.Interfaces.DataContracts.Subsystem.Queues);
                    }
                    catch (Exception ex)
                    {
                        using (var lm = new LogManager())
                        {
                            lm.WriteCriticalWithEventLogging(this, "Messaging Service",
                                "Error retrieving a message from the tenant outbox; exception: {0}",
                                ex.ExceptionAsString());
                        }

                        SubsystemHealthHelper.RaiseSubsystemHealthIssue(
                            StateService.Interfaces.DataContracts.Subsystem.Queues,
                            ex.ExceptionAsString(), "Error retrieving a message from the tenant's outbox: " + ex.Message);

                        if (ex is System.Data.EntityException)
                        {
                            try
                            {
                                using (
                                    var proxy = NotificationServiceProxyFactory.CreateFromCatalog("localhost",
                                        ConnectorServiceUtils.CatalogServicePortNumber))
                                {
                                    proxy.NotifyTenantRestart(_tenantId);
                                }
                            }
                            catch (Exception)
                            {
                                message = null;
                            }
                        }
                    }

                    if (message == null)
                    {
                        stc.SetResult(false);
                        CleanupDisposables();
                        return false;
                    }

                    stc.SetResult(true);
                        
                    _limitCount.WaitOne();

                    try
                    {
                        Task.Factory.StartNew(() => DoInnerWork(message, cancellationToken), cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                    }
                    catch (Exception)
                    {
                        _limitCount.Release();
                        PutResponseBackOnOutputQueue(message);
                        CleanupDisposables();

                        throw;
                    }

                    return true;
                }
            }
        }

        #endregion
    }
}
