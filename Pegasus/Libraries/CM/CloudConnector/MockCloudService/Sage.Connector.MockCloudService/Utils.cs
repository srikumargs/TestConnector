using System;
using System.IO;
using System.Runtime.Serialization;
using System.ServiceModel.Web;
using System.Text;
using System.Xml;
using Sage.Connector.ConnectorServiceCommon;
using Sage.Connector.Cloud.Integration.Interfaces.DataContracts;
using Sage.Connector.Cloud.Integration.Interfaces.Headers;

namespace Sage.CRE.Cloud.WebService.Connector
{
    internal static class ServiceUtils
    {
        /// <summary>
        /// Get the necessary data out of the current HTTP header
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="premiseAgent"></param>
        internal static void GetHttpHeaderData(out Guid tenantId, out PremiseAgent premiseAgent)
        {
            WebOperationContext ctx = WebOperationContext.Current;
            tenantId = GetGuid(ctx.IncomingRequest.Headers[
                HeaderCommon.ServiceHeaderKeys[ServiceHeaderKeyTypes.TenantId]]);

            var rawAgent = ctx.IncomingRequest.Headers[
                HeaderCommon.ServiceHeaderKeys[ServiceHeaderKeyTypes.PremiseAgent]];

            premiseAgent = Utils.JsonDeserialize<PremiseAgent>(rawAgent);
        }

        /// <summary>
        /// Dinky little helper method to throw an exception type that this service
        /// Recognizes as client facing when we cannot parse a GUID (for tenantId)
        /// </summary>
        /// <param name="guidString"></param>
        /// <returns></returns>
        internal static Guid GetGuid(string guidString)
        {
            Guid tenantGuid;
            try
            {
                tenantGuid = Guid.Parse(guidString);
            }
            catch (Exception ex)
            {
                // TODO: throw service specific exception (e.g. PremiseCommunicationException)
                //Take a look at how the real cloud treats this (connectivity fault?)
                throw new ArgumentException(ex.Message);
            }
            return tenantGuid;
        }
    }
}
