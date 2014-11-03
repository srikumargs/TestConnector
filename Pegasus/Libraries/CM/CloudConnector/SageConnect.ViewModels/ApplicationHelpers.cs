using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using Sage.Connector.Common;
using Sage.Connector.Data;
using Sage.Connector.Management;

namespace SageConnect.ViewModels
{
    /// <summary>
    /// Static class to store required static data
    /// </summary>
    public static class ApplicationHelpers
    {
        /// <summary>
        /// SageId Token
        /// </summary>
        public static string SageIdToken { get; set; }

        /// <summary>
        /// SageId login status
        /// </summary>
        public static bool SageloginSucess { get; set; }

        /// <summary>
        /// Site address used to connect to the cloud
        /// </summary>
        public static Uri SiteAddressUri { get; set; }

        /// <summary>
        /// AuthenticationUri address used to connect to the cloud [DEPRECATED]
        /// </summary>
        public static Uri AuthenticationUri { get; set; }

        /// <summary>
        /// Site address used to connect to the cloud for Registration
        /// </summary>
        public static Uri ConnectorServiceUri { get; set; }

        /// <summary>
        /// The sage id client id for the selected deployment
        /// </summary>
        public static String ClientId { get; set; }

        /// <summary>
        /// The sage id scope for the selected deployment
        /// </summary>
        public static String Scope { get; set; }

        private static SiteGroup _selectedSiteGroup;

        /// <summary>
        /// Site address used to connect to the cloud for registration
        /// </summary>
        public static SiteGroup SelectedSiteGroup
        {
            get { return _selectedSiteGroup; }
            set
            {
                if (value != null)
                {
                    _selectedSiteGroup = value;
                    SiteAddressUri = value.ConnectorServiceUri;
                    ConnectorServiceUri = value.ConnectorServiceUri;
                    ClientId = value.ClientId;
                    Scope = value.Scope;
                }
            }
        }

        /// <summary>
        /// To check the Tenant available in existing connection list
        /// </summary>
        /// <param name="tenantGuid"></param>
        /// <param name="registeredconnecterId"></param>
        /// <returns></returns>
        public static ConnectionState GetTenantStatus(Guid tenantGuid, string registeredconnecterId)
        {
          

            try
            {


                foreach (
                    PremiseConfigurationRecord premiseConfigurationRecord in
                        ConfigurationHelpers.GetAllTenantConfigurations())
                {

                    if (premiseConfigurationRecord.CloudTenantId == tenantGuid.ToString())
                        return ConnectionState.OnLine;

                }

                if (registeredconnecterId != string.Empty &&
                    registeredconnecterId != ConfigurationHelpers.GetConnectorId().ToString())
                    return ConnectionState.Configured;

               
            }
            catch (Exception ex)
            {
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(null, ex.ExceptionAsString());
                }
                
            }
            return ConnectionState.OffLine;
        }

        /// <summary>
        /// To validate for the cloud connection status for existing connection
        /// </summary>
        /// <param name="tenantGuid"></param>
        /// <param name="registeredconnecterId"></param>
        /// <returns></returns>
        public static ConnectionState CloudConnectionStatus(Guid tenantGuid, string registeredconnecterId)
        {
           
            foreach (
                PremiseConfigurationRecord premiseConfigurationRecord in
                    ConfigurationHelpers.GetAllTenantConfigurations())
            {
                if (premiseConfigurationRecord.CloudTenantId == tenantGuid.ToString())
                {
                    if (
                        ConfigurationHelpers.ValidateTenantConnection(ConnectorServiceUri.ToString(),
                            tenantGuid.ToString(),
                            premiseConfigurationRecord.CloudPremiseKey, premiseConfigurationRecord.CloudTenantClaim)
                            .Success)
                        return ConnectionState.OnLine;

                }

            }
            if (registeredconnecterId != string.Empty && registeredconnecterId != ConfigurationHelpers.GetConnectorId().ToString())
                return ConnectionState.Configured;
            return ConnectionState.OffLine;
        }

        /// <summary>
        /// To validate for the back office connection status for existing connection
        /// </summary>
        /// <param name="tenantGuid"></param>
        /// <returns></returns>
        public static bool BackofficeConnectionStatus(Guid tenantGuid)
        {
            
            foreach (
                PremiseConfigurationRecord premiseConfigurationRecord in
                    ConfigurationHelpers.GetAllTenantConfigurations())
            {
                if (premiseConfigurationRecord.CloudTenantId == tenantGuid.ToString())
                {

                    return
                        ConfigurationHelpers.ValidateBackOfficeConnection(premiseConfigurationRecord.ConnectorPluginId,
                            JsonConvert.DeserializeObject<Dictionary<string, string>>(
                                premiseConfigurationRecord.BackOfficeConnectionCredentials)).Success;

                }

            }
            return false;

        }

       
        /// -------------------------------------------------------------------------------------------------
        /// <summary> check if current process already running. if running, set focus to existing process and 
        ///           returns <see langword="true"/> otherwise returns <see langword="false"/>. </summary>
        /// <returns> <see langword="true"/> if it succeeds, <see langword="false"/> if it fails. </returns>
        /// -------------------------------------------------------------------------------------------------
        public static bool OpenMonitorRunning(bool openMonitor = true)
        {
            try
            {
                 IntPtr hwnd = IntPtr.Zero;
                if (SingleInstanceApplicationChecker.IsApplicationRunningOnThisMachine("ConnectorServiceMonitor",out hwnd))
                {
                    Process.Start(AppDomain.CurrentDomain.BaseDirectory +
                               "/Monitor/Tray/ConnectorServiceMonitor.exe");
                    SingleInstanceApplicationChecker.IsApplicationRunningOnThisMachine("ConnectorServiceMonitor",
                        out hwnd);
                }
                if (hwnd != IntPtr.Zero && openMonitor)
                {
                    NativeMethods.ShowWindowAsync(hwnd, NativeMethods.SW_SHOWNORMAL);
                    NativeMethods.SetForegroundWindow(hwnd);
                    NativeMethods.PostMessage(hwnd, NativeMethods.WM_CONNECTOR_SHOW_NORMAL, IntPtr.Zero, IntPtr.Zero);
                }
                return true;
            }
            catch (Exception ex)
            {
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(null, ex.ExceptionAsString());
                }
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool IsapplicationAlreadyRunning()
        {
            IntPtr hwnd = IntPtr.Zero;
            if (!SingleInstanceApplicationChecker.IsOnlyProcessOnThisMachine("", out hwnd))
            {
                if (hwnd != IntPtr.Zero)
                {
                    NativeMethods.ShowWindowAsync(hwnd, NativeMethods.SW_SHOWNORMAL);
                    NativeMethods.SetForegroundWindow(hwnd);
                    NativeMethods.PostMessage(hwnd, NativeMethods.WM_CONNECTOR_SHOW_NORMAL, IntPtr.Zero, IntPtr.Zero);
                    return true;
                }

            }
            return false;
        }
    }
}
