# VISUAL & ANIMATION DESIGN - FLUSH & FURY

## Core Direction

Chosen Style: Card → Empower Player → Attack

------------------------------------------------------------------------

## Design Goal

-   Tạo cảm giác "power fantasy"
-   Mỗi hand = 1 spectacle nhỏ
-   Phân biệt rõ Physical vs Magic
-   Feedback mạnh, nhanh, rõ

------------------------------------------------------------------------

## Animation Flow

### 1. Select Phase

-   Highlight các lá được chọn
-   Màu theo type:
    -   🔴 đỏ = physical
    -   🔵 xanh = magic
-   Lá rung nhẹ + glow

------------------------------------------------------------------------

### 2. Charge Phase

-   Lá bài bay về phía nhân vật
-   Xoay nhẹ trong không khí
-   Tạo particle effect
-   Âm thanh "charge"

------------------------------------------------------------------------

### 3. Conversion Phase

#### 🔴 Physical

-   Lá bài hấp thụ vào vũ khí
-   Vũ khí phát sáng đỏ
-   Scale nhẹ (tăng kích thước)

#### 🔵 Magic

-   Lá bài tan thành năng lượng
-   Tụ lại thành orb / spell
-   Hover trước khi bắn

------------------------------------------------------------------------

### 4. Attack Phase

#### Physical

-   Nhân vật chém / đập
-   Hit mạnh, có hit stop

#### Magic

-   Bắn spell / tia năng lượng
-   Có trail effect

------------------------------------------------------------------------

### 5. Impact Phase

-   Screen shake nhẹ
-   Hit stop (0.05--0.1s)
-   Damage number pop
-   Particle nổ

------------------------------------------------------------------------

### 6. Combo Feedback

Hiển thị text lớn: - "PAIR" - "STRAIGHT" - "FLUSH"

Có thể kèm: - âm thanh riêng - animation riêng

------------------------------------------------------------------------

## Hand Identity

-   Pair: nhanh, 2 hit
-   Three of a kind: combo 3 hit
-   Straight: chuỗi tấn công liên tục
-   Flush: AoE / lan

------------------------------------------------------------------------

## Relic Synergy Visual

Relic sẽ thay đổi hiệu ứng:

Ví dụ: - Fire relic → thêm lửa - Lightning relic → thêm điện - Poison
relic → hiệu ứng độc

------------------------------------------------------------------------

## Timing Rule

-   1 action: 1--2 giây tối đa
-   Không làm animation quá dài
-   Ưu tiên tốc độ + cảm giác "đã"

------------------------------------------------------------------------

## Design Principles

-   Rõ ràng (clarity)
-   Phản hồi mạnh (impact)
-   Không rườm rà (clean)
-   Dễ mở rộng (modular VFX)
