using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Jobs;
using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Collections;

public class TestJob : JobComponentSystem
{
    // [RequireComponentTag(typeof(Grounded), typeof(Selecting))]
    [RequireComponentTag(typeof(Grounded))]
    struct MoveJob : IJobForEach<PhysicsVelocity, Translation>
    {
        public float horizontal, vertical;
        // Physics World MUST be readonly or ERRORS
        [ReadOnly]
        public PhysicsWorld physicsWorld;
        public CollisionFilter filter;
        public bool space, hit;
        [BurstCompile]
        public void Execute(ref PhysicsVelocity velocity, ref Translation translation)
        {
            var rayInput = new RaycastInput();
            // rayInput.Start = translation.Value - new float3(0, 1, 0);
            // Debug.Log("creating ray...");
            rayInput.Start = translation.Value;
            rayInput.End = translation.Value - 1.1f * new float3(0, 1, 0);
            rayInput.Filter = filter;
            hit = physicsWorld.CollisionWorld.CastRay(rayInput);
            velocity.Linear = new float3(3 * horizontal, velocity.Linear.y, 3 * vertical);
            if (space && hit)
            {
                velocity.Linear = new float3(velocity.Linear.x, 6, velocity.Linear.z);
            }
        }
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var physicsWorld = World.Active.GetExistingSystem<Unity.Physics.Systems.BuildPhysicsWorld>();
        // Debug.Log("creating job...");
        var job = new MoveJob
        {
            horizontal = Input.GetAxis("Horizontal"),
            vertical = Input.GetAxis("Vertical"),
            space = Input.GetKeyDown(KeyCode.Space),
            physicsWorld = physicsWorld.PhysicsWorld,
            filter = new CollisionFilter{
                BelongsTo = ~0u,
                CollidesWith = (uint)(1 << 0),
            },
        };
        var newDeps = JobHandle.CombineDependencies(inputDeps, physicsWorld.FinalJobHandle);
        var handle = job.Schedule(this, newDeps);
        handle.Complete();
        return handle;
    }

}
