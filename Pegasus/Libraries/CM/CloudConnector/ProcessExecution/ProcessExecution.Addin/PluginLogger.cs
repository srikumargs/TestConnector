using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using Sage.Connector.DomainContracts.BackOffice;
using Sage.Connector.ProcessExecution.AddinView;


namespace Sage.Connector.ProcessExecution.Addin
{
    /// <summary>
    /// 
    /// </summary>
    internal class PluginLogger : ILogging
    {
        private readonly LoggerCore _logger;

        public PluginLogger(LoggerCore logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callersThis"></param>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="callerMemberName">Supplied by compiler</param>
        /// <param name="callerFilePath">Supplied by compiler</param>
        /// <param name="callerLineNumber">Supplied by compiler</param>
        public void Write(
            Object callersThis,
            LogLevel level, string message, 
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0
            )
        {
            WriteCore(callersThis, level, message, null, null, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="callersThis"></param>
        /// <param name="level"></param>
        /// <param name="message"></param>
        /// <param name="options"></param>
        /// <param name="dataTags"></param>
        /// <param name="callerMemberName">Supplied by compiler</param>
        /// <param name="callerFilePath">Supplied by compiler</param>
        /// <param name="callerLineNumber">Supplied by compiler</param>
        public void Write(
            Object callersThis,
            LogLevel level, string message, IDictionary<string, string> options, IDictionary<string, string> dataTags,
            [CallerMemberName] string callerMemberName = "",
            [CallerFilePath] string callerFilePath = "",
            [CallerLineNumber] int callerLineNumber = 0
            )
        {
            WriteCore(callersThis,level,message,options,dataTags,callerMemberName, callerFilePath, callerLineNumber);
        }

        private void WriteCore(
            Object callersThis,
            LogLevel logLevel, string message, 
            IDictionary<string, string> options, IDictionary<string, string> dataTags,
            string callerMemberName = "",
            string callerFilePath = "",
            int callerLineNumber = 0
            )
        {
            //TODO: wire in the dictionary's to the returns once path is in for the data.
            //need to build the channel to get the rest of the meta data about logging back to the log
            //type names, line numbers, TenantId, request in process, etc.
            IDictionary<string, string> logOptions = options ?? new Dictionary<string, string>();
            IDictionary<string, string> logDataTags = dataTags ?? new Dictionary<string, string>();
            logDataTags["CallerMemberName"] = callerMemberName;
            logDataTags["CallerFilePath"] = callerFilePath;
            logDataTags["CallerLineNumber"] = callerLineNumber.ToString(CultureInfo.InvariantCulture);

            switch (logLevel)
            {
                case LogLevel.Critical:
                    _logger.WriteCriticalWithEventLogging(null, null, message);
                    break;
                case LogLevel.Error:
                    _logger.WriteError(null, message);
                    break;
                case LogLevel.Warning:
                    _logger.WriteWarning(null, message);
                    break;
                case LogLevel.Information:
                    _logger.WriteInfo(null, message);
                    break;
                case LogLevel.Verbose:
                    _logger.WriteVerbose(null, message);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("logLevel");
            }
        }
    }
}
