using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using UnityEngine;

/// <summary>
/// System that handles player movement based on WASD input
/// </summary>
[BurstCompile]
[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct PlayerMovementSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<Player>();
    }
    
    public void OnUpdate(ref SystemState state)
    {
        // Get input (cannot burst compile this part due to Input usage)
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        
        float deltaTime = SystemAPI.Time.DeltaTime;
        
        // Process player movement
        foreach (var (transform, player) in SystemAPI.Query<RefRW<LocalTransform>, RefRO<Player>>())
        {
            float3 moveDirection = new float3(horizontal, 0, vertical);
            
            if (math.lengthsq(moveDirection) > 0.01f)
            {
                moveDirection = math.normalize(moveDirection);
                float3 newPosition = transform.ValueRO.Position + moveDirection * player.ValueRO.Speed * deltaTime;
                
                // Clamp to game bounds
                var boundsQuery = SystemAPI.QueryBuilder().WithAll<GameBounds>().Build();
                if (boundsQuery.CalculateEntityCount() > 0)
                {
                    var bounds = SystemAPI.GetSingleton<GameBounds>();
                    newPosition = math.clamp(newPosition, bounds.Min, bounds.Max);
                }
                
                transform.ValueRW.Position = newPosition;
            }
        }
    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
}