using System;
using Sage.Connector.ConnectorServiceCommon;
using System.Threading;
using Sage.Connector.Cloud.Integration.Interfaces.Requests;

namespace Sage.Connector.DispatchService.Internal
{
    /// <summary>
    /// Execution type that the binder element is being run as.
    /// </summary>
    public enum ExecutionTypeValue
    {
        /// <summary>
        /// Binder element is not being processed.
        /// </summary>
        None,

        /// <summary>
        /// Binder element is running under a thread count limited by NumberOfAllowableConcurrentExecutions for the tenant.
        /// </summary>
        Normal,

        /// <summary>
        /// Binder element is running under a reserved thread count limited by NumberOfAllowableReservedExecutions for the tenant.
        /// </summary>
        HighPriority
    }

    /// <summary>
    /// The element of the tenant binder queue
    /// </summary>
    internal sealed class BinderQueueElement
    {
        private ExecutionTypeValue _executionType;
        private readonly uint _priority;
        private long _maxBlobSize;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="dispatchIdentifier"></param>
        /// <param name="bindable"></param>
        /// <param name="requestWrapper"></param>
        public BinderQueueElement(string identifier, string dispatchIdentifier, IBindable bindable, RequestWrapper requestWrapper)
        {
            Identifier = identifier;
            DispatchIdentifier = dispatchIdentifier;
            Bindable = bindable;
            RequestWrap = requestWrapper;

            _executionType = ExecutionTypeValue.None;
            _priority = 100;

            if (requestWrapper != null)
            {
                var request = Utils.JsonDeserialize<Request>(requestWrapper.RequestPayload);
                _priority = (request != null) ? request.Priority : 100;
            }
        }

        /// <summary>
        /// Sets the internal execution state to one of either normal or high priority. This is needed to handle the two different
        /// count limiters used when determining what can run next.
        /// </summary>
        /// <param name="highPriority"></param>
        public void SetExecution(bool highPriority = false)
        {
            _executionType = (highPriority ? ExecutionTypeValue.HighPriority : ExecutionTypeValue.Normal);
        }

        /// <summary>
        /// Returns the execution type that this element is running under.
        /// </summary>
        public ExecutionTypeValue ExecutionType
        {
            get
            {
                return _executionType;
            }
        }

        /// <summary>
        /// A system-generated identifier for the element to allow element operations
        /// </summary>
        public String Identifier { get; private set; }

        /// <summary>
        /// The associated dispatch identifier for element cleanup
        /// </summary>
        public String DispatchIdentifier { get; private set; }

        /// <summary>
        /// The bindable interface to operate on the request
        /// </summary>
        public IBindable Bindable { get; private set; }

        /// <summary>
        /// The request message wrapper
        /// </summary>
        public RequestWrapper RequestWrap { get; private set; }

        /// <summary>
        /// The cancellation signal for this element
        /// </summary>
        public CancellationTokenSource CancelTokenSource { get; set; }

        /// <summary>
        /// The maximum blob size for the request.
        /// </summary>
        public long MaxBlobSize
        {
            get
            {
                return _maxBlobSize;
            }
            set
            {
                _maxBlobSize = value;
            }
        }

        /// <summary>
        /// Return the priority of the request.
        /// </summary>
        public uint Priority
        {
            get
            {
                return _priority;
            }
        }
    }
}
