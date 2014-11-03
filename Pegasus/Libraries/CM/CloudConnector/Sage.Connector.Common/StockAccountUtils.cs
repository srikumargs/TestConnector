using System;
using System.Collections.Generic;

namespace Sage.Connector.Common
{
    /// <summary>
    /// Enumerate the possible stock accounts we can handle
    /// </summary>
    public enum KnownStockAccountType
    {
        /// <summary>
        /// Default initializer
        /// </summary>
        None = 0,

        /// <summary>
        /// Local system account
        /// </summary>
        LocalSystem,

        /// <summary>
        /// Local service account
        /// </summary>
        LocalService,

        /// <summary>
        /// Network service account
        /// </summary>
        NetworkService
    }

    /// <summary>
    /// Helper for stock account types
    /// </summary>
    public static class StockAccountUtils
    {
        /// <summary>
        /// Given a login string, find the matching stock account type, if any
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static KnownStockAccountType GetStockAccountFromLoginString(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                foreach (KnownStockAccountType key in PossibleStockAccountLoginStrings.Keys)
                {
                    foreach (string possibleLoginString in PossibleStockAccountLoginStrings[key])
                    {
                        if (possibleLoginString.Equals(value, StringComparison.InvariantCultureIgnoreCase))
                        {
                            // Found a match
                            return key;
                        }
                    }
                }
            }

            // No matching stock accounts
            return KnownStockAccountType.None;
        }

        /// <summary>
        /// Get the hosting framework param to use given the stock account type
        /// </summary>
        /// <param name="stockAccountType"></param>
        /// <returns></returns>
        public static string GetHostingFrameworkParamForStockAccountType(KnownStockAccountType stockAccountType)
        {
            string result = null;
            if (StockAccountToHostingFrameworkParam.ContainsKey(stockAccountType))
            {
                result = StockAccountToHostingFrameworkParam[stockAccountType];
            }

            return result;
        }

        /// <summary>
        /// The param to use for the hosting framework for each stock account
        /// </summary>
        private static readonly Dictionary<KnownStockAccountType, string> StockAccountToHostingFrameworkParam
            = new Dictionary<KnownStockAccountType, string>()
                  {
                      {KnownStockAccountType.LocalSystem, "localsystem"},
                      {KnownStockAccountType.LocalService, "localservice"},
                      {KnownStockAccountType.NetworkService, "networkservice"}
                  };

        /// <summary>
        /// Possible string values that should convert to a stock account type that we handle
        /// </summary>
        private static readonly Dictionary<KnownStockAccountType, IEnumerable<String>> PossibleStockAccountLoginStrings
            = new Dictionary<KnownStockAccountType, IEnumerable<string>>()
                  {
                      {KnownStockAccountType.LocalSystem, new string[] {"LocalSystem", "Local System"}},
                      {KnownStockAccountType.LocalService, new string[] {"LocalService", "Local Service", @"NT AUTHORITY\LocalService", @"NT AUTHORITY\Local Service"}},
                      {KnownStockAccountType.NetworkService, new string[] {"NetworkService", "Network Service", @"NT AUTHORITY\NetworkService", @"NT AUTHORITY\Network Service"}}
                  };
    }
}
