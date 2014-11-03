using System.Windows.Controls;
using SageConnectorConfiguration.ViewModel;

namespace SageConnectorConfiguration.View
{
    /// <summary>
    /// designer class for DoneUserControl.xaml
    /// </summary>
    public partial class DoneUserControl : UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        public DoneUserControl()
        {
            InitializeComponent();
        }

        public void Initialize(RootViewModel rootVM)
        {
            this.DataContext = rootVM.Done;
        }
    }
}
