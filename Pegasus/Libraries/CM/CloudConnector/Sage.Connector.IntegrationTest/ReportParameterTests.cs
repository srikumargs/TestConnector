using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sage.Connector.TestUtilities;
using CloudContracts = Sage.Connector.Cloud.Integration.Interfaces.DataContracts;
using Convert = Sage.Connector.ConnectorServiceCommon.Convert;
using PremiseContracts = Sage.CRE.CloudConnector.Integration.Interfaces.DataContracts;


namespace Sage.Connector.IntegrationTest
{
    /// <summary>
    ///This is a test class for ReportTest and is intended
    ///to contain all ReportTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ReportParameterTests
    {
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
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
        }

        [TestCleanup]
        public void MyTestCleanup()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.StopHostingFx, TestUtils.UnitTestSetupConnectorMonitorServiceFlags.StopHostingFx);
        }

        #endregion

        [TestMethod]
        public void Test_IntegerTypeMapping()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.None);

            string name = "name";
            string displayText = "display";
            string metadata = "metadata";
            bool required = true;
            decimal defaultValue = 100m;
            int scale = 5;
            int precision = 2;
            decimal pos_minValue = 0;
            decimal pos_maxValue = 100;
            decimal neg_minValue = -100;
            decimal neg_maxValue= 0;
            decimal both_minValue = -100;
            decimal both_maxValue = 100;
            PremiseContracts.ReportParameterIntegerTypes intType = PremiseContracts.ReportParameterIntegerTypes.None;
            bool commaGroup = true;
            bool disallowZero = true;
            bool showZeroAsBlank = true;

            PremiseContracts.PercentageReportParam percentageParam = new PremiseContracts.PercentageReportParam(
                name,
                displayText,
                required,
                metadata,
                defaultValue,
                scale,
                precision,
                pos_minValue,
                pos_maxValue,
                intType,
                commaGroup,
                disallowZero,
                showZeroAsBlank);
            PremiseContracts.CurrencyReportParam currencyParam = new PremiseContracts.CurrencyReportParam(
                name,
                displayText,
                required,
                metadata,
                defaultValue,
                scale,
                precision,
                pos_minValue,
                pos_maxValue,
                intType,
                commaGroup,
                disallowZero,
                showZeroAsBlank);
            PremiseContracts.DecimalReportParam decimalParam = new PremiseContracts.DecimalReportParam(
                name,
                displayText,
                required,
                metadata,
                defaultValue,
                scale,
                precision,
                pos_minValue,
                pos_maxValue,
                intType,
                commaGroup,
                disallowZero,
                showZeroAsBlank);

            PremiseContracts.ReportDescriptor rd = new PremiseContracts.ReportDescriptor(
                "ABC", "DEF", "GHI", "JKL", "MNO", "QRS", "TUV", new PremiseContracts.ReportParam[] { percentageParam, currencyParam, decimalParam }, new PremiseContracts.SystemFilterParam[] { });

            CloudContracts.ReportDescriptor cloud_rd = Convert.ToCloudReportDescriptor(rd);
            Assert.IsNotNull(cloud_rd.ReportParams);
            Assert.IsTrue(cloud_rd.ReportParams[0] is CloudContracts.PercentageReportParam);
            Assert.IsTrue((cloud_rd.ReportParams[0] as CloudContracts.PercentageReportParam).IntegerType == CloudContracts.ReportParameterIntegerTypes.PositiveOnly);
            Assert.IsTrue(cloud_rd.ReportParams[1] is CloudContracts.CurrencyReportParam);
            Assert.IsTrue((cloud_rd.ReportParams[1] as CloudContracts.CurrencyReportParam).IntegerType == CloudContracts.ReportParameterIntegerTypes.PositiveOnly);
            Assert.IsTrue(cloud_rd.ReportParams[2] is CloudContracts.DecimalReportParam);
            Assert.IsTrue((cloud_rd.ReportParams[2] as CloudContracts.DecimalReportParam).IntegerType == CloudContracts.ReportParameterIntegerTypes.PositiveOnly);

            percentageParam = new PremiseContracts.PercentageReportParam(
                name,
                displayText,
                required,
                metadata,
                defaultValue,
                scale,
                precision,
                neg_minValue,
                neg_maxValue,
                intType,
                commaGroup,
                disallowZero,
                showZeroAsBlank);
            currencyParam = new PremiseContracts.CurrencyReportParam(
                name,
                displayText,
                required,
                metadata,
                defaultValue,
                scale,
                precision,
                neg_minValue,
                neg_maxValue,
                intType,
                commaGroup,
                disallowZero,
                showZeroAsBlank);
            decimalParam = new PremiseContracts.DecimalReportParam(
                name,
                displayText,
                required,
                metadata,
                defaultValue,
                scale,
                precision,
                neg_minValue,
                neg_maxValue,
                intType,
                commaGroup,
                disallowZero,
                showZeroAsBlank);

            rd = new PremiseContracts.ReportDescriptor(
                "ABC", "DEF", "GHI", "JKL", "MNO", "QRS", "TUV", new PremiseContracts.ReportParam[] { percentageParam, currencyParam, decimalParam }, new PremiseContracts.SystemFilterParam[] { });

            cloud_rd = Convert.ToCloudReportDescriptor(rd);
            Assert.IsNotNull(cloud_rd.ReportParams);
            Assert.IsTrue(cloud_rd.ReportParams[0] is CloudContracts.PercentageReportParam);
            Assert.IsTrue((cloud_rd.ReportParams[0] as CloudContracts.PercentageReportParam).IntegerType == CloudContracts.ReportParameterIntegerTypes.NegativeOnly);
            Assert.IsTrue(cloud_rd.ReportParams[1] is CloudContracts.CurrencyReportParam);
            Assert.IsTrue((cloud_rd.ReportParams[1] as CloudContracts.CurrencyReportParam).IntegerType == CloudContracts.ReportParameterIntegerTypes.NegativeOnly);
            Assert.IsTrue(cloud_rd.ReportParams[2] is CloudContracts.DecimalReportParam);
            Assert.IsTrue((cloud_rd.ReportParams[2] as CloudContracts.DecimalReportParam).IntegerType == CloudContracts.ReportParameterIntegerTypes.NegativeOnly);

            percentageParam = new PremiseContracts.PercentageReportParam(
                name,
                displayText,
                required,
                metadata,
                defaultValue,
                scale,
                precision,
                both_minValue,
                both_maxValue,
                intType,
                commaGroup,
                disallowZero,
                showZeroAsBlank);
            currencyParam = new PremiseContracts.CurrencyReportParam(
                name,
                displayText,
                required,
                metadata,
                defaultValue,
                scale,
                precision,
                both_minValue,
                both_maxValue,
                intType,
                commaGroup,
                disallowZero,
                showZeroAsBlank);
            decimalParam = new PremiseContracts.DecimalReportParam(
                name,
                displayText,
                required,
                metadata,
                defaultValue,
                scale,
                precision,
                both_minValue,
                both_maxValue,
                intType,
                commaGroup,
                disallowZero,
                showZeroAsBlank);

            rd = new PremiseContracts.ReportDescriptor(
                "ABC", "DEF", "GHI", "JKL", "MNO", "QRS", "TUV", new PremiseContracts.ReportParam[] { percentageParam, currencyParam, decimalParam }, new PremiseContracts.SystemFilterParam[] { });

            cloud_rd = Convert.ToCloudReportDescriptor(rd);
            Assert.IsNotNull(cloud_rd.ReportParams);
            Assert.IsTrue(cloud_rd.ReportParams[0] is CloudContracts.PercentageReportParam);
            Assert.IsTrue((cloud_rd.ReportParams[0] as CloudContracts.PercentageReportParam).IntegerType == CloudContracts.ReportParameterIntegerTypes.Both);
            Assert.IsTrue(cloud_rd.ReportParams[1] is CloudContracts.CurrencyReportParam);
            Assert.IsTrue((cloud_rd.ReportParams[1] as CloudContracts.CurrencyReportParam).IntegerType == CloudContracts.ReportParameterIntegerTypes.Both);
            Assert.IsTrue(cloud_rd.ReportParams[2] is CloudContracts.DecimalReportParam);
            Assert.IsTrue((cloud_rd.ReportParams[2] as CloudContracts.DecimalReportParam).IntegerType == CloudContracts.ReportParameterIntegerTypes.Both);
        }
    }
}
