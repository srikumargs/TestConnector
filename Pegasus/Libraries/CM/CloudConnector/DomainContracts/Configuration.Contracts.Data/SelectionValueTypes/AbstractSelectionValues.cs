using Sage.Connector.Configuration.Contracts.Data.DataTypes;

namespace Sage.Connector.Configuration.Contracts.Data.SelectionValueTypes
{
    
    /// <summary>
    /// Abstract exists in order to convert the values types
    /// </summary>
    public class AbstractSelectionValueTypes
    {
        /// <summary>
        /// SelectionTypes enum for each property data type
        /// </summary>
        public SelectionTypes SelectionType { get; protected set; }
    }
}
