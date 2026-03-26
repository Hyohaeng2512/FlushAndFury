# Implement-Ready Specs

Tai lieu nay chot rule de implement truc tiep vao pipeline combat hien tai.

## File map

- `Assets/Scripts/Config/Specs/CardModifier_ImplementReady.md`
- `Assets/Scripts/Config/Specs/Relic_ImplementReady.md`
- `Assets/Scripts/Config/Specs/Enemy_ImplementReady.md`

## Boundaries

- Khong doi thu tu pipeline da chot: `CardBuff -> Relic -> Boon -> Defense -> Block -> Finalize`.
- Rule trong spec uu tien deterministic (khong phu thuoc thu tu object trong scene).
- Neu cung layer va cung priority, xu ly theo `createdOrder` tang dan.
