using System;
using System.AddIn.Hosting;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Sage.Connector.Common;
using Sage.Connector.ConnectorServiceCommon;
using Sage.Connector.Data;
using Sage.Connector.DomainContracts.BackOffice;
using Sage.Connector.Logging;
using Sage.Connector.ProcessExecution.Events;
using Sage.Connector.ProcessExecution.Interfaces;

namespace Sage.Connector.ProcessExecution.RequestActivator
{
    /// <summary>
    /// 
    /// </summary>
    public class ProcessRequestActivation
    {

        //TODO KMS: We need a factory the cold/warm/hot startups and maintains the collection of the processes running.  
        //TODO KMS: Do we have one of these per back office company configuration?   Does it really do anything for us? 

        private readonly ConcurrentBag<Task> _concurrentTasks = new ConcurrentBag<Task>();
        private readonly IList<IProcessRequest> _processRequests = new List<IProcessRequest>();
        private readonly bool _newProcess = true;
        private readonly Platform _platform = Platform.X86;

        /// <summary>
        /// 
        /// </summary>
        public ProcessRequestActivation()
        {
            _newProcess = true;
            _platform = Platform.X86;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newProcess"></param>
        /// <param name="platform"></param>
        public ProcessRequestActivation(bool newProcess, Platform platform)
        {
            _newProcess = newProcess;
            _platform = platform;
        }

        /// <summary>
        /// Executes the process request.
        /// </summary>
        /// <param name="responseProcessFunc">The response process function.</param>
        /// <param name="trackingContext">The tracking context.</param>
        /// <param name="backOfficeCompanyConfiguration">The back office company configuration.</param>
        /// <param name="feature">The feature.</param>
        /// <param name="payload">The payload.</param>
        /// <param name="cancellationTokenSource">The cancellation token source.</param>
        public void ExecuteProcessRequest(EventHandler<ResponseEventArgs> responseProcessFunc, ActivityTrackingContext trackingContext, 
            IBackOfficeCompanyConfiguration backOfficeCompanyConfiguration, String feature, string payload,
            CancellationTokenSource cancellationTokenSource)
        {
            using (var lm = new LogManager())
            {
                lm.AdvanceActivityState(null, trackingContext, ActivityState.State7_InvokingProcessExecution, ActivityEntryStatus.InProgress);
                //lm.WriteInfoForRequest(null, rw.ActivityTrackingContext, "Invoking " + ib.GetType().FullName + " on request '" + rw.ActivityTrackingContext.RequestId + "'");

            }

            AddInProcess executeAddInProcess = null;
            IApp appObj = new AppObject();

            try
            {

                // Assume that the current directory is the application folder,  
                // and that it contains the pipeline folder structure.
                String addInRoot = Environment.CurrentDirectory + "\\Pipeline";

                // Update the cache files of the pipeline segments and add-ins. 
                string[] warnings = AddInStore.Update(addInRoot);
                foreach (string warning in warnings)
                {
                    Console.WriteLine(warning);
                }

                // Search for the specific add-in of the Process Execution.  
                // Although the method returns a collection, there should be only 1 Addin returned. 

                Collection<AddInToken> processTokens =
                    AddInStore.FindAddIn(typeof(IProcessRequest), addInRoot,
                    addInRoot + @"\AddIns\ProcessExecution\Sage.Connector.ProcessExecution.Addin.dll",
                    "Sage.Connector.ProcessExecution.Addin.ProcessExecution");



                //There should be only 1 add-in for the process execution
                Debug.Assert(processTokens.Count.Equals(1), "There should be exactly 1 process execution add-in.");

                var processToken = processTokens.First();
                
                Debug.Print("ProcessRequestActivation -------------");
                Debug.Print("Name: {0}", processToken.Name);
                Debug.Print("Description : {0}", processToken.Description);
                Debug.Print("AddinFullName: {0}", processToken.AddInFullName);
                Debug.Print("AssemblyName: {0}", processToken.AssemblyName);
                Debug.Print("Version: {0}", processToken.Version);

                executeAddInProcess = new AddInProcess(_platform);

                // Activate the selected AddInToken in a new application domain  
                //// with the Internet trust level.
                var processRequest = _newProcess
                    ? processToken.Activate<IProcessRequest>(executeAddInProcess, AddInSecurityLevel.FullTrust)
                    : processToken.Activate<IProcessRequest>(AddInSecurityLevel.FullTrust);


#if (DEBUG)
                var controller = AddInController.GetAddInController(processRequest);
                Debug.Print("Controller is running in current process: {0}",
                    controller.AddInEnvironment.Process.IsCurrentProcess);
                Debug.Print("Controller process id: {0}", controller.AddInEnvironment.Process.ProcessId);
                Debug.Print("Controller platform: {0}", controller.AddInEnvironment.Process.Platform);

                //dump the request to debug
                Debug.Print("Request Id: {0}", trackingContext.RequestId);
                Debug.Print("Tenant Id: {0}", trackingContext.TenantId);
                Debug.Print("backOfficeCompanyConfiguration: {0}", backOfficeCompanyConfiguration.ToString());
                Debug.Print("feature: {0}", feature);
                Debug.Print("payload: {0}", payload);

#endif
                _processRequests.Add(processRequest);

                processRequest.Initialize(appObj);
                CancellationTokenRegistration reg = cancellationTokenSource.Token.Register(processRequest.RequestCancellation);
                processRequest.ProcessResponse += responseProcessFunc;

                var dispatchTask = Task.Factory.StartNew(() => processRequest.ProcessRequest(trackingContext.RequestId,
                                                                                        trackingContext.TenantId,
                                                                                        trackingContext.Id,
                                                                                        backOfficeCompanyConfiguration,
                                                                                        feature,
                                                                                        payload)
                                                            , cancellationTokenSource.Token);


                Debug.Print("Task {0} executing", dispatchTask.Id);
                _concurrentTasks.Add(dispatchTask);
                Debug.Print("dispatch task {0} added -- concurrent tasks count: {1} ", dispatchTask.Id, _concurrentTasks.Count);
                
                //for internal white box test that cancel works.
                //cancellationTokenSource.Cancel();
                
                dispatchTask.Wait();
                
                //unhook the registration token
                //TODO: when we hook up cancellation Validate there is no race condition here.
                reg.Dispose();
                
                _processRequests.Remove(processRequest);

                _concurrentTasks.TryTake(out dispatchTask);
                Debug.Print("dispatch task {0} removed -- concurrent tasks count: {1}", dispatchTask.Id,
                    _concurrentTasks.Count);

            }
            catch (Exception ex)
            {
                using (LogManager lm = new LogManager())
                {
                    lm.WriteErrorForRequest(this, trackingContext, ex.ExceptionAsString());
                }
            }
            finally
            {
                if (executeAddInProcess != null)
                {
                    executeAddInProcess.Shutdown();
                }
            }
        }
    }
}
