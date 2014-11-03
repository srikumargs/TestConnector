using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sage.Connector.TestUtilities;

namespace Sage.Connector.ConnectorServiceCommon.Test
{
    [TestClass]
    public class RetryPolicyTest
    {
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

        /// <summary>
        /// Simple test of retry logic around notification calls
        /// </summary>
        [TestMethod]
        public void TestAllRetryPolicies()
        {
            TestUtils.UnitTestSetup(TestUtils.UnitTestSetupFlags.None);

            int retryCount = 0;
            bool actionSucceded = false;

            // Define the simple action
            Action retryAction = new Action(() =>
            {
                // Throw an exception so that we retry exactly once
                if (retryCount == 0)
                {
                    ++retryCount;
                    throw new Exception("Generic retry exception");
                }

                // Mark as succeeded on first retry
                actionSucceded = true;
            });

            // Test all existing policy types
            foreach (string currentRetryPurposeTypeName in Enum.GetNames(typeof(RetryPurpose)).Where((x) => x != "None"))
            {
                // Reset test variables
                retryCount = 0;
                actionSucceded = false;

                // Get the current retry purpose enum
                RetryPurpose retryPurpose = (RetryPurpose)Enum.Parse(typeof(RetryPurpose), currentRetryPurposeTypeName);

                // Don't test with some retry purposes, e.g. ones that only attempt the action once
                if (retryPurpose != RetryPurpose.QueueStore &&
                    retryPurpose != RetryPurpose.ConfigurationStore &&
                    retryPurpose != RetryPurpose.LogStore &&
                    retryPurpose != RetryPurpose.DatabaseCorruptionRecovery)
                {
                    try
                    {
                        // Run action in retry of this type
                        RetryPolicyManager.ExecuteInRetry(retryPurpose, retryAction, null);
                    }
                    catch (Exception ex)
                    {
                        // Threw a non-transient exception
                        // Make sure this exception is in fact non-transient
                        if (RetryPolicyManager.IsTransientExceptionForRetryPolicy(ex, retryPurpose))
                        {
                            Assert.Fail(String.Format(
                                "Exception '{0}' is transient for retry purpose '{1}', but was not retried!",
                                ex.GetType().Name,
                                Enum.GetName(typeof(RetryPurpose), retryPurpose)));
                        }
                    }

                    // Check results
                    Assert.AreEqual(1, retryCount,
                        String.Format("{0}: Number of retries should be exactly 1", currentRetryPurposeTypeName));
                    Assert.IsTrue(actionSucceded,
                        String.Format("{0}: The test action should have succeeded after the first retry", currentRetryPurposeTypeName));
                }
            }
        }
    }
}
