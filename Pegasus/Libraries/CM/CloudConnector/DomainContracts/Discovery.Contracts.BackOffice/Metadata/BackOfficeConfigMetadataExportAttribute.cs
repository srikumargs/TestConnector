using System;
using System.ComponentModel.Composition;
using Sage.Connector.Discovery.Contracts.Data.Metadata;

namespace Sage.Connector.Discovery.Contracts.BackOffice.Metadata
{/// <summary>
    /// Back Office metadata export attribute. 
    /// To be used ONLY on the <see cref="IDiscovery"/> back office plugin implementation
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false), MetadataAttribute]
    public class BackOfficeConfigMetadataExportAttribute : ExportAttribute, IBackOfficeConfigMetadata
    {
        /// <summary>
        /// The id representing this back office application
        /// </summary>
        public String BackOfficeId { get; private set; }

        /// <summary>
        /// The nice name of the back office plugin
        /// </summary>
        public String BackOfficeName { get; private set; }
        /// <summary>
        /// Valid Values {"X86", "AnyCPU", X64"}
        /// </summary>
        public String Platform { get; private set; }

        /// <summary>
        /// The constructor for the set of feature attributes
        /// </summary>
        /// <param name="backOfficeId">The id representing this back office application</param>
        /// <param name="backOfficeName">The nice name of the back office plugin</param>
        /// <param name="platform"> Valid Values {"X86", "AnyCPU", X64"}</param>
        public BackOfficeConfigMetadataExportAttribute(string backOfficeId, string backOfficeName, string platform)
        {

            if (string.IsNullOrWhiteSpace(backOfficeId))
                throw new ArgumentNullException("backOfficeId", "BackOfficeId must be supplied in the metadata.");
            if (string.IsNullOrWhiteSpace(backOfficeName))
                throw new ArgumentNullException("backOfficeName", "BackOfficeName must be supplied in the metadata.");
            if (string.IsNullOrWhiteSpace(platform))
                throw new ArgumentNullException("platform", "Platform must be supplied in the metadata.");

            backOfficeId = backOfficeId.Trim();
            backOfficeName = backOfficeName.Trim();
            platform = platform.Trim();

            //resolving for case
            if (platform.Equals("X86", StringComparison.CurrentCultureIgnoreCase))
            {
                platform = "X86";
            }
            else if (platform.Equals("X64", StringComparison.CurrentCultureIgnoreCase))
            {
                platform = "X64";
            }
            else if (platform.Equals("AnyCPU", StringComparison.CurrentCultureIgnoreCase))
            {
                platform = "AnyCPU";
            }

            BackOfficeId = backOfficeId;
            BackOfficeName = backOfficeName;
            Platform = platform;
        }
    }
}
