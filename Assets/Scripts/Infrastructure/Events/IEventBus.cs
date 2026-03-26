using System;

namespace FlushAndFury.Infrastructure.Events
{
    public interface IEventBus
    {
        void Publish<TEvent>(TEvent eventData);
        void Subscribe<TEvent>(Action<TEvent> listener);
        void Unsubscribe<TEvent>(Action<TEvent> listener);
    }
}
