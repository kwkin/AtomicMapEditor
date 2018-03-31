using Prism.Events;

namespace AtomicMapEditor.Infrastructure.Events
{
    public class NotificationEvent<T> : PubSubEvent<NotificationMessage<T>>
    {
    }
}
