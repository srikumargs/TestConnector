using System;
using Sage.Connector.Discovery.Contracts.Data.Metadata;

namespace Sage.Connector.Discovery.Contracts.Integration.Responses
{
    /// <summary>
    /// The Back Office Company Connection information 
    /// </summary>
    public class BackOfficeConfig : IBackOfficeConfigMetadata
    {

        /// <summary>
        /// Default Constructor
        /// </summary>
        public BackOfficeConfig()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="backOfficeId"></param>
        /// <param name="backOfficeProductName"></param>
        /// <param name="platform"></param>
        public BackOfficeConfig(string backOfficeId, string backOfficeProductName, string platform)
        {
            BackOfficeId = backOfficeId;
            BackOfficeName = backOfficeProductName;
            Platform = platform;
        }
        /// <summary>
        /// The Back Office Company Id, Required
        /// </summary>
        public String BackOfficeId { get; set; }

        /// <summary>
        /// The Back Office Product Name
        /// </summary>
        public String BackOfficeName { get; set; }

        /// <summary>
        /// The Back Office Platform {"X86", "AnyCPU", "X64"}
        /// </summary>
        public String Platform { get; set; }


    }
}