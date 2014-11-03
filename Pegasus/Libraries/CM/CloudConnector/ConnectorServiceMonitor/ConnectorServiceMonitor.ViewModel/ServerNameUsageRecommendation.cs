
namespace ConnectorServiceMonitor.ViewModel
{
    /// <summary>
    /// Indicates how the ServerName should be provided.
    /// </summary>
    public enum ServerNameUsageRecommendation
    {
        /// <summary>
        /// The usage recommendation is invalid
        /// </summary>
        /// <remarks>
        /// This is default value the runtime automatically initializes any ServerNameUsageRecommendation instance to
        /// </remarks>
        None,

        /// <summary>
        /// The server name should be used as is
        /// </summary>
        UseServerName,

        /// <summary>
        /// The fallback ip address should be used
        /// </summary>
        UseFallbackIPAddress
    }
}
