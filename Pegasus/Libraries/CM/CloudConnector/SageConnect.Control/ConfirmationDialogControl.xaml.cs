using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using SageConnect.ViewModels;

namespace SageConnect.Control
{
    /// <summary>
    /// Interaction logic for ConfirmationDialogControl.xaml
    /// </summary>
    public partial class ConfirmationDialogControl
    {

        /// <summary>
        /// Confirm Setting Event Publish
        /// </summary>
        public static readonly RoutedEvent ConfirmSettingsClickEvent = EventManager.RegisterRoutedEvent(
            "ConfirmSettingsClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SettingsControl));
        /// <summary>
        /// Cancel Setting Event Publish
        /// </summary>
        public static readonly RoutedEvent CancelSettingsClickEvent = EventManager.RegisterRoutedEvent(
            "CancelSettingsClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(SettingsControl));
        /// <summary>
        /// 
        /// </summary>
        public ConfirmationDialogControl()
        {
            InitializeComponent();
            ClearsettingsButton.Focus();
        }

        private void UIElement_OnGotFocus(object sender, RoutedEventArgs e)
        {
            Button design = (Button) sender;

            design.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#41a940");
        }

        private void UIElement_OnLostFocus(object sender, RoutedEventArgs e)
        {
            Button design = (Button)sender;
            design.Background = (SolidColorBrush)new BrushConverter().ConvertFrom("#9a9a9b");
        }
        /// <summary>
        /// Close Button in to hid the Control
        /// </summary>
        public event RoutedEventHandler ConfirmSettingsClick
        {
            add { AddHandler(ConfirmSettingsClickEvent, value); }
            remove { RemoveHandler(ConfirmSettingsClickEvent, value); }
        }
        private void ConfirmSettings_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(ConfirmSettingsClickEvent));
            Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// Close Button in to hid the Control
        /// </summary>
        public event RoutedEventHandler CancelSettingsClick
        {
            add { AddHandler(CancelSettingsClickEvent, value); }
            remove { RemoveHandler(CancelSettingsClickEvent, value); }
        }
        private void CancelSettings_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(CancelSettingsClickEvent));
            Visibility = Visibility.Hidden;
        }

        private void Hyperlink_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start(HelpLinkManager.FindString("ConfirmationDialog_ClearSettingsHelp"));
        }
    }
}
