using Unity.Entities;
using UnityEngine;

/// <summary>
/// Simple verification system to check that DOTS is working correctly
/// Reports the entity count and system status to the console
/// </summary>
[UpdateInGroup(typeof(PresentationSystemGroup))]
public partial struct GameStateDebugSystem : ISystem
{
    private float _lastReportTime;
    
    public void OnCreate(ref SystemState state)
    {
        _lastReportTime = 0f;
    }
    
    public void OnUpdate(ref SystemState state)
    {
        float currentTime = (float)SystemAPI.Time.ElapsedTime;
        
        // Report status every 5 seconds
        if (currentTime - _lastReportTime >= 5f)
        {
            var playerQuery = SystemAPI.QueryBuilder().WithAll<Player>().Build();
            var alienQuery = SystemAPI.QueryBuilder().WithAll<Alien>().Build();
            var bulletQuery = SystemAPI.QueryBuilder().WithAll<Bullet>().Build();
            
            int playerCount = playerQuery.CalculateEntityCount();
            int alienCount = alienQuery.CalculateEntityCount();
            int bulletCount = bulletQuery.CalculateEntityCount();
            
            Debug.Log($"Game Status - Players: {playerCount}, Aliens: {alienCount}, Bullets: {bulletCount}");
            
            _lastReportTime = currentTime;
        }
    }
    
    public void OnDestroy(ref SystemState state)
    {
    }
}