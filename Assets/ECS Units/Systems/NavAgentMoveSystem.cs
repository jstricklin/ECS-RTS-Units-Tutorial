using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;

public class NavAgentMoveSystem : JobComponentSystem
{
    public struct NavAgentMoveJob : IJobForEach<Translation, UnitNavAgent>
    {
        public float dT;

        public void Execute(ref Translation position, [ReadOnly] ref UnitNavAgent agent)
        {
            float distance = math.distance(agent.finalDestination, position.Value);
            float3 direction = math.normalize(agent.finalDestination - position.Value);
            float speed = 5;
            if (!(distance < 0.5f) && agent.agentStatus == NavAgentStatus.Moving)
            {
                position.Value += direction * speed * dT;
            } 
        }
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var job = new NavAgentMoveJob
        {
            dT = Time.deltaTime,
        };
        return job.Schedule(this, inputDeps);
    }
}
