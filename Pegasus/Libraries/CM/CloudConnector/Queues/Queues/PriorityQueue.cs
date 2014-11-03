using System;
using System.Collections.Generic;

namespace Sage.Connector.Queues
{
    /// <summary>
    /// Wrapper class for objects that need to be prioritized.
    /// </summary>
    /// <typeparam name="T">The type of value that will be managed.</typeparam>
    public sealed class PriorityValue<T> : IComparable<PriorityValue<T>>
    {
        #region Private declarations

        private readonly T _value;
        private readonly int _priority;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="value">The type value to maintain.</param>
        /// <param name="priority">The priority of the object instance.</param>
        public PriorityValue(T value, int priority)
        {
            _value = value;
            _priority = priority;
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
        public int CompareTo(PriorityValue<T> other)
        {
            return (Priority - other.Priority);
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
    public sealed class PriorityQueue<T> : IEnumerable<PriorityValue<T>>
    {
        #region Private Declarations

        private readonly List<PriorityValue<T>> _data;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        public PriorityQueue()
        {
            _data = new List<PriorityValue<T>>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="enumerable">The enumerable source containing PriorityValue items to add to the queue.</param>
        public PriorityQueue(IEnumerable<PriorityValue<T>> enumerable)
        {
            _data = new List<PriorityValue<T>>();

            foreach (var item in enumerable)
            {
                Enqueue(item.Value, item.Priority);
            }
        }

        #endregion

        #region IEnumerable

        /// <summary>
        /// Returns the genetic enumerator object. Note that this is not guaranteed to be in priority order, as 
        /// only the Dequeue ensures ordering.
        /// </summary>
        /// <returns> Returns the genetic enumerator object.</returns>
        public IEnumerator<PriorityValue<T>> GetEnumerator()
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
        public void Enqueue(T item, int priority)
        {
            if (priority < 0) throw new ArgumentOutOfRangeException("priority");

            _data.Add(new PriorityValue<T>(item, priority));

            var count = _data.Count - 1;

            while (count > 0)
            {
                var parent = (count - 1) / 2;

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
            if (_data.Count == 0) throw new InvalidOperationException("The priority queue is empty.");

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