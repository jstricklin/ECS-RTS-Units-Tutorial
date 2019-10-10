using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;

public class DeselectSystem : ComponentSystem
{
    EntityQuery m_highlights;
    
    protected override void OnCreate()
    {
        m_highlights = GetEntityQuery(typeof(Highlight));
    }
    protected override void OnUpdate()
    {
        using (var highlights = m_highlights.ToEntityArray(Allocator.TempJob))
        {
            foreach (var highlight in highlights)
            {
                var parent = EntityManager.GetComponentData<Parent>(highlight).Value;
                if (EntityManager.HasComponent<Deselecting>(parent))
                {
                    EntityManager.RemoveComponent<Deselecting>(parent);
                    EntityManager.DestroyEntity(highlight);
                }
            }
        }
    }
}
