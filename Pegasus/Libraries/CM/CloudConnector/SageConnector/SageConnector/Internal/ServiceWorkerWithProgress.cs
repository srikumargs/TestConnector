using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Security;
using System.Threading;
using System.Windows.Forms;
using Sage.Connector.Common;
using Sage.Connector.Management;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.Connector.StateService.Proxy;
using Sage.CRE.Core.UI;
using SageConnector.ViewModel;

namespace SageConnector.Internal
{
    /// <summary>
    /// A helper class to call into services and present a "please wait" progress dialog
    /// </summary>
    internal sealed class ServiceWorkerWithProgress : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        public ServiceWorkerWithProgress(IWin32Window owner)
        {
            _owner = owner;
        }

        public void Dispose()
        {
            lock (_lockObject)
            {
                if (_backgroundWorker != null)
                {
                    _backgroundWorker.Dispose();
                    _backgroundWorker = null;
                }

                if (_progressForm != null)
                {
                    _progressForm.Dispose();
                    _progressForm = null;
                }
            }
        }

        public Boolean UserCancelled
        { get { return _userCancelled; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="backOfficeId"></param>
        /// <param name="caption"></param>
        /// <param name="backOfficeConnectionCredentials"></param>
        /// <returns></returns>
        public ValidateBackOfficeConnectionResponse ValidateBackOfficeConnection(String backOfficeId, String caption, string backOfficeConnectionCredentials)
        {
            InitializeProgressForm(caption);

            InitializeBackgroundWorker(
                _backgroundWorker_ValidateBackOfficeConnection_DoWork,
                _backgroundWorker_ValidateBackOfficeConnection_WorkCompleted,
                new Tuple<String, String>(backOfficeId, backOfficeConnectionCredentials));

            ShowProgressDialog();

            return _validateBackOfficeConnectionResponse;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="cloudEndpoint"></param>
        /// <param name="cloudTenantId"></param>
        /// <param name="cloudPremiseKey"></param>
        /// <returns></returns>
        public ValidateTenantConnectionResponse ValidateTenantConnection(String caption,
            String cloudEndpoint,
            String cloudTenantId,
            String cloudPremiseKey)
        {
            InitializeProgressForm(caption);

            InitializeBackgroundWorker(
                _backgroundWorker_ValidateTenantConnection_DoWork,
                _backgroundWorker_ValidateTenantConnection_WorkCompleted,
                new Tuple<String, String, String>(cloudEndpoint, cloudTenantId, cloudPremiseKey));

            ShowProgressDialog();

            return _validateTenantConnectionResponse;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="backOfficeId"></param>
        /// <param name="backOfficeConnectionCredentials"></param>
        /// <param name="cloudEndpoint"></param>
        /// <param name="cloudTenantId"></param>
        /// <param name="cloudPremiseKey"></param>
        /// <param name="validateBackOfficeConnectionResponse"></param>
        /// <param name="validateTenantConnectionResponse"></param>
        public void ValidateConnections(
            String caption,
            String backOfficeId,
            String backOfficeConnectionCredentials,
            String cloudEndpoint,
            String cloudTenantId,
            String cloudPremiseKey,
            out ValidateBackOfficeConnectionResponse validateBackOfficeConnectionResponse,
            out ValidateTenantConnectionResponse validateTenantConnectionResponse)
        {
            validateBackOfficeConnectionResponse = null;
            validateTenantConnectionResponse = null;

            InitializeProgressForm(caption);

            //note took backOfficeUserName out as that is now part of the credentials and we blew out the max tuple size.
            //will get fixed up when we clear out the other back office info that is no longer needed separate items
            InitializeBackgroundWorker(
                _backgroundWorker_ValidateConnections_DoWork,
                _backgroundWorker_ValidateConnections_WorkCompleted,
                new Tuple<String, String, String, String, Tuple<String>>(backOfficeId, backOfficeConnectionCredentials, cloudEndpoint, cloudTenantId, new Tuple<String>(cloudPremiseKey)));

            ShowProgressDialog();

            validateBackOfficeConnectionResponse = _validateBackOfficeConnectionResponse;
            validateTenantConnectionResponse = _validateTenantConnectionResponse;
        }

        //public BackOfficeConnectionsForCredentialsResponse BackOfficeConnectionsForCredentials(String caption,
        //    String pluginId,
        //    String userName,
        //    SecureString password)
        //{
        //    InitializeProgressForm(caption);

        //    InitializeBackgroundWorker(
        //        _backgroundWorker_BackOfficeConnectionsForCredentials_DoWork,
        //        _backgroundWorker_BackOfficeConnectionsForCredentials_WorkCompleted,
        //        new Tuple<String, String, SecureString>(pluginId, userName, password));

        //    ShowProgressDialog();

        //    return _backOfficeConnectionsForCredentialsResponse;
        //}

        /// <summary>
        /// BackOfficePlugins work 
        /// </summary>
        /// <param name="caption"></param>
        /// <returns></returns>
        public BackOfficePluginsResponse BackOfficePlugins(String caption)
        {
            InitializeProgressForm(caption);

            InitializeBackgroundWorker(
                _backgroundWorker_BackOfficePlugins_DoWork,
                _backgroundWorker_BackOfficePlugins_WorkCompleted,
                null);

            ShowProgressDialog();

            return _backOfficePluginsResponse;
        }

        public ValidateBackOfficeAdminCredentialsResponse ValidateBackOfficeAdminCredentials(String caption,
            String pluginId,
            IDictionary<string, string> credentials
            )
        {
            InitializeProgressForm(caption);

            InitializeBackgroundWorker(
                _backgroundWorker_ValidateBackOfficeAdminCredentials_DoWork,
                _backgroundWorker_ValidateBackOfficeAdminCredentials_WorkCompleted,
                new Tuple<String, IDictionary<string, string>>(pluginId, credentials));

            ShowProgressDialog();

            return _validateBackOfficeAdminCredentialsResponse;
        }

        public void ValidateBackOfficeAdminCredentialsAndGetBackOfficeConnections(String caption,
            String pluginId,
            IDictionary<string, string> credentials,
            out ValidateBackOfficeAdminCredentialsResponse validateBackOfficeAdminCredentialsResponse,
            out BackOfficeConnectionsForCredentialsResponse backOfficeConnectionsForCredentialsResponse)
        {
            validateBackOfficeAdminCredentialsResponse = null;
            backOfficeConnectionsForCredentialsResponse = null;

            InitializeProgressForm(caption);

            InitializeBackgroundWorker(
                _backgroundWorker_ValidateBackOfficeAdminCredentialsAndGetBackOfficeConnections_DoWork,
                _backgroundWorker_ValidateBackOfficeAdminCredentialsAndGetBackOfficeConnections_WorkCompleted,
                new Tuple<String, IDictionary<string, string>>(pluginId, credentials));

            ShowProgressDialog();

            validateBackOfficeAdminCredentialsResponse = _validateBackOfficeAdminCredentialsResponse;
            backOfficeConnectionsForCredentialsResponse = _backOfficeConnectionsForCredentialsResponse;
        }

        /// <summary>
        /// Update the active status of all connections that we manage
        /// Stores any error message collection in the model as a result
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="configurations"></param>
        public void ValidateAndUpdateActiveStatusOnAllConnections(String caption, ConfigurationViewModelList configurations)
        {
            InitializeProgressForm(caption);

            InitializeBackgroundWorker(
                _backgroundWorker_ValidateAndUpdateActiveStatusOnAllConnections_DoWork,
                _backgroundWorker_ValidateAndUpdateActiveStatusOnAllConnections_WorkCompleted,
                configurations);

            ShowProgressDialog();
        }

        #region Private methods
        private void InitializeProgressForm(String caption)
        {
            _userCancelled = false;
            _validateTenantConnectionResponse = null;
            _validateBackOfficeConnectionResponse = null;
            _backOfficeConnectionsForCredentialsResponse = null;
            _validateBackOfficeAdminCredentialsResponse = null;

            _progressForm = new ProgressForm();
            _progressForm.UserCanRequestCancel = true;
            _progressForm.UserCancelled += new EventHandler(_progressForm_UserCancelled);
            _progressForm.Text = caption;
        }

        private void InitializeBackgroundWorker(DoWorkEventHandler doWorkEventHandler, RunWorkerCompletedEventHandler runWorkerCompletedEventHandler, Object argument)
        {
            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.WorkerSupportsCancellation = true;
            _backgroundWorker.DoWork += new DoWorkEventHandler(doWorkEventHandler);
            _backgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(runWorkerCompletedEventHandler);
            _backgroundWorker.RunWorkerAsync(argument);
        }

        private void ShowProgressDialog()
        {
            _progressForm.ShowDialog(_owner);
        }

        private Boolean HandleCancelOrUpdateProgress(DoWorkEventArgs e, Int32 sleep, MethodInvoker i)
        {
            lock (_lockObject)
            {
                if (_backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return false;
                }
                else
                {
                    // artificial delay so that the text is up for a minimum time
                    if (sleep != 0)
                    {
                        System.Threading.Thread.Sleep(sleep);
                    }

                    // update status
                    if (i != null)
                    {
                        _progressForm.BeginInvoke(i);
                    }
                }
            }

            return true;
        }

        private void _progressForm_UserCancelled(Object sender, EventArgs e)
        {
            if (_backgroundWorker.WorkerSupportsCancellation)
            {
                lock (_lockObject)
                {
                    _backgroundWorker.CancelAsync();
                    _progressForm.Dispose();
                    _progressForm = null;
                    _userCancelled = true;
                }
            }
        }

        private void HandleWorkCompleted(RunWorkerCompletedEventArgs e, Action<Object> i)
        {
            lock (_lockObject)
            {
                if (!_userCancelled)
                {
                    i(e.Result);
                }

                if (_progressForm != null)
                {
                    _progressForm.Dispose();
                    _progressForm = null;
                }
            }
        }

        private void _backgroundWorker_ValidateBackOfficeConnection_DoWork(Object sender, DoWorkEventArgs e)
        {
            try
            {
                Tuple<String, String> argument = e.Argument as Tuple<String, String>;

                // wait until the progress form has been created (so we can update it)
                _progressForm.WaitUntilReady();
                _progressForm.ShowMarqueeProgressBar();

                // do the real work
                // Validate back office connection via the state service using model data
                var stateSerivce = new BackOfficeServiceManager();
                e.Result = stateSerivce.ValidateBackOfficeConnectionCredentialsAsString(
                        argument.Item1,
                        argument.Item2);
                

                // give a little more time to fully spin up
                if (!HandleCancelOrUpdateProgress(e, 2000, null))
                    return;
            }
            catch (Exception ex)
            {
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(null, ex.ExceptionAsString());
                }
            }
        }

        private void _backgroundWorker_ValidateBackOfficeConnection_WorkCompleted(Object sender, RunWorkerCompletedEventArgs e)
        {
            HandleWorkCompleted(e, (result) => { _validateBackOfficeConnectionResponse = (result as ValidateBackOfficeConnectionResponse); });
        }

        private void _backgroundWorker_ValidateTenantConnection_DoWork(Object sender, DoWorkEventArgs e)
        {
            try
            {
                Tuple<String, String, String> argument = e.Argument as Tuple<String, String, String>;

                // wait until the progress form has been created (so we can update it)
                _progressForm.WaitUntilReady();
                _progressForm.ShowMarqueeProgressBar();

                // do the real work
                using (var proxy = TenantValidationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                {
                    e.Result = proxy.ValidateTenantConnection(argument.Item1, argument.Item2, argument.Item3, String.Empty);
                }

                // give a little more time to fully spin up
                if (!HandleCancelOrUpdateProgress(e, 2000, null))
                    return;
            }
            catch (Exception ex)
            {
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(null, ex.ExceptionAsString());
                }
            }
        }

        private void _backgroundWorker_ValidateTenantConnection_WorkCompleted(Object sender, RunWorkerCompletedEventArgs e)
        {
            HandleWorkCompleted(e, (result) => { _validateTenantConnectionResponse = (result as ValidateTenantConnectionResponse); });
        }

        private void _backgroundWorker_ValidateConnections_DoWork(Object sender, DoWorkEventArgs e)
        {
            try
            {
                Tuple<String, String, String, String, Tuple<String>> argument = e.Argument as Tuple<String, String, String, String, Tuple<String>>;

                // wait until the progress form has been created (so we can update it)
                _progressForm.WaitUntilReady();
                _progressForm.ShowMarqueeProgressBar();

                // do the real work
                ValidateBackOfficeConnectionResponse returnPart1 = null;

                // do the real work
                // Validate back office connection via the state service using model data
                var stateSerivce = new BackOfficeServiceManager();
                returnPart1 = stateSerivce.ValidateBackOfficeConnectionCredentialsAsString(
                        argument.Item1,
                        argument.Item2);

                if (!HandleCancelOrUpdateProgress(e, 0, null))
                    return;

                ValidateTenantConnectionResponse returnPart2 = null;
                using (var proxy = TenantValidationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                {
                    returnPart2 = proxy.ValidateTenantConnection(argument.Item3, argument.Item4, argument.Item5.Item1, String.Empty);
                }

                // give a little more time to fully spin up
                if (!HandleCancelOrUpdateProgress(e, 2000, null))
                    return;

                e.Result = new Tuple<ValidateBackOfficeConnectionResponse, ValidateTenantConnectionResponse>(returnPart1, returnPart2);
            }
            catch (Exception ex)
            {
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(null, ex.ExceptionAsString());
                }
            }
        }
        private void _backgroundWorker_ValidateConnections_WorkCompleted(Object sender, RunWorkerCompletedEventArgs e)
        {
            HandleWorkCompleted(e, (result) =>
            {
                var theResult = (result as Tuple<ValidateBackOfficeConnectionResponse, ValidateTenantConnectionResponse>);
                if (theResult != null)
                {
                    _validateBackOfficeConnectionResponse = theResult.Item1;
                    _validateTenantConnectionResponse = theResult.Item2;
                }
            });
        }

        private void _backgroundWorker_BackOfficePlugins_DoWork(Object sender, DoWorkEventArgs e)
        {
            try
            {
                Tuple<String, String, SecureString> argument = e.Argument as Tuple<String, String, SecureString>;

                // wait until the progress form has been created (so we can update it)
                _progressForm.WaitUntilReady();
                _progressForm.ShowMarqueeProgressBar();

                // do the real work
                var stateService = new BackOfficeServiceManager();
                e.Result = stateService.GetBackOfficePlugins();

                // give a little more time to fully spin up
                if (!HandleCancelOrUpdateProgress(e, 2000, null))
                    return;
            }
            catch (Exception ex)
            {
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(null, ex.ExceptionAsString());
                }
            }
        }

        private void _backgroundWorker_BackOfficePlugins_WorkCompleted(Object sender, RunWorkerCompletedEventArgs e)
        {
            HandleWorkCompleted(e, (result) => { _backOfficePluginsResponse = (result as BackOfficePluginsResponse); });
        }

  

        //private void _backgroundWorker_BackOfficeConnectionsForCredentials_DoWork(Object sender, DoWorkEventArgs e)
        //{
        //    try
        //    {
        //        Tuple<String, String, SecureString> argument = e.Argument as Tuple<String, String, SecureString>;

        //        // wait until the progress form has been created (so we can update it)
        //        _progressForm.WaitUntilReady();
        //        _progressForm.ShowMarqueeProgressBar();

        //        // do the real work
        //        using (var proxy = BackOfficeValidationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
        //        {
        //            e.Result = proxy.BackOfficeConnectionsForCredentials(
        //                argument.Item1,
        //                argument.Item2,
        //                argument.Item3.ToNonSecureString());
        //        }


        //        // give a little more time to fully spin up
        //        if (!HandleCancelOrUpdateProgress(e, 2000, null))
        //            return;
        //    }
        //    catch (Exception ex)
        //    {
        //        using (var logger = new SimpleTraceLogger())
        //        {
        //            logger.WriteError(null, ex.ExceptionAsString());
        //        }
        //    }
        //}

        //private void _backgroundWorker_BackOfficeConnectionsForCredentials_WorkCompleted(Object sender, RunWorkerCompletedEventArgs e)
        //{
        //    HandleWorkCompleted(e, (result) => { _backOfficeConnectionsForCredentialsResponse = (result as BackOfficeConnectionsForCredentialsResponse); });
        //}

        private void _backgroundWorker_ValidateBackOfficeAdminCredentials_DoWork(Object sender, DoWorkEventArgs e)
        {
            try
            {
                var argument = e.Argument as Tuple<String, IDictionary<string, string>>;

                // wait until the progress form has been created (so we can update it)
                _progressForm.WaitUntilReady();
                _progressForm.ShowMarqueeProgressBar();

                // do the real work
                var stateService = new BackOfficeServiceManager();
                e.Result = stateService.ValidateBackOfficeAdminCredentials(
                        argument.Item1,
                        argument.Item2);
                


                // give a little more time to fully spin up
                if (!HandleCancelOrUpdateProgress(e, 2000, null))
                    return;
            }
            catch (Exception ex)
            {
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(null, ex.ExceptionAsString());
                }
            }
        }

        private void _backgroundWorker_ValidateBackOfficeAdminCredentials_WorkCompleted(Object sender, RunWorkerCompletedEventArgs e)
        {
            HandleWorkCompleted(e, (result) => { _validateBackOfficeAdminCredentialsResponse = (result as ValidateBackOfficeAdminCredentialsResponse); });
        }

        private void _backgroundWorker_ValidateBackOfficeAdminCredentialsAndGetBackOfficeConnections_DoWork(Object sender, DoWorkEventArgs e)
        {
            try
            {
                var argument = e.Argument as Tuple<String, IDictionary<string, string>>;

                // wait until the progress form has been created (so we can update it)
                _progressForm.WaitUntilReady();
                _progressForm.ShowMarqueeProgressBar();

                // do the real work
                ValidateBackOfficeAdminCredentialsResponse returnPart1 = null;
                var stateService = new BackOfficeServiceManager();
                returnPart1 = stateService.ValidateBackOfficeAdminCredentials(
                        argument.Item1,
                        argument.Item2);
          

                if (!HandleCancelOrUpdateProgress(e, 0, null))
                    return;

                //Change this to use connection credentials? Flow seems correct.
                // For now always return null for this
                BackOfficeConnectionsForCredentialsResponse returnPart2 = null;
                //using (var proxy = BackOfficeValidationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                //{
                //    returnPart2 = proxy.BackOfficeConnectionsForCredentials(
                //        argument.Item1,
                //        argument.Item2,
                //        argument.Item3.ToNonSecureString());
                //}

                // give a little more time to fully spin up
                if (!HandleCancelOrUpdateProgress(e, 2000, null))
                    return;

                e.Result = new Tuple<ValidateBackOfficeAdminCredentialsResponse,
                    BackOfficeConnectionsForCredentialsResponse>(returnPart1, returnPart2);
            }
            catch (Exception ex)
            {
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(null, ex.ExceptionAsString());
                }
            }
        }

        private void _backgroundWorker_ValidateBackOfficeAdminCredentialsAndGetBackOfficeConnections_WorkCompleted(Object sender, RunWorkerCompletedEventArgs e)
        {
            HandleWorkCompleted(e, (result) =>
            {
                var theResult = (result as Tuple<ValidateBackOfficeAdminCredentialsResponse, BackOfficeConnectionsForCredentialsResponse>);
                if (theResult != null)
                {
                    _validateBackOfficeAdminCredentialsResponse = theResult.Item1;
                    _backOfficeConnectionsForCredentialsResponse = theResult.Item2;
                }
            });
        }

        private void _backgroundWorker_ValidateAndUpdateActiveStatusOnAllConnections_DoWork(Object sender, DoWorkEventArgs e)
        {
            try
            {
                ConfigurationViewModelList argument = e.Argument as ConfigurationViewModelList;
                Int32 total = argument.Count * 3;  // we update progress 3 times per model

                // wait until the progress form has been created (so we can update it)
                _progressForm.WaitUntilReady();
                _progressForm.ShowNormalProgressBar(1, 0, total);

                // do the real work
                Int32 count = 1;
                MethodInvoker i = delegate()
                {
                    lock (_lockObject)
                    {
                        if (argument.Count > 1)
                        {
                            _progressForm.UpdateStatus(String.Format("Refreshing connections ({0} of {1})", ((count - 1) / 3) + 1, argument.Count), count++, total);
                        }
                        else
                        {
                            _progressForm.UpdateStatus("Refreshing connection", count++, argument.Count * 3);
                        }
                    }
                };

                var stateService = new BackOfficeServiceManager();
                foreach (ConfigurationViewModel model in argument)
                {
                    try
                    {
                        // step 1 of 3: Validate back office connection
                        if (!HandleCancelOrUpdateProgress(e, 0, i))
                            return;

                        ValidateBackOfficeConnectionResponse backOfficeConnectionResponse = null;
                        backOfficeConnectionResponse = stateService.ValidateBackOfficeConnectionCredentialsAsString(model.ConnectorPluginId,
                                model.BackOfficeConnectionCredentials);
                    

                        string[] backOfficeErrors;
                        BackOfficeConnectivityStatus backOfficeConnectivityStatus =
                            ConnectorUtilities.ProcessValidateBackOfficeConnectionResponse(backOfficeConnectionResponse, out backOfficeErrors);


                        // step 2 of 3:  Validate tenant connection
                        if (!HandleCancelOrUpdateProgress(e, 500, i))
                            return;


                        ValidateTenantConnectionResponse tenantConnectionResponse = null;
                        using (var proxy = TenantValidationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                        {
                            tenantConnectionResponse = proxy.ValidateTenantConnection(model.CloudEndpoint, model.CloudTenantId, model.CloudPremiseKey, String.Empty);
                        }

                        string[] tenantErrors;
                        string cloudCompanyName, cloudCompanyUrl;
                        TenantConnectivityStatus tenantConnectivityStatus =
                            ConnectorUtilities.ProcessValidateTenantConnectionResponse(tenantConnectionResponse, out cloudCompanyName, out cloudCompanyUrl, out tenantErrors);


                        // step 3 of 3:  Update system status
                        if (!HandleCancelOrUpdateProgress(e, 500, i))
                            return;


                        // Update the model based on the cloud response
                        if (!string.IsNullOrEmpty(cloudCompanyName) || !string.IsNullOrEmpty(cloudCompanyUrl))
                        {
                            model.CloudCompanyName = cloudCompanyName;
                            model.CloudCompanyUrl = cloudCompanyUrl;
                        }

                        // Update the state service with the results of our verifications above
                        ConnectorUtilities.UpdateConnectionStatusesInStateService(
                            model.CloudTenantId,
                            backOfficeConnectivityStatus,
                            tenantConnectivityStatus);

                        // Refresh all connection statuses for this config
                        model.RefreshConnectionStatuses();


                        if (!HandleCancelOrUpdateProgress(e, 500, null))
                            return;
                    }
                    catch (Exception)
                    {
                        // Update call above handles all exceptions
                    }
                }


                // give a little more time to fully spin up
                if (!HandleCancelOrUpdateProgress(e, 2000, null))
                    return;
            }
            catch (Exception ex)
            {
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(null, ex.ExceptionAsString());
                }
            }
        }

        private void _backgroundWorker_ValidateAndUpdateActiveStatusOnAllConnections_WorkCompleted(Object sender, RunWorkerCompletedEventArgs e)
        {
            HandleWorkCompleted(e, (result) => { });
        }



        #endregion

        /// <summary>
        /// get the needed admin credentials from the back office
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="pluginId"></param>
        /// <returns></returns>
        public ManagementCredentialsNeededResponse AdminCredentialsNeeded(String caption, 
            String pluginId)
        {
            InitializeProgressForm(caption);

            InitializeBackgroundWorker(
                _backgroundWorker_ManagementCredentialsNeeded_DoWork,
                _backgroundWorker_ManagementCredentialsNeeded_WorkCompleted,
                new Tuple<String>(pluginId));

            ShowProgressDialog();

            return _managementCredentialsNeededResponse;
        }


        private void _backgroundWorker_ManagementCredentialsNeeded_DoWork(Object sender, DoWorkEventArgs e)
        {
            try
            {
                Tuple<String> argument = e.Argument as Tuple<String>;

                // wait until the progress form has been created (so we can update it)
                _progressForm.WaitUntilReady();
                _progressForm.ShowMarqueeProgressBar();

                // do the real work
                var stateSerivce = new BackOfficeServiceManager();
                e.Result = stateSerivce.GetManagementCredentialsNeeded(argument.Item1);
  
                // give a little more time to fully spin up
                if (!HandleCancelOrUpdateProgress(e, 2000, null))
                    return;
            }
            catch (Exception ex)
            {
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(null, ex.ExceptionAsString());
                }
            }
        }

        private void _backgroundWorker_ManagementCredentialsNeeded_WorkCompleted(Object sender, RunWorkerCompletedEventArgs e)
        {
            HandleWorkCompleted(e, (result) => { _managementCredentialsNeededResponse = (result as ManagementCredentialsNeededResponse); });
        }

         /// <summary>
        /// get the needed connection credentials from the back office
        /// </summary>
        /// <param name="caption"></param>
        /// <param name="pluginId"></param>
        /// <param name="managementCredentials"></param>
        /// <param name="connectionCredentials"></param>
        /// <returns></returns>
        public ConnectionCredentialsNeededResponse ConnectionCredentialsNeeded(String caption, 
            String pluginId,
            IDictionary<string, string> managementCredentials,
            IDictionary<string, string> connectionCredentials)
        {
            InitializeProgressForm(caption);

            InitializeBackgroundWorker(
                _backgroundWorker_ConnectionCredentialsNeeded_DoWork,
                _backgroundWorker_ConnectionCredentialsNeeded_WorkCompleted,
                new Tuple<String, IDictionary<string, string>, IDictionary<string, string>>(pluginId, managementCredentials, connectionCredentials));

            ShowProgressDialog();

            return _connectionCredentialsNeededResponse;
        }


        private void _backgroundWorker_ConnectionCredentialsNeeded_DoWork(Object sender, DoWorkEventArgs e)
        {
            try
            {
                Tuple<String, IDictionary<string, string>, IDictionary<string, string>> argument = e.Argument as Tuple<String, IDictionary<string, string>,IDictionary<string, string>>;

                // wait until the progress form has been created (so we can update it)
                _progressForm.WaitUntilReady();
                _progressForm.ShowMarqueeProgressBar();

                // do the real work
                var stateService = new BackOfficeServiceManager();
                e.Result = stateService.GetConnectionCredentialsNeeded(argument.Item1, argument.Item2, argument.Item3);
                

                // give a little more time to fully spin up
                if (!HandleCancelOrUpdateProgress(e, 2000, null))
                    return;
            }
            catch (Exception ex)
            {
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(null, ex.ExceptionAsString());
                }
            }
        }

        private void _backgroundWorker_ConnectionCredentialsNeeded_WorkCompleted(Object sender, RunWorkerCompletedEventArgs e)
        {
            HandleWorkCompleted(e, (result) => { _connectionCredentialsNeededResponse = (result as ConnectionCredentialsNeededResponse); });
        }
        

        #region Private fields
        private readonly IWin32Window _owner;
        private readonly Object _lockObject = new Object();
        private BackgroundWorker _backgroundWorker;
        private ProgressForm _progressForm;
        private Boolean _userCancelled;
        private ValidateTenantConnectionResponse _validateTenantConnectionResponse;
        private ValidateBackOfficeConnectionResponse _validateBackOfficeConnectionResponse;
        private BackOfficeConnectionsForCredentialsResponse _backOfficeConnectionsForCredentialsResponse;
        private ValidateBackOfficeAdminCredentialsResponse _validateBackOfficeAdminCredentialsResponse;
        private BackOfficePluginsResponse _backOfficePluginsResponse;
        private ManagementCredentialsNeededResponse _managementCredentialsNeededResponse;
        private ConnectionCredentialsNeededResponse _connectionCredentialsNeededResponse;
        private FeatureResponse _featureResponse;

        //TODO: JSB wow this pattern needs to be revised when we get time. Needs to be more of an async with direct response.

        #endregion

        internal FeatureResponse FeatureRequest(string caption, string backOfficeId, string backOfficeCredentials, string featureId, string tenantId, string payload)
        {
            InitializeProgressForm(caption);

            InitializeBackgroundWorker(
                _backgroundWorker_FeatureRequest_DoWork,
                _backgroundWorker_FeatureRequest_WorkCompleted,
                new Tuple<String, String, String, String, String>(backOfficeId, backOfficeCredentials, featureId, tenantId, payload));

            ShowProgressDialog();

            return _featureResponse;
        }

        private void _backgroundWorker_FeatureRequest_WorkCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            HandleWorkCompleted(e, (result) => { _featureResponse = (result as FeatureResponse); });
        }

        private void _backgroundWorker_FeatureRequest_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Tuple<String, String, String, String, String> argument = e.Argument as Tuple<String, String, String, String, String>;

                // wait until the progress form has been created (so we can update it)
                _progressForm.WaitUntilReady();
                _progressForm.ShowMarqueeProgressBar();

                // do the real work
                var stateService = new FeatureServiceManager();
                e.Result = stateService.GetFeatureResponse(
                        argument.Item1,
                        argument.Item2,
                        argument.Item3,
                        argument.Item4,
                        argument.Item5);
                

                // give a little more time to fully spin up
                if (!HandleCancelOrUpdateProgress(e, 2000, null))
                    return;
            }
            catch (Exception ex)
            {
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(null, ex.ExceptionAsString());
                }
            }
        }
    }
}