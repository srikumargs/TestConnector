using System.Threading;
using System.Diagnostics;
using Sage.Connector.DomainContracts.BackOffice;
using Sage.Connector.DomainContracts.Data;
using System;
using System.ComponentModel.Composition;
using System.Resources;

namespace Sage.Connector.DomainMediator.Core
{
    /// <summary>
    /// The Domain feature request interface.
    /// </summary>
    public interface IDomainFeatureRequest
    {
        /// <summary>
        /// The required information in order to process a domain's feature
        /// </summary>
        /// <param name="context">The <see cref="IProcessContext"/></param>
        /// <param name="requestId">The request Id</param>
        /// <param name="tenantId">The tenant Id making the request.</param>
        /// <param name="backOfficeCompanyConfiguration">The Back office company configuration <see cref="IBackOfficeCompanyConfiguration"/></param>
        /// <param name="payload">The request payload</param>
        void FeatureRequest(IProcessContext context, Guid requestId, String tenantId, IBackOfficeCompanyConfiguration backOfficeCompanyConfiguration, String payload);
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IProcessContext
    {
        /// <summary>
        /// 
        /// </summary>
        IResponseHandler ResponseHandler { get; }

        /// <summary>
        /// 
        /// </summary>
        ILogging PluginLogger { get; }

        /// <summary>
        ///
        /// </summary>
        CancellationToken CancellationToken { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        ISessionContext GetSessionContext();

        /// <summary>
        /// 
        /// </summary>
        uint EntityChunkCount { get; }



        //Add tracking id, tenant id and request id to process context to help with activity tracing.
        //Design wise we have a tension with these being here. In one way it would be good to pass these
        //as part of a structure thru the whole chain.On the other we are currently single request per context
        //and these really are "ambient" properties for the whole chain as long as we are single.

        /// <summary>
        /// Gets or sets the tracking identifier.
        /// </summary>
        /// <value>
        /// The tracking identifier.
        /// </value>
        Guid TrackingId { get; set; }

        /// <summary>
        /// Gets or sets the tenant identifier.
        /// </summary>
        /// <value>
        /// The tenant identifier.
        /// </value>
        String TenantId { get; set; }

        /// <summary>
        /// Gets or sets the request identifier.
        /// </summary>
        /// <value>
        /// The request identifier.
        /// </value>
        Guid RequestId { get; set; }

        /// <summary>
        /// Tracks the plugin invoke.
        /// </summary>
        void TrackPluginInvoke();

        /// <summary>
        /// Tracks the plugin complete.
        /// </summary>
        void TrackPluginComplete();

    }

    /// <summary>
    /// The Domain Mediator's Feature data
    /// </summary>
    public interface IFeatureMetaData
    {
        /// <summary>
        /// The Feature to be processed by the domain mediator.
        /// </summary>
        String Name { get; }
        /// <summary>
        /// 
        /// </summary>
        String DisplayName { get; }
        /// <summary>
        /// 
        /// </summary>
        String InterfaceName { get; }


    }

    /// <summary>
    /// Feature metadata export attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false), MetadataAttribute]
    public class FeatureMetadataExportAttribute : ExportAttribute, IFeatureMetaData
    {



        /// <summary>
        /// The name of the feature to be processed by the domain mediator.
        /// </summary>
        public String Name
        {
            get; private set;
        }

        /// <summary>
        /// The display name of the feature to be used by configuration
        /// </summary>
        public String DisplayName { get; private set; }
        
        /// <summary>
        /// Name of the Interface supported by the feature. 
        /// </summary>
        public String InterfaceName { get; private set; }

        /// <summary>
        /// The constructor for the set of feature attributes
        /// </summary>
        /// <param name="name"></param>
        /// <param name="featureDescType"></param>
        /// <param name="interfaceName"></param>
        public FeatureMetadataExportAttribute(string name, Type featureDescType, string interfaceName)
            : base(typeof(IFeatureMetaData))
        {
            Name = name;
            try
            {
                DisplayName = new ResourceManager(featureDescType).GetString(name);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                DisplayName = name;
            }

            InterfaceName = interfaceName; 
        }

        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="displayName"></param>
        /// <param name="interfaceName"></param>
        public FeatureMetadataExportAttribute(string name, string displayName, string interfaceName)
            : base(typeof (IFeatureMetaData))
        {
            Name = name;
            DisplayName = displayName;
            InterfaceName = interfaceName; 


        }

    }

}
   
