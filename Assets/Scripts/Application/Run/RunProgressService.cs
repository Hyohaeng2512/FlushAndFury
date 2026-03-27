using FlushAndFury.Infrastructure.Events;
using UnityEngine;

namespace FlushAndFury.Application.Run
{
    public sealed class RunProgressService
    {
        private readonly IEventBus eventBus;

        public int MaxHp { get; private set; }
        public int CurrentHp { get; private set; }
        public int Gold { get; private set; }

        public RunProgressService(IEventBus bus)
        {
            eventBus = bus;
            MaxHp = 80;
            CurrentHp = 80;
            Gold = 0;
            eventBus?.Publish(new RunStateChanged(MaxHp, CurrentHp, Gold, "Initialized"));
        }

        public void SetCurrentHp(int hp, string reason)
        {
            CurrentHp = Mathf.Clamp(hp, 0, MaxHp);
            eventBus?.Publish(new RunStateChanged(MaxHp, CurrentHp, Gold, reason));
        }

        public void AddGold(int amount, string reason)
        {
            Gold = Mathf.Max(0, Gold + amount);
            eventBus?.Publish(new RunStateChanged(MaxHp, CurrentHp, Gold, reason));
        }
    }
}
