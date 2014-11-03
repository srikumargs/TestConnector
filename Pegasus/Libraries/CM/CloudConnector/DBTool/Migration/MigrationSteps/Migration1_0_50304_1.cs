using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;
using Sage.Connector.DBTool.Internal;
using Sage.Connector.DBTool.Migration;

namespace Sage.Connector.DBTool.Migration
{
    class Migration1_0_50304_1 : IMigrationStep
    {
        private string _versionString = "1.0.50304.1";

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

            string[] sql = new string[] 
            {
                @"ALTER TABLE [PremiseConfigurations] ADD [SiteAddress] nvarchar(4000) NULL",
                @"ALTER TABLE [PremiseConfigurations] DROP COLUMN [CloudEndpoint]",
                @"UPDATE [PremiseConfigurations]
                  SET [SiteAddress] = 'https://www.sageconstructionanywhere.com/' "
            };

        //dont let thsi go live..
//            string[] sql = new string[] 
//                        {
//                            @"ALTER TABLE [PremiseConfigurations] ADD [SiteAddress2] nvarchar(4000) NULL",
//                            @"ALTER TABLE [PremiseConfigurations] DROP COLUMN [SiteAddress]",
//                            @"UPDATE [PremiseConfigurations]
//                              SET [SiteAddress2] = 'https://www.sageconstructionanywhere.com/zoiks' "
//                        };

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
