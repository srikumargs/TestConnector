using System;
using Sage.Connector.DomainContracts.Data.Metadata;

namespace Sage.Connector.Discovery.Contracts.Data.Metadata
{
    /// <summary>
    /// Back office configuration metadata
    /// </summary>
    public interface IBackOfficeConfigMetadata : IBackOfficeData
    {
        /// <summary>
        /// The nice name of the back office plugin
        /// </summary>
        String BackOfficeName { get; }
        /// <summary>
        /// Valid Values {"X86", "AnyCPU", X64"}
        /// </summary>
        String Platform { get; }
    }
}
