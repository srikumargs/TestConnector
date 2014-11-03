using Sage.Connector.DBTool.Internal;

namespace Sage.Connector.DBTool.Migration
{
    interface IMigrationStep
    {
        ExitCode Upgrade(MigrationInfo info);
    }
}
