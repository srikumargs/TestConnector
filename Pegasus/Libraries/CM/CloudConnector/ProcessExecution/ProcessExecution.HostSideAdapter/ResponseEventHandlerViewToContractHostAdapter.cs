using System.AddIn.Pipeline;
using Sage.Connector.ProcessExecution.Interfaces;

namespace Sage.Connector.ProcessExecution.HostSideAdapter
{
    /// <summary>
    /// Response Event Handler View To Contract Host Adapter
    /// </summary>
    public class ResponseEventHandlerViewToContractHostAdapter : ContractBase, IResponseEventHandler
    {

        private readonly object _view;

        private readonly System.Reflection.MethodInfo _event;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="view">The view object</param>
        /// <param name="eventProp"><see cref="System.Reflection.MethodInfo"/></param>
        public ResponseEventHandlerViewToContractHostAdapter(object view, System.Reflection.MethodInfo eventProp)
        {
            _view = view;
            _event = eventProp;
        }

        /// <summary>
        /// Reponse Event Handler
        /// </summary>
        /// <param name="args"><see cref="IResponseEventArgs"/></param>
        /// <returns>true if the event is canceled.</returns>
        public bool Handler(IResponseEventArgs args)
        {
            ResponseEventArgsContractToViewHostAdapter adaptedArgs = new ResponseEventArgsContractToViewHostAdapter(args);
            object[] argsArray = new object[1];
            argsArray[0] = adaptedArgs;
            _event.Invoke(_view, argsArray);
            return adaptedArgs.Cancel;
        }

        /// <summary>
        /// Get the Response Event Args source view
        /// </summary>
        /// <returns>The Response Event Args source view object</returns>
        internal object GetSourceView()
        {
            return _view;
        }
    }
}
