using System;
using System.Text;
using Sage.Connector.Common;
using Sage.Diagnostics;

namespace Sage.Connector.Logging
{
    /// <summary>
    /// 
    /// </summary>
    /// 
    [TraceListenerIgnoreType]
    public sealed class StackTraceContext : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="caller"></param>
        public StackTraceContext(Object caller) :
            this(caller, String.Empty)
        { }

        /// <summary>
        /// 
        /// 
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="message"></param>
        /// <param name="list"></param>
        public StackTraceContext(Object caller, String message, params Object[] list)
        {
            StringBuilder sb = new StringBuilder("invoked: ");

            if (list != null)
            {
                _message = String.Format(message, list);
            }
            else
            {
                _message = message;
            }

            sb.Append(_message);
            _caller = caller;

            using (var lm = new LogManager())
            {
                (lm as ILogging).VerboseTrace(_caller, sb.ToString());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        public void SetResult(Object result)
        {
            if (result != null)
            {
                _result = result.ToString();
            }

            _resultSet = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            StringBuilder sb = new StringBuilder("complete: ");
            sb.Append(_message);

            if (_resultSet)
            {
                sb.AppendFormat("; result: {0}", _result ?? "null");
            }

            using (var lm = new LogManager())
            {
                (lm as ILogging).VerboseTrace(_caller, sb.ToString());
            }
        }

        private readonly String _message;
        private readonly Object _caller;
        private String _result;
        private Boolean _resultSet;
    }
}
