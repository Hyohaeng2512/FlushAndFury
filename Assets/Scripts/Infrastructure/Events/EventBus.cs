using System;
using System.Collections.Generic;

namespace FlushAndFury.Infrastructure.Events
{
    public sealed class EventBus : IEventBus
    {
        private readonly Dictionary<Type, Delegate> listeners = new Dictionary<Type, Delegate>();

        public void Publish<TEvent>(TEvent eventData)
        {
            Type eventType = typeof(TEvent);
            if (!listeners.TryGetValue(eventType, out Delegate listener))
            {
                return;
            }

            if (listener is Action<TEvent> action)
            {
                action.Invoke(eventData);
            }
        }

        public void Subscribe<TEvent>(Action<TEvent> listener)
        {
            Type eventType = typeof(TEvent);

            if (listeners.TryGetValue(eventType, out Delegate existing))
            {
                listeners[eventType] = Delegate.Combine(existing, listener);
                return;
            }

            listeners[eventType] = listener;
        }

        public void Unsubscribe<TEvent>(Action<TEvent> listener)
        {
            Type eventType = typeof(TEvent);
            if (!listeners.TryGetValue(eventType, out Delegate existing))
            {
                return;
            }

            Delegate updated = Delegate.Remove(existing, listener);
            if (updated == null)
            {
                listeners.Remove(eventType);
                return;
            }

            listeners[eventType] = updated;
        }
    }
}
