using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Rendering;
using UnityEngine;
using Unity.Burst;
using Unity.Collections;

/// <summary>
/// System that creates render meshes for entities using Entities Graphics
/// </summary>
[BurstCompile]
[UpdateInGroup(typeof(InitializationSystemGroup))]
[UpdateAfter(typeof(GameInitializationSystem))]
public partial struct RenderingSetupSystem : ISystem
{
    private bool _initialized;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _initialized = false;
    }
    
    public void OnUpdate(ref SystemState state)
    {
        if (_initialized) return;
        
        var entityManager = state.EntityManager;
        
        // Create materials and meshes on main thread
        var playerMaterial = CreateMaterial(Color.green);
        var alienMaterial = CreateMaterial(Color.red);
        var bulletMaterial = CreateMaterial(Color.yellow);
        
        var cubeMesh = CreateCubeMesh();
        var sphereMesh = CreateSphereMesh();
        
        // Set up rendering for player entities
        foreach (var (transform, player, entity) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<Player>>().WithEntityAccess())
        {
            var renderMeshArray = new RenderMeshArray(new Material[] { playerMaterial }, new Mesh[] { cubeMesh });
            var renderMeshDescription = new RenderMeshDescription
            {
                FilterSettings = RenderFilterSettings.Default,
                LightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off
            };
            
            RenderMeshUtility.AddComponents(entity, entityManager, renderMeshDescription, renderMeshArray, MaterialMeshIndex.Default);
        }
        
        // Set up rendering for existing aliens (if any)
        foreach (var (transform, alien, entity) in SystemAPI.Query<RefRO<LocalTransform>, RefRO<Alien>>().WithEntityAccess())
        {
            var renderMeshArray = new RenderMeshArray(new Material[] { alienMaterial }, new Mesh[] { sphereMesh });
            var renderMeshDescription = new RenderMeshDescription
            {
                FilterSettings = RenderFilterSettings.Default,
                LightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off
            };
            
            RenderMeshUtility.AddComponents(entity, entityManager, renderMeshDescription, renderMeshArray, MaterialMeshIndex.Default);
        }
        
        // Store materials and meshes for future spawns
        var renderDataEntity = entityManager.CreateEntity();
        entityManager.AddComponentData(renderDataEntity, new RenderingData
        {
            PlayerMaterial = playerMaterial,
            AlienMaterial = alienMaterial,
            BulletMaterial = bulletMaterial,
            CubeMesh = cubeMesh,
            SphereMesh = sphereMesh
        });
        
        _initialized = true;
    }
    
    private Material CreateMaterial(Color color)
    {
        var material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        material.color = color;
        material.SetFloat("_Metallic", 0f);
        material.SetFloat("_Smoothness", 0.2f);
        return material;
    }
    
    private Mesh CreateCubeMesh()
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        var mesh = go.GetComponent<MeshFilter>().sharedMesh;
        Object.DestroyImmediate(go);
        return mesh;
    }
    
    private Mesh CreateSphereMesh()
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        var mesh = go.GetComponent<MeshFilter>().sharedMesh;
        Object.DestroyImmediate(go);
        return mesh;
    }
    
    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {
    }
}

/// <summary>
/// Component to store rendering data for runtime spawning
/// </summary>
public struct RenderingData : IComponentData
{
    public Material PlayerMaterial;
    public Material AlienMaterial;
    public Material BulletMaterial;
    public Mesh CubeMesh;
    public Mesh SphereMesh;
}