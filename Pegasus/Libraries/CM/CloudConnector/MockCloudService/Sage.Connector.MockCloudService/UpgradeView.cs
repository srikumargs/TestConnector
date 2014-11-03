using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sage.Connector.SageCloudService
{
    /// <summary>
    /// 
    /// </summary>
    public class UpgradeView
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="prodCurVer"></param>
        /// <param name="prodMinVer"></param>
        /// <param name="intCurVer"></param>
        /// <param name="intMinVer"></param>
        /// <param name="upgradeAvailableDesc"></param>
        /// <param name="upgradeRequiredDesc"></param>
        /// <param name="upgradeAvailableLink"></param>
        /// <param name="upgradeRequiredLink"></param>
        /// <param name="upgradeAvailableDate"></param>
        /// <param name="upgradeRequiredDate"></param>
        public UpgradeView(string prodCurVer, string prodMinVer, string intCurVer, string intMinVer, string upgradeAvailableDesc, string upgradeRequiredDesc, string upgradeAvailableLink, string upgradeRequiredLink, DateTime upgradeAvailableDate, DateTime upgradeRequiredDate)
        {
            CurrentProductVersion = prodCurVer;
            CurrentInterfaceVersion = intCurVer;
            UpgradeAvailableDate = upgradeAvailableDate;
            UpgradeAvailableDescription = upgradeAvailableDesc;
            UpgradeAvailableLink = upgradeAvailableLink;
            MinProductVersion = prodMinVer;
            MinInterfaceVersion = intMinVer;
            UpgradeRequiredDate = upgradeRequiredDate;
            UpgradeRequiredDescription = upgradeRequiredDesc;
            UpgradeRequiredLink = upgradeRequiredLink;
        }
        /// <summary>
        /// 
        /// </summary>
        public string CurrentProductVersion
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MinProductVersion
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string CurrentInterfaceVersion
        { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MinInterfaceVersion
        { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string UpgradeAvailableDescription
        { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string UpgradeAvailableLink
        { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime UpgradeAvailableDate
        { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string UpgradeRequiredDescription
        { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string UpgradeRequiredLink
        { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime UpgradeRequiredDate
        { get; set; }

    }
}
