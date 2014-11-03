using System.Collections.ObjectModel;

namespace SageConnect.Internal
{
    /// <summary>
    /// Holds the Image and Message details for the legend
    /// </summary>
    public class ImageLegend
    {
        /// <summary>
        /// Represent Image for legends
        /// </summary>
        public string LegendImageSource { get; set; }

        /// <summary>
        /// Property for description of Image
        /// </summary>
        public string ImageDescription { get; set; }
    }


    /// <summary>
    /// Collection Class for the ImageLegend
    /// </summary>
    public class ImageLegendCollection:ObservableCollection<ImageLegend>
    { }
}
