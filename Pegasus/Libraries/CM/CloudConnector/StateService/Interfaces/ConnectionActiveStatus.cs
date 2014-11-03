namespace Sage.Connector.StateService.Interfaces
{

    /// <summary>
    /// Connection active status
    /// </summary>
    public enum ConnectionActiveStatus
    {
        /// <summary>
        /// No ConnectionActiveStatus (default value automatically initialized by runtime)
        /// </summary>
        None = 0,

        /// <summary>
        /// Connection is actively communicating
        /// </summary>
        Active,

        /// <summary>
        /// Connection is not actively communicating
        /// </summary>
        Inactive,

        /// <summary>
        /// Connection is configured to communicate, but is not functioning properly
        /// </summary>
        Broken
    }
}