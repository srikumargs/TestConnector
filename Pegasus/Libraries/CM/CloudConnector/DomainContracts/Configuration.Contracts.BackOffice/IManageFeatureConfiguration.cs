using System;
using System.Collections.Generic;
using Sage.Connector.Configuration.Contracts.Data;
using Sage.Connector.Configuration.Contracts.Data.Responses;
using Sage.Connector.Configuration.Contracts.Data.SelectionValueTypes;
using Sage.Connector.DomainContracts.BackOffice;

namespace Sage.Connector.Configuration.Contracts.BackOffice
{
    /// <summary>
    /// Feature properties that require configuration values in order to support feature
    /// This could be back office company specific. 
    /// </summary>
    public interface IManageFeatureConfiguration : IBackOfficeFeaturePropertyHandler
    {
        /// <summary>
        /// Get the list of properties for this feature
        /// </summary>
        /// <returns>If there aren't any properties, then there is no need to implement <see cref="IManageFeatureConfiguration"/></returns>
        ICollection<PropertyDefinition> GetFeatureProperties();

        /// <summary>
        /// Validate values associated with the back offices property configurations required for the feature
        /// The BeginSession is called before this method to allow for connection to the database.  
        /// EndSession is called after to close the connection to the database. 
        /// </summary>
        /// <param name="featurePropertyValuePairs">A dictionary keyed by property name with value to be validated </param>
        /// <returns>An collection of <see cref="PropertyValuePairValidationResponse"/></returns>
        ICollection<PropertyValuePairValidationResponse> ValidateFeatureConfigurationValues(IDictionary<String, Object> featurePropertyValuePairs);

        /// <summary>
        /// Setup the set of entry values for the set of properties defined for this feature.
        /// </summary>
        /// <param name="propertyEntryValues">Data Type specific default selection values by property name
        /// To be used in the configuration entry for the company. 
        /// In the case of a List type, a list will be given to populate.  
        /// In the case of Lookup type, a Dictionary will be given to populate with key value pairs. </param>
        void SetupFeatureConfigurationEntryValues(IDictionary<String, AbstractSelectionValueTypes> propertyEntryValues);


    }

}
