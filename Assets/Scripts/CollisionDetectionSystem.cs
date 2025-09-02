using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using Unity.Collections;

/// <summary>
/// System that handles collision detection between bullets and aliens
/// </summary>
[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(BulletMovementSystem))]
[UpdateAfter(typeof(AlienMovementSystem))]
public partial struct CollisionDetectionSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Bullet>();
        state.RequireForUpdate<Alien>();
    }
    
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var entityCommandBuffer = new EntityCommandBuffer(Allocator.Temp);
        
        // Get all bullet entities and positions
        var bulletQuery = SystemAPI.QueryBuilder().WithAll<Bullet, LocalTransform>().Build();
        var alienQuery = SystemAPI.QueryBuilder().WithAll<Alien, LocalTransform>().Build();
        
        var bulletEntities = bulletQuery.ToEntityArray(Allocator.Temp);
        var bulletTransforms = bulletQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
        
        var alienEntities = alienQuery.ToEntityArray(Allocator.Temp);
        var alienTransforms = alienQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
        
        const float collisionDistance = 1.5f; // Simple sphere collision radius
        const float collisionDistanceSq = collisionDistance * collisionDistance;
        
        // Check collisions between bullets and aliens
        for (int b = 0; b < bulletEntities.Length; b++)
        {
            var bulletPos = bulletTransforms[b].Position;
            bool bulletHit = false;
            
            for (int a = 0; a < alienEntities.Length; a++)
            {
                var alienPos = alienTransforms[a].Position;
                
                // Calculate distance squared for performance
                float distanceSq = math.distancesq(bulletPos, alienPos);
                
                if (distanceSq <= collisionDistanceSq)
                {
                    // Collision detected - destroy both bullet and alien
                    entityCommandBuffer.DestroyEntity(bulletEntities[b]);
                    entityCommandBuffer.DestroyEntity(alienEntities[a]);
                    
                    bulletHit = true;
                    break; // Bullet can only hit one alien
                }
            }
            
            if (bulletHit)
                break; // No need to check more bullets for this frame
        }
        
        // Clean up temporary arrays
        bulletEntities.Dispose();
        bulletTransforms.Dispose();
        alienEntities.Dispose();
        alienTransforms.Dispose();
        
        // Execute commands
        entityCommandBuffer.Playback(state.EntityManager);
        entityCommandBuffer.Dispose();
    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
}