using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class SelectingSystem : ComponentSystem
{
    EntityQuery m_highlights;
    EntityQuery m_selectedUnits;
    protected override void OnCreate()
    {
        m_highlights = GetEntityQuery(typeof(HighlightSpawner));
        m_selectedUnits = GetEntityQuery(typeof(Selecting));
    }
    protected override void OnUpdate()
    {
        //Get all selected units
        using (var selectedUnits = m_selectedUnits.ToEntityArray(Allocator.TempJob))
        using (var highlights = m_highlights.ToEntityArray(Allocator.TempJob))
        {
            // better way to do this? find.
            var highlight = highlights[0];
            var prefab = EntityManager.GetComponentData<HighlightSpawner>(highlight).Prefab;
            foreach(var selectedUnit in selectedUnits)
            {
                // remove component from unit so system doesnt constantly run
                EntityManager.RemoveComponent<Selecting>(selectedUnit);

                //get prefab from spawner and set translation to get a LocalWorld
                var entity = EntityManager.Instantiate(prefab);
                EntityManager.AddComponent(entity, typeof(Highlight));
                // for chld to succesfully have a parent it needs:
                // 1 LocaToWorld (translation or rotation)
                // 2 LocalToParent
                // 3 Parent
                EntityManager.SetComponentData(entity, new Translation { Value = new float3(0, -0.5f, 0) });
                var localParent = EntityManager.GetComponentData<LocalToWorld>(selectedUnit).Value;
                EntityManager.AddComponentData(entity, new LocalToParent {Value = localParent });
                EntityManager.AddComponentData(entity, new Parent { Value = selectedUnit });
                Debug.Log("adding highlight");
            }
        }
    }
}
