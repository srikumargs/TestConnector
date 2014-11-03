using System;
using System.Collections.Generic;
using System.Windows;


namespace SageConnect.Control
{
    /// <summary>
    /// Property control class with the required properties for the control
    /// </summary>
   public class PropertyControl
    {

       /// <summary>
       /// Name of the control
       /// </summary>
       public string Name { get; set; }

       /// <summary>
       /// Datatype of the control 
       /// </summary>
       public DataTypeEnum DataType { get; set; }

       /// <summary>
       /// Look and feel selection type
       /// </summary>
       public SelectTypes SelectionType { get; set; }

       /// <summary>
       /// Display name for the conrol
       /// </summary>

       public string DisplayName { get; set; }
       /// <summary>
       /// Itemsource for Lookup
       /// </summary>
        public Dictionary<string, string> ItemDataDictionary { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public EventHandler CommandAction;

       /// <summary>
       /// Display Value of the Property/Control
       /// </summary>

       public string DisplayValue { get; set; }

       /// <summary>
       /// Value Selected in Control
       /// </summary>

       public string SelectedValue { get; set; }

       /// <summary>
       /// To set password component
       /// </summary>
       public bool IsPassword { get; set; }

       /// <summary>
       /// 
       /// </summary>
        public UIElement Element;
        /// <summary>
        /// 
        /// </summary>

        public bool IsPath { get; set; }


        /// <summary>
        /// 
        /// </summary>

        public bool IsDisabled { get; set; }



        /// <summary>
        /// 
        /// </summary>

        public string ErrorDescription { get; set; }

        /// <summary>
        /// 
        /// </summary>

        public bool Error { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
   public class Controlcollection : List<PropertyControl>
    {

    }

    /// <summary>
    /// Data types of the contol
    /// </summary>
   public enum DataTypeEnum
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
    /// Selection types
    /// </summary>
   public enum SelectTypes
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
       private SelectTypes _selectionEntryType = SelectTypes.None;

       /// <summary>
       /// DataTypes enum for each property data type
       /// </summary>
       public DataTypeEnum DataTypesEnum { get; protected set; }

       /// <summary>
       /// Selection Entry type.  The default is set to None
       /// </summary>
       public virtual SelectTypes SelectionType
       {
           get { return _selectionEntryType; }
           set { _selectionEntryType = value; }
       }
   }
}
