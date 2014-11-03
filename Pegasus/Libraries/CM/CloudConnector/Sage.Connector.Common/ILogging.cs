using System;

namespace Sage.Connector.Common
{
    /// <summary>
    /// A basic interface abstraction around certain methods we would like to be able to call on the LogManager
    /// from inside Common, but cannot, because the Common assembly would then create a circular reference on
    /// the Logging assembly.
    /// </summary>
    public interface ILogging
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="message"></param>
        /// <param name="list"></param>
        void VerboseTrace(Object caller, String message, params object[] list);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="message"></param>
        /// <param name="list"></param>
        void InfoTrace(Object caller, String message, params object[] list);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="message"></param>
        /// <param name="list"></param>
        void WarningTrace(Object caller, String message, params object[] list);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="message"></param>
        /// <param name="list"></param>
        void ErrorTrace(Object caller, String message, params object[] list);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="message"></param>
        /// <param name="list"></param>
        void WriteError(Object caller, String message, params object[] list);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="source"></param>
        /// <param name="message"></param>
        /// <param name="list"></param>
        void WriteCriticalWithEventLogging(Object caller, string source, string message, params object[] list);
    }
}
