using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sage.Connector.TestUtilities;
using Sage.CRE.CloudConnector.Integration.Interfaces.DataContracts;
using Sage.CRE.CloudConnector.Integration.Interfaces.Utils;
using CC = Sage.Connector.Cloud.Integration.Interfaces.DataContracts;
using PC = Sage.CRE.CloudConnector.Integration.Interfaces.DataContracts;


namespace Sage.Connector.IntegrationTest
{
    /// <summary>
    ///This is a test class for CompanyTest and is intended
    ///to contain all CompanyTest Unit Tests
    ///</summary>
    [TestClass]
    public class ContractMapperTests
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
        public void TestMapper_SystemFilterParam()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.None);
            List<PC.ReportParam> reportParams = DataContractStaticDataClass.GetReportParameters(16, true);
            List<PC.SystemFilterParam> premiseList = new List<PC.SystemFilterParam>();
            
            //Add know type first then add all the rest.
            foreach (PC.ReportParam param in reportParams)
            {
                premiseList.AddRange(DataContractStaticDataClass.GetSingleValueSystemFilterParam(param));
            }

            //Convert to cloud parameters
            CC.SystemFilterParam[] cloudArray;
            cloudArray = ConnectorServiceCommon.Convert.ToCloudSystemFilterParam(premiseList.ToArray());
            List<CC.SystemFilterParam> cloudList = new List<CC.SystemFilterParam>(cloudArray);

            //test that premise and cloud versions are equal
            PropertyComparisonResults results = new PropertyComparisonResults();
            bool test = DataObjectComparisonUtil.AreObjectsEqual(premiseList, cloudList, ref results, true, DataContractStaticDataClass.PropertyIgnoreList());
            ValidateComparisons(results);

            results.Clear();
            
            //invalidate the premise guy and reconvert
            //get index of Percentage report param
            int index = reportParams.FindIndex(0, x => x.GetType() == typeof (PercentageReportParam));

            //PC.PercentageReportParam altered = reportParams[index];
            //Assert.IsNotNull(altered, "Parameter should be of type PercentageReportParameter");
            PropertyTuple property = new PropertyTuple(reportParams[index].PropertyInfo(x => x.Name), "Bad Name");

            PC.PercentageReportParam newParam = new PC.PercentageReportParam(reportParams[index] as PercentageReportParam, new List<PropertyTuple> { property });
            premiseList[10] = DataContractStaticDataClass.GetSingleValueSystemFilterParam(newParam).First();
            
            test = DataObjectComparisonUtil.AreObjectsEqual(premiseList, cloudList, ref results, true, DataContractStaticDataClass.PropertyIgnoreList());
            Assert.IsFalse(test, "Should have been invalidated with this issue.");

            //re convert and retest
            cloudArray = ConnectorServiceCommon.Convert.ToCloudSystemFilterParam(premiseList.ToArray());
            cloudList = new List<CC.SystemFilterParam>(cloudArray);

            results.Clear();
            test = DataObjectComparisonUtil.AreObjectsEqual(premiseList, cloudList, ref results, true, DataContractStaticDataClass.PropertyIgnoreList());
            ValidateComparisons(results);
        }

        [TestMethod]
        public void TestMapper_PremiseSystemFilterValuesNullValues()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.None); 
            
            int initialCount = 0;
            List<CC.SystemFilterParamValue> cloudList = new List<CC.SystemFilterParamValue>();

            List<CC.ReportParamValue> paramValues = new List<CC.ReportParamValue>();
            paramValues.Add(DataContractStaticDataClass.GetNullCloudBooleanReportParamValue());
            paramValues.Add(DataContractStaticDataClass.GetNullCloudCurrencyReportParamValue());
            paramValues.Add(DataContractStaticDataClass.GetNullCloudDateMonthDayReportParamValue());
            paramValues.Add(DataContractStaticDataClass.GetNullCloudDateMonthYearReportParamValue());
            paramValues.Add(DataContractStaticDataClass.GetNullCloudDateReportParamValue());
            paramValues.Add(DataContractStaticDataClass.GetNullCloudDateTimeReportParamValue());
            paramValues.Add(DataContractStaticDataClass.GetNullCloudDecimalReportParamValue());
            paramValues.Add(DataContractStaticDataClass.GetNullCloudMultiSelectReportParamValue());
            paramValues.Add(DataContractStaticDataClass.GetNullCloudPercentageParamValue());
            paramValues.Add(DataContractStaticDataClass.GetNullCloudSocialSecurityNumberParamValue());

            cloudList.AddRange(DataContractStaticDataClass.GetCloudListOfSingleValueSystemFilterParamValue(paramValues));
            initialCount = cloudList.Count;

            List<PC.SystemFilterParamValue> premiseList =
                new List<PC.SystemFilterParamValue>(ConnectorServiceCommon.Convert.ToPremiseSystemFilterValues(cloudList.ToArray()));
            Assert.AreEqual(initialCount, premiseList.Count, "Conversion dropped some of the parameter values");

            PropertyComparisonResults results = new PropertyComparisonResults();
            bool test = DataObjectComparisonUtil.AreObjectsEqual(premiseList, cloudList, ref results, true, DataContractStaticDataClass.PropertyIgnoreList());
            ValidateComparisons(results);
        }

        [TestMethod]
        public void TestMapper_PremiseSystemFilterValues()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.None);

            List<CC.SystemFilterParamValue> cloudList = new List<CC.SystemFilterParamValue>();
            CC.DecimalReportParamValue paramValue = DataContractStaticDataClass.GetCloudDecimalReportParamValue();
            cloudList.AddRange(DataContractStaticDataClass.GetCloudSingleValueSystemFilterParamValue(paramValue));

            List<PC.SystemFilterParamValue> premiseList =
                new List<PC.SystemFilterParamValue>(ConnectorServiceCommon.Convert.ToPremiseSystemFilterValues(cloudList.ToArray()));

            PropertyComparisonResults results = new PropertyComparisonResults();
            bool test = DataObjectComparisonUtil.AreObjectsEqual(premiseList, cloudList, ref results, true, DataContractStaticDataClass.PropertyIgnoreList());
            ValidateComparisons(results);

            //invalidate the premise guy and reconvert
            Sage.Connector.Cloud.Integration.Interfaces.Utils.PropertyTuple property =
                new Sage.Connector.Cloud.Integration.Interfaces.Utils.PropertyTuple(paramValue.PropertyInfo(x => x.Value), -1.3m);
            paramValue = new CC.DecimalReportParamValue(paramValue, new List<Sage.Connector.Cloud.Integration.Interfaces.Utils.PropertyTuple> { property });
            cloudList = new List<CC.SystemFilterParamValue>(DataContractStaticDataClass.GetCloudSingleValueSystemFilterParamValue(paramValue));

            test = DataObjectComparisonUtil.AreObjectsEqual(premiseList, cloudList, ref results, true, DataContractStaticDataClass.PropertyIgnoreList());
            Assert.IsFalse(test, "Should have been invalidated with this issue.");

            //re convert and retest
            premiseList = new List<PC.SystemFilterParamValue>(ConnectorServiceCommon.Convert.ToPremiseSystemFilterValues(cloudList.ToArray()));

            results.Clear();
            test = DataObjectComparisonUtil.AreObjectsEqual(premiseList, cloudList, ref results, true, DataContractStaticDataClass.PropertyIgnoreList());
            ValidateComparisons(results);
        }
        

        [TestMethod]
        public void TestMapper_PremiseParamValues()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.None);

            List<CC.ReportParamValue> cloudList = new List<CC.ReportParamValue>{
            DataContractStaticDataClass.GetCloudBooleanReportParamValue(),
            DataContractStaticDataClass.GetCloudCurrencyReportParamValue(),
            DataContractStaticDataClass.GetCloudDateMonthDayReportParamValue(), 
            DataContractStaticDataClass.GetCloudDateMonthYearReportParamValue(),
            DataContractStaticDataClass.GetCloudDateReportParamValue(),
            DataContractStaticDataClass.GetCloudDateTimeReportParamValue(),
            DataContractStaticDataClass.GetCloudDecimalReportParamValue(),
            DataContractStaticDataClass.GetCloudMultiSelectReportParamValue(),
            };

            List<PC.ReportParamValue> premiseList = 
                new List<PC.ReportParamValue>(ConnectorServiceCommon.Convert.ToPremiseParameterValues(cloudList.ToArray()));

            PropertyComparisonResults results = new PropertyComparisonResults();
            bool test = DataObjectComparisonUtil.AreObjectsEqual(cloudList, premiseList, ref results, true, DataContractStaticDataClass.PropertyIgnoreList());
            ValidateComparisons(results);

            //Negative test
            results.Clear();

            //change the 8th item in premise list
            CC.MultiSelectReportParamValue tempCopy = cloudList[7] as CC.MultiSelectReportParamValue;
            Sage.Connector.Cloud.Integration.Interfaces.Utils.PropertyTuple property = 
                new Sage.Connector.Cloud.Integration.Interfaces.Utils.PropertyTuple(tempCopy.PropertyInfo(x => x.Values), 
                    new CC.StringList(DataContractStaticDataClass.GetListOfStrings(9, "blah").ToArray()));

            cloudList[7] = new CC.MultiSelectReportParamValue(tempCopy, new Sage.Connector.Cloud.Integration.Interfaces.Utils.PropertyTuple[] { property });
            //premiseList = new List<PC.ReportParam>(premiseList);

            //Should now fail comparison
            test = DataObjectComparisonUtil.AreObjectsEqual(cloudList, premiseList, ref results, true, DataContractStaticDataClass.PropertyIgnoreList());
            Assert.IsFalse(test,
                "Failed to detect a mismatch in the eighth item of the collection");

            //Rerun the conversion and compare results again.
            results.Clear();
            premiseList = new List<PC.ReportParamValue>(ConnectorServiceCommon.Convert.ToPremiseParameterValues(cloudList.ToArray()));
            test = DataObjectComparisonUtil.AreObjectsEqual(cloudList, premiseList, ref results, true, DataContractStaticDataClass.PropertyIgnoreList());
            ValidateComparisons(results);
        }

        [TestMethod]
        public void TestMapper_ReportDescriptors()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.None);

            List<PC.ReportDescriptor> premiseList =
                new List<PC.ReportDescriptor>(DataContractStaticDataClass.GetReportDescriptorList(5));
            CC.ReportDescriptor[] cloudArray;
            cloudArray = ConnectorServiceCommon.Convert.ToCloudReportDescriptors(premiseList.ToArray());
            List<CC.ReportDescriptor> cloudList = new List<CC.ReportDescriptor>(cloudArray);

            PropertyComparisonResults results = new PropertyComparisonResults();
            bool test = DataObjectComparisonUtil.AreObjectsEqual(premiseList, cloudList, ref results, true, DataContractStaticDataClass.PropertyIgnoreList());
            ValidateComparisons(results);

            //Negative test
            results.Clear();

            //change the 1st descriptor to have a different value...the compare to original cloud collection
            PropertyTuple property = new PropertyTuple(premiseList[4].PropertyInfo(x => x.UniqueIdentifier), "5555555");
            premiseList[4] = new PC.ReportDescriptor(premiseList[4], new PropertyTuple[] { property });

            //Should now fail comparison
            test = DataObjectComparisonUtil.AreObjectsEqual(premiseList, cloudList, ref results, true, DataContractStaticDataClass.PropertyIgnoreList());
            Assert.IsFalse(test,
                "Failed to detect a mismatch UniqueID in the last Report descriptor in the collection");

            //Rerun the conversion and compare results again.
            results.Clear();
            cloudArray = ConnectorServiceCommon.Convert.ToCloudReportDescriptors(premiseList.ToArray());
            Assert.IsTrue(DataObjectComparisonUtil.AreObjectsEqual(premiseList, cloudArray, ref results, true, DataContractStaticDataClass.PropertyIgnoreList()),
                "Re-conversion didn't correct the mismatch");
        }

        [TestMethod]
        public void TestMapper_TestReportDescriptor()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.None);

            //Test without Entity Type tags on report parameters
            List<PC.ReportDescriptor> premiseList =
                new List<PC.ReportDescriptor>(DataContractStaticDataClass.GetReportDescriptorList(1, false));
            CC.ReportDescriptor[] cloudArray;
            cloudArray = new CC.ReportDescriptor[]{ConnectorServiceCommon.Convert.ToCloudReportDescriptor(premiseList[0])};
           
            List<CC.ReportDescriptor> cloudList = new List<CC.ReportDescriptor>(cloudArray);

            PropertyComparisonResults results = new PropertyComparisonResults();
            bool test = DataObjectComparisonUtil.AreObjectsEqual(premiseList, cloudList, ref results, true, DataContractStaticDataClass.PropertyIgnoreList());
            ValidateComparisons(results);

            //Test with Entity Type tags on report parameters
            premiseList = new List<PC.ReportDescriptor>(DataContractStaticDataClass.GetReportDescriptorList(1, true));
            cloudArray = new CC.ReportDescriptor[] { ConnectorServiceCommon.Convert.ToCloudReportDescriptor(premiseList[0]) };

            cloudList = new List<CC.ReportDescriptor>(cloudArray);

            results.Clear();
            test = DataObjectComparisonUtil.AreObjectsEqual(premiseList, cloudList, ref results, true, DataContractStaticDataClass.PropertyIgnoreList());
            ValidateComparisons(results);


            //Negative test
            results.Clear();

            //change the 1st descriptor to have a different value...the compare to original cloud collection
            PropertyTuple property = new PropertyTuple(premiseList[0].PropertyInfo(x => x.MenuPath), "Here or there");
            premiseList[0] = new PC.ReportDescriptor(premiseList[0], new PropertyTuple[] { property });

            //Should now fail comparison
            test = DataObjectComparisonUtil.AreObjectsEqual(premiseList, cloudList, ref results, true, DataContractStaticDataClass.PropertyIgnoreList());
            Assert.IsFalse(test,
                "Failed to detect a mismatch menu path in the Report descriptor in the collection");

            //Rerun the conversion and compare results again.
            results.Clear();
            cloudArray = new CC.ReportDescriptor[1];
            cloudArray[0] = ConnectorServiceCommon.Convert.ToCloudReportDescriptor(premiseList[0]);
            Assert.IsTrue(DataObjectComparisonUtil.AreObjectsEqual(premiseList, cloudArray, ref results, true, DataContractStaticDataClass.PropertyIgnoreList()),
                "Re-conversion didn't correct the mismatch");
        }



        [TestMethod]
        public void TestMapper_ErrorInformation()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.None);

            List<PC.ErrorInformation> premiseList = 
                new List<PC.ErrorInformation>(DataContractStaticDataClass.GetErrorInformationList(5));
            CC.ErrorInformation[] cloudArray = ConnectorServiceCommon.Convert.ToCloudErrorInformationList(premiseList.ToArray());
            List<CC.ErrorInformation> cloudList = new List<CC.ErrorInformation>(cloudArray);

            PropertyComparisonResults results = new PropertyComparisonResults();
            bool test = DataObjectComparisonUtil.AreObjectsEqual(premiseList, cloudList, ref results, true, DataContractStaticDataClass.PropertyIgnoreList());
            ValidateComparisons(results);
            
            //Now with User facing description
            premiseList = new List<PC.ErrorInformation>(DataContractStaticDataClass.GetErrorInformationListWithCustomerFacing(5));
            cloudArray = ConnectorServiceCommon.Convert.ToCloudErrorInformationList(premiseList.ToArray());
            cloudList = new List<CC.ErrorInformation>(cloudArray);

            results = new PropertyComparisonResults();
            test = DataObjectComparisonUtil.AreObjectsEqual(premiseList, cloudList, ref results, true, DataContractStaticDataClass.PropertyIgnoreList());
            ValidateComparisons(results);
        }

        [TestMethod]
        public void TestMapper_ReportParam()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.None);

            List<PC.ReportParam> premiseList =
                new List<PC.ReportParam>{
                    DataContractStaticDataClass.GetDateMonthYearReportParameter(),
                    DataContractStaticDataClass.GetCurrencyReportParameter(false),
                    DataContractStaticDataClass.GetBooleanReportParameter()
                };

            CC.ReportParam[] cloudArray;
            cloudArray = ConnectorServiceCommon.Convert.ToCloudReportParams(premiseList.ToArray());
            List<CC.ReportParam> cloudList = new List<CC.ReportParam>(cloudArray);

            PropertyComparisonResults results = new PropertyComparisonResults();
            bool test = DataObjectComparisonUtil.AreObjectsEqual(premiseList, cloudList, ref results, true, DataContractStaticDataClass.PropertyIgnoreList());
            ValidateComparisons(results);

            //Negative test
            results.Clear();

            //change the 2nd item in premise list
            PC.CurrencyReportParam tempCopy = premiseList[1] as PC.CurrencyReportParam;
            PropertyTuple property = new PropertyTuple(tempCopy.PropertyInfo(x => x.DisallowZero), false);

            premiseList[1] = new PC.CurrencyReportParam(tempCopy, new PropertyTuple[] { property });

            //Should now fail comparison
            test = DataObjectComparisonUtil.AreObjectsEqual(premiseList, cloudList, ref results, true, DataContractStaticDataClass.PropertyIgnoreList());
            Assert.IsFalse(test,
                "Failed to detect a mismatch in the second item of the collection");

            //Rerun the conversion and compare results again.
            results.Clear();
            cloudArray = ConnectorServiceCommon.Convert.ToCloudReportParams(premiseList.ToArray());
            Assert.IsTrue(DataObjectComparisonUtil.AreObjectsEqual(premiseList, cloudArray, ref results, true, DataContractStaticDataClass.PropertyIgnoreList()),
                "Re-conversion didn't correct the mismatch");


        }


        //Validates the end result 
        public void ValidateComparisons(PropertyComparisonResults results)
        {
            foreach (PropertyComparisonResult result in results.ResultList)
            {
                if (result.ExceptionOccured)
                {
                    Assert.Inconclusive(result.Message);
                }
                else
                {
                    Assert.IsTrue(result.AreEqual, string.Format("Property: {0} does not match for contract. Message: {1}",
                        result.PropertyName, result.Message));
                }
            }
        }
    }
}
