# Flush & Fury - Unity Sample Architecture

## Folder Layout

- `Assets/Scripts/Core`: bootstrap, DI scopes, app entry.
- `Assets/Scripts/Domain`: pure gameplay contracts/models (no Unity dependency where possible).
- `Assets/Scripts/Application`: use cases and orchestration for gameplay flows.
- `Assets/Scripts/Infrastructure`: concrete implementations (calculator, RNG, event bus).
- `Assets/Scripts/Presentation`: MonoBehaviour presenters/controllers.
- `Assets/Scripts/Config`: ScriptableObject definitions for data-driven content.

## Combat Priority (current)

1. Card Buff Layer
2. Relic Layer
3. Boon Layer
4. Defense/Block/Finalize

## Suggested Installers

- `ProjectLifetimeScope`: global services (RNG, event bus, shared systems).
- `BattleLifetimeScope`: battle-local services (combat calculator, use cases).
