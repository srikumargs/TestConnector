using System;
using Sage.Diagnostics;

namespace Sage.Connector.Common
{
    /// <summary>
    /// A stubbed-out implementation of various LogManager api's for use when we don't have direct access to the LogManager
    /// (e.g., from the Connector UI due to permissions access to the DB), or when we otherwise don't want the entries to
    /// go to the LogStore (e.g., during retry operations inside the LogStore).
    /// </summary>
    public sealed class SimpleTraceLogger : IDisposable, ILogging
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="message"></param>
        /// <param name="list"></param>
        public void VerboseTrace(object caller, string message, params object[] list)
        { System.Diagnostics.Trace.WriteLine((list != null) ? String.Format(message, list) : message); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="message"></param>
        /// <param name="list"></param>
        public void InfoTrace(object caller, string message, params object[] list)
        { System.Diagnostics.Trace.WriteLine((list != null) ? String.Format(message, list) : message); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="message"></param>
        /// <param name="list"></param>
        public void WarningTrace(object caller, string message, params object[] list)
        { System.Diagnostics.Trace.WriteLine((list != null) ? String.Format(message, list) : message); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="message"></param>
        /// <param name="list"></param>
        public void ErrorTrace(object caller, string message, params object[] list)
        { System.Diagnostics.Trace.WriteLine((list != null) ? String.Format(message, list) : message); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="message"></param>
        /// <param name="list"></param>
        public void WriteError(object caller, string message, params object[] list)
        { System.Diagnostics.Trace.WriteLine((list != null) ? String.Format(message, list) : message); }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <param name="list"></param>
        public void WriteCriticalWithEventLogging(object caller, string source, string message, params object[] list)
        {
            String messageToWrite = message;
            if (list != null)
            {
                messageToWrite = String.Format(messageToWrite, list);
            }
            System.Diagnostics.Trace.WriteLine(messageToWrite);
            EventLogger.CreateEventLogSource(EventLogger.LogName, ConnectorRegistryUtils.FullProductName);
            EventLogger.WriteMessage(ConnectorRegistryUtils.FullProductName, messageToWrite, MessageType.Error);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        { }
    }
}
