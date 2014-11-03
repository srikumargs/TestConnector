using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sage.Connector.DomainMediator.Core;

namespace SageConnector.Internal
{
    /// <summary>
    /// 
    /// </summary>
    public class FeatureMetadata : IFeatureMetaData
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="displayName"></param>
        /// <param name="interfaceName"></param>
        /// <param name="name"></param>
        public FeatureMetadata(string displayName, string interfaceName, string name)
        {
            Name = name;
            InterfaceName = interfaceName;
            DisplayName = displayName;
        }

        /// <summary>
        /// 
        /// </summary>
        public string DisplayName { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public string InterfaceName { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        public string Name { get; private set; }

    }
}
