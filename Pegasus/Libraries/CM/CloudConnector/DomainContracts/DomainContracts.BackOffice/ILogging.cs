using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Sage.Connector.DomainContracts.BackOffice
{
    /// <summary>
    /// Items written using these methods will be put in the connector log.
    /// This log will eventually be available in the cloud as well as locally
    /// </summary>
    public interface ILogging
    {
        /// <summary>
        /// Write a log entry to the connector
        /// </summary>
        /// <param name="callersThis">reference to caller to get data about source</param>
        /// <param name="level">severity of the entry</param>
        /// <param name="message">The message to log</param>
        /// <param name="callerMemberName">supplied by compiler</param>
        /// <param name="callerFilePath">supplied by compiler</param>
        /// <param name="callerLineNumber">supplied by compiler</param>
        void Write(
            Object callersThis,
            LogLevel level, string message, 
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0);

        /// <summary>
        /// Write a log entry to the connector
        /// </summary>
        /// <param name="callersThis">reference to caller to get data about source</param>
        /// <param name="level">severity of the entry</param>
        /// <param name="message">The message to log</param>
        /// <param name="options">
        /// Not yet implemented, will be support for finer control of logging. 
        /// Also used internally.
        /// </param>
        /// <param name="dataTags">
        /// Not yet implemented will be support for additional contextual data in a machine readable format.
        /// Also used internally.
        ///  </param>
        /// <param name="callerMemberName">supplied by compiler</param>
        /// <param name="callerFilePath">supplied by compiler</param>
        /// <param name="callerLineNumber">supplied by compiler</param>
        void Write(
            Object callersThis,
            LogLevel level, string message, IDictionary<string, string> options, IDictionary<string, string> dataTags,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0);
    }

    /// <summary>
    /// Levels for logging
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// A non-recoverable error
        /// </summary>
        Critical,
        /// <summary>
        /// A recoverable error
        /// </summary>
        Error,
        /// <summary>
        /// A warning
        /// </summary>
        Warning,
        /// <summary>
        /// Information
        /// </summary>
        Information,
        /// <summary>
        /// Verbose information
        /// </summary>
        Verbose
    }
}
