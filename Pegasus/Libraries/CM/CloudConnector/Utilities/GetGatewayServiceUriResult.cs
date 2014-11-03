using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sage.Connector.Utilities
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Note we have a particular sensitivity to the specific enum values in this enumeration callers may want
    /// to use comparison operations (e.g., ">") to figure out aggregate state across all tenant connections
    /// </remarks>
    public enum GetGatewayServiceUriResult
    {
        /// <summary>
        /// No GetGatewayServiceEndpointProgressState (default value automatically initialized by runtime)
        /// </summary>
        None = 0,

        /// <summary>
        /// Step 1: a local network interface is available
        /// </summary>
        Step1_LocalNetworkIsAvailable,

        /// <summary>
        /// Step 1.a: (Microsoft NCSI) a basic low-level tcp connection test was completed successfully
        /// </summary>
        Step1a_MicrosoftNCSITcpConnectionTested,

        /// <summary>
        /// Step 1.b: (Microsoft NCSI) a DNS lookup was completed
        /// </summary>
        Step1b_MicrosoftNCSIDnsGetHostEntry,

        /// <summary>
        /// Step 1.c: (Microsoft NCSI) successfully compared DNS response to expected values
        /// </summary>
        Step1c_MicrosoftNCSIDnsGetHostEntryValueCompare,

        /// <summary>
        /// Step 1.d: (Microsoft NCSI) the HTTP GET request for the ncsi.txt file was created
        /// </summary>
        Step1d_MicrosoftNCSIWebRequestCreated,

        /// <summary>
        /// Step 1.e: (Microsoft NCSI) a response from the HTTP GET request was received
        /// </summary>
        Step1e_MicrosoftNCSIResponseReceived,

        /// <summary>
        /// Step 1.f: (Microsoft NCSI) a response stream from the HTTP GET request was received
        /// </summary>
        Step1f_MicrosoftNCSIResponseStreamReceived,

        /// <summary>
        /// Step 1.g: (Microsoft NCSI) a stream reader for the returned HTTP GET request response stream was created
        /// </summary>
        Step1g_MicrosoftNCSIStreamReaderCreated,

        /// <summary>
        /// Step 1.h: (Microsoft NCSI) the response from the HTTP GET request was read to the end
        /// </summary>
        Step1h_MicrosoftNCSIResponseReadToEnd,

        /// <summary>
        /// Step 1.i: (Microsoft NCSI) Successfully compared response stream contents to expected value
        /// </summary>
        Step1i_MicrosoftNCSISuccess,

        /// <summary>
        /// Step 2: a basic low-level tcp connection test was completed successfully
        /// </summary>
        Step2_TcpConnectionTested,

        /// <summary>
        /// Step 3: the HTTP GET request for the ncsi.txt file was created
        /// </summary>
        Step3_WebRequestCreated,

        /// <summary>
        /// Step 4: a response from the HTTP GET request was received
        /// </summary>
        Step4_ResponseReceived,

        /// <summary>
        /// Step 5: a response stream from the HTTP GET request was received
        /// </summary>
        Step5_ResponseStreamReceived,

        /// <summary>
        /// Step 6: a stream reader for the returned HTTP GET request response stream was created
        /// </summary>
        Step6_StreamReaderCreated,

        /// <summary>
        /// Step 7: the response from the HTTP GET request was read to the end
        /// </summary>
        Step7_ResponseReadToEnd,

        /// <summary>
        /// Step 8: the response from the HTTP GET request was non-empty
        /// </summary>
        Step8_ResponseNonEmpty,

        /// <summary>
        /// Step 9: the response Name name/value pair was parsed
        /// </summary>
        Step9_ResponseNameValueParsed,

        /// <summary>
        /// Step 10: the response Name name/value was compared and matched the expected value
        /// </summary>
        Step10_ResponseNameValueCompared,

        /// <summary>
        /// Step 11: the response ConnectorServiceGateway name/value pair was parsed
        /// </summary>
        Step11_ResponseGatewayValueParsed,

        /// <summary>
        /// Step 12: the response ConnectorServiceGateway value was non-empty
        /// </summary>
        Step12_ResponseGatewayValueNonEmpty,

        /// <summary>
        /// Ste 13: the connector gateway service uri was created
        /// </summary>
        Step13_GatewayUriCreated,

        /// <summary>
        /// A basic low-level tcp connection test was completed successfully against the gateway service uri
        /// </summary>
        Success
    }
}
