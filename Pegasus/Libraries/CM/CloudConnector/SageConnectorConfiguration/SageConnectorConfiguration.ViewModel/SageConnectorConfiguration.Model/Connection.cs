using System;

namespace SageConnectorConfiguration.Model
{
    /// <summary>
    /// Back office connection information
    /// </summary>
    public class BackOfficeConnection
    {
        /// <summary>
        /// The name of the connection
        /// </summary>
        public String Name { get; set; }
        /// <summary>
        /// The display name of the connection
        /// </summary>
        public String DisplayName { get; set; }
        /// <summary>
        /// The internal representation of the connection
        /// </summary>
        public String ConnectionInformation { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Connection
    {
        /// <summary>
        /// 
        /// </summary>
        public Connection()
        {
            ConnectorInstalled = false;
            IsLocalSystemAccount = true;
            InstallMachine = System.Net.Dns.GetHostEntry(System.Environment.MachineName).HostName;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ConnectorInstalled { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public String InstallMachine { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsUserAccount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsLocalSystemAccount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string WindowsUserAccount { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string WindowsPassword { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TenantName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string TenantURL { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string BODataFolder { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BOID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BOAdminName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BOAdminPassword { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BOUserName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BOUserPassword { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ConfigurationLocation { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ConnectionKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string CurrentPackageVersion { get; set; }

        /// <summary>
        /// Translate file / directory specification to
        /// file path to the configuration XML file
        /// </summary>
        public string ConfigurationFileLocation
        {
            get
            {
                if (!String.IsNullOrEmpty(ConfigurationLocation))
                {
                    if (System.IO.File.Exists(ConfigurationLocation))
                        return ConfigurationLocation;

                    if (System.IO.Directory.Exists(ConfigurationLocation))
                        return System.IO.Path.Combine(ConfigurationLocation, "SageConnectorConfiguration.xml");
                }

                return String.Empty;
            }
        }

        /// <summary>
        /// The derived effective user (real or preconfigured)
        /// </summary>
        public String EffectiveUser
        {
            get
            {
                if (IsUserAccount)
                    return WindowsUserAccount;

                if (IsLocalSystemAccount)
                    return "localsystem";

                return String.Empty;
            }
        }

        /// <summary>
        /// The derived effective password (real or blank)
        /// </summary>
        public String EffectivePassword
        {
            get
            {
                if (IsUserAccount)
                    return WindowsPassword;
                return String.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public BackOfficeConnection SelectedConnection { get; set; }

    }
}
