using System;
using System.Diagnostics;
using System.Reflection;

namespace Sage.Connector.MonitorService.Internal
{
    internal sealed class StateCache
    {
        public void Start()
        {
            _startTimeUtc = DateTime.UtcNow;
        }

        public void Stop()
        {
            _startTimeUtc = null;
        }

        public String Version
        { get { return _version; } }

        public TimeSpan Uptime
        {
            get
            {
                TimeSpan result = new TimeSpan(0);

                if (_startTimeUtc.HasValue)
                {
                    result = DateTime.UtcNow.Subtract(_startTimeUtc.Value);
                }

                return result;
            }
        }

        private String _version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
        private DateTime? _startTimeUtc;
    }
}
