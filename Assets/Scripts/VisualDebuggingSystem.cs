using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using UnityEngine;
using Unity.Collections;

/// <summary>
/// System that provides visual debugging by drawing Gizmos for entities
/// This system helps visualize entities without needing full rendering setup
/// </summary>
[BurstCompile]
[UpdateInGroup(typeof(PresentationSystemGroup))]
public partial struct VisualDebuggingSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // This system runs regardless of entity requirements for debugging
    }
    
    public void OnUpdate(ref SystemState state)
    {
        // This system needs to run in main thread for Debug drawing
        if (!Application.isPlaying) return;
        
        // Draw player entities
        foreach (var (transform, player) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<Player>>())
        {
            Debug.DrawRay(transform.ValueRO.Position, Vector3.up * 2f, Color.green, 0.1f);
            Debug.DrawRay(transform.ValueRO.Position, Vector3.forward * 1f, Color.blue, 0.1f);
            
            // Draw a simple cross to represent the player
            var pos = transform.ValueRO.Position;
            Debug.DrawLine(pos + new float3(-0.5f, 0, 0), pos + new float3(0.5f, 0, 0), Color.green, 0.1f);
            Debug.DrawLine(pos + new float3(0, 0, -0.5f), pos + new float3(0, 0, 0.5f), Color.green, 0.1f);
        }
        
        // Draw alien entities
        foreach (var (transform, alien) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<Alien>>())
        {
            Debug.DrawRay(transform.ValueRO.Position, Vector3.up * 1f, Color.red, 0.1f);
            
            // Draw alien as a diamond shape
            var pos = transform.ValueRO.Position;
            Debug.DrawLine(pos + new float3(-0.3f, 0, 0), pos + new float3(0, 0, 0.3f), Color.red, 0.1f);
            Debug.DrawLine(pos + new float3(0, 0, 0.3f), pos + new float3(0.3f, 0, 0), Color.red, 0.1f);
            Debug.DrawLine(pos + new float3(0.3f, 0, 0), pos + new float3(0, 0, -0.3f), Color.red, 0.1f);
            Debug.DrawLine(pos + new float3(0, 0, -0.3f), pos + new float3(-0.3f, 0, 0), Color.red, 0.1f);
        }
        
        // Draw bullet entities
        foreach (var (transform, bullet) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<Bullet>>())
        {
            Debug.DrawRay(transform.ValueRO.Position, Vector3.up * 0.5f, Color.yellow, 0.1f);
            
            // Draw bullet trajectory
            var pos = transform.ValueRO.Position;
            var endPos = pos + bullet.ValueRO.Direction * 0.5f;
            Debug.DrawLine(pos, endPos, Color.yellow, 0.1f);
        }
        
        // Draw game bounds
        var boundsQuery = SystemAPI.QueryBuilder().WithAll<GameBounds>().Build();
        if (boundsQuery.CalculateEntityCount() > 0)
        {
            var bounds = SystemAPI.GetSingleton<GameBounds>();
            
            // Draw boundary lines
            var min = bounds.Min;
            var max = bounds.Max;
            
            // Bottom rectangle
            Debug.DrawLine(new float3(min.x, min.y, min.z), new float3(max.x, min.y, min.z), Color.white, 0.1f);
            Debug.DrawLine(new float3(max.x, min.y, min.z), new float3(max.x, min.y, max.z), Color.white, 0.1f);
            Debug.DrawLine(new float3(max.x, min.y, max.z), new float3(min.x, min.y, max.z), Color.white, 0.1f);
            Debug.DrawLine(new float3(min.x, min.y, max.z), new float3(min.x, min.y, min.z), Color.white, 0.1f);
        }
    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
}