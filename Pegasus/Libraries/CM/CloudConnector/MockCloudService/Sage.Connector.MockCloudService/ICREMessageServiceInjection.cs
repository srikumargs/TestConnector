using System.Collections.Generic;
using System.ServiceModel;
using Sage.Connector.Cloud.Integration.Interfaces.Requests;
using Sage.Connector.Cloud.Integration.Interfaces.Responses;

namespace Sage.Connector.SageCloudService
{
    /// <summary>
    /// Service contract for external manipulate of cloud/premise
    /// message stores (for unit testing)
    /// </summary>
    [ServiceContract]
    public interface ICREMessageServiceInjection
    {
        /// <summary>
        /// May soon be deprecated, but allow external
        /// (unit testing) to peek at outgoing messages
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        [OperationContract]
        IEnumerable<Request> ExternalPeekOutboxMessage(string tenantId);

        /// <summary>
        /// May soon be deprecated, but allow external
        /// (unit testing) to peek at incoming messages
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        [OperationContract]
        IEnumerable<Response> ExternalPeekInboxMessage(string tenantId);

        /// <summary>
        /// May soon be deprecated, but allow external
        /// (unit testing) to inject a message intended for the premise
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [OperationContract]
        bool ExternalAddToPremiseMessage(string tenantId, Request message);
    }
}