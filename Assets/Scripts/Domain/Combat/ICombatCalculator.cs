namespace FlushAndFury.Domain.Combat
{
    public interface ICombatCalculator
    {
        DamageContext Resolve(DamageContext context);
    }
}
