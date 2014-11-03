using System;

namespace Sage.Connector.Signalr.Common
{
    /// <summary>
    /// Common constants and defines for the SignalR common connector controller.
    /// </summary>
    public sealed class ControllerCommon
    {
        /// <summary>
        /// Name used to connect to the connector hub.
        /// </summary>
        public const string HubName = "ConnectedServices";

        /// <summary>
        /// The url format for the SignalR endpoint.
        /// </summary>
        public const string SignalREndpoint = "{0}/SignalR";
    }

    /// <summary>
    /// Common constants and defines for the SignalR notification system.
    /// </summary>
    public sealed class NotificationCommon
    {
        /// <summary>
        /// Max number of times to resend a notification to a client.
        /// </summary>
        public static int MaxRetries = 3;
        
        /// <summary>
        /// Amount of time to wait before retrying a notification to a client.
        /// </summary>
        public static TimeSpan RetryTime = TimeSpan.FromSeconds(10);

        /// <summary>
        /// Frequency (in ms) to check for and dispatch notifications.
        /// </summary>
        public static int DispatchFrequency = 200;
    }
}
