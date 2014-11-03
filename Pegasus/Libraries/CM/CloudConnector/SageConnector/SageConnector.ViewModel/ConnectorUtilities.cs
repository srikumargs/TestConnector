using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Sage.Connector.Common;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.Connector.StateService.Proxy;

namespace SageConnector.ViewModel
{
    /// <summary>
    /// Common functions in support of the application. Only that which is UI or otherwise should
    /// not be exposed to unit tests.
    /// </summary>
    static public class ConnectorUtilities
    {
        /// <summary>
        /// Show any URI in the default browser
        /// </summary>
        /// <param name="uri"></param>
        static public void ShowTenantSite(Uri uri)
        {
            ShowUri(uri);
        }

        /// <summary>
        /// Show the help for main form
        /// </summary>
        static public void ShowMainFormHelp()
        {
            Uri helpLink = ConnectorViewModel.MainFormHelpUri;
            ShowUri(helpLink);
        }

        /// <summary>
        /// Show the help for the detail form
        /// </summary>
        static public void ShowDetailFormHelp()
        {
            Uri helpLink = ConnectorViewModel.DetailFormHelpUri;
            ShowUri(helpLink);
        }

        /// <summary>
        /// Show the help for the connector requests form
        /// </summary>
        static public void ShowRequestFormHelp()
        {
            Uri helpLink = ConnectorViewModel.RequestFormHelpUri;
            ShowUri(helpLink);
        }

        /// <summary>
        /// Show the help for the account selection form
        /// </summary>
        public static void ShowAccountSelectionFormHelp()
        {
            Uri helpLink = ConnectorViewModel.AccountSelectionFormHelpUri;
            ShowUri(helpLink);
        }

        static private void ShowUri(Uri uri)
        {
            ProcessStartInfo sInfo = new ProcessStartInfo(uri.AbsoluteUri);
            Process.Start(sInfo);
        }
        
        #region Connectivity Helpers

        /// <summary>
        /// Verify the connetion with the cloud
        /// </summary>
        /// <param name="response"></param>
        /// <param name="cloudCompanyName"></param>
        /// <param name="cloudCompanyUrl"></param>
        /// <param name="errors"></param>
        /// <returns></returns>              
        public static TenantConnectivityStatus ProcessValidateTenantConnectionResponse(
            ValidateTenantConnectionResponse response, 
            out string cloudCompanyName,
            out string cloudCompanyUrl,
            out string[] errors)
        {
            // Init
            List<string> errorList = new List<string>();

            // Init out params
            cloudCompanyName = string.Empty;
            cloudCompanyUrl = string.Empty;
            errors = new string[] { };

            try
            {
                // Build the error messages and validity flag
                if (response != null)
                {
                    if (response.TenantConnectivityStatus == TenantConnectivityStatus.Normal)
                    {
                        // Set valid company name and url
                        cloudCompanyName = response.Name;
                        cloudCompanyUrl = response.SiteAddress.ToString();
                    }

                    errorList.Add(TranslateCloudConnectivityStatus(response.TenantConnectivityStatus));
                }
                else
                {
                    errorList.Add(TranslateCloudConnectivityStatus(TenantConnectivityStatus.None));
                }
            }
            catch (Exception ex)
            {
                // Write any exceptions out to the event log
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteCriticalWithEventLogging(
                        null, 
                        "Validate Cloud Configuration", 
                        "Error validating tenant connection: " + ex.ExceptionAsString());
                }

                // Add to the error list
                errorList.Add(ex.Message);
            }

            // Set the out error array
            errors = errorList.ToArray();

            // Get the status from the full response
            // Get the cloud status from the verify response
            TenantConnectivityStatus cloudStatus =
                (response == null)
                ? TenantConnectivityStatus.None
                : response.TenantConnectivityStatus;

            return cloudStatus;
        }


        /// <summary>
        /// Validate the current settings connecting to the back office
        /// Assemble an array of error strings from the back office describing potential issues
        /// </summary>
        /// <param name="response"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public static BackOfficeConnectivityStatus ProcessValidateBackOfficeConnectionResponse(
            ValidateBackOfficeConnectionResponse response,
            out string[] errors)
        {
            // Init
            BackOfficeConnectivityStatus backOfficeStatus = BackOfficeConnectivityStatus.None;
            errors = new string[] { };
            List<string> errorList = new List<string>();

            try
            {
                // Build the error messages and validity flag
                if (response != null)
                {
                    // Set the back office connectivity status
                    backOfficeStatus = response.BackOfficeConnectivityStatus;
                    

                    // Add any user facing messages to the error list
                    if (response.UserFacingMessages != null &&
                        response.UserFacingMessages.Count() > 0)
                    {
                        errorList.AddRange(response.UserFacingMessages);
                    }

                    // Write the raw error messages to the event log
                    if (response.RawErrorMessage != null &&
                        response.RawErrorMessage.Count() > 0)
                    {
                        using (var logger = new SimpleTraceLogger())
                        {
                            foreach (var error in response.RawErrorMessage)
                            {
                                if (!string.IsNullOrEmpty(error))
                                {
                                    logger.WriteCriticalWithEventLogging(null, "Back Office Connectivity", error);
                                }
                            }
                        }
                    }

                    if (backOfficeStatus != BackOfficeConnectivityStatus.Normal && errorList.Count == 0)
                    {
                        // If no user-facing error was generated, return our default error
                        errorList.Add(ResourcesViewModel.ConnectorLogin_DefaultErrorResponseFromBackoffice);
                    }
                }
                else
                {
                    // Null response error
                    errorList.Add(ResourcesViewModel.ConnectorLogin_NoResponseFromBackoffice);
                }
            }
            catch (Exception ex)
            {
                // Write any exceptions out to the event log
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteCriticalWithEventLogging(null,
                        "Validate Cloud Configuration",
                        "Error validating tenant connection: " + ex.ExceptionAsString());
                }

                // Add to the error list
                errorList.Add(ex.Message);
            }

            // Set the out error array
            errors = errorList.ToArray();

            // Return the status
            return backOfficeStatus;
        }

        /// <summary>
        /// Hanlde updating the state service given the results of our verify calls
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="backOfficeConnectivityStatus"></param>
        /// <param name="tenantConnectivityStatus"></param>
        public static void UpdateConnectionStatusesInStateService(
            string tenantId,
            BackOfficeConnectivityStatus backOfficeConnectivityStatus,
            TenantConnectivityStatus tenantConnectivityStatus)
        {
            try
            {
                using (var proxy = StateServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                {
                    proxy.UpdateBackOfficeConnectivityStatus(tenantId, backOfficeConnectivityStatus);
                    proxy.UpdateTenantConnectivityStatus(tenantId, tenantConnectivityStatus);
                }
            }
            catch (Exception ex)
            {
                // Write any exceptions out to the event log
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteCriticalWithEventLogging(null,
                        "Save Cloud Configuration",
                        "Error updating state service: " + ex.ExceptionAsString());
                }
            }
        }

        /// <summary>
        /// Translate cloud connectivity status to a user facing response
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public static string TranslateCloudConnectivityStatus(TenantConnectivityStatus status)
        {
            switch (status)
            {
                case TenantConnectivityStatus.CloudUnavailable:
                    return ResourcesViewModel.ConnectionDetail_CloudUnavailable;
                case TenantConnectivityStatus.CommunicationFailure:
                    return ResourcesViewModel.ConnectionDetail_CommunicationFailure;
                case TenantConnectivityStatus.GatewayServiceUnavailable:
                    return ResourcesViewModel.ConnectionDetail_GatewayServiceUnavailable;
                case TenantConnectivityStatus.IncompatibleClient:
                    return ResourcesViewModel.ConnectionDetail_IncompatibleClient;
                case TenantConnectivityStatus.InvalidConnectionInformation:
                    return ResourcesViewModel.ConnectionDetail_InvalidConnectionInformation;
                case TenantConnectivityStatus.Reconfigure:
                    return ResourcesViewModel.ConnectionDetail_Reconfigure;
                case TenantConnectivityStatus.TenantDisabled:
                    return ResourcesViewModel.ConnectionDetail_TenantDisabled;
                case TenantConnectivityStatus.InternetConnectionUnavailable:
                    return ResourcesViewModel.ConnectionDetail_InternetConnectionUnavailable;
                case TenantConnectivityStatus.LocalNetworkUnavailable:
                    return ResourcesViewModel.ConnectionDetail_LocalNetworkUnavailable;
                case TenantConnectivityStatus.None:
                    return ResourcesViewModel.ConnectionDetail_None;
                case TenantConnectivityStatus.Normal:
                    return ResourcesViewModel.ConnectionDetail_Normal;
            }

            return ResourcesViewModel.ConnectionDetail_None;
        }

        #endregion
    }
}
