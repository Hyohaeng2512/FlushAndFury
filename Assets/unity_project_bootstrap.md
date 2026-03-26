# UNITY PROJECT BOOTSTRAP - FLUSH & FURY

Tai lieu nay de ban tao project Unity moi va copy bo khung code (skeleton) DI + combat pipeline vao de chay tiep.

## 1) Yeu cau

- Unity 2022 LTS (khuyen nghi 2022.3.x)
- Package Manager ho tro Git URL
- VContainer package

## 2) Cai VContainer

Trong Unity:

- Window -> Package Manager
- Add package from git URL...
- Dan URL (mot trong hai):

```text
https://github.com/hadashiA/VContainer.git?path=VContainer/Assets/VContainer
```

Neu team ban co mirror/noi bo thi dung URL noi bo tuong ung.

## 3) Cau truc thu muc can tao

Tao dung tree sau trong `Assets/Scripts`:

```text
Assets/
  Scripts/
    README_ARCHITECTURE.md

    Core/
      Bootstrap/
        GameEntryPoint.cs
      DI/
        ProjectLifetimeScope.cs
        BattleLifetimeScope.cs

    Domain/
      Combat/
        DamageContext.cs
        CardBuffContext.cs
        RelicContext.cs
        BoonContext.cs
        IDamageStep.cs
        ICombatCalculator.cs

    Application/
      Combat/
        CombatActionCommand.cs
        ResolveCombatActionUseCase.cs

    Infrastructure/
      Combat/
        CombatCalculator.cs
        Steps/
          CardBuffStep.cs
          RelicStep.cs
          BoonStep.cs
          DefenseStep.cs
          BlockStep.cs
          FinalizeStep.cs
      Random/
        IRngService.cs
        SeededRngService.cs
      Events/
        IEventBus.cs
        EventBus.cs

    Presentation/
      Battle/
        BattlePresenter.cs

    Config/
      Cards/
        CardModifierDefinition.cs
      Relics/
        RelicDefinition.cs
      Boons/
        MapBoonDefinition.cs
```

## 4) Thu tu execute da chot

Combat pipeline:

1. Card Buff Layer
2. Relic Layer
3. Boon Layer
4. Defense Layer
5. Block Layer
6. Finalize Layer

Rule uu tien bonus: `Card Buff -> Relic -> Boon`.

## 5) Scene setup toi thieu de chay

Trong scene test:

1. Tao 1 GameObject: `ProjectScope`
2. Gan component `ProjectLifetimeScope`
3. Tao prefab co component `BattleLifetimeScope`
4. Keo prefab `BattleLifetimeScope` vao field `battleLifetimeScopePrefab` trong `ProjectLifetimeScope`
5. Tao 1 GameObject: `EntryPoint`
6. Gan component `GameEntryPoint`

Luc nay dependency se duoc inject theo scope.

## 6) Check nhanh sau khi import

- Khong con compile error ve namespace
- Khong con missing package `VContainer`
- `ProjectLifetimeScope` va `BattleLifetimeScope` resolve duoc service

## 7) Mapping sang game that (de mo rong)

- Card modifier logic -> `CardBuffStep`
- Relic trigger logic -> `RelicStep`
- Buff qua man (Map Boon) -> `BoonStep`
- Defend/Block logic -> `DefenseStep` + `BlockStep`
- Clamp, round, final damage event -> `FinalizeStep`

## 8) Ghi chu quan trong

- Day la bo khung, chua co business logic.
- Nen giu Domain it phu thuoc Unity de de test.
- Khi co them mechanic moi, uu tien them step/interface thay vi chen if/else vao 1 class lon.
