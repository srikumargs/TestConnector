
using System;

namespace Sage.Connector.Discovery.Contracts.Data
{
    /// <summary>
    /// Information that the about the plug in and the back office.
    /// </summary>
    public class PluginInformation
    {
        /// <summary>
        /// Short ID of the back office this plug in serves
        /// </summary>
        /// <remarks>
        /// Returns even if the back office is not installed
        /// This will be used as both an id and directory name.
        /// It should not contain any chars other then letters and numbers.
        /// </remarks>
        public string BackOfficeId { get; set; }

        /// <summary>
        /// Gets or sets the automatic update product identifier.
        /// </summary>
        /// <value>
        /// The automatic update product identifier.
        /// </value>
        /// <remarks>
        /// This replaces the obsolete field BackOfficePluginProductId, new name for consistency.
        /// Used to with AutoUpdateProductVersion to find the list of possible updates from AU.
        /// </remarks>
        public string AutoUpdateProductId { get; set; }

        /// <summary>
        /// Gets or sets the automatic update product version.
        /// This is expected to be empty if the back office is not installed.
        /// </summary>
        /// <value>
        /// The automatic update product version.
        /// </value>
        /// <remarks>
        /// Used with the AutoUpdateProductId to find the list of updates from AU.
        ///
        /// This may have the same value as BackOfficeVersion. The separate value
        /// allows for the case where different version schemes are used.
        /// </remarks>
        public string AutoUpdateProductVersion { get; set; }
        
        /// <summary>
        /// Gets or sets the name of the automatic update component base.
        /// </summary>
        /// <value>
        /// The name of the automatic update component base.
        /// </value>
        /// <remarks>
        /// Expected to be of the form BackOfficePlugin.[BackOfficeId].
        /// For example "BackOfficePlugin.Mock."
        /// Once a list of possible updates is available from AU this will be used to filter
        /// them down to the relevant updates. This will be used to check the prefix/BaseName of
        /// the AU UpdateId. 
        /// </remarks>
        public string AutoUpdateComponentBaseName { get; set; }

        /// <summary>
        /// User friendly name of the back office for this plug in 
        /// Expected to work even if the back office is not installed
        /// </summary>
        public string BackOfficeName { get; set; }

        /// <summary>
        /// The version of the back office present. IF installed.
        /// This is expected to be empty if the bad office is not installed.
        /// </summary>
        /// <remarks>
        /// Version of the back office installed. If the back office is installed.
        /// Is empty string if the back office is not installed.
        /// </remarks>
        public string BackOfficeVersion { get; set; }

        /// <summary>
        /// Version of the discovery plugin
        /// </summary>
        public string PluginVersion { get; set; }
        
        /// <summary>
        /// X86,AnyCpu etc what 
        /// </summary>
        public string Platform { get; set; }

        /// <summary>
        /// Does the plugin need to run in a user context?
        /// Would be best if none did but S100Contractor for example needs mapped drives
        /// </summary>
        /// <remarks>
        /// Maybe revised. This is not fully cooked. It is to help with the case
        /// of the plugin needing a mapped drive or to run in a user context
        /// Maybe make the plugin responsible for impersonation?
        /// </remarks>
        public bool RunAsUserIsRequired { get; set; }

        //--------------------- retired ------------
        /// <summary>
        /// The product identifier for the back office plugin set as 
        /// identified in Sage Auto Update.
        /// Expected to work even if the back office is not installed
        /// </summary>
        /// <remarks>
        /// If the connector is configuring your back office for use,
        /// it will use this to get the latest/correct plug ins for configuration 
        /// and the application areas from sage auto update.
        /// </remarks>
        [Obsolete]
        public string BackOfficePluginProductId { get; set; }

        //maybe obsolete based on how new UX for configuration works
        /// <summary>
        /// The terminology for the administrator for the plugin
        /// </summary>
        public string LoginAdministratorTerm { get; set; }

        /// <summary>
        /// URI used to provide help for the plugin configuration experience
        /// Expected to work even if the back office is not installed
        /// </summary>
        /// <remarks>
        /// Maybe get this from the cloud instead
        /// </remarks>
        public string HelpUri { get; set; }
    }
}
