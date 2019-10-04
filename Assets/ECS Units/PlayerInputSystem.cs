using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

public class PlayerInputSystem : JobComponentSystem
{
    struct PlayerInputJob : IJobForEach<PlayerInput>
    {
        public BlittableBool leftClick;
        public BlittableBool rightClick;
        public float3 mousePosition;

        public void Execute(ref PlayerInput data)
        {
            data.LeftClick = leftClick;
            data.RightClick = rightClick;
            data.MousePosition = mousePosition;
        }
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var mousePos = Input.mousePosition;
        var ray = Camera.main.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            mousePos = new float3(hit.point.x, 0, hit.point.z);
        }
        var job = new PlayerInputJob
        {
            leftClick = Input.GetMouseButtonDown(0),
            rightClick = Input.GetMouseButtonDown(1),
            mousePosition = Input.mousePosition
        };
        return job.Schedule(this, inputDeps);
    }

}
