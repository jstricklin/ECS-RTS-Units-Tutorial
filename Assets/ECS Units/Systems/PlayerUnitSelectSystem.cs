using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;

public class PlayerUnitSelectSystem : JobComponentSystem
{
    EntityCommandBufferSystem m_EntityCommandBufferSystem;
    [ReadOnly]
    BuildPhysicsWorld m_BuildPhysicsWorldSystem;
    protected override void OnCreate()
    {
        Debug.Log("creating system.");
    }

    struct PlayerUnitSelectJob : IJobForEachWithEntity<PlayerInput>
    {
        [ReadOnly]
        public EntityCommandBuffer.Concurrent CommandBuffer;
        [ReadOnly]
        public ComponentDataFromEntity<PlayerUnitSelect> Selected;
        [ReadOnly]
        public PhysicsWorld physicsWorld;
        public CollisionFilter filter;
        public RaycastInput raycastInput;
        public bool hit;
        public Unity.Physics.RaycastHit hitData;
        public void Execute(Entity entity, int index, [ReadOnly] ref PlayerInput input)
        {
            if (input.LeftClick)
            {
                // var rayInput = new RaycastInput();
                // rayInput.Start = Camera.main.ScreenPointToRay(Input.mousePosition).origin;
                // rayInput.End = Camera.main.ScreenPointToRay(Input.mousePosition).direction * 1000f;
                // rayInput.Filter = filter;
                hit = physicsWorld.CollisionWorld.CastRay(raycastInput, out hitData);
                
                if (Selected.Exists(entity))
                {
                    CommandBuffer.RemoveComponent<PlayerUnitSelect>(index, entity);
                    CommandBuffer.AddComponent(index, entity, new Deselecting());
                } 
                if (hit) {
                    Entity e = physicsWorld.Bodies[hitData.RigidBodyIndex].Entity;
                    CommandBuffer.AddComponent(index, e, new PlayerUnitSelect());
                    CommandBuffer.AddComponent(index, e, new Selecting());
                }
            }
            
        }
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        m_EntityCommandBufferSystem = World.GetOrCreateSystem<EntityCommandBufferSystem>();
        m_BuildPhysicsWorldSystem = World.Active.GetExistingSystem<BuildPhysicsWorld>();
        RaycastInput _raycastInput = new RaycastInput();
        _raycastInput.Start = Camera.main.ScreenPointToRay(Input.mousePosition).origin;
        _raycastInput.End = Camera.main.ScreenPointToRay(Input.mousePosition).direction * 1000f;
        _raycastInput.Filter = new CollisionFilter {
                BelongsTo = ~0u,
                // BelongsTo = (uint)(1 << 1),
                CollidesWith = (uint)(1 << 2),
            };
        var job = new PlayerUnitSelectJob 
        {
            CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer().ToConcurrent(),
            Selected = GetComponentDataFromEntity<PlayerUnitSelect>(),
            physicsWorld = m_BuildPhysicsWorldSystem.PhysicsWorld,
            raycastInput = _raycastInput,
            hitData = new Unity.Physics.RaycastHit(),
        };
        inputDeps = JobHandle.CombineDependencies(inputDeps, m_BuildPhysicsWorldSystem.FinalJobHandle);
        // m_EntityCommandBufferSystem.AddJobHandleForProducer(job);
        var handle = job.Schedule(this, inputDeps);
        handle.Complete();
        // m_EntityCommandBufferSystem.AddJobHandleForProducer(handle);
        // return job;
        return handle;
    }

}