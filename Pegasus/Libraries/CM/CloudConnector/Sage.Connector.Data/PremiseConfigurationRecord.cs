using System;
using ConfigurationStore = Sage.Connector.PremiseStore.ConfigurationStore;

namespace Sage.Connector.Data
{
    /// <summary>
    /// Premise configuration record
    /// </summary>
    [Serializable]
    public class PremiseConfigurationRecord : IComparable
    {
        /// <summary>
        /// Internal Constructor
        /// </summary>
        internal PremiseConfigurationRecord()
        {
            _id = Guid.NewGuid();
            _premiseAgent = string.Empty;
            _tenantId = string.Empty;
            _premiseKey = string.Empty;
            _siteAddress = string.Empty;

            _backOfficeConnectionEnabledToReceive = true;
            _cloudConnectionEnabledToReceive = true;
            _cloudConnectionEnabledToSend = true;
            _cloudCompanyUrl = string.Empty;
            _cloudCompanyName = string.Empty;
            _backOfficeCompanyName = string.Empty;
            _backOfficeCompanyUniqueId = string.Empty;
            _backOfficeAllowableConcurrentExecutions = 1;

            _sentDocumentStoragePolicy = 0;
            _sentDocumentStorageDays = 0;
            _sentDocumentStorageMBs = 0;
            _sentDocumentFolderName = string.Empty;
            _connectorPluginId = string.Empty;
            _backOfficeProductName = string.Empty;
            _backOfficeConnectionCredentials = string.Empty;

            _backOfficeCompanyUniqueId = "Placeholder";
            
            _cloudTenantClaim = string.Empty;
            _backOfficeConnectionCredentialsDescription = string.Empty;
        }

        /// <summary>
        /// Internal Constructor
        /// </summary>
        /// <param name="pc"></param>
        internal PremiseConfigurationRecord(ConfigurationStore.PremiseConfiguration pc)
        {
            _id = pc.Id;
            _premiseAgent = pc.PremiseAgent;
            _tenantId = pc.CloudTenantId.ToLower();
            _premiseKey = pc.CloudPremiseKey;
            _siteAddress = pc.SiteAddress;
            _minCommunicationFailureRetryInterval = pc.MinCommunicationFailureRetryInterval;
            _maxCommunicationFailureRetryInterval = pc.MaxCommunicationFailureRetryInterval;

            _backOfficeConnectionEnabledToReceive = pc.BackOfficeConnectionEnabledToReceive;
            _cloudConnectionEnabledToReceive = pc.CloudConnectionEnabledToReceive;
            _cloudConnectionEnabledToSend = pc.CloudConnectionEnabledToSend;
            _cloudCompanyUrl = pc.CloudCompanyUrl;
            _cloudCompanyName = pc.CloudCompanyName;
            _backOfficeCompanyName = pc.BackOfficeCompanyName;
            
            _backOfficeAllowableConcurrentExecutions = pc.BackOfficeAllowableConcurrentExecutions;
            _sentDocumentStoragePolicy = pc.SentDocumentStoragePolicy;
            _sentDocumentStorageDays = pc.SentDocumentStorageDays;
            _sentDocumentStorageMBs = pc.SentDocumentStorageMBs;
            _sentDocumentFolderName = pc.SentDocumentFolderName;

            _connectorPluginId = pc.ConnectorPluginId;
            _backOfficeProductName = pc.BackOfficeProductName;

            _backOfficeConnectionCredentials = pc.BackOfficeConnectionCredentials;
            _backOfficeCompanyUniqueId = pc.BackOfficeCompanyUniqueId;

            _cloudTenantClaim = pc.CloudTenantClaim;

            _backOfficeConnectionCredentialsDescription = pc.BackOfficeConnectionCredentialsDescription;
        }

        /// <summary>
        /// Identifier
        /// </summary>
        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }
        private Guid _id;


        /// <summary>
        /// The premise agent string
        /// </summary>
        public string PremiseAgent
        {
            get { return _premiseAgent; }
            set { _premiseAgent = value; }
        }
        private string _premiseAgent;

        /// <summary>
        /// The tenant identifier
        /// </summary>
        public string CloudTenantId
        {
            get { return _tenantId; }
            set { _tenantId = value.ToLower(); }
        }
        private string _tenantId;

        /// <summary>
        /// Premise Key
        /// </summary>
        public string CloudPremiseKey
        {
            get { return _premiseKey; }
            set { _premiseKey = value; }
        }
        private string _premiseKey;

        /// <summary>
        /// Endpoint
        /// </summary>
        public String SiteAddress
        {
            get { return _siteAddress; }
            set { _siteAddress = value; }
        }
        private string _siteAddress;

        /// <summary>
        /// Should the back office connection be enabled to receive from the connector
        /// </summary>
        public Boolean BackOfficeConnectionEnabledToReceive
        {
            get { return _backOfficeConnectionEnabledToReceive; }
            set { _backOfficeConnectionEnabledToReceive = value; }
        }
        private Boolean _backOfficeConnectionEnabledToReceive;

        /// <summary>
        /// Should the cloud connection be enabled to receive from the cloud
        /// </summary>
        public Boolean CloudConnectionEnabledToReceive
        {
            get { return _cloudConnectionEnabledToReceive; }
            set { _cloudConnectionEnabledToReceive = value; }
        }
        private Boolean _cloudConnectionEnabledToReceive;

        /// <summary>
        /// Should the cloud connection be enabled to Send to the cloud
        /// </summary>
        public Boolean CloudConnectionEnabledToSend
        {
            get { return _cloudConnectionEnabledToSend; }
            set { _cloudConnectionEnabledToSend = value; }
        }
        private Boolean _cloudConnectionEnabledToSend;

        /// <summary>
        /// URL of the company in the cloud
        /// </summary>
        public String CloudCompanyUrl
        {
            get { return _cloudCompanyUrl; }
            set { _cloudCompanyUrl = value; }
        }
        private String _cloudCompanyUrl;

        /// <summary>
        /// Name of the company in the cloud
        /// </summary>
        public String CloudCompanyName
        {
            get { return _cloudCompanyName; }
            set { _cloudCompanyName = value; }
        }
        private String _cloudCompanyName;

        /// <summary>
        /// Name of the company in the back office
        /// </summary>
        public String BackOfficeCompanyName
        {
            get { return _backOfficeCompanyName; }
            set { _backOfficeCompanyName = value; }
        }
        private String _backOfficeCompanyName;

        /// <summary>
        /// Storage policy
        /// </summary>
        public Int16 SentDocumentStoragePolicy
        {
            get { return _sentDocumentStoragePolicy; }
            set { _sentDocumentStoragePolicy = value; }
        }
        private Int16 _sentDocumentStoragePolicy;

        /// <summary>
        /// Number of days to store sent documents
        /// </summary>
        public short SentDocumentStorageDays
        {
            get { return _sentDocumentStorageDays; }
            set { _sentDocumentStorageDays = value; }
        }
        private Int16 _sentDocumentStorageDays;

        /// <summary>
        /// Number of megabytes of sent document directory capacity
        /// </summary>
        public long SentDocumentStorageMBs
        {
            get { return _sentDocumentStorageMBs; }
            set { _sentDocumentStorageMBs = value; }
        }
        private Int64 _sentDocumentStorageMBs;

        /// <summary>
        /// Sent folder name
        /// </summary>
        public string SentDocumentFolderName
        {
            get { return _sentDocumentFolderName; }
            set { _sentDocumentFolderName = value; }
        }
        private String _sentDocumentFolderName;

        /// <summary>
        /// The number of concurrent back office binding executions for this tenant
        /// </summary>
        public Int16 BackOfficeAllowableConcurrentExecutions
        {
            get { return _backOfficeAllowableConcurrentExecutions; }
            set { _backOfficeAllowableConcurrentExecutions = value; }
        }
        private Int16 _backOfficeAllowableConcurrentExecutions;

        /// <summary>
        /// The minimum amount of time between polling attempts when the cloud is determined to be unavailable
        /// </summary>
        public Int32 MinCommunicationFailureRetryInterval
        {
            get { return _minCommunicationFailureRetryInterval; }
            set { _minCommunicationFailureRetryInterval = value; }
        }
        private Int32 _minCommunicationFailureRetryInterval;

        /// <summary>
        /// The maximum amount of time between polling attempts when the cloud is determined to be unavailable
        /// </summary>
        public Int32 MaxCommunicationFailureRetryInterval
        {
            get { return _maxCommunicationFailureRetryInterval; }
            set { _maxCommunicationFailureRetryInterval = value; }
        }
        private Int32 _maxCommunicationFailureRetryInterval;

        /// <summary>
        /// 
        /// </summary>
        public String ConnectorPluginId 
        {
            get { return _connectorPluginId; }
            set
            {
                if (_connectorPluginId != value)
                {
                    _connectorPluginId = value;
                }
            }
        }
        private String _connectorPluginId;

        /// <summary>
        /// 
        /// </summary>
        public String BackOfficeProductName
        {
            get { return _backOfficeProductName; }
            set { _backOfficeProductName = value; }
        }
        private String _backOfficeProductName;

        /// <summary>
        /// Get the BackOfficeConnectionCredentials
        /// Note this is in json format.
        /// </summary>
        public String BackOfficeConnectionCredentials
        {
            get { return _backOfficeConnectionCredentials; }
            set { _backOfficeConnectionCredentials = value; }
        }

        private string _backOfficeConnectionCredentials;

        /// <summary>
        /// Identifier that uniquely indicates a specific back office company.
        /// </summary>
        public String BackOfficeCompanyUniqueId
        {
            get { return _backOfficeCompanyUniqueId; }
            set { _backOfficeCompanyUniqueId = value; }
        }

        private string _backOfficeCompanyUniqueId;
        
        /// <summary>
        /// Persisted claim to supply to the cloud for web operations
        /// </summary>
        public String CloudTenantClaim
        {
            get { return _cloudTenantClaim; }
            set { _cloudTenantClaim = value; }
        }

        private string _cloudTenantClaim;


        /// <summary>
        /// Gets or sets the back office connection credentials description.
        /// </summary>
        /// <value>
        /// The back office connection credentials description.
        /// </value>
        public string BackOfficeConnectionCredentialsDescription
        {
            get { return _backOfficeConnectionCredentialsDescription; }
            set { _backOfficeConnectionCredentialsDescription = value; }
        }
        private string _backOfficeConnectionCredentialsDescription;

        #region IComparable Members

        /// <summary>
        /// Default comparison function
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            PremiseConfigurationRecord rec2 = obj as PremiseConfigurationRecord;

            if (rec2 == null)
            {
                // Second record is null, so consider it less than this instance
                return 1;
            }

            // Compare sorting field values
            return String.Compare(BackOfficeCompanyName, rec2.BackOfficeCompanyName, StringComparison.Ordinal);

        }

        #endregion
    }
}
