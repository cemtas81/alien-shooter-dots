using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;

/// <summary>
/// System that handles alien movement with boundary bouncing
/// </summary>
[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct AlienMovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Alien>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        
        // Get game bounds
        GameBounds bounds = default;
        bool hasBounds = false;
        
        var boundsQuery = SystemAPI.QueryBuilder().WithAll<GameBounds>().Build();
        if (boundsQuery.CalculateEntityCount() > 0)
        {
            bounds = SystemAPI.GetSingleton<GameBounds>();
            hasBounds = true;
        }
        
        // Move aliens and handle boundary bouncing
        foreach (var (transform, alien) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Alien>>())
        {
            // Calculate new position
            float3 movement = alien.ValueRO.Direction * alien.ValueRO.Speed * deltaTime;
            float3 newPosition = transform.ValueRO.Position + movement;
            
            // Handle boundary bouncing if bounds exist
            if (hasBounds)
            {
                float3 newDirection = alien.ValueRO.Direction;
                bool directionChanged = false;
                
                // Check X boundaries
                if (newPosition.x <= bounds.Min.x || newPosition.x >= bounds.Max.x)
                {
                    newDirection.x = -newDirection.x;
                    newPosition.x = math.clamp(newPosition.x, bounds.Min.x, bounds.Max.x);
                    directionChanged = true;
                }
                
                // Check Z boundaries
                if (newPosition.z <= bounds.Min.z || newPosition.z >= bounds.Max.z)
                {
                    newDirection.z = -newDirection.z;
                    newPosition.z = math.clamp(newPosition.z, bounds.Min.z, bounds.Max.z);
                    directionChanged = true;
                }
                
                // Update direction if it changed
                if (directionChanged)
                {
                    var updatedAlien = alien.ValueRO;
                    updatedAlien.Direction = newDirection;
                    alien.ValueRW = updatedAlien;
                }
            }
            
            // Update position
            transform.ValueRW.Position = newPosition;
        }
    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
}