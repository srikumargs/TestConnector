using System;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using Sage.Connector.Common;
using Sage.Connector.ConnectorServiceCommon;
using Sage.Connector.Logging;
using Sage.Connector.Utilities.Internal;

namespace Sage.Connector.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class InternetConnectivityStatusHelper
    {
        static InternetConnectivityStatusHelper()
        {
            // cause the ServerCertificateValidationCallback to be setup
            CloudUtils.SetupServerCertificateValidationCallback();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteAddress"></param>
        /// <returns></returns>
        public static GetGatewayServiceUriResponse GetGatewayServiceUri(Uri siteAddress)
        {
            GetGatewayServiceUriResult result = GetGatewayServiceUriResult.None;
            Uri uri = null;
            using (var stc = new StackTraceContext(null))
            {
                try
                {
                    if (NetworkInterface.GetIsNetworkAvailable())
                    {
                        result = GetGatewayServiceUriResult.Step1_LocalNetworkIsAvailable;

                        // Inspired by
                        // Appendix K: Network Connectivity Status Indicator and Resulting Internet Communication in Windows Vista
                        // http://technet.microsoft.com/en-us/library/cc766017(v=ws.10).aspx

                        String activeWebProbeContent = "Sage Construction Anywhere NCSI";
                        String activeWebProbePath = "ncsi.txt";

                        String gatewayAddress = String.Empty;
                        if (!siteAddress.Scheme.StartsWith("mock-"))
                        {
                            if (!siteAddress.AbsoluteUri.EndsWith("/"))
                            {
                                // NOTE: new Uri(x,y) does not do a true combine _unless_ x ends with a "/".
                                // Make sure that we are are getting what we expect 
                                siteAddress = new Uri(siteAddress.AbsoluteUri + "/");
                            }

                            Uri ncsiUri = new Uri(siteAddress, activeWebProbePath);
                            if (ConnectivityStatusUtils.BasicTcpConnectivityTest(ncsiUri, ConnectorRegistryUtils.InternetConnectivityStatusTcpConnectTimeout))
                            {
                                result = GetGatewayServiceUriResult.Step2_TcpConnectionTested;

                                var request = WebRequest.Create(ncsiUri);
                                result = GetGatewayServiceUriResult.Step3_WebRequestCreated;

                                using (var response = request.GetResponse())
                                {
                                    result = GetGatewayServiceUriResult.Step4_ResponseReceived;

                                    Stream stream = null;
                                    try
                                    {
                                        stream = response.GetResponseStream();
                                        result = GetGatewayServiceUriResult.Step5_ResponseStreamReceived;

                                        using (var reader = new StreamReader(stream))
                                        {
                                            result = GetGatewayServiceUriResult.Step6_StreamReaderCreated;

                                            stream = null;
                                            String responseFromServer = reader.ReadToEnd();
                                            result = GetGatewayServiceUriResult.Step7_ResponseReadToEnd;

                                            if (!String.IsNullOrEmpty(responseFromServer))
                                            {
                                                result = GetGatewayServiceUriResult.Step8_ResponseNonEmpty;

                                                String[] splitResponseFromServer = responseFromServer.Split(';');
                                                if (splitResponseFromServer.Length > 1)
                                                {
                                                    String prefix = "Name=";
                                                    if (splitResponseFromServer[0].StartsWith(prefix))
                                                    {
                                                        result = GetGatewayServiceUriResult.Step9_ResponseNameValueParsed;

                                                        var value = splitResponseFromServer[0].Remove(0, prefix.Length).Trim('\'');
                                                        if (value == activeWebProbeContent)
                                                        {
                                                            result = GetGatewayServiceUriResult.Step10_ResponseNameValueCompared;

                                                            prefix = "ConnectorServiceGateway=";
                                                            if (splitResponseFromServer[1].StartsWith(prefix))
                                                            {
                                                                result = GetGatewayServiceUriResult.Step11_ResponseGatewayValueParsed;

                                                                gatewayAddress = splitResponseFromServer[1].Remove(0, prefix.Length).Trim('\'');
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    finally
                                    {
                                        if (stream != null)
                                        {
                                            stream.Dispose();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // basic connectivity to our cloud site just failed.  As an experiment, try out Microsoft's NCSI site to determine if this is 
                                // a problem larger than just connectivity to our cloud site.

                                var microsoftNCSIResult = ConnectivityStatusUtils.MicrosoftNCSITest();
                                result = MapToGetGatewayServiceUriResult(result, microsoftNCSIResult);
                            }
                        }
                        else
                        {
                            // Mock case:  emulate behavior of the cloud being down
                            String prefix = "mock-";
                            gatewayAddress = siteAddress.ToString().Remove(0, prefix.Length);
                        }

                        if (!String.IsNullOrEmpty(gatewayAddress))
                        {
                            result = GetGatewayServiceUriResult.Step12_ResponseGatewayValueNonEmpty;

                            if (Uri.TryCreate(gatewayAddress, UriKind.Absolute, out uri))
                            {
                                result = GetGatewayServiceUriResult.Step13_GatewayUriCreated;

                                if (ConnectivityStatusUtils.BasicTcpConnectivityTest(uri, ConnectorRegistryUtils.InternetConnectivityStatusTcpConnectTimeout))
                                {
                                    result = GetGatewayServiceUriResult.Success;
                                }
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

            return new GetGatewayServiceUriResponse(result, uri);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <param name="microsoftNCSIResult"></param>
        /// <returns></returns>
        private static GetGatewayServiceUriResult MapToGetGatewayServiceUriResult(GetGatewayServiceUriResult result, MicrosoftNCSITestResult microsoftNCSIResult)
        {
            switch (microsoftNCSIResult)
            {
                case MicrosoftNCSITestResult.None:
                    // nothing new to say, leave result unchanged
                    break;
                case MicrosoftNCSITestResult.Step1_TcpConnectionTested:
                    result = GetGatewayServiceUriResult.Step1a_MicrosoftNCSITcpConnectionTested;
                    break;
                case MicrosoftNCSITestResult.Step2_DnsGetHostEntry:
                    result = GetGatewayServiceUriResult.Step1b_MicrosoftNCSIDnsGetHostEntry;
                    break;
                case MicrosoftNCSITestResult.Step3_DnsGetHostEntryValueCompare:
                    result = GetGatewayServiceUriResult.Step1c_MicrosoftNCSIDnsGetHostEntryValueCompare;
                    break;
                case MicrosoftNCSITestResult.Step4_WebRequestCreated:
                    result = GetGatewayServiceUriResult.Step1d_MicrosoftNCSIWebRequestCreated;
                    break;
                case MicrosoftNCSITestResult.Step5_ResponseReceived:
                    result = GetGatewayServiceUriResult.Step1e_MicrosoftNCSIResponseReceived;
                    break;
                case MicrosoftNCSITestResult.Step6_ResponseStreamReceived:
                    result = GetGatewayServiceUriResult.Step1f_MicrosoftNCSIResponseStreamReceived;
                    break;
                case MicrosoftNCSITestResult.Step7_StreamReaderCreated:
                    result = GetGatewayServiceUriResult.Step1g_MicrosoftNCSIStreamReaderCreated;
                    break;
                case MicrosoftNCSITestResult.Step8_ResponseReadToEnd:
                    result = GetGatewayServiceUriResult.Step1h_MicrosoftNCSIResponseReadToEnd;
                    break;
                case MicrosoftNCSITestResult.Success:
                    result = GetGatewayServiceUriResult.Step1i_MicrosoftNCSISuccess;
                    break;
                default:
                    using (var lm = new LogManager())
                    {
                        lm.WriteError(null, "Unknown result: {0}", microsoftNCSIResult.ToString());
                    }
                    break;
            }

            return result;
        }
    }
}
