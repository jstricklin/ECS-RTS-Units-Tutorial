using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public struct PlayerInput : IComponentData
{
    public BlittableBool LeftClick;
    public BlittableBool RightClick;
}
