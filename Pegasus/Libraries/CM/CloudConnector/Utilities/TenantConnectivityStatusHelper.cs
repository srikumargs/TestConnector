using System;
using System.Net.NetworkInformation;
using Sage.Connector.Common;
using Sage.Connector.ConnectorServiceCommon;
using Sage.Connector.Logging;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.Connector.Utilities.Internal;

namespace Sage.Connector.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class TenantConnectivityStatusHelper
    {
        static TenantConnectivityStatusHelper()
        {
            // cause the ServerCertificateValidationCallback to be setup
            CloudUtils.SetupServerCertificateValidationCallback();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static TenantConnectivityStatus GetTenantConnectivityStatus(String tenantId, Uri uri)
        {
            var result = TenantConnectivityStatus.None;
            using (var stc = new StackTraceContext(null))
            {
                try
                {
                    if (NetworkInterface.GetIsNetworkAvailable())
                    {
                        result = TenantConnectivityStatus.LocalNetworkUnavailable;

                        if (ConnectivityStatusUtils.BasicTcpConnectivityTest(uri, ConnectorRegistryUtils.InternetConnectivityStatusTcpConnectTimeout))
                        {
                            result = TenantConnectivityStatus.Normal;
                        }
                        else
                        {
                            // basic connectivity to our cloud site just failed.  As an experiment, try out Microsoft's NCSI site to determine if this is 
                            // a problem larger than just connectivity to the our site.

                            var microsoftNCSIResult = ConnectivityStatusUtils.MicrosoftNCSITest();
                            if (microsoftNCSIResult == MicrosoftNCSITestResult.Success)
                            {
                                result = TenantConnectivityStatus.CloudUnavailable;
                            }
                            else
                            {
                                result = TenantConnectivityStatus.InternetConnectionUnavailable;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    using (var lm = new LogManager())
                    {
                        lm.WriteError(null, ex.ExceptionAsString());
                    }
                }

                stc.SetResult(result);
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="getGatewayServiceUriResponse"></param>
        /// <returns></returns>
        public static TenantConnectivityStatus MapToTenantConnectivtyStatus(GetGatewayServiceUriResponse getGatewayServiceUriResponse)
        {
            TenantConnectivityStatus result = TenantConnectivityStatus.None;

            if (getGatewayServiceUriResponse != null)
            {
                if (getGatewayServiceUriResponse.Result == GetGatewayServiceUriResult.Success)
                {
                    result = TenantConnectivityStatus.Normal;
                }
                else if (getGatewayServiceUriResponse.Result < GetGatewayServiceUriResult.Step1_LocalNetworkIsAvailable)
                {
                    result = TenantConnectivityStatus.LocalNetworkUnavailable;
                }
                else if (getGatewayServiceUriResponse.Result < GetGatewayServiceUriResult.Step2_TcpConnectionTested)
                {
                    // we failed to connect to the site in order to retrieve the gateway address
                    // the cloud may be down, or internet connectivity may be broken, figure
                    // out which is most likely
                    if (getGatewayServiceUriResponse.Result == GetGatewayServiceUriResult.Step1i_MicrosoftNCSISuccess)
                    {
                        // test of Microsoft NCSI is working, so cloud is probably down
                        result = TenantConnectivityStatus.CloudUnavailable;
                    }
                    else
                    {
                        result = TenantConnectivityStatus.InternetConnectionUnavailable;
                    }
                }
                else if (getGatewayServiceUriResponse.Result == GetGatewayServiceUriResult.Step13_GatewayUriCreated)
                {
                    // we successfully retrieved the gateway address, but failed to advance past
                    // the basic TCP conneciton test, cloud must be down
                    result = TenantConnectivityStatus.GatewayServiceUnavailable;
                }
                else
                {
                    result = TenantConnectivityStatus.CommunicationFailure;
                }
            }

            return result;
        }
    }
}
