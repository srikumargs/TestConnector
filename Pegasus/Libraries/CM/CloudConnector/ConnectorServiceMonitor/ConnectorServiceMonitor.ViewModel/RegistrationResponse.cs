using System;

namespace ConnectorServiceMonitor.ViewModel
{
    /// <summary>
    /// A bit-wise flags enum that returns the results of a reconfiguration request
    /// </summary>
    [Flags]
    public enum RegistrationResponse
    {
        /// <summary>
        /// The reconfiguration process did not result in any change
        /// </summary>
        /// <remarks>
        /// This is default value the runtime automatically initializes any ReconfigureResponse instance to
        /// </remarks>
        None = 0x0,

        /// <summary>
        /// Registration request succeeded
        /// </summary>
        /// <remarks>check for ServerChangeDetected and ProcessOperatingModeChangeDetected in order to know what happened</remarks>
        Succeded = 0x01,

        /// <summary>
        /// Registration request failed
        /// </summary>
        Failed = 0x02,

        /// <summary>
        /// The server was changed
        /// </summary>
        /// <remarks>May be set when Succeded</remarks>
        ServerChangeDetected = 0x10,

        /// <summary>
        /// The process operating mode was changed
        /// </summary>
        /// <remarks>May be set when Succeded</remarks>
        ProcessOperatingModeChangeDetected = 0x20,
    }
}
