using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Sage.Connector.Management
{
    /// <summary>
    /// 
    /// </summary>
    public static class DeveloperFlags
    {
        /// <summary>
        /// Should we show the end point address in the UI?
        /// </summary>
        /// <returns></returns>
        public static bool ShowEndPointAddress()
        {
            bool retval = false;
            try
            {
                string variableTest = Environment.GetEnvironmentVariable(SAGE_CONNECTOR_SHOW_ENDPOINT);
                if (!string.IsNullOrEmpty(variableTest) && variableTest.Equals("1"))
                {
                    retval = true;
                }
            }
            catch (SecurityException)
            {
                // Insufficient rights to read variable
            }
            return retval;
        }
        private const string SAGE_CONNECTOR_SHOW_ENDPOINT = "SAGE_CONNECTOR_SHOW_ENDPOINT";


        /// <summary>
        /// Is this a production deployment
        /// </summary>
        /// <returns></returns>
        public static bool IsNonProduction()
        {
            bool retval = false;
            try
            {
                string variableTest = Environment.GetEnvironmentVariable(SAGE_CONNECTOR_NON_PRODUCTION);
                if (!string.IsNullOrEmpty(variableTest) && variableTest.Equals("1"))
                {
                    retval = true;
                }
            }
            catch (SecurityException)
            {
                // Insufficient rights to read variable
            }
            return retval;
        }
        private const string SAGE_CONNECTOR_NON_PRODUCTION = "SAGE_CONNECTOR_NON_PRODUCTION";
    }
}
