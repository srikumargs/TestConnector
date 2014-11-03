using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;



namespace SageConnect.ViewModels
{
    /// <summary>
    /// View Model for Error Message display
    /// </summary>
    public class ErrorMessageViewModel : INotifyPropertyChanged
    {

        /// <summary>
        /// 
        /// </summary>
        public ErrorMessageViewModel()
        {
            if (_errorMessageViewModel == null)
                _errorMessageViewModel = this;
        }

        private static ErrorMessageViewModel _errorMessageViewModel;

        /// <summary>
        /// 
        /// </summary>
        public static ErrorMessageViewModel ErrorMessageViewModelInstance
        {
            get { return _errorMessageViewModel; }
            set { _errorMessageViewModel = value; }
        }

        private Visibility _visibility;
        private Visibility _displayokbutton;
        private Visibility _errorImageVisibility;
        private Visibility _monitorCheckboxVisibility;
        private Visibility _helplinkvisibility;
        private Visibility _progressbarVisibility;
        private string _errormessage;
        private string _errorcaption;
        private double _erroropacity;
        private string _monitoropencaption;
        private string _hyperlinktext;
        private string _uriforHyperLink;
        private bool _monitoropenstatus;
        private Brush _imageBackground;
        /// <summary>
        /// To display teh Error Message conrol
        /// </summary>
        public Visibility DisplayErrorMessage
        {
            get
            {
                return _visibility;
            }
            set
            {
                _visibility = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// To display teh Error Image conrol
        /// </summary>
        public Visibility DisplayErrorImage
        {
            get
            {
                return _errorImageVisibility;
            }
            set
            {
                _errorImageVisibility = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// To display teh Error  conrol
        /// </summary>
        public Visibility DisplayConfirmationMessage
        {
            get
            {
                return _visibility;
            }
            set
            {
                _visibility = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// To display Progress bar 
        /// </summary>
        public Visibility DisplayProgressbar
        {
            get
            {
                return _progressbarVisibility;
            }
            set
            {
                _progressbarVisibility = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// To display Button for the Save Message
        /// </summary>
        public Visibility DisplayOkButton
        {
            get
            {
                return _displayokbutton;
            }
            set
            {
                _displayokbutton = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// To display the Open monitor check box control
        /// </summary>
        public Visibility DisplayMonitorOpenCheckbox
        {
            get
            {
                return _monitorCheckboxVisibility;
            }
            set
            {
                _monitorCheckboxVisibility = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// To display the help Link in the message.
        /// </summary>
        public Visibility HelpLinkVisibility
        {
            get
            {
                return _helplinkvisibility;
            }
            set
            {
                _helplinkvisibility = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// To Error Message to be displayed in the control
        /// </summary>
        public string ErrorMessage
        {
            get { return _errormessage; }
            set
            {
                _errormessage = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// To Error opacity  can set to the background of the  control
        /// </summary>
        public double ErrorOpacity
        {
            get { return _erroropacity; }
            set
            {
                _erroropacity = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Caption based on the message
        /// </summary>
        public string ErrorCaption
        {
            get { return _errorcaption; }
            set
            {
                _errorcaption = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Caption to the checkbox control
        /// </summary>
        public string MonitorOpenCaption
        {
            get { return _monitoropencaption; }
            set
            {
                _monitoropencaption = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// To HyperLink to be displayed in the control after error message if required
        /// </summary>
        public string HyperLinkText
        {
            get { return _hyperlinktext; }
            set
            {
                _hyperlinktext = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// URI of for the more information hyperlinks based on message
        /// </summary>
        public string UriforHyperLink
        {
            get { return _uriforHyperLink; }
            set
            {
                _uriforHyperLink = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// To Error Message to be displayed in the control
        /// </summary>
        public bool MonitorOpenstatus
        {
            get { return _monitoropenstatus; }
            set
            {
                _monitoropenstatus = value;
                OnPropertyChanged();
            }
        }

        private BitmapImage _errorImageSource;

        /// <summary>
        /// To set the Error Image Source for the Control
        /// </summary>
        public BitmapImage ErrorImageSource
        {
            get { return _errorImageSource; }
            set
            {
                _errorImageSource = value;
                OnPropertyChanged();

            }
        }

        private Thickness _messageBorderMargin;

        /// <summary>
        /// To set the Thickness of the border in case of error message
        /// </summary>
        public Thickness MessageBorderMargin
        {
            get { return _messageBorderMargin; }
            set
            {
                _messageBorderMargin = value;
                OnPropertyChanged();

            }
        }
        
        private Thickness _messageTextMargin;

        /// <summary>
        /// To set the Thickness of the border in case of error message
        /// </summary>
        public Thickness MessageTextMargin
        {
            get { return _messageTextMargin; }
            set
            {
                _messageTextMargin = value;
                OnPropertyChanged();

            }
        }

        private Thickness _messageImageMargin;

        /// <summary>
        /// To set the Thickness of the border in case of error message
        /// </summary>
        public Thickness MessageImageMargin
        {
            get { return _messageImageMargin; }
            set
            {
                _messageImageMargin = value;
                OnPropertyChanged();
            }
        }

        private Thickness _okButtonMargin;

        /// <summary>
        /// To set the Margin for the button in information messages
        /// </summary>
        public Thickness OkButtonMargin
        {
            get { return _okButtonMargin; }
            set
            {
                _okButtonMargin = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// To set backgroud of the image
        /// </summary>
        public Brush ImageBackgroundColor
        {
            get { return _imageBackground; }
            set
            {
                _imageBackground = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Even handler Property Changed
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"> </param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        private void MessageSettings(string errormessage, string errorcaption,Thickness messagebordermarginThickness,
            Thickness messagetextmarginThickness,Visibility okButtonVisibility,Visibility monitoropencheckboxVisibility,
            BitmapImage errorimagesourceImage,SolidColorBrush imageBackgroundcolor,Visibility helplinkVisibility,Thickness okButtonmargin =default(Thickness) )
        {
            DisplayErrorMessage = Visibility.Visible;
            DisplayErrorImage = Visibility.Visible;
            MessageImageMargin = new Thickness(117, 62, 599, 558);
            ErrorMessage = errormessage;
            ErrorCaption = errorcaption;
            MessageBorderMargin = messagebordermarginThickness;
            MessageTextMargin = messagetextmarginThickness;
            DisplayOkButton = okButtonVisibility;
            DisplayMonitorOpenCheckbox = monitoropencheckboxVisibility;
            MonitorOpenstatus = false;
            ErrorImageSource = errorimagesourceImage;
            ImageBackgroundColor = imageBackgroundcolor;
            HelpLinkVisibility = helplinkVisibility;
            if (okButtonVisibility == Visibility.Visible)
                OkButtonMargin = okButtonmargin;
        }

        /// <summary>
        /// To Display teh Error Message
        /// </summary>
        public void ShowErrorMessage(string errormessage, string errorcaption)
        {
            MessageSettings(errormessage, errorcaption, new Thickness(117, 91, 127, 450), new Thickness(118, 92, 0, 481),
                Visibility.Hidden,Visibility.Hidden,new BitmapImage(new Uri("Resources/Images/error_alert_white.png", UriKind.Relative)),
                (SolidColorBrush)new BrushConverter().ConvertFrom("#ea212d"), Visibility.Hidden);
        }

        /// <summary>
        /// To Display teh Error Message
        /// </summary>
        public void ShowErrorMessage(string errormessage, string errorcaption, string helpLinkuri)
        {
            MessageSettings(errormessage, errorcaption, new Thickness(117, 91, 127, 450), new Thickness(118, 92, 0, 481),
                Visibility.Hidden, Visibility.Hidden, new BitmapImage(new Uri("Resources/Images/error_alert_white.png", UriKind.Relative)),
                (SolidColorBrush)new BrushConverter().ConvertFrom("#ea212d"), Visibility.Visible);
            UriforHyperLink = helpLinkuri;
        }
        /// <summary>
        /// To display the Information Message 
        /// </summary>
        /// <param name="errormessage"></param>
        /// <param name="errorcaption"></param>
        /// <param name="helpLinkuri"></param>
        public void ShowInformationMessage(string errormessage, string errorcaption,string helpLinkuri)
        {
            MessageSettings(errormessage, errorcaption, new Thickness(117, 91, 127, 450), new Thickness(118, 92, 0, 481),
                Visibility.Visible, Visibility.Hidden, new BitmapImage(new Uri("Resources/Images/error_alert_white.png", UriKind.Relative)),
                 (SolidColorBrush)new BrushConverter().ConvertFrom("#009fda"), Visibility.Visible, new Thickness(460, 144, 160, 450));
            UriforHyperLink = helpLinkuri;
            
        }

        /// <summary>
        /// Tdisplay the Information Message 
        /// </summary>
        /// <param name="errormessage"></param>
        /// <param name="errorcaption"></param>
        public void ShowInformationMessage(string errormessage, string errorcaption)
        {
            MessageSettings(errormessage, errorcaption, new Thickness(117, 91, 127, 450), new Thickness(118, 92, 0, 481),
                Visibility.Visible, Visibility.Hidden,new BitmapImage(new Uri("Resources/Images/error_alert_white.png", UriKind.Relative)),
                 (SolidColorBrush)new BrushConverter().ConvertFrom("#009fda"), Visibility.Hidden, new Thickness(460, 144, 160, 450));
        }


        /// <summary>
        /// Display save message with check box and ok button
        /// </summary>
        /// <param name="errormessage"></param>
        /// <param name="errorcaption"></param>
        /// <param name="checkboxcaption"></param>
        public void ShowSaveInformationMessage(string errormessage, string errorcaption, string checkboxcaption)
        {
            MessageSettings(errormessage, errorcaption, new Thickness(117, 91, 127, 380), new Thickness(118, 92, 0, 420),
                Visibility.Visible,Visibility.Visible,new BitmapImage(new Uri("Resources/Images/check_mark_white.png", UriKind.Relative)),
            (SolidColorBrush)new BrushConverter().ConvertFrom("#3ca942"), Visibility.Hidden, new Thickness(460, 205, 160, 371));
            MonitorOpenCaption = checkboxcaption;
        }

        /// <summary>
        /// To hide Error,Confirmation and progress bar controls from the user.
        /// </summary>
        public void HideAllControls()
        {
            DisplayErrorMessage = Visibility.Hidden;
            DisplayConfirmationMessage = Visibility.Hidden;
            DisplayProgressbar = Visibility.Hidden;

        }

    }

    
}
