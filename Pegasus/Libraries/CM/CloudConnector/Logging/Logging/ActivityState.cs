
namespace Sage.Connector.Logging
{
    /// <summary>
    /// 
    /// </summary>
    public enum ActivityState
    {
        /// <summary>
        /// No ActivityState (default value automatically initialized by runtime)
        /// </summary>
        None = 0,

        /// <summary>
        /// New request is received from the Cloud
        /// </summary>
        State1_RequestReceivedFromCloud,

        /// <summary>
        /// Request is placed on the tenant's input queue by Messaging Service
        /// </summary>
        State2_EnqueueTenantInboxRequest,

        /// <summary>
        /// Request is retrieved from the tenant's input queue by the Dispatch Service
        /// </summary>
        State3_DequeueTenantInboxRequest,

        /// <summary>
        /// Request is placed on the tenant's binder work queue by the Dispatch Service
        /// </summary>
        State4_EnqueueTenantBinderRequest,

        /// <summary>
        /// Request is retrieved from the tenant's binder work queue by the Dispatch Service
        /// </summary>
        State5_DequeueTenantBinderRequest,

        /// <summary>
        /// Request-specific bindable work is about to begin
        /// </summary>
        State6_InvokingBindableWork,

        /// <summary>
        /// Process execution start, MAF execution
        /// </summary>
        State7_InvokingProcessExecution,

        /// <summary>
        /// Initial domain mediation MEF phase 1
        /// </summary>
        State8_InvokingDomainMediation,

        /// <summary>
        /// Invoking work found in domain mediation MEF phase 2, usually the back office plugin.
        /// </summary>
        State9_InvokingMediationBoundWork,

        /// <summary>
        /// The state10_ mediation bound work complete
        /// </summary>
        State10_MediationBoundWorkComplete,

        /// <summary>
        /// The state11_ domain mediation complete
        /// </summary>
        State11_DomainMediationComplete,

        /// <summary>
        /// The state12_ process execution complete
        /// </summary>
        State12_ProcessExecutionComplete,                
            
        /// <summary>
        /// Request-specific bindable work has completed
        /// </summary>
        State13_BindableWorkComplete,

        /// <summary>
        /// Response is placed on the tenant's output queue
        /// </summary>
        State14_EnqueueTenantOutboxResponse,

        /// <summary>
        /// Response is retrieved from the tenant's output queue by the Messaging Service
        /// </summary>
        State15_DequeueTenantOutboxResponse,

        /// <summary>
        /// Any related uploads are completed and the cloud response is finalized
        /// </summary>
        State16_UploadsCompletedAndResponseFinalized,

        /// <summary>
        /// Cloud response is sent
        /// </summary>
        State17_ResponseSentToCloud
    }
}
