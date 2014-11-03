using System;
using Newtonsoft.Json.Linq;
using Sage.Connector.Configuration.Contracts.Data.DataTypes;
using Sage.Connector.DomainMediator.Core.JsonConverters;

namespace Sage.Connector.Configuration.Mediator.JsonConverters
{

    /// <summary>
    /// Abstract Data Type Converter
    /// </summary>
    public class AbstractDataTypeConverter : JsonCreationConverter<AbstractDataType>
    {
        /// <summary>
        /// Create the data type
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="jObject"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        protected override AbstractDataType Create(Type objectType, JObject jObject)
        {

            var propertyDataType = (DataTypesEnum)Enum.Parse(typeof(DataTypesEnum), (string)jObject.Property("DataTypesEnum"));
            switch (propertyDataType)
            {
                case DataTypesEnum.StringType:
                    return new StringType();
                case DataTypesEnum.DecimalType:
                    return new DecimalType();
                case DataTypesEnum.BooleanType:
                    return new BooleanType();
                case DataTypesEnum.IntegerType:
                    return new IntegerType();
                case DataTypesEnum.DateTimeType:
                    return new DateTimeType();

            }
            throw new ApplicationException(String.Format("The given property data type {0} is not supported!", propertyDataType));
        }


    }

  

}
