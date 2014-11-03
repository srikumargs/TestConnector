using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sage.Connector.TestUtilities;
using CloudContracts = Sage.Connector.Cloud.Integration.Interfaces.DataContracts;
using PremiseContracts = Sage.CRE.CloudConnector.Integration.Interfaces.DataContracts;

namespace Sage.Connector.ConnectorServiceCommon.Test
{
    /// <summary>
    /// Summary description for CloudConnectorCommonTests
    /// </summary>
    [TestClass]
    public class CloudConnectorCommonTests
    {
        public CloudConnectorCommonTests()
        {
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx);
        }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestNullableReportParameterValues()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.None);

            List<CloudContracts.ReportParamValue> rpvs = new List<CloudContracts.ReportParamValue>();

            CloudContracts.TimeElapsedReportParamValue terpv = new CloudContracts.TimeElapsedReportParamValue("TERPV", "PREMISE", null);
            rpvs.Add(terpv);
            CloudContracts.TimeOfDayReportParamValue todrpv = new CloudContracts.TimeOfDayReportParamValue("TODRPV", "PREMISE", null);
            rpvs.Add(todrpv);
            CloudContracts.CurrencyReportParamValue crpv = new CloudContracts.CurrencyReportParamValue("CRPV", "PREMISE", null);
            rpvs.Add(crpv);
            CloudContracts.DateMonthDayReportParamValue dmdrpv = new CloudContracts.DateMonthDayReportParamValue("DMDRPV", "PREMISE", null);
            rpvs.Add(dmdrpv);
            CloudContracts.DateMonthYearReportParamValue dmyrpv = new CloudContracts.DateMonthYearReportParamValue("DMYRPV", "PREMISE", null);
            rpvs.Add(dmyrpv);
            CloudContracts.DateReportParamValue drpv = new CloudContracts.DateReportParamValue("DRPV", "PREMISE", null);
            rpvs.Add(drpv);
            CloudContracts.DateTimeReportParamValue dtrpv = new CloudContracts.DateTimeReportParamValue("DTRPV", "PREMISE", null);
            rpvs.Add(dtrpv);
            CloudContracts.BooleanReportParamValue brpv = new CloudContracts.BooleanReportParamValue("BRPV", "PREMISE", null);
            rpvs.Add(brpv);
            CloudContracts.DecimalReportParamValue decrpv = new CloudContracts.DecimalReportParamValue("DECRPV", "PREMISE", null);
            rpvs.Add(decrpv);
            CloudContracts.PercentageReportParamValue prpv = new CloudContracts.PercentageReportParamValue("PRPV", "PREMISE", null);
            rpvs.Add(prpv);
            CloudContracts.SocialSecurityNumberReportParamValue ssnrpv = new CloudContracts.SocialSecurityNumberReportParamValue("SSNRPV", "PREMISE", null);
            rpvs.Add(ssnrpv);
            
            PremiseContracts.ReportParamValue[] premise_rpvs_list = Convert.ToPremiseParameterValues(rpvs.ToArray());
            Assert.IsTrue(rpvs.Count == premise_rpvs_list.Length);

            foreach (PremiseContracts.ReportParamValue value in premise_rpvs_list)
            {
                switch (value.Name)
                {
                    case "TERPV" :
                        Assert.IsTrue(value is PremiseContracts.TimeElapsedReportParamValue);
                        break;
                    case "TODRPV" :
                        Assert.IsTrue(value is PremiseContracts.TimeOfDayReportParamValue);
                        break;
                    case "CRPV" :
                        Assert.IsTrue(value is PremiseContracts.CurrencyReportParamValue);
                        break;
                    case "DMDRPV" :
                        Assert.IsTrue(value is PremiseContracts.DateMonthDayReportParamValue);
                        break;
                    case "DMYRPV" :
                        Assert.IsTrue(value is PremiseContracts.DateMonthYearReportParamValue);
                        break;
                    case "DRPV" :
                        Assert.IsTrue(value is PremiseContracts.DateReportParamValue);
                        break;
                    case "DTRPV" :
                        Assert.IsTrue(value is PremiseContracts.DateTimeReportParamValue);
                        break;
                    case "BRPV" :
                        Assert.IsTrue(value is PremiseContracts.BooleanReportParamValue);
                        break;
                    case "DECRPV":
                        Assert.IsTrue(value is PremiseContracts.DecimalReportParamValue);
                        break;
                    case "PRPV" :
                        Assert.IsTrue(value is PremiseContracts.PercentageReportParamValue);
                        break;
                    case "SSNRPV" :
                        Assert.IsTrue(value is PremiseContracts.SocialSecurityNumberReportParamValue);
                        break;
                    default :
                        Assert.Fail("Unexpected premise report value.");
                        break;
                }
            }
        }

        [TestMethod]
        public void TestReportCurrencyParameterMapper()
        {
            TestReportNumberParameterMapper(true);
        }

        [TestMethod]
        public void TestReportNumericParameterMapper()
        {
            TestReportNumberParameterMapper(false);
        }

        private void TestReportNumberParameterMapper(bool isCurrency)
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.None);

            List<PremiseContracts.ReportParam> parameters = new List<PremiseContracts.ReportParam>();
            parameters.Add(ReportParameterTestDefaults.GetNumericDefaults(isCurrency));

            PremiseContracts.ReportDescriptor theReport = ReportParameterTestDefaults.GetReportDefaults(parameters.ToArray());

            CloudContracts.ReportDescriptor[] cloudReport = Convert.ToCloudReportDescriptors(new PremiseContracts.ReportDescriptor[] { theReport });
            Assert.IsNotNull(cloudReport, "ReportDescriptor list came back null");
            Assert.IsTrue(cloudReport.Length > 0, "ReportDescriptor list is empty.");

            Assert.AreEqual(cloudReport[0].ApplicationName, theReport.ApplicationName, "Report property does not match");
            Assert.AreEqual(cloudReport[0].Description, theReport.Description, "Report property does not match");
            Assert.AreEqual(cloudReport[0].MenuPath, theReport.MenuPath, "Report property does not match"); 
            Assert.AreEqual(cloudReport[0].Category, theReport.Category, "Report property does not match"); 
            Assert.AreEqual(cloudReport[0].Name, theReport.Name, "Report property does not match");
            Assert.AreEqual(cloudReport[0].Path, theReport.Path, "Report property does not match");
            Assert.AreEqual(cloudReport[0].UniqueIdentifier, theReport.UniqueIdentifier, "Report property does not match");

            Assert.IsNotNull(cloudReport[0].ReportParams, "Report parameters null");
            Assert.AreEqual(1, cloudReport[0].ReportParams.Length, "Expected 1 report parameter");
            if (isCurrency)
            {
                CloudContracts.CurrencyReportParam cloudParam = cloudReport[0].ReportParams[0] as CloudContracts.CurrencyReportParam;
                Assert.IsInstanceOfType(cloudParam, typeof(CloudContracts.CurrencyReportParam), "Report parameter type is incorrect");
                PremiseContracts.NumberReportParam premParam = theReport.ReportParams[0] as PremiseContracts.NumberReportParam;

                //Assert.AreEqual(premParam.SelectionValues, cloudParam.DefinedValues);
                Assert.AreEqual(premParam.DefaultValue, cloudParam.DefaultValue, "Parameter property does not match");
                Assert.AreEqual(premParam.IsRequired, cloudParam.IsRequired, "Parameter property does not match");
                Assert.AreEqual(premParam.MaximumValue, cloudParam.MaximumValue, "Parameter property does not match");
                Assert.AreEqual(premParam.MinimumValue, cloudParam.MinimumValue, "Parameter property does not match");
                Assert.AreEqual(premParam.Name, cloudParam.Name, "Parameter property does not match");
                Assert.AreEqual(premParam.Precision, cloudParam.Precision, "Parameter property does not match");
                Assert.AreEqual(premParam.Scale, cloudParam.Scale, "Parameter property does not match");

            }
            else
            {
                CloudContracts.NumberReportParam cloudParam = cloudReport[0].ReportParams[0] as CloudContracts.NumberReportParam;
                Assert.IsInstanceOfType(cloudParam, typeof(CloudContracts.NumberReportParam), "Report parameter type is incorrect");
                PremiseContracts.NumberReportParam premParam = theReport.ReportParams[0] as PremiseContracts.NumberReportParam;

                //Assert.AreEqual(premParam.SelectionValues, cloudParam.DefinedValues);
                Assert.AreEqual(premParam.DefaultValue, cloudParam.DefaultValue, "Parameter property does not match");
                Assert.AreEqual(premParam.IsRequired, cloudParam.IsRequired, "Parameter property does not match");
                Assert.AreEqual(premParam.MaximumValue, cloudParam.MaximumValue, "Parameter property does not match");
                Assert.AreEqual(premParam.MinimumValue, cloudParam.MinimumValue, "Parameter property does not match");
                Assert.AreEqual(premParam.Name, cloudParam.Name, "Parameter property does not match");
                Assert.AreEqual(premParam.Precision, cloudParam.Precision, "Parameter property does not match");
                Assert.AreEqual(premParam.Scale, cloudParam.Scale, "Parameter property does not match");
            }
        }

        [TestMethod]
        public void TestReportBooleanParameterMapper()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.None);

            List<PremiseContracts.ReportParam> parameters = new List<PremiseContracts.ReportParam>();

            PremiseContracts.BooleanReportParam param = ReportParameterTestDefaults.GetBooleanDefaults();
            parameters.Add(param);
            PremiseContracts.ReportDescriptor boolReport = ReportParameterTestDefaults.GetReportDefaults(parameters.ToArray());

            CloudContracts.ReportDescriptor[] cloudReport = Convert.ToCloudReportDescriptors(new PremiseContracts.ReportDescriptor[] { boolReport });
            Assert.IsNotNull(cloudReport, "ReportDescriptor list came back null");
            Assert.IsTrue(cloudReport.Length > 0, "ReportDescriptor list is empty.");

            Assert.AreEqual(cloudReport[0].ApplicationName, boolReport.ApplicationName, "Report property does not match");
            Assert.AreEqual(cloudReport[0].Description, boolReport.Description, "Report property does not match");
            Assert.AreEqual(cloudReport[0].MenuPath, boolReport.MenuPath, "Report property does not match"); 
            Assert.AreEqual(cloudReport[0].Category, boolReport.Category, "Report property does not match"); 
            Assert.AreEqual(cloudReport[0].Name, boolReport.Name, "Report property does not match");
            Assert.AreEqual(cloudReport[0].Path, boolReport.Path, "Report property does not match");
            Assert.AreEqual(cloudReport[0].UniqueIdentifier, boolReport.UniqueIdentifier, "Report property does not match");

            Assert.IsNotNull(cloudReport[0].ReportParams, "Report parameters null");
            Assert.AreEqual(1, cloudReport[0].ReportParams.Length, "Expected 1 report parameter");
            CloudContracts.BooleanReportParam cloudParam = cloudReport[0].ReportParams[0] as CloudContracts.BooleanReportParam;
            Assert.IsInstanceOfType(cloudParam, typeof(CloudContracts.BooleanReportParam), "Report parameter type is incorrect");
            PremiseContracts.BooleanReportParam premParam = boolReport.ReportParams[0] as PremiseContracts.BooleanReportParam;

            Assert.AreEqual(premParam.DefaultValue, cloudParam.DefaultValue, "Parameter property does not match");
            Assert.AreEqual(premParam.IsRequired, cloudParam.IsRequired, "Parameter property does not match");
            Assert.AreEqual(premParam.Name, cloudParam.Name, "Parameter property does not match");
        }

        [TestMethod]
        public void TestReportDateParameterMapper()
        {
            DateTimeParameterTester();
        }

        private void DateTimeParameterTester()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.None);

            List<PremiseContracts.ReportParam> parameters = new List<PremiseContracts.ReportParam>();
            parameters.Add(ReportParameterTestDefaults.GetDateDefaults());

            PremiseContracts.ReportDescriptor theReport = ReportParameterTestDefaults.GetReportDefaults(parameters.ToArray());

            CloudContracts.ReportDescriptor[] cloudReport = Convert.ToCloudReportDescriptors(new PremiseContracts.ReportDescriptor[] { theReport });
            Assert.IsNotNull(cloudReport, "ReportDescriptor list came back null");
            Assert.IsTrue(cloudReport.Length > 0, "ReportDescriptor list is empty.");

            Assert.AreEqual(cloudReport[0].ApplicationName, theReport.ApplicationName, "Report property does not match");
            Assert.AreEqual(cloudReport[0].Description, theReport.Description, "Report property does not match");
            Assert.AreEqual(cloudReport[0].MenuPath, theReport.MenuPath, "Report property does not match"); 
            Assert.AreEqual(cloudReport[0].Category, theReport.Category, "Report property does not match"); 
            Assert.AreEqual(cloudReport[0].Name, theReport.Name, "Report property does not match");
            Assert.AreEqual(cloudReport[0].Path, theReport.Path, "Report property does not match");
            Assert.AreEqual(cloudReport[0].UniqueIdentifier, theReport.UniqueIdentifier, "Report property does not match");

            Assert.IsNotNull(cloudReport[0].ReportParams, "Report parameters null");
            Assert.AreEqual(1, cloudReport[0].ReportParams.Length, "Expected 1 report parameter");
            CloudContracts.DateTimeReportParam cloudParam = cloudReport[0].ReportParams[0] as CloudContracts.DateTimeReportParam;
            Assert.IsInstanceOfType(cloudParam, typeof(CloudContracts.DateTimeReportParam), "Report parameter type is incorrect");
            PremiseContracts.DateTimeReportParam premParam = theReport.ReportParams[0] as PremiseContracts.DateTimeReportParam;

            Assert.AreEqual(premParam.DefaultValue, cloudParam.DefaultValue, "Parameter property does not match");
            Assert.AreEqual(premParam.IsRequired, cloudParam.IsRequired, "Parameter property does not match");
            Assert.AreEqual(premParam.Name, cloudParam.Name, "Parameter property does not match");
        }


        public static class ReportParameterTestDefaults
        {
            //static fields
            private static DateTime DEFAULT_DATETIME = new DateTime(2011, 1, 1);
            private static DateTime DEFAULT_MINDATETIME = new DateTime(2000, 1, 1);
            private static DateTime DEFAULT_MAXDATETIME = new DateTime(2099, 1, 1);


            public static PremiseContracts.ReportDescriptor GetReportDefaults(PremiseContracts.ReportParam[] reportParams)
            {
                PremiseContracts.ReportDescriptor r = new PremiseContracts.ReportDescriptor("111111-111111-111111-111111-111111-111111", "ReportName", "Report Description", "Category", "Application Name", "basepath/subpath", "D:\\FilePath", reportParams, new PremiseContracts.SystemFilterParam[] { });
                return r;
            }

            public static PremiseContracts.StringReportParam GetStringDefaults()
            {
                PremiseContracts.StringReportParam p = new PremiseContracts.StringReportParam(
                    "Name",
                    "Prompt",
                    false,
                    null,
                    "DefaultValue",
                    64);
                return p;
            }

            public static PremiseContracts.BooleanReportParam GetBooleanDefaults()
            {
                PremiseContracts.BooleanReportParam p = new PremiseContracts.BooleanReportParam(
                    "Name",
                    "Prompt",
                    false,
                    null,
                    false);
                return p;
            }


            public static PremiseContracts.NumberReportParam GetNumericDefaults(bool currency)
            {
                PremiseContracts.NumberReportParam p = new PremiseContracts.DecimalReportParam(
                    "Name",
                    "Prompt",
                    false,
                    null,
                    1m,
                    1,
                    2,
                    -100m,
                    10000m,
                    PremiseContracts.ReportParameterIntegerTypes.Both,
                    false,
                    false,
                    false);

                if (currency)
                {
                    p = new PremiseContracts.CurrencyReportParam(
                    "Name",
                    "Prompt",
                    false,
                    null,
                    1m,
                    1,
                    2,
                    -100m,
                    10000m,
                    PremiseContracts.ReportParameterIntegerTypes.Both,
                    false,
                    false,
                    false);
                }

                return p;
            }

            public static PremiseContracts.TimeElapsedReportParam GetElapsedTimeDefaults()
            {
                PremiseContracts.TimeElapsedReportParam p = new PremiseContracts.TimeElapsedReportParam(
                    "Name",
                    "Prompt",
                    false,
                    null,
                    new TimeSpan(DEFAULT_DATETIME.Ticks),
                    new TimeSpan(DEFAULT_MINDATETIME.Ticks),
                    new TimeSpan(DEFAULT_MAXDATETIME.Ticks));
                return p;
            }

            public static PremiseContracts.DateTimeReportParam GetDateDefaults()
            {
                PremiseContracts.DateTimeReportParam p = new PremiseContracts.DateTimeReportParam(
                    "Name",
                    "Prompt",
                    false,
                    null,
                    DEFAULT_DATETIME,
                    DEFAULT_MINDATETIME,
                    DEFAULT_MAXDATETIME,
                    true);
                return p;
            }
        }
    }
}
