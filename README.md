# Arcade Jam 2

Local 2-player Greek mythology platformer — **Zeus vs Poseidon** — with rhythm duels.

## Characters & art

| Player | Character | Assets used |
|--------|-----------|-------------|
| **Player 1** | Zeus | Stand, Walk, Jump, Dance GIFs |
| **Player 2** | Poseidon | Stand + Jump GIF |
| **Ground** | Floating island | `GroundTile.png` |
| **Bullets** | Fireball | `Effects/Fireball.gif` |
| **Hits** | Explosion | `Effects/Explosion.gif` |
| **Menu** | Game logo | `UI/GameLogo.png` |

Art lives in `Assets/Art/`. Animated GIFs are exported to sprite strips in `Assets/Resources/Animations/` and play automatically (walk, jump, shoot, rhythm emote, fireball, explosion).

## What is in the game so far

1. **Main menu** — Title screen with logo, full controls, and **Start Game**. Press Space, Enter, or either Jump key to start.
2. **Ready screen** — Both players press Jump to start the match.
3. **Platformer fight** — Move, jump, aim in 4 directions, shoot bullets. Avoid spikes and pits.
4. **Health** — Each player starts with 3 HP. Hazards and losing a rhythm duel cost 1 HP.
5. **Rhythm duels** — Match pauses briefly. Arrows appear on beat; press the matching direction on time. Higher score wins; loser takes 1 damage.
6. **Auto duels** — A duel also starts automatically about every 48 beats (slower default tempo).
7. **On-beat bonus** — Jump or shoot on the beat for a stronger jump or faster shot.
8. **Game over** — At 0 HP, the other player wins. Press Jump or Enter to return to the main menu.
9. **Random special bonus** — A yellow **!** appears above one or both players. Press **Special (B)** in time for a random reward.

## Custom audio

Match music: `Assets/Audio/FIGHT_MUSIC_OrchestraOfNoir.mp3` (Orchestra of Noir fight theme).

The rhythm system **syncs to the song** during matches — duels, note timing, and on-beat bonuses follow the music at **120 BPM** by default.

To fine-tune sync in Unity, select **GameController → Game Settings**:
- **Rhythm Bpm** — raise/lower if beats feel early or late (try 118–122)
- **Music Beat Offset** — nudge if the first beat should start slightly into the track
- **Music Volume** — loudness of the fight theme

## Rhythm speed

Default tempo is **120 BPM**, synced to the fight music on **Game Settings** / **Beat Conductor**.

## In-game controls HUD

During matches, a compact controls panel appears in the **bottom-left corner**. Rhythm duels use the **cloud** at the top of the screen — arrows fall from the cloud toward the hit line.

## Controls (one keyboard, two players)

Player 1 and Player 2 use **different keys** so both can play on the same keyboard.

### Player 1

| Action | Keyboard | Gamepad 1 |
|--------|----------|-----------|
| Move | A / D | Left stick |
| Aim | W / A / S / D | Left stick |
| Jump | Space | A button |
| Shoot | R | X button |
| Start rhythm duel | Y | Y button |
| **Special bonus** | **T** | **B button** |
| Ready / Start | Space | A button |

### Player 2

| Action | Keyboard option A | Keyboard option B | Gamepad 2 |
|--------|-------------------|-------------------|-----------|
| Move | Arrow keys | I / L | Left stick |
| Aim | Arrow keys | I / J / K / L | Left stick |
| Jump | Right Shift | Right Shift | A button |
| Shoot | N | N | X button |
| Start rhythm duel | , (comma) | , (comma) | Y button |
| **Special bonus** | **M** | **M** | **B button** |
| Ready / Start | Right Shift | Right Shift | A button |

### Special bonus (random)

When a yellow **!** floats above your character, press **Special** quickly (within ~4 seconds):

| Result | What it does |
|--------|----------------|
| **STRIKE!** | Opponent loses 1 HP |
| **HEAL!** | You gain 1 HP (max 3) |
| **SPEED!** | Faster movement for 5 seconds |
| **RAPID!** | Faster shooting for 5 seconds |

### During a rhythm duel

- Watch the big arrow (↑ ↓ ← →).
- Press the **same direction on the beat**.
- **Player 1:** WASD  
- **Player 2:** Arrow keys **or** IJKL  
- Ratings: **PERFECT** (+2), **GOOD** (+1), **BAD** (0), **MISS** (-1)

## Best setup

- **2 gamepads** — smoothest experience (Gamepad 1 = P1, Gamepad 2 = P2).
- **1 keyboard** — P1 uses left side (WASD + Space/R/Y), P2 uses right side (IJKL or Arrows + Shift/N/,).
