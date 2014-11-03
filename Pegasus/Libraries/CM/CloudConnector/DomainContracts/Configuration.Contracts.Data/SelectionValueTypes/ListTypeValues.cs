using System;
using System.Collections.Generic;
using Sage.Connector.Configuration.Contracts.Data.DataTypes;

namespace Sage.Connector.Configuration.Contracts.Data.SelectionValueTypes
{
    /// <summary>
    /// List Type Values
    /// </summary>
    public class ListTypeValues : AbstractSelectionValueTypes
    {
        private IList<Object> _listValues;

        /// <summary>
        /// Constructor
        /// </summary>
        public ListTypeValues()
        {
            SelectionType= SelectionTypes.List;
        }
        /// <summary>
        /// List Values
        /// </summary>
        public IList<Object> ListValues
        {
            get { return _listValues ?? (_listValues = new List<Object>()); }
            set { _listValues = value; }
        }
    }
}
