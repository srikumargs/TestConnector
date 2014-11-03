using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Sage.Connector.Configuration.Contracts.Data;
using Sage.Connector.Configuration.Contracts.Data.DataTypes;
using Sage.Connector.Configuration.Contracts.Data.Responses;
using Sage.Connector.Configuration.Contracts.Data.SelectionValueTypes;
using Sage.Connector.Configuration.Mediator;
using Sage.Connector.Configuration.Mediator.JsonConverters;
using Sage.Connector.DomainContracts.Responses;
using Sage.Connector.DomainMediator.Core.JsonConverters;
using Sage.Connector.StateService.Interfaces.DataContracts;
using SageConnector.Internal;
using SageConnector.Properties;
using SageConnector.ViewModel;

namespace SageConnector
{
    /// <summary>
    /// 
    /// </summary>
    public partial class CloudConnectorFeatureConfigurationsForm : Form
    {
        // ReSharper disable once NotAccessedField.Local
        private ConfigurationViewModel _configuration;
        // ReSharper disable once NotAccessedField.Local
        private ConnectionCredentialsNeededResponse _credentials;
        private readonly IDictionary<FeatureMetadata, IList<PropertyDefinition>> _featurePropertyLists;
        private IDictionary<string, Dictionary<String, AbstractSelectionValueTypes>> _featurePropertyEntryValues;

        private readonly ICollection<Object> _dataObjects = new Collection<object>();

        private readonly IList<FeaturePropControl> _featurePropControls = new List<FeaturePropControl>();

        private bool _canClose = true;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="credentials"></param>
        /// <param name="featurePropertyLists"></param>
        public CloudConnectorFeatureConfigurationsForm(ConfigurationViewModel config, ConnectionCredentialsNeededResponse credentials,
            Dictionary<FeatureMetadata, IList<PropertyDefinition>> featurePropertyLists)
        {

            Debug.Assert(config != null, "ConfigurationViewModel cannot be null");
            _configuration = config;
            _credentials = credentials;
            _featurePropertyLists = featurePropertyLists;
            InitializeComponent();
        }

        private void CloudConnectorFeatureConfigurationsForm_Load(object sender, EventArgs e)
        {
            if (!_featurePropertyLists.Any())
                return;

            grpBackOfficeIdCompany.Text = string.Format(grpBackOfficeIdCompany.Text, _configuration.ConnectorPluginId,
                _configuration.BackOfficeCompanyName);
            


            _featurePropertyEntryValues = GetFeaturePropertyEntryValues();
            var currentFeaturePropValuePairs = GetCompanyFeaturePropertyValuePairs();


            const int controlRowHeight = 25;
            const int featureGroupOffsetHeight = 40;
            int featureRow = 0;
            int tabIndex = Controls.Count - 1;
            tableLayoutPanel1.RowCount = _featurePropertyLists.Count();
            tableLayoutPanel1.AutoScroll = true;

            foreach (var featurePropertyList in _featurePropertyLists)
            {
                tabIndex++;

                // 
                // groupBox
                // 
                var groupBox = new GroupBox();

                groupBox.Location = new Point(3, 3);
                groupBox.Name = "groupBox" + featurePropertyList.Key.Name;
                groupBox.AutoSize = false;
                groupBox.TabIndex = tabIndex;
                groupBox.TabStop = false;
                groupBox.Text = featurePropertyList.Key.DisplayName;
                groupBox.Size = new Size(400, featurePropertyList.Value.Count * controlRowHeight + featureGroupOffsetHeight);
                groupBox.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
                groupBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                tableLayoutPanel1.Controls.Add(groupBox, 0, featureRow);
                // 
                // tableLayoutPanel of the Feature Set
                // 
                tabIndex++;
                var tableLayoutPanel = new TableLayoutPanel();



                tableLayoutPanel.TabIndex = tabIndex;
                tableLayoutPanel.AutoSize = true;
                tableLayoutPanel.ColumnCount = 2;
                tableLayoutPanel.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);

                for (int i = 0; i < tableLayoutPanel.RowCount; i++)
                {
                    tableLayoutPanel.ColumnStyles.Add(new ColumnStyle
                    {
                        SizeType = SizeType.AutoSize
                    });
                }

                tableLayoutPanel.Dock = DockStyle.Top;
                tableLayoutPanel.Location = new Point(3, 16);
                tableLayoutPanel.Name = "tableLayoutPanel" + featurePropertyList.Key.Name;
                tableLayoutPanel.Padding = new Padding(5, 5, 0, 0);
                tableLayoutPanel.RowCount = featurePropertyList.Value.Count;
                for (var i = 0; i < featurePropertyList.Value.Count; i++)
                {
                    tableLayoutPanel.RowStyles.Add(new RowStyle
                    {
                        SizeType = SizeType.Absolute,
                        Height = controlRowHeight
                    });
                }
                tableLayoutPanel.Size = new Size(793, 54);
                tableLayoutPanel.AutoSize = true;
                tableLayoutPanel.TabIndex = tabIndex;
                groupBox.Controls.Add(tableLayoutPanel);

                int propRow = 0;


                IDictionary<String, AbstractSelectionValueTypes> featurePropertiesEntryValues = new Dictionary<string, AbstractSelectionValueTypes>();

                if (_featurePropertyEntryValues.ContainsKey(featurePropertyList.Key.Name))
                {
                    featurePropertiesEntryValues = _featurePropertyEntryValues[featurePropertyList.Key.Name];
                }

                Dictionary<string, object> currPropValuePairs = new Dictionary<string, object>();
                if (currentFeaturePropValuePairs.ContainsKey(featurePropertyList.Key.Name))
                {
                    currPropValuePairs = currentFeaturePropValuePairs[featurePropertyList.Key.Name];
                }
                foreach (var prop in featurePropertyList.Value)
                {
                    tabIndex++;
                    // 
                    // propLabel
                    // 
                    var propLabel = new Label();
                    propLabel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
                    propLabel.AutoSize = true;
                    propLabel.Location = new Point(8, 9);
                    propLabel.Name = "label" + featurePropertyList.Key.Name + prop.PropertyName;
                    propLabel.Padding = new Padding(0, 0, 18, 5);
                    propLabel.Size = new Size(110, 18);
                    propLabel.TabIndex = tabIndex;
                    propLabel.Text = prop.DisplayName;

                    tableLayoutPanel.Controls.Add(propLabel, 0, propRow);


                    AbstractSelectionValueTypes propEntryValues = featurePropertiesEntryValues.ContainsKey(prop.PropertyName)
                        ? featurePropertiesEntryValues[prop.PropertyName] : null;

                    // 
                    // propEntry
                    // 
                    tabIndex++;
                    Control propEntry = CreateEntryControl(prop, propEntryValues);
                    propEntry.Location = new Point(124, 3);
                    propEntry.Name = "entry" + featurePropertyList.Key.Name + prop.PropertyName;
                    propEntry.TabIndex = tabIndex;

                    tableLayoutPanel.Controls.Add(propEntry, 1, propRow);

                    if (currPropValuePairs.ContainsKey(prop.PropertyName))
                    {
                        SetEntryControlValue(propEntry, prop, propEntryValues, currPropValuePairs[prop.PropertyName]);
                    }
                    else
                    {
                        SetEntryControlValue(propEntry, prop, propEntryValues, null);
                    }


                    // Set up the ToolTip text for the entry control.
                    toolTip1.SetToolTip(propEntry, prop.Description);


                    _featurePropControls.Add(new FeaturePropControl
                    {
                        EntryControl = propEntry,
                        FeatureName = featurePropertyList.Key.Name,
                        PropertyName = prop.PropertyName
                    });
                    propRow++;
                }



                featureRow++;
            }

        }

        private void SetEntryControlValue(Control control, PropertyDefinition prop,
            AbstractSelectionValueTypes propTypeValues, object value)
        {

            switch (prop.PropertyDataType.SelectionType)
            {
                case SelectionTypes.None:
                    {
                        if (value == null)
                            return;

                        switch (prop.PropertyDataType.DataTypesEnum)
                        {
                            case DataTypesEnum.BooleanType:
                                {
                                    CheckBox entryControl = control as CheckBox;
                                    if (entryControl == null)
                                        throw new ApplicationException(string.Format("Expected CheckBox control for '{0}'",
                                            prop.DisplayName));

                                    entryControl.Checked = Boolean.Parse(value.ToString());
                                    break;

                                }
                            case DataTypesEnum.DateTimeType:
                                {
                                    DateTimePicker entryControl = control as DateTimePicker;
                                    if (entryControl == null)
                                        throw new ApplicationException(string.Format(
                                            "Expected DateTimePicker control for '{0}'",
                                            prop.DisplayName));
                                    entryControl.Value = DateTime.Parse(value.ToString());
                                    break;

                                }
                            case DataTypesEnum.StringType:
                                {
                                    TextBox entryControl = control as TextBox;
                                    if (entryControl == null)
                                        throw new ApplicationException(string.Format("Expected TextBox control for '{0}'",
                                            prop.DisplayName));
                                    entryControl.Text = value.ToString();
                                    break;
                                }
                            case DataTypesEnum.IntegerType:
                                {
                                    NumericUpDown entryControl = control as NumericUpDown;

                                    if (entryControl == null)
                                        throw new ApplicationException(
                                            string.Format("Expected NumericUpDown control for '{0}'",
                                                prop.DisplayName));
                                    entryControl.Value = Int32.Parse(value.ToString());
                                    break;
                                }

                            case DataTypesEnum.DecimalType:
                                {
                                    NumericUpDown entryControl = control as NumericUpDown;
                                    if (entryControl == null)
                                        throw new ApplicationException(string.Format("Expected NumericUpDown control for '{0}'",
                                            prop.DisplayName));
                                    entryControl.Value = Decimal.Parse(value.ToString());
                                    break;

                                }
                        }
                        break;
                    }

                case SelectionTypes.List:
                    {
                        var listTypeValues = propTypeValues as ListTypeValues ?? new ListTypeValues();

                        ComboBox entryControl = control as ComboBox;
                        if (entryControl == null)
                            throw new ApplicationException(string.Format("Expected ComboBox control for '{0}'",
                                prop.DisplayName));

                        //var selectedIndex = listTypeValues.ListValues.ToList().IndexOf(value);
                        entryControl.SelectedItem = value;


                        break;
                    }
                case SelectionTypes.Lookup:
                    {
                        var lookupTypeValues = propTypeValues as LookupTypeValues ?? new LookupTypeValues();

                        ComboBox entryControl = control as ComboBox;
                        if (entryControl == null)
                            throw new ApplicationException(string.Format("Expected ComboBox control for '{0}'",
                                prop.DisplayName));

                        if (value == null)
                        {
                            entryControl.SelectedItem = null;
                        }
                        else
                        {
                            var kvPair =
                           (from l in lookupTypeValues.LookupValues where l.Key.Equals(value) select l).FirstOrDefault();

                            if (kvPair.Key.Equals(value))
                            {
                                entryControl.SelectedItem = kvPair;
                            }
                            else
                            {
                                entryControl.SelectedItem = null;
                            }
                        }
                        break;
                    }
                default:
                    {
                        if (value == null)
                            return;

                        TextBox entryControl = control as TextBox;
                        if (entryControl == null)
                            throw new ApplicationException(string.Format("Expected TextBox control for '{0}'",
                                prop.DisplayName));
                        entryControl.Text = value.ToString();
                        break;
                    }

            }


        }


        private IDictionary<string, Dictionary<String, AbstractSelectionValueTypes>> GetFeaturePropertyEntryValues()
        {
            if (_featurePropertyEntryValues != null)
                return _featurePropertyEntryValues;




            var featurePropSetups = new Dictionary<string, Dictionary<String, AbstractSelectionValueTypes>>();
            foreach (var featureSet in _featurePropertyLists)
            {
                var propDefinitions = (List<PropertyDefinition>)featureSet.Value;
                var propSetupDictionary = new Dictionary<String, AbstractSelectionValueTypes>();
                foreach (var propDef in propDefinitions)
                {
                    if (propDef.PropertyDataType.SelectionType.Equals(SelectionTypes.List))
                    {
                        propSetupDictionary.Add(propDef.PropertyName, new ListTypeValues());
                        continue;
                    }
                    if (propDef.PropertyDataType.SelectionType.Equals(SelectionTypes.Lookup))
                    {
                        propSetupDictionary.Add(propDef.PropertyName, new LookupTypeValues());
                    }

                    //other types unsupported for backoffice setup at this time. 
                }
                featurePropSetups.Add(featureSet.Key.Name, propSetupDictionary);
            }

            if (!featurePropSetups.Any())
            {
                _featurePropertyEntryValues = new Dictionary<string, Dictionary<string, AbstractSelectionValueTypes>>();
                return _featurePropertyEntryValues;
            }

            var cfg = new DomainMediatorJsonSerializerSettings();
            cfg.ContractResolver = new DictionaryFriendlyContractResolver();
            cfg.Converters.Add(new KeyValuePairConverter());

            string setupFeatureConfigPayload = JsonConvert.SerializeObject(featurePropSetups, cfg);

            cfg.Converters.Add(new AbstractSelectionValueTypesConverter());


            var featureResponse = MakeFeatureRequest("Getting feature configuration property entry values",
            "SetupCompanyFeaturePropertySelectionValues", setupFeatureConfigPayload);

            cfg = new DomainMediatorJsonSerializerSettings();
            cfg.ContractResolver = new DictionaryFriendlyContractResolver();
            cfg.Converters.Add(new KeyValuePairConverter());
            cfg.Converters.Add(new AbstractDataTypeConverter());
            cfg.Converters.Add(new AbstractSelectionValueTypesConverter());
            var featurePropEntryValues = JsonConvert.DeserializeObject<IList<KeyValuePair<string,
               IList<KeyValuePair<String, AbstractSelectionValueTypes>>>>>(featureResponse.Payload, cfg);

            Dictionary<string, Dictionary<string, AbstractSelectionValueTypes>> test;
            test = featurePropEntryValues.ToDictionary(x => x.Key, y => y.Value.ToDictionary(a => a.Key, b => b.Value));
            _featurePropertyEntryValues = test;
            return _featurePropertyEntryValues;
        }

        private Dictionary<String, Dictionary<string, object>> GetCompanyFeaturePropertyValuePairs()
        {
            var featureListPayload = JsonConvert.SerializeObject(from f in _featurePropertyLists select f.Key.Name, new DomainMediatorJsonSerializerSettings());

            var featureResponse = MakeFeatureRequest("Get Company Feature Configuration", "GetCompanyFeaturePropertyValuePairs",
                featureListPayload);

            var cfg = new DomainMediatorJsonSerializerSettings
            {
                ContractResolver = new DictionaryFriendlyContractResolver()
            };
            cfg.Converters.Add(new KeyValuePairConverter());

            var currentValues = JsonConvert.DeserializeObject<IList<KeyValuePair<String, IList<KeyValuePair<string, object>>>>>(featureResponse.Payload, cfg);
            return currentValues.ToDictionary(x => x.Key, y => y.Value.ToDictionary(a => a.Key, b => b.Value));
        }

        private Control CreateEntryControl(PropertyDefinition prop, AbstractSelectionValueTypes propTypeValues)
        {
            switch (prop.PropertyDataType.SelectionType)
            {
                case SelectionTypes.None:
                    {
                        switch (prop.PropertyDataType.DataTypesEnum)
                        {
                            case DataTypesEnum.BooleanType:
                                {
                                    var boolControl = new CheckBox { Text = "" };
                                    return boolControl;
                                }
                            case DataTypesEnum.DateTimeType:
                                {
                                    return new DateTimePicker();
                                }
                            case DataTypesEnum.StringType:
                                {
                                    var textBox = new TextBox();
                                    var dataType = prop.PropertyDataType as StringType;
                                    if (dataType != null && dataType.MaxLength > 0)
                                    {
                                        textBox.MaxLength = dataType.MaxLength;
                                        textBox.Width = dataType.MaxLength * 20;
                                    }
                                    return textBox;
                                }
                            case DataTypesEnum.DecimalType:
                                {
                                    var numericUpDown = new NumericUpDown
                                    {
                                        Maximum = Decimal.MaxValue,
                                        DecimalPlaces = 2
                                    };

                                    return numericUpDown;
                                }

                            case DataTypesEnum.IntegerType:
                                {
                                    var numericUpDown = new NumericUpDown
                                    {
                                        Maximum = Int32.MaxValue,
                                        DecimalPlaces = 0
                                    };

                                    return numericUpDown;
                                }
                        }
                        break;
                    }

                case SelectionTypes.List:
                    {
                        var listTypeValues = propTypeValues as ListTypeValues ?? new ListTypeValues();
                        _dataObjects.Add(listTypeValues.ListValues);
                        var comboBox = new ComboBox
                        {
                            DropDownStyle = ComboBoxStyle.DropDownList,
                            DataSource = new BindingSource(listTypeValues.ListValues, null),
                            ValueMember = "Value",
                            DisplayMember = "Value",
                            Width = 400,
                            AutoSize = true
                        };
                        return comboBox;
                    }
                case SelectionTypes.Lookup:
                    {
                        var lookupTypeValues = propTypeValues as LookupTypeValues ?? new LookupTypeValues();
                        _dataObjects.Add(lookupTypeValues.LookupValues);
                        var comboBox = new ComboBox
                        {
                            DropDownStyle = ComboBoxStyle.DropDownList,
                            DataSource = new BindingSource(lookupTypeValues.LookupValues, null),
                            ValueMember = "Key",
                            DisplayMember = "Value",
                            Width = 400,
                            AutoSize = true
                        };
                        return comboBox;
                    }

            }

            //not corresponding control for property definition
            var entryControl = new TextBox();
            entryControl.Width = 400;
            return entryControl;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            var validateFeaturePropValuePairs = new Dictionary<string, IDictionary<String, Object>>();
            var savePropValuePairs = new Dictionary<string, IDictionary<String, Object>>();

            foreach (var featurePropControl in _featurePropControls)
            {
                errorProvider1.SetError(featurePropControl.EntryControl, "");

                Debug.Print("{0}, {1}", featurePropControl.FeatureName, featurePropControl.PropertyName);
                FeaturePropControl control = featurePropControl;
                var featureProps = (from fp in _featurePropertyLists
                                    where fp.Key.Name.Equals(control.FeatureName)
                                    select fp).FirstOrDefault();
                var prop = (from p in featureProps.Value where p.PropertyName.Equals(control.PropertyName) select p).FirstOrDefault();
                if (prop == null)
                    throw new ApplicationException(string.Format("Property definition missing for '{0}' property of '{1}' feature",
                        control.PropertyName, featurePropControl.FeatureName));

                //TODO: at this point we have the property definition and can determine how to get the value out of the control to be saved. 
                object value = GetEntryControlValue(prop, control.EntryControl).ToString();

                if (!savePropValuePairs.ContainsKey(control.FeatureName))
                {
                    savePropValuePairs.Add(control.FeatureName, new Dictionary<string, object>());
                }
                savePropValuePairs[control.FeatureName].Add(prop.PropertyName, value);

                if (prop.BackOfficeValidation)
                {
                    if (!validateFeaturePropValuePairs.ContainsKey(control.FeatureName))
                    {
                        validateFeaturePropValuePairs.Add(control.FeatureName, new Dictionary<string, object>());
                    }
                    validateFeaturePropValuePairs[control.FeatureName].Add(prop.PropertyName, value);
                }
            }

            bool canSave = true;
            Dictionary<string, ICollection<PropertyValuePairValidationResponse>> validationResponses = ValidateFeaturePropertyValuePairs(validateFeaturePropValuePairs);
            foreach (var validationResponse in validationResponses)
            {
                string featureName = validationResponse.Key;
                foreach (var propValuePairResponse in validationResponse.Value)
                {
                    if (propValuePairResponse.Status.Equals(Status.Failure))
                    {
                        Control entryControl = (from c in _featurePropControls
                                                where c.FeatureName.Equals(featureName)
                                                && c.PropertyName.Equals(propValuePairResponse.PropertyValuePair.Key)
                                                select c.EntryControl).FirstOrDefault();
                        if (entryControl != null)
                        {
                            var msgs = from m in propValuePairResponse.Diagnoses select m.UserFacingMessage;
                            string msg = null;
                            // ReSharper disable once ConditionIsAlwaysTrueOrFalse -- this is not a valid assessment of the code.
                            if (msgs != null)
                            {
                                msg = string.Join(Environment.NewLine, msgs);
                            }
                            msg = (String.IsNullOrWhiteSpace(msg)) ? "InvaliValue" : msg;

                            errorProvider1.SetError(entryControl, msg);
                        }
                        canSave = false;
                    }
                }
            }
            _canClose = canSave;
            if (canSave)
            {
                var response = SaveFeaturePropertyValuePairs(savePropValuePairs);
                if (response.Status.Equals(Status.Failure))
                {
                    _canClose = false;
                    string msg = @"Unexpected error. Entry values could not be saved.";
                    if (response.Diagnoses.Any())
                        msg = response.Diagnoses[0].UserFacingMessage;
                    //dislay Error Message and cancel the close
                    MessageBox.Show(msg,
                        @"Save Feature Configuration Entry Values",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);

                }
            }

        }

        private object GetEntryControlValue(PropertyDefinition prop, Control control)
        {

            switch (prop.PropertyDataType.SelectionType)
            {
                case SelectionTypes.None:
                    {
                        switch (prop.PropertyDataType.DataTypesEnum)
                        {
                            case DataTypesEnum.BooleanType:
                                {
                                    CheckBox entryControl = control as CheckBox;
                                    if (entryControl == null)
                                        throw new ApplicationException(string.Format("Expected CheckBox control for '{0}'",
                                            prop.DisplayName));
                                    return entryControl.Checked;

                                }
                            case DataTypesEnum.DateTimeType:
                                {
                                    DateTimePicker entryControl = control as DateTimePicker;
                                    if (entryControl == null)
                                        throw new ApplicationException(string.Format(
                                            "Expected DateTimePicker control for '{0}'",
                                            prop.DisplayName));
                                    return entryControl.Value;

                                }
                            case DataTypesEnum.StringType:
                                {
                                    TextBox entryControl = control as TextBox;
                                    if (entryControl == null)
                                        throw new ApplicationException(string.Format("Expected TextBox control for '{0}'",
                                            prop.DisplayName));
                                    return entryControl.Text;
                                }
                            case DataTypesEnum.IntegerType:
                            case DataTypesEnum.DecimalType:
                                {
                                    NumericUpDown entryControl = control as NumericUpDown;
                                    if (entryControl == null)
                                        throw new ApplicationException(string.Format("Expected NumericUpDown control for '{0}'",
                                            prop.DisplayName));
                                    return entryControl.Value;

                                }
                        }
                        break;
                    }

                case SelectionTypes.List:
                case SelectionTypes.Lookup:
                    {
                        ComboBox entryControl = control as ComboBox;
                        if (entryControl == null)
                            throw new ApplicationException(string.Format("Expected ComboBox control for '{0}'",
                                prop.DisplayName));
                        return entryControl.SelectedValue;
                    }


            }

            TextBox textBox = control as TextBox;
            if (textBox == null)
            {
                throw new ApplicationException(string.Format("Expected TextBox control for '{0}'",
                                         prop.DisplayName));
            }
            return textBox.Text;
        }



        private Dictionary<string, ICollection<PropertyValuePairValidationResponse>> ValidateFeaturePropertyValuePairs(Dictionary<string, IDictionary<string, object>> validateFeaturePropValuePairs)
        {
            var cfg = new DomainMediatorJsonSerializerSettings();
            cfg.ContractResolver = new DictionaryFriendlyContractResolver();
            cfg.Converters.Add(new KeyValuePairConverter());

            string validateFeatureConfigPayload = JsonConvert.SerializeObject(validateFeaturePropValuePairs, cfg);

            var featureResponse = MakeFeatureRequest("Validating feature configuration entry values",
            FeatureMessageTypes.ValidateCompanyFeaturePropertyValuePairs, validateFeatureConfigPayload);

            var response = JsonConvert.DeserializeObject<IList<KeyValuePair<String, ICollection<PropertyValuePairValidationResponse>>>>(featureResponse.Payload, cfg);
            if (response == null)
                throw new ApplicationException("Invalid response returned from validation");

            return response.ToDictionary(x => x.Key, y => y.Value);
        }




        private Response SaveFeaturePropertyValuePairs(Dictionary<string, IDictionary<string, object>> savePropValuePairs)
        {
            var cfg = new DomainMediatorJsonSerializerSettings();
            cfg.ContractResolver = new DictionaryFriendlyContractResolver();
            cfg.Converters.Add(new KeyValuePairConverter());

            string saveFeatureConfigPayload = JsonConvert.SerializeObject(savePropValuePairs, cfg);

            var featureResponse = MakeFeatureRequest("Saving feature configuration entry values",
            FeatureMessageTypes.SaveCompanyFeaturePropertyValuePairs, saveFeatureConfigPayload);

            var response = JsonConvert.DeserializeObject<Response>(featureResponse.Payload, cfg);
            if (response == null)
                throw new ApplicationException("Invalid response returned from save feature entry values");

            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="featureId"></param>
        /// <param name="payload"></param>
        /// <returns></returns>
        private FeatureResponse MakeFeatureRequest(string caption, string featureId, string payload)
        {
            using (var serviceWorker = new ServiceWorkerWithProgress(this))
            {
                FeatureResponse featureResponse = serviceWorker.FeatureRequest(caption,
                                                                               _configuration.ConnectorPluginId,
                                                                               _configuration.BackOfficeConnectionCredentials,
                                                                               featureId,
                                                                               _configuration.CloudTenantId,
                                                                               payload);
                return featureResponse;
            }
        }

        private class FeaturePropControl
        {
            public String FeatureName { get; set; }
            public String PropertyName { get; set; }

            public Control EntryControl { get; set; }
        }

        private void CloudConnectorFeatureConfigurationsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                e.Cancel = !_canClose;
            }
            finally
            {
                _canClose = true;
            }
        }
    }
}
