# Card Modifier - Implement Ready Spec v1

## 1) Scope

Spec nay mo ta cach map `Enchant / Tag / Seal` vao combat pipeline layer `CardBuff`.

## 2) Runtime Data Contract

## Card runtime model

- `cardId: string`
- `rank: int` (1..13)
- `suit: enum` (Spade, Heart, Club, Diamond)
- `damageType: enum` (Physical, Magic)
- `tags: List<string>`
- `enchantId: string | null`
- `sealId: string | null`

## Hand resolve input

- `playedCards: List<CardRuntimeModel>`
- `handPattern: enum` (HighCard, Pair, ThreeKind, Straight, Flush, FullHouse, FourKind, StraightFlush)
- `discardCountThisTurn: int`

## CardBuffStep output to DamageContext

- `context.CardBuff.FlatDamageBonus`
- `context.CardBuff.DamageMultiplier`
- `context.DamageType` (co the bi convert boi enchant)

## 3) Rule Order Trong Card Layer

1. Ap dung Enchant theo `priority` tang dan.
2. Tong hop Tag bonus.
3. Ap dung Seal effect ngay truoc khi roi Card layer.

Tie-break rule: cung priority -> `createdOrder` tang dan.

## 4) Enchant Catalog v1

## ENCHANT_FLAT_3

- Trigger: `OnCardBuffLayer`
- Condition: card co enchant nay va card nam trong `playedCards`
- Effect: `FlatDamageBonus += 3`
- Priority: `100`

## ENCHANT_ENERGY_1

- Trigger: `OnCardBuffLayer`
- Condition: nhu tren
- Effect: khong tac dong damage, publish event `EnergyGainRequested(+1)`
- Priority: `110`

## ENCHANT_BURN_2

- Trigger: `OnCardBuffLayer`
- Condition: nhu tren
- Effect: publish event `ApplyStatusToTarget(Burn, 2)`
- Priority: `120`

## ENCHANT_CONVERT_TO_MAGIC

- Trigger: `OnCardBuffLayer`
- Condition: nhu tren
- Effect: `context.DamageType = Magic`
- Priority: `130`

## 5) Tag Catalog v1

Tag khong tu buff so truc tiep neu khong co rule support.

## TAG_WARRIOR

- Trigger: `OnCardBuffLayer`
- Condition: hand co >= 1 card tag Warrior
- Effect: `FlatDamageBonus += 1`
- Priority: `200`

## TAG_ARCANE

- Trigger: `OnCardBuffLayer`
- Condition: hand co >= 1 card tag Arcane va `DamageType == Magic`
- Effect: `DamageMultiplier *= 1.1`
- Priority: `210`

## TAG_LUCKY

- Trigger: `OnCardBuffLayer`
- Condition: hand co >= 1 card tag Lucky
- Effect: publish event `LuckyProcCheckRequested`
- Priority: `220`

## 6) Seal Catalog v1

Seal chu yeu tac dong deck/reward, chi bridge event trong combat.

## SEAL_RETURN

- Trigger: `OnCardBuffLayerEnd`
- Condition: card co seal nay
- Effect: publish event `CardReturnToDeckRequested(cardId)`
- Priority: `300`

## SEAL_GOLD_ON_KILL

- Trigger: `OnCardBuffLayerEnd`
- Condition: card co seal nay
- Effect: publish event `GoldOnKillFlagRequested(cardId)`
- Priority: `310`

## SEAL_TOP_DECK

- Trigger: `OnCardBuffLayerEnd`
- Condition: card co seal nay
- Effect: publish event `CardPlaceTopDeckRequested(cardId)`
- Priority: `320`

## 7) Validation Rules

- Moi card toi da 1 enchant.
- Moi card toi da 1 seal.
- Tag la tap hop, khong trung lap.
- Card data invalid -> skip effect va log warning, khong crash.

## 8) Test Cases (Given/When/Then)

## TC-CARD-01

- Given: BaseDamage=10, hand co 1 card `ENCHANT_FLAT_3`
- When: resolve CardBuff layer
- Then: CurrentDamage=13

## TC-CARD-02

- Given: BaseDamage=10, hand co tag Arcane, damageType Magic
- When: resolve CardBuff layer
- Then: CurrentDamage=11 (x1.1)

## TC-CARD-03

- Given: card co `ENCHANT_CONVERT_TO_MAGIC`
- When: resolve CardBuff layer
- Then: `DamageType == Magic`
