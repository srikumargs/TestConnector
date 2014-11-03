
namespace Sage.Connector.Utilities.Internal
{
    /// <summary>
    /// Result for TestMicrosoftNCSI()
    /// </summary>
    internal enum MicrosoftNCSITestResult
    {
        /// <summary>
        /// No TestMicrosoftNCSIResult (default value automatically initialized by runtime)
        /// </summary>
        None = 0,

        /// <summary>
        /// Step 1: a basic low-level tcp connection test was completed
        /// </summary>
        Step1_TcpConnectionTested,

        /// <summary>
        /// Step 2: a DNS lookup was completed
        /// </summary>
        Step2_DnsGetHostEntry,

        /// <summary>
        /// Step 3: successfully compared DNS response to expected values
        /// </summary>
        Step3_DnsGetHostEntryValueCompare,

        /// <summary>
        /// Step 4: the HTTP GET request for the ncsi.txt file was created
        /// </summary>
        Step4_WebRequestCreated,

        /// <summary>
        /// Step 5: a response from the HTTP GET request was received
        /// </summary>
        Step5_ResponseReceived,

        /// <summary>
        /// Step 6: a response stream from the HTTP GET request was received
        /// </summary>
        Step6_ResponseStreamReceived,

        /// <summary>
        /// Step 7: a stream reader for the returned HTTP GET request response stream was created
        /// </summary>
        Step7_StreamReaderCreated,

        /// <summary>
        /// Step 8: the response from the HTTP GET request was read to the end
        /// </summary>
        Step8_ResponseReadToEnd,

        /// <summary>
        /// Successfully compared response stream contents to expected value
        /// </summary>
        Success
    }
}
