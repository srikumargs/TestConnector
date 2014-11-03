using System;
using System.Diagnostics;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Description;
using Sage.Connector.Cloud.Integration.Interfaces.DataContracts;
using Sage.Connector.Cloud.Integration.Interfaces.EndpointBehaviors;
using Sage.Connector.Cloud.Integration.Interfaces.MessageInspectors;

namespace Sage.Connector.ConnectorServiceCommon
{
    /// <summary>
    /// Helper class for collaboration with cloud
    /// </summary>
    public static class CloudUtils
    {
        static CloudUtils()
        {
            // Since our DEV SSL certificate won't validate properly, we must explicitly let it through in order to prevent
            // "System.Net.WebException: The underlying connection was closed: Could not establish trust relationship for the SSL/TLS secure channel."
            // This is needed because our private cloud will fail SSL cert verification otherwise.
            String allowDevSslCerts = Environment.GetEnvironmentVariable("SAGE_CONNECTOR_ALLOW_DEV_SSL_CERTS", EnvironmentVariableTarget.Machine);
            if (!String.IsNullOrEmpty(allowDevSslCerts) && allowDevSslCerts == "1")
            {
                ServicePointManager.ServerCertificateValidationCallback += new RemoteCertificateValidationCallback(ServerCertificateValidationCallback);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public static void SetupServerCertificateValidationCallback()
        {
            // do nothing but cause the static constructor to JIT and run
        }

        /// <summary>
        /// Creates the binding used for cloud communication
        /// Note: creates appropriate security for binding based on what URL
        /// The client is asking for (http vs https)
        /// </summary>
        /// <param name="serviceUri"></param>
        /// <returns></returns>
        public static WSHttpBinding CreateCloudBinding(Uri serviceUri)
        {
            // Set up defaults
            WSHttpBinding binding = new WSHttpBinding();
            // TODO: magic numbers which require evaluation!  be advised these may also be reflected in Cloud code as well
            binding.MaxReceivedMessageSize = 5000000;
            binding.MaxBufferPoolSize = 5000000;
            //binding.MaxBufferSize = 5000000;
            binding.ReaderQuotas.MaxStringContentLength = 5000000;
            binding.ReaderQuotas.MaxArrayLength = 5000000;
            binding.ReaderQuotas.MaxBytesPerRead = 5000000;
            binding.ReaderQuotas.MaxDepth = 5000000;
            binding.ReaderQuotas.MaxNameTableCharCount = 5000000;

            // Set up security
            if (serviceUri.Scheme.ToLowerInvariant() == "https")
            {
                // Asking for secure channel
                binding.Security.Mode = SecurityMode.Transport;
            }
            else
            {
                // Security off
                binding.Security.Mode = SecurityMode.None;
            }

            return binding;
        }

        /// <summary>
        /// Create the endpoint behavior that sets the appropriate headers
        /// </summary>
        /// <param name="tenantId"></param>
        /// <param name="premiseKey"></param>
        /// <param name="premiseAgent"></param>
        /// <param name="logger"></param>
        /// <returns></returns>
        public static IEndpointBehavior CreateHeaderEndpointBehavior(
            String tenantId, 
            String premiseKey,
            PremiseAgent premiseAgent,
            MessageLogger logger)
        {
            // Create the endpoint behavior for message hashing, logging
            // Note: pass in a null logger if your message will be considered noise
            // For example, the get requests message
            SetHttpRequestHeaderClientBehavior behavior = new SetHttpRequestHeaderClientBehavior(
                tenantId, premiseKey, premiseAgent, logger);

            return behavior;
        }

        /// <summary>
        /// Validates the received server cert
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="cert"></param>
        /// <param name="chain"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        private static bool ServerCertificateValidationCallback(object sender, X509Certificate cert, X509Chain chain, System.Net.Security.SslPolicyErrors error)
        {
            if (cert != null)
            {
                Trace.WriteLine(String.Format("{0}.ServerCertificateValidationCallback(): {1}", typeof(CloudUtils).FullName, cert.Subject));
            }

            //// Since our DEV SSL certificate won't validate properly, we must explicitly let it through in order to prevent
            //// "System.Net.WebException: The underlying connection was closed: Could not establish trust relationship for the SSL/TLS secure channel."
            //// This is needed because our private cloud will fail SSL cert verification otherwise.
            return cert.Subject.Contains("CN=127.0.0.1") ||
                cert.Subject.Contains("CN=www.sageconstructionanywhere.com") ||
                cert.Subject.Contains("CN=app.sageconstructionanywhere.com") ||
                cert.Subject.Contains("CN=demo.sageconstructionanywhere.com") ||
                cert.Subject.Contains("CN=beta.sageconstructionanywhere.com") || 
                cert.Subject.Contains("CN=trade.sageconstructionanywhere.com") || 
                cert.Subject.Contains("CN=support.sageconstructionanywhere.com") || 
                cert.Subject.Contains("CN=*.cloudapp.net") || 
                cert.Subject.Contains("CN=*.blob.core.windows.net");
        }
    }
}

