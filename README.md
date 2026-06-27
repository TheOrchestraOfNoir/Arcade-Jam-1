# Arcade Jam 2

Local 2-player Greek mythology platformer — **Zeus vs Poseidon** — with rhythm duels.

## Characters & art

| Player | Character | Assets used |
|--------|-----------|-------------|
| **Player 1** | Zeus | Stand, Walk, Jump GIFs |
| **Player 2** | Poseidon | Stand + Jump GIF |
| **Ground** | Tile | `GroundTile.png` |

Art lives in `Assets/Art/`. Sprites swap automatically when you move or jump (`PlayerCharacterVisual.cs`).

## What is in the game so far

1. **Ready screen** — Both players press Jump to start.
2. **Platformer fight** — Move, jump, aim in 4 directions, shoot bullets. Avoid spikes and pits.
3. **Health** — Each player starts with 3 HP. Hazards and losing a rhythm duel cost 1 HP.
4. **Rhythm duels** — Match pauses briefly. Arrows appear on beat; press the matching direction on time. Higher score wins; loser takes 1 damage.
5. **Auto duels** — A duel also starts automatically about every 32 beats (~16 seconds).
6. **On-beat bonus** — Jump or shoot on the beat for a stronger jump or faster shot.
7. **Game over** — At 0 HP, the other player wins. Press Jump to restart.
8. **Random special bonus** — A yellow **!** appears above one or both players. Press **Special (B)** in time for a random reward.

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
| Ready / Restart | Space | A button |

### Player 2

| Action | Keyboard option A | Keyboard option B | Gamepad 2 |
|--------|-------------------|-------------------|-----------|
| Move | Arrow keys | I / L | Left stick |
| Aim | Arrow keys | I / J / K / L | Left stick |
| Jump | Right Shift | Right Shift | A button |
| Shoot | N | N | X button |
| Start rhythm duel | , (comma) | , (comma) | Y button |
| **Special bonus** | **M** | **M** | **B button** |
| Ready / Restart | Right Shift | Right Shift | A button |

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
