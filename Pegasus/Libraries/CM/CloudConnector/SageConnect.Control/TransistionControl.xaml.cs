using System;
using System.Collections.Generic;
using Sage.Connector.Configuration.Contracts.Data;
using Sage.Connector.Configuration.Contracts.Data.SelectionValueTypes;
using SageConnect.ViewModels;


namespace SageConnect.Control
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>

    public partial class TransistionControl
    {
        /// <summary>
        /// 
        /// </summary>
        public TransistionControlViewModel TransistionViewModel;
        /// <summary>
        /// Control constructor
        /// </summary>
        public TransistionControl()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Create the dynamic control for loading the admin and Connection credentials
        /// </summary>
        /// <param name="controlcollection"></param>
        /// <param name="headerText"></param>
        /// <param name="commandTypes"></param>
        /// <param name="selectedconnection"></param>
        /// <param name="presistcontrols"></param>
        /// <param name="reset"></param>
        /// <param name="readOnly"></param>
        public void CreateContent(Controlcollection controlcollection, String headerText,
            CommandTypes commandTypes,string selectedconnection,bool presistcontrols,bool reset,bool readOnly)
        {
             TransistionViewModel = new TransistionControlViewModel(controlcollection, headerText, commandTypes, selectedconnection, presistcontrols, reset, readOnly);
            Content = TransistionViewModel.Content;
         }


        /// <summary>
        /// Create the dynamic control to load the Feature configuration based on feature config properties
        /// </summary>
        /// <param name="featurePropertyLists"></param>
        /// <param name="featurePropertyEntryValues"></param>
        /// <param name="featurePropertyValuePairs"></param>
        /// <param name="commandTypes"></param>
        /// <param name="readOnly"></param>
        public void CreateFeatureContent(Dictionary<FeatureMetadata, IList<PropertyDefinition>> featurePropertyLists, IDictionary<string, Dictionary<String, AbstractSelectionValueTypes>> featurePropertyEntryValues, IDictionary<String, Dictionary<string, object>> featurePropertyValuePairs,CommandTypes commandTypes,bool readOnly)
         {
           TransistionViewModel = new TransistionControlViewModel();
           TransistionViewModel.IntilizeFeatureControl(featurePropertyLists, featurePropertyEntryValues, featurePropertyValuePairs,commandTypes,readOnly);
           Content = TransistionViewModel.Content;
         }
        /// <summary>
        /// Reload the feature configuration data based using same feature control
        /// </summary>
        /// <param name="featurePropertyLists"></param>
        /// <param name="featurePropertyEntryValues"></param>
        /// <param name="featurePropertyValuePairs"></param>
        /// <param name="commandTypes"></param>
        /// <param name="readOnly"></param>
        public void ReloadFeatureContent(Dictionary<FeatureMetadata, IList<PropertyDefinition>> featurePropertyLists, IDictionary<string, Dictionary<String, AbstractSelectionValueTypes>> featurePropertyEntryValues, IDictionary<String, Dictionary<string, object>> featurePropertyValuePairs, CommandTypes commandTypes, bool readOnly)
        {
            TransistionViewModel.IntilizeFeatureControl(featurePropertyLists, featurePropertyEntryValues, featurePropertyValuePairs, commandTypes, readOnly);
            Content = TransistionViewModel.Content;
        }
        /// <summary>
        /// Setting the Control's readonly
        /// </summary>
        /// <param name="readOnly"></param>
        public void SetControlReadOnly(bool readOnly)
        {
            TransistionViewModel.SetConnectionReadOnly(readOnly);
        }
 }
}

