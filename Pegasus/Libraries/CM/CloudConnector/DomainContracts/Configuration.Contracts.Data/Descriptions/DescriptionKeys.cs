using System;

namespace Sage.Connector.Configuration.Contracts.Data.Descriptions
{
    /// <summary>
    /// Keys that can be added to a description to add capabilities.
    /// By default all items are strings, as they are persisted as such.
    /// Adding keys can customize how they are displayed or edited
    /// Expected to be used by builder.
    /// </summary>
    static public class DescriptionKeys
    {
        /// <summary>
        /// Used to sort the description items
        /// type int 
        /// </summary>
        static public readonly string DisplayOrder = "DisplayOrder";

        /// <summary>
        /// Short user facing name 
        /// type string
        /// </summary>
        static public readonly string DisplayName = "DisplayName";

        /// <summary>
        /// User facing description. 
        /// This is expected to be sentence or three.
        /// type string
        /// </summary>
        static public readonly string Description = "Description";

        /// <summary>
        /// Should the item be shown as a password
        /// </summary>
        static public readonly string IsPassword = "IsPassword";

        /// <summary>
        /// Should the item be treated as path. 
        /// This will allow a picker when it makes sense.
        /// </summary>
        static public readonly string IsPath = "IsPath";

        /// <summary>
        /// Value to show in the list
        /// IList of string
        /// </summary>
        /// <remarks>
        /// Will be value returned for list unless ValueId is present.
        /// </remarks>
        static public readonly string ValueName = "ValueName";

        /// <summary>
        /// Value to return for the selected ValueName
        /// Optional, will be ignored unless ValueName is present
        /// IList of string
        /// </summary>
        static public string ValueId = "ValueId";

        /// <summary>
        /// Longer description of item value
        /// Optional, will be ignored unless ValueDisplayName is present
        /// IList of string
        /// </summary>
        static public string ValueDescription = "ValueDescription";
    }

    /// <summary>
    /// Not currently looked for.
    /// But may make sense to add.
    /// </summary>
    class PossibleFutureKeys
    {
        //possible future attributes
        /// <summary>
        /// type int
        /// </summary>
        public string MinLength = "MinLength";

        /// <summary>
        /// type int
        /// </summary>
        public string MaxLength = "MaxLength";

        /// <summary>
        /// Give info that may help with rendering. 
        /// Possibly "class" name?
        ///    DisplayDescribeValueList
        ///     SimpleList
        ///     PathString
        ///     SimpleString
        /// </summary>
        public string DisplayHint = "DisplayHint";

 

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// IsCustomValueAllowed rules in the case of a list
        /// </remarks>
        public string IsReadonly = "IsReadonly";

        /// <summary>
        /// 
        /// </summary>
        public String IsNumber = "IsNumber";
    }
}
