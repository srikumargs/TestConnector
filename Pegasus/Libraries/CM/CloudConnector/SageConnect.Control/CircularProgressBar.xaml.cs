using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
namespace SageConnect.Control
{
    /// <summary>
    /// Interaction logic for CircularProgressBar.xaml
    /// </summary>
// ReSharper disable once RedundantExtendsListEntry
    public partial class CircularProgressBar : UserControl
    {
        /// <summary>
        /// Circular progress bar for the  service start up
        /// </summary>
        public CircularProgressBar()
        {
            InitializeComponent();
            //Timeline.DesiredFrameRateProperty.OverrideMetadata(
            //     typeof(Timeline),
            //          new FrameworkPropertyMetadata { DefaultValue = 8 });
        }
    }
}
