using System;
using System.Text;
using Sage.Connector.DomainContracts.BackOffice;

namespace Sage.Connector.ProcessExecution.RequestActivator
{
    /// <summary>
    /// Back office configuration object to pass to the back office.
    /// </summary>
    public class BackOfficeCompanyConfigurationObject : IBackOfficeCompanyConfiguration
    {
        /// <summary>
        /// Back Office PluginId
        /// </summary>
        public string BackOfficeId { get; set; }

        /// <summary>
        /// ConnectionCredentials
        /// </summary>
        public string ConnectionCredentials { get; set; }

        /// <summary>
        /// Gets the data path.
        /// This is where the DM can store files for the tenant/request
        /// </summary>
        /// <value>
        /// The data path base.
        /// </value>
        /// <remarks>
        /// This value if populated is the data storage path available to the DM layer.
        /// </remarks>
        public string DataStoragePath { get; set; }


        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            const string spacer = " | ";
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("BackOfficeId: {0}", BackOfficeId ?? string.Empty);
            sb.Append(spacer);
            sb.AppendFormat("ConnectionCredentials: {0}", ConnectionCredentials ?? String.Empty);
            sb.Append(spacer);
            sb.AppendFormat("DataStoragePath: {0}", DataStoragePath ?? string.Empty);
            sb.AppendLine();
            String retval = sb.ToString();
            return retval;
        }
    }
}
