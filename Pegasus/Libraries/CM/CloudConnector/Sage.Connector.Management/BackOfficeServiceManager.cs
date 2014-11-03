using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sage.Connector.Common;
using Sage.Connector.StateService.Interfaces;
using Sage.Connector.StateService.Interfaces.DataContracts;
using Sage.Connector.StateService.Proxy;
using Newtonsoft.Json;

//TODO Should this have an IDispose? Impact on consumers
namespace Sage.Connector.Management
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// We are on one level implementing the interface
    /// </remarks>
    ///
    public class BackOfficeServiceManager //: IBackOfficeValidationService//, ITenantValidationService, IFeatureService
    {
        ///// <summary>
        ///// Gets the management credentials needed.
        ///// </summary>
        ///// <param name="pluginId">The plugin identifier.</param>
        ///// <param name="cancellationToken">The cancellation token.</param>
        //public async Task<ManagementCredentialsNeededResponse> GetManagementCredentialsNeededAsync(string pluginId, CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        //// wait until the progress form has been created (so we can update it)
        //        //_progressForm.WaitUntilReady();
        //        //_progressForm.ShowMarqueeProgressBar();

        //        ManagementCredentialsNeededResponse response;
        //        // do the real work
        //        using (var proxy = BackOfficeValidationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
        //        {
        //            response = proxy.GetManagementCredentialsNeeded(pluginId);
        //        }

        //        return response;
        //        //// give a little more time to fully spin up
        //        //if (!HandleCancelOrUpdateProgress(e, 2000, null))
        //        //    return;
        //    }
        //    catch (Exception ex)
        //    {
        //        using (var logger = new SimpleTraceLogger())
        //        {
        //            logger.WriteError(null, ex.ExceptionAsString());
        //        }
        //    }
        //}

        /// <summary>
        /// Get the set of available back office plugins
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public BackOfficePluginsResponse GetBackOfficePlugins()
        {
            BackOfficePluginsResponse response = null;
            try
            {
                // do the real work
                using (var proxy = BackOfficeValidationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                {
                    response = proxy.GetBackOfficePlugins();
                }
            }
            catch (Exception ex)
            {
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(null, ex.ExceptionAsString());
                }
            }
            return response;
        }


        //what happens when the test button is pushed.
        ///// <summary>
        ///// </summary>
        ///// <param name="backOfficeId"></param>
        ///// <param name="companyConnectionCredentials"></param>
        ///// <returns></returns>
        ///// <exception cref="System.NotImplementedException"></exception>
        //public ValidateBackOfficeConnectionResponse ValidateBackOfficeConnection(string backOfficeId, IDictionary<string, string> companyConnectionCredentials)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// </summary>
        /// <param name="backOfficeId"></param>
        /// <param name="companyConnectionCredentials"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public ValidateBackOfficeConnectionResponse ValidateBackOfficeConnectionCredentialsAsString(
            string backOfficeId,
            string companyConnectionCredentials)
        {
            ValidateBackOfficeConnectionResponse response = null;
            try
            {
                // do the real work
                using (var proxy = BackOfficeValidationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                {
                    response = proxy.ValidateBackOfficeConnectionCredentialsAsString(backOfficeId, companyConnectionCredentials);
                }
            }
            catch (Exception ex)
            {
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(null, ex.ExceptionAsString());
                }
            }
            return response;
        }

        /// <summary>
        /// </summary>
        /// <param name="backOfficeId"></param>
        /// <param name="credentials"></param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public ValidateBackOfficeAdminCredentialsResponse ValidateBackOfficeAdminCredentials(string backOfficeId, IDictionary<string, string> credentials)
        {
            ValidateBackOfficeAdminCredentialsResponse response = null;
            try
            {
                // do the real work
                using (var proxy = BackOfficeValidationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                {
                    response = proxy.ValidateBackOfficeAdminCredentials(backOfficeId, credentials);
                }
            }
            catch (Exception ex)
            {
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(null, ex.ExceptionAsString());
                }
            }
            return response;
        }

        /// <summary>
        /// Gets the management credentials needed.
        /// </summary>
        /// <param name="pluginId">The plugin identifier.</param>
        /// <returns></returns>
        public ManagementCredentialsNeededResponse GetManagementCredentialsNeeded(string pluginId)
        {
            ManagementCredentialsNeededResponse response = null;
            try
            {
                // do the real work
                using (var proxy = BackOfficeValidationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                {
                    response = proxy.GetManagementCredentialsNeeded(pluginId);
                }
            }
            catch (Exception ex)
            {
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(null, ex.ExceptionAsString());
                }
            }
            return response;
            //TODO Sort out exception or null return. When do we log? here or above?
            //ASYNC? 
            //Cancellation Token?
            //Type management for dependencies
            //hard load issues.?
            //proxy dependencies?
        }

        //                var stateSerivce = new StateServiceWrapper();
                //e.Result = stateSerivce.GetManagementCredentialsNeeded(argument.Item1);

        /// <summary>
        /// Gets the connection credentials needed.
        /// </summary>
        /// <param name="backOfficeId">The back office identifier.</param>
        /// <param name="companyManagementCredentials">The company management credentials.</param>
        /// <param name="companyConnectionCredentials">The company connection credentials.</param>
        /// <returns></returns>
        public ConnectionCredentialsNeededResponse GetConnectionCredentialsNeeded(string backOfficeId, IDictionary<string, string> companyManagementCredentials, IDictionary<string, string> companyConnectionCredentials)
        {
            ConnectionCredentialsNeededResponse response = null;
            try
            {
                // do the real work
                using (var proxy = BackOfficeValidationServiceProxyFactory.CreateFromCatalog("localhost", ConnectorServiceUtils.CatalogServicePortNumber))
                {
                    response = proxy.GetConnectionCredentialsNeeded(backOfficeId, companyManagementCredentials, companyConnectionCredentials);
                }
            }
            catch (Exception ex)
            {
                using (var logger = new SimpleTraceLogger())
                {
                    logger.WriteError(null, ex.ExceptionAsString());
                }
            }
            return response;
        }

        //in ConfigurationHelpers
        ///// <summary>
        ///// Validate the tenant id and premise key as valid.
        ///// </summary>
        ///// <param name="siteAddress"></param>
        ///// <param name="tenantId"></param>
        ///// <param name="premiseId"></param>
        ///// <returns></returns>
        ///// <exception cref="System.NotImplementedException"></exception>
        //public ValidateTenantConnectionResponse ValidateTenantConnection(string siteAddress, string tenantId, string premiseId)
        //{
        //    throw new NotImplementedException();
        //}



        /// <summary>
        /// Strings the string dictionary to json string.
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <returns></returns>
        public static string StringStringDictonaryToJsonString(IDictionary<string, string> dictionary)
        {
            string retval = JsonConvert.SerializeObject(dictionary);
            return retval;
        }

    }
}

