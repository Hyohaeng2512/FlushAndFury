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
