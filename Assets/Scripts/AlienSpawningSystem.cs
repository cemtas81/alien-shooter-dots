using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using Unity.Collections;

/// <summary>
/// System that handles alien spawning
/// </summary>
[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct AlienSpawningSystem : ISystem
{
    private Random _random;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _random = new Random(123); // Fixed seed for deterministic behavior
        state.RequireForUpdate<AlienSpawner>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float currentTime = (float)SystemAPI.Time.ElapsedTime;
        var entityManager = state.EntityManager;
        
        // Count existing aliens
        var alienQuery = SystemAPI.QueryBuilder().WithAll<Alien>().Build();
        int currentAlienCount = alienQuery.CalculateEntityCount();
        
        // Process each spawner
        foreach (var (spawner, entity) in SystemAPI.Query<RefRW<AlienSpawner>>().WithEntityAccess())
        {
            // Check if we should spawn
            if (currentAlienCount >= spawner.ValueRO.MaxAliens)
                continue;
                
            if (currentTime - spawner.ValueRO.LastSpawnTime < spawner.ValueRO.SpawnCooldown)
                continue;
            
            // Spawn alien
            var alienEntity = entityManager.CreateEntity();
            
            // Random position within spawn area
            float3 spawnPosition = new float3(
                _random.NextFloat(spawner.ValueRO.SpawnAreaMin.x, spawner.ValueRO.SpawnAreaMax.x),
                spawner.ValueRO.SpawnAreaMin.y,
                _random.NextFloat(spawner.ValueRO.SpawnAreaMin.z, spawner.ValueRO.SpawnAreaMax.z)
            );
            
            // Random initial direction
            float3 randomDirection = new float3(
                _random.NextFloat(-1f, 1f),
                0,
                _random.NextFloat(-1f, 1f)
            );
            randomDirection = math.normalize(randomDirection);
            
            entityManager.AddComponentData(alienEntity, LocalTransform.FromPosition(spawnPosition));
            entityManager.AddComponentData(alienEntity, new Alien
            {
                Speed = _random.NextFloat(3f, 8f),
                Direction = randomDirection
            });
            
            // Update spawner's last spawn time
            var updatedSpawner = spawner.ValueRO;
            updatedSpawner.LastSpawnTime = currentTime;
            spawner.ValueRW = updatedSpawner;
        }
    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
}