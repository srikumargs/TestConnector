using System;

namespace Sage.Connector.AutoUpdate
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAutoUpdateTimerIntervalProvider
    {
        /// <summary>
        /// Refreshes this instance.
        /// </summary>
        void Refresh();
        
        /// <summary>
        /// Interval to use on startup.
        /// </summary>
        TimeSpan StartInterval { get; }
    
        /// <summary>
        /// Interval to use to drive, periodic auto update query and settings change detection.
        /// </summary>
        TimeSpan HeartbeatInterval { get; }

        /// <summary>
        /// Interval in hours between queries to the update service.
        /// </summary>
        TimeSpan AutoUpdateQueryInterval { get; }


        /// <summary>
        /// Gets a value indicating whether this instance is query interval suspended.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is query interval suspended; otherwise, <c>false</c>.
        /// </value>
        bool IsAutoUpdateQuerySuspended { get; }

    }
}
