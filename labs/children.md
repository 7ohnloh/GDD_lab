# Unity for Children: status
Source: https://natalieagus.github.io/50033/docs/children/checkoff, /singletons, /static, /scriptableobjects, /coroutines, /building

Checked 2026-10-14. Checkoff max 5 minutes of video, binary grading per feature (graders lenient). The handout uses `GameManagerWeek4`/`World-1-1` naming. This project has one scene, `SampleScene`, and the only build-settings scene is `SampleScene`.

## Graded checkoff
- [ ] 1. Player feedback (SFX recorded by yourself): partial.
  - [ ] Music stops on pause and resumes: no pause feature at all (no Time.timeScale pause toggle, no pause button).
  - [ ] Powerup spawn sound: coin sound exists (`smb_coin.wav` on Coin child), but no mushroom/powerup. Also the handout wants self-recorded sounds, not the stock SMB ones.
  - [ ] Goomba dying sound: no AudioSource on Goomba/stomp (`smb_stomp` is only a planned item in toddlers.md).
  - [?] Game Over sound: `GameOverSound` object and `MusicManager` GameOver snapshot exist in the scene. Check it plays in game.
- [ ] 2. Scene navigation: only `SampleScene` exists. No main menu, no loading scene, no return-to-menu. No SceneManager use in any script.
- [ ] 3. Persistent highscore: no `PlayerPrefs`/IntVariable. No reset button, no second world, no score carry-over.
- [ ] 4. Two powerups plus the spawn-bug fix: Coin boxes and bricks exist (babies). No mushroom/starman/1-UP, no `IPowerup`/`BasePowerup`. The bug (hit box from above) is only meaningful once the powerup is a Rigidbody2D that spawns via impulse.
- [ ] 5. Restart via ScriptableObjects (resets Goombas, powerups, music, score): restart exists for Goombas/score via GameManager events, and music snapshot Gameplay is restored by `MusicManager.GameRestart`. No ScriptableObject anywhere, and powerups are not reset. Must be shown in code/Inspector.

## Handout prerequisites
### Singleton page (/singletons, /static)
- [ ] `Singleton<T>.cs` base class, GameManager inherits it. Code: new script plus GameManager change.
- [ ] Build Settings contains all scenes. Editor: File > Build Profiles > Open Scene List, drag scenes in.
- [ ] `NextScene.cs` on a castle (EdgeCollider2D trigger, `nextSceneName`). Editor: castle object in scene, layer, sorting layer. No Castle object in the scene.
- [ ] World 2 scene (`World-1-2`): not created. Editor: build a second scene, Mario/Camera/Canvas copies, spawn position.
- [ ] Static-instance refactor: only GameManager persists, others subscribe via `GameManager.instance` in Awake, Script Execution Order for GameManager, `RestartButtonController`. Currently Mario/EnemyManager/HUD are wired through Inspector UnityEvents (scene references, which break across scenes).
- [ ] Camera finds Mario by tag: no `CameraController.cs` exists (the camera is probably static or follows another way, check).
- [ ] `IInteractiveButton`, `PauseButtonController` (pause/play icons): missing.
- [ ] `IPowerup`, `IPowerupController`, `PowerupType` enum, `BasePowerup` (abstract), `MagicMushroomPowerup`, `QuestionBoxPowerupController`, `BrickPowerupController`: missing.

### Scriptable Objects page
- [ ] `GameConstants` SO + `SMBConstants` asset, linked from PlayerMovement/EnemyMovement. Not present. Code: small script, edit PlayerMovement Start.
- [ ] `Variable<T>`, `IntVariable`, `GameScore` asset, wired into GameManager and HUDManager (highscore text "TOP- 000000"). Missing.
- [ ] Optional `GameEvent` SO: missing.
- [ ] Persist highscore across Editor restarts (SO asset keeps `previousHighestValue`). For builds the handout says use PlayerPrefs or JSON.

### Coroutines page
- [ ] Loading scene with CanvasGroup fade (`LoadAndWait.cs`), return to main menu: missing. Video-dependent setup, watch the page video.
- [ ] Button highlight (Normal alpha 0 / Highlighted alpha 1) and reset selection: missing, video-dependent.
- [ ] Powerup bug fix (collider disabled + Static body until `SpawnPowerup`, wait one frame via coroutine before impulse): not applicable until powerups exist. Video-dependent.
- [x] Coroutines already used (`BounceOnHitFromBelow.LockAfterBounce`, `EnemyMovement.HideAfterDelay`).
- Async/Task and Resources sections are demos, not graded.

### Building page
- [ ] Make a playable build and playtest (Build Profiles, scene list). Gotchas: no `UnityEditor` APIs at runtime, PlayerPrefs.Save(), exactly one EventSystem, static fields reset.

## Gotchas
- Handout names `GameManagerWeek4`, `PlayerMovementWeek4`, `World-1-1`/`World-1-2` and positions `(-10.24, -4.35)`. This project: `GameManager`, `SampleScene`, Mario spawn `(-22.46, -1.45)`.
- Handout layer 10 = Pipe and layer `interactive` for the castle. Here: Ground 3, Enemies 6, Obstacles 7. No Pipe/interactive layer. Create or use your own layer and update the mushroom turn-around check.
- Tags: `Ground`, `Enemy`, `Manager`, `Player`.
- Input: project uses the Input System via ActionManager/PlayerInput. PlayerInput and the EventSystem need the Input System UI Input Module (check the EventSystem in the scene), or buttons won't click in new scenes.
- If Mario/Canvas/Enemies are duplicated per scene (handout approach), the scene-local Inspector UnityEvent wiring (GameManager events to HUD, Enemies, Mario) must be replaced by `GameManager.instance` subscriptions in Awake.
- Existing Toddlers state: a `SuperMarioBros.mixer` (groups SFX, Player, Player Dies, snapshots GameOver/Gameplay) and `MusicManager` now exist. toddlers.md still says mixer not started, so update that file.
- Pause + music: music must stop on pause. Mixer updateMode is already UnscaledTime, so a Paused snapshot or `AudioListener.pause` works.
- Sounds in `Assets/Sounds` are the stock SMB set. The checkoff says to record your own SFX (grader is lenient).

## Next 3 actions
1. Do the Singleton/scene work first: `Singleton<T>`, GameManager singleton, MainMenu + Loading + second world scenes, build settings, NextScene, highscore reset button.
2. Add SO `IntVariable` GameScore (+ `GameConstants`), use it for score, highscore and restart reset, then persist the highscore (SO in Editor, PlayerPrefs for safety).
3. Add powerup hierarchy (`IPowerup`/`BasePowerup`, mushroom plus a second powerup), apply the coroutine spawn-bug fix, add pause button + music stop, Goomba-death and powerup SFX, then record the under-5-minute video.
