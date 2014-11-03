
namespace Sage.Connector.DBTool.Internal
{
    /// <summary>
    /// The operation to be invoked in the shim EXE
    /// </summary>
    public enum Operation
    {
        /// <summary>
        /// No Operation (default value automatically initialized by runtime)
        /// </summary>
        None = 0,

        /// <summary>
        /// 
        /// </summary>
        SchemaUpgradeAndDataMigrate,

        /// <summary>
        /// 
        /// </summary>
        HardCorruptDatabase,

        /// <summary>
        /// 
        /// </summary>
        RepairDatabase,

        /// <summary>
        /// 
        /// </summary>
        TestConnection,
    }
}
