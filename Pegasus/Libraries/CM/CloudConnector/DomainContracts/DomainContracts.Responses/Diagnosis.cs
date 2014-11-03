using System;

namespace Sage.Connector.DomainContracts.Responses
{
    /// <summary>
    /// The status code which can be used on the response end to determine how to 
    /// handle the data associated with this diagnosis message. 
    /// </summary>
    public enum Severity
    {
        /// <summary>
        ///
        /// </summary>
        Error,

        /// <summary>
        /// 
        /// </summary>
        Warning,

        /// <summary>
        /// 
        /// </summary>
        Information
    }
    /// <summary>
    /// Diagnosis implementation
    /// </summary>
    public class Diagnosis
    {
        /// <summary>
        /// The status of the data associated with this Diagnosis
        /// </summary>
        public Severity  Severity { get; set; }

        /// <summary>
        /// The message for user display
        /// </summary>
        public String UserFacingMessage { get; set; }

        /// <summary>
        /// Techanical detail message such as stack trace.
        /// </summary>
        public String RawMessage { get; set; }
    }
}
