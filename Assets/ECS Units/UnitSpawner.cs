using Unity.Entities;

// ReSharper disable once InconsistentNaming
public struct UnitSpawner : IComponentData
{
    public int CountX, CountY;
    public Entity Prefab;
}
