using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using UnityEngine;

namespace Ships.ECS
{
    [UpdateBefore(typeof(TargetSystem))]
    public class MovementSystem : JobComponentSystem
    {
        
        [BurstCompile]
        struct MovementJob : IJobProcessComponentData<Position, Rotation, MoveSpeed, Target>
        {
            public float topBound;
            public float bottonBound;
            public float deltaTime;
            public float rotationSpeed;
            public float team;
            public bool isTarget;

            public void Execute(ref Position pos, ref Rotation rot, [ReadOnly] ref MoveSpeed speed, [ReadOnly] ref Target target)
            {
                float3 value = pos.Value;
                Quaternion rotVal = rot.Value;
                
                value += deltaTime * speed.Value * math.forward(rot.Value);
                
                var dir = math.normalize(target.Value - value);
                var look = Quaternion.LookRotation(dir);
                rotVal = Quaternion.Slerp(rotVal, look, deltaTime * rotationSpeed);

                rot.Value = rotVal;
                pos.Value = value;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            MovementJob moveJob = new MovementJob
            {
                topBound = GameManager.GM.topBound,
                bottonBound = GameManager.GM.bottonBound,
                deltaTime = Time.deltaTime,
                rotationSpeed = GameManager.GM.rotationSpeed
            };

            JobHandle moveHandle = moveJob.Schedule(this, inputDeps);

            return moveHandle;
        }
    }
}
