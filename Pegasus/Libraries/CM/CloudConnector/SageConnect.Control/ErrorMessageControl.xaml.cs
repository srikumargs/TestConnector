using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using SageConnect.ViewModels;

namespace SageConnect.Control
{
    /// <summary>
    /// Interaction logic for ErrorMessageControl.xaml
    /// </summary>
// ReSharper disable once RedundantExtendsListEntry
    public partial class ErrorMessageControl : UserControl
    {

        /// <summary>
        /// Cancel Button even click for the control
        /// </summary>
        public static readonly RoutedEvent CancelButtonClickEvent = EventManager.RegisterRoutedEvent(
            "CancelButtonClick", RoutingStrategy.Bubble, typeof (RoutedEventHandler), typeof (ErrorMessageControl));
 
        
        /// <summary>
        /// Control to display error message to the customers
        /// </summary>
        public ErrorMessageControl()
        {
            InitializeComponent();


        }

      
        #region RoutedEvents

        /// <summary>
        /// Close Button in to hid the Control
        /// </summary>
        public event RoutedEventHandler CancelButtonClick
        {
            add { AddHandler(CancelButtonClickEvent, value); }
            remove { RemoveHandler(CancelButtonClickEvent, value); }
        }

        #endregion

       

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(CancelButtonClickEvent));
        }


        private void Hyperlink_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start(ErrorMessageViewModel.ErrorMessageViewModelInstance.UriforHyperLink);
        }
    }
}
