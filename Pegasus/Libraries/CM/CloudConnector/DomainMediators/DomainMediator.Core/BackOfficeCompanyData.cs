using Newtonsoft.Json;
using Sage.Connector.DomainContracts.BackOffice;
using Sage.Connector.DomainContracts.Data;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Sage.Connector.DomainMediator.Core
{
    /// <summary>
    /// Class to help provide IBackOfficeCompanyData to the plugins
    /// </summary>
    /// <remarks>
    /// Consider if we want this internal and friended.
    /// </remarks>
    public class BackOfficeCompanyData : IBackOfficeCompanyData
    {
        /// <summary>
        /// BackOfficeId 
        /// </summary>
        public string BackOfficeId 
        { get; set; }

        /// <summary>
        /// ConnectionCredentials
        /// </summary>
        public IDictionary<string, string> ConnectionCredentials 
        { get; set; }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static IBackOfficeCompanyData FromConfiguration(IBackOfficeCompanyConfiguration configuration)
        {
            var data = new BackOfficeCompanyData();
            data.BackOfficeId = configuration.BackOfficeId;

            IDictionary<string, string> dictionary =
                JsonConvert.DeserializeObject<IDictionary<string, string>>(configuration.ConnectionCredentials);
            data.ConnectionCredentials = dictionary;
            
            return data;
        }
    }
}
