namespace Sage.Connector.Configuration.Contracts.Data.DataTypes
{
    /// <summary>
    /// String Data Type which provides the ability to set the max length of the value
    /// </summary>
    public class StringType : AbstractDataType
    {

        /// <summary>
        /// String Type Attributes
        /// </summary>
        public StringType()
        {
            DataTypesEnum = DataTypesEnum.StringType;
        }
        /// <summary>
        /// Max Length. The Default is which implies no maximum length, which means as much as the textbox entry allows. 
        /// It is strongly suggested a max length value entered here.
        /// </summary>
        public int MaxLength { get; set; }

       
    }
}
