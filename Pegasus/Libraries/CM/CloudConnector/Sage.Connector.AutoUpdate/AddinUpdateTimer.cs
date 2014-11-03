using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sage.Connector.AutoUpdate.Addin;
using Sage.Connector.Common;
using Sage.Connector.Logging;

namespace Sage.Connector.AutoUpdate
{
    /// <summary>
    /// 
    /// </summary>
    public class AddinUpdateTimer : IDisposable
    {
        private readonly object _lock = new object();
        private Timer _timer;
        private DateTime _lastUpdateCheck;
        private readonly Action _triggerdAction;
        private CancellationTokenSource _cancel = new CancellationTokenSource();
        private IAutoUpdateTimerIntervalProvider _intervalProvider;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="AddinUpdateTimer" /> class.
        /// </summary>
        /// <param name="action">The function.</param>
        /// <param name="intervalProvider">The interval provider.</param>
        /// <exception cref="System.ArgumentNullException">function</exception>
        public AddinUpdateTimer(Action action, IAutoUpdateTimerIntervalProvider intervalProvider)
        {
            if(action == null) throw new ArgumentNullException("function");
            if (intervalProvider == null) throw new ArgumentNullException("intervalProvider");

            //set last check to min val, so when first timer check hits after start interval
            //we will actually do a check.
            _lastUpdateCheck = DateTime.MinValue;
            _intervalProvider = intervalProvider;

            _timer = new Timer(Callback, this, _intervalProvider.StartInterval, Timeout.InfiniteTimeSpan);
            _triggerdAction = action;
            
        }
        
        /// <summary>
        /// Destructor
        /// </summary>
        ~AddinUpdateTimer()
        {
            Dispose(false);
        }

        /// <summary>
        /// Sets the last update.
        /// </summary>
        public void SetLastUpdate()
        {
            _lastUpdateCheck = DateTime.Now;
        }

        /// <summary>
        /// Determines if the update operation has been canceled.
        /// </summary>
        /// <returns>True if the update has been canceled or the instance has been disposed of.</returns>
        private bool IsCancelled()
        {
            return ((_cancel == null) || _cancel.IsCancellationRequested);
        }

        private void Callback(Object state)
        {
            if (IsCancelled()) return;
            
            //update the intervals
            _intervalProvider.Refresh();

            var nextTime = _intervalProvider.HeartbeatInterval;

            try
            {

                if (!_intervalProvider.IsAutoUpdateQuerySuspended)
                {
                    if (!Monitor.TryEnter(_lock)) return;

                    try
                    {
                        if ((DateTime.Now - _lastUpdateCheck) < _intervalProvider.AutoUpdateQueryInterval) return;

                        try
                        {
                            _triggerdAction();

                            _lastUpdateCheck = DateTime.Now;
                            
                        }
                        catch (Exception)
                        {
                            nextTime = Timeout.InfiniteTimeSpan;
                            throw;
                        }
                    }
                    finally
                    {
                        Monitor.Exit(_lock);
                    }
                }
            }
            catch (Exception ex)
            {
                using (var lm = new LogManager())
                {
                    lm.WriteError(this, ex.ExceptionAsString());
                }
            }
            finally
            {
                if (!IsCancelled())
                {
                    _timer.Change(nextTime, Timeout.InfiniteTimeSpan);
                }
            }
        }

        /// <summary>
        /// Dispose of managed and unmanaged resources.
        /// </summary>
        /// <param name="disposing">True if resources should be cleaned up.</param>
        private void Dispose(bool disposing)
        {
            if (!disposing || _disposed) return;

            if (_cancel != null)
            {
                _cancel.Cancel();
            }

            lock (_lock)
            {
                if (_timer != null)
                {
                    _timer.Dispose();
                    _timer = null;
                }

                if (_cancel != null)
                {
                    _cancel.Dispose();
                    _cancel = null;
                }

                _disposed = true;
            }
        }
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
