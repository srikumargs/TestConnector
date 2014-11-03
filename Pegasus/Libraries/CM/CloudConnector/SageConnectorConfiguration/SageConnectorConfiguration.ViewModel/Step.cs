using System;

namespace SageConnectorConfiguration.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    public class Step : ViewModelBase
    {
        /// <summary>
        /// view model for the main window
        /// </summary>
        public RootViewModel RootViewModel { get; set; }

        private string _name;
        /// <summary>
        /// Name
        /// </summary>
        public String Name
        {
            get { return _name; }
            set
            {
                if (value != _name)
                {
                    _name = value;
                    RaisePropertyChanged(() => Name);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String ID { get; set; }

        private string _tooltip;
        /// <summary>
        /// Tooltip
        /// </summary>
        public String Tooltip
        {
            get { return _tooltip; }
            set
            {
                if (value != _tooltip)
                {
                    _tooltip = value;
                    RaisePropertyChanged(() => Tooltip);
                }
            }
        }

        private Boolean _isEnabled = true;
        /// <summary>
        /// IsEnabled
        /// </summary>
        public Boolean IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (value != _isEnabled)
                {
                    _isEnabled = value;
                    RaisePropertyChanged(() => IsEnabled);
                }
            }
        }

        private Boolean _isVisible = false;
        /// <summary>
        /// IsVisible
        /// </summary>
        public Boolean IsVisible
        {
            get { return _isVisible; }
            set
            {
                if (value != _isVisible)
                {
                    _isVisible = value;
                    RaisePropertyChanged(() => IsVisible);
                }
            }
        }

        private Boolean _isError = false;
        /// <summary>
        /// IsError
        /// </summary>
        public Boolean IsError
        {
            get { return _isError; }
            set
            {
                if (value != _isError)
                {
                    _isError = value;
                    RaisePropertyChanged(() => IsError);
                }
            }
        }

        private Boolean _isOk = false;
        /// <summary>
        /// IsOk
        /// </summary>
        public Boolean IsOk
        {
            get { return _isOk; }
            set
            {
                if (value != _isOk)
                {
                    _isOk = value;
                    RaisePropertyChanged(() => IsOk);
                }
            }
        }

        private Boolean _isVisited = false;
        /// <summary>
        /// IsVisited
        /// </summary>
        public Boolean IsVisited
        {
            get { return _isVisited; }
            set
            {
                if (value != _isVisited)
                {
                    _isVisited = value;
                    RaisePropertyChanged(() => IsVisited);
                }
            }
        }

        private Boolean _isCurrentlySelected;
        /// <summary>
        /// IsCurrentlySelected
        /// </summary>
        public Boolean IsCurrentlySelected
        {
            get { return _isCurrentlySelected; }
            set
            {
                if (value != _isCurrentlySelected)
                {
                    _isCurrentlySelected = value;
                    RaisePropertyChanged(() => IsCurrentlySelected);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Initialize() { }

    }
}
