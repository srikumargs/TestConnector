using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using Sage.Connector.Common;
using Sage.Connector.Management;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.Connector.StateService.Proxy;

namespace SageConnector.ViewModel
{
    /// <summary>
    /// Common functions in support of the application. Only that which is application logic.
    /// </summary>
    static public class ConnectorViewModel
    {
        /// <summary>
        /// Uri to show help for main form
        /// </summary>
        static public Uri MainFormHelpUri
        { 
            get 
            {
                Uri help = CreateHelpUriFromFragment(ResourcesViewModel.MainFormHelpPathFragment);
                return help; 
            } 
        }

        /// <summary>
        /// Uri to show help for detail form
        /// </summary>
        static public Uri DetailFormHelpUri
        { 
            get 
            {
                Uri help = CreateHelpUriFromFragment(ResourcesViewModel.DetailFormHelpPathFragment);
                return help; 
            } 
        }

        /// <summary>
        /// Uri to show help for connector requests form
        /// </summary>
        static public Uri RequestFormHelpUri
        {
            get
            {
                Uri help = CreateHelpUriFromFragment(ResourcesViewModel.RequestsFormHelpPathFragment);
                return help;
            }
        }

        /// <summary>
        /// Uri to show help for account selection form
        /// </summary>
        public static Uri AccountSelectionFormHelpUri
        {
            get
            {
                Uri help = CreateHelpUriFromFragment((ResourcesViewModel.AccountSelectionFormHelpPathFragment));
                return help;
            }
        }

        /// <summary>
        /// Uri to show the System Requirements help topic
        /// </summary>
        public static Uri SystemRequiresmentsHelpUri
        {
            get 
            {
                Uri help = CreateHelpUriFromFragment(ResourcesViewModel.SystemRequirementsHelpPathFragment);
                return help;
            }
        }

        /// <summary>
        /// Create the full UI for a help topic from a help path fragment
        /// </summary>
        /// <param name="helpPathFragment"></param>
        /// <returns></returns>
        static private Uri CreateHelpUriFromFragment(string helpPathFragment)
        {
            string baseHelpUrl = ConnectorRegistryUtils.ProductHelpBaseUrl;
            string fullHelpUrl = baseHelpUrl + helpPathFragment;
            Uri help = new Uri(fullHelpUrl);
            return help;
        }
        
        /// <summary>
        /// Title for the main form of the application
        /// </summary>
        static public String MainFormTitle
        {
            get { return ConnectorRegistryUtils.BriefProductName; }
        }

        /// <summary>
        /// Name of the product that should be shown to user.
        /// </summary>
        static public string ProductName
        {
            get { return ConnectorRegistryUtils.BriefProductName; }
        }

        /// <summary>
        /// BackOfficeProductTerm
        /// </summary>
        static public string BackOfficeProductTermSentenceCaps
        { get { return "Back office"; } }

        /// <summary>
        /// 
        /// </summary>
        static public string BackOfficeProductTermLowercase
        { get { return "back office"; } }


        /// <summary>
        /// 
        /// </summary>
        static public string ConnectorServiceNotEnabledMessage
        { get { return String.Format(ResourcesViewModel.ConnectorServiceNotEnabledMessageFormat, ConnectorServiceUtils.ServiceDisplayName, ConnectorRegistryUtils.BriefProductName).Replace("\\n", "\r"); } }

        /// <summary>
        /// PluggedInProductName
        /// </summary>
        static public string PluggedInProductNameConnectionDetails
        {
            get
            {
                return ConnectorViewModel.BackOfficeProductTermSentenceCaps + " connection details";
            }
        }

        /// <summary>
        /// Name of the service host as shown to the user.
        /// </summary>
        static public String ServiceDisplayName
        {
            get { return ConnectorServiceUtils.ServiceDisplayName; }
        }

        /// <summary>
        /// Is the hosting framework service ready.
        /// </summary>
        /// <returns></returns>
        static public bool IsHostingFrameworkServiceReady(ILogging logging)
        {
            bool retval = ConnectorServiceUtils.IsServiceReady() && ConnectorMonitorServiceUtils.IsServiceRegistered(logging);
            return retval;
        }

        /// <summary>
        /// Gets the Current version of the application
        /// </summary>
        /// <returns></returns>
        static public string ApplicationVersion()
        {
            //string retval = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            string retVal = string.Empty;

            Assembly assembly = Assembly.GetExecutingAssembly();
            if ((null != assembly) && (System.IO.File.Exists(assembly.Location)))
            {
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                retVal = fvi.FileVersion;
            }
            return retVal;
        }

        /// <summary>
        /// Gets the AssemblyConfiguration attribute of the application
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2135:SecurityRuleSetLevel2MethodsShouldNotBeProtectedWithLinkDemandsFxCopRule"), SuppressMessage("Microsoft.Design", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "The call restricts the action returning the file version.")]
        [SecurityPermission(SecurityAction.LinkDemand)]
        public static string GetBuildSourceInformation()
        {
            if (_buildSourceInformation == null)
            {
                if (DeveloperFlags.ShowEndPointAddress())
                {
                    Assembly assembly = Assembly.GetExecutingAssembly();
                    Object[] customAttributes = assembly.GetCustomAttributes(typeof(AssemblyConfigurationAttribute), false);
                    if (customAttributes.Length > 0)
                    {
                        AssemblyConfigurationAttribute assemblyConfigurationAttribute = (AssemblyConfigurationAttribute)customAttributes[0];
                        var splitAssemblyConfiguration = assemblyConfigurationAttribute.Configuration.Split(';');
                        var assemblyConfigurationDictionary = splitAssemblyConfiguration.ToDictionary(x => x.Substring(0, x.IndexOf('=')));
                        var sageBuildType = assemblyConfigurationDictionary["SageBuildType"];
                        sageBuildType = sageBuildType.Substring(sageBuildType.IndexOf('=') + 1);
                        var buildDefinitionName = assemblyConfigurationDictionary["BuildDefinitionName"];
                        buildDefinitionName = buildDefinitionName.Substring(buildDefinitionName.IndexOf('=') + 1);
                        var sageServerPath = assemblyConfigurationDictionary["SageServerPath"];
                        sageServerPath = sageServerPath.Substring(sageServerPath.IndexOf('=') + 1);
                        _buildSourceInformation = String.Format(CultureInfo.InvariantCulture, " ({0} - {1})", !String.IsNullOrEmpty(buildDefinitionName) ? buildDefinitionName : sageBuildType, sageServerPath);
                    }
                }
                else
                {
                    _buildSourceInformation = String.Empty;
                }
            }

            return _buildSourceInformation;
        }

        private static String _buildSourceInformation = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        static public BackOfficePlugin[] ProcessBackOfficePluginsResponse(BackOfficePluginsResponse response)
        {
            List<String> errorList = new List<String>();
            BackOfficePlugin[] result = { };
            using (var logger = new SimpleTraceLogger())
            {
                if (null != response)
                {
                    result = response.BackOfficePlugins;

                    if ((null != response.UserFacingMessages) &&
                        (response.UserFacingMessages.Any()))
                    {
                        errorList.AddRange(response.UserFacingMessages);
                    }


                    if (null != response.RawErrorMessage &&
                        (response.RawErrorMessage.Any()))
                    {
                        foreach (var error in response.RawErrorMessage)
                        {
                            if (!String.IsNullOrEmpty(error))
                            {
                                logger.WriteCriticalWithEventLogging(null, "Back Office Plugins", error);
                            }
                        }
                    }
                }
            }

            return result;
        }
        /// <summary>
        /// Logs any errors
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        static public BackOfficeConnection[] ProcessBackOfficeConnectionsForCredentialsResponse(BackOfficeConnectionsForCredentialsResponse response)
        {
            List<String> errorList = new List<String>();
            BackOfficeConnection[] result = { };
            using (var logger = new SimpleTraceLogger())
            {
                if (null != response)
                {
                    result = response.BackOfficeConnections;

                    //JSB - This needs review. We Add user facing errors to a an error list then ignore the messages and the list.
                    //We then log any raw error messages. This seems a bit off somehow...

                    if ((null != response.UserFacingMessages) &&
                        (response.UserFacingMessages.Any()))
                    {
                        errorList.AddRange(response.UserFacingMessages);
                    }


                    if (null != response.RawErrorMessage &&
                        (response.RawErrorMessage.Any()))
                    {
                        foreach (var error in response.RawErrorMessage)
                        {
                            if (!String.IsNullOrEmpty(error))
                            {
                                logger.WriteCriticalWithEventLogging(null, "Back Office Data Connections", error);
                            }
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Boolean IsAnyConnectableBackOfficeAvailable()
        {
            Boolean result = false;

            result = ConnectorPluginsViewModel.GetConnectorPlugins().Any();

            return result;
        }

        /// <summary>
        /// TODO KMS: Remove.  This shouold not be necessary
        /// </summary>
        /// <returns></returns>
        public static Boolean IsConnectorPluginAvailable(ConnectorPlugin connectorPlugin)
        { return true; }

        /// <summary>
        /// TODO KMS: Remove.  This shouold not be necessary
        /// </summary>
        /// <param name="productPluginPath"></param>
        /// <returns></returns>
        public static Boolean IsValidConnectorPath(string productPluginPath)
        { return true; }

        /// <summary>
        /// Check if the provided user name and password belongs to a back office administrator.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="errors"></param>
        /// <returns>true if provided info matches a back office admin</returns>
        static public Boolean ProcessValidateBackOfficeAdminCredentialsResponse(ValidateBackOfficeAdminCredentialsResponse response, out string[] errors)
        {
            List<string> errorList = new List<string>();
            Boolean returnValue = false;

            using (var logger = new SimpleTraceLogger())
            {
                try
                {
                    if (null != response)
                    {
                        returnValue = response.IsValid;

                        if ((null != response.UserFacingMessages) &&
                            (response.UserFacingMessages.Any()))
                        {
                            errorList.AddRange(response.UserFacingMessages);
                        }


                        if (null != response.RawErrorMessage &&
                            (response.RawErrorMessage.Any()))
                        {
                            foreach (var error in response.RawErrorMessage)
                            {
                                if (!string.IsNullOrEmpty(error))
                                {
                                    logger.WriteCriticalWithEventLogging(null, "Back Office Administrator Validation", error);
                                }
                            }
                        }


                        if (!returnValue && errorList.Count == 0)
                        {
                            // If no user-facing error was returned, add a default failure message
                            errorList.Add(ResourcesViewModel.ConnectorLogin_LoginError_BackofficeValidationDidNotSucceed);
                        }
                    }
                    else
                    {
                        errorList.Add(ResourcesViewModel.ConnectorLogin_NoResponseFromBackoffice);
                    }
                }
                catch (Exception ex)
                {
                    logger.WriteCriticalWithEventLogging(null, "Back Office Administrator Validation", "Error validating admin credentials: " + ex.ExceptionAsString());
                }

                errors = errorList.ToArray();
                return returnValue;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        static public ConnectorState GetConnectorState()
        {
            //TODO: do we need a pop up? if the state service is dead where sort of hosed...

            ConnectorState connectorState = null;
            try
            { 
                using (var proxy = StateServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                {
                    connectorState = proxy.GetConnectorState();
                }
            }
            catch (Exception ex)
            {
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteCriticalWithEventLogging(null, "Get Connector State", "Error getting connector state from service: " + ex.ExceptionAsString());
                }
            }
            return connectorState;
        }
    }
}
