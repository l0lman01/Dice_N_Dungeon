# Dice n Dungeon

Game jam project (3–4 people, 1–2 weeks). The player is a die in a dungeon.
Unity 6000.6.9f1, URP, Input System 1.20 (project-wide actions), Cinemachine 6.6.

## Game design (3C)
- **Camera**: Cinemachine Camera, Orbital Follow in **Three Ring** style, vertical axis locked (fixed height and pitch), horizontal axis orbits around the player's Y axis (Look action). Binding Mode = World Space so the camera does not turn with the player. Rotation Composer looks at the player.
- **Character**: always faces the camera forward projected on the ground (pitch ignored).
- **Controls**: movement is relative to the character's orientation (W forward, A/D strafe). Space / Gamepad East = **Roll** (dodge with i-frames). When the roll ends the die lands on a face; that face equips its ability (passive: stat modifiers, active: skill fired with Attack).
- The dungeon / map generation (`Assets/Scripts/MapMaker.cs`) is owned by another team member: don't modify it unless asked.

## Code layout
```
Assets/_Project/
  Scripts/      Game.Runtime.asmdef, namespace DiceNDungeon.<Folder>
    Core/       Stat + StatModifier (pure C#), OrientationMath (pure C#), event channels (ScriptableObject)
    Input/      PlayerInputReader (namespace DiceNDungeon.Inputs): the ONLY class that touches the Input System
    Player/     PlayerOrientation, PlayerMotor, PlayerRoll, PlayerDie, PlayerAbilities, PlayerStats, PlayerContext
    Dice/       DieFaceData, DieDefinition (ScriptableObjects), DieRoller, DieOrientation (pure C#)
    Abilities/  Ability -> PassiveAbility (stat modifiers) / ActiveAbility (Activate) -> ShockwaveAbility example
    Combat/     Health, IDamageable, IInvulnerabilitySource, DamageInfo
  Editor/       Game.Editor.asmdef (custom inspectors, debug tools)
  Tests/EditMode/ Game.Tests.EditMode.asmdef (NUnit tests for pure C# logic)
```
Player flow: `PlayerInputReader` -> `PlayerOrientation` (faces camera) -> `PlayerMotor` (moves, can be overridden) ;
`PlayerRoll.RollEnded` -> `PlayerDie.Roll()` -> `FaceRolled` -> `PlayerAbilities` equips the face's `Ability`.
The die mesh is a child (`dieVisual`); the root with the CharacterController never rolls.

## Coding conventions
- Inspector fields are always `[SerializeField] private`, never `public`, each with a `[Tooltip]` and an end-of-line comment giving meaning and unit:
  ```csharp
  [Tooltip("Ground speed in meters per second.")]
  [SerializeField] private float moveSpeed = 6f; // m/s
  ```
  Expose values with read-only properties (`public float MoveSpeed => moveSpeed;`).
- One MonoBehaviour / ScriptableObject per file, file name = class name.
- Communicate with C# events inside the player, ScriptableObject event channels across systems. No new singletons.
- Logic that doesn't need a scene goes in pure C# classes, with an EditMode test.
- ScriptableObject abilities are shared assets: never store per-player state in them.
- Abilities get everything through `PlayerContext`, not `GetComponent`.

## Unity rules for agents
- When moving or renaming an asset, move its `.meta` too (`git mv` both) to keep the GUID, or prefab references break.
- New files and folders need a `.meta` with a unique GUID; commit them.
- Don't hand-edit scenes or prefabs (YAML) unless asked: describe the Editor steps instead.
- Avoid `UnityEngine.Input` (old input manager); use `PlayerInputReader`.
- There is no Unity Editor in cloud sessions: code can't be compiled or tested there, so review carefully and ask the user to check the Console and Test Runner.

## Team workflow
- Personal sandbox scenes (`Assets/Scenes/<Name>.unity`), shared work goes into prefabs.
- One feature per branch. Tests: Window > General > Test Runner > EditMode.
