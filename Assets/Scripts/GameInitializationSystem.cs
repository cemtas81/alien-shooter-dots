using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using UnityEngine;
using Unity.Burst;

/// <summary>
/// System that initializes the game entities and spawners
/// </summary>
[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct GameInitializationSystem : ISystem
{
    private bool _initialized;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _initialized = false;
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (_initialized) return;
        
        var entityManager = state.EntityManager;
        
        // Create player entity
        var playerEntity = entityManager.CreateEntity();
        entityManager.AddComponentData(playerEntity, new Player
        {
            Speed = 10f,
            ShootCooldown = 0.2f,
            LastShootTime = float3.zero
        });
        
        entityManager.AddComponentData(playerEntity, LocalTransform.FromPosition(new float3(0, 0, 0)));
        
        // Create basic render mesh for player (will be handled by rendering system)
        // For now, we'll use a simple setup - proper mesh rendering will be added later
        
        // Create alien spawner entity
        var spawnerEntity = entityManager.CreateEntity();
        entityManager.AddComponentData(spawnerEntity, new AlienSpawner
        {
            MaxAliens = 20,
            SpawnCooldown = 1f,
            LastSpawnTime = 0f,
            SpawnAreaMin = new float3(-20, 0, 10),
            SpawnAreaMax = new float3(20, 0, 20)
        });
        
        _initialized = true;
        
        Debug.Log("Game initialized with player and spawner entities");
    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
}