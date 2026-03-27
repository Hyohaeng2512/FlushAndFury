# GAME DESIGN DOCUMENT (GDD) + ENEMY DESIGN - VERSION 0.3

------------------------------------------------------------------------

# 🧠 CORE CONCEPT

Roguelike deckbuilder kết hợp: - Poker (Balatro) - Turn-based combat
(Slay the Spire)

------------------------------------------------------------------------

# 🔁 CORE LOOP

1.  Chọn đường trên map\
2.  Combat\
3.  Nhận reward\
4.  Build deck / relic\
5.  Đánh boss\
6.  Sang map mới → chọn passive

------------------------------------------------------------------------

# 🗺️ MAP SYSTEM

-   Combat\
-   Elite\
-   Shop\
-   Event\
-   Boss\
-   2--3 path

------------------------------------------------------------------------

# ⚔️ COMBAT SYSTEM

## Turn Structure

Player: - 2 Energy\
- Chơi poker hand (tối đa 5 lá)\
- 1 discard (tối đa 3 lá)

Enemy: - Attack / effect

------------------------------------------------------------------------

## Hand System

-   Hand size: 8\
-   Có thể tăng qua relic\
-   Draw lại mỗi turn

------------------------------------------------------------------------

## Damage System

-   🔴 Physical\
-   🔵 Magic

  Type       Đặc điểm
  ---------- ----------------
  Physical   scale strength
  Magic      AoE, effect

------------------------------------------------------------------------

## Defend / Block System

-   Block la lop phong thu tam thoi, giam damage theo tung hit\
-   Cong thuc: FinalDamageTaken = max(0, floor(IncomingDamage - Block))\
-   Damage tru vao Block truoc, phan con lai moi tru HP\
-   Rule v1: Block reset ve 0 tai TurnStart(Player)

### Block Timing

-   Gain Block: xay ra trong player action (card/relic/event)\
-   Consume Block: khi nhan hit\
-   Decay/Reset: tai TurnStart(Player)

### Design Muc Tieu

-   Tang decision depth (tan cong vs phong thu)\
-   Tao dat dien cho relic/passive ve phong thu\
-   Lam ro intent enemy va counterplay

------------------------------------------------------------------------

# 🃏 CARD SYSTEM

## Deck

-   52 lá\
-   Có thể transform / upgrade

------------------------------------------------------------------------

## Modifier

### Enchant

-   +damage\
-   +energy\
-   status

### Tag

-   Warrior\
-   Arcane\
-   Lucky

### Seal

-   Return\
-   Gold\
-   Top deck

------------------------------------------------------------------------

# 🧩 RELIC SYSTEM

-   Core build system\
-   Trigger theo hành động

------------------------------------------------------------------------

# 🏆 BOON SYSTEM (MAP BOON)

Chon sau moi map: - Convert damage\
- Double trigger\
- Special rule

## Execute Priority (Combat)

-   Thu tu uu tien bonus: Card Buff -> Relic -> Boon\
-   Neu cung 1 layer thi sort theo priority (so nho chay truoc)\
-   Neu tiep tuc trung priority thi xu ly theo thu tu tao effect (deterministic)

------------------------------------------------------------------------

# 💰 REWARD

-   Modifier\
-   Relic\
-   Remove

------------------------------------------------------------------------

# 🔧 DECK MANIPULATION

-   Transform\
-   Upgrade\
-   Draw control\
-   Limited remove

------------------------------------------------------------------------

# ⚖️ BALANCE

-   50% Relic\
-   30% Card\
-   20% Passive

------------------------------------------------------------------------

# 👾 ENEMY DESIGN

## Core System

-   HP\
-   Intent\
-   Pattern

------------------------------------------------------------------------

## Enemy Types

### 1. Basic (Stat check)

-   Máu cao\
-   Damage ổn định

------------------------------------------------------------------------

### 2. Mechanic Enemy

#### Shield Guardian

-   Chỉ nhận damage nếu bị hit ≥2 lần/turn

#### Mirror Beast

-   Damage cao → phản lại

#### Phase Shifter

-   Chỉ nhận 🔴 hoặc 🔵 mỗi turn

------------------------------------------------------------------------

### 3. Chaos Enemy

#### Card Thief

-   Remove card trong hand

#### Burn Demon

-   Không đánh → tăng damage

------------------------------------------------------------------------

## Interaction System

### Với Poker

-   Immune Pair\
-   Buff Straight\
-   Nerf Flush

------------------------------------------------------------------------

### Với Tag

-   Weak vs Warrior\
-   Immune Arcane

------------------------------------------------------------------------

### Với Discard

-   Discard → enemy buff / debuff

------------------------------------------------------------------------

# 👑 BOSS DESIGN

## Design Rule

-   Có mechanic riêng\
-   Counter build

------------------------------------------------------------------------

## Examples

### The Judge

-   Spam cùng hand → debuff

### The Gambler

-   Random rule mỗi turn

### The Converter

-   Convert damage type

------------------------------------------------------------------------

# 🎨 VISUAL & ANIMATION (CORE DIRECTION)

## Chosen Style

Card → Empower Player → Attack

------------------------------------------------------------------------

## Design Goal

-   Power fantasy mạnh\
-   Mỗi hand = spectacle ngắn\
-   Phân biệt rõ 🔴 / 🔵\
-   Feedback rõ ràng, nhanh

------------------------------------------------------------------------

## Animation Flow

### 1. Select Phase

-   Highlight lá bài\
-   Glow theo type (🔴 / 🔵)

### 2. Charge Phase

-   Lá bay về nhân vật\
-   Xoay + particle + sound

### 3. Conversion Phase

-   🔴 → hấp thụ vào vũ khí (glow, scale)\
-   🔵 → biến thành năng lượng / spell

### 4. Attack Phase

-   Physical → chém / đập\
-   Magic → bắn spell

### 5. Impact Phase

-   Hit stop\
-   Screen shake nhẹ\
-   Damage number

### 6. Combo Feedback

-   Hiển thị: PAIR / STRAIGHT / FLUSH\
-   Có animation + sound riêng

------------------------------------------------------------------------

## Hand Identity

-   Pair: nhanh, 2 hit\
-   Three: combo 3 hit\
-   Straight: chain attack\
-   Flush: AoE

------------------------------------------------------------------------

## Relic Visual Synergy

-   Fire → hiệu ứng lửa\
-   Lightning → điện\
-   Poison → độc

------------------------------------------------------------------------

## Timing Rule

-   1 action: 1--2s\
-   Ưu tiên nhanh + "đã tay"

------------------------------------------------------------------------

# 🎯 DESIGN GOAL

-   Decision-based gameplay\
-   Combo-driven\
-   Enemy ép adapt\
-   Feedback mạnh, gây nghiện

------------------------------------------------------------------------

# 🧭 RUN PERSISTENCE RULE (UPDATE)

## Player State Giữ Xuyên Map (giống Slay the Spire)

-   Player chỉ giữ 2 chỉ số chính xuyên suốt map/run: `HP` và `Gold`\
-   `HP` không reset sau mỗi combat, chỉ đổi theo damage/heal từ combat/event/relic\
-   `Gold` giữ xuyên map, dùng cho shop/reward flow

## Battle-Only State (reset theo battle/turn)

-   Player/Enemy `Block`, `Defense`, combat status tạm thời là state trong battle\
-   Rule hiện tại giữ nguyên: Block xử lý trong pipeline combat và reset theo turn rule\
-   Enemy chỉ cần state cốt lõi là `HP` (không cần giữ persist giữa node)

------------------------------------------------------------------------

# ✅ IMPLEMENTATION CHECKLIST (CURRENT)

## Core Combat

- [x] Pipeline combat: Card Buff -> Relic -> Boon -> Defense -> Block -> Finalize
- [x] Log theo từng step: CurrentDamage + DamageType
- [x] Final damage floor/clamp theo công thức

## Card / Relic / Enemy Rules

- [x] Card rules v1: Enchant / Tag / Seal + event hook
- [x] Relic rules v1: trigger theo condition + event hook
- [x] Enemy defense rules v1: ShieldGuardian / MirrorBeast / PhaseShifter / CardThief / BurnDemon

## Turn Flow / Intent

- [x] Turn flow state machine: Player -> Enemy -> Next Turn
- [x] Enemy intent telegraph
- [x] Enemy intent consume event (`EnemyIntentConsumed`)
- [x] Enemy action executor tách riêng (`EnemyActionExecutor`)
- [x] Turn snapshot event/log
- [x] Guard rails cho flow và enemy resolve

## Status System

- [x] Status system v1 cho Player + Enemy
- [x] Stack / duration / tick cho Strength, Burn, Vulnerable, Weak
- [x] Hook với `ApplyStatusToPlayerRequested` và `ApplyStatusToTargetRequested`

## Data-Driven Content (SO)

- [x] Card/Relic/Enemy chuyển sang ScriptableObject config
- [x] Database + profile đã nối vào DI scope
- [x] Có tool generate assets: `Tools > FlushAndFury > Generate Combat Data Assets`
- [x] Có fallback hardcode khi chưa gán database/profile

## Pending Next Steps

- [ ] CombatHealthService: HP/Block apply thật cho player/enemy
- [ ] Run-level persistent state service: PlayerHP + Gold xuyên map
- [ ] BattleEnd/Reward flow cập nhật vào run state
- [ ] HUD binding cho intent/status/hp (khi bắt đầu làm UI)
