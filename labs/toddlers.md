# Toddlers lab: status
Source: https://natalieagus.github.io/50033/docs/toddlers/checkoff, /management (The Input System), /observer-pattern (The Observer Pattern), /audio-management (Audio Management)

Last updated 2026-10-05. Checked against the saved scene and scripts.

The Goomba stomp is not taught in the handout. It is the checkoff exercise, built with the observer-pattern tools.

## Graded checkoff
- [x] **Input System**: done. `Assets/InputSystem/MarioActions` (map `gameplay`: move/jump/jumphold/click/point, scheme `MarioActions` = Keyboard + Mouse). PlayerInput on Mario uses Invoke Unity Events into `ActionManager`, which fires UnityEvents into `PlayerMovement.Jump/JumpHold/MoveCheck`. PlayerMovement has no legacy `Input` calls left. JumpOverGoomba still uses legacy `Input`, but it gets disabled for the stomp.
- [ ] **AudioMixer + Audio Groups + narrated video**: not started. No `.mixer` asset and no music/SFX AudioSources in the scene. Needs one meaningful effect *other than* ducking the music on game over.
- [x] **Goomba stomp via events, with Goomba animation, score up, jump-over scoring disabled**: done. Goomba Animator (`brown_goomba.controller`: GoombaWalk ↔ GoombaStomped via `onStomped` / `gameRestart`). `EnemyMovement.Stomp()` squashes, stops, disables the collider, fires `UnityEvent<int> stomped` (wired Dynamic to `GameManager.IncreaseScore`), and hides the sprite after 0.5 s (coroutine). `PlayerMovement.OnTriggerEnter2D` decides stomp vs death by whether Mario is falling and more than 0.5 above the Goomba, and bounces Mario. The JumpOverGoomba component is disabled on Mario. The restart path un-squashes the Goomba.

## Observer Pattern page: done
- `GameManager` (tag `Manager`) with events gameStart / gameRestart / scoreChange (Dynamic, wired to HUDManager.SetScore) / gameOver.
- `HUDManager` on Canvas (adapted to this UI: panel toggle, both score texts, the always-visible restart button).
- `EnemyManager` on Enemies, plus `EnemyMovement.GameRestart()`.
- `PlayerMovement` only references GameManager. Death calls `gameManager.GameOver()`. `GameRestart()` resets Mario to (-22.46, -1.45).
- The Game Over and Game Restart events toggle Enemies and Static Environment via `SetActive`. Both restart buttons call `GameManager.GameRestart`.
- `JumpOverGoomba` calls `gameManager.IncreaseScore(1)` (to be disabled for checkoff).
- `AnimationEventIntTool` on the Coin in both coin prefabs (parameter 1). CoinPop.anim has one event at 0.5 s calling `TriggerIntEvent`. All 5 scene coins are linked to `GameManager.IncreaseScore` (Dynamic).

## Remaining
### Audio Management (planned order: A1 mixer, A2 sources, A3 duck + send, A4 extra effect, A5 video)
- Sources to add: music `01-main-theme-overworld` to Background Sound; jump `smb_jump-small` and stomp `smb_stomp` to Player; death `08-you-re-dead` to Player Dies. Route the coin/box prefab sources to SFX.
- [ ] Mixer `SuperMarioBros`: Master > Background Sound, SFX > Player, Player Dies.
- [ ] Background Sound: Duck Volume. Player Dies: Send to Background Sound.
- [ ] AudioSources: music, jump/stomp SFX, death sound. Route every source (including the coin/box prefab sources) to a group.
- [ ] One extra meaningful effect (checkoff requirement), explained in the video.
- [ ] Record the narrated video showing the Mixer and Inspector settings.

## Gotchas
- Mario has no Animator. The handout's `marioAnimator` lines were dropped.
- Goomba: trigger BoxCollider2D, kinematic Rigidbody2D, layer Enemies (6), tag `Enemy`.
- UnityEvent<int> wiring: always pick the Dynamic entry. The static one shows a number field and sends a constant.
- Prefab coin links are per instance. Don't Apply All overrides.
