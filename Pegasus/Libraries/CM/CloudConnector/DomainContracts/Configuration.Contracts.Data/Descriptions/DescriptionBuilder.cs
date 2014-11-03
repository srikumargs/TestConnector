using System;
using System.Collections.Generic;

namespace Sage.Connector.Configuration.Contracts.Data.Descriptions
{
    /// <summary>
    /// Support for DescriptionBuilders
    /// </summary>
    public abstract class DescriptionBuilder
    {
        /// <summary>
        /// Name to display
        /// </summary>
        public string DisplayName
        {
            get
            {
                object value;
                _description.TryGetValue(DescriptionKeys.DisplayName, out value);
                return (string)value;
            }
            set { _description[DescriptionKeys.DisplayName] = value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DescriptionBuilder"/> class.
        /// </summary>
        protected DescriptionBuilder()
        {
            _description = new Dictionary<string, object>();
        }

        /// <summary>
        /// Check if done building has been called. 
        /// Will throw an InvalidOperationException by design if 
        /// </summary>
        protected void CheckDoneBuilding()
        {
            if (_doneBuilding)
                throw new InvalidOperationException();
        }
        
        /// <summary>
        /// Are we done building
        /// </summary>
        protected bool _doneBuilding;
        
        /// <summary>
        /// dictionary to use for constructing.
        /// </summary>
        protected readonly Dictionary<string, object> _description;
    }

    /// <summary>
    /// Build a list with id, name and description
    /// One common use is for company lists
    /// </summary>
    public class ThreePartListDescriptionBuilder : DescriptionBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThreePartListDescriptionBuilder"/> class.
        /// </summary>
        public ThreePartListDescriptionBuilder()
            :base()
        {
            _ids = new List<string>();
            _names = new List<string>();
            _valueDescriptions = new List<string>();
        }

        /// <summary>
        /// Add item to the list
        /// </summary>
        /// <param name="id">internal id</param>
        /// <param name="name">Name to show</param>
        /// <param name="description">description to show</param>
        public void Add(string id, string name, string description)
        {
            CheckDoneBuilding();

            _ids.Add(id);
            _names.Add(name);
            _valueDescriptions.Add(description);
        }
        
        /// <summary>
        /// Returns the description
        /// </summary>
        /// <returns>Dictionary with the specified descriptions</returns>
        public IDictionary<string, object> ToDescription()
        {
            CheckDoneBuilding();

            _doneBuilding = true;
            
            _description[DescriptionKeys.ValueId] = _ids;
            _description[DescriptionKeys.ValueName] = _names;
            _description[DescriptionKeys.ValueDescription] = _valueDescriptions;
            return _description;
        }

        readonly IList<string> _ids;
        readonly IList<string> _names;
        readonly IList<string> _valueDescriptions;
    }

    /// <summary>
    /// Builds a simple string value
    /// </summary>
    public class SimpleStringDescriptionBuilder : DescriptionBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleStringDescriptionBuilder"/> class.
        /// </summary>
        public SimpleStringDescriptionBuilder()
            :base()
        {
        }

        /// <summary>
        /// If true the string is treated as a password
        /// </summary>
        public bool? IsPassword
        {
            get
            {
                object value;
                _description.TryGetValue(DescriptionKeys.IsPassword, out value);
                return (bool?)value;
            }
            set { _description[DescriptionKeys.IsPassword] = value; }
        }

        /// <summary>
        /// Returns the description
        /// </summary>
        /// <returns>Dictionary with the specified descriptions</returns>
        public IDictionary<string, object> ToDescription()
        {
            CheckDoneBuilding();

            _doneBuilding = true;
            return _description;
        }
    }

    /// <summary>
    /// Builds a path string, editor is expected to supply a path picker if possible
    /// </summary>
    public class PathStringDescriptionBuilder : DescriptionBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PathStringDescriptionBuilder"/> class.
        /// </summary>
        public PathStringDescriptionBuilder()
            :base()
        {
            IsPath = true;
        }

        /// <summary>
        /// Property to control if this is treated as a path, is set to true by default
        /// </summary>
        public bool? IsPath
        {
            get
            {
                object value;
                _description.TryGetValue(DescriptionKeys.IsPath, out value);
                return (bool?)value;
            }
            set { _description[DescriptionKeys.IsPath] = value; }
        }

        /// <summary>
        /// Returns the description
        /// </summary>
        /// <returns>Dictionary with the specified descriptions</returns>
        public IDictionary<string, object> ToDescription()
        {
            CheckDoneBuilding();
            _doneBuilding = true;
            return _description;
        }
    }

    /// <summary>
    /// Build a list with items that consist only of a name
    /// </summary>
    public class SimpleListDescriptionBuilder : DescriptionBuilder
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleListDescriptionBuilder"/> class.
        /// </summary>
        public SimpleListDescriptionBuilder()
        {
            _names = new List<string>();
        }

        /// <summary>
        /// Add an item to the list
        /// </summary>
        /// <param name="name">Name to display for the list item</param>
        public void Add(string name)
        {
            CheckDoneBuilding();
            _names.Add(name);
        }

        /// <summary>
        /// Returns the description
        /// </summary>
        /// <returns>Dictionary with the specified descriptions</returns>
        public IDictionary<string, object> ToDescription()
        {
            CheckDoneBuilding();

            _doneBuilding = true;
            _description[DescriptionKeys.ValueName] = _names;
            return _description;
        }

        readonly IList<string> _names;
    }
}
