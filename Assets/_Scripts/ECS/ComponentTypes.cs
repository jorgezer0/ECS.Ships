using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

namespace Ships.ECS
{
    public struct Team_A : IComponentData { }
    public struct Team_B : IComponentData { }

    public struct Projectile : IComponentData { }
}