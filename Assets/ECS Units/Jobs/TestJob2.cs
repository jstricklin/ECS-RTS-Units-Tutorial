using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Burst;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Jobs;
using Unity.Collections;

public class TestJobSystem2 : JobComponentSystem
{
    struct TestJob2 : IJobForEachWithEntity<PlayerInput>
    {
        [ReadOnly]
        public PhysicsWorld physicsWorld;
        [BurstCompile]
        public void Execute(Entity entity, int index, ref PlayerInput c0)
        {
            // Debug.Log("test job 2 running.");
        }
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var _physicsWorld = World.Active.GetExistingSystem<BuildPhysicsWorld>();
        var job = new TestJob2 
        {
            physicsWorld = _physicsWorld.PhysicsWorld,
        };
        var newDeps = JobHandle.CombineDependencies(inputDeps, _physicsWorld.FinalJobHandle);
        var handle = job.Schedule(this, newDeps);
        handle.Complete();
        return handle;
    }
}
