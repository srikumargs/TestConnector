using System;
using System.Windows.Input;

namespace SageConnect.ViewModels
{
    /// <summary>
    /// Command Handler for the Commands
    /// </summary>
    public class CommandHandler : ICommand
    {
        private Action _action;
        private bool _canExecute;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="canExecute"></param>
        public CommandHandler(Action action, bool canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }
        /// <summary>
        /// 
        /// </summary>
        event EventHandler ICommand.CanExecuteChanged
        {
            add {  }
            remove {  }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            _action();
        }
    }
}
