using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Collider = Unity.Physics.Collider;

[RequiresEntityConversion]
public class UnitSpawnerProxy : MonoBehaviour, IConvertGameObjectToEntity
{
    public GameObject Prefab;
    public int CountX, CountY;
    GameObjectConversionSystem ConversionSystem;
    Entity entity;
    EntityManager entityManager;
    // Referenced prefabs have to be declared so that the conversion system knows about them ahead of time
    // public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    // {
        
    //     referencedPrefabs.Add(Prefab);
    // }
    void Start()
    {
    }
    // Lets you convert the editor data representation to the entity optimal runtime representation
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        Entity sourceEntity = GameObjectConversionUtility.ConvertGameObjectHierarchy(Prefab, World.Active);
        // entityManager = World.Active.EntityManager;
        entityManager = dstManager;
        BlobAssetReference<Collider> sourceCollider = entityManager.GetComponentData<PhysicsCollider>(sourceEntity).Value;
        for (var x = 0; x < CountX; x++)
        {
            for (var y = 0; y < CountY; y++)
            {
                Entity instance = entityManager.Instantiate(sourceEntity);
                var pos = entityManager.GetComponentData<LocalToWorld>(sourceEntity).Value;
                var position = math.transform(pos,
                    new float3(x * 2, 0, y * 2));
                entityManager.SetComponentData<Translation>(instance, new Translation { Value = position });
                entityManager.SetComponentData<PhysicsCollider>(instance, new PhysicsCollider { Value = sourceCollider });
                entityManager.AddComponent(instance, typeof(Grounded));
                entityManager.AddComponent(instance, typeof(PlayerInput));
                entityManager.AddComponent(instance, typeof(UnitNavAgent));
                
            }
        }
        entityManager.DestroyEntity(entity);
        // ConversionSystem = conversionSystem;
        // var spawnerData = new UnitSpawner
        // {
        //     // The referenced prefab will b e converted due to DeclareReferencedPrefabs.
        //     // So here we simply map the game object to an entity reference to that prefab.
        //     Prefab = conversionSystem.GetPrimaryEntity(Prefab),
        //     CountX = CountX,
        //     CountY = CountY
        // };
        // dstManager.AddComponentData(entity, spawnerData);
    }
// Referenced prefavsa have to be declared so that conversion system klnows about them ahead of time

}
// public class UnitSpawnerComponent : SharedComponentDataProxy<UnitSpawnerProxy>   
// {

// }