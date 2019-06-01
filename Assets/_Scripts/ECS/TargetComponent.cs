using System;
using Unity.Entities;
using Unity.Mathematics;

namespace Ships.ECS
{
    [Serializable]
    public struct Target : IComponentData
    {
        public float3 Value;
    }

    public class TargetComponent : ComponentDataProxy<Target> { }
}