using System;

// This source file resides in the "LinkedSource" source code folder in order to enable
// multiple assemblies to share the implementation without requiring the class to be exposed as a
// public type of any shared assembly.
//
// Requires:
//  - n/a
namespace Sage.Connector.LinkedSource
{
    /// <summary>
    /// Utility class to help with versioning and version compares.
    /// </summary>
    internal static class ConnectorVersionUtils
    {
        /// <summary>
        /// Is the passed in interface version compatible
        /// </summary>
        /// <param name="minimumConnectorIntegrationInterfacesVersion"></param>
        /// <param name="backOfficeInterfaceVersion"></param>
        /// <returns>
        /// True if interface is compatible
        /// False if interface is not compatible
        /// Null if it cannot be determined based on the data.
        /// </returns>
        static public bool? IsCompatibleBackOfficeInterfaceVersion(String minimumConnectorIntegrationInterfacesVersion, Version backOfficeInterfaceVersion)
        {
            //note local builds are build int16.Max for revision so they always win on the same day.
            //to prevent issues do have a hard stop, just inform of upgrade.

            bool? retval = null;
            if (backOfficeInterfaceVersion != null)
            {
                Version firstCompatibleVersion;
                bool foundVersion = TryParse(minimumConnectorIntegrationInterfacesVersion, out firstCompatibleVersion);
                if (foundVersion)
                {
                    int result = backOfficeInterfaceVersion.CompareTo(firstCompatibleVersion);

                    //we are compatible if we are the same rev or later then the first compatible version
                    retval = (result >= 0);
                }
            }
            return retval;
        }

        // TryParse doesn't exist in .NET 3.5; and parts of the Connector are pinned back at 3.5 for maximum backoffice compatibility
        static public Boolean TryParse(String versionAsString, out Version version)
        {
            Boolean result = false;

            version = null;
            try
            {
                version = new Version(versionAsString);
                result = true;
            }
            catch (Exception)
            { }

            return result;
        }
    }
}
