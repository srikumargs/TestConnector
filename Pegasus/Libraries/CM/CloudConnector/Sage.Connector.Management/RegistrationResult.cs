using System;
using Sage.Connector.Cloud.Integration.Interfaces.WebAPI;
using Sage.Connector.MessagingService.Interfaces;

namespace Sage.Connector.Management
{
    /// <summary>
    /// 
    /// </summary>
    public class RegistrationResult
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationResult"/> class.
        /// </summary>
        public RegistrationResult()
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationResult"/> class.
        /// </summary>
        /// <param name="registration">The registration.</param>
        public RegistrationResult(TenantRegistrationWithErrorInfo registration)
        {
            if (null == registration)
            {
                Successful = false;
                RegistrationError = "Registration failed.";
                return;
            }

            this.Successful = registration.Succeeded;
            this.RegistrationError = registration.ErrorMessage;
            this.SiteAddressBaseUri = registration.SiteAddressBaseUri;
            this.TenantClaim = registration.TenantClaim;
            this.TenantId = registration.TenantId;
            this.TenantKey = registration.TenantKey;
            this.TenantName = registration.TenantName;
            this.TenantUrl = registration.TenantUrl;
            this.Successful = !String.IsNullOrWhiteSpace(TenantClaim);
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="RegistrationResult"/> is successful.
        /// </summary>
        /// <value>
        ///   <c>true</c> if successful; otherwise, <c>false</c>.
        /// </value>
        public bool Successful { get; internal set; }

        /// <summary>
        /// Gets the tenant identifier.
        /// </summary>
        /// <value>
        /// The tenant identifier.
        /// </value>
        public string TenantId { get; internal set; }

        /// <summary>
        /// Gets the tenant key.
        /// </summary>
        /// <value>
        /// The tenant key.
        /// </value>
        public string TenantKey { get; internal set; }
    
        /// <summary>
        /// Gets the tenant claim.
        /// </summary>
        /// <value>
        /// The tenant claim.
        /// </value>
        public string TenantClaim { get; internal set; }
        
        /// <summary>
        /// Gets the name of the tenant.
        /// </summary>
        /// <value>
        /// The name of the tenant.
        /// </value>
        public string TenantName { get; internal set; }

        /// <summary>
        /// Gets or sets the tenant URL.
        /// </summary>
        /// <value>
        /// The tenant URL.
        /// </value>
        public Uri TenantUrl { get; internal set; }

        /// <summary>
        /// Gets the site address base URI.
        /// </summary>
        /// <value>
        /// The site address base URI.
        /// </value>
        public Uri SiteAddressBaseUri { get; internal set; }

        /// <summary>
        /// The error returned from a registration failure
        /// </summary>
        public String RegistrationError { get; internal set; }
    }
}