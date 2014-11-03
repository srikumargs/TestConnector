namespace Sage.Connector.Common
{
    /// <summary>
    /// Used to control configuration change. How does the back office structure security.
    /// </summary>
    public enum ApplicationSecurityMode
    {
        /// <summary>
        /// 
        /// </summary>
        None = 0,

        /// <summary>
        /// 
        /// </summary>
        UsesGlobalApplicationSecurityAdministrator,

        /// <summary>
        /// 
        /// </summary>
        UsesPerCompanyAdministrator
    }
}
