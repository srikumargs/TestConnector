using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SageConnect.Control
{
    /// <summary>
    /// Interaction logic for SettingsControl.xaml
    /// </summary>
// ReSharper disable once RedundantExtendsListEntry
    public partial class SettingsControl : UserControl
    {
        /// <summary>
        /// Constructor for Setting Control
        /// </summary>
        public SettingsControl()
        {
            InitializeComponent();
            SettingsGrid.DataContext = this;
        }


        /// <summary>
        /// Edit Setting Event Publish
        /// </summary>
        public static readonly RoutedEvent EditSettingsClickEvent = EventManager.RegisterRoutedEvent(
            "EditSettingsClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SettingsControl));

        /// <summary>
        /// Clear Setting Event Publish
        /// </summary>
        public static readonly RoutedEvent ClearSettingsClickEvent = EventManager.RegisterRoutedEvent(
            "ClearSettingsClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SettingsControl));

        /// <summary>
        /// Help Setting Event Publish
        /// </summary>
        public static readonly RoutedEvent HelpSettingsClickEvent = EventManager.RegisterRoutedEvent(
            "HelpSettingsClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SettingsControl));

        /// <summary>
        /// Description to be displayed in belwo the caption
        /// </summary>
        public static readonly DependencyProperty NameDescriptionProperty = DependencyProperty.Register("MessageDescription", typeof(string),
            typeof(SettingsControl), new PropertyMetadata(string.Empty));


        /// <summary>
        /// Description of header to be displayed 
        /// </summary>
        public static readonly DependencyProperty HeaderDescriptionProperty = DependencyProperty.Register("HeaderDescription", typeof(string),
            typeof(SettingsControl), new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        private static void TextChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SettingsControl searchTextBox = (SettingsControl)d;
            searchTextBox.HeaderTextBlock.Text = (string)e.NewValue;
        }

        /// <summary>
        /// Visiblity condition of the Edit Setting Label
        /// </summary>
        public static readonly DependencyProperty DisplayHelpSettingsProperty = DependencyProperty.Register("ShowHelpSettings", typeof(object),
            typeof(SettingsControl), new PropertyMetadata(true));

        /// <summary>
        /// Visiblity condition of the  Settings Label
        /// </summary>
        public static readonly DependencyProperty DisplaySettingsProperty = DependencyProperty.Register("ShowSettings", typeof(object),
            typeof(SettingsControl), new PropertyMetadata(true));

        /// <summary>
        /// To display the tooltip for settings
        /// </summary>
        public static readonly DependencyProperty EditSettingsToolTipProperty = DependencyProperty.Register("EditSettingsToolTip", typeof(string),
            typeof(SettingsControl), new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// To display the tooltip for settings
        /// </summary>
        public static readonly DependencyProperty ClearSettingsToolTipProperty = DependencyProperty.Register("ClearSettingsToolTip", typeof(string),
            typeof(SettingsControl), new FrameworkPropertyMetadata(default(string), FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        
        /// <summary>
        /// Close Button in to hid the Control
        /// </summary>
        public event RoutedEventHandler EditSettingsClick
        {
            add { AddHandler(EditSettingsClickEvent, value); }
            remove { RemoveHandler(EditSettingsClickEvent, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string MessageDescription
        {
            get { return (string) GetValue(NameDescriptionProperty); }

            set
            {
                SetValue(NameDescriptionProperty, value);
                //DescriptionTextBlock.Text = value;
                // set the value of the control's text here...!
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public string HeaderDescription
        {
            get { return (string)GetValue(HeaderDescriptionProperty); }

            set
            {
                SetValue(HeaderDescriptionProperty, value);
                //HeaderTextBlock.Text = value;
                // set the value of the control's text here...!

            }
        }
        private  void OnHeaderDescriptionProperty(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            HeaderDescription = "test";
        }
        /// <summary>
        /// 
        /// </summary>
        public Visibility ShowHelpSettings
        {
            get { return (Visibility)GetValue(DisplayHelpSettingsProperty); }

            set
            {
                SetValue(DisplayHelpSettingsProperty, value);
                if (value == Visibility.Visible) SetHelpSettingsUi();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Visibility ShowSettings
        {
            get { return (Visibility)GetValue(DisplaySettingsProperty); }

            set
            {
                SetValue(DisplaySettingsProperty, value);
            }
        }
        private void SetHelpSettingsUi()
        {
            LineRectangle.Height = 1;
            HeaderTextBlock.FontSize = 14;
        }

        /// <summary>
        ///  Tool tip setter property for Edit Settings
        /// </summary>
        public string EditSettingsToolTip
        {
            get { return (string)GetValue(EditSettingsToolTipProperty); }

            set
            {
                SetValue(EditSettingsToolTipProperty, value);
            }
        }
        /// <summary>
        ///  Tool tip setter property for Edit Settings
        /// </summary>
        public string ClearSettingsToolTip
        {
            get { return (string)GetValue(ClearSettingsToolTipProperty); }

            set
            {
                SetValue(ClearSettingsToolTipProperty, value);
            }
        }
        /// <summary>
        /// Close Button in to hid the Control
        /// </summary>
        public event RoutedEventHandler ClearSettingsClick
        {
            add { AddHandler(ClearSettingsClickEvent, value); }
            remove { RemoveHandler(ClearSettingsClickEvent, value); }
        }
        /// <summary>
        /// Close Button in to hid the Control
        /// </summary>
        public event RoutedEventHandler HelpSettingsClick
        {
            add { AddHandler(HelpSettingsClickEvent, value); }
            remove { RemoveHandler(HelpSettingsClickEvent, value); }
        }
        private void clearSettings_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(ClearSettingsClickEvent));
        }

        private void EditSetting_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(EditSettingsClickEvent));
        }
        private void HelpSetting_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(HelpSettingsClickEvent));
        }

        private void MouseEnter_Event(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void MouseLeave_Event(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
        }
    }
}
