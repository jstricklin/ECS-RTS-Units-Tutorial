using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public struct Grounded : IComponentData { }
public struct Selecting : IComponentData { }
public struct Deselecting : IComponentData { }
public struct PlayerUnitSelect : IComponentData { }
public struct Highlight : IComponentData { }