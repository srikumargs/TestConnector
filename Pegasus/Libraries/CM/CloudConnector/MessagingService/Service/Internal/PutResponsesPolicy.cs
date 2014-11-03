using System;
using Sage.Connector.Data;

namespace Sage.Connector.MessagingService.Internal
{
    /// <summary>
    /// Policy class that governs PutResponses behavior; respects peak/off-peak cloud policy
    /// </summary>
    internal sealed class PutResponsesPolicy
    {
        /// <summary>
        /// Initializes a new instance of the PutResponsesPolicy class
        /// </summary>
        /// <param name="pcr"></param>
        /// <param name="configParams"></param>
        public PutResponsesPolicy(
            PremiseConfigurationRecord pcr,
            Sage.Connector.Cloud.Integration.Interfaces.WebAPI.Configuration configParams)
        {
            // Set the associated configuration record and config params
            Configuration = pcr;
            ConfigParams = configParams;
        }

        /// <summary>
        /// The configuration record
        /// </summary>
        public PremiseConfigurationRecord Configuration { get; private set; }

        /// <summary>
        /// The remote config params
        /// </summary>
        public Sage.Connector.Cloud.Integration.Interfaces.WebAPI.Configuration ConfigParams { get; private set; }

        /// <summary>
        /// Gets the maximum size of a response desired in a single PutResponses call; influenced by current time of day
        /// May retrun different values based on the current time of day (peak vs. non-peak)
        /// </summary>
        /// <returns></returns>
        public UInt32 GetCurrentLargeResponseSizeThreshold()
        {
            return ConfigParams.LargeResponseSizeThreshold;
        }        
    }
}
