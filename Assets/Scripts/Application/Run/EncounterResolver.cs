using FlushAndFury.Domain.Run;

namespace FlushAndFury.Application.Run
{
    public interface IEncounterResolver
    {
        EncounterResolution Resolve(MapNodeType nodeType);
    }

    public readonly struct EncounterResolution
    {
        public readonly bool RequiresCombat;
        public readonly bool AutoResolved;
        public readonly string Reason;

        public EncounterResolution(bool requiresCombat, bool autoResolved, string reason)
        {
            RequiresCombat = requiresCombat;
            AutoResolved = autoResolved;
            Reason = reason;
        }
    }

    public sealed class EncounterResolver : IEncounterResolver
    {
        public EncounterResolution Resolve(MapNodeType nodeType)
        {
            switch (nodeType)
            {
                case MapNodeType.Combat:
                case MapNodeType.Elite:
                case MapNodeType.Boss:
                    return new EncounterResolution(true, false, "Combat");
                case MapNodeType.Start:
                case MapNodeType.Shop:
                case MapNodeType.Event:
                    return new EncounterResolution(false, true, "AutoResolved");
                default:
                    return new EncounterResolution(false, true, "DefaultResolved");
            }
        }
    }
}
