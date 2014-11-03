using System;
using System.ServiceModel;
using Sage.Connector.Common;
using Sage.Connector.ConnectorServiceCommon;
using Sage.Connector.Data;
using Sage.Connector.Logging;
using Sage.Connector.NotificationService.Proxy;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.Connector.Utilities;
using Sage.Connector.Cloud.Integration.Interfaces.Faults;

namespace Sage.Connector.MessagingService.Internal
{
    /// <summary>
    /// Types of fault exceptions that we handle
    /// Used so that the callers of ProcessFaultException do not have to do 
    /// Any converting of the generic exception themselves to determine which
    /// FaultException it was processed as.
    /// </summary>
    public enum FaultExceptionType
    {
        /// <summary>
        /// Not a fault exception
        /// </summary>
        None = 0,

        /// <summary>
        /// A fault exception, but not one that we understand
        /// </summary>
        GenericFaultException,

        /// <summary>
        /// Error serializing or deserializing across WCF boundary
        /// </summary>
        Serialization,

        /// <summary>
        /// Connectivity fault
        /// </summary>
        Connectivity,

        /// <summary>
        /// IncompatibleClient fault
        /// </summary>
        IncompatibleClient,

        /// <summary>
        /// RetiredEndpoint fault
        /// </summary>
        RetiredEndpoint,

        /// <summary>
        /// TenantConnectionDisabled fault
        /// </summary>
        TenantConnectionDisabled,

        /// <summary>
        /// InvalidResponse fault
        /// </summary>
        InvalidResponse
    }

    /// <summary>
    /// Provide utilities for processing faults received from the cloud
    /// </summary>
    public static class FaultHelper
    {
        /// <summary>
        /// Write fault detail out as a warning
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="fault"></param>
        public static void WriteFaultAsWarning(object caller, BaseDataContractFault fault)
        {
            using (var lm = new LogManager())
            {
                lm.WriteWarning(caller, FormatFaultMessage(fault));
            }
        }

        /// <summary>
        /// Write fault detail out as an error
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="fault"></param>
        public static void WriteFaultAsError(object caller, BaseDataContractFault fault)
        {
            using (var lm = new LogManager())
            {
                lm.WriteError(caller, FormatFaultMessage(fault));
            }
        }

        /// <summary>
        /// Provide common formatting for writing out fault details
        /// </summary>
        /// <param name="fault"></param>
        /// <returns></returns>
        private static string FormatFaultMessage(BaseDataContractFault fault)
        {
            return String.Format(
                "Fault of type '{0}' encountered.  Fault message: '{1}'",
                fault.GetType().Name,
                fault.Message);
        }

        /// <summary>
        /// Sends subscriber of service information update
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="fault"></param>
        public static void NotifyRetiredEndpoint(string tenantId, RetiredEndpointFault fault)
        {
            try
            {
                using (var proxy = NotificationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                {
                    proxy.NotifyServiceInfoUpdated(tenantId, fault.SiteAddressBaseUri);
                }
            }
            catch (Exception ex)
            {
                using (var lm = new LogManager())
                {
                    lm.WriteError(null, ex.ExceptionAsString());
                }
                SubsystemHealthHelper.RaiseSubsystemHealthIssue(Subsystem.NotificationService, ex.ExceptionAsString(), "Failed to notify of retired endpoint: " + ex.Message);
            }
        }   

        /// <summary>
        /// Disable effective cloud request and responses
        /// </summary>
        /// <param name="pcr">Premise configuration record</param>
        private static void DisableTenantCommunication(PremiseConfigurationRecord pcr)
        {
            if (null != pcr)
            {
                //Prevent update configuration from triggering RetrieveRemoteConfig for no reason.
                if (pcr.CloudConnectionEnabledToReceive || pcr.CloudConnectionEnabledToSend)
                {
                    pcr.CloudConnectionEnabledToReceive = false;
                    pcr.CloudConnectionEnabledToSend = false;
                    ConfigurationSettingFactory.UpdateConfiguration(pcr);
                }
            }
        }

        /// <summary>
        /// Handle client incompatibility by disabling effective cloud collaboration
        /// </summary>
        /// <param name="pcr"></param>
        /// <param name="fault"></param>
        public static void IncompatibleDisableCollaboration(PremiseConfigurationRecord pcr, IncompatibleClientFault fault)
        {
            DisableTenantCommunication(pcr);
        }

        /// <summary>
        /// Handle disable request by either disabling or deleting the tenant configuration
        /// </summary>
        /// <param name="pcr"></param>
        /// <param name="action"></param>
        public static void TenantDisableCollaboration(PremiseConfigurationRecord pcr, TenantConnectionDisabledAction action)
        {
            switch (action)
            {
                case TenantConnectionDisabledAction.Delete:
                    ConfigurationSettingFactory.DeleteTenant(pcr.CloudTenantId);
                    break;
                case TenantConnectionDisabledAction.DisableOnly:
                    DisableTenantCommunication(pcr);
                    break;
            }
        }

        private class FaultResult
        {
            public FaultResult()
            {
                FaultType = FaultExceptionType.None;
                ConnectivityStatusForUpdate = TenantConnectivityStatus.None;
            }

            public FaultResult(FaultExceptionType faultType, TenantConnectivityStatus connectivityStatusForUpdate)
            {
                FaultType = faultType;
                ConnectivityStatusForUpdate = connectivityStatusForUpdate;
            }

            public FaultExceptionType FaultType { get; set; }
            public TenantConnectivityStatus ConnectivityStatusForUpdate { get; set; }
        }

        /// <summary>
        /// Common logic for fault exception processing
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="caller"></param>
        /// <param name="tenantId"></param>
        /// <param name="endpointAddress"></param>
        /// <param name="isGenericCaseAWarning"></param>
        /// <param name="genericExceptionText"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static FaultExceptionType ProcessFaultException(
            Exception ex,
            Object caller,
            string tenantId,
            Uri endpointAddress,
            string genericExceptionText,
            bool isGenericCaseAWarning = false,
            ActivityTrackingContext context = null)
        {
            FaultResult fr = null;
            
            //note that order is important here.
            fr = fr ?? TryProcessAsSerializationFault(tenantId, caller, ex);
            fr = fr ?? TryProcessAsConnectivityFault(tenantId, caller, ex);
            fr = fr ?? TryProcessAsIncompatibleClientFault(tenantId, caller, ex);
            fr = fr ?? TryProcessAsRetiredEndpointFault(tenantId, caller, ex);
            fr = fr ?? TryProcessAsTenantConnectionDisabledFault(tenantId, caller, ex);
            fr = fr ?? TryProcessAsInvalidResponseFault(tenantId, caller, ex);
            fr = fr ?? ProcessAsDefault(tenantId, caller, ex, genericExceptionText, isGenericCaseAWarning, context);

            // Update the state service if necessary
            if (fr.ConnectivityStatusForUpdate != TenantConnectivityStatus.None && endpointAddress != null)
            {
                CloudConnectivityStateMonitorHelper.UpdateTenantConnectivityStatusToIfUriTestSucceeds(
                    tenantId,
                    endpointAddress,
                    fr.ConnectivityStatusForUpdate);
            }

            return fr.FaultType;
        }

        private static FaultResult ProcessAsDefault(
            string tenantId,
            object caller,
            Exception ex,
            string genericExceptionText,
            bool isGenericCaseAWarning, 
            ActivityTrackingContext context)
        {
            // Not an exception that we know (default exception handling)
            FaultResult fr = new FaultResult();
            using (var lm = new LogManager())
            {
                string errorString = String.Format("{0}; exception: {1}", genericExceptionText, ex.ExceptionAsString());
                if (isGenericCaseAWarning && context != null)
                {
                    lm.WriteWarningForRequest(caller, context, errorString);
                }
                else
                {
                    if (context != null)
                    {
                        lm.WriteErrorForRequest(caller, context, errorString);
                    }
                    else
                    {
                        lm.WriteError(caller, errorString);
                    }
                }
            }

            fr.ConnectivityStatusForUpdate = TenantConnectivityStatus.CommunicationFailure;

            // Currently set to fault exception type none
            // Provide an indicator if this is a fault exception though
            // Just one we didn't understand
            if (ex is FaultException)
            {
                fr.FaultType = FaultExceptionType.GenericFaultException;
            }
            
            return fr;
        }

        static private FaultResult TryProcessAsSerializationFault(string tenantId, object caller, Exception ex)
        {
            // Serialization fault
            // This type created when a NetDispatcherFaultException is picked up in the service's
            // Custom IErrorHandler, since that exception is marked as internal, and it makes it to
            // This location as a generic FaultException, which we do not know how to handle
            FaultResult retval = null;
            var sfe = ex as FaultException<SerializationFault>;
            if (sfe != null)
            {
                WriteFaultAsError(caller, sfe.Detail);
                retval = new FaultResult(FaultExceptionType.Serialization, TenantConnectivityStatus.None) ;
            }
        
            return retval;
        }

        static private FaultResult TryProcessAsConnectivityFault(string tenantId, object caller, Exception ex)
        {
            // Connectivity fault exception (connection information incorrect for tenant)
            FaultResult retval = null;
            var cfe = ex as FaultException<ConnectivityFault>;
            if (cfe != null)
            {
                WriteFaultAsError(caller, cfe.Detail);
                retval = new FaultResult(FaultExceptionType.Connectivity, TenantConnectivityStatus.InvalidConnectionInformation);
                
                //shut down communication for this tenant. This message only comes thru for credential/validation issues.
                PremiseConfigurationRecord pcr = ConfigurationSettingFactory.RetrieveConfiguration(tenantId);
                TenantDisableCollaboration(pcr, TenantConnectionDisabledAction.DisableOnly);
            }
            
            return retval;
        }

        static private FaultResult TryProcessAsIncompatibleClientFault(string tenantId, object caller, Exception ex)
        {
            // Incompatible client fault
            FaultResult retval = null;
            var icfe = ex as FaultException<IncompatibleClientFault>;
            if (icfe != null)
            {
                WriteFaultAsError(caller, icfe.Detail);
                using (var proxy = NotificationServiceProxyFactory.CreateFromCatalog("localhost",ConnectorServiceUtils.CatalogServicePortNumber))
                {
                    proxy.NotifyIncompatibleClient();
                }

                CloudConnectivityStateMonitorHelper.NotifyIncompatibleClientFault(icfe.Detail);
                retval = new FaultResult(FaultExceptionType.IncompatibleClient,TenantConnectivityStatus.IncompatibleClient);
            }
         
            return retval;
        }

        static private FaultResult TryProcessAsRetiredEndpointFault(string tenantId, object caller, Exception ex)
        {
            // Retired endpoint fault (Endpoint no longer exists)
            FaultResult retval = null;
            var refe = ex as FaultException<RetiredEndpointFault>;
            if (refe != null)
            {
                WriteFaultAsError(caller, refe.Detail);
                NotifyRetiredEndpoint(tenantId, refe.Detail);
                retval = new FaultResult(FaultExceptionType.RetiredEndpoint, TenantConnectivityStatus.Reconfigure);
            }
            
            return retval;
        }

        static private FaultResult TryProcessAsTenantConnectionDisabledFault(string tenantId, object caller, Exception ex)
        {
            // Tenant connection disabled fault
            FaultResult retval = null;
            var tcdfe = ex as FaultException<TenantConnectionDisabledFault>;
            if (tcdfe != null)
            {
                WriteFaultAsError(caller, tcdfe.Detail);
                PremiseConfigurationRecord pcr = ConfigurationSettingFactory.RetrieveConfiguration(tenantId);
                TenantDisableCollaboration(pcr, tcdfe.Detail.Action);
                retval = new FaultResult(FaultExceptionType.TenantConnectionDisabled, TenantConnectivityStatus.TenantDisabled);
            }
            
            return retval;
        }

        static private FaultResult TryProcessAsInvalidResponseFault(string tenantId, object caller, Exception ex)
        {
            // Invalid response fault (response deemed invalid by the cloud)
            // This exception should only happen for the PutResponse call
            FaultResult retval = null;
            var irfe = ex as FaultException<InvalidResponseFault>;
            if (irfe != null)
            {
                // Response deemed invalid by the cloud
                WriteFaultAsError(caller, irfe.Detail);
                retval = new FaultResult(FaultExceptionType.InvalidResponse, TenantConnectivityStatus.Normal);
            }
            
            return retval;
        }
    }
}
