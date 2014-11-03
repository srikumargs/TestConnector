using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sage.Connector.AutoUpdate.Addin;

namespace Sage.Connector.AutoUpdate
{
    /// <summary>
    /// 
    /// </summary>
    public class AutoUpdateTimerIntervalProvider : IAutoUpdateTimerIntervalProvider
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="AutoUpdateTimerIntervalProvider"/> class from being created.
        /// </summary>
        public AutoUpdateTimerIntervalProvider ()
        {
            //do we need to pass in config provider?
            //do not really want this to know about the config file explicitly..
            Refresh();
        }

        /// <summary>
        /// Refreshes this instance.
        /// </summary>
        public void Refresh()
        {
            StartInterval = GetInteval(IntervalConfigKeys.StartInterval, AddinTimes.StartInterval);
            HeartbeatInterval = GetInteval(IntervalConfigKeys.HeartbeatInterval, AddinTimes.HeartbeatInterval);
            AutoUpdateQueryInterval = GetInteval(IntervalConfigKeys.AutoUpdateQueryInterval, AddinTimes.QueryInterval);
            IsAutoUpdateQuerySuspended = GetIsSuspended(false);
        }


        private bool GetIsSuspended(bool fallback)
        {
            bool retval = fallback;
            string setting = ConfigurationProvider.GetIntervalSetting(IntervalConfigKeys.SuspendPeriodicUpdate);
            if (!string.IsNullOrWhiteSpace(setting))
            {
                //be more liberal in accepting "True", and "true " etc not just "True" as TryParse requires.
                setting = setting.Trim();
                if (String.Compare(setting, "true", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    retval = true;
                }
            }

            return retval;
        }

        private TimeSpan GetInteval(string interval, TimeSpan fallback)
        {
            TimeSpan retval = fallback;
            string setting = ConfigurationProvider.GetIntervalSetting(interval);
            if (!string.IsNullOrWhiteSpace(setting))
            {
                var parts = setting.Split(new char[]{','}, StringSplitOptions.None);
                
                int numberOfSegmentsForATimeSpan = 5;
                if (parts.Length == numberOfSegmentsForATimeSpan)
                {
                    bool segmentFailed = false;
                    //chop the inbound string into an int array, anything that does not parse is 0
                    int[] partValues = new int[parts.Length];
                    for (int i = parts.Length -1 ; i >= 0; i--)
                    {
                        string part = parts[i];
                        int value;
                        
                        // if we succeed we get a value, if not we get 0. 
                        bool conversionWorked = int.TryParse(part, out value);
                        if (!conversionWorked)
                        {
                            segmentFailed = true;
                            break;
                        }
                        partValues[i] = value;
                    }

                    if (!segmentFailed)
                    {
                        try
                        {
                            TimeSpan span = new TimeSpan(partValues[0], partValues[1], partValues[2], partValues[3], partValues[4]);
                            retval = span;
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            //if we did not get all the parts or any are out of range then just use the fall back and eat the exception.   
                        }
                    }
                }
            }
            return retval;
        }

        /// <summary>
        /// Interval to use on startup.
        /// </summary>
        public TimeSpan StartInterval { get; private set; }
        /// <summary>
        /// Interval to to drive auto update queries and configuration checks.
        /// </summary>
        public TimeSpan HeartbeatInterval { get; private set; }
        /// <summary>
        /// Interval in hours between queries to the update service.
        /// </summary>
        public TimeSpan AutoUpdateQueryInterval { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this instance is query interval suspended.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is query interval suspended; otherwise, <c>false</c>.
        /// </value>
        public bool IsAutoUpdateQuerySuspended { get; private set; }
    }
}
