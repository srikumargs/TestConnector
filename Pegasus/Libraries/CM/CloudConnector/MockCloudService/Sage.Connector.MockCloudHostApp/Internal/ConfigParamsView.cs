using System;

namespace Sage.Connector.MockCloudHostApp
{
    /// <summary>
    /// 
    /// </summary>
    public class ConfigParamsView
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        public ConfigParamsView(Sage.Connector.Cloud.Integration.Interfaces.WebAPI.Configuration configuration)
        {
            InitializeFromConfigParams(configuration);
        }

        private void InitializeFromConfigParams(Sage.Connector.Cloud.Integration.Interfaces.WebAPI.Configuration configuration)
        {
            ConfigurationBaseURI = configuration.ConfigurationBaseUri.ToString();
            ConfigurationResourcePath = configuration.ConfigurationResourcePath;
            RequestBaseURI = configuration.RequestBaseUri.ToString();
            RequestResourcePath = configuration.RequestResourcePath;
            ResponseBaseURI = configuration.RequestBaseUri.ToString();
            ResponseResourcePath = configuration.ResponseResourcePath;
            RequestUploadResourcePath = configuration.RequestUploadResourcePath;
            ResponseUploadResourcePath = configuration.ResponseUploadResourcePath;
            NotificationResourceURI = configuration.NotificationResourceUri.ToString();
            MinimumConnectorProductVersion = configuration.MinimumConnectorProductVersion;
            UpgradeConnectorProductVersion = configuration.UpgradeConnectorProductVersion;
            UpgradeConnectorProductDate = configuration.UpgradeConnectorPublicationDate.ToShortDateString();
            UpgradeConnectorDescription = configuration.UpgradeConnectorDescription;
            UpgradeConnectorLinkURI = configuration.UpgradeConnectorLinkUri.ToString();
            TenantPublicURI = configuration.TenantPublicUri.ToString();
            TenantName = configuration.TenantName;
            MaximumBlobSize = configuration.MaxBlobSize.ToString();
            LargeResponseSizeThreshold = configuration.LargeResponseSizeThreshold.ToString();
            SuggestedConnectorUptimeDuration = configuration.SuggestedMaxConnectorUptimeDuration.ToString();
            MinimumCommFailureRetryInterval = configuration.MinCommunicationFailureRetryInterval.ToString();
            MaximumCommFailureRetryInterval = configuration.MaxCommunicationFailureRetryInterval.ToString();
            IsDirty = false;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Sage.Connector.Cloud.Integration.Interfaces.WebAPI.Configuration GetUpdatedConfiguration()
        {
            return new Cloud.Integration.Interfaces.WebAPI.Configuration()
            {
                ConfigurationBaseUri = new Uri(ConfigurationBaseURI),
                ConfigurationResourcePath = ConfigurationResourcePath,
                RequestBaseUri = new Uri(RequestBaseURI),
                RequestResourcePath = RequestResourcePath,
                ResponseBaseUri = new Uri(ResponseBaseURI),
                ResponseResourcePath = ResponseResourcePath,
                RequestUploadResourcePath = RequestUploadResourcePath,
                ResponseUploadResourcePath = ResponseUploadResourcePath,
                NotificationResourceUri = new Uri(NotificationResourceURI),
                MinimumConnectorProductVersion = MinimumConnectorProductVersion,
                UpgradeConnectorProductVersion = UpgradeConnectorProductVersion,
                UpgradeConnectorPublicationDate = Convert.ToDateTime(UpgradeConnectorProductDate),
                UpgradeConnectorLinkUri = new Uri(UpgradeConnectorLinkURI),
                TenantName = TenantName,
                MaxBlobSize = Convert.ToUInt32(MaximumBlobSize),
                LargeResponseSizeThreshold = Convert.ToUInt32(LargeResponseSizeThreshold),
                SuggestedMaxConnectorUptimeDuration = GetValidTimespanFromString(SuggestedConnectorUptimeDuration),
                MinCommunicationFailureRetryInterval = GetValidTimespanFromString(MinimumCommFailureRetryInterval),
                MaxCommunicationFailureRetryInterval = GetValidTimespanFromString(MaximumCommFailureRetryInterval)
            };
        }


        private TimeSpan GetValidTimespanFromString(string value)
        {
            TimeSpan ts;
            try { ts = TimeSpan.Parse(value); }
            catch { ts = new TimeSpan(0); }
            return ts;
        }

        private string GetValidStringFromURIString(string uriCandidate, bool isRequired = true)
        {
            if (String.IsNullOrEmpty(uriCandidate))
                return String.Empty;

            Uri trialUri = null;
            if (Uri.TryCreate(uriCandidate, UriKind.Absolute, out trialUri))
                return trialUri.ToString();

            return String.Empty;
        }

        private string GetValidStringFromVersionString(string versionCandidate, bool isRequired = true)
        {
            if (String.IsNullOrEmpty(versionCandidate))
                return String.Empty;

            Version trialVersion = null;
            if (Version.TryParse(versionCandidate, out trialVersion))
                return trialVersion.ToString();

            return String.Empty;
        }

        private string GetValidStringFromDateString(string dateCandidate, bool isRequired = true)
        {
            if (String.IsNullOrEmpty(dateCandidate))
                return String.Empty;

            DateTime trialDate;
            if (DateTime.TryParse(dateCandidate, out trialDate))
                return trialDate.Date.ToShortDateString();

            return String.Empty;
        }

        private string GetValidStringFromWholeNumberString(string wholeNumberCandidate, bool isRequired = true)
        {
            if (String.IsNullOrEmpty(wholeNumberCandidate))
                return String.Empty;

            ulong trialNumber;
            if (UInt64.TryParse(wholeNumberCandidate, out trialNumber))
                return trialNumber.ToString();

            return String.Empty;
        }

        private string GetValidStringFromTimespanString(string timeSpanCandididate, bool isRequired = true)
        {
            if (String.IsNullOrEmpty(timeSpanCandididate))
                return String.Empty;

            TimeSpan trialTimeSpan;
            if (TimeSpan.TryParse(timeSpanCandididate, out trialTimeSpan))
                return trialTimeSpan.ToString();

            return String.Empty;
        }


        private string _configurationBaseURI;
        /// <summary>
        /// 
        /// </summary>
        public string ConfigurationBaseURI
        {
            get { return _configurationBaseURI; }
            set {
                _configurationBaseURI = GetValidStringFromURIString(value);
                IsDirty = true;
            }
        }

        private string _configurationResourcePath;
        /// <summary>
        /// 
        /// </summary>
        public string ConfigurationResourcePath
        {
            get { return _configurationResourcePath; }
            set
            {
                _configurationResourcePath = value;
                IsDirty = true;
            }
        }

        private string _requestBaseURI;
        /// <summary>
        /// 
        /// </summary>
        public string RequestBaseURI
        {
            get { return _requestBaseURI; }
            set
            {
                _requestBaseURI = GetValidStringFromURIString(value);
                IsDirty = true;
            }
        }

        private string _requestResourcePath;
        /// <summary>
        /// 
        /// </summary>
        public string RequestResourcePath
        {
            get { return _requestResourcePath; }
            set
            {
                _requestResourcePath = value;
                IsDirty = true;
            }
        }

        private string _responseBaseURI;
        /// <summary>
        /// 
        /// </summary>
        public string ResponseBaseURI {
            get { return _responseBaseURI; }
            set
            {
                _responseBaseURI = GetValidStringFromURIString(value);
                IsDirty = true;
            }
        }

        private string _responseResourcePath;
        /// <summary>
        /// 
        /// </summary>
        public string ResponseResourcePath
        {
            get { return _responseResourcePath; }
            set
            {
                _responseResourcePath = value;
                IsDirty = true;
            }
        }

        private string _requestUploadResourcePath;
        /// <summary>
        /// 
        /// </summary>
        public string RequestUploadResourcePath
        {
            get { return _requestUploadResourcePath; }
            set
            {
                _requestUploadResourcePath = value;
                IsDirty = true;
            }
        }

        private string _responseUploadResourcePath;
        /// <summary>
        /// 
        /// </summary>
        public string ResponseUploadResourcePath
        {
            get { return _responseUploadResourcePath; }
            set
            {
                _responseUploadResourcePath = value;
                IsDirty = true;
            }
        }

        private string _notificationResourceURI;
        /// <summary>
        /// 
        /// </summary>
        public string NotificationResourceURI
        {
            get { return _notificationResourceURI; }
            set
            {
                _notificationResourceURI = GetValidStringFromURIString(value);
                IsDirty = true;
            }
        }

        private string _minimumConnectorProductVersion;
        /// <summary>
        /// 
        /// </summary>
        public string MinimumConnectorProductVersion
        {
            get { return _minimumConnectorProductVersion;  }
            set
            {
                _minimumConnectorProductVersion = GetValidStringFromVersionString(value);
                IsDirty = true;
            }
        }

        private string _upgradeConnectorProductVersion;
        /// <summary>
        /// 
        /// </summary>
        public string UpgradeConnectorProductVersion
        {
            get { return _upgradeConnectorProductVersion; }
            set
            {
                _upgradeConnectorProductVersion = GetValidStringFromVersionString(value);
                IsDirty = true;
            }
        }

        private string _upgradeConnectorProductDate;
        /// <summary>
        /// 
        /// </summary>
        public string UpgradeConnectorProductDate
        {
            get { return _upgradeConnectorProductDate; }
            set
            {
                _upgradeConnectorProductDate = GetValidStringFromDateString(value);
                IsDirty = true;
            }
        }

        private string _upgradeConnectorDescription;
        /// <summary>
        /// 
        /// </summary>
        public string UpgradeConnectorDescription
        {
            get { return _upgradeConnectorDescription; }
            set
            {
                _upgradeConnectorDescription = value;
                IsDirty = true;
            }
        }

        private string _upgradeConnectorLinkURI;
        /// <summary>
        /// 
        /// </summary>
        public string UpgradeConnectorLinkURI
        {
            get { return _upgradeConnectorLinkURI; }
            set
            {
                _upgradeConnectorLinkURI = GetValidStringFromURIString(value);
                IsDirty = true;
            }
        }

        private string _tenantPublicURI;
        /// <summary>
        /// 
        /// </summary>
        public string TenantPublicURI
        {
            get { return _tenantPublicURI; }
            set
            {
                _tenantPublicURI = GetValidStringFromURIString(value);
                IsDirty = true;
            }
        }

        private string _tenantName;
        /// <summary>
        /// 
        /// </summary>
        public string TenantName
        {
            get { return _tenantName; }
            set
            {
                _tenantName = value;
                IsDirty = true;
            }
        }

        private string _maximumBlobSize;
        /// <summary>
        /// 
        /// </summary>
        public string MaximumBlobSize
        {
            get { return _maximumBlobSize; }
            set
            {
                _maximumBlobSize = GetValidStringFromWholeNumberString(value);
                IsDirty = true;
            }
        }

        private string _largeResponseSizeThreshold;
        /// <summary>
        /// 
        /// </summary>
        public string LargeResponseSizeThreshold
        {
            get { return _largeResponseSizeThreshold; }
            set
            {
                _largeResponseSizeThreshold = GetValidStringFromWholeNumberString(value);
                IsDirty = true;
            }
        }

        private string _suggestedConnectorUptimeDuration;
        /// <summary>
        /// 
        /// </summary>
        public string SuggestedConnectorUptimeDuration
        {
            get { return _suggestedConnectorUptimeDuration; }
            set
            {
                _suggestedConnectorUptimeDuration = GetValidStringFromTimespanString(value);
                IsDirty = true;
            }
        }

        private string _minimumCommFailureRetryInterval;
        /// <summary>
        /// 
        /// </summary>
        public string MinimumCommFailureRetryInterval
        {
            get { return _minimumCommFailureRetryInterval; }
            set
            {
                _minimumCommFailureRetryInterval = GetValidStringFromTimespanString(value);
                IsDirty = true;
            }
        }

        private string _maximumCommFailureRetryInterval;
        /// <summary>
        /// 
        /// </summary>
        public string MaximumCommFailureRetryInterval
        {
            get { return _maximumCommFailureRetryInterval; }
            set
            {
                _maximumCommFailureRetryInterval = GetValidStringFromTimespanString(value);
                IsDirty = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsDirty { get; set; }
    }
}
