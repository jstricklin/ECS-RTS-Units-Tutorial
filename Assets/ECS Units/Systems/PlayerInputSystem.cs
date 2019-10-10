using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Burst;

public class PlayerInputSystem : JobComponentSystem
{
    [BurstCompile]
    struct PlayerInputJob : IJobForEach<PlayerInput>
    {
        public BlittableBool leftClick;
        public BlittableBool rightClick;

        public void Execute(ref PlayerInput data)
        {
            data.LeftClick = leftClick;
            data.RightClick = rightClick;
        }
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new PlayerInputJob
        {
            leftClick = Input.GetMouseButtonDown(0),
            rightClick = Input.GetMouseButtonDown(1),
        };
        return job.Schedule(this, inputDeps);
    }

}
