using System.Windows.Controls;
using SageConnectorConfiguration.ViewModel;

namespace SageConnectorConfiguration.View
{
    /// <summary>
    /// designer class for ConfigureUserControl.xaml
    /// </summary>
    public partial class ConfigureUserControl : UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        public ConfigureUserControl()
        {
            InitializeComponent();
        }

        public void Initialize(RootViewModel rootVM)
        {
            this.DataContext = rootVM.Configure;
        }
    }
}
