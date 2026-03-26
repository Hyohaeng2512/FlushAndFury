# COMBAT FORMULA

## Resolve Order (chu thu tu)

1. Base Damage (theo hand)
2. Card Buff Layer (Enchant / Tag / Seal effect cua la bai)
3. Relic Layer (trigger theo hanh dong)
4. Boon Layer (buff qua man)
5. x Multipliers (% bonus, vulnerability, weakness...)
6. Damage Type Conversion (neu co)
7. Defense Layer (Resist/Immune/Phase rule)
8. Block Absorb
9. Clamp/Round -> Final Damage

------------------------------------------------------------------------

## Suggested Naming

- Buff qua man: **Boon**
- Ten day du trong UI/doc: **Map Boon**
- Ly do: ngan, de nho, dung phong cach roguelike, tach ro voi Relic

------------------------------------------------------------------------

## Damage Formula

CardAdjustedDamage = applyCardBuff(BaseDamage)

RelicAdjustedDamage = applyRelic(CardAdjustedDamage)

BoonAdjustedDamage = applyBoon(RelicAdjustedDamage)

RawDamage = BoonAdjustedDamage x Multipliers

AfterDefense = applyDefenseRules(RawDamage)

FinalDamageTaken = max(0, floor(AfterDefense - TargetBlock))

------------------------------------------------------------------------

## Block System

- Block giam damage nhan vao theo tung hit
- Block bi tru truoc HP
- HP chi mat khi damage vuot qua Block
- De xai ban dau: Block reset ve 0 tai TurnStart(Player)

------------------------------------------------------------------------

## Example

Straight: Base = 14

Enchant +3, Relic +2, Multiplier x1.2

RawDamage = (14 + 3 + 2) x 1.2 = 22.8

AfterDefense = 22.8

TargetBlock = 6

FinalDamageTaken = max(0, floor(22.8 - 6)) = 16

------------------------------------------------------------------------

## Strength Scaling

- Physical: +2 damage per stack
- Magic: +1 damage + effect scaling

------------------------------------------------------------------------

## Notes

- Moi tinh toan dung cung mot seed/random stream cho combat
- Neu co reflect/thorns: xu ly sau khi FinalDamageTaken da duoc xac dinh
- Rule uu tien bonus theo yeu cau hien tai: Card Buff -> Relic -> Boon
