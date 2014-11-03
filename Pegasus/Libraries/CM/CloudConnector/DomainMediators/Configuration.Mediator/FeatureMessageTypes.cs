
namespace Sage.Connector.Configuration.Mediator
{
    /// <summary>
    /// The supported set of features by the configuration domain mediator. 
    /// </summary>
    public static class FeatureMessageTypes
    {
        /// <summary>
        /// Get Feature Configuration Properties.  
        /// The set does not depend on back office connection and is therefore considered discovery. 
        /// The assumption is that the property default entry would be required no matter which company is selected.
        /// Of course the default value would/could be different for each company and need to be entered for each company.
        /// 
        /// The feature property definitions are persisted by BackOfficeID and Feature Name (i.e. SyncCustomers).
        /// The key is the property name and the value is a json serialized property definition. 
        /// </summary>
        public const string GetFeatureConfigurationProperties = "GetFeatureConfigurationProperties";

        /// <summary>
        /// Setup Company Feature Property Selection values for those properties set to a List or Lookup selection type.
        /// For instance, if there is a key/value pair for PaymentTerms for the SyncCustomers feature for the 
        /// t
        /// </summary>
        public const string SetupCompanyFeaturePropertySelectionValues = "SetupCompanyFeaturePropertySelectionValues";
    
       
        /// <summary>
        /// Validate Company Feature Property Value pairs for those properties that have been set to require 
        /// back office validation.  Back Office validation means that a begin/end session is performed on
        /// the features containing properties that require validation. 
        /// </summary>
        public const string ValidateCompanyFeaturePropertyValuePairs = "ValidateCompanyFeaturePropertyValuePairs";


        /// <summary>
        /// Save persists the Company Feature properties pairs in a storage dictionary (SQL CE database)
        /// the name of the database is by TenantId and Feature Name (i.e. SyncCustomers).
        /// The Key is the Property Name and the value is an object representing the default property value. 
        /// </summary>
        public const string SaveCompanyFeaturePropertyValuePairs = "SaveCompanyFeaturePropertyValuePairs";     

        /// <summary>
        /// Get the Company Feature properties pairs from the storage dictionary (SQL CE database)
        /// the name of the database is by TenantId and Feature Name (i.e. SyncCustomers).
        /// The Key is the Property Name and the value is an object representing the default property value. 
        /// </summary>
        public const string GetCompanyFeaturePropertyValuePairs = "GetCompanyFeaturePropertyValuePairs";     

        /// <summary>
        /// 
        /// </summary>
        public const string GetCompanyConnectionCredentialsNeeded = "GetCompanyConnectionCredentialsNeeded";

        /// <summary>
        /// 
        /// </summary>
        public const string GetCompanyConnectionManagementCredentialsNeeded = "GetCompanyConnectionManagementCredentialsNeeded";

        /// <summary>
        /// 
        /// </summary>
        public const string ValidateCompanyConnectionCredentials = "ValidateCompanyConnectionCredentials";

        /// <summary>
        /// 
        /// </summary>
        public const string ValidateCompanyConnectionManagementCredentials = "ValidateCompanyConnectionManagementCredentials";
    }
}
