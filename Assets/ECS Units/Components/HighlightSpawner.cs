using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct HighlightSpawner : IComponentData 
{
    public Entity Prefab;
}
