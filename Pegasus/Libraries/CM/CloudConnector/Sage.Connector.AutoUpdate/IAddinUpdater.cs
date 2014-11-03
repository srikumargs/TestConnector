using System;

namespace Sage.Connector.AutoUpdate
{
    /// <summary>
    /// Interface definition for addin auto updater implementations.
    /// </summary>
    public interface IAddinUpdater : IDisposable
    {
        /// <summary>
        /// Applies the update that is available in the staging folder.
        /// </summary>
        /// <returns>True if the update was applied, otherwise false.</returns>
        bool ApplyUpdate();

        /// <summary>
        /// Determines if an update is available and ready to be applied.
        /// </summary>
        /// <returns>True if an update is available, otherwise false.</returns>
        bool IsUpdateAvailable();

        /// <summary>
        /// Determines if an update is currently in progress.
        /// </summary>
        /// <returns>True if an update is in progress, otherwise false.</returns>
        bool UpdateInProgress();

        /// <summary>
        /// Performs a forced check for updates on the staging server.
        /// </summary>
        /// <returns>True is an update has been found and downloaded to the staging folder.</returns>
        bool CheckForUpdates();

        /// <summary>
        /// Return the product version to check for updates with..
        /// </summary>
        string ProductVersion { get; }

        /// <summary>
        /// Returns the addin path that is being monitored.
        /// </summary>
        string AddinPath { get; }

        /// <summary>
        /// Returns the product id associated with the IAddinUpdater.
        /// </summary>
        string ProductId { get; }
    }
}
