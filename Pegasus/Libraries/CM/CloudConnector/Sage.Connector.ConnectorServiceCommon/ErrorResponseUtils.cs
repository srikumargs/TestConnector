using System;
using System.Collections.Generic;
using System.Linq;
using Sage.Connector.Cloud.Integration.Interfaces.Requests;
using Sage.Connector.Common;
using Sage.Connector.Cloud.Integration.Interfaces.Responses;
using CloudDataContracts = Sage.Connector.Cloud.Integration.Interfaces.DataContracts;
using PremiseContracts = Sage.CRE.CloudConnector.Integration.Interfaces.DataContracts;

namespace Sage.Connector.ConnectorServiceCommon
{
    /// <summary>
    /// Provide some utilities for error response generation
    /// </summary>
    public static class ErrorResponseUtils
    {
        #region Public Methods

        /// <summary>
        /// Generate the retry error response
        /// Will create a delete response if above threshold for retries
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="errors"></param>
        /// <param name="retryCount"></param>
        /// <param name="suggestedRetryInterval"></param>
        /// <returns>Generated error response</returns>
        public static ErrorResponse GenerateRetryErrorResponse(
            Guid requestId,
            PremiseContracts.ErrorInformation[] errors,
            UInt32 retryCount,
            TimeSpan? suggestedRetryInterval)
        {
            // Check the retry threshold
            if (retryCount > _maxRetries)
            {
                // Exceeded retry threshold: fail it
                return GenerateErrorResponse(requestId, errors, ErrorResponseAction.Fail);
            }

            CloudDataContracts.ErrorInformation[] cloudErrors = Convert.ToCloudErrorInformationList(errors);
            
            // Create the retry error response with the suggested interval
            ErrorResponse errorResponse = new ErrorResponse(
                requestId,
                Guid.NewGuid(),
                DateTime.UtcNow,
                cloudErrors,
                ErrorResponseAction.Retry,
                suggestedRetryInterval);
           
            return errorResponse;
        }

        /// <summary>
        /// Generate the retry error response
        /// Will create a delete response if above threshold for retries
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="rawErrorMessage"></param>
        /// <param name="userFacingErrorMessage"></param>
        /// <param name="retryCount"></param>
        /// <param name="suggestedRetryInterval"></param>
        /// <returns>Generated error response</returns>
        public static ErrorResponse GenerateRetryErrorResponse(
            Guid requestId,
            string rawErrorMessage,
            string userFacingErrorMessage,
            UInt32 retryCount,
            TimeSpan? suggestedRetryInterval)
        {
            // Check the retry threshold
            if (retryCount > _maxRetries)
            {
                // Exceeded retry threshold: fail it
                return GenerateErrorResponse(requestId, rawErrorMessage, userFacingErrorMessage, ErrorResponseAction.Fail);
            }

            CloudDataContracts.ErrorInformation[] cloudErrors =
                new CloudDataContracts.ErrorInformation[]
                    {
                        new CloudDataContracts.ErrorInformation(rawErrorMessage, userFacingErrorMessage)
                    };

            // Create the retry error response with the suggested retry interval
            ErrorResponse errorResponse = new ErrorResponse(
                requestId,
                Guid.NewGuid(),
                DateTime.UtcNow,
                cloudErrors,
                ErrorResponseAction.Retry,
                suggestedRetryInterval);

            return errorResponse;
        }

        /// <summary>
        /// Generate error response without extra retry data
        /// Defaults to Fail error response action
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="errorMessages"></param>
        /// <param name="errorResponseAction"></param>
        /// <returns></returns>
        public static ErrorResponse GenerateErrorResponse(
            Guid requestId,
            PremiseContracts.ErrorInformation[] errorMessages,
            ErrorResponseAction errorResponseAction = ErrorResponseAction.Fail)
        {
            CloudDataContracts.ErrorInformation[] cloudErrors = 
                Convert.ToCloudErrorInformationList(errorMessages);

            // Create the failed error response
            ErrorResponse errorResponse = new ErrorResponse(
                requestId,
                Guid.NewGuid(),
                DateTime.UtcNow,
                cloudErrors,
                errorResponseAction, 
                null);

            return errorResponse;
        }

        /// <summary>
        /// Generate error response without extra retry data
        /// Defaults to Fail error response action
        /// </summary>
        /// <param name="requestId"></param>
        /// <param name="rawErrorMessage"></param>
        /// <param name="userFacingErrorMessage"></param>
        /// <param name="errorResponseAction"></param>
        /// <returns></returns>
        public static ErrorResponse GenerateErrorResponse(
            Guid requestId,
            string rawErrorMessage,
            string userFacingErrorMessage,
            ErrorResponseAction errorResponseAction = ErrorResponseAction.Fail)
        {
            CloudDataContracts.ErrorInformation[] cloudErrors =
                new CloudDataContracts.ErrorInformation[]
                    {
                        new CloudDataContracts.ErrorInformation(rawErrorMessage, userFacingErrorMessage)
                    };

            // Create the delete error response
            ErrorResponse errorResponse = new ErrorResponse(
                requestId,
                Guid.NewGuid(),
                DateTime.UtcNow,
                cloudErrors,
                errorResponseAction,
                null);

            return errorResponse;
        }

        /// <summary>
        /// For requests to the back end, a common method to create an error response based
        /// On the errors returned, or to provide a generic error message for the request 
        /// Type if the error list is empty
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="caller"></param>
        /// <param name="requestWrapper"></param>
        /// <param name="errors"></param>
        /// <param name="suggestedRetryInterval"></param>
        /// <param name="traces"></param>
        /// <param name="logging"></param>
        /// <returns></returns>
        public static ResponseWrapper GenerateErrorResponseFromBackOfficeError<T>(
            object caller, 
            RequestWrapper requestWrapper,
            PremiseContracts.ErrorInformation[] errors, 
            TimeSpan? suggestedRetryInterval,
            IList<String> traces, 
            ILogging logging)
            where T : PremiseContracts.Response
        {
            // Create the error string
            string errorString =
                (errors != null && errors.Count() > 0)
                ? String.Join(",\n", errors.Select((ei) => ei.RawErrorMessage))
                : String.Format("Unspecified back office error processing request of type '{0}'", typeof(T).Name);
            
            // Log the errors
            logging.ErrorTrace(caller, errorString);
            
            // Log the traces, if any
            if (traces != null && traces.Count() > 0)
            {
                logging.InfoTrace(caller, String.Join(",\n", traces));
            }

            // Retrieve the retry count
            var request = Utils.JsonDeserialize<Request>(requestWrapper.RequestPayload);

            // Create a retry error response
            ErrorResponse errorResponse =
                ErrorResponseUtils.GenerateRetryErrorResponse(
                    requestWrapper.ActivityTrackingContext.RequestId,
                    errors,
                    (request != null) ? request.RetryCount : 0,
                    suggestedRetryInterval);

            // Package up as a response wrapper
            ResponseWrapper response = new ResponseWrapper(
                requestWrapper,
                errorResponse.Id,
                errorResponse.GetType().FullName,
                Utils.JsonSerialize(errorResponse),
                null);

            return response;
        }

        #endregion



        #region Private Members

        private static readonly UInt32 _maxRetries = ConnectorRegistryUtils.ErrorResponseRetryMax;

        #endregion
    }
}
