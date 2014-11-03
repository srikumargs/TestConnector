using System.Windows.Controls;
using SageConnectorConfiguration.ViewModel;

namespace SageConnectorConfiguration.View
{
    /// <summary>
    /// designer class for InstallUserControl.xaml
    /// </summary>
    public partial class InstallUserControl : UserControl
    {
        /// <summary>
        /// ctor
        /// </summary>
        public InstallUserControl()
        {
            InitializeComponent();
        }

        public void Initialize(RootViewModel rootVM)
        {
            this.DataContext = rootVM.Install;
        }
    }
}
