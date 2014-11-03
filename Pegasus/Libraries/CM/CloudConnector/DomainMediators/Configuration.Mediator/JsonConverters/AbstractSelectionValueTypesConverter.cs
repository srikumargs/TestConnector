using System;
using Newtonsoft.Json.Linq;
using Sage.Connector.Configuration.Contracts.Data.DataTypes;
using Sage.Connector.Configuration.Contracts.Data.SelectionValueTypes;
using Sage.Connector.DomainMediator.Core.JsonConverters;

namespace Sage.Connector.Configuration.Mediator.JsonConverters
{  /// <summary>
    /// AbstractDataTypeValuesConverter
    /// </summary>
    public class AbstractSelectionValueTypesConverter : JsonCreationConverter<AbstractSelectionValueTypes>
    {
        /// <summary>
        /// Create the Selection entry type Values - to hold the set of selection values for entry
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="jObject"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        protected override AbstractSelectionValueTypes Create(Type objectType, JObject jObject)
        {

            var propertyDataType = (SelectionTypes)Enum.Parse(typeof(SelectionTypes), (string)jObject.Property("SelectionType"));
            switch (propertyDataType)
            {
                case SelectionTypes.Lookup:
                    return new LookupTypeValues();
                case SelectionTypes.List:
                    return new ListTypeValues();
            }
            throw new ApplicationException(String.Format("The given property data type {0} is not supported!", propertyDataType));
        }


    }
}
