using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Sage.Connector.DomainContracts.BackOffice;
using Sage.Connector.DomainContracts.Responses;
using Sage.Connector.DomainMediator.Core.Utilities;

namespace Sage.Connector.DomainMediator.Core
{
    /// <summary>
    /// Abstract Class for the Domain Mediators.   
    /// Designed to simplify make implementation of the individual feature request. 
    /// </summary>
    public abstract class AbstractDomainMediator : IDomainFeatureRequest, IDisposable
    {
        /// <summary>
        /// Addin 
        /// </summary>
        protected string ProcessExecutionPath
        {
            get
            {
                return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); 
            }
        }

        /// <summary>
        /// Gets the feature data storage path.
        /// </summary>
        /// <returns></returns>
        protected string GetFeatureDataStoragePath()
        {
            //TODO add guard for empty?
            return _dataStoragePath;
        }

        /// <summary>
        /// Gets the synchronize data storage path.
        /// </summary>
        /// <returns></returns>
        protected string GetSyncDataStoragePath()
        {
            //TODO add guard for empty?
            return _dataStoragePath;
        }

        /// <summary>
        /// Setups the data file path.
        /// </summary>
        /// <param name="path">The path.</param>
        protected void SetupDataStoragePath(string path)
        {
            _dataStoragePath = path;
        }

        private string _dataStoragePath;

        /// <summary>
        /// Track whether Dispose has been called. 
        /// </summary>
        protected bool Disposed = false;

        /// <summary>
        /// The <see cref="CompositionContainer"/>
        /// </summary>
        protected CompositionContainer Container;

        private const String PropertyValuesStorageIdFormat = "{0}_{1}";


        /// <summary>
        /// Base constructor
        /// </summary>
        protected AbstractDomainMediator()
        {
        }

        /// <summary>
        /// Creates the composition container.
        /// </summary>
        /// <param name="relativePath">The backoffice identifier.</param>
        protected void CreateCompositionContainer(string relativePath = null)
        {
            //TODO merge with other version of this
            string pluginPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (!string.IsNullOrWhiteSpace(relativePath))
            {
                pluginPath = Path.Combine(pluginPath, relativePath);
            }
            var safeCatalog = new SafeDirectoryCatalog(pluginPath);
            
            Container = new CompositionContainer(safeCatalog.Catalog);

            //Fill the imports of this object
            try
            {
                Container.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                Debug.Print(compositionException.ToString());
            }
        }

        /// <summary>
        /// Creates the composition container.
        /// </summary>
        /// <param name="relativePaths">The relative paths.</param>
        /// <exception cref="System.NullReferenceException">relativePaths</exception>
        protected void CreateCompositionContainer(IEnumerable<string> relativePaths)
        {
            if(relativePaths == null) throw new NullReferenceException("relativePaths");

            string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            List<string> targetPaths = new List<string>();
            foreach (string path in relativePaths)
            {
                if (!string.IsNullOrWhiteSpace(path))
                {
                    string pluginPath;
                    if (path == GetBasePartialPath())
                    {
                        pluginPath = basePath;
                    }
                    else
                    {
                        pluginPath = Path.Combine(basePath, path);
                    }
                    
                    targetPaths.Add(pluginPath);
                }
            }
            
            var safeCatalog = new SafeDirectoryCatalog(targetPaths);

            Container = new CompositionContainer(safeCatalog.Catalog);

            //Fill the imports of this object
            try
            {
                Container.ComposeParts(this);
            }
            catch (CompositionException compositionException)
            {
                Debug.Print(compositionException.ToString());
            }
        }


        /// <summary>
        /// Gets the discovery plugins partial path.
        /// </summary>
        /// <returns></returns>
        protected string GetDiscoveryPluginsPartialPath()
        {
            return _discoveryPluginsPartialPath; 
        }
        //NOTE: the folder name and path is knowledge that is shared with the auto update module.
        //To mitigate auto update issues this is not drawn from a common dll.
        //This may want to be revisited at some point and passed thru process execution.
        private const string _discoveryPluginsRelativeFolderPath = "..";
        private const string _discoveryPluginsFolderName = "DiscoveryPlugins";
        private readonly string _discoveryPluginsPartialPath = Path.Combine(_discoveryPluginsRelativeFolderPath, _discoveryPluginsFolderName);

        /// <summary>
        /// Gets the base partial path.
        /// </summary>
        /// <returns></returns>
        protected string GetBasePartialPath()
        {
            return @"\.";
        }


        /// <summary>
        /// Gets the back office plugin partial path.
        /// </summary>
        /// <param name="backOfficeId">The back office identifier.</param>
        /// <returns></returns>
        protected string GetBackOfficePluginPartialPath(string backOfficeId)
        {
            string path = Path.Combine(_backOfficePluginsPartialPath, backOfficeId);
            return path;
        }
        //NOTE: the folder name and path is knowledge that is shared with the auto update module.
        //To mitigate auto update issues this is not drawn from a common dll. This may want to be revisited at some point.
        //This may want to be revisited at some point and passed thru process execution.
        private const string _backOfficePluginsRelativeFolderPath = "..";
        private const string _backOfficePluginsFolderName = "BackOfficePlugins";
        private readonly string _backOfficePluginsPartialPath = Path.Combine(_backOfficePluginsRelativeFolderPath, _backOfficePluginsFolderName);

        /// <summary>
        /// Begin Session 
        /// </summary>
        /// <param name="sessionContext">The <see cref="ISessionContext"/></param>
        /// <param name="sessionHandler">Back Office Session Handler</param>
        /// <param name="companyConfig">Back Office Company Configuration</param>
        /// <param name="response">Response</param>
        /// <exception cref="ApplicationException"></exception>
        protected void BeginBackOfficeSession(ISessionContext sessionContext,IBackOfficeSessionHandler sessionHandler, 
            IBackOfficeCompanyConfiguration companyConfig, Response response)
        {
            //Begin back office session with configuration in order to make back office connections.
            var clientResponse = sessionHandler.BeginSession(sessionContext, BackOfficeCompanyData.FromConfiguration(companyConfig));
            CheckResponse(clientResponse, response);
        }
        /// <summary>
        /// Check the response and throw and error for bad status. 
        /// </summary>
        /// <param name="clientResponse"></param>
        /// <param name="featureResopnse"></param>
        /// <exception cref="ApplicationException"></exception>
        protected void CheckResponse(Response clientResponse, Response featureResopnse)
        {
            if (clientResponse == null || clientResponse.Status.Equals(Status.Failure))
            {
                featureResopnse.Status = Status.Failure;
                featureResopnse.Diagnoses = clientResponse != null
                    ? clientResponse.Diagnoses
                    : null;

                throw new ApplicationException(String.Format("{0} Failure Response", GetFeatureName()));
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="backofficeFeatureProcessor"></param>
        /// <param name="tenantId"></param>
        /// <param name="response"><see cref="Response"/></param>
        /// <typeparam name="T"></typeparam>
        protected void InitializeFeaturePropertyDefaults<T>(T backofficeFeatureProcessor, string tenantId, Response response) where T : class
        {
            var backOfficeFeaturePropertyHandler = backofficeFeatureProcessor as IBackOfficeFeaturePropertyHandler;
            CheckResponse(backOfficeFeaturePropertyHandler == null
                ? new Response { Status = Status.Success }
                : backOfficeFeaturePropertyHandler.Initialize(GetDefaultPropertyValuesReadOnlyDictionary(tenantId)), response);
        }

        /// <summary>
        /// Get the External id property name.  If null throw an error
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        protected string GetExternalIdPropertyName(Type type)
        {
            string idPropertyName = type.GetExternalIdPropertyName();
            if (idPropertyName == null)
            {
                Debug.Print("Missing External ID attribute");
                throw new ApplicationException("Missing External ID attribute");
            }
            return idPropertyName;
        }
  
        /// <summary>
        /// Get the ReadOnly version of the Default Property Values Storage Dictionary
        /// </summary>
        /// <param name="tenantId">for the tenant</param>
        /// <returns>read only dictionary</returns>
        protected IDictionary<string, object> GetDefaultPropertyValuesReadOnlyDictionary(string tenantId)
        {

            if (StorageDictionary<string, object>.Exists(GetFeatureDataStoragePath(), String.Format(PropertyValuesStorageIdFormat, tenantId, GetFeatureName())))
            {
                var storageDictionary = GetDefaultPropertyValuesDictionary(tenantId, StorageMode.ReadOnly);
                KeyValuePair<String, Object>[] storageCopy = new KeyValuePair<string, object>[storageDictionary.Count];
                storageDictionary.CopyTo(storageCopy, 0);
                var featurePairs = storageCopy.ToDictionary(x => x.Key, y => y.Value);
                var readonlyDictionary = new ReadOnlyDictionary<string, object>(featurePairs);
                return readonlyDictionary;
            }

            return (new ReadOnlyDictionary<string, object>(new Dictionary<string, object>()));

        }


        /// <summary>
        /// get the Storage dictionary associated with the property values for the tenant for **this** feature.
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="mode"><see cref="StorageMode"/></param>
        /// <returns></returns>
        protected StorageDictionary<string, object> GetDefaultPropertyValuesDictionary(string tenantId, StorageMode mode)
        {

            return GetDefaultPropertyValuesDictionary(tenantId, GetFeatureName(), mode);
        }

    

        /// <summary>
        /// Get the Storage Dictionary for the tenant and feature.
        /// </summary>
        /// <param name="tenantId">Tenant used as part of storage Id</param>
        /// <param name="featureName">Feature Name which is part of storage Id</param>
        /// <param name="mode"><see cref="StorageMode"/></param>
        /// <returns></returns>
        protected StorageDictionary<string, object> GetDefaultPropertyValuesDictionary(string tenantId, string featureName, StorageMode mode)
        {

            string storageId = String.Format(PropertyValuesStorageIdFormat, tenantId, featureName);

            var storageDicationary = new StorageDictionary<string, object>(GetFeatureDataStoragePath(), storageId, mode);
            return storageDicationary;
        }
        /// <summary>
        /// Get **this** feature name from the metadata attribute
        /// </summary>
        /// <returns></returns>
        protected string GetFeatureName()
        {
            return GetFeatureName(GetType());
        }

        /// <summary>
        /// Get **this** feature name from the metadata attribute
        /// </summary>
        /// <returns></returns>
        protected string GetFeatureName(Type featureProcessorType)
        {

            var featureAttr = featureProcessorType.GetCustomAttributes(typeof(FeatureMetadataExportAttribute)).First() as FeatureMetadataExportAttribute;
            Debug.Assert(featureAttr != null);
            Debug.Print(featureAttr.Name);
            return featureAttr.Name;
        }
        /// <summary>
        /// Set response status and diagnosis information for an error exception
        /// </summary>
        /// <param name="response"><see cref="Response"/></param>
        /// <param name="ex"><see cref="Exception"/></param>
        protected void ProcessException(Response response, Exception ex)
        {
            //TODO KMS: RESEARCH access to feature metadata from this abstract
            response.Status = Status.Failure;
            response.Diagnoses.Add(new Diagnosis
            {
                Severity = Severity.Error,
                UserFacingMessage = ex.Message,
                RawMessage = ex.Message + ex.StackTrace

            });
        }

        /// <summary>
        /// Dispose properly of this instance.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method. 
            // Therefore, you should call GC.SupressFinalize to 
            // take this object off the finalization queue 
            // and prevent finalization code for this object 
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose(bool disposing) executes in two distinct scenarios. 
        /// If disposing equals true, the method has been called directly 
        /// or indirectly by a user's code. Managed and unmanaged resources 
        /// can be disposed. 
        /// If disposing equals false, the method has been called by the 
        /// runtime from inside the finalizer and you should not reference 
        /// other objects. Only unmanaged resources can be disposed. 
        /// </summary>
        /// <param name="disposing">When false, the method has been called by the 
        /// runtime from inside the finalizer and you should not reference 
        /// other objects. Only unmanaged resources can be disposed. 
        ///  </param>
        protected virtual void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called. 
            if (!Disposed)
            {
                // If disposing equals true, dispose all managed 
                // and unmanaged resources. 
                if (disposing)
                {
                    // Dispose managed resources.
                    Container.Dispose();
                }

                // Note disposing has been done.
                Disposed = true;

            }
        }

        /// <summary>
        /// Feature Request implementation
        /// </summary>
        /// <param name="processContext">The <see cref="IProcessContext"/></param>
        /// <param name="requestId">The Connector Request Id</param>
        /// <param name="tenantId">The tenant Id making the request.</param>
        /// <param name="backOfficeCompanyConfiguration">The back office company configuration <see cref="IBackOfficeCompanyConfiguration"/></param>
        /// <param name="payload">String representing the payload to process.</param>
        public abstract void FeatureRequest(IProcessContext processContext, Guid requestId, string tenantId,
            IBackOfficeCompanyConfiguration backOfficeCompanyConfiguration, string payload);


        /// <summary>
        /// Remove the storage dictionary 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="identifier"></param>
        protected void RemoveStorageDictionary(string path, string identifier)
        {
            StorageDictionary<String, Object>.RemoveStorage(path, identifier);
        }
    }
}
