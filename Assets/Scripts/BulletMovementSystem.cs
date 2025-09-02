using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using Unity.Collections;

/// <summary>
/// System that handles bullet movement and lifetime
/// </summary>
[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct BulletMovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Bullet>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        float deltaTime = SystemAPI.Time.DeltaTime;
        var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);
        
        // Move bullets and update lifetime
        foreach (var (transform, bullet, entity) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<Bullet>>().WithEntityAccess())
        {
            // Update lifetime
            bullet.ValueRW.Lifetime += deltaTime;
            
            // Destroy bullet if lifetime exceeded
            if (bullet.ValueRO.Lifetime >= bullet.ValueRO.MaxLifetime)
            {
                entityCommandBuffer.DestroyEntity(entity);
                continue;
            }
            
            // Move bullet
            float3 movement = bullet.ValueRO.Direction * bullet.ValueRO.Speed * deltaTime;
            transform.ValueRW.Position += movement;
            
            // Check if bullet is out of bounds and destroy it
            var boundsQuery = SystemAPI.QueryBuilder().WithAll<GameBounds>().Build();
            if (boundsQuery.CalculateEntityCount() > 0)
            {
                var bounds = SystemAPI.GetSingleton<GameBounds>();
                float3 pos = transform.ValueRO.Position;
                
                if (pos.x < bounds.Min.x || pos.x > bounds.Max.x ||
                    pos.z < bounds.Min.z || pos.z > bounds.Max.z)
                {
                    entityCommandBuffer.DestroyEntity(entity);
                }
            }
        }
        
        entityCommandBuffer.Playback(state.EntityManager);
        entityCommandBuffer.Dispose();
    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
}