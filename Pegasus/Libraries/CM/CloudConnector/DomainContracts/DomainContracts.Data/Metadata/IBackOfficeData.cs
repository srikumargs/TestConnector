using System;

namespace Sage.Connector.DomainContracts.Data.Metadata
{
    /// <summary>
    /// The interface used to identify the back office plugin
    /// This is to be used on any feature except the main configuration features
    /// </summary>
    public interface IBackOfficeData
    {
        /// <summary>
        /// The Id representing the back office used to know which plugin will process the request. 
        /// </summary>
        String BackOfficeId { get; }

    }
}
