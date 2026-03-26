# Relic - Implement Ready Spec v1

## 1) Scope

Spec nay dinh nghia trigger, condition, effect cho relic trong layer `RelicStep`.

## 2) Runtime Data Contract

## Player relic runtime model

- `relicId: string`
- `stack: int`
- `createdOrder: int`
- `isActive: bool`

## Combat context input cho RelicStep

- `handPattern`
- `playedCardTags`
- `damageType`
- `hpLostThisTurn`
- `discardCountThisTurn`

## RelicStep output

- `context.Relic.FlatDamageBonus`
- `context.Relic.DamageMultiplier`
- publish side effects qua EventBus

## 3) Global Priority Rules

- Layer order da khoa: Relic la sau CardBuff, truoc Boon.
- Trong Relic layer: sort theo `priority` tang dan.
- Tie-break: `createdOrder` tang dan.

## 4) Relic Catalog v1

## RELIC_WARRIOR_EMBLEM

- Trigger: `OnRelicLayer`
- Condition: `handPattern == Pair` va ton tai tag `Warrior`
- Effect: publish event `ApplyStatusToPlayer(Strength, +2, 2 turns)`
- Damage impact truc tiep: none
- Priority: `100`

## RELIC_ARCANE_CORE

- Trigger: `OnRelicLayer`
- Condition: `damageType == Magic`
- Effect: `context.Relic.DamageMultiplier *= 1.3`
- Priority: `110`

## RELIC_BLOOD_PACT

- Trigger: `OnRelicLayer`
- Condition: `hpLostThisTurn > 0`
- Effect: publish event `ApplyStatusToPlayer(Strength, +1, permanentInBattle)`
- Damage impact truc tiep: none
- Priority: `120`

## RELIC_GAMBLERS_COIN

- Trigger: `OnRelicLayer`
- Condition: `discardCountThisTurn > 0`
- Effect: publish event `DealRandomEnemyDamage(5)`
- Damage impact truc tiep: none
- Priority: `130`

## RELIC_PERFECT_FLOW

- Trigger: `OnRelicLayer`
- Condition: `handPattern == Straight`
- Effect: publish event `DrawCardRequested(1)`
- Damage impact truc tiep: none
- Priority: `140`

## 5) Stacking Rule

- 2 relic cung `relicId` khong stack effect mac dinh (v1).
- Neu can stack, mo them field `stackPolicy: None | Additive | Multiplicative` trong data.

## 6) Edge Cases

- Relic bi disable (`isActive=false`) -> bo qua.
- Condition khong dat -> bo qua, khong log error.
- Event publish fail -> log warning, combat van tiep tuc.

## 7) Test Cases

## TC-RELIC-01

- Given: BaseDamage=20, damageType=Magic, co Arcane Core
- When: resolve Relic layer
- Then: CurrentDamage=26

## TC-RELIC-02

- Given: hand Pair co Warrior tag, co Warrior Emblem
- When: resolve Relic layer
- Then: event ApplyStatus Strength+2 duoc publish 1 lan

## TC-RELIC-03

- Given: discardCountThisTurn=2, co Gambler's Coin
- When: resolve Relic layer
- Then: event DealRandomEnemyDamage(5) duoc publish 1 lan
