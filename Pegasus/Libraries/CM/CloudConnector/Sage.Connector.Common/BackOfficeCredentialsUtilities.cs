using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Sage.Connector.Common
{
    /// <summary>
    /// 
    /// </summary>
    public static class BackOfficeCredentialsUtilities
    {
        /// <summary>
        /// Create Connection Credentials for use with Mock.
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userPassword"></param>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public static string CreateConnectionCredentialsForMock(string userName, string userPassword, string companyId)
        {
            //TODO: JSB remove any use not indented for mock.
            //Put the values for user name, password and back office connection info into object and then into json.
            dynamic connectionCredentials = new ExpandoObject();
            connectionCredentials.Credentials = new ExpandoObject();
            connectionCredentials.Credentials.UserId = userName;
            connectionCredentials.Credentials.Password = userPassword;

            if (companyId != null) 
            {
                connectionCredentials.Credentials.CompanyId = companyId;
            }

            string retval = JsonConvert.SerializeObject(connectionCredentials);
            return retval;
        }
    }
}
