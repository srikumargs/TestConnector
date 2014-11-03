using System;

namespace SageConnectorConfiguration.ViewModel
{
    /// <summary>
    /// 
    /// </summary>
    public class DoneViewModel : Step
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootViewModel"></param>
        public DoneViewModel(RootViewModel rootViewModel)
        {
            RootViewModel = rootViewModel;
            Name = "Done";
            ID = "Done";
        }

        /// <summary>
        /// The cloud tenant name
        /// </summary>
        public string TenantName
        {
            get { return RootViewModel.Connection.TenantName; }
        }

        /// <summary>
        /// The cloud tenant URL
        /// </summary>
        public string TenantURL
        {
            get { return RootViewModel.Connection.TenantURL; }
        }

        /// <summary>
        /// Back office data folder
        /// </summary>
        public string BackofficeDataFolder
        {
            get 
            {
                if (RootViewModel.Connection.SelectedConnection != null)
                    return RootViewModel.Connection.SelectedConnection.DisplayName;
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Initialize()
        {
            // this control is about to display
            RootViewModel.IsNextVisible = false;
            RootViewModel.IsPreviousVisible = false;
            RootViewModel.IsCloseVisible = true;
            RootViewModel.IsCancelVisible = false;
            RootViewModel.IsInstallVisible = false;
            RootViewModel.IsInstallEnabled = false;
            RootViewModel.IsConfigureEnabled = false;
            RootViewModel.IsConfigureVisible = false;
            RootViewModel.IsOKEnabled = false;
            RootViewModel.IsOKVisible = false;
        }
    }
}
