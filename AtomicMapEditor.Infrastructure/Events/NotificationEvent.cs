using Prism.Events;

namespace Ame.Infrastructure.Events
{
    public class NotificationEvent<T> : PubSubEvent<NotificationMessage<T>>
    {
    }
}
