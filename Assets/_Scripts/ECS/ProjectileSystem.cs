using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using UnityEngine;

namespace Ships.ECS
{
    [UpdateAfter(typeof(ShootingSystem))]
    public class ProjectileSystem : JobComponentSystem
    {

        [BurstCompile]
        struct ProjectileMovementJob : IJobForEachWithEntity<Translation, Rotation, MoveSpeed, Projectile>
        {
            public float deltaTime;

            public void Execute(Entity entity, int index, ref Translation translation, [ReadOnly] ref Rotation rotation, [ReadOnly] ref MoveSpeed moveSpeed, [ReadOnly] ref Projectile projectile)
            {
                float3 value = translation.Value;
                Quaternion rotVal = rotation.Value;

                value += deltaTime * moveSpeed.Value * math.forward(rotation.Value);

                translation.Value = value;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            ProjectileMovementJob projectileMovementJob = new ProjectileMovementJob
            {
                deltaTime = Time.deltaTime
            };
            
            JobHandle jobHandle = projectileMovementJob.Schedule(this, inputDeps);

            return jobHandle;
        }
    }
}
