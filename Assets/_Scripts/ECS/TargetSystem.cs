using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using UnityEngine;

namespace Ships.ECS
{
    public class TargetSystem : JobComponentSystem
    {
        
        struct Team
        {
            [ReadOnly] public Translation position;
        }

        [BurstCompile(CompileSynchronously = true)]
        struct GetTargetsJobA : IJobForEachWithEntity<Translation, Rotation, Target, Team_A>
        {
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Translation> enemyArray;

            public void Execute(Entity entity, int i, [ReadOnly] ref Translation translation, [ReadOnly] ref Rotation rotation, ref Target target, [ReadOnly] ref Team_A team_A)
            {
                var lastDot = 0f;
                for (int j = 0; j < enemyArray.Length; j++)
                {
                    if (lastDot > 0.9999) break;

                    var t = enemyArray[j].Value;
                    var dot = math.dot(math.forward(rotation.Value), math.normalize(t - translation.Value));

                    if (dot > lastDot)
                    {
                        target.Value = t;
                        lastDot = dot;
                    }

                }
            }
        }

        [BurstCompile(CompileSynchronously = true)]
        struct GetTargetsJobB : IJobForEachWithEntity<Translation, Rotation, Target, Team_B>
        {
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Translation> enemyArray;

            public void Execute(Entity entity, int i, [ReadOnly] ref Translation translation, [ReadOnly] ref Rotation rotation, ref Target target, [ReadOnly] ref Team_B team_B)
            {
                var lastDot = 0f;
                for (int j = 0; j < enemyArray.Length; j++)
                {
                    if (lastDot > 0.9999) break;

                    var t = enemyArray[j].Value;
                    var dot = math.dot(math.forward(rotation.Value), math.normalize(t - translation.Value));

                    if (dot > lastDot)
                    {
                        target.Value = t;
                        lastDot = dot;
                    }

                }
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            EntityQuery teamA = GetEntityQuery(typeof(Team_A), ComponentType.ReadOnly<Translation>(), typeof(Target));
            EntityQuery teamB = GetEntityQuery(typeof(Team_B), ComponentType.ReadOnly<Translation>(), typeof(Target));
            
            NativeArray<Translation> teamATranslationArray = teamA.ToComponentDataArray<Translation>(Allocator.TempJob);
            NativeArray<Translation> teamBTranslationArray = teamB.ToComponentDataArray<Translation>(Allocator.TempJob);

            var teamAtoTeamB = new GetTargetsJobA
            {
                enemyArray = teamBTranslationArray
            };
            JobHandle jobHandle = teamAtoTeamB.Schedule(this, inputDeps);

            var teamBtoTeamA = new GetTargetsJobB
            {
                enemyArray = teamATranslationArray
            }.Schedule(this, jobHandle);
            
            return teamBtoTeamA;
        }
    }
}
