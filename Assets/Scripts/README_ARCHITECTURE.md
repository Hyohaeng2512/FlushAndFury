# Flush & Fury - Scripts Architecture

## Layers

- `Core`: Entry point and scene composition.
- `Domain`: Combat contracts and shared models.
- `Application`: Use cases for gameplay flows.
- `Infrastructure`: Concrete implementations (calculator, RNG, event bus).
- `Presentation`: MonoBehaviour presenters/controllers.
- `Config`: ScriptableObject definitions.

## Combat Pipeline (Demo)

1. Card Buff Step
2. Relic Step
3. Boon Step
4. Defense Step
5. Mitigation Step
6. Finalize Step
