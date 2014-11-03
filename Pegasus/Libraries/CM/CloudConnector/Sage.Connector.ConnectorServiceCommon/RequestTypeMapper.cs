using System;
using Sage.Connector.Cloud.Integration.Interfaces.DataContracts;
using Sage.Connector.Cloud.Integration.Interfaces.Requests;

namespace Sage.Connector.ConnectorServiceCommon
{
    /// <summary>
    /// 
    /// </summary>
    public static class RequestTypeMapper
    {
        /// <summary>
        /// Maps request type to a displayable name
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static String RequestDisplayName(Request request)
        {
            if (null == request)
            {
                return String.Empty;
            }
            if (request is DomainMediationRequest)
            {
                return Resource1.Request_DomainMediationDisplayName; 
            }
            if (request is GetLogRequest)
            {
                return Resource1.GetLogRequestDisplayName;
            }
            if (request is GetMetricsRequest)
            {
                return Resource1.GetMetricsRequestDisplayname;
            }
            if (request is HealthCheckRequest)
            {
                return Resource1.HealthCheckRequestDisplayName;
            }
            if (request is LoopBackRequest)
            {
                return Resource1.LoopBackRequestDisplayName;
            }
            if (request is UpdateConfigParamsRequest)
            {
                return Resource1.UpdateConfigParamRequestDisplayName;
            }
            if (request is UpdateSiteServiceInfoRequest)
            {
                return Resource1.UpdateSiteServiceInfoRequestDisplayName;
            }

            return request.GetType().FullName;
        }

        /// <summary>
        /// Requests the name of the secondary type.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public static string RequestInnerTypeName(Request request)
        {
            //Note: think this path maybe not be in use. So may not need this function.
            //primary setting of the secondary type happens in binding after the domain mediation type is deserialized.

            //TODO: expand with more end user friendly names for the requests maybe.
            //This has a dependency implication. We cant really know the request types running thru the system nor should we.
            //To do a really good job at this we need to get the mappings to end user friendly versions from the domain mediation system

            DomainMediationRequest dmRequest = request as DomainMediationRequest;
            if (dmRequest != null && dmRequest.DomainMediationEntry != null)
            {
                return dmRequest.DomainMediationEntry.DomainFeatureRequest;
            }

            return String.Empty;
        }
    }
}
