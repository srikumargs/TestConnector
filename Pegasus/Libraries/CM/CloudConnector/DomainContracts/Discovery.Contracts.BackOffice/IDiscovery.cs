using System;
using Sage.Connector.Discovery.Contracts.Data;
using Sage.Connector.DomainContracts.BackOffice;

namespace Sage.Connector.Discovery.Contracts.BackOffice
{
    /// <summary>
    /// API to help install and configuration
    /// These APIs are expected to be callable with or without MAF/MEF if needed.
    /// </summary>
    /// <remarks>
    /// Note that Implementation of the plugin is dependent on domain contracts core for the BackOfficeConfigMetadataExportAttribute's inheritance chain.
    /// without that the implementation would be completely isolated.
    /// </remarks>
    public interface IDiscovery
    {
        /// <summary>
        /// Check that the back office software is installed and available to be used by the plug in
        /// </summary>
        /// <param name="sessionContext">session context to provide services</param>
        /// <returns>true when the back office is installed, false otherwise</returns>
        Boolean IsBackOfficeInstalled(ISessionContext sessionContext);

        /// <summary>
        /// Get the details of the plugin and back office needed to allow configuration
        /// </summary>
        /// <param name="sessionContext">session context to provide services</param>
        /// <returns>The <see cref="PluginInformation"/></returns>
        PluginInformation GetPluginInformation(ISessionContext sessionContext);
    }
}
