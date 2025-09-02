using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using UnityEngine;

/// <summary>
/// System that handles player shooting mechanics
/// </summary>
[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(PlayerMovementSystem))]
public partial struct PlayerShootingSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Player>();
    }
    
    public void OnUpdate(ref SystemState state)
    {
        // Get input (cannot burst compile this part due to Input usage)
        bool shootPressed = Input.GetKey(KeyCode.Space) || Input.GetButton("Jump");
        
        if (!shootPressed) return;
        
        float currentTime = (float)SystemAPI.Time.ElapsedTime;
        var entityManager = state.EntityManager;
        
        // Process shooting for each player
        foreach (var (transform, player, entity) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<Player>>().WithEntityAccess())
        {
            // Check cooldown
            if (currentTime - player.ValueRO.LastShootTime.x < player.ValueRO.ShootCooldown)
                continue;
                
            // Create bullet entity
            var bulletEntity = entityManager.CreateEntity();
            
            // Set bullet position slightly ahead of player
            float3 bulletPosition = transform.ValueRO.Position + new float3(0, 0, 1f);
            
            entityManager.AddComponentData(bulletEntity, LocalTransform.FromPosition(bulletPosition));
            entityManager.AddComponentData(bulletEntity, new Bullet
            {
                Speed = 25f,
                Direction = new float3(0, 0, 1), // Forward direction
                Lifetime = 0f,
                MaxLifetime = 3f
            });
            
            // Update player's last shoot time
            var updatedPlayer = player.ValueRO;
            updatedPlayer.LastShootTime = new float3(currentTime, 0, 0);
            player.ValueRW = updatedPlayer;
        }
    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
}