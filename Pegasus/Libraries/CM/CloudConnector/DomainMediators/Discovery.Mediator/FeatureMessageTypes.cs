
namespace Sage.Connector.Discovery.Mediator
{
    /// <summary>
    /// The supported set of features by the configuration domain mediator. 
    /// </summary>
    public static class FeatureMessageTypes
    {
        /// <summary>
        /// 
        /// </summary>
        public const string ValidateBackOfficeIsInstalled = "ValidateBackOfficeIsInstalled"; 

        /// <summary>
        /// 
        /// </summary>
        public const string GetPluginInformation = "GetPluginInformation";
        
        /// <summary>
        /// 
        /// </summary>
        public const string GetPluginInformationCollection = "GetPluginInformationCollection";

        /// <summary>
        /// 
        /// </summary>
        public const string GetInstalledBackOfficePluginInformationCollection = "GetInstalledBackOfficePluginInformationCollection";

        /// <summary>
        /// 
        /// </summary>
        public const string GetBackOfficeConfiguration = "GetBackOfficeConfiguration";
    }

   
}
