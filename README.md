# Alien Shooter DOTS (Unity 2022.3 LTS, URP, Entities/Burst/Jobs)

This repository contains a ready-to-open Unity project implementing a minimal 3D Alien Shooter using Unity DOTS/ECS/Jobs with Burst and Entities Graphics for rendering.

## Controls
- **Move**: W / A / S / D
- **Shoot**: Space

## Requirements
- Unity 2022.3 LTS (tested with 2022.3.40f1)
- URP, Entities, Entities Graphics, Burst, Mathematics (installed via manifest)

## How to run
1. Clone the repo.
2. Open the project folder in Unity Hub (Unity 2022.3 LTS).
3. Open `Assets/Scenes/Main.unity` and press Play.

## Technical Implementation
This project demonstrates a complete DOTS/ECS implementation with the following systems:

### Systems
- **GameInitializationSystem**: Sets up the initial game state, creates player and spawner entities
- **PlayerMovementSystem**: Handles WASD movement with boundary clamping  
- **PlayerShootingSystem**: Creates bullet entities when Space is pressed
- **BulletMovementSystem**: Moves bullets and handles lifetime/bounds checking
- **AlienSpawningSystem**: Spawns aliens up to a maximum count with random positions and directions
- **AlienMovementSystem**: Moves aliens with boundary bouncing
- **CollisionDetectionSystem**: Detects bullet-alien collisions and destroys entities

### Components
- **Player**: Player entity data (speed, cooldown, etc.)
- **Alien**: Alien entity data (speed, direction)
- **Bullet**: Bullet entity data (speed, direction, lifetime)
- **GameBounds**: Defines the playable area boundaries
- **AlienSpawner**: Controls alien spawning parameters

### Key Features
- All core logic implemented as DOTS ECS systems with BurstCompile for performance
- Entity-based architecture with proper component separation
- Efficient collision detection using squared distance calculations
- Boundary system for constraining movement and destroying out-of-bounds entities
- Proper entity lifecycle management with EntityCommandBuffer

### Architecture Notes
- Uses Unity's new Entity API (Entities 1.x)
- Systems are organized in appropriate SystemGroups
- BurstCompile attribute used where possible for maximum performance
- Input handling separated from burst-compiled logic
- Efficient queries using SystemAPI

## Project Structure
```
Assets/
├── Scenes/
│   └── Main.unity          # Main game scene
├── Scripts/
│   ├── Bootstrap.cs        # MonoBehaviour bootstrap
│   ├── Components.cs       # ECS component definitions
│   └── *System.cs          # ECS system implementations
└── Settings/
    └── URP/                # Universal Render Pipeline assets
```

## Performance Considerations
- All gameplay systems use Burst compilation
- Efficient collision detection with early exits
- Proper use of EntityCommandBuffer for structural changes
- Square distance calculations to avoid expensive sqrt operations
- Entity queries optimized with proper component requirements

This implementation serves as a solid foundation for DOTS-based games and demonstrates best practices for Unity ECS architecture.