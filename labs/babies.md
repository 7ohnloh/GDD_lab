# Unity for Babies: done (pending in-game test by user)
Source: https://natalieagus.github.io/50033/docs/babies/checkoff

## Graded checkoff
- [x] Two or more interactable objects with sound (Question Box and Brick)

### Question Box (`Assets/Prefabs/Question-Box-Spring.prefab`, 4 in scene)
- [x] Bounces only when hit from below: `BounceOnHitFromBelow.cs` keeps the box on FreezeAll and unlocks Y only when the contact normal y > 0.5
- [x] Blinks: `QuestionBoxBlink.anim`, 6 samples, looping
- [x] Coin spawns, animates and lands back in the box: `Coin` child with `CoinPop.anim` (y 0→3→0), Order in Layer -1
- [x] Sound on spawn: AudioSource `smb_coin.wav` with Play On Awake, triggered when the script enables the Coin
- [x] Disabled sprite: Animator trigger `hit` goes to `QuestionBoxUsed` (`misc-3_30`)
- [x] Stops bouncing once disabled: `used` flag, relocked to its start position after `settleTime`

### Brick
- [x] Bounces once, only from below: same script, no Animator
- [x] Two variants: `Brick-Coin-Spring.prefab` (coin) and `Brick-Spring.prefab` (no coin)
- [x] Doesn't break
- [x] Sound on coin: the coin variant uses the same Coin child
- [x] No score change

## Setup notes
- Layers: Enemies = 6, Obstacles = 7. Collision matrix unticks Obstacles↔Obstacles, Obstacles↔Ground, Enemies↔Obstacles and Enemies↔Ground.
- Mario is tagged `Player`, with Gravity Scale 4, Freeze Rotation Z, Speed 25 and Up Speed 22. Jumping is re-enabled on layers 3, 6 and 7 through `collisionLayerMask` in `PlayerMovement.cs`.
