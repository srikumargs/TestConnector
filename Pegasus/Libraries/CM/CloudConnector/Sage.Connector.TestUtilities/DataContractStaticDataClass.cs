using System;
using System.Collections.Generic;
using System.Linq;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.CRE.CloudConnector.Integration.Interfaces.DataContracts;
using CC = Sage.Connector.Cloud.Integration.Interfaces.DataContracts;
using PC = Sage.CRE.CloudConnector.Integration.Interfaces.DataContracts;

namespace Sage.Connector.TestUtilities
{
    public class DataContractStaticDataClass
    {
        #region Static Data Items

        private static List<string> _listOfStrings = new List<string>{"string0",
        "string1", "string2", "string3", "string4", "string5"
        , "string6", "string7", "string8", "string9", "string10"
        , "string11", "string12", "string13", "string14", "string15"
        , "string16", "string17", "string18", "string19", "string20"
        , "string21", "string22", "string23", "string24", "string25"
        };

        public static string uniqueId1 = "uniqueId1";
        public static string uniqueId2 = "uniqueId2";
        public static string companyUniqueId1 = "companyUniqueId1";
        public static string companyUniqueId2 = "companyUniqueId2";
        public static string name1 = "name1";
        public static string name2 = "name2";
        public static string description1 = "description1";
        public static string description2 = "description2";
        public static string displayText1 = "displayText1";
        public static string displayText2 = "displayText2";
        public static string trade1 = "trade1";
        public static string trade2 = "trade2";
        public static string region1 = "region1";
        public static string region2 = "region2";
        public static string location1 = "location1";
        public static string location2 = "location2";
        public static string address1_1 = "address1_1";
        public static string address1_2 = "address1_2";
        public static string address2_1 = "address2_1";
        public static string address2_2 = "address2_2";
        public static string address3_1 = "address3_1";
        public static string address3_2 = "address3_2";
        public static string address4_1 = "address4_1";
        public static string address4_2 = "address4_2";
        public static string city1 = "city1";
        public static string city2 = "city2";
        public static string state1 = "state1";
        public static string state2 = "state2";
        public static string zip1 = "zip1";
        public static string zip2 = "zip2";
        public static string country1 = "country1";
        public static string country2 = "country2";
        public static string phone1 = "phone1";
        public static string phone2 = "phone2";
        public static string extension1 = "extension1";
        public static string extension2 = "extension2";
        public static string mobile1 = "mobile1";
        public static string mobile2 = "mobile2";
        public static string pager1 = "pager1";
        public static string pager2 = "pager2";
        public static string fax1 = "fax1";
        public static string fax2 = "fax2";
        public static string email1 = "email1";
        public static string email2 = "email2";
        public static string web1 = "web1";
        public static string web2 = "web2";
        public static string status1 = "status1";
        public static string status2 = "status2";
        public static string justText1 = "justText1";
        public static string justText2 = "justText2";

        public static bool required1 = true;
        public static bool required2 = false;

        public static int intValue1 = 13;
        public static int intValue2 = 11185;

        public static DateTime dateThisMonth = new DateTime(2012, 2, 01);
        public static DateTime dateCplYearsAgo = new DateTime(2010, 6, 11);
        public static DateTime date1 = new DateTime(1989, 8, 30);
        public static DateTime date2 = new DateTime(2011, 11, 7);
        public static DateTime dateInvalid = new DateTime(2012, 2, 28);

        public static TimeSpan timespanMinValue1 = new TimeSpan(0);
        public static TimeSpan timespanMinValue2 = new TimeSpan(1000);
        public static TimeSpan timespanMaxValue1 = new TimeSpan(50000);
        public static TimeSpan timespanMaxValue2 = new TimeSpan(100000);
        public static TimeSpan timespanDefaultValue1 = new TimeSpan(500);
        public static TimeSpan timespanDefaultValue2 = new TimeSpan(90000);

        public static string FileVersion1 = "1.11.1111";
        public static string FileVersion2 = "2.2.22.222";

        public static string ProductName1 = "Sage product of some sort";
        public static string ProductCode1 = "SCA";
        public static string ProductVersion1 = "9.8.8888";
        
        public static string ProductName2 = "Sage product of some sort";
        public static string ProductVersion2 = "9.8.7894";

        public static string InterfaceVersion1 = "29.8.11";
        public static string InterfaceVersion2 = "19.8.55";
        public static string MinInterfaceVersion1 = "3.33.333";


        private static PC.JobEntityTypeTag entityType1 = new PC.JobEntityTypeTag();
        private static PC.VendorEntityTypeTag entityType2 = new PC.VendorEntityTypeTag();



        #endregion

        #region Static Data Collections

        public static ConnectorState GetConnectorState(string postText)
        {
            string fileVersion = "fileVersion-" + postText;
            string productCode = "productCode-" + postText;
            string productName = "productName-" + postText;
            string productVersion = "productVersion-" + postText;
            string interfaceVersion = "interfaceVersion-" + postText;
            string boInterfaceVersion = "boInterfaceVersion-" + postText;
            string minBoIntegrationInterfaceVersion = "minBoIntegrationInterfaceVersion-" + postText;
            DateTime systemDateTimeUtc = DataContractStaticDataClass.date1;
            TimeSpan uptime = DataContractStaticDataClass.timespanDefaultValue1;
            TimeSpan maxUptimeBeforeRestart = DataContractStaticDataClass.timespanMaxValue1;
            RestartMode restartMode = RestartMode.RestartIntervalSpecified;
            TimeSpan timeToBlackout = DataContractStaticDataClass.timespanMaxValue1;
            ConnectorUpdateStatus connectorUpdateStatus = ConnectorUpdateStatus.UpdateAvailable;
            List<SubsystemHealthMessage> subsystemHealthMessages = GetSubSystemHealthMessages(3);
            List<IntegratedConnectionState> integratedConnectionStates = new List<IntegratedConnectionState>();

            return new ConnectorState(
                    fileVersion,
                    productCode,
                    productName,
                    productVersion,
                    interfaceVersion,
                    boInterfaceVersion,
                    minBoIntegrationInterfaceVersion,
                    systemDateTimeUtc,
                    uptime,
                    maxUptimeBeforeRestart,
                    restartMode,
                    timeToBlackout,
                    connectorUpdateStatus,
                    GetGenericUpdateInfo("1"),
                    subsystemHealthMessages,
                    integratedConnectionStates
                    );
        }

        public static List<SubsystemHealthMessage> GetSubSystemHealthMessages(int qty)
        {
            List<SubsystemHealthMessage> messages = new List<SubsystemHealthMessage>();
            for (int i = 1; i <= qty; i++)
            {
                messages.Add(
                    new SubsystemHealthMessage(
                        Subsystem.ConfigurationService,
                        "RawMessage"+ i.ToString(),
                        "UserFacingMessage" + i.ToString(),
                        DataContractStaticDataClass.date1,
                        i+234
                        ));
            }
            return messages;
        }

        public static UpdateInfo GetGenericUpdateInfo(string postText)
        {
            return new UpdateInfo(
                 "Product Version "+ postText,
                 DataContractStaticDataClass.date2,
                 "Update Description" + postText,
                 new Uri("http://www.google.com"));

        }

        public static string[] PropertyIgnoreList()
        {
            return new string[] { "ExtensionData" };
        }

        public static string[] GetListOfStrings(int qty)
        {
            return GetListOfStrings(qty, "");
        }

        public static string[] GetListOfStrings(int qty, string preText)
        {
            string[] test = new string[qty];
            _listOfStrings.CopyTo(0, test, 0, qty);
            if (!string.IsNullOrEmpty(preText))
            {
                for (int i = 0; i < test.Length; i++)
                {
                    test[i] = string.Concat(preText + test[i]);
                }
            }
            return test;
        }

        public static PC.ErrorInformation[] GetErrorInformationListWithCustomerFacing(int qty)
        {
            PC.ErrorInformation[] list = new PC.ErrorInformation[qty];

            for (int i = 0; i < qty; i++)
            {
                PC.ErrorInformation item = new PC.ErrorInformation(_listOfStrings[i], "User Facing Error_" + i.ToString());
                list[i] = item;
            }
            return list;
        }

        public static PC.ErrorInformation[] GetErrorInformationList(int qty)
        {
            PC.ErrorInformation[] list = new PC.ErrorInformation[qty];

            for (int i = 0; i < qty; i++)
            {
                PC.ErrorInformation item = new PC.ErrorInformation(_listOfStrings[i]);
                list[i] = item;
            }
            return list;
        }

        public static PC.Job[] GetJobList(int qty)
        {
            PC.Job[] list = new PC.Job[qty];
            for (int i = 0; i < qty; i++)
            {
                PC.Job item = new PC.Job(
                    i.ToString() + "_" + uniqueId1,
                    i.ToString() + "_" + name1,
                    i.ToString() + "_" + description1,
                    i.ToString() + "_" + address2_1,
                    i.ToString() + "_" + address1_1,
                    i.ToString() + "_" + city1,
                    i.ToString() + "_" + state1,
                    i.ToString() + "_" + country1,
                    i.ToString() + "_" + zip1,
                    i.ToString() + "_" + phone1,
                    i.ToString() + "_" + fax1,
                    i.ToString() + "_" + mobile1,
                    i.ToString() + "_" + status1
                    );
                list[i] = item;
            }
            return list;
        }

        public static PC.CompanyContact[] GetCompanyContactList(int qty)
        {
            PC.CompanyContact[] list = new PC.CompanyContact[qty];

            for (int i = 0; i < qty; i++)
            {
                PC.CompanyContact item = new PC.CompanyContact(
                    i.ToString() + "_" + uniqueId1,
                    i.ToString() + "_" + name1,
                    GetListOfStrings(3),
                    GetListOfStrings(3),
                    i.ToString() + "_" + region1,
                    i.ToString() + "_" + location1,
                    i.ToString() + "_" + address1_1,
                    i.ToString() + "_" + address2_1,
                    i.ToString() + "_" + address3_1,
                    i.ToString() + "_" + address4_1,
                    i.ToString() + "_" + city1,
                    i.ToString() + "_" + state1,
                    i.ToString() + "_" + country1,
                    i.ToString() + "_" + zip1,
                    i.ToString() + "_" + phone1,
                    i.ToString() + "_" + fax1,
                    i.ToString() + "_" + email1,
                    i.ToString() + "_" + web1,
                    i.ToString() + "_" + status1);
                list[i] = item;
            }
            return list;
        }

        public static PC.PersonContact[] GetPersonContactList(int qty)
        {
            PC.PersonContact[] list = new PC.PersonContact[qty];

            for (int i = 0; i < qty; i++)
            {
                PC.PersonContact item = new PC.PersonContact(
                    i.ToString() + "_" + uniqueId1,
                    i.ToString() + "_" + companyUniqueId1,
                    i.ToString() + "_" + name1,
                    i.ToString() + "_" + description1,
                    i.ToString() + "_" + phone1,
                    i.ToString() + "_" + extension1,
                    i.ToString() + "_" + fax1,
                    i.ToString() + "_" + mobile1,
                    i.ToString() + "_" + pager1,
                    i.ToString() + "_" + email1,
                    i.ToString() + "_" + web1,
                    i.ToString() + "_" + status1);
                list[i] = item;
            }
            return list;
        }

        public static PC.ReportDescriptor[] GetReportDescriptorList(int qty, bool withEntityType=true)
        {
            PC.ReportDescriptor[] list = new PC.ReportDescriptor[qty];
            for (int i = 0; i < qty; i++)
            {
                PC.ReportParam[] prams = GetReportParams(16, withEntityType);
                PC.ReportDescriptor item = new PC.ReportDescriptor(
                    i.ToString() + "_" + uniqueId1,
                    i.ToString() + "_" + description1,
                    i.ToString() + "_" + uniqueId1,
                    i.ToString() + "_" + description1,
                    i.ToString() + "_" + description1,
                    i.ToString() + "_" + description1,
                    i.ToString() + "_" + description1,
                    prams,
                    GetSingleValueSystemFilterParam(prams.First())
                    );
                list[i] = item;
            }
            return list;
        }

        public static PC.ReportParam[] GetReportParams(int qty, bool withEntityType=true)
        {
            return GetReportParameters(qty,withEntityType).ToArray();
        }

        public static PC.BooleanReportParam GetBooleanReportParameter(bool withEntityType=true)
        {
            if(withEntityType)
                return new PC.BooleanReportParam(name1, displayText1, required1, justText1, entityType1, true);
            else
                return new PC.BooleanReportParam(name1, displayText1, required1, justText1, true);
        }

        public static PC.CurrencyReportParam GetCurrencyReportParameter(bool getDefault = true, bool withEntityType = true)
        {
            if (getDefault)
            {
                if (withEntityType)
                    return new PC.CurrencyReportParam(
                        name1,
                        displayText1,
                        required1,
                        justText1,
                        entityType1,
                        44.55m,
                        1,
                        2,
                        -10.00m,
                        2342343.00m,
                        PC.ReportParameterIntegerTypes.Both,
                        true,
                        false,
                        false
                        );
                else
                    return new PC.CurrencyReportParam(
                        name1,
                        displayText1,
                        required1,
                        justText1,
                        44.55m,
                        1,
                        2,
                        -10.00m,
                        2342343.00m,
                        PC.ReportParameterIntegerTypes.Both,
                        true,
                        false,
                        false
                        );
            }
            else
            {
                if(withEntityType)
                    return new PC.CurrencyReportParam(
                        name2,
                        displayText2,
                        required2,
                        justText2,
                        entityType2,
                        44.65m,
                        1,
                        2,
                        10.01m,
                        2342353.00m,
                        PC.ReportParameterIntegerTypes.PositiveOnly,
                        true,
                        true,
                        false
                        );
                else
                    return new PC.CurrencyReportParam(
                        name2,
                        displayText2,
                        required2,
                        justText2,
                        44.65m,
                        1,
                        2,
                        10.01m,
                        2342353.00m,
                        PC.ReportParameterIntegerTypes.PositiveOnly,
                        true,
                        true,
                        false
                        );
            }
        }

        public static PC.DecimalReportParam GetDecimalReportParameter(bool withEntityType = true)
        {
            if(withEntityType)
                return new PC.DecimalReportParam(
                    name1,
                    displayText1,
                    required1,
                    justText1,
                    entityType1,
                    7.22m,
                    1,
                    2,
                    1.00m,
                    555.00m,
                    PC.ReportParameterIntegerTypes.PositiveOnly,
                    true,
                    false,
                    false
                    );
            else
                return new PC.DecimalReportParam(
                    name1,
                    displayText1,
                    required1,
                    justText1,
                    7.22m,
                    1,
                    2,
                    1.00m,
                    555.00m,
                    PC.ReportParameterIntegerTypes.PositiveOnly,
                    true,
                    false,
                    false
                    );
        }

        public static PC.DateMonthDayReportParam GetDateMonthDayReportParam(bool withEntityType = true)
        {
            if(withEntityType)
                return new PC.DateMonthDayReportParam(
                    name1,
                    displayText1,
                    false,
                    justText1,
                    entityType1,
                    null,
                    DateTime.MinValue,
                    DateTime.MaxValue,
                    false);
            else
                return new PC.DateMonthDayReportParam(
                    name1,
                    displayText1,
                    false,
                    justText1,
                    null,
                    DateTime.MinValue,
                    DateTime.MaxValue,
                    false);
        }

        public static PC.DateMonthYearReportParam GetDateMonthYearReportParameter(bool withEntityType = true)
        {
            if(withEntityType)
                return new PC.DateMonthYearReportParam(
                    name1,
                    displayText1,
                    false,
                    justText1,
                    entityType1,
                    null,
                    DateTime.MinValue,
                    DateTime.MaxValue,
                    false);
            else
                return new PC.DateMonthYearReportParam(
                    name1,
                    displayText1,
                    false,
                    justText1,
                    null,
                    DateTime.MinValue,
                    DateTime.MaxValue,
                    false);
        }

        public static PC.DateReportParam GetDateReportParam(bool withEntityType = true)
        {
            if(withEntityType)
                return new PC.DateReportParam(
                    name1,
                    displayText1,
                    false,
                    justText1,
                    entityType1,
                    null,
                    DateTime.MinValue,
                    DateTime.MaxValue,
                    false);
            else
                return new PC.DateReportParam(
                    name1,
                    displayText1,
                    false,
                    justText1,
                    null,
                    DateTime.MinValue,
                    DateTime.MaxValue,
                    false);
        }

        public static PC.DateTimeReportParam GetDateTimeReportParam(bool withEntityType = true)
        {
            if(withEntityType)
                return new PC.DateTimeReportParam(
                    name1,
                    displayText1,
                    false,
                    justText1,
                    entityType1,
                    null,
                    DateTime.MinValue,
                    DateTime.MaxValue.AddYears(-120),
                    false);
            else
                return new PC.DateTimeReportParam(
                    name1,
                    displayText1,
                    false,
                    justText1,
                    null,
                    DateTime.MinValue,
                    DateTime.MaxValue.AddYears(-120),
                    false);
        }

        public static PC.InformationalTextReportParam GetInformationalTextReportParam(bool withEntityType = true)
        {
            if(withEntityType)
                return new PC.InformationalTextReportParam(
                    name1,
                    displayText1,
                    justText1,
                    entityType1);
            else
                return new PC.InformationalTextReportParam(
                    name1,
                    displayText1,
                    justText1);
        }

        public static PC.MultiSelectReportParam GetMultiSelectReportParam(bool withEntityType = true)
        {
            if(withEntityType)
                return new PC.MultiSelectReportParam(
                    name1,
                    displayText1,
                    justText1,
                    entityType1,
                    new List<string>(GetListOfStrings(4)),
                    GetKeyNameList(),
                    1,
                    3,
                    true
                    );
            else
                return new PC.MultiSelectReportParam(
                    name1,
                    displayText1,
                    justText1,
                    new List<string>(GetListOfStrings(4)),
                    GetKeyNameList(),
                    1,
                    3,
                    true
                    );
        }

        public static PC.SingleSelectReportParam GetSingleSelectReportParam(bool withEntityType = true)
        {
            if(withEntityType)
                return new PC.SingleSelectReportParam(
                    name1,
                    displayText1,
                    false,
                    justText1,
                    entityType1,
                    "name2",
                    GetKeyNameList(),
                    false
                    );
            else
                return new PC.SingleSelectReportParam(
                    name1,
                    displayText1,
                    false,
                    justText1,
                    "name2",
                    GetKeyNameList(),
                    false
                    );
        }

        public static PC.PercentageReportParam GetPercentageReportParameter(bool withEntityType = true)
        {
            if(withEntityType)
                return new PC.PercentageReportParam(
                    name1,
                    displayText1,
                    required1,
                    justText1,
                    entityType1,
                    10.0m,
                    1,
                    2,
                    -100.00m,
                    100.00m,
                    PC.ReportParameterIntegerTypes.Both,
                    false,
                    false,
                    false
                    );
            else
                return new PC.PercentageReportParam(
                    name1,
                    displayText1,
                    required1,
                    justText1,
                    10.0m,
                    1,
                    2,
                    -100.00m,
                    100.00m,
                    PC.ReportParameterIntegerTypes.Both,
                    false,
                    false,
                    false
                    );
        }

        public static PC.PhoneNumberReportParam GetPhoneNumberReportParam(bool withEntityType = true)
        {
            if(withEntityType)
                return new PC.PhoneNumberReportParam(
                    name1,
                    displayText1,
                    true,
                    justText1,
                    entityType1,
                    phone1);
            else
                return new PC.PhoneNumberReportParam(
                    name1,
                    displayText1,
                    true,
                    justText1,
                    phone1);
        }

        public static PC.StringReportParam GetStringReportParam(bool withEntityType = true)
        {
            if(withEntityType)
                return new PC.StringReportParam(
                    name1,
                    displayText1,
                    false,
                    justText1,
                    entityType1,
                    "",
                    50);
            else
                return new PC.StringReportParam(
                    name1,
                    displayText1,
                    false,
                    justText1,
                    "",
                    50);
        }

        public static PC.SocialSecurityNumberReportParam GetSocialSecurityNumberReportParam(bool withEntityType = true)
        {
            if(withEntityType)
                return new PC.SocialSecurityNumberReportParam(
                    name1,
                    displayText1,
                    true,
                    justText1,
                    entityType1,
                    0);
            else
                return new PC.SocialSecurityNumberReportParam(
                    name1,
                    displayText1,
                    true,
                    justText1,
                    0);
        }

        public static PC.TimeElapsedReportParam GetTimeElapsedReportParam(bool withEntityType = true)
        {
            if(withEntityType)
                return new PC.TimeElapsedReportParam(
                    name1,
                    displayText1,
                    true,
                    justText1,
                    entityType1,
                    timespanDefaultValue1,
                    timespanMinValue1,
                    timespanMaxValue1);
            else
                return new PC.TimeElapsedReportParam(
                    name1,
                    displayText1,
                    true,
                    justText1,
                    timespanDefaultValue1,
                    timespanMinValue1,
                    timespanMaxValue1);
        }

        public static PC.TimeOfDayReportParam GetTimeOfDayReportParam(bool withEntityType = true)
        {
            if(withEntityType)

                return new PC.TimeOfDayReportParam(
                    name2,
                    displayText2,
                    true,
                    justText2,
                    entityType1,
                    timespanDefaultValue2,
                    true,
                    timespanMinValue2,
                    timespanMaxValue2);
            else
                return new PC.TimeOfDayReportParam(
                    name2,
                    displayText2,
                    true,
                    justText2,
                    timespanDefaultValue2,
                    true,
                    timespanMinValue2,
                    timespanMaxValue2);
        }

        public static List<PC.KeyName> GetKeyNameList()
        {
            return new List<PC.KeyName>{
                new PC.KeyName("key1", "name1"),
                new PC.KeyName("key2", "name2"),
                new PC.KeyName("key3", "name3")};
        }

        public static List<PC.ReportParam> GetReportParameters(int qty, bool withEntityType = true)
        {
            List<PC.ReportParam> retValues = new List<PC.ReportParam>();
            int counter = 0;
            for (int i = 1; i <= qty; i++)
            {
                //recycles through report parameters
                if(++counter >16) 
                    counter = 1;

                PC.ReportParam retValue;
                switch (counter)
                {
                    case 1:
                        { retValue = GetBooleanReportParameter(withEntityType); } break;
                    case 2:
                        { retValue = GetCurrencyReportParameter(withEntityType); } break;
                    case 3:
                        { retValue = GetDecimalReportParameter(withEntityType); } break;
                    case 4:
                        { retValue = GetDateMonthDayReportParam(withEntityType); } break;
                    case 5:
                        { retValue = GetDateMonthYearReportParameter(withEntityType); } break;
                    case 6:
                        { retValue = GetDateReportParam(withEntityType); } break;
                    case 7:
                        { retValue = GetDateTimeReportParam(withEntityType); } break;
                    case 8:
                        { retValue = GetInformationalTextReportParam(withEntityType); } break;
                    case 9:
                        { retValue = GetMultiSelectReportParam(withEntityType); } break;
                    case 10:
                        { retValue = GetSingleSelectReportParam(withEntityType); } break;
                    case 11:
                        { retValue = GetPercentageReportParameter(withEntityType); } break;
                    case 12:
                        { retValue = GetPhoneNumberReportParam(withEntityType); } break;
                    case 13:
                        { retValue = GetStringReportParam(withEntityType); } break;
                    case 14:
                        { retValue = GetSocialSecurityNumberReportParam(withEntityType); } break;
                    case 15:
                        { retValue = GetTimeElapsedReportParam(withEntityType); } break;
                    case 16:
                        { retValue = GetTimeOfDayReportParam(withEntityType); } break;
                    default:
                        { retValue = GetStringReportParam(withEntityType); } break;
                }
                retValues.Add(retValue);
            }
            return retValues;
        }


        public static PC.ReportParam GetRandomReportParameter(bool withEntityTypeTag=true)
        {
            Random rand = new Random(DateTime.Now.Millisecond);
            int choice = rand.Next(1, 16);

            PC.ReportParam retValue;

            switch (choice)
            {
                case 1:
                    { retValue = GetBooleanReportParameter(withEntityTypeTag); } break;
                case 2:
                    { retValue = GetCurrencyReportParameter(withEntityTypeTag); } break;
                case 3:
                    { retValue = GetDecimalReportParameter(withEntityTypeTag); } break;
                case 4:
                    { retValue = GetDateMonthDayReportParam(withEntityTypeTag); } break;
                case 5:
                    { retValue = GetDateMonthYearReportParameter(withEntityTypeTag); } break;
                case 6:
                    { retValue = GetDateReportParam(withEntityTypeTag); } break;
                case 7:
                    { retValue = GetDateTimeReportParam(withEntityTypeTag); } break;
                case 8:
                    { retValue = GetInformationalTextReportParam(withEntityTypeTag); } break;
                case 9:
                    { retValue = GetMultiSelectReportParam(withEntityTypeTag); } break;
                case 10:
                    { retValue = GetSingleSelectReportParam(withEntityTypeTag); } break;
                case 11:
                    { retValue = GetPercentageReportParameter(withEntityTypeTag); } break;
                case 12:
                    { retValue = GetPhoneNumberReportParam(withEntityTypeTag); } break;
                case 13:
                    { retValue = GetStringReportParam(withEntityTypeTag); } break;
                case 14:
                    { retValue = GetSocialSecurityNumberReportParam(withEntityTypeTag); } break;
                case 15:
                    { retValue = GetTimeElapsedReportParam(withEntityTypeTag); } break;
                case 16:
                    { retValue = GetTimeOfDayReportParam(withEntityTypeTag); } break;
                default:
                    { retValue = GetStringReportParam(withEntityTypeTag); } break;
            }
            return retValue;
        }

        public static PC.StringReportParam[] GetStringReportParam(int qty)
        {
            PC.StringReportParam[] list = new PC.StringReportParam[qty];

            for (int i = 0; i < qty; i++)
            {
                PC.StringReportParam item = new PC.StringReportParam(
                    i.ToString() + "_" + name1,
                    i.ToString() + "_" + displayText1,
                    required2,
                    i.ToString() + "_" + justText1,
                    entityType1,
                    i.ToString() + "_" + displayText1,
                    intValue1
                    );
                list[i] = item;
            }
            return list;
        }

        public static PC.Vendor[] GetVendor(int qty)
        {
            PC.Vendor[] list = new PC.Vendor[qty];

            for (int i = 0; i < qty; i++)
            {
                PC.Vendor item = new PC.Vendor(
                    uniqueId1,
                    companyUniqueId1,
                    name1,
                    description1,
                    "vendorType1",
                    address1_1,
                    address2_1,
                    city1,
                    state1,
                    country1,
                    zip1,
                    phone1,
                    fax1,
                    mobile1,
                    "status1");
                list[i] = item;
            }
            return list;
        }

        public static PC.Job[] GetJob(int qty)
        {
            PC.Job[] list = new PC.Job[qty];

            for (int i = 0; i < qty; i++)
            {
                PC.Job item = new PC.Job(
                    uniqueId1,
                    name1,
                    description1,
                    address1_1,
                    address2_1,
                    city1,
                    state1,
                    country1,
                    zip1,
                    phone1,
                    fax1,
                    mobile1,
                    "status1");

                list[i] = item;
            }
            return list;
        }
        
        public static List<PC.SingleValueSystemFilterParam> GetSingleValueSystemFilterParam(PC.ReportParam reportParam)
        {
            PC.SingleValueSystemFilterParam p = new PC.SingleValueSystemFilterParam(reportParam);
            return new List<PC.SingleValueSystemFilterParam> { p };
        }

        public static List<PC.SingleValueSystemFilterParamValue> GetSingleValueSystemFilterParamValue(PC.ReportParamValue reportParamValue)
        {
            PC.SingleValueSystemFilterParamValue p = new PC.SingleValueSystemFilterParamValue(reportParamValue);
            return new List<PC.SingleValueSystemFilterParamValue> { p };

        }


        public static PC.BooleanReportParamValue GetBooleanReportParamValue()
        {
            return new PC.BooleanReportParamValue(name1, justText1, true);
        }
        public static PC.DateMonthDayReportParamValue GetDateMonthDayReportParamValue()
        {
            return new PC.DateMonthDayReportParamValue(name1, justText1, date1);
        }
        public static PC.DateMonthYearReportParamValue GetDateMonthYearReportParamValue()
        {
            return new PC.DateMonthYearReportParamValue(name1, justText1, date1);
        }
        public static PC.DateReportParamValue GetDateReportParamValue()
        {
            return new PC.DateReportParamValue(name1, justText1, date1);
        }
        public static PC.DateTimeReportParamValue GetDateTimeReportParamValue()
        {
            return new PC.DateTimeReportParamValue(name1, justText1, date1);
        }
        public static PC.DecimalReportParamValue GetDecimalReportParamValue()
        {
            return new PC.DecimalReportParamValue(name1, justText1, 2999.22m);
        }
        public static PC.MultiSelectReportParamValue GetMultiSelectReportParamValue()
        {
            return new PC.MultiSelectReportParamValue(name1, justText1, GetListOfStrings(8));
        }
        public static PC.CurrencyReportParamValue GetCurrencyReportParamValue(bool getDefault = true)
        {
            if (getDefault)
                return new PC.CurrencyReportParamValue(name1, justText1, 33.01m);
            else
                return new PC.CurrencyReportParamValue(name2, justText2, 552.00m);
        }

        #endregion

        #region Cloud report param values

        public static string[] BasicIgnoreList()
        {
            List<string> ignoreList = new List<string> {"ExtensionData"};
            return ignoreList.ToArray();
        }

        public static List<CC.SingleValueSystemFilterParamValue> GetCloudSingleValueSystemFilterParamValue(CC.ReportParamValue reportParamValue)
        {
            CC.SingleValueSystemFilterParamValue p = new CC.SingleValueSystemFilterParamValue(reportParamValue);
            return new List<CC.SingleValueSystemFilterParamValue> { p };
        }
        public static List<CC.SingleValueSystemFilterParamValue> GetCloudListOfSingleValueSystemFilterParamValue(List<CC.ReportParamValue> reportParamValues)
        {
            List<CC.SingleValueSystemFilterParamValue> list = new List<CC.SingleValueSystemFilterParamValue>();
            foreach (CC.ReportParamValue item in reportParamValues)
            {
                list.Add(new CC.SingleValueSystemFilterParamValue(item));
            }
            return list;
        }

        public static CC.BooleanReportParamValue GetCloudBooleanReportParamValue()
        {
            return new CC.BooleanReportParamValue(name1, justText1, true);
        }
        public static CC.DateMonthDayReportParamValue GetCloudDateMonthDayReportParamValue()
        {
            return new CC.DateMonthDayReportParamValue(name1, justText1, date1);
        }
        public static CC.DateMonthYearReportParamValue GetCloudDateMonthYearReportParamValue()
        {
            return new CC.DateMonthYearReportParamValue(name1, justText1, date1);
        }
        public static CC.DateReportParamValue GetCloudDateReportParamValue()
        {
            return new CC.DateReportParamValue(name1, justText1, date1);
        }
        public static CC.DateTimeReportParamValue GetCloudDateTimeReportParamValue()
        {
            return new CC.DateTimeReportParamValue(name1, justText1, date1);
        }
        public static CC.DecimalReportParamValue GetCloudDecimalReportParamValue()
        {
            return new CC.DecimalReportParamValue(name1, justText1, 299.99m);
        }
        public static CC.MultiSelectReportParamValue GetCloudMultiSelectReportParamValue()
        {
            return new CC.MultiSelectReportParamValue(name1, justText1, GetListOfStrings(8));
        }
        public static CC.CurrencyReportParamValue GetCloudCurrencyReportParamValue(bool getDefault = true)
        {
            if (getDefault)
                return new CC.CurrencyReportParamValue(name1, justText1, 33.01m);
            else
                return new CC.CurrencyReportParamValue(name2, justText2, 552.00m);
        }
        public static CC.PercentageReportParamValue GetCloudPercentageParamValue()
        {
            return new CC.PercentageReportParamValue(name1, justText1, 34.8m);
        }
        public static CC.SocialSecurityNumberReportParamValue GetCloudSocialSecurityNumberParamValue()
        {
            return new CC.SocialSecurityNumberReportParamValue(name1, justText1, 540787890);
        }

        public static CC.BooleanReportParamValue GetNullCloudBooleanReportParamValue()
        {
            return new CC.BooleanReportParamValue(name1, justText1, null);
        }
        public static CC.DateMonthDayReportParamValue GetNullCloudDateMonthDayReportParamValue()
        {
            return new CC.DateMonthDayReportParamValue(name1, justText1, null);
        }
        public static CC.DateMonthYearReportParamValue GetNullCloudDateMonthYearReportParamValue()
        {
            return new CC.DateMonthYearReportParamValue(name1, justText1, null);
        }
        public static CC.DateReportParamValue GetNullCloudDateReportParamValue()
        {
            return new CC.DateReportParamValue(name1, justText1, null);
        }
        public static CC.DateTimeReportParamValue GetNullCloudDateTimeReportParamValue()
        {
            return new CC.DateTimeReportParamValue(name1, justText1, null);
        }
        public static CC.DecimalReportParamValue GetNullCloudDecimalReportParamValue()
        {
            return new CC.DecimalReportParamValue(name1, justText1, null);
        }
        public static CC.MultiSelectReportParamValue GetNullCloudMultiSelectReportParamValue()
        {
            return new CC.MultiSelectReportParamValue(name1, justText1, null);
        }
        public static CC.CurrencyReportParamValue GetNullCloudCurrencyReportParamValue(bool getDefault = true)
        {
            if (getDefault)
                return new CC.CurrencyReportParamValue(name1, justText1, null);
            else
                return new CC.CurrencyReportParamValue(name2, justText2, null);
        }
        public static CC.PercentageReportParamValue GetNullCloudPercentageParamValue()
        {
            return new CC.PercentageReportParamValue(name1, justText1, null);
        }
        public static CC.SocialSecurityNumberReportParamValue GetNullCloudSocialSecurityNumberParamValue()
        {
            return new CC.SocialSecurityNumberReportParamValue(name1, justText1, null);
        }

        #endregion

    }
}
