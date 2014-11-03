using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;
using Sage.Connector.Configuration.Contracts.Data;
using Sage.Connector.Configuration.Contracts.Data.DataTypes;
using Sage.Connector.Configuration.Contracts.Data.Responses;
using Sage.Connector.Configuration.Contracts.Data.SelectionValueTypes;
using Sage.Connector.DomainContracts.Responses;
using SageConnect.ViewModels;
using Xceed.Wpf.Toolkit;


namespace SageConnect.Control
{
    /// <summary>
    /// View model for the user control
    /// </summary>
    public class TransistionControlViewModel : INotifyPropertyChanged
    {

        /// <summary>
        /// Default constructor - required for xaml code for datacontext reference
        /// </summary>
        public TransistionControlViewModel()
        {
        }


        /// <summary>
        /// 
        /// </summary>
        public TransistionControlViewModel(Controlcollection controlcollection, String headerText,
            CommandTypes commandTypes, string selectedconnection, bool presistcontrols, bool reset, bool readOnly)
        {
            if (reset)
            {
                _removecontrols = null;
                _controlcollection = null;
            }
            _selectedConnection = selectedconnection;
            Intilizecontrol(controlcollection, headerText,
            commandTypes, presistcontrols, readOnly);
        }

        private static Controlcollection _controlcollection;

        private static List<PropertyControl> _removecontrols;

        private static Grid _dynamicGrid;


        /// <summary>
        ///  Command click events
        /// </summary>
        public event EventHandler CommandoneclickEvent;

        /// <summary>
        ///  Command click events
        /// </summary>
        public event EventHandler CommandtwoclickEvent;


        /// <summary>
        ///  Command Selected Events
        /// </summary>
        public event EventHandler CommandSelectedEvent;

        /// <summary>
        ///  Command Event for company Selected 
        /// </summary>
        public event EventHandler CommandCompanySelectedEvent;

        /// <summary>
        ///  Constructor for the view model 
        /// </summary>
        public void Intilizecontrol(Controlcollection controlcollection, String headerText,
            CommandTypes commandTypes, bool presistcontrols, bool readOnly)
        {
            if (_controlcollection == null)
            {
                _controlcollection = controlcollection;

            }
            else
            {
                if (_removecontrols != null)
                {
                    RemoveControls(controlcollection);
                }
                if (!presistcontrols)
                    _removecontrols = controlcollection;

                _controlcollection.AddRange(controlcollection);
            }


            ControlHeaderText = headerText;
            CommandDisplay = commandTypes;
            DrawControl(readOnly);
            Displaycontent();
        }

        private void RemoveControls(Controlcollection newcontrols)
        {
            foreach (PropertyControl prop in newcontrols)
            {
                if (_removecontrols.Exists(x => x.DisplayName.Contains(prop.DisplayName)))
                    _controlcollection.RemoveAt(_controlcollection.FindIndex(x => x.DisplayName.Contains(prop.DisplayName)));
            }
        }

        /// <summary>
        ///  draws the controls sent thru the property bag
        /// </summary>
        private void DrawControl(bool readOnly)
        {
            _dynamicGrid = new Grid();
            NameScope.SetNameScope(_dynamicGrid, new NameScope());
            int controlscount = _controlcollection.Count;
            _dynamicGrid.Width = 350;
            _dynamicGrid.HorizontalAlignment = HorizontalAlignment.Left;
            _dynamicGrid.VerticalAlignment = VerticalAlignment.Top;


            // Create Columns
            ColumnDefinition gridCol1 = new ColumnDefinition();
            ColumnDefinition gridCol2 = new ColumnDefinition();
            gridCol1.Width = new GridLength(_dynamicGrid.Width * (40.0 / 100.0));
            gridCol2.Width = new GridLength(_dynamicGrid.Width * (60.0 / 100.0));

            _dynamicGrid.ColumnDefinitions.Add(gridCol1);
            _dynamicGrid.ColumnDefinitions.Add(gridCol2);

            RowDefinition gridRow1 = new RowDefinition { Height = new GridLength(36) };
            _dynamicGrid.RowDefinitions.Add(gridRow1);


            for (int rowcount = 1; rowcount <= controlscount + 2; rowcount++)
            {
                RowDefinition gridRow = new RowDefinition { Height = new GridLength(26) };
                _dynamicGrid.RowDefinitions.Add(gridRow);
            }

            //// Add first column header
            var settingsHeader = new SettingsControl
            {

                ShowHelpSettings = Visibility.Visible,
                ShowSettings = Visibility.Hidden,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                HeaderDescription = ControlHeaderText,
                MessageDescription = CustomerFacingMessages.Settings_ConnectionsMessage,
                Margin = new Thickness(0),

            };
            settingsHeader.HelpSettingsClick += ConnectionHelpClick;



            Grid.SetRow(settingsHeader, 0);
            Grid.SetColumn(settingsHeader, 0);
            Grid.SetRowSpan(settingsHeader, 3);
            Grid.SetColumnSpan(settingsHeader, 2);


            _dynamicGrid.Children.Add(settingsHeader);

            int row = 2;



            foreach (PropertyControl control in _controlcollection)
            {
                UIElement dynamicCaption = CreateCaptionUiElement(control);
                Grid.SetRow(dynamicCaption, row);
                Grid.SetColumn(dynamicCaption, 0);
                UIElement dynamicUiElement = CreateEntryControl(control);
                if (!control.IsDisabled) dynamicUiElement.IsEnabled = !readOnly;

                Grid.SetRow(dynamicUiElement, row);
                Grid.SetColumn(dynamicUiElement, 1);
                if (control.SelectionType == SelectTypes.List)
                {
                    Grid.SetRowSpan(dynamicUiElement, 2);
                    _dynamicGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(26) });
                    row++;
                }

                _dynamicGrid.Children.Add(dynamicCaption);
                _dynamicGrid.Children.Add(dynamicUiElement);
                control.Element = dynamicUiElement;

                row++;
            }

            DisplayCommand(_dynamicGrid, false);
            Content = _dynamicGrid;
        }


        /// <summary>
        /// To set the Connection Controls Readonly
        /// </summary>
        /// <param name="readOnly"></param>
        public void SetConnectionReadOnly(bool readOnly)
        {
            ConnectionControlReadOnly(readOnly);
            FeatureControlReadOnly(readOnly);
            CommandReadOnly(readOnly);
        }
        private static void ConnectionHelpClick(object sender, RoutedEventArgs e)
        {
            SettingsControl control = (SettingsControl)sender;
            Process.Start(control.Name == "FeatureConfiguration"
                ? HelpLinkManager.FindString("Settings_Helpfile_English")
                : HelpLinkManager.FindString("Features_Helpfile_English"));
        }


        /// <summary>
        /// Display the command required by the user for control
        /// </summary>
        /// <param name="dynamicGrid"></param>
        /// <param name="readOnly"></param>
        private void DisplayCommand(Grid dynamicGrid, bool readOnly)
        {
            switch (CommandDisplay)
            {
                case CommandTypes.OkCancel:
                    {
                        CreateTwinCommands(dynamicGrid, CustomerFacingMessages.ButtonCaption_Ok, CustomerFacingMessages.ButtonCaption_Cancel, false, readOnly);
                        break;
                    }
                case CommandTypes.SubmitOnly:
                    {
                        CreateTwinCommands(dynamicGrid, CustomerFacingMessages.ButtonCaption_Submit, "", true, readOnly);
                        break;
                    }
                case CommandTypes.CancelOnly:
                    {
                        CreateTwinCommands(dynamicGrid, CustomerFacingMessages.ButtonCaption_Cancel, "", true, readOnly);
                        break;
                    }
                case CommandTypes.ValidateCancel:
                    {
                        CreateTwinCommands(dynamicGrid, CustomerFacingMessages.ButtonCaption_Validate, CustomerFacingMessages.ButtonCaption_Cancel, false, readOnly);
                        break;
                    }
                case CommandTypes.ValidateOnly:
                    {
                        CreateTwinCommands(dynamicGrid, CustomerFacingMessages.ButtonCaption_Validate, "", true, readOnly);
                        break;
                    }
                case CommandTypes.RegisterCancel:
                    {
                        CreateTwinCommands(dynamicGrid, CustomerFacingMessages.ButtonCaption_Register, CustomerFacingMessages.ButtonCaption_Cancel, false, readOnly);
                        break;
                    }
                case CommandTypes.RegisterOnly:
                    {
                        CreateTwinCommands(dynamicGrid, CustomerFacingMessages.ButtonCaption_Register, "", true, readOnly);
                        break;
                    }
                case CommandTypes.SaveOnly:
                    {
                        CreateTwinCommands(dynamicGrid, CustomerFacingMessages.ButtonCaption_Save, "", true, readOnly);
                        break;
                    }
                case CommandTypes.None:
                    {
                        break;
                    }
            }
        }

        /// <summary>
        /// creating the command to display in the control
        /// </summary>
        /// <param name="dynamicGrid"></param>
        /// <param name="captionone"></param>
        /// <param name="captiontwo"></param>
        /// <param name="singlecommand"></param>
        /// <param name="readOnly"></param>
        private void CreateTwinCommands(Grid dynamicGrid, String captionone, String captiontwo, bool singlecommand, bool readOnly)
        {
            var commandone = new Button
            {
                Content = captionone,
                HorizontalAlignment = singlecommand ? HorizontalAlignment.Right : HorizontalAlignment.Left,
                Margin = new Thickness(2),
                Background = (!readOnly ? (SolidColorBrush)new BrushConverter().ConvertFrom("#41a940") : (SolidColorBrush)new BrushConverter().ConvertFrom("#9a9a9b")),
                Visibility = (readOnly ? Visibility.Hidden : Visibility.Visible),
                Foreground = new SolidColorBrush(Colors.White),
                BorderThickness = new Thickness(0),
                IsEnabled = !readOnly,
                Width = 80,
                Name = "commandone"
            };

            Grid.SetRow(commandone, dynamicGrid.RowDefinitions.Count);
            Grid.SetColumn(commandone, 1);
            dynamicGrid.Children.Add(commandone);
            dynamicGrid.RegisterName(commandone.Name, commandone);


            commandone.Click += CommandoneClick;
            if (!singlecommand)
            {
                var commandtwo = new Button
                {
                    Content = captiontwo,
                    Width = 80,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Background = (!readOnly ? (SolidColorBrush)new BrushConverter().ConvertFrom("#41a940") : (SolidColorBrush)new BrushConverter().ConvertFrom("#9a9a9b")),
                    Visibility = (readOnly ? Visibility.Hidden : Visibility.Visible),
                    Foreground = new SolidColorBrush(Colors.White),
                    BorderThickness = new Thickness(0),
                    IsEnabled = !readOnly,
                    Name = "commandtwo"
                };

                Grid.SetRow(commandtwo, dynamicGrid.RowDefinitions.Count);
                Grid.SetColumn(commandtwo, 1);
                dynamicGrid.Children.Add(commandtwo);
                dynamicGrid.RegisterName(commandtwo.Name, commandtwo);
                commandtwo.Click += CommandtwoClick;
            }

        }


        private void CommandReadOnly(bool readOnly)
        {
            Button element = (Button)_dynamicGrid.Children.OfType<FrameworkElement>()
                .FirstOrDefault(e => e.Name == "commandone");
            if (element != null)
            {
                element.IsEnabled = !readOnly;
                element.Visibility = (readOnly ? Visibility.Hidden : Visibility.Visible);
                element.Background = (!readOnly
                    ? (SolidColorBrush)new BrushConverter().ConvertFrom("#41a940")
                    : (SolidColorBrush)new BrushConverter().ConvertFrom("#9a9a9b"));
            }
            element = (Button)_dynamicGrid.Children.OfType<FrameworkElement>()
                  .FirstOrDefault(e => e.Name == "commandtwo");
            if (element != null)
            {
                element.IsEnabled = !readOnly;
                element.Visibility = (readOnly ? Visibility.Hidden : Visibility.Visible);
                element.Background = (!readOnly
                   ? (SolidColorBrush)new BrushConverter().ConvertFrom("#41a940")
                   : (SolidColorBrush)new BrushConverter().ConvertFrom("#9a9a9b"));
            }

        }
        private bool RemoveElement(string elementname)
        {
            var element = _dynamicGrid.Children.OfType<FrameworkElement>()
                .FirstOrDefault(e => e.Name == elementname);
            _dynamicGrid.Children.Remove(element);
            if (element != null)
            {
                _dynamicGrid.UnregisterName(elementname);
                return true;
            }
            return false;
        }
        private void RemoveCommands()
        {
            var element = _dynamicGrid.Children.OfType<FrameworkElement>()
                .FirstOrDefault(e => e.Name == "commandone");
            _dynamicGrid.Children.Remove(element);
            if (element != null) _dynamicGrid.UnregisterName("commandone");
            element = _dynamicGrid.Children.OfType<FrameworkElement>()
                  .FirstOrDefault(e => e.Name == "commandtwo");
            _dynamicGrid.Children.Remove(element);
            if (element != null) _dynamicGrid.UnregisterName("commandtwo");

        }
        private void CommandoneClick(object sender, RoutedEventArgs e)
        {
            if (CommandoneclickEvent != null)
            {
                Selectedvalues();
                if (_featurePropertyLists != null) Getsavevalue();
                CommandoneclickEvent(this, new EventArgs());
            }
        }

        private void CommandtwoClick(object sender, RoutedEventArgs e)
        {
            if (CommandtwoclickEvent != null)
            {
                CommandtwoclickEvent(this, new EventArgs());
            }
        }

        private void CommandSelected(object sender, RoutedEventArgs e)
        {
            if (CommandSelectedEvent != null)
            {
                Selectedvalues();
                PropertyControl propControl = _controlcollection.ElementAtOrDefault(0);
                _controlcollection.Clear();
                if (_removecontrols != null) _removecontrols.Clear();
                _controlcollection.Add(propControl);
                CommandSelectedEvent(this, new EventArgs());
            }
        }

        private void CommandCompanySelected(object sender, RoutedEventArgs e)
        {

            Selectedvalues();
            CommandCompanySelectedEvent(this, new EventArgs());

        }
        /// <summary>
        /// 
        /// </summary>
        public void Displaycontent()
        {
            ControlContent = Content;
        }

        /// <summary>
        /// The property to set teh header for the control
        /// </summary>
        public string ControlHeaderText { get; set; }

        /// <summary>
        /// The property to set or get the command types for the control header
        /// </summary>
        public CommandTypes CommandDisplay { get; set; }

        private Grid _controlContent;
        /// <summary>
        /// to get the content control to set the content
        /// </summary>
        //
        public Grid ControlContent
        {
            get { return _controlContent; }
            set
            {
                _controlContent = value;
                // ReSharper disable once ExplicitCallerInfoArgument
                OnPropertyChanged("Content");
            }
        }

        private UIElement CreateCaptionUiElement(PropertyControl control)
        {
            TextBlock txt = new TextBlock
            {
                Text = control.DisplayName,
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Colors.Black),
                VerticalAlignment = VerticalAlignment.Top,
                TextTrimming = TextTrimming.CharacterEllipsis
            };
            return txt;
        }
        private UIElement CreateCaptionUiElement(PropertyDefinition control)
        {
            TextBlock txt = new TextBlock
            {
                Text = control.DisplayName,
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Colors.Black),
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(10, 0, 0, 0),
                TextTrimming = TextTrimming.CharacterEllipsis
            };
            return txt;
        }
        private UIElement CreateEntryControl(PropertyControl control)
        {


            switch (control.SelectionType)
            {

                case SelectTypes.None:
                    switch (control.DataType)
                    {
                        case DataTypeEnum.BooleanType:
                            {
                                var boolControl = new CheckBox { IsEnabled = !control.IsDisabled, Margin = new Thickness(0, 0, 0, 1), BorderThickness = new Thickness(1) };

                                return boolControl;
                            }
                        case DataTypeEnum.DateTimeType:
                            {
                                return new DatePicker { IsEnabled = !control.IsDisabled, Margin = new Thickness(0, 0, 0, 1), BorderThickness = new Thickness(1) };
                            }
                        case DataTypeEnum.StringType:
                            {
                                if (control.IsPassword)
                                {
                                    PasswordBox textBox = new PasswordBox { IsEnabled = !control.IsDisabled, Margin = new Thickness(0, 0, 0, 1), BorderThickness = new Thickness(1),BorderBrush= Brushes.Gray };
                                    //var dataType = control.DataType as StringType;
                                    //if (dataType != null && dataType.MaxLength > 0)
                                    //{
                                    //    textBox.MaxLength = dataType.MaxLength;
                                    //    textBox.Width = dataType.MaxLength * 20;
                                    //}
                                    if (control.DisplayValue != string.Empty) textBox.Password = control.DisplayValue;

                                    return textBox;
                                }
                                if (control.IsPath)
                                {
                                    var textBox = new TextBox { Margin = new Thickness(0, 0, 0, 1), BorderThickness = new Thickness(2) };



                                    if (control.DisplayValue != string.Empty) textBox.Text = control.DisplayValue;
                                    textBox.Width = (_dynamicGrid.Width * (60.0 / 100.0)) - 25;
                                    var button = new Button
                                    {
                                        Width = 25,
                                        Content = "...",
                                        HorizontalAlignment = HorizontalAlignment.Right,
                                        
                                    };
                                    StackPanel panel = new StackPanel
                                    {
                                        IsEnabled = !control.IsDisabled,
                                        Orientation = Orientation.Horizontal
                                    };
                                    panel.Children.Add(textBox);
                                    panel.Children.Add(button);
                                    button.Tag = textBox;

                                    button.Click += FiledialogOpen;
                                    return panel;

                                }
                                else
                                {
                                    var textBox = new TextBox { IsEnabled = !control.IsDisabled, Margin = new Thickness(0, 0, 0, 1), BorderThickness = new Thickness(1), BorderBrush = Brushes.Gray };
                                    //var dataType = control.DataType as StringType;
                                    //if (dataType != null && dataType.MaxLength > 0)
                                    //{
                                    //    textBox.MaxLength = dataType.MaxLength;
                                    //    textBox.Width = dataType.MaxLength * 20;
                                    //}

                                    if (control.DisplayValue != string.Empty) textBox.Text = control.DisplayValue;
                                    return textBox;
                                }
                            }
                        case DataTypeEnum.DecimalType:
                            {
                                var numericUpDown = new DecimalUpDown
                                {
                                    Maximum = Decimal.MaxValue,
                                    FormatString = "F2",
                                    IsEnabled = !control.IsDisabled,
                                    Margin = new Thickness(0, 0, 0, 1),
                                    BorderThickness = new Thickness(1)
                                };

                                return numericUpDown;
                            }

                        case DataTypeEnum.IntegerType:
                            {

                                var numericUpDown = new IntegerUpDown
                                {
                                    Maximum = Int32.MaxValue,
                                    FormatString = "N",
                                    IsEnabled = !control.IsDisabled,
                                    Margin = new Thickness(0, 0, 0, 1),
                                    BorderThickness = new Thickness(1)
                                };

                                return numericUpDown;
                            }
                    }
                    break;


                case SelectTypes.List:
                    {
                        // var listTypeValues = propTypeValues as ListTypeValues ?? new ListTypeValues();
                        //_dataObjects.Add(listTypeValues.ListValues);
                        var comboBox = new ListBox
                        {

                            //DropDownStyle = ComboBoxStyle.DropDownList,
                            //  ItemsSource = new BindingSource(listTypeValues.ListValues, null),

                            SelectedValuePath = "Value",
                            DisplayMemberPath = "Value",
                            Width = (_dynamicGrid.Width * (60.0 / 100.0)),
                            // Height = _dynamicGrid.RowDefinitions.First().Height.Value * 2,
                            IsEnabled = !control.IsDisabled,
                            IsTextSearchEnabled = true,
                            Margin = new Thickness(0, 0, 0, 1)
                        };
                        if (control.DisplayValue != string.Empty) comboBox.SelectedValue = control.DisplayValue;

                        return comboBox;
                    }
                case SelectTypes.Lookup:
                    {

                        var comboBox = new ComboBox
                        {
                            ItemsSource = control.ItemDataDictionary,
                            SelectedValuePath = "Key",
                            DisplayMemberPath = "Value",
                            Width = (_dynamicGrid.Width * (60.0 / 100.0)),
                            IsTextSearchEnabled = true,
                            IsEditable = true,
                            IsEnabled = !control.IsDisabled,
                            SelectedValue = (control.SelectedValue != string.Empty ? control.SelectedValue : string.Empty),
                            Margin = new Thickness(0, 0, 0, 1)

                        };

                        comboBox.Margin = new Thickness(0, 0, 0, 1);

                        if (control.CommandAction != null && !(control.Name.Equals("companyid", StringComparison.InvariantCultureIgnoreCase) || control.Name.Equals("companycode", StringComparison.InvariantCultureIgnoreCase)))
                        {
                            CommandSelectedEvent += control.CommandAction;
                            comboBox.SelectionChanged += CommandSelected;
                        }
                        else if (control.CommandAction != null && (control.Name.Equals("companyid", StringComparison.InvariantCultureIgnoreCase) || control.Name.Equals("companycode", StringComparison.InvariantCultureIgnoreCase)))
                        {
                            CommandCompanySelectedEvent += control.CommandAction;
                            comboBox.SelectionChanged += CommandCompanySelected;
                        }


                        return comboBox;
                    }


            }

            //not corresponding control for property definition
            var entryControl = new TextBox { Width = 400 };
            return entryControl;
        }

        private void FiledialogOpen(object sender, RoutedEventArgs e)
        {
            Button s = sender as Button;
            if (s != null)
            {
                TextBox target = s.Tag as TextBox;
                if (target != null)
                {

                    var dialog = new OpenFileDialog { InitialDirectory = target.Text };
                    var result = dialog.ShowDialog();
                    if (result == true)
                    {
                        target.Text = dialog.FileName;
                    }

                }
            }
        }

        private void ConnectionControlReadOnly(bool readOnly)
        {
            ControlValues = new Dictionary<string, string>();
            foreach (var propControl in _controlcollection)
            {
                propControl.Element.IsEnabled = !readOnly;
            }

        }

        private void Selectedvalues()
        {
            ControlValues = new Dictionary<string, string>();
            foreach (var propControl in _controlcollection)
            {
                ControlValues.Add(propControl.Name, GetEntryControlValue(propControl, propControl.Element));
            }

        }

        private string GetEntryControlValue(PropertyControl prop, UIElement control)
        {
            switch (prop.SelectionType)
            {
                case SelectTypes.None:
                    {
                        switch (prop.DataType)
                        {
                            case DataTypeEnum.BooleanType:
                                {
                                    CheckBox entryControl = control as CheckBox;
                                    if (entryControl == null)
                                        throw new ApplicationException(string.Format("Expected CheckBox control for '{0}'",
                                            prop.DisplayName));
                                    prop.DisplayValue = prop.SelectedValue = entryControl.IsChecked.ToString();
                                    return entryControl.IsChecked.ToString();

                                }
                            case DataTypeEnum.DateTimeType:
                                {
                                    DatePicker entryControl = control as DatePicker;
                                    if (entryControl == null)
                                        throw new ApplicationException(string.Format(
                                            "Expected DateTimePicker control for '{0}'",
                                            prop.DisplayName));
                                    prop.DisplayValue = prop.SelectedValue = entryControl.SelectedDate.ToString();
                                    return entryControl.SelectedDate.ToString();

                                }
                            case DataTypeEnum.StringType:
                                {
                                    if (prop.IsPassword)
                                    {
                                        PasswordBox entryControl = control as PasswordBox;
                                        if (entryControl == null)
                                            throw new ApplicationException(
                                                string.Format("Expected Password control for '{0}'",
                                                    prop.DisplayName));
                                        prop.DisplayValue = prop.SelectedValue = entryControl.Password;
                                        return entryControl.Password;
                                    }
                                    if (prop.IsPath)
                                    {
                                        StackPanel entryControl = control as StackPanel;
                                        if (entryControl == null)
                                            throw new ApplicationException(
                                                string.Format("Expected FolderPicker control for '{0}'",
                                                    prop.DisplayName));
                                        TextBox entryControl1 = entryControl.Children[0] as TextBox;
                                        if (entryControl1 == null)
                                            throw new ApplicationException(
                                                string.Format("Expected Password control for '{0}'",
                                                    prop.DisplayName));
                                        prop.DisplayValue = prop.SelectedValue = entryControl1.Text;
                                        return entryControl1.Text;
                                    }
                                    else
                                    {
                                        TextBox entryControl = control as TextBox;
                                        if (entryControl == null)
                                            throw new ApplicationException(
                                                string.Format("Expected TextBox control for '{0}'",
                                                    prop.DisplayName));
                                        prop.DisplayValue = prop.SelectedValue = entryControl.Text;
                                        return entryControl.Text;
                                    }

                                }
                            case DataTypeEnum.IntegerType:
                                {
                                    IntegerUpDown entryControl = control as IntegerUpDown;
                                    if (entryControl == null)
                                        throw new ApplicationException(string.Format("Expected NumericUpDown control for '{0}'",
                                            prop.DisplayName));
                                    prop.DisplayValue = prop.SelectedValue = entryControl.Value.ToString();
                                    return entryControl.Value.ToString();

                                }
                            case DataTypeEnum.DecimalType:
                                {
                                    DecimalUpDown entryControl = control as DecimalUpDown;
                                    if (entryControl == null)
                                        throw new ApplicationException(string.Format("Expected NumericUpDown control for '{0}'",
                                            prop.DisplayName));
                                    prop.DisplayValue = prop.SelectedValue = entryControl.Value.ToString();
                                    return entryControl.Value.ToString();

                                }
                        }
                        break;
                    }

                case SelectTypes.List:
                case SelectTypes.Lookup:
                    {
                        ComboBox entryControl = control as ComboBox;
                        if (entryControl == null)
                            throw new ApplicationException(string.Format("Expected ComboBox control for '{0}'",
                                prop.DisplayName));
                        //return entryControl.SelectedValue.ToString();
                        if (entryControl.SelectedValue != null)
                        {


                            KeyValuePair<string, string> selectedPair = (KeyValuePair<string, string>)entryControl.SelectedItem;

                            prop.SelectedValue = selectedPair.Key;
                            prop.DisplayValue = selectedPair.Value;

                            //prop.SelectedValue = entryControl.SelectedValue.ToString();
                            //prop.DisplayValue =  ;
                        }
                        return prop.SelectedValue;
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





        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> ControlValues { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, IDictionary<String, Object>> FeatureSaveValues { get; set; }
        /// <summary>
        /// Property Changed event derived from the List.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;


        ///  <summary>
        /// Create the OnPropertyChanged method to raise the event  
        ///  </summary>
        /// <param name="propertyName"></param>
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// 
        /// </summary>
        public Grid Content
        {
            get { return _controlContent; }
            set
            {
                _controlContent = value;
                OnPropertyChanged();
            }
        }

        #region featureConfiguration
        private readonly ICollection<Object> _dataObjects = new Collection<object>();
        private IDictionary<FeatureMetadata, IList<PropertyDefinition>> _featurePropertyLists;

        private IDictionary<string, Dictionary<String, AbstractSelectionValueTypes>> _featurePropertyEntryValues;
        private readonly IList<FeaturePropControl> _featurePropControls = new List<FeaturePropControl>();
        private readonly string _selectedConnection;

        private void DrawFeatureControl(
            IDictionary<string, Dictionary<String, AbstractSelectionValueTypes>> featurePropertyEntryValues,
            IDictionary<String, Dictionary<string, object>> featurePropertyValuePairs, bool readOnly)
        {
            if (_featurePropertyLists == null || !_featurePropertyLists.Any())
                return;

            if (RemoveElement("FeatureConfigurationsettings"))
                _dynamicGrid.RowDefinitions.RemoveRange(_dynamicGrid.RowDefinitions.Count - 3, 3);
            RemoveElement("FeatureScrollbar");
            RemoveElement("FeatureGrid");
            _featurePropControls.Clear();


            _featurePropertyEntryValues = featurePropertyEntryValues;
            var currentFeaturePropValuePairs = featurePropertyValuePairs;



            _dynamicGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(26) });

            var featuresettingsHeader = new SettingsControl
            {
                Name = "FeatureConfigurationsettings",
                ShowHelpSettings = Visibility.Visible,
                ShowSettings = Visibility.Hidden,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                HeaderDescription = CustomerFacingMessages.Settings_FeatureHeader,
                MessageDescription = CustomerFacingMessages.Settings_FeatureMessage,
                Margin = new Thickness(0),
            };
            featuresettingsHeader.HelpSettingsClick += ConnectionHelpClick;
            _dynamicGrid.RowDefinitions[_dynamicGrid.RowDefinitions.Count - 2].Height = new GridLength(46);
            Grid.SetRow(featuresettingsHeader, _dynamicGrid.RowDefinitions.Count - 2);
            Grid.SetColumn(featuresettingsHeader, 0);
            Grid.SetRowSpan(featuresettingsHeader, 3);
            Grid.SetColumnSpan(featuresettingsHeader, 2);

            _dynamicGrid.RegisterName(featuresettingsHeader.Name, "FeatureConfigurationsettings");


            _dynamicGrid.Children.Add(featuresettingsHeader);

            ScrollViewer scroll = new ScrollViewer
            {
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                BorderBrush = new SolidColorBrush(Colors.Gray),
                BorderThickness = new Thickness(1),
                Name = "FeatureScrollbar"
            };
            Grid featureGrid = new Grid
            {
                Name = "FeatureGrid",
                Background = Brushes.White
            };

            featureGrid.ColumnDefinitions.Add(new ColumnDefinition());
            featureGrid.ColumnDefinitions.Add(new ColumnDefinition());
            featureGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(26) });

            _dynamicGrid.RegisterName(scroll.Name, "FeatureScrollbar");
            _dynamicGrid.RegisterName(featureGrid.Name, "FeatureGrid");


            int propRow = 0;
            foreach (var featurePropertyList in _featurePropertyLists)
            {



                IDictionary<String, AbstractSelectionValueTypes> featurePropertiesEntryValues =
                    new Dictionary<string, AbstractSelectionValueTypes>();
                TextBlock headerTextBlock = new TextBlock
               {
                   Text = featurePropertyList.Key.DisplayName,
                   FontSize = 12,
                   FontFamily = new FontFamily("Arial"),
                   FontWeight = FontWeights.Regular,
                   Foreground = new SolidColorBrush(Colors.Gray),
                   VerticalAlignment = VerticalAlignment.Top
               };
                RowDefinition gridHeadRow = new RowDefinition { Height = new GridLength(26) };
                featureGrid.RowDefinitions.Add(gridHeadRow);
                Grid.SetRow(headerTextBlock, propRow);
                Grid.SetColumn(headerTextBlock, 0);
                Grid.SetColumnSpan(headerTextBlock, 2);
                featureGrid.Children.Add(headerTextBlock);
                propRow++;
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



                    AbstractSelectionValueTypes propEntryValues =
                        featurePropertiesEntryValues.ContainsKey(prop.PropertyName)
                            ? featurePropertiesEntryValues[prop.PropertyName]
                            : null;

                    // 
                    // propEntry
                    // 
                    RowDefinition gridRow = new RowDefinition { Height = new GridLength(26) };
                    featureGrid.RowDefinitions.Add(gridRow);
                    //UIElement propEntry = CreateFeatureControl(prop, propEntryValues);

                    UIElement dynamicCaption = CreateCaptionUiElement(prop);
                    Grid.SetRow(dynamicCaption, propRow);
                    Grid.SetColumn(dynamicCaption, 0);
                    UIElement propEntry = CreateFeatureControl(prop, propEntryValues);
                    propEntry.IsEnabled = !readOnly;
                    Grid.SetRow(propEntry, propRow);
                    Grid.SetColumn(propEntry, 1);
                    if (propEntry is ListBox)
                    {
                        Grid.SetRowSpan(propEntry, 2);
                        featureGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(26) });
                        propRow++;
                    }
                    featureGrid.Children.Add(dynamicCaption);
                    featureGrid.Children.Add(propEntry);

                    SetFeatureControlValue(propEntry, prop, propEntryValues,
                        currPropValuePairs.ContainsKey(prop.PropertyName) ? currPropValuePairs[prop.PropertyName] : null);


                    // Set up the ToolTip text for the entry control.
                    //toolTip1.SetToolTip(propEntry, prop.Description);


                    _featurePropControls.Add(new FeaturePropControl
                    {
                        EntryControl = propEntry,
                        FeatureName = featurePropertyList.Key.Name,
                        PropertyName = prop.PropertyName
                    });
                    propRow++;
                }



                //featureRow++;
            }
            scroll.Content = featureGrid;
            RowDefinition scrollRow = new RowDefinition { Height = new GridLength(180) };
            _dynamicGrid.RowDefinitions.Add(scrollRow);

            Grid.SetRow(scroll, _dynamicGrid.RowDefinitions.Count - 1);
            Grid.SetColumn(scroll, 0);
            Grid.SetRowSpan(scroll, 1);
            Grid.SetColumnSpan(scroll, 2);
            _dynamicGrid.Children.Add(scroll);
            _dynamicGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(36) });
        }


        private UIElement CreateFeatureControl(PropertyDefinition prop, AbstractSelectionValueTypes propTypeValues)
        {
            switch (prop.PropertyDataType.SelectionType)
            {
                case SelectionTypes.None:
                    {
                        switch (prop.PropertyDataType.DataTypesEnum)
                        {
                            case DataTypesEnum.BooleanType:
                                {
                                    var boolControl = new CheckBox { Content = "", Margin = new Thickness(0, 0, 0, 1), BorderThickness = new Thickness(1) };

                                    return boolControl;
                                }
                            case DataTypesEnum.DateTimeType:
                                {
                                    return new DatePicker { Margin = new Thickness(0, 0, 0, 1), BorderThickness = new Thickness(1) };
                                }
                            case DataTypesEnum.StringType:
                                {
                                    var textBox = new TextBox { Margin = new Thickness(0, 0, 0, 1), BorderThickness = new Thickness(1) };
                                    var dataType = prop.PropertyDataType as StringType;
                                    if (dataType != null && dataType.MaxLength > 0)
                                    {
                                        textBox.MaxLength = dataType.MaxLength;
                                    }
                                    textBox.IsReadOnly = false;
                                    return textBox;
                                }
                            case DataTypesEnum.DecimalType:
                                {
                                    var numericUpDown = new DecimalUpDown
                                    {
                                        Maximum = Decimal.MaxValue,
                                        FormatString = "F2",
                                        Margin = new Thickness(0, 0, 0, 1),
                                        BorderThickness = new Thickness(1),
                                        BorderBrush = Brushes.DarkGray 
                                    };

                                    return numericUpDown;
                                }

                            case DataTypesEnum.IntegerType:
                                {
                                    var numericUpDown = new IntegerUpDown
                                    {
                                        Maximum = Int32.MaxValue,
                                        FormatString = "N0",
                                        Margin = new Thickness(0, 0, 0, 1),
                                        BorderThickness = new Thickness(1),
                                        BorderBrush = Brushes.DarkGray 
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
                        var comboBox = new ListBox
                        {

                            ItemsSource = listTypeValues.ListValues,
                            // Height = _dynamicGrid.RowDefinitions.First().Height.Value * 2,                   
                            //SelectedValuePath = "Value",
                            //DisplayMemberPath = "Value",
                            IsTextSearchEnabled = true,
                            Margin = new Thickness(0, 0, 0, 1),
                            BorderThickness = new Thickness(1),

                        };
                        return comboBox;

                    }
                case SelectionTypes.Lookup:
                    {
                        var lookupTypeValues = propTypeValues as LookupTypeValues ?? new LookupTypeValues();
                        _dataObjects.Add(lookupTypeValues.LookupValues);
                        var comboBox = new ComboBox
                        {
                            ItemsSource = lookupTypeValues.LookupValues,
                            SelectedValuePath = "Key",
                            DisplayMemberPath = "Value",
                            IsEditable = true,
                            Margin = new Thickness(0, 0, 0, 1),
                            BorderThickness = new Thickness(1)
                        };
                        return comboBox;
                    }

            }

            //not corresponding control for property definition
            var entryControl = new TextBox { Width = 400 };
            return entryControl;
        }

        private object GetFeatureControlValue(PropertyDefinition prop, UIElement control)
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
                                    return entryControl.IsChecked;

                                }
                            case DataTypesEnum.DateTimeType:
                                {
                                    DatePicker entryControl = control as DatePicker;
                                    if (entryControl == null)
                                        throw new ApplicationException(string.Format(
                                            "Expected DateTimePicker control for '{0}'",
                                            prop.DisplayName));

                                    if (entryControl.SelectedDate != null)
                                        return entryControl.SelectedDate;
                                    return string.Empty;
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
                                {
                                    IntegerUpDown entryControl = control as IntegerUpDown;
                                    if (entryControl == null)
                                        throw new ApplicationException(string.Format("Expected NumericUpDown control for '{0}'",
                                            prop.DisplayName));

                                    if (entryControl.Value != null)
                                        return entryControl.Value;
                                    return string.Empty;
                                }
                            case DataTypesEnum.DecimalType:
                                {
                                    DecimalUpDown entryControl = control as DecimalUpDown;
                                    if (entryControl == null)
                                        throw new ApplicationException(string.Format("Expected NumericUpDown control for '{0}'",
                                            prop.DisplayName));

                                    if (entryControl.Value != null)
                                        return entryControl.Value;
                                    return string.Empty;
                                }
                        }
                        break;
                    }

                case SelectionTypes.List:
                    {
                        ListBox entryControl = control as ListBox;
                        if (entryControl == null)
                            throw new ApplicationException(string.Format("Expected ComboBox control for '{0}'",
                                prop.DisplayName));

                        if (entryControl.SelectedValue != null)
                            return entryControl.SelectedValue;
                        return string.Empty;
                    }

                case SelectionTypes.Lookup:
                    {
                        ComboBox entryControl = control as ComboBox;
                        if (entryControl == null)
                            throw new ApplicationException(string.Format("Expected ComboBox control for '{0}'",
                                prop.DisplayName));

                        if (entryControl.SelectedValue != null)
                            return entryControl.SelectedValue;
                        return string.Empty;
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

        private void SetFeatureControlValue(UIElement control, PropertyDefinition prop,
          AbstractSelectionValueTypes propTypeValues, object value)
        {

            switch (prop.PropertyDataType.SelectionType)
            {
                case SelectionTypes.None:
                    {
                        if (value == null || value.ToString() == "")
                            return;

                        switch (prop.PropertyDataType.DataTypesEnum)
                        {
                            case DataTypesEnum.BooleanType:
                                {
                                    CheckBox entryControl = control as CheckBox;
                                    if (entryControl == null)
                                        throw new ApplicationException(string.Format("Expected CheckBox control for '{0}'",
                                            prop.DisplayName));

                                    entryControl.IsChecked = Boolean.Parse(value.ToString());
                                    break;

                                }
                            case DataTypesEnum.DateTimeType:
                                {
                                    DatePicker entryControl = control as DatePicker;
                                    if (entryControl == null)
                                        throw new ApplicationException(string.Format(
                                            "Expected DateTimePicker control for '{0}'",
                                            prop.DisplayName));
                                    entryControl.SelectedDate = DateTime.Parse(value.ToString());
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

                                    IntegerUpDown entryControl = control as IntegerUpDown;
                                    if (entryControl == null)
                                        throw new ApplicationException(
                                            string.Format("Expected NumericUpDown control for '{0}'",
                                                prop.DisplayName));
                                    entryControl.Value = Int32.Parse(value.ToString());
                                    break;
                                }

                            case DataTypesEnum.DecimalType:
                                {
                                    DecimalUpDown entryControl = control as DecimalUpDown;
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

                        ListBox entryControl = control as ListBox;
                        if (entryControl == null)
                            throw new ApplicationException(string.Format("Expected ComboBox control for '{0}'",
                                prop.DisplayName));
                        if (value != null)
                        {
                            entryControl.SelectedIndex = listTypeValues.ListValues.ToList().IndexOf(value);
                            if (entryControl.SelectedItem != null)
                                entryControl.ScrollIntoView(entryControl.SelectedItem);

                        }
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

                            if ((kvPair.Key != null) && kvPair.Key.Equals(value))
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

        /// <summary>
        /// VAlidate the values selected in feature configuration
        /// </summary>

        public bool ValidateFeatureConfigurationvalues(Dictionary<string, ICollection<PropertyValuePairValidationResponse>> validationResponses)
        {
            ResetError();
            if (ValidateForEmptyValues())
                return false;
            bool canSave = true;
            foreach (var validationResponse in validationResponses)
            {
                string featureName = validationResponse.Key;
                if (validationResponse.Value == null) continue;

                foreach (var propValuePairResponse in validationResponse.Value)
                {
                    if (propValuePairResponse.Status.Equals(Status.Failure))
                    {
                        UIElement entryControl = (from c in _featurePropControls
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

                            SetFeatureControlError(entryControl, msg, false);
                        }
                        canSave = false;
                    }
                }
            }
            return canSave;

        }

        private bool ValidateForEmptyValues()
        {
            bool error = false;
            foreach (var featurePropControl in _featurePropControls)
            {
                //errorProvider1.SetError(featurePropControl.EntryControl, "");

                Debug.Print("{0}, {1}", featurePropControl.FeatureName, featurePropControl.PropertyName);
                FeaturePropControl control = featurePropControl;
                var featureProps = (from fp in _featurePropertyLists
                                    where fp.Key.Name.Equals(control.FeatureName)
                                    select fp).FirstOrDefault();
                var prop = (from p in featureProps.Value where p.PropertyName.Equals(control.PropertyName) select p).FirstOrDefault();
                if (prop == null)
                    throw new ApplicationException(string.Format("Property definition missing for '{0}' property of '{1}' feature",
                        control.PropertyName, featurePropControl.FeatureName));
                object value = GetFeatureControlValue(prop, control.EntryControl).ToString();
                if (value.ToString() == string.Empty)
                {
                    SetFeatureControlError(control.EntryControl, "All Fields are Required ", false);
                    error = true;
                }


            }
            return error;
        }
        private void ResetError()
        {
            foreach (var featurePropControl in _featurePropControls)
            {
                //errorProvider1.SetError(featurePropControl.EntryControl, "");

                Debug.Print("{0}, {1}", featurePropControl.FeatureName, featurePropControl.PropertyName);
                FeaturePropControl control = featurePropControl;
                var featureProps = (from fp in _featurePropertyLists
                                    where fp.Key.Name.Equals(control.FeatureName)
                                    select fp).FirstOrDefault();
                var prop = (from p in featureProps.Value where p.PropertyName.Equals(control.PropertyName) select p).FirstOrDefault();
                if (prop == null)
                    throw new ApplicationException(string.Format("Property definition missing for '{0}' property of '{1}' feature",
                        control.PropertyName, featurePropControl.FeatureName));
                SetFeatureControlError(control.EntryControl, "", true);

            }

        }
        private void FeatureControlReadOnly(bool readOnly)
        {
            foreach (var featurePropControl in _featurePropControls)
            {
                //errorProvider1.SetError(featurePropControl.EntryControl, "");

                Debug.Print("{0}, {1}", featurePropControl.FeatureName, featurePropControl.PropertyName);
                FeaturePropControl control = featurePropControl;
                var featureProps = (from fp in _featurePropertyLists
                                    where fp.Key.Name.Equals(control.FeatureName)
                                    select fp).FirstOrDefault();
                var prop = (from p in featureProps.Value where p.PropertyName.Equals(control.PropertyName) select p).FirstOrDefault();
                if (prop == null)
                    throw new ApplicationException(string.Format("Property definition missing for '{0}' property of '{1}' feature",
                        control.PropertyName, featurePropControl.FeatureName));
                control.EntryControl.IsEnabled = !readOnly;

            }

        }
        private void Getsavevalue()
        {
            var validateFeaturePropValuePairs = new Dictionary<string, IDictionary<String, Object>>();
            var savePropValuePairs = new Dictionary<string, IDictionary<String, Object>>();

            foreach (var featurePropControl in _featurePropControls)
            {
                //errorProvider1.SetError(featurePropControl.EntryControl, "");

                Debug.Print("{0}, {1}", featurePropControl.FeatureName, featurePropControl.PropertyName);
                FeaturePropControl control = featurePropControl;
                var featureProps = (from fp in _featurePropertyLists
                                    where fp.Key.Name.Equals(control.FeatureName)
                                    select fp).FirstOrDefault();
                var prop = (from p in featureProps.Value where p.PropertyName.Equals(control.PropertyName) select p).FirstOrDefault();
                if (prop == null)
                    throw new ApplicationException(string.Format("Property definition missing for '{0}' property of '{1}' feature",
                        control.PropertyName, featurePropControl.FeatureName));

                object value = GetFeatureControlValue(prop, control.EntryControl).ToString();

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

            FeatureSaveValues = savePropValuePairs;
        }


        private void SetFeatureControlError(UIElement control, string message, bool reset)
        {

            var errorBrushes = (reset ? Brushes.Black : Brushes.Red);

            if (control is DatePicker)
            {
                DatePicker entryControl = control as DatePicker;
                //entryControl.Foreground = errorBrushes;
                entryControl.ToolTip = message;

                entryControl.BorderBrush = errorBrushes;
                if (reset) entryControl.ClearValue(Border.BorderBrushProperty);
                return;
            }
            if (control is TextBox)
            {
                TextBox entryControl = control as TextBox;
                //entryControl.Foreground = errorBrushes;
                entryControl.ToolTip = message;


                entryControl.BorderBrush = errorBrushes;
                if (reset) entryControl.ClearValue(Border.BorderBrushProperty);
                return;
            }
            if (control is DecimalUpDown)
            {
                DecimalUpDown entryControl = control as DecimalUpDown;
                //entryControl.Foreground = errorBrushes;
                entryControl.ToolTip = message;


                entryControl.BorderBrush = errorBrushes;
                if (reset) entryControl.ClearValue(Border.BorderBrushProperty);
                return;
            }
            if (control is IntegerUpDown)
            {
                IntegerUpDown entryControl = control as IntegerUpDown;
                //entryControl.Foreground = errorBrushes;
                entryControl.ToolTip = message;


                entryControl.BorderBrush = errorBrushes;
                if (reset) entryControl.ClearValue(Border.BorderBrushProperty);
                return;
            }

            if (control is ListBox)
            {
                ListBox entryControl = control as ListBox;
                //entryControl.Foreground = errorBrushes;
                entryControl.ToolTip = message;


                entryControl.BorderBrush = errorBrushes;
                if (reset) entryControl.ClearValue(Border.BorderBrushProperty);
                return;
            }
            if (control is ComboBox)
            {
                ComboBox entryControl = control as ComboBox;
                //entryControl.Foreground = errorBrushes;
                entryControl.ToolTip = message;


                entryControl.BorderBrush = errorBrushes;
                if (reset) entryControl.ClearValue(Border.BorderBrushProperty);
            }
        }


        #endregion

        /// <summary>
        /// TO initilize the feature control components
        /// </summary>
        /// <param name="featurePropertyLists"></param>
        /// <param name="featurePropertyEntryValues"></param>
        /// <param name="featurePropertyValuePairs"></param>
        /// <param name="commandTypes"></param>
        /// <param name="readOnly"></param>
        public void IntilizeFeatureControl(Dictionary<FeatureMetadata, IList<PropertyDefinition>> featurePropertyLists, IDictionary<string, Dictionary<string, AbstractSelectionValueTypes>> featurePropertyEntryValues, IDictionary<String, Dictionary<string, object>> featurePropertyValuePairs, CommandTypes commandTypes, bool readOnly)
        {
            _featurePropertyLists = featurePropertyLists;
            CommandDisplay = commandTypes;
            RemoveCommands();
            DrawFeatureControl(featurePropertyEntryValues, featurePropertyValuePairs, readOnly);
            if (_dynamicGrid != null)
                DisplayCommand(_dynamicGrid, readOnly);
            Content = _dynamicGrid;

        }

        /// <summary>
        /// 
        /// </summary>
        public class FeaturePropControl
        {
            /// <summary>
            /// 
            /// </summary>
            public String FeatureName { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public String PropertyName { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public UIElement EntryControl { get; set; }
        }

    }
    /// <summary>
    /// List of command buttons to be  displayed in the control
    /// </summary>
    public enum CommandTypes
    {
        /// <summary>
        /// To display the OK and Cancel Button
        /// </summary>
        OkCancel,

        /// <summary>
        /// Display OK Onlyh Button
        /// </summary>
        SubmitOnly,

        /// <summary>
        /// Cancel Only
        /// </summary>
        CancelOnly,
        /// <summary>
        /// TO display validate cancel Button
        /// </summary>
        ValidateCancel,
        /// <summary>
        /// TO display validate Button
        /// </summary>
        ValidateOnly,
        /// <summary>
        /// TO display Register Button
        /// </summary>
        RegisterCancel,
        /// <summary>
        /// No Command Buttons to display
        /// </summary>
        None,
        /// <summary>
        /// TO display register command only
        /// </summary>
        RegisterOnly,
        /// <summary>
        /// TO display register Save only
        /// </summary>
        SaveOnly
    }




}
