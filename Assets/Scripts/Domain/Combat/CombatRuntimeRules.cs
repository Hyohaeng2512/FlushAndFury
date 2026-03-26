namespace FlushAndFury.Domain.Combat
{
    public enum HandPattern
    {
        HighCard = 0,
        Pair = 1,
        ThreeKind = 2,
        Straight = 3,
        Flush = 4,
        FullHouse = 5,
        FourKind = 6,
        StraightFlush = 7,
    }

    public enum EnemyDefenseRule
    {
        None = 0,
        BasicBrute = 1,
        ShieldGuardian = 2,
        MirrorBeast = 3,
        PhaseShifter = 4,
        CardThief = 5,
        BurnDemon = 6,
    }

    public enum EnemyPhaseMask
    {
        AcceptAll = 0,
        PhysicalOnly = 1,
        MagicOnly = 2,
    }

    public static class CombatModifierIds
    {
        public const string TagWarrior = "TAG_WARRIOR";
        public const string TagArcane = "TAG_ARCANE";
        public const string TagLucky = "TAG_LUCKY";

        public const string EnchantFlat3 = "ENCHANT_FLAT_3";
        public const string EnchantEnergy1 = "ENCHANT_ENERGY_1";
        public const string EnchantBurn2 = "ENCHANT_BURN_2";
        public const string EnchantConvertToMagic = "ENCHANT_CONVERT_TO_MAGIC";

        public const string SealReturn = "SEAL_RETURN";
        public const string SealGoldOnKill = "SEAL_GOLD_ON_KILL";
        public const string SealTopDeck = "SEAL_TOP_DECK";

        public const string RelicWarriorEmblem = "RELIC_WARRIOR_EMBLEM";
        public const string RelicArcaneCore = "RELIC_ARCANE_CORE";
        public const string RelicBloodPact = "RELIC_BLOOD_PACT";
        public const string RelicGamblersCoin = "RELIC_GAMBLERS_COIN";
        public const string RelicPerfectFlow = "RELIC_PERFECT_FLOW";
    }
}
