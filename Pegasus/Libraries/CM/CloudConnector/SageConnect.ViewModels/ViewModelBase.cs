using System;
using System.Windows.Input;

namespace SageConnect.ViewModels
{
    /// <summary>
    /// View Model Base 
    /// </summary>
    public class ViewModelBase
    {


        /// <summary>
        /// ConnectorLoginViewModel to set the context for the Main form
        /// </summary>
        public ConnectorUserLoginViewModel ConnectorUserLoginViewModel { set; get; }
        /// <summary>
        /// Constructor for view model base
        /// </summary>
        public ViewModelBase()
        {
            _canExecute = true;
        }
        private ICommand _clickCommand;
        /// <summary>
        /// Property for click command
        /// </summary>
        public ICommand ClickCommand
        {
            get
            {
                return _clickCommand ?? (_clickCommand = new CommandHandler(ExecuteCommandAction, _canExecute));
            }
        }
        /// <summary>
        /// ;
        /// </summary>
        
        private bool _canExecute;

        /// <summary>
        /// Execute command for click command
        /// </summary>
        public Action ExecuteCommandAction { get; set; }

    

    
    }
}
