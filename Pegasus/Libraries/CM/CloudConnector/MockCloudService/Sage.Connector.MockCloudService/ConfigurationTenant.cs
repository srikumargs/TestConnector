using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using CloudContracts = Sage.Connector.Cloud.Integration.Interfaces.DataContracts;

namespace Sage.Connector.SageCloudService
{


    /// <summary>
    /// Root item.  Collection of Tenants
    /// </summary>
    [Serializable()]
    [XmlRoot("LoadTestTenantCollection")]
    public class ConfigurationTenants
    {
        /// <summary>
        /// 
        /// </summary>
        [XmlArray("Tenants")]
        [XmlArrayItem("Tenant", typeof(ConfigurationTenant))]
        public ConfigurationTenant[] Tenants { get; set; }
    }

    /// <summary>
    /// Individual Tenant configuration.
    /// </summary>
    [Serializable()]
    public class ConfigurationTenant
    {
        private string _tenantId;

        /// <summary>
        /// Endpoint address for this tenant
        /// </summary>
        [System.Xml.Serialization.XmlElement("EndpointAddress")]
        public string EndpointAddress { get; set; }

        /// <summary>
        /// Tenant Id
        /// </summary>
        [System.Xml.Serialization.XmlElement("TenantId")]
        public string TenantId
        {
            get { return _tenantId.ToLower(); }
            set { _tenantId = value.ToLower();  }
        }

        /// <summary>
        /// Premise Key
        /// </summary>
        [System.Xml.Serialization.XmlElement("PremiseKey")]
        public string PremiseKey { get; set; }

        /// <summary>
        /// Test Duration in minutes
        /// </summary>
        [System.Xml.Serialization.XmlElement("TestTime")]
        public double TestTime { get; set; }

        /// <summary>
        /// Delay in seconds, between continued requests
        /// </summary>
        [System.Xml.Serialization.XmlElement("RequestDelay")]
        public double RequestDelay { get; set; }

        /// <summary>
        /// Collection of ConfigurationRequests that will be bulk entered to the queue on startup
        /// </summary>
        [XmlArray("InitialRequests")]
        [XmlArrayItem("Request", typeof(ConfigurationRequest))]
        public ConfigurationRequest[] InitialRequests { get; set; }

        /// <summary>
        /// Collection of ConfigurationRequests that will be looped through and sent with a RequestDelay in between.
        /// </summary>
        [XmlArray("ContinuedRequests")]
        [XmlArrayItem("Request", typeof(ConfigurationRequest))]
        public ConfigurationRequest[] ContinuedRequests { get; set; }

        /// <summary>
        /// Configuration for updating client 
        /// </summary>
        [System.Xml.Serialization.XmlElement("UpdateConfigurationParameters")]
        public UpdateConfigurationParams UpdateConfigurationParameters { get; set; }
    }

    /// <summary>
    /// Configuration parameters
    /// </summary>
    [Serializable()]
    public class UpdateConfigurationParams
    {
        /// <summary>
        /// Base URI to retrieve configuration information
        /// </summary>
        [XmlElement("ConfigurationBaseUri")]
        public String ConfigurationBaseUriString
        {
            get { return ConfigurationBaseUri.ToString(); }
            set { ConfigurationBaseUri = new Uri(value); }
        }
        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore]
        public Uri ConfigurationBaseUri { get; set; }

        /// <summary>
        /// URI Path to configuration information
        /// </summary>
        [XmlElement("ConfigurationResourcePath")]
        public String ConfigurationResourcePath { get; set; }

        /// <summary>
        /// Base URI to get requests
        /// </summary>
        [XmlElement("RequestBaseUri")]
        public String RequestBaseUriString
        {
            get { return RequestBaseUri.ToString(); }
            set { RequestBaseUri = new Uri(value); }
        }
        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore]
        public Uri RequestBaseUri { get; set; }

        /// <summary>
        /// URI Path to get requests
        /// </summary>
        [XmlElement("RequestResourcePath")]
        public String RequestResourcePath { get; set; }

        /// <summary>
        /// Base URI to post responses
        /// </summary>
        [XmlElement("ResponseBaseUri")]
        public String ResponseBaseUriString
        {
            get { return ResponseBaseUri.ToString(); }
            set { ResponseBaseUri = new Uri(value); }
        }
        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore]
        public Uri ResponseBaseUri { get; set; }

        /// <summary>
        /// URI Path to post responses
        /// </summary>
        [XmlElement("ResponseResourcePath")]
        public String ResponseResourcePath { get; set; }

        /// <summary>
        /// URI Path to post request for upload location
        /// </summary>
        [XmlElement("RequestUploadResourcePath")]
        public String RequestUploadResourcePath { get; set; }
        /// <summary>
        /// URI Path to post upload completion
        /// </summary>
        [XmlElement("ResponseUploadResourcePath")]
        public String ResponseUploadResourcePath { get; set; }

        /// <summary>
        /// URI Path to persistent request notification channel
        /// </summary>
        [XmlElement("NotificationResourceUri")]
        public String NotificationResourceUriString
        {
            get { return NotificationResourceUri.ToString(); }
            set { NotificationResourceUri = new Uri(value); }
        }
        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore]
        public Uri NotificationResourceUri { get; set; }

        /// <summary>
        /// The minimum connector supported connector version string
        /// </summary>
        [XmlElement("MinimumConnectorProductVersion")]
        public String MinimumConnectorProductVersion { get; set; }

        /// <summary>
        /// The newest available connector product version
        /// </summary>
        [XmlElement("UpgradeConnectorProductVersion")]
        public String UpgradeConnectorProductVersion { get; set; }

        /// <summary>
        /// The publication date of the newest connector
        /// </summary>
        [XmlElement("UpgradeConnectorPublicationDateString")]
        public String UpgradeConnectorPublicationDateString
        {
            get { return UpgradeConnectorPublicationDate.ToShortDateString(); }
            set { UpgradeConnectorPublicationDate = Convert.ToDateTime(value); }
        }
        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore]
        public DateTime UpgradeConnectorPublicationDate { get; set; }

        /// <summary>
        /// The feature of the newest connector
        /// </summary>
        [XmlElement("UpgradeConnectorDescription")]
        public String UpgradeConnectorDescription { get; set; }

        /// <summary>
        /// The link to the connector upgrade
        /// </summary>
        [XmlElement("UpgradeConnectorLinkUri")]
        public String UpgradeConnectorLinkUriString
        {
            get { return UpgradeConnectorLinkUri.ToString(); }
            set { UpgradeConnectorLinkUri = new Uri(value); }
        }
        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore]
        public Uri UpgradeConnectorLinkUri { get; set; }

        /// <summary>
        /// The address to the core web site
        /// </summary>
        [XmlElement("SiteAddressBaseUri")]
        public String SiteAddressBaseUriString
        {
            get { return SiteAddressBaseUri.ToString(); }
            set { SiteAddressBaseUri = new Uri(value); }
        }
        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore]
        public Uri SiteAddressBaseUri { get; set; }

        /// <summary>
        /// The address to the tenant web site
        /// </summary>
        [XmlElement("TenantPublicUri")]
        public String TenantPublicUriString { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore]
        public Uri TenantPublicUri { get; set; }

        /// <summary>
        /// The name of the tenant
        /// </summary>
        [XmlElement("TenantName")]
        public String TenantName { get; set; }

        /// <summary>
        /// The maximum upload blob size
        /// </summary>
        [XmlElement("MaxBlobSize")]
        public UInt32 MaxBlobSize { get; set; }

        /// <summary>
        /// The threshold to switch to blob response uploads
        /// </summary>
        [XmlElement("LargeResponseSizeThreshold")]
        public UInt32 LargeResponseSizeThreshold { get; set; }

        /// <summary>
        /// The suggested connector uptime interval
        /// </summary>
        [XmlElement("SuggestedMaxConnectorUptimeDuration")]
        public String SuggestedMaxConnectorUptimeDurationString
        {
            get { return SuggestedMaxConnectorUptimeDuration.ToString(); }
            set { SuggestedMaxConnectorUptimeDuration = TimeSpan.Parse(value); }
        }
        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore]
        public TimeSpan SuggestedMaxConnectorUptimeDuration { get; set; }

        /// <summary>
        /// The minumum wait period before retrying communication failure
        /// </summary>
        [XmlElement("MinCommunicationFailureRetryInterval")]
        public String MinCommunicationFailureRetryIntervalString
        {
            get { return MinCommunicationFailureRetryInterval.ToString(); }
            set { MinCommunicationFailureRetryInterval = TimeSpan.Parse(value); }
        }
        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore]
        public TimeSpan MinCommunicationFailureRetryInterval { get; set; }

        /// <summary>
        /// The maximum wait period before retrying communication failure
        /// </summary>
        [XmlElement("MaxCommunicationFailureRetryInterval")]
        public String MaxCommunicationFailureRetryIntervalString
        {
            get { return MaxCommunicationFailureRetryInterval.ToString(); }
            set { MaxCommunicationFailureRetryInterval = TimeSpan.Parse(value); }
        }
        /// <summary>
        /// 
        /// </summary>
        [XmlIgnore]
        public TimeSpan MaxCommunicationFailureRetryInterval { get; set; }
    }
    

    /// <summary>
    /// Tenant request
    /// </summary>
    [Serializable()]
    public class ConfigurationRequest
    {
        /// <summary>
        /// Request Type ( Loopback, ReportList, Report1-3, Custom, Garbage ) 
        /// </summary>
        [System.Xml.Serialization.XmlElement("RequestTypeName")]
        public string RequestTypeName { get; set; }

        /// <summary>
        /// This is the Type of the request expected by the system. (LoopBackRequest, ReportDescriptorListRequest,
        ///   or RunReportRequest)
        /// </summary>
        [System.Xml.Serialization.XmlElement("RequestType")]
        public string RequestType { get; set; }

        /// <summary>
        /// Unique Id associated with request.  For reports it is \\PathToReport\ReportName.rpt
        /// </summary>
        [System.Xml.Serialization.XmlElement("ReportUniqueId")]
        public string ReportUniqueId { get; set; }

        /// <summary>
        /// Collection of ConfigurationRequestParameters
        /// </summary>
        [XmlArray("Parameters")]
        [XmlArrayItem("Parameter", typeof(ConfigurationRequestParameter))]
        public ConfigurationRequestParameter[] Parameters { get; set; }

        /// <summary>
        /// For pass through data only.  Not read from xml
        /// </summary>
        [XmlIgnore]
        public object PassThru { get; set; }
    
    }

    /// <summary>
    /// Request Parameter.  Used for reports.
    /// </summary>
    [Serializable()]
    public class ConfigurationRequestParameter
    {
        /// <summary>
        /// Parameter Value type: (BooleanReportParameterValue, StringReportParameterValue, etc)
        /// </summary>
        [System.Xml.Serialization.XmlElement("ParameterType")]
        public string ParamType { get; set; }

        /// <summary>
        /// Name of the parameter
        /// </summary>
        [System.Xml.Serialization.XmlElement("ParameterName")]
        public string ParamName { get; set; }

        /// <summary>
        /// Value for this parameter
        /// </summary>
        [System.Xml.Serialization.XmlElement("ParameterValue")]
        public string ParamValue { get; set; }

        /// <summary>
        /// Premise Metadata passed through
        /// </summary>
        [System.Xml.Serialization.XmlElement("PremiseMetadata")]
        public string PremMetadata { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class ConfigurationTenantUtils
    {
        static string CONFIG_XML_FILENAME = "Sage.Connector.MockCloudHostAppConfig.xml";

        /// <summary>
        /// Saves Configuration to Sage.Connector.MockCloudHostAppConfig.xml file.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="list"></param>
        public static void SerializeAndSaveXmlTenants(string path, ConfigurationTenants list)
        {
            ConfigurationTenants serializableCollection = new ConfigurationTenants();
            string configXmlFile = Path.Combine(path, CONFIG_XML_FILENAME);

            using (var writer = XmlWriter.Create(configXmlFile))
            {
                try
                {
                    XmlSerializer s = new XmlSerializer(typeof(ConfigurationTenants));
                    s.Serialize(writer, serializableCollection);

                }
                catch (Exception ex)
                {
                    string test = ex.Message;
                }
            }
        }

        /// <summary>
        /// Reads Sage.Connector.MockCloudHostAppConfig.xml file and creates ConfigurationTenant collection.
        /// </summary>
        /// <returns></returns>
        public static ConfigurationTenants DeserializeTenants(string path)
        {
            ConfigurationTenants configTenantCollection = null;
            string configXmlFile = Path.Combine(path, CONFIG_XML_FILENAME);

            if (File.Exists(configXmlFile))
            {
                try
                {
                    using (var reader = new StreamReader(configXmlFile))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(ConfigurationTenants));
                        configTenantCollection = (ConfigurationTenants)serializer.Deserialize(reader);
                    }
                }
                catch (Exception)
                {
                    configTenantCollection = null;
                }
            }
            return configTenantCollection;
        }

        /// <summary>
        /// Converts Parameters to ReportParameterValue array
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static CloudContracts.ReportParamValue[] ConvertToReportParameterValueList(ConfigurationRequestParameter[] parameters)
        {
            List<CloudContracts.ReportParamValue> valueList = new List<CloudContracts.ReportParamValue>();

            foreach (ConfigurationRequestParameter p in parameters)
            {
                switch (p.ParamType)
                {
                    case "BooleanReportParamValue":
                        {
                            CloudContracts.BooleanReportParamValue newParameter = new CloudContracts.BooleanReportParamValue(
                                p.ParamName,
                                p.PremMetadata,
                                Convert.ToBoolean(p.ParamValue));
                            valueList.Add(newParameter);
                        }
                        break;
                    case "CurrencyReportParamValue":
                        {
                            CloudContracts.CurrencyReportParamValue newParameter = new CloudContracts.CurrencyReportParamValue(
                                p.ParamName,
                                p.PremMetadata,
                                Convert.ToDecimal(p.ParamValue));
                            valueList.Add(newParameter);
                        }
                        break;
                    case "DateMonthDayReportParamValue":
                        {
                            CloudContracts.DateMonthDayReportParamValue newParameter = new CloudContracts.DateMonthDayReportParamValue(
                                p.ParamName,
                                p.PremMetadata,
                                Convert.ToDateTime(p.ParamValue));
                            valueList.Add(newParameter);
                        }
                        break;
                    case "DateMonthYearReportParamValue":
                        {
                            CloudContracts.DateMonthYearReportParamValue newParameter = new CloudContracts.DateMonthYearReportParamValue(
                                p.ParamName,
                                p.PremMetadata,
                                Convert.ToDateTime(p.ParamValue));
                            valueList.Add(newParameter);
                        }
                        break;
                    case "DateReportParamValue":
                        {
                            CloudContracts.DateReportParamValue newParameter = new CloudContracts.DateReportParamValue(
                                p.ParamName,
                                p.PremMetadata,
                                Convert.ToDateTime(p.ParamValue));
                            valueList.Add(newParameter);
                        }
                        break;
                    case "DateTimeReportParamValue":
                        {
                            CloudContracts.DateTimeReportParamValue newParameter = new CloudContracts.DateTimeReportParamValue(
                                p.ParamName,
                                p.PremMetadata,
                                Convert.ToDateTime(p.ParamValue));
                            valueList.Add(newParameter);
                        }
                        break;
                    case "DecimalReportParamValue":
                        {
                            CloudContracts.DecimalReportParamValue newParameter = new CloudContracts.DecimalReportParamValue(
                                p.ParamName,
                                p.PremMetadata,
                                Convert.ToDecimal(p.ParamValue));
                            valueList.Add(newParameter);
                        }
                        break;
                    case "MultiSelectReportParamValue":
                        {
                            //Split comma separated values.
                            List<string> options = new List<string>(p.ParamValue.Split(','));
                            CloudContracts.MultiSelectReportParamValue newParameter = new CloudContracts.MultiSelectReportParamValue(
                                p.ParamName, 
                                p.PremMetadata,
                                options);
                        }
                        break;
                    case "PercentageReportParamValue":
                        {
                            CloudContracts.PercentageReportParamValue newParameter = new CloudContracts.PercentageReportParamValue(
                                p.ParamName,
                                p.PremMetadata,
                                Convert.ToDecimal(p.ParamValue));
                            valueList.Add(newParameter);
                        }
                        break;
                    case "PhoneNumberReportParamValue":
                        {
                            CloudContracts.PhoneNumberReportParamValue newParameter = new CloudContracts.PhoneNumberReportParamValue(
                                p.ParamName,
                                p.PremMetadata,
                                p.ParamValue);
                        }
                        break;
                    case "SingleSelectReportParamValue":
                        {
                            CloudContracts.SingleSelectReportParamValue newParameter = new CloudContracts.SingleSelectReportParamValue(
                                p.ParamName,
                                p.PremMetadata,
                                p.ParamValue);
                        }
                        break;
                    case "SocialSecurityNumberReportParamValue":
                        {
                            CloudContracts.SocialSecurityNumberReportParamValue newParameter = new CloudContracts.SocialSecurityNumberReportParamValue(
                                p.ParamName,
                                p.PremMetadata,
                                Convert.ToInt32(p.ParamValue));
                        }
                        break;
                    case "StringReportParamValue":
                        {
                            CloudContracts.StringReportParamValue newParameter = new CloudContracts.StringReportParamValue(
                                p.ParamName,
                                p.ParamValue,
                                p.PremMetadata);
                        }
                        break;
                    case "TimeElapsedReportParamValue":
                        {
                            TimeSpan? parameterValue = null;

                            string[] timeSpanList = p.ParamValue.Split(',');
                            if (timeSpanList.Length == 4)
                            {
                                parameterValue = new TimeSpan(
                                    Convert.ToInt32(timeSpanList[0]),//days
                                    Convert.ToInt32(timeSpanList[1]),//hours
                                    Convert.ToInt32(timeSpanList[2]),//minutes
                                    Convert.ToInt32(timeSpanList[3]));//seconds
                            
                            }
                            else if (timeSpanList.Length == 3)
                            {
                                parameterValue = new TimeSpan(
                                    Convert.ToInt32(timeSpanList[0]),//hours
                                    Convert.ToInt32(timeSpanList[1]),//minutes
                                    Convert.ToInt32(timeSpanList[2]));//seconds
                            }
                            else if (timeSpanList.Length == 1)
                            {
                                parameterValue = new TimeSpan(
                                    Convert.ToInt32(timeSpanList[0]));//ticks
                            }

                            CloudContracts.TimeElapsedReportParamValue newParameter = new CloudContracts.TimeElapsedReportParamValue(
                                p.ParamName,
                                p.PremMetadata,
                                parameterValue);
                            valueList.Add(newParameter);
                        }
                        break;
                    case "TimeOfDayReportParamValue":
                        {
                            TimeSpan? parameterValue = null;

                            string[] timeSpanList = p.ParamValue.Split(',');
                            if (timeSpanList.Length == 4)
                            {
                                parameterValue = new TimeSpan(
                                    Convert.ToInt32(timeSpanList[0]),//days
                                    Convert.ToInt32(timeSpanList[1]),//hours
                                    Convert.ToInt32(timeSpanList[2]),//minutes
                                    Convert.ToInt32(timeSpanList[3]));//seconds

                            }
                            else if (timeSpanList.Length == 3)
                            {
                                parameterValue = new TimeSpan(
                                    Convert.ToInt32(timeSpanList[0]),//hours
                                    Convert.ToInt32(timeSpanList[1]),//minutes
                                    Convert.ToInt32(timeSpanList[2]));//seconds
                            }
                            else if (timeSpanList.Length == 1)
                            {
                                parameterValue = new TimeSpan(
                                    Convert.ToInt32(timeSpanList[0]));//ticks
                            }

                            CloudContracts.TimeOfDayReportParamValue newParameter = new CloudContracts.TimeOfDayReportParamValue(
                                p.ParamName,
                                p.PremMetadata,
                                parameterValue);
                            valueList.Add(newParameter);
                        }
                        break;
                }
            }

            return valueList.ToArray();
        }
    }
}
