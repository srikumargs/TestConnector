
namespace Sage.Connector.Configuration.Contracts.Data.DataTypes
{
    /// <summary>
    /// Made this private because we are not sure that we can go this far at this point. 
    /// </summary>
    public enum DataTypesEnum
    {
        /// <summary>
        /// String.  
        /// </summary>
        StringType,

        /// <summary>
        /// Decimal, 
        /// </summary>
        DecimalType,

        /// <summary>
        /// Integer 
        /// </summary>
        IntegerType,


        /// <summary>
        /// DateTime data type
        /// </summary>
        DateTimeType,


        /// <summary>
        /// Boolean data type
        /// </summary>
        BooleanType
    }

    /// <summary>
    /// Selection Types
    /// </summary>
    public enum SelectionTypes
    {
        /// <summary>
        /// free form entry for the data type
        /// </summary>
        None,

        /// <summary>
        /// Lookup key value pair selection.  Key value is the specified data stored as property value
        /// </summary>
        Lookup,

        /// <summary>
        /// List of Data type selection 
        /// </summary>
        List
    }

    /// <summary>
    /// DataType properties
    /// </summary>
    public class AbstractDataType
    {
        private SelectionTypes _selectionEntryType = SelectionTypes.None;

        /// <summary>
        /// DataTypes enum for each property data type
        /// </summary>
        public DataTypesEnum DataTypesEnum { get; protected set; }

        /// <summary>
        /// Selection Entry type.  The default is set to None
        /// </summary>
        public virtual SelectionTypes SelectionType
        {
            get { return _selectionEntryType; }
            set { _selectionEntryType = value; }
        }
    }
}
