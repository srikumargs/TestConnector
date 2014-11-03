using System;

namespace Sage.Connector.Common.DataContracts
{
    /// <summary>
    /// 
    /// </summary>
    public static class RequestUtilities
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static string GetCustomerFacingRequestStatus(RequestStatus status)
        {
            switch (status)
            {
                case RequestStatus.CompletedWithErrorResponse:
                    return "Completed with error";
                case RequestStatus.InProgressBindableWorkProcessing:
                    return "In progress";
                case RequestStatus.InProgress:
                    return "Request pending";
                case RequestStatus.CompletedWithSuccessResponse:
                    return "Complete";
                case RequestStatus.CompletedWithCancelResponse:
                    return "Canceled";
                default:
                    return "";
            }
        }
    }
}
