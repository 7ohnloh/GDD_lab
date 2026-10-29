# Unity for Teens: status
Source: https://natalieagus.github.io/50033/docs/teen/checkoff, /teen/scriptobjgamearch, /teen/fsm

Checked 2026-10-24. Recording max 5 min, binary grading per feature, lenient on buffs/powerups and restart. Note: labs/children.md is stale (it says no ScriptableObjects, highscore, pause or mushroom, but `IntVariable`, `GameScore.asset`, `MagicMushroom.cs`, `PauseButtonController.cs`, `MainMenu.unity` all exist now).

## Graded checkoff
- [ ] 1a. Mario FSM per diagram (SmallMario, SuperMario, FireMario, InvincibleSmallMario, DeadMario): no `StateController`, `State`, `Action`, `Decision`, `Transition` scripts. No FireMario or fire flower/fireball. Mario state is only `PlayerMovement` flags. The diagram image is not readable by me; check it on the checkoff page.
- [ ] 1b. Separate buff FSM: a `BuffStateController : StateController` on Mario. Missing. The /fsm page I fetched has no buff FSM section, only the checkoff page mentions it plus a buff diagram, so you design it from the diagram.
- [ ] 1c. Collectible/buff that expires after a few seconds: none (the mushroom is permanent, starman absent). Invincibility after damage plus a `CountdownDecision` would satisfy this.
- [ ] 1d. Restart returns every component to its original state: partial. `GameManager.gameRestart` resets Mario position, Goombas, score, bricks. Needs to also reset FSM state and powerups.
- [ ] 2a. No Singletons, no `DontDestroyOnLoad` object in the Hierarchy: probably already true. No `DontDestroyOnLoad` or static `instance` in any script. But scripts find each other via `FindAnyObjectByType<GameManager>()` (BounceOnHitFromBelow:24, PauseButtonController:15) and `FindGameObjectWithTag("Manager")` (JumpOverGoomba:19), and GameManager is a scene object wired via UnityEvents. That is the pattern the handout wants replaced by SO GameEvents.
- [ ] 2b. Multi-scene navigation with persisted values: `MainMenu.unity` and `SampleScene.unity` are both in Build Settings; `GameScore.asset` (IntVariable with `highscoreKey`/PlayerPrefs) persists. [?] Handout wants at least two clean-slate world scenes (World-1-1 and 1-2) plus menu/loading. There is only one world scene. Check manually that score/highscore visibly persists across menu -> game in a recording.
- [ ] 3. Sound effects on every state change: some clips exist in `Assets/Sounds` (`smb_1-up`, `smb_coin`, `smb_pause`, `smb_jump-*`, `smb_fireball`, `smb_kick`...). No per-state-change audio wiring. Need to check for powerup/damage/dead/fire clips beyond what is listed.
- [ ] 4. Recording shows Mario Inspector (or scripts), Hierarchy with no DontDestroyOnLoad, scene navigation, buff pickup/expiry and restart, with audio.

## Handout prerequisites
### ScriptableObject Game Architecture page
- [ ] Preparation: two clean-slate scenes, prefab copies without scripts. Optional (the handout allows working on a copy or doing it in place). Editor: duplicate project/scene, add scenes to File > Build Profiles.
- [x] Score as SO: `Variable<T>`, `IntVariable`, `GameScore.asset` exist (Assets/Variable.cs, IntVariable.cs, GameScore.asset).
- [ ] `GameEvent<T>` + `GameEventListener<T>` generic base: missing. Code: 2 new scripts.
- [ ] Pinned variants `Void`/`SimpleGameEvent`, `int`, `IPowerup` (a GameEvent and a Listener script each, 6 scripts total): missing. Video-dependent for the other two variants.
- [ ] Create event assets (handout says about 11, e.g. `OnGameRestart`, `OnDamagePlayer`, score change, game over, powerup collected): missing. Video-dependent. Editor: Create > ScriptableObjects > ...
- [ ] Subscribe via listeners: replace `AddListener` calls in `BounceOnHitFromBelow`, `PauseButtonController`, `EnemyMovement`, `HUDManager`, `MusicManager`, `PlayerMovement` etc. Editor: add listener component, set Event and Response in the Inspector.
- [ ] Raise via `RestartButtonController` UnityEvent -> `SimpleGameEvent.Raise` (or direct `GameEvent` field). Code + Editor.
- [ ] `IPowerupApplicable`/`RequestPowerupEffect` routing for powerups: missing (the project has a single `MagicMushroom.cs`).
- [ ] Migrate whole project, remove `GameManager` UnityEvent hub: the biggest item. Video-dependent.

### Finite State Machine page
- [ ] Framework: `State`, `Action`, `EventAction` + `ActionType` enum, `Decision`, `Transition`, `StateController`. 6 scripts, mostly copied from the handout.
- [ ] `MarioStateController`, `MarioState` enum, `PowerupType` enum (`Default`, `MagicMushroom`, `FireFlower`, `Damage`...), `SetPowerup`.
- [ ] Actions: `SetupAnimatorAction`, `ClearPowerupAction`, `FireAttackAction`, `InvincibleAction` (flicker coroutine, `gameConstants.flickerInterval`). Video-dependent.
- [ ] Action assets: `SetupSmallMario/SuperMario/FireMario/DeadMario`, `ClearPowerup`, `FireAttackAction`, `InvincibleAction`, `MarioFaceRight` (`BoolVariable`). Editor: needs 4 Mario Animator Controllers (Small, Super, Fire, Dead) plus animations. Existing controllers are only Goomba, Coin and Question Box. Mario sprite sheet is `Assets/Sprites/mario.png`, which may not include super/fire frames: check.
- [ ] Fireball prefab (Rigidbody2D, collider, AudioSource, `FireballController`, 3 max, destroy after 2 s or on Enemy tag), Z key to fire. Editor: build prefab. Input: add a `fire` action to `Assets/InputSystem/MarioActions.inputactions` (Z), route via `ActionManager` to `MarioStateController.Fire`. Tag `Enemy` exists.
- [ ] Decisions: `TransformDecision` (+ `EnumExtension`), `CountdownDecision`, and 4 `ShouldTransformTo*` assets plus a countdown asset. Video-dependent.
- [ ] State assets: `RemainInState`, `SmallMario`, `SuperMario`, `FireMario`, `InvincibleSmallMario`, `DeadMario`. Editor-heavy wiring.
- [ ] Attach `MarioStateController` to Mario: Start State `SmallMario`, Remain State `RemainInState`. Mario is `m_Name: Mario` in SampleScene.
- [ ] Modify damage (`PlayerMovement` death path -> `SetPowerup(Damage)`) and powerups (`MagicMushroom` -> `SetPowerup`; new `FireFlowerPowerup`). Video-dependent.
- [ ] Fire flower pickup and a question box that spawns it: only mushroom exists. Reuse `Question-Box-Spring.prefab` pattern.

### Gotchas
- Handout names (`PlayerController`, `GameManager.instance`, `World-1-1`) differ from this project (`PlayerMovement`, a scene-object `GameManager` tagged `Manager`, `SampleScene`). Mario's spawn is (-22.46, -1.45).
- Tags: `Enemy` is singular. Handout snippets check the `Player` tag, so confirm Mario has it.
- Project is Unity 6, so use `linearVelocity`, and the new Input System, not the legacy `Input.GetKey(Z)`.
- `GameManager.Awake/Start` sets score to 0 on every load and calls `LoadHighestValue`. Check that the highscore actually persists across scenes.
- Only 1 world scene exists, but the handout's "navigation with persistent values" assumes two worlds or menu plus world. Menu-to-game may be accepted: confirm with a TA if unsure.
- `labs/children.md` is out of date.
- Unity cannot serialize open generics: each event type needs a concrete subclass.
- The /fsm page I fetched did not contain a buff FSM walkthrough, so recheck the page in a browser before starting the buff part.

## Plan (follow the handout, user writes the code)
1. SO events: `GameEvent`/`GameEventListener`, event assets, raising events (~2h)
2. Migrate the whole project to events (~2-2.5h)
3. FSM framework and `MarioStateController` (~2h)
4. Mario animators, action/decision/state assets, damage and mushroom (~3-4h)
5. Fire attack: fireball, fire flower, shoot key (~1.5-2h)
6. Checkoff extras: buff FSM with a star, World 1-2 finish line, sounds, restart check (~2-3h)
7. Rehearse and record (~30m)

## Next 3 actions
1. Add the SO event system: `GameEvent<T>`, `GameEventListener<T>`, and `Void`/`int`/`IPowerup` subclasses, then create event assets and migrate `GameManager`'s four UnityEvents.
2. Paste in the pluggable FSM framework scripts, create `MarioStateController` and `MarioState`/`PowerupType`, then build the 4 Mario Animator Controllers.
3. Build action/decision/state assets for SmallMario, SuperMario, FireMario, InvincibleSmallMario and DeadMario, then add `BuffStateController` and a timed buff.
