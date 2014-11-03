using System;
using System.Collections.Generic;
using System.Threading;

namespace Sage.Connector.Utilities.Platform.DotNet
{
    /// <summary>
    /// Class to handle ordered processing of work items on the thread pool.
    /// </summary>
    public class ThreadTaskQueue
    {
        /// <summary>
        /// Internal state
        /// </summary>
        private class QTask
        {
            public WaitCallback Callback { get; set; }
            public Object State { get; set; }
        }

        private readonly Object _lock = new Object();
        private readonly Queue<QTask> _tasks = new Queue<QTask>();
        private volatile int _runningCount;        

        /// <summary>
        /// Queues a new work item request on the queue. 
        /// </summary>
        /// <param name="callback">The WaitCallback to process.</param>
        /// <param name="state">The state associated with the callback.</param>
        public void Queue(WaitCallback callback, Object state)
        {
            lock (_lock)
            {
                _tasks.Enqueue(new QTask { Callback = callback, State = state });
            }

            ProcessTaskQueue();
        }

        /// <summary>
        /// Clears all work from the task queue.
        /// </summary>
        public void Clear()
        {
            lock (_lock)
            {
                _tasks.Clear();
            }
        }

        /// <summary>
        /// Returns the count of queued work items.
        /// </summary>
        public int Count
        {
            get
            {
                lock (_lock)
                {
                    return _tasks.Count;
                }
            }
        }

        /// <summary>
        /// Processes the next queued work item when the running count hits zero.
        /// </summary>
        private void ProcessTaskQueue()
        {
            lock (_lock)
            {
                if (_tasks.Count == 0) return;

                if (_runningCount == 0)
                {
                    QTask schedule = _tasks.Dequeue();

                    Action completionTask = () =>
                    {
                        try
                        {
                            schedule.Callback(schedule.State);
                        }
                        finally
                        {
                            OnTaskCompleted();
                        }
                    };

                    _runningCount++;

                    ThreadPool.QueueUserWorkItem(_ => completionTask());
                }
            }
        }

        /// <summary>
        /// Called on work item completion to allow for running of the next queued task.
        /// </summary>
        private void OnTaskCompleted()
        {
            --_runningCount;
                    
            ProcessTaskQueue();
        }
    }
}
