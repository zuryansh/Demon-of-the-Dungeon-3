# Demon of the Dungeon 3

A top-down dungeon crawler with procedurally generated levels, built solo in Unity (C#).

This is the **third attempt** at an idea I first tried in 9th grade and again in 12th grade. Both earlier attempts were abandoned — not from losing interest, but because I correctly identified that my architecture and Unity skills weren't yet sufficient to execute what I wanted: real-time combat state management and procedural generation both stopped me cold. This version is the first to get meaningfully past both walls.

<!-- GIF: full gameplay loop — movement, combat, a generated dungeon being explored -->
<!-- ![Gameplay](docs/gifs/gameplay.gif) -->

---

## Contents

- **Procedural dungeon generation** — `RoomGenerator` runs three interchangeable algorithms (random walk, random noise, Perlin noise) with automatic connectivity validation and retry-on-failure; `RoomAssembler` then lays the generated rooms out into a full dungeon using weighted anchor-based placement and collision-checked positioning.
- **A combat system shared between the player and every enemy type** — one `AttackRuntime`/`Effect`/`Hitbox` pipeline drives both, instead of separate player and enemy combat code.
- **Composition-based enemy AI** — enemy behavior is assembled from interchangeable movement/attack modules rather than baked into per-enemy classes.
- **Data-driven content** — weapons, attacks, and enemies are authored as `ScriptableObject` assets, not hardcoded values, so design iteration doesn't require touching code.

<!-- GIF: procedural generation running — a few rooms being placed and connected -->
<!-- ![Procedural generation](docs/gifs/procgen.gif) -->

---

## Architecture highlights

### A decoupled attack pipeline, built for the player, reused by everything else
`AttackRuntime` (tracks an attack's progress through its animation), `Effect` (a polymorphic, serializable strategy for what an attack *does* — damage, knockback, particles), and `Hitbox` (overlap detection that ignores repeat hits per swing) were built to solve player combat alone. They turned out to be decoupled enough that `EnemyAttackModule` reuses all three directly for every enemy, with no rewrite. (More on how this happened in the section below — it wasn't planned.)

### Composition over inheritance for enemies
`EnemyBrain` doesn't implement movement or attack behavior itself — it orchestrates a pluggable `EnemyMovementModule` and `EnemyAttackModule` (e.g. `Chase`, `ProximityAttack`), found via `GetComponent` at startup and wired together. Adding a new enemy archetype means composing existing modules on a new prefab, not writing a new monolithic enemy class.

```csharp
movementModule = GetComponent<EnemyMovementModule>();
attackModule = GetComponent<EnemyAttackModule>();
movementModule.Brain = this;
attackModule.Brain = this;
```

### Data-driven combat and enemies via ScriptableObjects
`AttackData`, `WeaponData`, and `EnemySO` hold all per-attack and per-enemy tuning (timing windows, animations, effects) as assets. `Effect` subclasses (`DamageEffect`, `KnockBackEffect`, `SpawnParticlesEffect`) are stored polymorphically in a list via `[SerializeReference, SubclassSelector]` — Unity can't natively serialize a `List<AbstractClass>`, so this uses a community extension to make effects pluggable and composable per-attack directly in the Inspector, instead of a hardcoded enum-and-switch.

### RoomGenerator and RoomAssembler — where most of the actual effort went
These two scripts took the longest to get right, and most of what makes the procedural generation feel deliberate rather than random lives here.

**`RoomGenerator`** produces a single room's tile layout and is built around three completely interchangeable algorithms behind one dispatch point — random walk, random noise, and Perlin noise — so swapping or A/B-testing generation styles never touches anything downstream. It doesn't trust its own output: every generated room is flood-filled from its start position and checked against the total floor tile count, and anything that isn't fully connected gets thrown out and regenerated with a new seed, in a loop, until it passes.

**`RoomAssembler`** takes a batch of rooms from the generator and actually lays out the dungeon — and this is the part with the most moving pieces. Room placement is anchor-based: each new room picks one or more already-placed "anchor" rooms (weighted by `WeightedChoice`, so the number of anchors per room follows a tunable distribution rather than a fixed rule), averages their positions, and searches outward from that average for a valid, non-overlapping spot using bounding-box intersection tests. If a spot can't be found after a capped number of attempts, it backs off and logs which method failed via `[CallerMemberName]` rather than just silently giving up. The first two rooms are placed linearly as a guaranteed-connected backbone before the anchor-based placement takes over for the rest, so the dungeon always has a sane starting structure to build outward from.

Both scripts also have actual editor tooling built around them (`[FoldoutGroup]`, `[Button("Start Assembly")]`, etc.) rather than bare `[SerializeField]`s, so I could iterate on generation parameters without leaving the Inspector or writing throwaway debug scripts each time.

<!-- GIF: RoomAssembler placing rooms one at a time, anchors and connections visible -->
<!-- ![Room assembly](docs/gifs/assembler.gif) -->

---

## How that decoupled pipeline actually happened

It wasn't designed for reuse. I was stuck on the player's attack system specifically, for about a week, just trying to get it to work at all. I went back and forth on how to structure attack progress, timing windows, and effects before landing on what's now `AttackRuntime` and `Effect`. By the time that was working for the player, I was honestly fed up enough with the problem that extending it to enemies wasn't something I was actively planning — I wasn't thinking about enemies at all yet.

What actually mattered is that the decisions I made under that pressure — keeping "what tracks attack progress," "what an attack does," and "what counts as a hit" as separate, decoupled pieces (`AttackRuntime`, `Effect`, `Hitbox`) instead of bundling them into one player-specific class — turned out to be sound for reasons I wasn't solving for at the time. When I later built enemy combat, `EnemyAttackModule` could just reuse the same three pieces directly, with no rewrite. That wasn't foresight. It was a side effect of the parts being decoupled enough on their own terms to survive a use case I hadn't designed for yet — which told me, after the fact, that those specific decisions had actually been the right ones, even though I made them for a much narrower reason in the moment.

I made one deliberate tradeoff during that same week, separate from the structure question: `AttackRuntime` ties attack timing directly to `Animator.normalizedTime` rather than driving attacks with code-side timers or coroutines. The timer-based version would have been simpler to build and easier to reason about. I chose the animation-driven version anyway, specifically to force myself to get better at actually animating things, instead of letting code-side timing let me avoid that skill. It's a harder system to get right — most of the build-only bugs in this project's history came from exactly this coupling between animation state and combat logic — but that part of the tradeoff was made on purpose.

---

## Decisions made to avoid technical debt

- **Shared combat pipeline, not parallel implementations.** Player and enemy combat both run through `AttackRuntime`/`Effect`/`Hitbox`. The alternative — writing separate, simpler combat logic per enemy — would have been faster initially but would mean every future combat fix needs to be found and re-applied N times.
- **ScriptableObject-driven data instead of hardcoded values.** New weapons, attacks, and enemies are new assets, not new code. This was a deliberate choice to keep design iteration decoupled from recompilation.
- **Strategy-pattern effects instead of a switch statement.** `Effect` is abstract; `DamageEffect`/`KnockBackEffect`/`SpawnParticlesEffect` each implement `Apply()` independently. Adding a new effect type doesn't require editing every place that already dispatches on effect type.
- **Generate-and-test procedural generation, not generate-and-hope.** Connectivity is validated before a room is accepted. This also means the three generation algorithms can stay fully interchangeable — none of them need their own bespoke validity logic, because validation happens once, after the fact, regardless of which algorithm produced the output.

### Where this wasn't applied consistently — and I know it
Not every part of the codebase follows these decisions evenly yet, and I'd rather say so than have it discovered:

- `RoomGenerator.GenerateRoom()`'s retry loop has no maximum-attempt cap, unlike the equivalent retry logic in `RoomAssembler.GetValidRoomPosNear()`, which does. I know which pattern is correct here — closing this gap is on my list before I'd call generation "done."
- `RoomPalleteSO.Pallete` rebuilds its lookup dictionary from two parallel lists on every access rather than caching it, because Unity has no native dictionary serialization and my first attempt at a change-only-rebuild cache (via a setter) didn't work cleanly with Inspector edits. The fix is an `OnValidate()`-invalidated lazy cache; I haven't gotten to it yet because current dungeon sizes make the cost negligible, but I know exactly where this stops being true.
- A couple of scripts (`EnemyBrain.cs`, `ProximityAttack.cs`) still live outside the `_Scripts` folder convention used by the rest of the project — leftover from before that convention was settled.

---

## Tech stack

- **Unity** (2D, URP/Built-in — *fill in version*)
- **[DOTween](https://dotween.demigiant.com/)** — used for simple movement tweens (e.g. enemy lunge attacks) rather than hand-rolled interpolation.
- **[SerializeReference Extensions (MackySoft)](https://github.com/mackysoft/Unity-SerializeReferenceExtensions)** — enables polymorphic `List<Effect>` serialization in the Inspector, which Unity doesn't support natively.
- **[EditorAttributes](https://github.com/sandrigogomes/EditorAttributes)** — conditional fields, foldout groups, and inspector buttons for the generation/assembly tools, used to build actual tooling around the procedural systems rather than relying on raw `[SerializeField]`s.

---

## What's next

- Adding a procedural Weapon Generation system that generates unique weapon loot. But the Weapon Base and animation will remain the same just with different effects.
- Adding json Serialisation to Room Data to allow for custom authored rooms to be used in the assembly 
- Better Integration of the rooms and enemies since they are currently only in seperate test environments
- More enemy archetypes, taking advantage of the existing module composition.

<!-- GIF: a future feature in progress, or a "before/after" of a fixed bug -->
<!-- ![In progress](docs/gifs/wip.gif) -->
