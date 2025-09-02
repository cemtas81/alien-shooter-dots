using UnityEngine;
using Unity.Entities;

/// <summary>
/// Bootstrap MonoBehaviour that initializes the DOTS world and systems
/// </summary>
public class Bootstrap : MonoBehaviour
{
    void Start()
    {
        // Set target frame rate
        Application.targetFrameRate = 60;
        
        // Initialize default world if not already created
        if (World.DefaultGameObjectInjectionWorld == null)
        {
            DefaultWorldInitialization.Initialize("Default World");
        }
        
        var world = World.DefaultGameObjectInjectionWorld;
        if (world != null)
        {
            var entityManager = world.EntityManager;
            
            // Create game bounds entity
            var boundsEntity = entityManager.CreateEntity();
            entityManager.AddComponentData(boundsEntity, new GameBounds
            {
                Min = new Unity.Mathematics.float3(-25, 0, -25),
                Max = new Unity.Mathematics.float3(25, 0, 25)
            });
            
            Debug.Log("Alien Shooter DOTS initialized successfully!");
        }
    }
}