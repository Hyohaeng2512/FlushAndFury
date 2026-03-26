using FlushAndFury.Config.Cards;
using FlushAndFury.Config.Enemies;
using FlushAndFury.Config.Relics;
using FlushAndFury.Domain.Combat;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FlushAndFury.EditorTools
{
    public static class CombatDataAssetGenerator
    {
        private const string RootPath = "Assets/GameData/Combat";
        private const string CardRulesPath = RootPath + "/CardRules";
        private const string RelicRulesPath = RootPath + "/RelicRules";
        private const string EnemyIntentPath = RootPath + "/EnemyIntentProfiles";

        [MenuItem("Tools/FlushAndFury/Generate Combat Data Assets")]
        public static void Generate()
        {
            EnsureFolder("Assets/GameData");
            EnsureFolder(RootPath);
            EnsureFolder(CardRulesPath);
            EnsureFolder(RelicRulesPath);
            EnsureFolder(EnemyIntentPath);

            List<CardRuleDefinition> cardRules = GenerateCardRules();
            List<RelicRuleDefinition> relicRules = GenerateRelicRules();
            GenerateCardRuleDatabase(cardRules);
            GenerateRelicRuleDatabase(relicRules);
            GenerateEnemyIntentProfile();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[CombatDataAssetGenerator] Generated combat data assets in Assets/GameData/Combat.");
        }

        private static List<CardRuleDefinition> GenerateCardRules()
        {
            List<CardRuleDefinition> assets = new List<CardRuleDefinition>
            {
                CreateCardRule("Enchant_Flat3", CombatModifierIds.EnchantFlat3, CardRuleCategory.Enchant, 100, CardRuleEffectType.FlatDamageBonus, 3, 1f, string.Empty),
                CreateCardRule("Enchant_Energy1", CombatModifierIds.EnchantEnergy1, CardRuleCategory.Enchant, 110, CardRuleEffectType.PublishEnergyGain, 1, 1f, string.Empty),
                CreateCardRule("Enchant_Burn2", CombatModifierIds.EnchantBurn2, CardRuleCategory.Enchant, 120, CardRuleEffectType.PublishApplyStatusToTarget, 2, 1f, "Burn"),
                CreateCardRule("Enchant_ConvertToMagic", CombatModifierIds.EnchantConvertToMagic, CardRuleCategory.Enchant, 130, CardRuleEffectType.ConvertToMagic, 0, 1f, string.Empty),
                CreateCardRule("Tag_Warrior", CombatModifierIds.TagWarrior, CardRuleCategory.Tag, 200, CardRuleEffectType.FlatDamageBonus, 1, 1f, string.Empty),
                CreateCardRule("Tag_Arcane", CombatModifierIds.TagArcane, CardRuleCategory.Tag, 210, CardRuleEffectType.DamageMultiplier, 0, 1.1f, string.Empty),
                CreateCardRule("Tag_Lucky", CombatModifierIds.TagLucky, CardRuleCategory.Tag, 220, CardRuleEffectType.PublishLuckyProc, 0, 1f, string.Empty),
                CreateCardRule("Seal_Return", CombatModifierIds.SealReturn, CardRuleCategory.Seal, 300, CardRuleEffectType.PublishCardReturnToDeck, 0, 1f, string.Empty),
                CreateCardRule("Seal_GoldOnKill", CombatModifierIds.SealGoldOnKill, CardRuleCategory.Seal, 310, CardRuleEffectType.PublishGoldOnKillFlag, 0, 1f, string.Empty),
                CreateCardRule("Seal_TopDeck", CombatModifierIds.SealTopDeck, CardRuleCategory.Seal, 320, CardRuleEffectType.PublishCardPlaceTopDeck, 0, 1f, string.Empty),
            };

            return assets;
        }

        private static List<RelicRuleDefinition> GenerateRelicRules()
        {
            List<RelicRuleDefinition> assets = new List<RelicRuleDefinition>
            {
                CreateRelicRule(
                    "Relic_WarriorEmblem",
                    CombatModifierIds.RelicWarriorEmblem,
                    100,
                    RelicRuleActionType.ApplyPlayerStatus,
                    false,
                    false,
                    false,
                    true,
                    HandPattern.Pair,
                    CombatModifierIds.TagWarrior,
                    2,
                    1f,
                    "Strength",
                    2
                ),
                CreateRelicRule(
                    "Relic_ArcaneCore",
                    CombatModifierIds.RelicArcaneCore,
                    110,
                    RelicRuleActionType.ModifyDamageMultiplier,
                    true,
                    false,
                    false,
                    false,
                    HandPattern.HighCard,
                    string.Empty,
                    0,
                    1.3f,
                    string.Empty,
                    1
                ),
                CreateRelicRule(
                    "Relic_BloodPact",
                    CombatModifierIds.RelicBloodPact,
                    120,
                    RelicRuleActionType.ApplyPlayerStatus,
                    false,
                    true,
                    false,
                    false,
                    HandPattern.HighCard,
                    string.Empty,
                    1,
                    1f,
                    "Strength",
                    -1
                ),
                CreateRelicRule(
                    "Relic_GamblersCoin",
                    CombatModifierIds.RelicGamblersCoin,
                    130,
                    RelicRuleActionType.DealRandomEnemyDamage,
                    false,
                    false,
                    true,
                    false,
                    HandPattern.HighCard,
                    string.Empty,
                    5,
                    1f,
                    string.Empty,
                    1
                ),
                CreateRelicRule(
                    "Relic_PerfectFlow",
                    CombatModifierIds.RelicPerfectFlow,
                    140,
                    RelicRuleActionType.DrawCard,
                    false,
                    false,
                    false,
                    true,
                    HandPattern.Straight,
                    string.Empty,
                    1,
                    1f,
                    string.Empty,
                    1
                ),
            };

            return assets;
        }

        private static void GenerateCardRuleDatabase(List<CardRuleDefinition> rules)
        {
            string path = CardRulesPath + "/CardRuleDatabase.asset";
            CardRuleDatabase database = AssetDatabase.LoadAssetAtPath<CardRuleDatabase>(path);
            if (database == null)
            {
                database = ScriptableObject.CreateInstance<CardRuleDatabase>();
                AssetDatabase.CreateAsset(database, path);
            }

            SerializedObject so = new SerializedObject(database);
            SerializedProperty list = so.FindProperty("rules");
            list.arraySize = rules.Count;
            for (int i = 0; i < rules.Count; i++)
            {
                list.GetArrayElementAtIndex(i).objectReferenceValue = rules[i];
            }

            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(database);
        }

        private static void GenerateRelicRuleDatabase(List<RelicRuleDefinition> rules)
        {
            string path = RelicRulesPath + "/RelicRuleDatabase.asset";
            RelicRuleDatabase database = AssetDatabase.LoadAssetAtPath<RelicRuleDatabase>(path);
            if (database == null)
            {
                database = ScriptableObject.CreateInstance<RelicRuleDatabase>();
                AssetDatabase.CreateAsset(database, path);
            }

            SerializedObject so = new SerializedObject(database);
            SerializedProperty list = so.FindProperty("rules");
            list.arraySize = rules.Count;
            for (int i = 0; i < rules.Count; i++)
            {
                list.GetArrayElementAtIndex(i).objectReferenceValue = rules[i];
            }

            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(database);
        }

        private static void GenerateEnemyIntentProfile()
        {
            string path = EnemyIntentPath + "/DefaultEnemyIntentProfile.asset";
            EnemyIntentProfile profile = AssetDatabase.LoadAssetAtPath<EnemyIntentProfile>(path);
            if (profile == null)
            {
                profile = ScriptableObject.CreateInstance<EnemyIntentProfile>();
                AssetDatabase.CreateAsset(profile, path);
            }

            SerializedObject so = new SerializedObject(profile);
            SerializedProperty options = so.FindProperty("options");
            options.arraySize = 3;

            SetEnemyOption(options.GetArrayElementAtIndex(0), EnemyIntentType.Attack, 7, 10, 3, "Direct attack");
            SetEnemyOption(options.GetArrayElementAtIndex(1), EnemyIntentType.Defend, 6, 6, 2, "Gain block");
            SetEnemyOption(options.GetArrayElementAtIndex(2), EnemyIntentType.Debuff, 1, 1, 1, "Apply weak");

            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(profile);
        }

        private static CardRuleDefinition CreateCardRule(
            string assetName,
            string id,
            CardRuleCategory category,
            int priority,
            CardRuleEffectType effectType,
            int intValue,
            float floatValue,
            string stringValue
        )
        {
            string path = $"{CardRulesPath}/{assetName}.asset";
            CardRuleDefinition asset = AssetDatabase.LoadAssetAtPath<CardRuleDefinition>(path);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<CardRuleDefinition>();
                AssetDatabase.CreateAsset(asset, path);
            }

            SerializedObject so = new SerializedObject(asset);
            so.FindProperty("id").stringValue = id;
            so.FindProperty("category").enumValueIndex = (int)category;
            so.FindProperty("priority").intValue = priority;
            so.FindProperty("effectType").enumValueIndex = (int)effectType;
            so.FindProperty("intValue").intValue = intValue;
            so.FindProperty("floatValue").floatValue = floatValue;
            so.FindProperty("stringValue").stringValue = stringValue;
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(asset);

            return asset;
        }

        private static RelicRuleDefinition CreateRelicRule(
            string assetName,
            string id,
            int priority,
            RelicRuleActionType actionType,
            bool requireMagicDamage,
            bool requireHpLostThisTurn,
            bool requireDiscardThisTurn,
            bool useHandPattern,
            HandPattern requiredHandPattern,
            string requiredTagId,
            int intValue,
            float floatValue,
            string stringValue,
            int durationTurns
        )
        {
            string path = $"{RelicRulesPath}/{assetName}.asset";
            RelicRuleDefinition asset = AssetDatabase.LoadAssetAtPath<RelicRuleDefinition>(path);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<RelicRuleDefinition>();
                AssetDatabase.CreateAsset(asset, path);
            }

            SerializedObject so = new SerializedObject(asset);
            so.FindProperty("id").stringValue = id;
            so.FindProperty("priority").intValue = priority;
            so.FindProperty("actionType").enumValueIndex = (int)actionType;
            so.FindProperty("requireMagicDamage").boolValue = requireMagicDamage;
            so.FindProperty("requireHpLostThisTurn").boolValue = requireHpLostThisTurn;
            so.FindProperty("requireDiscardThisTurn").boolValue = requireDiscardThisTurn;
            so.FindProperty("useHandPattern").boolValue = useHandPattern;
            so.FindProperty("requiredHandPattern").enumValueIndex = (int)requiredHandPattern;
            so.FindProperty("requiredTagId").stringValue = requiredTagId;
            so.FindProperty("intValue").intValue = intValue;
            so.FindProperty("floatValue").floatValue = floatValue;
            so.FindProperty("stringValue").stringValue = stringValue;
            so.FindProperty("durationTurns").intValue = durationTurns;
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(asset);

            return asset;
        }

        private static void SetEnemyOption(SerializedProperty option, EnemyIntentType intentType, int minValue, int maxValue, int weight, string description)
        {
            option.FindPropertyRelative("intentType").enumValueIndex = (int)intentType;
            option.FindPropertyRelative("minValue").intValue = minValue;
            option.FindPropertyRelative("maxValue").intValue = maxValue;
            option.FindPropertyRelative("weight").intValue = weight;
            option.FindPropertyRelative("description").stringValue = description;
        }

        private static void EnsureFolder(string fullPath)
        {
            if (AssetDatabase.IsValidFolder(fullPath))
            {
                return;
            }

            int slash = fullPath.LastIndexOf('/');
            string parent = fullPath.Substring(0, slash);
            string child = fullPath.Substring(slash + 1);
            if (!AssetDatabase.IsValidFolder(parent))
            {
                EnsureFolder(parent);
            }

            AssetDatabase.CreateFolder(parent, child);
        }
    }
}
