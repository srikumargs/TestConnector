using System;
using Sage.Connector.Configuration.Contracts.Data.DataTypes;

namespace Sage.Connector.Configuration.Contracts.Data
{

    /// <summary>
    /// Default implementation of the property definition. 
    /// </summary>
    public class PropertyDefinition
    {

        /// <summary>
        /// Default constructor for deserialization
        /// </summary>
        public PropertyDefinition()
        {
        }

        /// <summary>
        /// Constructor.  The Default property data type is String, The default BackOfficeValidation is true, but set to false to avoid the extra
        /// performance hit to the database if validation is not necessary as .
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <param name="displayName">Property Display Name used in an UI entry label.</param>
        /// <param name="description">Property Description to provide more information to assist the user in entering the required value</param>
        public PropertyDefinition(String propertyName, String displayName, String description)
        {
            BackOfficeValidation = true;
            PropertyDataType = new StringType();
            if (String.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentNullException("propertyName");
            }
            if (String.IsNullOrWhiteSpace(displayName))
            {
                throw new ArgumentNullException("displayName");
            }
          

            PropertyName = propertyName;
            DisplayName = displayName;
            Description = description;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="propertyName">Property name</param>
        /// <param name="displayName">Property Display Name used in an UI entry label.</param>
        /// <param name="description">Property Description to provide more information to assist the user in entering the required value</param>
        /// <param name="propertyDataType">Data Type from the selected set of Property data types</param>
        /// <param name="backOfficeValidation">true if backoffice  validation is required, false otherwise.  The default is false which means
        /// the entry is accepted as is without going to the back office to begin a session and validate the value</param>
        public PropertyDefinition(String propertyName, String displayName, String description,  AbstractDataType propertyDataType, Boolean backOfficeValidation)
        {
            BackOfficeValidation = true;
            PropertyDataType = new StringType();
            if (String.IsNullOrWhiteSpace(propertyName) )
            {
                throw new ArgumentNullException("propertyName");
            }
            if ( String.IsNullOrWhiteSpace(displayName) )
            {
                throw new ArgumentNullException("displayName");
            }
            if (propertyDataType == null)
            {
                throw new ArgumentNullException("propertyDataType");
            }

            PropertyName = propertyName;
            DisplayName = displayName;
            Description = description;
            PropertyDataType = propertyDataType;
            BackOfficeValidation = backOfficeValidation;
        }

        /// <summary>
        /// Name of the property
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Display a short text for entry. 
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Long description of the property. 
        /// </summary>
        public string Description { get; set; }


        /// <summary>
        /// DataType for the entry.  
        /// </summary>
        public AbstractDataType PropertyDataType { get; set; }

        /// <summary>
        /// Set to true, when entry requires a backoffice validation, which involves a BeginSession for connection to the company database.
        /// When true, the Validation method to validate the PropertyDefinition value is called.  When set to false, the method is not called and 
        /// therefore a session hit to the company database is avoided. 
        /// </summary>
        public bool BackOfficeValidation { get; set; }
    }

}
