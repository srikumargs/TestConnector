using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sage.Ssdp.Security.Client;

namespace Sage.Connector.AutoUpdate.Addin
{
    class LocalSoftwareUpdate : ISoftwareUpdate
    {
        public string BundleName { get; set; }
        public string UpdateId { get; set; }
        public string ProductId { get; set; }
        public string VersionId { get; set; }
        public string UpdateTypeId { get; set; }
        public string FileName { get; set; }
        public long FileLength { get; set; }
        public string MimeType { get; set; }
        public string FriendlyName { get; set; }
        public string Description { get; set; }
        public IAttribute[] Attributes { get; set; }
        public IDependency[] Dependencies { get; set; }
    }
}
