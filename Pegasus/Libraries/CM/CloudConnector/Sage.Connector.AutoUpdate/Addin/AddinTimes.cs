using System;

namespace Sage.Connector.AutoUpdate.Addin
{
    /// <summary>
    /// Constant class for addin time intervals.
    /// </summary>
    internal static class AddinTimes
    {
        /// <summary>
        /// Interval to use on startup.
        /// </summary>
        public static readonly TimeSpan StartInterval = new TimeSpan(0, 1, 0);
        //start used to be 30sec but now 1min, and configured to not auto force check at that time.

        /// <summary>
        /// Interval to use when we have activity; eg downloads, staging transfer, etc.
        /// </summary>
        public static readonly TimeSpan ActiveInterval = new TimeSpan(0,3,0);

        /// <summary>
        /// Interval to use for waking up and checking for actions or changes.
        /// </summary>
        public static readonly TimeSpan HeartbeatInterval = new TimeSpan(0, 10, 0);

        /// <summary>
        /// Interval in hours between queries to the update service.
        /// </summary>
        public static readonly TimeSpan QueryInterval = new TimeSpan(6, 0, 0);

        /// <summary>
        /// Interval to use for download timeout.
        /// </summary>
        public const int DownloadActivityTimeout = 30000;

        /// <summary>
        /// Interval to use for update check timeout.
        /// </summary>
        public const int CheckTimeout = 10000;

        /// <summary>
        /// Interval to use for connection timeout.
        /// </summary>
        public const int ConnectTimeout = 10000;
    }
}