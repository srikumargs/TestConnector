using Sage.Connector.ProcessExecution.Interfaces;

namespace Sage.Connector.ProcessExecution.AddinSideAdapter
{
    /// <summary>
    /// Resposne Event Handler View to Contract AddIn Adapter
    /// </summary>
    public class ResponseEventHandlerViewToContractAddInAdapter : System.AddIn.Pipeline.ContractBase, IResponseEventHandler
    {

        private readonly object _view;

        private readonly System.Reflection.MethodInfo _event;
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="view">The response event handler view object</param>
        /// <param name="eventProp"><see cref="System.Reflection.MethodInfo"/></param>
        public ResponseEventHandlerViewToContractAddInAdapter(object view, System.Reflection.MethodInfo eventProp)
        {
            _view = view;
            _event = eventProp;
        }

        /// <summary>
        /// The Response Event Handler
        /// </summary>
        /// <param name="args"><see cref="IResponseEventArgs"/></param>
        /// <returns>true if the handler canceled the event</returns>
        public bool Handler(IResponseEventArgs args)
        {
            ResponseEventArgsContractToViewAddInAdapter adaptedArgs = new ResponseEventArgsContractToViewAddInAdapter(args);
            object[] argsArray = new object[1];
            argsArray[0] = adaptedArgs;
            _event.Invoke(_view, argsArray);
            return adaptedArgs.Cancel;
        }

        /// <summary>
        /// Response Event Handler Source view
        /// </summary>
        /// <returns>The Response Event Handler source view</returns>
        internal object GetSourceView()
        {
            return _view;
        }
    }
}
