using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sage.Connector.Queues
{
    /// <summary>
    /// Wrapper class for objects that need to be prioritized.
    /// </summary>
    /// <typeparam name="T">The type of value that will be managed.</typeparam>
    public sealed class PriorityTimeStampValue<T> : IComparable<PriorityTimeStampValue<T>>
    {
        #region Private declarations

        private readonly T _value;
        private readonly int _priority;
        private readonly DateTime? _timeStamp;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">The type value to maintain.</param>
        /// <param name="priority">The priority of the object instance.</param>
        /// <param name="timeStamp">The date time.</param>
        public PriorityTimeStampValue(T value, int priority, DateTime? timeStamp)
        {
            _value = value;
            _priority = priority;
            _timeStamp = timeStamp;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Overridden ToString method.
        /// </summary>
        /// <returns>The class name along with the assigned priority.</returns>
        public override string ToString()
        {
            return String.Format("{0}({1})", base.ToString(), _priority);
        }

        /// <summary>
        /// Compare implementation.
        /// </summary>
        /// <param name="other">The PriorityValue to compare values with.</param>
        /// <returns>The difference between priorities.</returns>
        public int CompareTo(PriorityTimeStampValue<T> other)
        {
            int retval = (Priority - other.Priority);
            if (retval == 0)
            {
                //note that objects with no time stamp come after objects with a time stamp for this implementation
                if (TimeStamp == null && other.TimeStamp == null)
                {
                    retval = 0;
                }
                else if (TimeStamp != null && other.TimeStamp == null)
                {
                    //No time stamp on other object so put it after the current.
                    retval = -1;
                }
                else if (TimeStamp == null && other.TimeStamp != null)
                {
                    //No time stamp on current object so put it after the other.
                    retval = 1;
                }
                else
                {
                    //only option left both TimeStamp and other.TimeStamp are not null
                    DateTime timeStampA = _timeStamp.Value;
                    DateTime timeStampB = other.TimeStamp.Value;
                    retval = timeStampA.CompareTo(timeStampB);
                }
            }
            
            return retval;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns the priority.
        /// </summary>
        public int Priority
        {
            get
            {
                return _priority;
            }
        }

        /// <summary>
        /// Returns the date.
        /// </summary>
        public DateTime? TimeStamp
        {
            get
            {
                return _timeStamp;
            }
        }


        /// <summary>
        /// Returns the type value.
        /// </summary>
        public T Value
        {
            get
            {
                return _value;
            }
        }

        #endregion
    }


    /// <summary>
    /// Class definition for a generic priority queue. A list is used for storing the values with a binary sort 
    /// implementation, making this O(log n) in usage.
    /// </summary>
    /// <typeparam name="T">The type of values that will be managed.</typeparam>
    public sealed class PriorityTimeStampQueue<T> : IEnumerable<PriorityTimeStampValue<T>>
    {
        #region Private Declarations

        private readonly List<PriorityTimeStampValue<T>> _data;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public PriorityTimeStampQueue()
        {
            _data = new List<PriorityTimeStampValue<T>>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="enumerable">The enumerable source containing PriorityValue items to add to the queue.</param>
        public PriorityTimeStampQueue(IEnumerable<PriorityTimeStampValue<T>> enumerable)
        {
            _data = new List<PriorityTimeStampValue<T>>();

            foreach (var item in enumerable)
            {
                Enqueue(item.Value, item.Priority, item.TimeStamp);
            }
        }

        #endregion

        #region IEnumerable

        /// <summary>
        /// Returns the genetic enumerator object. Note that this is not guaranteed to be in priority order, as 
        /// only the Dequeue ensures ordering.
        /// </summary>
        /// <returns> Returns the genetic enumerator object.</returns>
        public IEnumerator<PriorityTimeStampValue<T>> GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        /// <summary>
        /// Returns the enumerator object.
        /// </summary>
        /// <returns> Returns the enumerator object.</returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds the item and its priority to the list of queued items.
        /// </summary>
        /// <param name="item">The item to add to the queue.</param>
        /// <param name="priority">The priority value of the item.</param>
        /// <param name="timeStamp">The time stamp.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">priority</exception>
        public void Enqueue(T item, int priority, DateTime? timeStamp)
        {
            if (priority < 0) throw new ArgumentOutOfRangeException("priority");

            _data.Add(new PriorityTimeStampValue<T>(item, priority, timeStamp));

            int count = _data.Count - 1;

            while (count > 0)
            {
                int parent = (count - 1) / 2;

                if (_data[count].CompareTo(_data[parent]) >= 0) break;

                var tmp = _data[count];
                _data[count] = _data[parent];
                _data[parent] = tmp;

                count = parent;
            }
        }

        /// <summary>
        /// Removes and returns the item with the highest priority (lowest value) from the queue. 
        /// If the queue is empty then the default value for the type will be returned. This will
        /// be null for reference types.
        /// </summary>
        /// <returns>The item with the highest priority (lowest value).</returns>
        public T Dequeue()
        {
            if (_data.Count == 0) throw new InvalidOperationException("The priority time stamp queue is empty.");

            var first = _data[0];

            var last = _data.Count - 1;
            var parent = 0;

            _data[0] = _data[last];

            _data.RemoveAt(last--);

            while (true)
            {
                var child = parent * 2 + 1;
                if (child > last) break;

                var right = child + 1;
                if (right <= last && _data[right].CompareTo(_data[child]) < 0) child = right;
                if (_data[parent].CompareTo(_data[child]) <= 0) break;

                var tmp = _data[parent];
                _data[parent] = _data[child];
                _data[child] = tmp;

                parent = child;
            }

            return first.Value;
        }

        /// <summary>
        /// Returns the item priority value with the highest priority (lowest value) from the queue.
        /// If the queue is empty then (-1) will be returned.
        /// </summary>
        /// <returns>The priority value of the next item on the queue.</returns>
        public int PeekPriority()
        {
            return (_data.Count == 0) ? (-1) : _data[0].Priority;
        }

        /// <summary>
        /// Returns the item with the highest priority (lowest value) from the queue.
        /// If the queue is empty then the default value for the type will be returned.
        /// This will be null for reference types.
        /// </summary>
        /// <returns></returns>
        public T Peek()
        {
            return (_data.Count == 0) ? default(T) : _data[0].Value;
        }

        /// <summary>
        /// Clears all elements from the queue.
        /// </summary>
        public void Clear()
        {
            _data.Clear();
        }

        /// <summary>
        /// Verifies that the queue is in a consistent (ordered) state.
        /// </summary>
        /// <returns>True if the queue is in a consistent state.</returns>
        public bool IsConsistent()
        {
            if (_data.Count == 0) return true;

            var last = _data.Count - 1;

            for (var parent = 0; parent < _data.Count; ++parent)
            {
                var leftchild = 2 * parent + 1;
                var rightchild = 2 * parent + 2;

                if (leftchild <= last && _data[parent].CompareTo(_data[leftchild]) > 0) return false;
                if (rightchild <= last && _data[parent].CompareTo(_data[rightchild]) > 0) return false;
            }

            return true;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns the count of items in the queue.
        /// </summary>
        /// <returns>The count of items in the queue.</returns>
        public int Count
        {
            get
            {
                return _data.Count;
            }
        }

        #endregion
    }
}
