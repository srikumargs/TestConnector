using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Sage.Connector.Common;
using Sage.Connector.LinkedSource;

namespace Sage.Connector.StateService.Interfaces.DataContracts
{
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Namespace = ServiceConstants.V1_SERVICE_NAMESPACE, Name = "UpdateInfoContract")]
    public class UpdateInfo : IExtensibleDataObject
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the UpdateInfo class
        /// </summary>
        /// <param name="productVersion"></param>
        /// <param name="publicationDate"></param>
        /// <param name="updateDescription"></param>
        /// <param name="updateLinkUri"></param>
        public UpdateInfo(String productVersion, DateTime publicationDate, String updateDescription, Uri updateLinkUri)
        {
            ProductVersion = productVersion;
            PublicationDate = publicationDate;
            UpdateDescription = updateDescription;
            UpdateLinkUri = updateLinkUri;
        }
 
                /// <summary>
        /// Initializes a new instance of the UpdateInfo class from an existing instance and a collection of propertyTuples
        /// </summary>
        /// <param name="source"></param>
        /// <param name="propertyTuples"></param>
        public UpdateInfo(UpdateInfo source, IEnumerable<PropertyTuple> propertyTuples)
        {
            ProductVersion = source.ProductVersion;
            PublicationDate = source.PublicationDate;
            UpdateDescription = source.UpdateDescription;
            UpdateLinkUri = source.UpdateLinkUri;
            ExtensionData = source.ExtensionData;

            var myPropertyTuples = propertyTuples.Where(x => x.Item1.DeclaringType == typeof(UpdateInfo));
            foreach (var tuple in myPropertyTuples)
            {
                tuple.Item1.SetValue(this, tuple.Item2, null);
            }
        }
        #endregion


        #region Public properties
        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "ProductVersion", IsRequired = true, Order = 0)]
        public String ProductVersion { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "PublicationDate", IsRequired = true, Order = 1)]
        public DateTime PublicationDate { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "UpdateDescription", IsRequired = true, Order = 3)]
        public String UpdateDescription { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "UpdateLinkUri", IsRequired = true, Order = 4)]
        public Uri UpdateLinkUri { get; protected set; }

        /// <summary>
        /// To support forward-compatible data contracts
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }
        #endregion
    }
}
