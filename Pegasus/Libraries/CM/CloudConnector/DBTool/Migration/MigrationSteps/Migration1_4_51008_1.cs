using System;
using System.Data;
using System.Data.SqlServerCe;
using Sage.Connector.DBTool.Internal;

namespace Sage.Connector.DBTool.Migration
{



    /// <summary>
    /// Left as Example - Not in use
    /// Update to Log store to add columns... .
    /// </summary>
    internal class Migration1_4_51008_1 : IMigrationStep
    {
        private string _versionString = "1.4.51008.1";

        public ExitCode Upgrade(MigrationInfo info)
        {
            //from here out we have a working configuration if something blows up.
            SqlCeConnectionStringBuilder builder = new SqlCeConnectionStringBuilder();
            string configName = "ConfigurationStore.sdf";
            builder.DataSource = info.GetWipFileNameCreateIfNeeded(configName);

            using (SqlCeConnection conn = new SqlCeConnection(builder.ToString()))
            {
                conn.Open();
                ApplySchemaChange(conn, info.WriteLine);
            }

            info.WriteLine(string.Format("ConfigurationStore updated to version {0}.", _versionString));
            
            return ExitCode.Success;
        }


        /// <summary>
        /// Actually change the schema 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="writeLine"> </param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "Local SQL only so no injection possible")]
        private void ApplySchemaChange(SqlCeConnection connection, Action<String> writeLine)
        {
            //NOTE: Move to only in memory transforms and dump to latest schema if we are not ok with losing "not null" status on columns in some cases
            //check Alter Table T-SQL restrictions.
            //unless we do only in memory transforms up to the latest version then right
            //we are going to have schema drift on null vs. not null columns

            // Connector 1.4 has the following changes:
            // - added non-nullable ConnectorPluginId column to PremiseConfigurations
            // - defaulted this new column to appropriate values for S300CRE (since that is the only pre-existing
            //   back office connection that should exist at this point)
            // - dropped BackOfficeProductPluginShimExePath
            // - dropped BackOfficeProductPluginPath
            string[] sql = new string[] 
            {
                @"ALTER TABLE [PremiseConfigurations] ADD [ConnectorPluginId] nvarchar(200) NULL",
                @"UPDATE [PremiseConfigurations] SET [ConnectorPluginId] = 'Sage300CRE'",
                @"ALTER TABLE [PremiseConfigurations] ALTER COLUMN [ConnectorPluginId] nvarchar(200) NOT NULL",
                @"ALTER TABLE [PremiseConfigurations] DROP COLUMN [BackOfficeProductPluginShimExePath]",
                @"ALTER TABLE [PremiseConfigurations] DROP COLUMN [BackOfficeProductPluginPath]",
            };

            foreach (string s in sql)
            {
                using (SqlCeCommand cmd = new SqlCeCommand())
                {
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = s;
                    cmd.Connection = connection;
                    cmd.ExecuteNonQuery();
                }
            }
            writeLine(string.Format("ConfigurationStore schema updated to  {0}.", _versionString));
        }
    }
}

