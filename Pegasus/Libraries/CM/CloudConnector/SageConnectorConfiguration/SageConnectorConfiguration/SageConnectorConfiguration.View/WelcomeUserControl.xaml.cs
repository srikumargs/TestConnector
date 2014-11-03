using System;
using System.Windows.Controls;
using SageConnectorConfiguration.ViewModel;

namespace SageConnectorConfiguration.View
{
    /// <summary>
    /// designer class for WelcomeUserControl.xaml
    /// </summary>
    public partial class WelcomeUserControl : UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        public WelcomeUserControl()
        {
            InitializeComponent();
        }

        public void Initialize(RootViewModel rootVM)
        {
            this.DataContext = rootVM.Welcome;
        }
    }
}
