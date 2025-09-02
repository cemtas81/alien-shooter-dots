using Unity.Entities;
using Unity.Mathematics;

/// <summary>
/// Component for player entities
/// </summary>
public struct Player : IComponentData
{
    public float Speed;
    public float3 LastShootTime;
    public float ShootCooldown;
}

/// <summary>
/// Component for alien entities
/// </summary>
public struct Alien : IComponentData
{
    public float Speed;
    public float3 Direction;
}

/// <summary>
/// Component for bullet entities
/// </summary>
public struct Bullet : IComponentData
{
    public float Speed;
    public float3 Direction;
    public float Lifetime;
    public float MaxLifetime;
}

/// <summary>
/// Component for game boundaries
/// </summary>
public struct GameBounds : IComponentData
{
    public float3 Min;
    public float3 Max;
}

/// <summary>
/// Component for entities that can be destroyed
/// </summary>
public struct Destructible : IComponentData
{
}

/// <summary>
/// Tag component for entities that should be spawned
/// </summary>
public struct SpawnTag : IComponentData
{
}

/// <summary>
/// Component for alien spawner
/// </summary>
public struct AlienSpawner : IComponentData
{
    public int MaxAliens;
    public float SpawnCooldown;
    public float LastSpawnTime;
    public float3 SpawnAreaMin;
    public float3 SpawnAreaMax;
}