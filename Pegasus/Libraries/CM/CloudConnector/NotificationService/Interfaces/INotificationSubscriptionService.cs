using System.Net.Security;
using System.ServiceModel;
using Sage.Connector.LinkedSource;
using Sage.SystemModel;

namespace Sage.Connector.NotificationService.Interfaces
{
    /// <summary>
    /// Interface for subscribing to the INotificationServiceCallback events triggered by the NotificationService.
    /// </summary>
    /// <remarks>
    /// Although this interface defines no methods, it is needed so that the appropriate CallbackContract
    /// can be identified.
    /// </remarks>
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(INotificationCallback), Namespace = ServiceConstants.V1_SERVICE_NAMESPACE, ProtectionLevel = ProtectionLevel.EncryptAndSign)]
    public interface INotificationSubscriptionService : ISubscriptionService
    { }
}
