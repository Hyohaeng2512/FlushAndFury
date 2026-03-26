namespace FlushAndFury.Domain.Combat
{
    public interface IDamageStep
    {
        int Order { get; }
        void Execute(DamageContext context);
    }
}
