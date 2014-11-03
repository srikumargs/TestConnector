using System;
using System.Collections.Generic;
using Sage.Connector.Cloud.Integration.Interfaces.DataContracts;
using CloudContracts = Sage.Connector.Cloud.Integration.Interfaces.DataContracts;
using PremiseContracts = Sage.CRE.CloudConnector.Integration.Interfaces.DataContracts;

namespace Sage.Connector.ConnectorServiceCommon
{
    /// <summary>
    /// Helper class to map external premise interfaces
    /// to cloud interfaces
    /// </summary>
    public static class Convert
    {



    
        /// <summary>
        /// Converts an array of premise reports to an array of cloud reports
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static CloudContracts.ReportDescriptor[] ToCloudReportDescriptors(PremiseContracts.ReportDescriptor[] input)
        {
            var result = new List<CloudContracts.ReportDescriptor>();

            if (null != input)
            {
                foreach (var report in input)
                {
                    var reportDescriptor = ToCloudReportDescriptor(report);
                    if (reportDescriptor != null)
                    {
                        result.Add(reportDescriptor);
                    }
                }
            }

            return result.ToArray();
        }



        /// <summary>
        /// Converts an array of cloud ReportParamValue's to an array of premise ParameterValue's
        /// </summary>
        /// <param name="reportParamValues"></param>
        /// <returns></returns>
        public static PremiseContracts.ReportParamValue[] ToPremiseParameterValues(CloudContracts.ReportParamValue[] reportParamValues)
        {
            var result = new List<PremiseContracts.ReportParamValue>();

            int count = reportParamValues.Length;
            for (int i = 0; i < count; i++)
            {
                PremiseContracts.EntityTypeTag entityType = ToPremiseEntityType(reportParamValues[i].EntityType);

                if (reportParamValues[i] is CloudContracts.StringReportParamValue)
                {
                    var cloudParamValue = reportParamValues[i] as CloudContracts.StringReportParamValue;
                    result.Add(new PremiseContracts.StringReportParamValue(cloudParamValue.Name, cloudParamValue.PremiseMetadata, entityType, cloudParamValue.Value));
                }

                else if (reportParamValues[i] is CloudContracts.TimeElapsedReportParamValue)
                {
                    var cloudParamValue = reportParamValues[i] as CloudContracts.TimeElapsedReportParamValue;
                    result.Add(new PremiseContracts.TimeElapsedReportParamValue(cloudParamValue.Name, cloudParamValue.PremiseMetadata, entityType, cloudParamValue.Value));
                }
                else if (reportParamValues[i] is CloudContracts.TimeOfDayReportParamValue)
                {
                    var cloudParamValue = reportParamValues[i] as CloudContracts.TimeOfDayReportParamValue;
                    result.Add(new PremiseContracts.TimeOfDayReportParamValue(cloudParamValue.Name, cloudParamValue.PremiseMetadata, entityType, cloudParamValue.Value));
                }
                else if (reportParamValues[i] is CloudContracts.CurrencyReportParamValue)
                {
                    var cloudParamValue = reportParamValues[i] as CloudContracts.CurrencyReportParamValue;
                    result.Add(new PremiseContracts.CurrencyReportParamValue(cloudParamValue.Name, cloudParamValue.PremiseMetadata, entityType, cloudParamValue.Value));
                }
                else if (reportParamValues[i] is CloudContracts.DateMonthDayReportParamValue)
                {
                    var cloudParamValue = reportParamValues[i] as CloudContracts.DateMonthDayReportParamValue;
                    result.Add(new PremiseContracts.DateMonthDayReportParamValue(cloudParamValue.Name, cloudParamValue.PremiseMetadata, entityType, cloudParamValue.Value));
                }
                else if (reportParamValues[i] is CloudContracts.DateMonthYearReportParamValue)
                {
                    var cloudParamValue = reportParamValues[i] as CloudContracts.DateMonthYearReportParamValue;
                    result.Add(new PremiseContracts.DateMonthYearReportParamValue(cloudParamValue.Name, cloudParamValue.PremiseMetadata, entityType, cloudParamValue.Value));
                }
                else if (reportParamValues[i] is CloudContracts.DateReportParamValue)
                {
                    var cloudParamValue = reportParamValues[i] as CloudContracts.DateReportParamValue;
                    result.Add(new PremiseContracts.DateReportParamValue(cloudParamValue.Name, cloudParamValue.PremiseMetadata, entityType, cloudParamValue.Value));
                }
                else if (reportParamValues[i] is CloudContracts.DateTimeReportParamValue)
                {
                    var cloudParamValue = reportParamValues[i] as CloudContracts.DateTimeReportParamValue;
                    result.Add(new PremiseContracts.DateTimeReportParamValue(cloudParamValue.Name, cloudParamValue.PremiseMetadata, entityType, cloudParamValue.Value));
                }
                else if (reportParamValues[i] is CloudContracts.BooleanReportParamValue)
                {
                    var cloudParamValue = reportParamValues[i] as CloudContracts.BooleanReportParamValue;
                    result.Add(new PremiseContracts.BooleanReportParamValue(cloudParamValue.Name, cloudParamValue.PremiseMetadata, entityType, cloudParamValue.Value));
                }
                else if (reportParamValues[i] is CloudContracts.DecimalReportParamValue)
                {
                    var cloudParamValue = reportParamValues[i] as CloudContracts.DecimalReportParamValue;
                    result.Add(new PremiseContracts.DecimalReportParamValue(cloudParamValue.Name, cloudParamValue.PremiseMetadata, entityType, cloudParamValue.Value));
                }
                else if (reportParamValues[i] is CloudContracts.PhoneNumberReportParamValue)
                {
                    var cloudParamValue = reportParamValues[i] as CloudContracts.PhoneNumberReportParamValue;
                    result.Add(new PremiseContracts.PhoneNumberReportParamValue(cloudParamValue.Name, cloudParamValue.PremiseMetadata, entityType, cloudParamValue.Value));
                }
                else if (reportParamValues[i] is CloudContracts.PercentageReportParamValue)
                {
                    var cloudParamValue = reportParamValues[i] as CloudContracts.PercentageReportParamValue;
                    result.Add(new PremiseContracts.PercentageReportParamValue(cloudParamValue.Name, cloudParamValue.PremiseMetadata, entityType, cloudParamValue.Value));
                }
                else if (reportParamValues[i] is CloudContracts.SingleSelectReportParamValue)
                {
                    var cloudParamValue = reportParamValues[i] as CloudContracts.SingleSelectReportParamValue;
                    result.Add(new PremiseContracts.SingleSelectReportParamValue(cloudParamValue.Name, cloudParamValue.PremiseMetadata, entityType, cloudParamValue.Value));
                }
                else if (reportParamValues[i] is CloudContracts.MultiSelectReportParamValue)
                {
                    var cloudParamValue = reportParamValues[i] as CloudContracts.MultiSelectReportParamValue;
                    string[] values;
                    if (cloudParamValue.Values != null)
                    {
                        values = cloudParamValue.Values.ToArray();
                    }
                    else
                    {
                        values = new string[] { };
                    }
                    result.Add(new PremiseContracts.MultiSelectReportParamValue(cloudParamValue.Name, cloudParamValue.PremiseMetadata, entityType, values));
                }
                else if (reportParamValues[i] is CloudContracts.SocialSecurityNumberReportParamValue)
                {
                    var cloudParamValue = reportParamValues[i] as CloudContracts.SocialSecurityNumberReportParamValue;
                    result.Add(new PremiseContracts.SocialSecurityNumberReportParamValue(cloudParamValue.Name, cloudParamValue.PremiseMetadata, entityType, cloudParamValue.Value));
                }
            }

            return result.ToArray();
        }







        private static PremiseContracts.EntityTypeTag ToPremiseEntityType(CloudContracts.EntityTypeTag cloudEntityType)
        {
            if (cloudEntityType == null)
            {
                return null;
            }
            if (cloudEntityType is CloudContracts.JobEntityTypeTag)
            {
                return new PremiseContracts.JobEntityTypeTag();
            }
            else if (cloudEntityType is CloudContracts.VendorEntityTypeTag)
            {
                return new PremiseContracts.VendorEntityTypeTag();
            }

            return null;
        }

        private static CloudContracts.EntityTypeTag ToCloudEntityType(PremiseContracts.EntityTypeTag premiseEntityType)
        {
            if (premiseEntityType == null)
            {
                return null;
            }
            if (premiseEntityType is PremiseContracts.JobEntityTypeTag)
            {
                return new CloudContracts.JobEntityTypeTag();
            }
            else if (premiseEntityType is PremiseContracts.VendorEntityTypeTag)
            {
                return new CloudContracts.VendorEntityTypeTag();
            }

            return null;
        }

        private static CloudContracts.KeyName[] ToCloudKeyNames(PremiseContracts.KeyName[] values)
        {
            var result = new List<CloudContracts.KeyName>();

            if (null != values)
            {
                foreach (var keyName in values)
                {
                    result.Add(new CloudContracts.KeyName(keyName.Key, keyName.Name));
                }
            }

            return result.ToArray();
        }

        private static CloudContracts.BooleanReportParam ToCloudBooleanReportParam(PremiseContracts.BooleanReportParam input)
        {
            CloudContracts.BooleanReportParam result = null;

            if (null != input)
            {
                result = new CloudContracts.BooleanReportParam(
                    input.Name,
                    input.DisplayText,
                    input.IsRequired,
                    input.PremiseMetadata,
                    ToCloudEntityType(input.EntityType),
                    input.DefaultValue);
            }

            return result;
        }

        private static CloudContracts.CurrencyReportParam ToCloudCurrencyReportParam(PremiseContracts.CurrencyReportParam input)
        {
            CloudContracts.CurrencyReportParam result = null;

            if (null != input)
            {
                CloudContracts.ReportParameterIntegerTypes integerType = CloudContracts.ReportParameterIntegerTypes.None;

                if ((input.MinimumValue < 0) && (input.MaximumValue > 0))
                {
                    integerType = CloudContracts.ReportParameterIntegerTypes.Both;
                }
                else if ((input.MinimumValue <= 0) && (input.MaximumValue <= 0))
                {
                    integerType = CloudContracts.ReportParameterIntegerTypes.NegativeOnly;
                }
                else if ((input.MinimumValue >= 0) && (input.MaximumValue >= 0))
                {
                    integerType = CloudContracts.ReportParameterIntegerTypes.PositiveOnly;
                }

                result = new CloudContracts.CurrencyReportParam(
                    input.Name,
                    input.DisplayText,
                    input.IsRequired,
                    input.PremiseMetadata,
                    ToCloudEntityType(input.EntityType),
                    input.DefaultValue,
                    input.Scale,
                    input.Precision,
                    input.MinimumValue,
                    input.MaximumValue,
                    integerType,
                    input.CommaGrouping,
                    input.DisallowZero,
                    input.ShowZeroAsBlank);
            }
            return result;
        }

        private static CloudContracts.DateReportParam ToCloudDateReportParam(PremiseContracts.DateReportParam input)
        {
            CloudContracts.DateReportParam result = null;

            if (null != input)
            {
                result = new CloudContracts.DateReportParam(
                    input.Name,
                    input.DisplayText,
                    input.IsRequired,
                    input.PremiseMetadata,
                    ToCloudEntityType(input.EntityType),
                    input.DefaultValue,
                    input.MinimumValue ?? DateTime.MinValue,
                    input.MaximumValue ?? DateTime.MaxValue,
                    input.DefaultCurrent);
            }

            return result;
        }

        private static CloudContracts.DateTimeReportParam ToCloudDateTimeReportParam(PremiseContracts.DateTimeReportParam input)
        {
            CloudContracts.DateTimeReportParam result = null;

            if (null != input)
            {
                result = new CloudContracts.DateTimeReportParam(
                    input.Name,
                    input.DisplayText,
                    input.IsRequired,
                    input.PremiseMetadata,
                    ToCloudEntityType(input.EntityType),
                    input.DefaultValue,
                    input.MinimumValue ?? DateTime.MinValue,
                    input.MaximumValue ?? DateTime.MaxValue,
                    input.DefaultCurrent);
            }

            return result;
        }

        private static CloudContracts.DateMonthDayReportParam ToCloudDateMonthDayReportParam(PremiseContracts.DateMonthDayReportParam input)
        {
            CloudContracts.DateMonthDayReportParam result = null;

            if (null != input)
            {
                result = new CloudContracts.DateMonthDayReportParam(
                    input.Name,
                    input.DisplayText,
                    input.IsRequired,
                    input.PremiseMetadata,
                    ToCloudEntityType(input.EntityType),
                    input.DefaultValue,
                    input.MinimumValue ?? DateTime.MinValue,
                    input.MaximumValue ?? DateTime.MaxValue,
                    input.DefaultCurrent);
            }

            return result;
        }

        private static CloudContracts.DateMonthYearReportParam ToCloudDateMonthYearReportParam(PremiseContracts.DateMonthYearReportParam input)
        {
            CloudContracts.DateMonthYearReportParam result = null;

            if (null != input)
            {
                result = new CloudContracts.DateMonthYearReportParam(
                    input.Name,
                    input.DisplayText,
                    input.IsRequired,
                    input.PremiseMetadata,
                    ToCloudEntityType(input.EntityType),
                    input.DefaultValue,
                    input.MinimumValue ?? DateTime.MinValue,
                    input.MaximumValue ?? DateTime.MaxValue,
                    input.DefaultCurrent);
            }

            return result;
        }

        private static CloudContracts.SocialSecurityNumberReportParam ToCloudSocialSecurityNumberReportParam(PremiseContracts.SocialSecurityNumberReportParam input)
        {
            CloudContracts.SocialSecurityNumberReportParam result = null;

            if (null != input)
            {
                result = new CloudContracts.SocialSecurityNumberReportParam(
                        input.Name,
                        input.DisplayText,
                        input.IsRequired,
                        input.PremiseMetadata,
                        ToCloudEntityType(input.EntityType),
                        input.DefaultValue);
            }

            return result;
        }

        private static CloudContracts.DecimalReportParam ToCloudDecimalReportParam(PremiseContracts.DecimalReportParam input)
        {
            CloudContracts.DecimalReportParam result = null;

            if (null != input)
            {
                CloudContracts.ReportParameterIntegerTypes integerType = CloudContracts.ReportParameterIntegerTypes.None;

                if ((input.MinimumValue < 0) && (input.MaximumValue > 0))
                {
                    integerType = CloudContracts.ReportParameterIntegerTypes.Both;
                }
                else if ((input.MinimumValue <= 0) && (input.MaximumValue <= 0))
                {
                    integerType = CloudContracts.ReportParameterIntegerTypes.NegativeOnly;
                }
                else if ((input.MinimumValue >= 0) && (input.MaximumValue >= 0))
                {
                    integerType = CloudContracts.ReportParameterIntegerTypes.PositiveOnly;
                }

                result = new CloudContracts.DecimalReportParam(
                    input.Name,
                    input.DisplayText,
                    input.IsRequired,
                    input.PremiseMetadata,
                    ToCloudEntityType(input.EntityType),
                    input.DefaultValue,
                    input.Scale,
                    input.Precision,
                    input.MinimumValue,
                    input.MaximumValue,
                    integerType,
                    input.CommaGrouping,
                    input.DisallowZero,
                    input.ShowZeroAsBlank);
            }

            return result;
        }

        private static CloudContracts.PercentageReportParam ToCloudPercentageReportParam(PremiseContracts.PercentageReportParam input)
        {
            CloudContracts.PercentageReportParam result = null;

            if (null != input)
            {
                CloudContracts.ReportParameterIntegerTypes integerType = CloudContracts.ReportParameterIntegerTypes.None;

                if ((input.MinimumValue < 0) && (input.MaximumValue > 0))
                {
                    integerType = CloudContracts.ReportParameterIntegerTypes.Both;
                }
                else if ((input.MinimumValue <= 0) && (input.MaximumValue <= 0))
                {
                    integerType = CloudContracts.ReportParameterIntegerTypes.NegativeOnly;
                }
                else if ((input.MinimumValue >= 0) && (input.MaximumValue >= 0))
                {
                    integerType = CloudContracts.ReportParameterIntegerTypes.PositiveOnly;
                }

                result = new CloudContracts.PercentageReportParam(
                    input.Name,
                    input.DisplayText,
                    input.IsRequired,
                    input.PremiseMetadata,
                    ToCloudEntityType(input.EntityType),
                    input.DefaultValue,
                    input.Scale,
                    input.Precision,
                    input.MinimumValue,
                    input.MaximumValue,
                    integerType,
                    input.CommaGrouping,
                    input.DisallowZero,
                    input.ShowZeroAsBlank);
            }

            return result;
        }

        private static CloudContracts.PhoneNumberReportParam ToCloudPhoneNumberReportParam(PremiseContracts.PhoneNumberReportParam input)
        {
            CloudContracts.PhoneNumberReportParam result = null;

            if (null != input)
            {
                result = new CloudContracts.PhoneNumberReportParam(
                        input.Name,
                        input.DisplayText,
                        input.IsRequired,
                        input.PremiseMetadata,
                        ToCloudEntityType(input.EntityType),
                        input.DefaultValue);
            }
            return result;
        }

        private static CloudContracts.StringReportParam ToCloudStringReportParam(PremiseContracts.StringReportParam input)
        {
            CloudContracts.StringReportParam result = null;

            if (null != input)
            {
                result = new CloudContracts.StringReportParam(
                    input.Name,
                    input.DisplayText,
                    input.IsRequired,
                    input.PremiseMetadata,
                    ToCloudEntityType(input.EntityType),
                    input.DefaultValue,
                    input.Length);
            }

            return result;
        }

        private static CloudContracts.TimeElapsedReportParam ToCloudTimeElapsedReportParam(PremiseContracts.TimeElapsedReportParam input)
        {
            CloudContracts.TimeElapsedReportParam result = null;

            if (null != input)
            {
                TimeSpan? def = null;
                if (input.DefaultValue.HasValue)
                {
                    def = input.DefaultValue.Value;
                }
                TimeSpan? max = null;
                if (input.MaximumValue.HasValue)
                {
                    max = input.MaximumValue.Value;
                }
                TimeSpan? min = null;
                if (input.MinimumValue.HasValue)
                {
                    min = input.MinimumValue.Value;
                }

                result = new CloudContracts.TimeElapsedReportParam(input.Name,
                    input.DisplayText,
                    input.IsRequired,
                    input.PremiseMetadata,
                    ToCloudEntityType(input.EntityType),
                    def,
                    min,
                    max);
            }

            return result;
        }

        private static CloudContracts.TimeOfDayReportParam ToCloudTimeOfDayReportParam(PremiseContracts.TimeOfDayReportParam input)
        {
            CloudContracts.TimeOfDayReportParam result = null;

            if (null != input)
            {
                TimeSpan? def = null;
                if (input.DefaultValue.HasValue)
                {
                    def = input.DefaultValue.Value;
                }
                TimeSpan? max = null;
                if (input.MaximumValue.HasValue)
                {
                    max = input.MaximumValue.Value;
                }
                TimeSpan? min = null;
                if (input.MinimumValue.HasValue)
                {
                    min = input.MinimumValue.Value;
                }

                result = new CloudContracts.TimeOfDayReportParam(input.Name,
                    input.DisplayText,
                    input.IsRequired,
                    input.PremiseMetadata,
                    ToCloudEntityType(input.EntityType),
                    def,
                    input.UseCurrentTimeAsDefault,
                    min,
                    max);
            }

            return result;
        }

        private static CloudContracts.SingleSelectReportParam ToCloudSingleSelectReportParam(PremiseContracts.SingleSelectReportParam input)
        {
            CloudContracts.SingleSelectReportParam result = null;

            if (null != input)
            {
                result = new CloudContracts.SingleSelectReportParam(
                    input.Name,
                    input.DisplayText,
                    input.IsRequired,
                    input.PremiseMetadata,
                    ToCloudEntityType(input.EntityType),
                    EnsureStringNotNull(input.DefaultSelectionValue),
                    ToCloudKeyNames(input.AvailableSelectionValues),
                    input.IsCustomValueAllowed);
            }
            return result;
        }

        private static CloudContracts.MultiSelectReportParam ToCloudMultiSelectReportParam(PremiseContracts.MultiSelectReportParam input)
        {
            CloudContracts.MultiSelectReportParam result = null;

            if (null != input)
            {
                result = new CloudContracts.MultiSelectReportParam(
                    input.Name,
                    input.DisplayText,
                    input.PremiseMetadata,
                    ToCloudEntityType(input.EntityType),
                    ToCloudStringList(input.DefaultSelectionValues),
                    ToCloudKeyNames(input.AvailableSelectionValues),
                    input.MinimumSelectionCount,
                    input.MaximumSelectionCount,
                    input.IsCustomValueAllowed);
            }
            return result;
        }

        private static string EnsureStringNotNull(string value)
        {
            string retval = value;
            if (retval == null)
            {
                retval = string.Empty;
            }
            return retval;
        }

        private static CloudContracts.StringList ToCloudStringList(PremiseContracts.StringList value)
        {
            var result = new CloudContracts.StringList();
            if (value != null)
            {
                result.AddRange(value);
            }
            return result;
        }

        /// <summary>
        /// Convert an array of Premise report parameters to Cloud format.
        /// </summary>
        /// <param name="premiseParameters"></param>
        /// <returns></returns>
        public static CloudContracts.ReportParam[] ToCloudReportParams(PremiseContracts.ReportParam[] premiseParameters)
        {
            var result = new List<CloudContracts.ReportParam>();

            if (null == premiseParameters)
            {
                return result.ToArray();
            }

            foreach (var parameter in premiseParameters)
            {
                CloudContracts.ReportParam reportParam = null;

                if (parameter is PremiseContracts.BooleanReportParam)
                {
                    reportParam = ToCloudBooleanReportParam(parameter as PremiseContracts.BooleanReportParam);
                }
                else if (parameter is PremiseContracts.CurrencyReportParam)
                {
                    reportParam = ToCloudCurrencyReportParam(parameter as PremiseContracts.CurrencyReportParam);
                }
                else if (parameter is PremiseContracts.DateMonthDayReportParam)
                {
                    reportParam = ToCloudDateMonthDayReportParam(parameter as PremiseContracts.DateMonthDayReportParam);
                }
                else if (parameter is PremiseContracts.DateMonthYearReportParam)
                {
                    reportParam = ToCloudDateMonthYearReportParam(parameter as PremiseContracts.DateMonthYearReportParam);
                }
                else if (parameter is PremiseContracts.DateReportParam)
                {
                    reportParam = ToCloudDateReportParam(parameter as PremiseContracts.DateReportParam);
                }
                else if (parameter is PremiseContracts.DateTimeReportParam)
                {
                    reportParam = ToCloudDateTimeReportParam(parameter as PremiseContracts.DateTimeReportParam);
                }
                else if (parameter is PremiseContracts.DecimalReportParam)
                {
                    reportParam = ToCloudDecimalReportParam(parameter as PremiseContracts.DecimalReportParam);
                }
                else if (parameter is PremiseContracts.MultiSelectReportParam)
                {
                    reportParam = ToCloudMultiSelectReportParam(parameter as PremiseContracts.MultiSelectReportParam);
                }
                else if (parameter is PremiseContracts.PercentageReportParam)
                {
                    reportParam = ToCloudPercentageReportParam(parameter as PremiseContracts.PercentageReportParam);
                }
                else if (parameter is PremiseContracts.PhoneNumberReportParam)
                {
                    reportParam = ToCloudPhoneNumberReportParam(parameter as PremiseContracts.PhoneNumberReportParam);
                }
                else if (parameter is PremiseContracts.SingleSelectReportParam)
                {
                    reportParam = ToCloudSingleSelectReportParam(parameter as PremiseContracts.SingleSelectReportParam);
                }
                else if (parameter is PremiseContracts.SocialSecurityNumberReportParam)
                {
                    reportParam = ToCloudSocialSecurityNumberReportParam(parameter as PremiseContracts.SocialSecurityNumberReportParam);
                }
                else if (parameter is PremiseContracts.StringReportParam)
                {
                    reportParam = ToCloudStringReportParam(parameter as PremiseContracts.StringReportParam);
                }
                else if (parameter is PremiseContracts.TimeElapsedReportParam)
                {
                    reportParam = ToCloudTimeElapsedReportParam(parameter as PremiseContracts.TimeElapsedReportParam);
                }
                else if (parameter is PremiseContracts.TimeOfDayReportParam)
                {
                    reportParam = ToCloudTimeOfDayReportParam(parameter as PremiseContracts.TimeOfDayReportParam);
                }
                else if (parameter is PremiseContracts.InformationalTextReportParam)
                {
                    reportParam = ToCloudInformationalTextReportParam(parameter as PremiseContracts.InformationalTextReportParam);
                }

                if (null != reportParam)
                {
                    result.Add(reportParam);
                }
            }

            return result.ToArray();
        }


        /// <summary>
        /// Convert a single report to cloud format.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static CloudContracts.ReportDescriptor ToCloudReportDescriptor(PremiseContracts.ReportDescriptor input)
        {
            CloudContracts.ReportDescriptor result = null;

            if (null != input)
            {
                if (input.IsIndeterminateResult)
                {
                    result = new CloudContracts.ReportDescriptor(
                        input.UniqueIdentifier,
                        input.Name,
                        ToCloudErrorInformation(input.IndeterminateResultMessage)
                        );
                }
                else
                {
                    result = new CloudContracts.ReportDescriptor(
                        input.UniqueIdentifier,
                        input.Name,
                        input.Description,
                        input.Category,
                        input.ApplicationName,
                        input.MenuPath,
                        input.Path,
                        ToCloudReportParams(input.ReportParams),
                        ToCloudSystemFilterParam(input.SystemFilterParams)
                        );
                }
            }

            return result;
        }

        private static CloudContracts.InformationalTextReportParam ToCloudInformationalTextReportParam(PremiseContracts.InformationalTextReportParam input)
        {
            CloudContracts.InformationalTextReportParam result = null;

            if (null != input)
            {
                result = new CloudContracts.InformationalTextReportParam(
                    input.Name,
                    input.DisplayText,
                    input.PremiseMetadata,
                    ToCloudEntityType(input.EntityType)
                );
            }

            return result;
        }

        /// <summary>
        /// Converts an array of premise ErrorInformation to an array of cloud ErrorInformation
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static CloudContracts.ErrorInformation[] ToCloudErrorInformationList(PremiseContracts.ErrorInformation[] input)
        {
            var result = new List<CloudContracts.ErrorInformation>();

            if (input != null)
            {
                foreach (PremiseContracts.ErrorInformation ei in input)
                {
                    CloudContracts.ErrorInformation cloudErrorInformation = ToCloudErrorInformation(ei);
                    if (cloudErrorInformation != null)
                    {
                        result.Add(cloudErrorInformation);
                    }
                }
            }

            return result.ToArray();
        }

        private static CloudContracts.ErrorInformation ToCloudErrorInformation(PremiseContracts.ErrorInformation input)
        {
            CloudContracts.ErrorInformation result = null;

            if (null != input)
            {
                result = new CloudContracts.ErrorInformation(
                    input.RawErrorMessage,
                    input.UserFacingErrorMessage);
            }

            return result;
        }

        /// <summary>
        /// Convert an array of Premise SystemFilterParam parameters to Cloud format.
        /// </summary>
        /// <param name="premiseParameters"></param>
        /// <returns></returns>
        public static CloudContracts.SystemFilterParam[] ToCloudSystemFilterParam(PremiseContracts.SystemFilterParam[] premiseParameters)
        {
            var result = new List<CloudContracts.SystemFilterParam>();

            if (null == premiseParameters)
            {
                return result.ToArray();
            }

            foreach (var parameter in premiseParameters)
            {
                CloudContracts.SystemFilterParam filterPram = ToCloudSystemFilterParam(parameter);
                if (null != filterPram)
                {
                    result.Add(filterPram);
                }
            }

            return result.ToArray();
        }

        private static CloudContracts.SingleValueSystemFilterParam ToCloudSystemFilterParam(PremiseContracts.SystemFilterParam input)
        {
            CloudContracts.SingleValueSystemFilterParam result = null;

            if (null != input)
            {
                if (input is PremiseContracts.SingleValueSystemFilterParam)
                {
                    PremiseContracts.SingleValueSystemFilterParam filter = input as PremiseContracts.SingleValueSystemFilterParam;
                    if (null != filter.SingleFilterParam)
                    {
                        CloudContracts.ReportParam[] cloudParams = ToCloudReportParams(new PremiseContracts.ReportParam[] { filter.SingleFilterParam });
                        if ((null != cloudParams) && (cloudParams.Length == 1))
                            result = new CloudContracts.SingleValueSystemFilterParam(cloudParams[0]);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Convert cloud system filter list to premise format
        /// </summary>
        /// <param name="cloudParameters"></param>
        /// <returns></returns>
        public static PremiseContracts.SystemFilterParamValue[] ToPremiseSystemFilterValues(CloudContracts.SystemFilterParamValue[] cloudParameters)
        {
            var result = new List<PremiseContracts.SystemFilterParamValue>();

            if (null == cloudParameters)
            {
                return result.ToArray();
            }

            foreach (var parameter in cloudParameters)
            {
                PremiseContracts.SystemFilterParamValue filterPram = ToPremiseSystemFilterParamValue(parameter);
                if (null != filterPram)
                {
                    result.Add(filterPram);
                }
            }

            return result.ToArray();
        }

        private static PremiseContracts.SingleValueSystemFilterParamValue ToPremiseSystemFilterParamValue(CloudContracts.SystemFilterParamValue input)
        {
            PremiseContracts.SingleValueSystemFilterParamValue result = null;

            if (null != input)
            {
                if (input is CloudContracts.SingleValueSystemFilterParamValue)
                {
                    CloudContracts.SingleValueSystemFilterParamValue filter = input as CloudContracts.SingleValueSystemFilterParamValue;
                    if (null != filter.SingleFilterParamValue)
                    {
                        PremiseContracts.ReportParamValue[] filterParams = ToPremiseParameterValues(new CloudContracts.ReportParamValue[] { filter.SingleFilterParamValue });
                        if ((null != filterParams) && (filterParams.Length == 1))
                            result = new PremiseContracts.SingleValueSystemFilterParamValue(filterParams[0]);
                    }
                }
            }

            return result;
        }
    }
}
