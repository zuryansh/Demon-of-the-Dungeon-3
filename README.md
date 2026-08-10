# Demon of the Dungeon 3

A solo-developed top-down dungeon crawler roguelike built in Unity — procedurally generated dungeons, a data-driven combat system, and modular enemy AI, designed and implemented from scratch by a single developer (design, art, and code).

**[Play the web build on itch.io →](https://zuri01.itch.io/demon-of-the-dungeon)**

This is the third iteration of the concept. The first two attempts (made in 9th and 12th grade) were abandoned once it became clear the skills needed hadn't caught up to the ambition yet. This version reflects what changed since then — most of the effort here went into building systems that could scale, rather than hardcoding one dungeon.

## Vision

The long-term plan is a run structure built around five enemy "races," each with its own floor, visual identity, and boss: **Slimes → Orcs → Humans → Elves → Angels**, roughly escalating from feral to civilized to divine.

**This is a roadmap, not a feature list of what's shipped.** Right now, the Slime floor is playable end-to-end. Orc enemy design (readable attack telegraphs, a teaching progression for the dodge mechanic) exists at the design stage but isn't implemented yet. Humans, Elves, and Angels are concept-level. I'm calling this out explicitly rather than letting screenshots imply otherwise — what's below is what actually runs.

## What's playable right now

- Procedurally generated dungeon rooms with organic, non-rectangular shapes, validated by a flood-fill algorithm to guarantee full connectivity
- A map thats layed out using a organic growing algorithim that places rooms according to its chosen anchors. 
- A full melee/ranged combat loop (sword, spear, bow) with combo timing, hit detection, and knockback/debuff effects
- A working Slime enemy floor: basic slimes, ranged slimes, and a larger "stacked" slime variant, with a room-based enemy spawner that scales difficulty to room size
- An intro cutscene and dialogue system
- Persistent scene management across multiple additively-loaded scenes, with dependency-graph-based loading/unloading

## Architecture

A few decisions worth calling out specifically, because they're the part of this project I'd want a technical reviewer to actually look at:

**Composition-based enemy AI.** `EnemyBrain` doesn't hardcode behavior — it coordinates a pluggable `EnemyMovementModule` and `EnemyAttackModule` per enemy, so new enemy types are built by swapping modules rather than branching a monolithic AI class.

**A shared, data-driven combat pipeline.** `AttackData` → `AttackRuntime` → `Effect` → `Hitbox` → `EffectContext` forms one pipeline used by the player, regular enemies, *and* bosses. Effects (damage, knockback, debuffs, etc.) are `[SerializeReference, SubclassSelector]` polymorphic classes editable straight in the Inspector — adding a new effect type means writing one small class, not touching the pipeline.

**More on the Effects**: The BEST Thing I thought of * The effects classes effectively act as the game effects from unreal engine. Allowing me to add essentially whatever I want, anywhere i want as long as i can generate a valid effect context. This has been used for things like : Damage, Stun, Camera Shake, Particles, Sounds, Knockback and later expanded into a Buff/Debuff Class for handling over time effects. Most things in unity can be done by accessing a specific component and calling a function on it that is pretty much what the effect class was made to do in one place so that it can be reused anywhere its needed.

**Two interchangeable attack-lifecycle models.** Melee combos finish based on animator progress (`AnimationAttackRuntime`); boss attacks finish based on arbitrary predicates — elapsed time, a wall-hit event, a projectile counter (`ConditionAttackRuntime`). Both expose the same completion event, so the rest of the system doesn't need to know which kind of attack is running. The reason the animated one was chosen for the player was to force me as the developer to actually make animations for each attack and not leave it unfinished.

**Type-keyed attack dispatch for bosses.** The `Boss` base class maintains a `Dictionary<Type, Func<AttackData, IEnumerator>>`; subclasses register handlers with `RegisterAttack<T>()`. Adding a boss attack is: write an `AttackData` subclass, write one method, register it — no growing switch statement on an enum. Most Powerfull in adding an arbitrary number of differnt bosses with differnt behavior as it just needs to override the attack methods from the prev class or register its own. the inherited bosses dont need to worry about anything else.

**Scene Dependency Loading System** The `GameSceneManager` class effectively implements a tree based scene loading system. Each scene has a corresponding `SceneData` asset that contains a list of scene datas its dependent on as well as BG music for that scene. In hindsight its probably overkill for this particular project for now but its extremely useful and reusable in any future projects. 

**Note on process:** for some of these systems, I used AI to explore architecture options (e.g. weighing a dispatch table against a switch-based approach) before implementing, testing, and debugging them myself. For most of these complex systems its my first time working with them and i found AI was most useful in informing me of best practices/possible solutions. But often failed to meet proper implementation/expansion standards. Atleast with the free models i was using.

## Bugs worth mentioning

The sword combo system had an intermittent animation bug that only appeared in built player, never in the editor. It turned out to be three separate, layered causes:

1. `Animator.CrossFade` isn't atomic — reading animator state immediately after calling it can return stale data.
 A same-state early-return guard in the attack code was incorrectly triggered by duplicate animation clips, silently dropping combo inputs.
`Animator.Play` doesn't reliably reset `normalizedTime`, which broke progress tracking for attack completion.

Each cause alone would have been a plausible full explanation; all three were actually stacked. Fixed with targeted fixes per cause and a `hasTicked` guard to prevent stale attack state from leaking across frames.

2. While building the GameSceneManager i discovered that if i already had scenes loaded additively in the editor then they wouldnt get properly picked up by SceneManager's active scenes. The fix was pretty much to wait a few frames for the data to get populated first. And then seeding the scenes manually by calling the OnSceneLoaded funtion on all of them manually. 

## Limitations

- Scale: since im a solo dev doing this in my free time while studying computer science. I cant really put that much time into it. Especially since i hand make almost all the assets. No AI is used for ART GENERATION. Unfortunately I cant avoid using AI for code since its part of my job.
- Testing: Basically done by me and my friends play testing
- Solo project — no code review from anyone else (except AI based audits I run myself for tips on improvement ), no branch/PR workflow. That's the honest state of it. Do plan to collaborate with people in the future.

## Tech

- Unity (2D URP)
- DOTween for tweening (boss jump attacks, UI transitions)
- Custom `[SerializeReference, SubclassSelector]`-based polymorphic effect system
- New Input System (keyboard/mouse + gamepad/joystick support)

## Running it

**Easiest:** play the current web build directly on [itch.io](https://zuri01.itch.io/demon-of-the-dungeon).

**From source:**
1. Clone the repo
2. Open with Unity (see `ProjectSettings` for exact version)
3. Open the Main scene under `Assets/Scenes/Main`
4. Press Play

## Future Plans
1. Making a Full Item System insprited by Binding Of Isaac. 
2. Making a Random Weapon Generation sytem: Will most likely using the pre existing effect system for diff weapon combinations that add random functionality to authored weapon templates. The weapons in the game currently are basically these templates
3. More Roguelike elements: Adding a hub area with NPC's and permanenet upgrades similar to Hades.
4. Adding hand authored rooms alongside generated ones in the map. Plan to use JSON serialisation to save designs to disc that can later be loaded into the `RoomAssembler` . The blueprint is already in place with the `RoomData` class usage.
5. Making a proper Trailer and possible release on Mobile/Steam. Although Mobile release is not the main plan I do enjoy seeing it run on a phone.

---

*Solo project — design, art, and programming all by me. Currently in active development.*
