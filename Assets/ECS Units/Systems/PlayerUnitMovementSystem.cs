using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Physics;
using Unity.Physics.Systems;

public class PlayerUnitMovementSystem : JobComponentSystem
{
    private BuildPhysicsWorld m_BuildPhysicsWorldSystem;
    public struct PlayerMovementJob : IJobForEach<PlayerInput, UnitNavAgent, PlayerUnitSelect>
    {
        
        public float dT;
        public float3 mousePos;
        [ReadOnly]
        public PhysicsWorld physicsWorld;
        public CollisionFilter filter;
        public RaycastInput raycastInput;
        public bool hit;
        public Unity.Physics.RaycastHit hitData;
        public void Execute([ReadOnly] ref PlayerInput playerInput, ref UnitNavAgent navAgent, [ReadOnly] ref PlayerUnitSelect selected)
        {
            if (playerInput.RightClick)
            {
                hit = physicsWorld.CollisionWorld.CastRay(raycastInput, out hitData);
                if (hit)
                {
                    var pos = hitData.Position;
                    pos.y = 0;
                    navAgent.finalDestination = pos;
                    navAgent.agentStatus = NavAgentStatus.Moving;
                }
            }
        }
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        m_BuildPhysicsWorldSystem = World.Active.GetExistingSystem<BuildPhysicsWorld>();
        RaycastInput _raycastInput = new RaycastInput();
        _raycastInput.Start = Camera.main.ScreenPointToRay(Input.mousePosition).origin;
        _raycastInput.End = Camera.main.ScreenPointToRay(Input.mousePosition).direction * 1000f;
        _raycastInput.Filter = new CollisionFilter {
                BelongsTo = ~0u,
                CollidesWith = (uint)(1 << 0),
            };
        var job = new PlayerMovementJob
        {
            physicsWorld = m_BuildPhysicsWorldSystem.PhysicsWorld,
            raycastInput = _raycastInput,
            hitData = new Unity.Physics.RaycastHit(),
        };
        inputDeps = JobHandle.CombineDependencies(inputDeps, m_BuildPhysicsWorldSystem.FinalJobHandle);
        var handle = job.Schedule(this, inputDeps);
        handle.Complete();
        return handle;
    }
}
