using System;

namespace Sage.Connector.DBTool.Internal
{
    internal sealed class Options
    {
        public Operation Operation { get; set; }
        public String InstanceDataDir { get; set; }
        public String PriorVersionBackupDir { get; set; }
        public String PriorVersion { get; set; }
        public String CurrentVersion { get; set; }
        public String OutputFileLocation { get; set; }
        public String DatabaseFileName { get; set; }
    }
}
