using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sage.Connector.ConnectorServiceCommon
{
    /// <summary> 
    /// Provides a task scheduler that ensures a maximum concurrency level while  running on top of
    /// the ThreadPool. 
    /// </summary> 
    public class LimitedConcurrencyLevelTaskScheduler : TaskScheduler
    {
        #region Private Members

        [ThreadStatic]
        private static bool _currentThreadIsProcessingItems;
        private readonly LinkedList<Task> _tasks = new LinkedList<Task>(); 
        private readonly int _maxDegreeOfParallelism;
        private int _delegatesQueuedOrRunning;

        #endregion

        /// <summary> 
        /// Initializes an instance of the LimitedConcurrencyLevelTaskScheduler class with the 
        /// specified degree of parallelism. 
        /// </summary> 
        /// <param name="maxDegreeOfParallelism">The maximum degree of parallelism provided by this scheduler.</param> 
        public LimitedConcurrencyLevelTaskScheduler(int maxDegreeOfParallelism)
        {
            if (maxDegreeOfParallelism < 1) throw new ArgumentOutOfRangeException("maxDegreeOfParallelism");

            _maxDegreeOfParallelism = maxDegreeOfParallelism;
        }

        /// <summary>Queues a task to the scheduler.</summary> 
        /// <param name="task">The task to be queued.</param> 
        protected sealed override void QueueTask(Task task)
        {
            lock (_tasks)
            {
                _tasks.AddLast(task);

                if (_delegatesQueuedOrRunning < _maxDegreeOfParallelism)
                {
                    ++_delegatesQueuedOrRunning;
                    NotifyThreadPoolOfPendingWork();
                }
            }
        }

        /// <summary> 
        /// Informs the ThreadPool that there's work to be executed for this scheduler. 
        /// </summary> 
        private void NotifyThreadPoolOfPendingWork()
        {
            ThreadPool.UnsafeQueueUserWorkItem(_ =>
            {
                _currentThreadIsProcessingItems = true;
                try
                {
                    while (true)
                    {
                        Task item;

                        lock (_tasks)
                        {
                            if (_tasks.Count == 0)
                            {
                                --_delegatesQueuedOrRunning;
                                break;
                            }

                            item = _tasks.First.Value;
                            _tasks.RemoveFirst();
                        }

                        TryExecuteTask(item);
                    }
                }
                finally
                {
                    _currentThreadIsProcessingItems = false;
                }
            }, null);
        }

        /// <summary>Attempts to execute the specified task on the current thread.</summary> 
        /// <param name="task">The task to be executed.</param> 
        /// <param name="taskWasPreviouslyQueued"></param> 
        /// <returns>Whether the task could be executed on the current thread.</returns> 
        protected sealed override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            if (!_currentThreadIsProcessingItems) return false;
            if (taskWasPreviouslyQueued) TryDequeue(task);

            return TryExecuteTask(task);
        }

        /// <summary>Attempts to remove a previously scheduled task from the scheduler.</summary> 
        /// <param name="task">The task to be removed.</param> 
        /// <returns>Whether the task could be found and removed.</returns> 
        protected sealed override bool TryDequeue(Task task)
        {
            lock (_tasks)
            {
                return _tasks.Remove(task);
            }
        }

        /// <summary>Gets the maximum concurrency level supported by this scheduler.</summary> 
        public sealed override int MaximumConcurrencyLevel
        {
            get
            {
                return _maxDegreeOfParallelism;
            }
        }

        /// <summary>Gets an enumerable of the tasks currently scheduled on this scheduler.</summary> 
        /// <returns>An enumerable of the tasks currently scheduled.</returns> 
        protected sealed override IEnumerable<Task> GetScheduledTasks()
        {
            bool lockTaken = false;

            try
            {
                Monitor.TryEnter(_tasks, ref lockTaken);
                if (lockTaken)
                {
                    return _tasks.ToArray();
                }
                throw new NotSupportedException();
            }
            finally
            {
                if (lockTaken) Monitor.Exit(_tasks);
            }
        }
    } 
    
    /// <summary>
    /// Helper class to enable typesafe invocation of the Task.Factory.StartNew
    /// </summary>
    public static class TaskFactoryHelper
    {
        private static readonly TaskFactory _limitedFactory = new TaskFactory(new LimitedConcurrencyLevelTaskScheduler(Environment.ProcessorCount * 2));

        /// <summary>
        /// Starts a new System.Threading.Tasks.Task using the limited concurrency task factory.
        /// </summary>
        /// <param name="action">The action to thread.</param>
        /// <returns>The new thread task.</returns>
        public static Task StartNewLimited(Action action)
        {
            return _limitedFactory.StartNew(action);
        }

        /// <summary>
        /// Creates and starts a System.Threading.Tasks.Task.
        /// </summary>
        /// <typeparam name="T">The type of the state object</typeparam>
        /// <param name="state">The state object to be passed to the thread</param>
        /// <param name="action">The thread function to be executed</param>
        /// <returns></returns>
        public static Task StartNew<T>(T state, Action<T> action)
        {
            return Task.Factory.StartNew(theState => action((T)theState), state);
        }

        /// <summary>
        /// Creates and starts a System.Threading.Tasks.Task.
        /// </summary>
        /// <typeparam name="T">The type of the state object</typeparam>
        /// <param name="factory">The task factory to schedule this task on.</param>
        /// <param name="state">The state object to be passed to the thread</param>
        /// <param name="action">The thread function to be executed</param>
        /// <returns></returns>
        public static Task StartNew<T>(TaskFactory factory, T state, Action<T> action)
        {
            return factory.StartNew(theState => action((T)theState), state);
            
        }
    }
}
