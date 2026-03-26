# Data-Driven Setup (ScriptableObject)

Sau khi update code, card/relic/enemy rules co the duoc dieu chinh bang ScriptableObject.

## 1) Tao assets

- Card rules:
  - `Create > FlushAndFury > Config > Card Rule`
  - `Create > FlushAndFury > Config > Card Rule Database`
- Relic rules:
  - `Create > FlushAndFury > Config > Relic Rule`
  - `Create > FlushAndFury > Config > Relic Rule Database`
- Enemy intent:
  - `Create > FlushAndFury > Config > Enemy Intent Profile`

Hoac dung auto-generate:

- `Tools > FlushAndFury > Generate Combat Data Assets`
- Script se tao full bo asset mau trong `Assets/GameData/Combat`.

## 2) Gan vao ProjectScope

Trong `ProjectLifetimeScope`:

- `cardRuleDatabase`
- `relicRuleDatabase`
- `enemyIntentProfile`

Neu chua gan database, code se fallback ve hardcode rule cu.

## 3) Rule mapping

- Card:
  - `CardRuleDefinition.Id` phai trung id runtime trong command (`PlayedCardEnchants/Tags/Seals`)
- Relic:
  - `RelicRuleDefinition.Id` phai trung id runtime trong `ActiveRelicIds`
- Enemy:
  - `EnemyIntentProfile.options` la bang telegraph/execute intent theo weight

## 4) Luu y balance

- Priority trong database se quyet dinh thu tu xu ly trong cung layer.
- Event side effects duoc gui qua EventBus nhu truoc.
