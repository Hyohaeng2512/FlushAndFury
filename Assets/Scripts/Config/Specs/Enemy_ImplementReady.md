# Enemy - Implement Ready Spec v1

## 1) Scope

Spec nay map mechanic enemy vao `DefenseStep` (va mot phan event side-effect) de dam bao combat deterministic.

## 2) Runtime Data Contract

## Enemy runtime model

- `enemyId: string`
- `enemyType: enum` (Basic, Mechanic, Chaos, Boss)
- `defenseRuleId: string`
- `currentPhaseMask: enum` (AcceptAll, PhysicalOnly, MagicOnly)
- `hitsTakenThisTurn: int`
- `reflectThreshold: int`
- `isBurning: bool`

## DefenseStep input

- `context.CurrentDamage`
- `context.DamageType`
- enemy runtime state o tren

## DefenseStep output

- `context.IsImmune`
- `context.DefenseMultiplier`
- publish side effects (reflect, buff, discard penalty)

## 3) Global Priority Rules (Defense Layer)

1. Immune/phase check
2. Threshold gate check
3. Resist/Vulnerable multiplier
4. Side-effects (reflect, buff)

## 4) Enemy Catalog v1

## ENEMY_BASIC_BRUTE

- Rule: khong co immunity dac biet
- Effect: `DefenseMultiplier = 1.0`

## ENEMY_SHIELD_GUARDIAN

- Rule: chi nhan damage neu bi hit >=2 lan/turn
- Condition: `hitsTakenThisTurn + 1 < 2`
- Effect: `IsImmune = true`
- Neu dat nguong: `IsImmune = false`

## ENEMY_MIRROR_BEAST

- Rule: neu sat thuong truoc block >= `reflectThreshold` thi phan sat thuong
- Default threshold v1: 15
- Effect: publish event `ReflectDamageToPlayer(floor(CurrentDamage * 0.5))`

## ENEMY_PHASE_SHIFTER

- Rule: moi turn chi nhan 1 loai damage theo `currentPhaseMask`
- `PhysicalOnly` + input Magic -> `IsImmune = true`
- `MagicOnly` + input Physical -> `IsImmune = true`

## ENEMY_CARD_THIEF

- Rule: sau khi player discard, enemy kich hoat buff
- Effect: publish event `EnemyGainBuff(AttackUp, +X)`
- Khong sua damage truc tiep trong DefenseStep

## ENEMY_BURN_DEMON

- Rule: neu player khong tan cong turn nay -> tang damage enemy turn sau
- Effect: publish event `EnemyChargeIfPlayerSkippedAttack`

## 5) Integration Notes voi Pipeline

- DefenseStep chi xu ly sat thuong dang vao muc tieu hien tai.
- Side effects duoc dua qua EventBus, khong chen logic UI vao step.
- BlockStep van chay sau DefenseStep theo dung pipeline.

## 6) Determinism Rules

- Khong dung `Time.time` hay random global trong DefenseStep.
- Neu can random (vi du target random), lay tu `IRngService` da seed.
- Moi turn reset state can reset: `hitsTakenThisTurn`, `currentPhaseMask` (neu rule bat buoc).

## 7) Test Cases

## TC-ENEMY-01 (Shield Guardian)

- Given: hitsTakenThisTurn=0
- When: incoming hit thu nhat
- Then: `IsImmune=true`, final damage = 0

## TC-ENEMY-02 (Phase Shifter)

- Given: phase=`PhysicalOnly`, incoming type=`Magic`
- When: resolve DefenseStep
- Then: `IsImmune=true`

## TC-ENEMY-03 (Mirror Beast)

- Given: CurrentDamage=20, threshold=15
- When: resolve DefenseStep
- Then: event `ReflectDamageToPlayer(10)` duoc publish
