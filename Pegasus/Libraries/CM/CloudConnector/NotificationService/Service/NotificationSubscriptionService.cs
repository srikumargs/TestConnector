using System.ServiceModel;
using Sage.Connector.NotificationService.Interfaces;
using Sage.ServiceModel;

namespace Sage.Connector.NotificationService
{
    /// <summary>
    /// 
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple, ConfigurationName = "NotificationSubscriptionService")]
    public sealed class NotificationSubscriptionService : SubscriptionManager<INotificationCallback>, INotificationSubscriptionService
    { }
}
