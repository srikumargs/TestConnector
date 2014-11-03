
namespace Sage.Connector.DBTool.Internal
{
    /// <summary>
    /// The possible exit codes returned from teh shim EXE
    /// </summary>
    public enum ExitCode
    {
        /// <summary>
        /// Operation completed normally
        /// </summary>
        Success = 0,

        /// <summary>
        /// Something went wrong
        /// </summary>
        OneOrMoreFailuresOccurred = 1,

        /// <summary>
        /// Migration failed but data files were left in a consistent "empty" state.
        /// </summary>
        MigrationFailedEmptyFilesInPlace = 2 
    }
}
