using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Sage.Connector.Cloud.Integration.Interfaces.DataContracts;
using Sage.Connector.Cloud.Integration.Interfaces.Responses;
using Sage.Connector.Cloud.Integration.Interfaces.Utils;
using Sage.Connector.LinkedSource;

namespace Sage.Connector.ConnectorServiceCommon
{
    /// <summary>
    /// Class defining what data is needed to carry out a file upload
    /// Prior to sending a response
    /// </summary>
    [DataContract(Namespace = ServiceConstants.V1_SERVICE_NAMESPACE, Name = "ResponseWrapperUploadSpecificationContract")]
    public sealed class ResponseWrapperUploadSpecification
    {
        #region Constructor

        /// <summary>
        /// Fully-sepecified constructor
        /// </summary>
        /// <param name="idPropertyName"></param>
        /// <param name="fileName"></param>
        public ResponseWrapperUploadSpecification(string idPropertyName, string fileName)
        {
            IdPropertyName = idPropertyName;
            FileName = fileName;
        }

        /// <summary>
        /// Mutable constructor
        /// </summary>
        /// <param name="source"></param>
        /// <param name="propertyTuples"></param>
        public ResponseWrapperUploadSpecification(ResponseWrapperUploadSpecification source, IEnumerable<PropertyTuple> propertyTuples)
        {
            IdPropertyName = source.IdPropertyName;
            FileName = source.FileName;

            var myPropertyTuples = propertyTuples.Where(x => x.Item1.DeclaringType == typeof(ResponseWrapperUploadSpecification));
            foreach (var tuple in myPropertyTuples)
            {
                tuple.Item1.SetValue(this, tuple.Item2, null);
            }
        }

        #endregion


        #region Properties

        /// <summary>
        /// The property that will store the upload Id resulting from the file upload
        /// Would prefer to use a PropertyInfo object here, but that cannot be serialized
        /// Using the data contract serializer
        /// </summary>
        [DataMember(Name = "IdPropertyInfo", IsRequired = true, Order = 0)]
        public string IdPropertyName { get; private set; }

        /// <summary>
        /// The full file name of the file to be uploaded
        /// </summary>
        [DataMember(Name = "FileName", IsRequired = true, Order = 2)]
        public string FileName { get; private set; }

        #endregion
    }


    /// <summary>
    /// Wrapper class for the response making it to the outbound queue.
    /// Additional data currently is a list of files to upload to the cloud along with
    /// The properties to fill with the resulting upload Ids
    /// </summary>
    [DataContract(Namespace = ServiceConstants.V1_SERVICE_NAMESPACE, Name = "ResponseWrapperContract")]
    public class ResponseWrapper : IExtensibleDataObject
    {
        #region Constructors

        /// <summary>
        /// Constructor from request wrapper and reponse
        /// </summary>
        /// <param name="originatingRequestWrapper"></param>
        /// <param name="response"></param>
        /// <param name="uploadSpecifications"></param>
        public ResponseWrapper(RequestWrapper originatingRequestWrapper, Response response,
            ResponseWrapperUploadSpecification[] uploadSpecifications)
        {
            ActivityTrackingContext = originatingRequestWrapper.ActivityTrackingContext;
            OriginalRequestPayload = originatingRequestWrapper.RequestPayload;
            ResponseId = response.Id;
            ResponseType = response.GetType().FullName;
            ResponsePayload = Utils.JsonSerialize(response);
            Uploads = uploadSpecifications;
        }

        /// <summary>
        /// Constructor from request wrapper
        /// </summary>
        /// <param name="originatingRequestWrapper"></param>
        /// <param name="responseId"></param>
        /// <param name="responseType"></param>
        /// <param name="responsePayload"></param>
        /// <param name="uploadSpecifications"></param>
        public ResponseWrapper(RequestWrapper originatingRequestWrapper, Guid responseId, String responseType,
            String responsePayload, ResponseWrapperUploadSpecification[] uploadSpecifications)
        {
            ActivityTrackingContext = originatingRequestWrapper.ActivityTrackingContext;
            OriginalRequestPayload = originatingRequestWrapper.RequestPayload;
            ResponseId = responseId;
            ResponseType = responseType;
            ResponsePayload = responsePayload;
            Uploads = uploadSpecifications;
        }

        /// <summary>
        /// Fully specified constructor
        /// </summary>
        /// <param name="trackingContext"></param>
        /// <param name="originalRequestPayload"></param>
        /// <param name="responseId"></param>
        /// <param name="responseType"></param>
        /// <param name="responsePayload"></param>
        /// <param name="uploadSpecifications"></param>
        public ResponseWrapper(ActivityTrackingContext trackingContext, String originalRequestPayload, Guid responseId, String responseType, String responsePayload, ResponseWrapperUploadSpecification[] uploadSpecifications)
        {
            ActivityTrackingContext = trackingContext;
            OriginalRequestPayload = originalRequestPayload;
            ResponseId = responseId;
            ResponseType = responseType;
            ResponsePayload = responsePayload;
            Uploads = uploadSpecifications;
        }

        /// <summary>
        /// Mutable constructor
        /// </summary>
        /// <param name="source"></param>
        /// <param name="propertyTuples"></param>
        public ResponseWrapper(ResponseWrapper source, IEnumerable<PropertyTuple> propertyTuples)
        {
            ActivityTrackingContext = source.ActivityTrackingContext;
            OriginalRequestPayload = source.OriginalRequestPayload;
            ResponseId = source.ResponseId;
            ResponseType = source.ResponseType;
            ResponsePayload = source.ResponsePayload;
            Uploads = source.Uploads;
            UploadSessions = source.UploadSessions;
            ExtensionData = source.ExtensionData;

            var myPropertyTuples = propertyTuples.Where(x => x.Item1.DeclaringType == typeof(ResponseWrapper));
            foreach (var tuple in myPropertyTuples)
            {
                tuple.Item1.SetValue(this, tuple.Item2, null);
            }
        }

        #endregion


        #region Properties
        /// <summary>
        /// 
        /// </summary>
        [DataMember(Name = "ActivityTrackingContext", IsRequired = true, Order = 0)]
        public ActivityTrackingContext ActivityTrackingContext { get; protected set; }

        /// <summary>
        /// The original request that produced this response
        /// </summary>
        [DataMember(Name = "OriginalRequestPayload", IsRequired = true, Order = 1)]
        public String OriginalRequestPayload { get; protected set; }

        /// <summary>
        /// The identifier of the response
        /// </summary>
        [DataMember(Name = "ResponseId", IsRequired = true, Order = 2)]
        public Guid ResponseId { get; protected set; }

        /// <summary>
        /// The type of the response
        /// </summary>
        [DataMember(Name = "ResponseType", IsRequired = true, Order = 3)]
        public String ResponseType { get; protected set; }

        /// <summary>
        /// The actual response
        /// </summary>
        [DataMember(Name = "ResponsePayload", IsRequired = true, Order = 4)]
        public String ResponsePayload { get; protected set; }

        /// <summary>
        /// A collection of files that need to be uploaded before sending the response
        /// Along with the fields that need to be populated with the Ids resulting from
        /// The uploads
        /// </summary>
        [DataMember(Name = "Uploads", IsRequired = true, Order = 5)]
        public ResponseWrapperUploadSpecification[] Uploads { get; protected set; }

        /// <summary>
        /// A collection of upload sessions succesfully uploaded to the cloud
        /// </summary>
        [DataMember(Name = "UploadSessions", IsRequired = true, Order = 6)]
        public UploadSessionInfo[] UploadSessions { get; set; }
        #endregion


        #region IExtensibleDataObject Members

        /// <summary>
        /// To support forward-compatible data contracts
        /// </summary>
        public ExtensionDataObject ExtensionData { get; set; }

        #endregion
    }
}
