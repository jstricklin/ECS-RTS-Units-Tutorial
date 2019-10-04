﻿using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

// [RequiresEntityConversion]
public class UnitSpawnerProxy : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    public GameObject Prefab;
    public int CountX, CountY;
    // Referenced prefabs have to be declared so that the conversion system knows about them ahead of time
    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(Prefab);
    }

    // Lets you convert the editor data representation to the entity optimal runtime representation
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var spawnerData = new UnitSpawner
        {
            // The referenced prefab will be converted due to DeclareReferencedPrefabs.
            // So here we simply map the game object to an entity reference to that prefab.
            Prefab = conversionSystem.GetPrimaryEntity(Prefab),
            CountX = CountX,
            CountY = CountY
        };
        dstManager.AddComponentData(entity, spawnerData);
    }
// Referenced prefavsa have to be declared so that conversion system klnows about them ahead of time

}
// public class UnitSpawnerComponent : SharedComponentDataProxy<UnitSpawnerProxy>   
// {

// }