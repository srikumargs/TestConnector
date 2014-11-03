using System.Resources;
using Newtonsoft.Json;
using Sage.Connector.Cloud.Integration.Interfaces;
using Sage.Connector.Cloud.Integration.Interfaces.DataContracts;
using Sage.Connector.Cloud.Integration.Interfaces.Requests;
using Sage.Connector.LinkedSource;
using Sage.Connector.MockCloudHostApp.Properties;
using Sage.Connector.SageCloudService;
using System;
using System.Runtime.Serialization.Formatters;

namespace Sage.Connector.MockCloudHostApp.Internal
{
    /// <summary>
    /// 
    /// </summary>
    public class ActionInvoker
    {

        /// <summary>
        ///
        /// </summary>
        /// <param name="action"></param>
        /// <param name="tenantId"></param>
        /// <param name="premKey"></param>
        /// <param name="configParams"></param>
        /// <param name="gatewayServiceInfo"></param>
        /// <param name="request"></param>
        public void PushAction(
            InvokeActionEnum action,
            string tenantId,
            string premKey,
            Cloud.Integration.Interfaces.WebAPI.Configuration configParams,
            ServiceInfo gatewayServiceInfo,
            ConfigurationRequest request = null)
        {
            InvokeAction(
                action,
                tenantId,
                premKey,
                configParams,
                gatewayServiceInfo,
                request);
        }

        /// <summary>
        /// Invoke the action. Change in Pegasus
        /// </summary>
        /// <param name="action"></param>
        /// <param name="tenantId"></param>
        /// <param name="premiseKey"></param>
        /// <param name="configParams"></param>
        /// <param name="gatewayServiceInfo"></param>
        /// <param name="customRequest"></param>
        private static void InvokeAction(
            InvokeActionEnum action,
            String tenantId,
            String premiseKey,
            Cloud.Integration.Interfaces.WebAPI.Configuration configParams,
            ServiceInfo gatewayServiceInfo,
            ConfigurationRequest customRequest = null)
        {
            //TODO: look at breaking the actions out into methods.
            switch (action)
            {

                case InvokeActionEnum.ProcessQuote:
                {
                    String payload = CreateProcessSalesDocumentPayload();

                    Guid testGuid1 = Guid.NewGuid();
                    DomainMediation dm = new DomainMediation(tenantId, "ProcessQuote", payload, "json");
                    DomainMediationRequest request = new DomainMediationRequest(testGuid1, DateTime.UtcNow, 0, 0,
                        "requestingUser", dm);

                    MockCloudService.StaticExternalAddToPremiseMessage(tenantId, request);
                    TraceUtils.WriteLine("Injected {1} Process Quote DM Request with Id: {0}", testGuid1, tenantId);
                    break;
                }
                case InvokeActionEnum.ProcessQuoteToOrder:
                {
                    String payload = CreateProcessSalesDocumentPayload();
                    Guid testGuid1 = Guid.NewGuid();
                    DomainMediation dm = new DomainMediation(tenantId, "ProcessQuoteToOrder", payload, "json");
                    DomainMediationRequest request = new DomainMediationRequest(testGuid1, DateTime.UtcNow, 0, 0, "requestingUser", dm);

                    MockCloudService.StaticExternalAddToPremiseMessage(tenantId, request);
                    TraceUtils.WriteLine("Injected {1} Process Quote To Order DM Request with Id: {0}", testGuid1, tenantId);
                    break;
                }
             
                                    
                case InvokeActionEnum.ProcessPaidOrder:
                {
                    String payload = CreateProcessSalesDocumentPayload();
                    Guid testGuid1 = Guid.NewGuid();
                    DomainMediation dm = new DomainMediation(tenantId, "ProcessPaidOrder", payload, "json");
                    DomainMediationRequest request = new DomainMediationRequest(testGuid1, DateTime.UtcNow, 0, 0, "requestingUser", dm);

                    MockCloudService.StaticExternalAddToPremiseMessage(tenantId, request);
                    TraceUtils.WriteLine("Injected {1} Process Paid Order DM Request with Id: {0}", testGuid1, tenantId);
                    break;
                }
                case InvokeActionEnum.ProcessStatements:
                {
                    String payload = CreateProcessProcessStatementsPayload();
                    Guid testGuid1 = Guid.NewGuid();
                    DomainMediation dm = new DomainMediation(tenantId, "ProcessStatements", payload, "json");
                    DomainMediationRequest request = new DomainMediationRequest(testGuid1, DateTime.UtcNow, 0, 0, "requestingUser", dm);

                    MockCloudService.StaticExternalAddToPremiseMessage(tenantId, request);
                    TraceUtils.WriteLine("Injected {1} Process Statements DM Request with Id: {0}", testGuid1, tenantId);
                    break;
                }

                case InvokeActionEnum.ProcessPayment:
                {
                    String payload = CreateProcessPaymentPayload();

                    Guid testGuid1 = Guid.NewGuid();
                    DomainMediation dm = new DomainMediation(tenantId, "ProcessPayment", payload, "json");
                    DomainMediationRequest request = new DomainMediationRequest(testGuid1, DateTime.UtcNow, 0, 0,
                        "requestingUser", dm);

                    MockCloudService.StaticExternalAddToPremiseMessage(tenantId, request);
                    TraceUtils.WriteLine("Injected {1} Process Payment DM Request with Id: {0}", testGuid1, tenantId);
                    break;
                }
                case InvokeActionEnum.ServiceProcessWorkOrderToInvoice:
                {
                    String payload = CreateProcessWorkOrderPayload();
                    Guid testGuid1 = Guid.NewGuid();
                    DomainMediation dm = new DomainMediation(tenantId, "ProcessWorkOrderToInvoice", payload, "json");
                    DomainMediationRequest request = new DomainMediationRequest(testGuid1, DateTime.UtcNow, 0, 0, "requestingUser", dm);

                    MockCloudService.StaticExternalAddToPremiseMessage(tenantId, request);
                    TraceUtils.WriteLine("Injected {1} Process Work Order to Invoice Request with Id: {0}", testGuid1, tenantId);
                    break;
                }
                case InvokeActionEnum.SyncTaxCodes:
                {
                    Guid testGuid1 = Guid.NewGuid();
                    DomainMediation dm = new DomainMediation(tenantId, "SyncTaxCodes", "{\"ResourceKindName\":\"TaxCodes\",\"CloudTick\":0}", "json");
                    DomainMediationRequest request = new DomainMediationRequest(testGuid1, DateTime.UtcNow, 0, 10, "requestingUser", dm);

                    MockCloudService.StaticExternalAddToPremiseMessage(tenantId, request);
                    TraceUtils.WriteLine("Injected {1} SyncTaxCodes Request with Id: {0}", testGuid1, tenantId);
                    break;
                }

                case InvokeActionEnum.SyncTaxSchedules:
                {
                    Guid testGuid1 = Guid.NewGuid();
                    DomainMediation dm = new DomainMediation(tenantId, "SyncTaxSchedules", "{\"ResourceKindName\":\"TaxSchedules\",\"CloudTick\":0}", "json");
                    DomainMediationRequest request = new DomainMediationRequest(testGuid1, DateTime.UtcNow, 0, 10, "requestingUser", dm);

                    MockCloudService.StaticExternalAddToPremiseMessage(tenantId, request);
                    TraceUtils.WriteLine("Injected {1} SyncTaxSchedules Request with Id: {0}", testGuid1, tenantId);
                    break;
                }

                case InvokeActionEnum.SyncInventoryItems:
                {
                    Guid testGuid1 = Guid.NewGuid();
                    DomainMediation dm = new DomainMediation(tenantId, "SyncInventoryItems", "{\"ResourceKindName\":\"InventoryItems\",\"CloudTick\":0}", "json");
                    DomainMediationRequest request = new DomainMediationRequest(testGuid1, DateTime.UtcNow, 0, 10, "requestingUser", dm);

                    MockCloudService.StaticExternalAddToPremiseMessage(tenantId, request);
                    TraceUtils.WriteLine("Injected {1} SyncInventoryItems Request with Id: {0}", testGuid1, tenantId);
                    break;
                }


                case InvokeActionEnum.SyncServiceTypes:
                {
                    Guid testGuid1 = Guid.NewGuid();
                    DomainMediation dm = new DomainMediation(tenantId, "SyncServiceTypes", "{\"ResourceKindName\":\"ServiceTypes\",\"CloudTick\":0}", "json");
                    DomainMediationRequest request = new DomainMediationRequest(testGuid1, DateTime.UtcNow, 0, 10, "requestingUser", dm);

                    MockCloudService.StaticExternalAddToPremiseMessage(tenantId, request);
                    TraceUtils.WriteLine("Injected {1} SyncServiceTypes Request with Id: {0}", testGuid1, tenantId);
                    break;
                }

                case InvokeActionEnum.SyncCustomers:
                    {
                        Guid testGuid1 = Guid.NewGuid();
                        DomainMediation dm = new DomainMediation(tenantId, "SyncCustomers", "{\"ResourceKindName\":\"Customers\",\"CloudTick\":1}", "json");
                        DomainMediationRequest request = new DomainMediationRequest(testGuid1, DateTime.UtcNow, 0, 10, "requestingUser", dm);

                        MockCloudService.StaticExternalAddToPremiseMessage(tenantId, request);
                        TraceUtils.WriteLine("Injected {1} SyncCustomers Request with Id: {0}", testGuid1, tenantId);
                        break;
                    }

                case InvokeActionEnum.SyncSalespersons:
                    {
                        Guid testGuid1 = Guid.NewGuid();
                        DomainMediation dm = new DomainMediation(tenantId, "SyncSalespersons", "{\"ResourceKindName\":\"Salespersons\",\"CloudTick\":0}", "json");
                        DomainMediationRequest request = new DomainMediationRequest(testGuid1, DateTime.UtcNow, 0, 10, "requestingUser", dm);

                        MockCloudService.StaticExternalAddToPremiseMessage(tenantId, request);
                        TraceUtils.WriteLine("Injected {1} SyncSalespersons Request with Id: {0}", testGuid1, tenantId);
                        break;
                    }

                case InvokeActionEnum.SyncSalespersonCustomers:
                    {
                        Guid testGuid1 = Guid.NewGuid();
                        DomainMediation dm = new DomainMediation(tenantId, "SyncSalespersonCustomers", "{\"ResourceKindName\":\"SalespersonCustomers\",\"CloudTick\":0}", "json");
                        DomainMediationRequest request = new DomainMediationRequest(testGuid1, DateTime.UtcNow, 0, 10, "requestingUser", dm);

                        MockCloudService.StaticExternalAddToPremiseMessage(tenantId, request);
                        TraceUtils.WriteLine("Injected {1} SyncSalespersonCustomers Request with Id: {0}", testGuid1, tenantId);
                        break;
                    }

                case InvokeActionEnum.SyncInvoices:
                    {
                        Guid testGuid1 = Guid.NewGuid();
                        DomainMediation dm = new DomainMediation(tenantId, "SyncInvoices", "{\"ResourceKindName\":\"Invoices\",\"CloudTick\":0}", "json");
                        DomainMediationRequest request = new DomainMediationRequest(testGuid1, DateTime.UtcNow, 0, 10, "requestingUser", dm);

                        MockCloudService.StaticExternalAddToPremiseMessage(tenantId, request);
                        TraceUtils.WriteLine("Injected {1} SyncInvoices Request with Id: {0}", testGuid1, tenantId);
                        break;
                    }

                case InvokeActionEnum.SyncInvoiceBalances:
                    {
                        Guid testGuid1 = Guid.NewGuid();
                        DomainMediation dm = new DomainMediation(tenantId, "SyncInvoiceBalances", "{\"ResourceKindName\":\"InvoicesBalances\",\"CloudTick\":0}", "json");
                        DomainMediationRequest request = new DomainMediationRequest(testGuid1, DateTime.UtcNow, 0, 10, "requestingUser", dm);

                        MockCloudService.StaticExternalAddToPremiseMessage(tenantId, request);
                        TraceUtils.WriteLine("Injected {1} SyncInvoiceBalances Request with Id: {0}", testGuid1, tenantId);
                        break;
                    }

                case InvokeActionEnum.ScheduledSynchronization:
                    {
                        Guid testGuid1 = Guid.NewGuid();
                        DomainMediation dm = new DomainMediation(tenantId, "ScheduledSynchronization", "{\"ResourceKindName\":\"ScheduledSynchronization\",\"CloudTick\":0}", "json");
                        DomainMediationRequest request = new DomainMediationRequest(testGuid1, DateTime.UtcNow, 0, 10, "requestingUser", dm);

                        MockCloudService.StaticExternalAddToPremiseMessage(tenantId, request);
                        TraceUtils.WriteLine("Injected {1} ScheduledSynchronization Request with Id: {0}", testGuid1, tenantId);
                        break;
                    }

                case InvokeActionEnum.GetBackOfficePluginConfiguration:
                    {
                        Guid testGuid1 = Guid.NewGuid();
                        DomainMediation dm = new DomainMediation(tenantId, "GetBackOfficeConfiguration", "", "json");
                        DomainMediationRequest request = new DomainMediationRequest(testGuid1, DateTime.UtcNow, 0, 10, "requestingUser", dm);

                        MockCloudService.StaticExternalAddToPremiseMessage(tenantId, request);
                        TraceUtils.WriteLine("Injected {1} GetBackOfficeConfiguration Request with Id: {0}", testGuid1, tenantId);
                        break;
                    }

              
                case InvokeActionEnum.Loopback:
                    {
                        Guid testGuid1 = Guid.NewGuid();
                        LoopBackRequest request = new LoopBackRequest(testGuid1, DateTime.UtcNow, 0, 1, "requestingUser");
                        MockCloudService.StaticExternalAddToPremiseMessage(tenantId, request);
                        TraceUtils.WriteLine("Injected {1} LoopBackRequest with Id: {0}", testGuid1, tenantId);
                    }
                    break;
                case InvokeActionEnum.UpdateConfigParams:
                    {
                        Guid testGuid1 = Guid.NewGuid();
                        //build up config params here.
                        var newParams = configParams;
                        newParams.LargeResponseSizeThreshold = 15000000;
                        newParams.MinCommunicationFailureRetryInterval = TimeSpan.FromSeconds(750);
                        newParams.MaxCommunicationFailureRetryInterval = TimeSpan.FromSeconds(30000);
                        UpdateConfigParamsRequest request = new UpdateConfigParamsRequest(testGuid1, DateTime.UtcNow, newParams, 0, 0, "requestingUser");
                        MockCloudService.StaticExternalAddToPremiseMessage(tenantId, request);
                        TraceUtils.WriteLine("Injected {1} UpdateConfigParamsRequest with Id: {0}", testGuid1, tenantId);
                    }
                    break;
                case InvokeActionEnum.UpdateCustomConfigParams:
                    {
                        Guid testGuid1 = Guid.NewGuid();
                        Sage.Connector.Cloud.Integration.Interfaces.WebAPI.Configuration newParams = null;
                        if (customRequest.PassThru != null)
                        {
                            if (customRequest.PassThru.GetType() == typeof(Sage.Connector.Cloud.Integration.Interfaces.WebAPI.Configuration))
                            {
                                newParams = customRequest.PassThru as Sage.Connector.Cloud.Integration.Interfaces.WebAPI.Configuration;
                            }
                        }

                        UpdateConfigParamsRequest request = new UpdateConfigParamsRequest(testGuid1, DateTime.UtcNow, newParams, 0, 0, "requestingUser");
                        MockCloudService.StaticExternalAddToPremiseMessage(tenantId, request);
                        TraceUtils.WriteLine("Injected {1} UpdateConfigParamsRequest with Id: {0}", testGuid1, tenantId);
                    }
                    break;
                case InvokeActionEnum.DeleteTenant:
                    {
                        MockCloudService.DeleteTenant(tenantId);
                    }
                    break;
                case InvokeActionEnum.HealthCheck:
                    {
                        Guid testGuid1 = Guid.NewGuid();
                        HealthCheckRequest request = new HealthCheckRequest(testGuid1, DateTime.UtcNow, 0, 2, "requestingUser");
                        MockCloudService.StaticExternalAddToPremiseMessage(tenantId, request);
                        TraceUtils.WriteLine("Injected {1} HealthCheckRequest with Id: {0}", testGuid1, tenantId);
                    }
                    break;

                case InvokeActionEnum.ValidateBackOfficeIsInstalled:
                    {
                        Guid testGuid1 = Guid.NewGuid();
                        DomainMediation dm = new DomainMediation(tenantId, "ValidateBackOfficeIsInstalled", "", "json");
                        DomainMediationRequest request = new DomainMediationRequest(testGuid1, DateTime.UtcNow, 0, 10, "requestingUser", dm);

                        MockCloudService.StaticExternalAddToPremiseMessage(tenantId, request);
                        TraceUtils.WriteLine("Injected {1} ValidateBackOfficeIsInstalled Request with Id: {0}", testGuid1, tenantId);
                        
                    }
                    break;

                case InvokeActionEnum.GetPluginInformation:
                    {
                        Guid testGuid1 = Guid.NewGuid();
                        DomainMediation dm = new DomainMediation(tenantId, "GetPluginInformation", "", "json");
                        DomainMediationRequest request = new DomainMediationRequest(testGuid1, DateTime.UtcNow, 0, 10, "requestingUser", dm);

                        MockCloudService.StaticExternalAddToPremiseMessage(tenantId, request);
                        TraceUtils.WriteLine("Injected {1} GetPluginInformation Request with Id: {0}", testGuid1, tenantId);

                    }
                    break;

                case InvokeActionEnum.GetPluginInformationCollection:
                    {
                        Guid testGuid1 = Guid.NewGuid();
                        DomainMediation dm = new DomainMediation(tenantId, "GetPluginInformationCollection", "", "json");
                        DomainMediationRequest request = new DomainMediationRequest(testGuid1, DateTime.UtcNow, 0, 10, "requestingUser", dm);

                        MockCloudService.StaticExternalAddToPremiseMessage(tenantId, request);
                        TraceUtils.WriteLine("Injected {1} GetPluginInformationCollection Request with Id: {0}", testGuid1, tenantId);

                    }
                    break;

                case InvokeActionEnum.GetInstalledBackOfficePluginInformationCollection:
                    {
                        Guid testGuid1 = Guid.NewGuid();
                        DomainMediation dm = new DomainMediation(tenantId, "GetInstalledBackOfficePluginInformationCollection", "", "json");
                        DomainMediationRequest request = new DomainMediationRequest(testGuid1, DateTime.UtcNow, 0, 10, "requestingUser", dm);

                        MockCloudService.StaticExternalAddToPremiseMessage(tenantId, request);
                        TraceUtils.WriteLine("Injected {1} GetInstalledBackOfficePluginInformationCollection Request with Id: {0}", testGuid1, tenantId);

                    }
                    break;

                case InvokeActionEnum.GetCompanyConnectionManagementCredentialsNeeded:
                    {
                        Guid testGuid1 = Guid.NewGuid();
                        string requestName = "GetCompanyConnectionManagementCredentialsNeeded";
                        DomainMediation dm = new DomainMediation(tenantId, requestName, "", "json");
                        DomainMediationRequest request = new DomainMediationRequest(testGuid1, DateTime.UtcNow, 0, 10, "requestingUser", dm);

                        MockCloudService.StaticExternalAddToPremiseMessage(tenantId, request);
                        TraceUtils.WriteLine("Injected {1} {2} Request with Id: {0}", testGuid1, tenantId, requestName);
                    }
                    break;

                case InvokeActionEnum.GetCompanyConnectionCredentialsNeeded:
                    {
                        //Guid testGuid1 = Guid.NewGuid();
                        //string requestName = "GetCompanyConnectionCredentialsNeeded";
                        //DomainMediation dm = new DomainMediation(tenantId, requestName, "", "json");
                        //DomainMediationRequest request = new DomainMediationRequest(testGuid1, DateTime.UtcNow, 0, 10, "requestingUser", dm);

                        //MockCloudService.StaticExternalAddToPremiseMessage(tenantId, request);
                        //TraceUtils.WriteLine("Injected {1} {2} Request with Id: {0}", testGuid1, tenantId, requestName);
                    }
                    break;
                    
                case InvokeActionEnum.ValidateCompanyConnectionCredentials:
                    {

                        //Guid testGuid1 = Guid.NewGuid();
                        //string requestName = "ValidateCompanyConnectionCredentials";
                        //DomainMediation dm = new DomainMediation(tenantId, requestName, "", "json");
                        //DomainMediationRequest request = new DomainMediationRequest(testGuid1, DateTime.UtcNow, 0, 10, "requestingUser", dm);

                        //MockCloudService.StaticExternalAddToPremiseMessage(tenantId, request);
                        //TraceUtils.WriteLine("Injected {1} {2} Request with Id: {0}", testGuid1, tenantId, requestName);
                    }
                    break;

                case InvokeActionEnum.ValidateCompanyConnectionManagementCredentials:
                    {
                        //Guid testGuid1 = Guid.NewGuid();
                        //string requestName = "ValidateCompanyConnectionManagementCredentials";
                        //DomainMediation dm = new DomainMediation(tenantId, requestName, "", "json");
                        //DomainMediationRequest request = new DomainMediationRequest(testGuid1, DateTime.UtcNow, 0, 10, "requestingUser", dm);

                        //MockCloudService.StaticExternalAddToPremiseMessage(tenantId, request);
                        //TraceUtils.WriteLine("Injected {1} {2} Request with Id: {0}", testGuid1, tenantId, requestName);
                    }
                    break;

            }
        }

        private static string CreateProcessPaymentPayload()
        {
            return new ResourceManager(typeof(Resources)).GetString("PaymentPayload");
        }

        private static string CreateProcessSalesDocumentPayload()
        {

            return new ResourceManager(typeof(Resources)).GetString("SalesDocumentPayload");
        }

        /// <summary>
        /// Create the payload for process statements
        /// </summary>
        /// <returns></returns>
        private static string CreateProcessProcessStatementsPayload()
        {
            return new ResourceManager(typeof (Resources)).GetString("ProcessStatementsPayload");

        }
        /// <summary>
        /// Create the payload for proces work order to Invoice 
        /// </summary>
        /// <returns></returns>
        private static string CreateProcessWorkOrderPayload()
        {
            return new ResourceManager(typeof (Resources)).GetString("WorkOrderPayload");

        }    
    }
}
