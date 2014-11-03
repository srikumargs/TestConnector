using System;
using System.Collections.Generic;
using Sage.Connector.Configuration.Contracts.Data.DataTypes;

namespace Sage.Connector.Configuration.Contracts.Data.SelectionValueTypes
{
    /// <summary>
    /// Lookup type values 
    /// </summary>
    public sealed class LookupTypeValues : AbstractSelectionValueTypes
    {
        private Dictionary<string, Object> _lookupValues;

        /// <summary>
        /// Constructor
        /// </summary>
        public LookupTypeValues()
        {
            SelectionType = SelectionTypes.Lookup;
        }

        /// <summary>
        /// Lookup Values
        /// </summary>
        public Dictionary<string, Object> LookupValues
        {
            get { return _lookupValues ?? (_lookupValues = new Dictionary<string, Object>()); }
            set { _lookupValues = value; }
        }
    }
  
}
