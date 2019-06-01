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
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Team> enemyArray;

            public void Execute(Entity entity, int i, [ReadOnly] ref Translation translation, [ReadOnly] ref Rotation rotation, [ReadOnly] ref Target target, [ReadOnly] ref Team_A team_A)
            {
                var lastDot = 0f;
                for (int j = 0; j < enemyArray.Length; j++)
                {
                    if (lastDot > 0.9999) break;

                    var t = enemyArray[j].position;
                    var dot = math.dot(math.forward(rotation.Value), math.normalize(t.Value - translation.Value));

                    if (dot > lastDot)
                    {
                        target.Value = t.Value;
                        lastDot = dot;
                    }

                }
            }
        }

        [BurstCompile(CompileSynchronously = true)]
        struct GetTargetsJobB : IJobForEachWithEntity<Translation, Rotation, Target, Team_B>
        {
            [DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Team> enemyArray;

            public void Execute(Entity entity, int i, [ReadOnly] ref Translation translation, [ReadOnly] ref Rotation rotation, [ReadOnly] ref Target target, [ReadOnly] ref Team_B team_B)
            {
                var lastDot = 0f;
                for (int j = 0; j < enemyArray.Length; j++)
                {
                    if (lastDot > 0.9999) break;

                    var t = enemyArray[j].position;
                    var dot = math.dot(math.forward(rotation.Value), math.normalize(t.Value - translation.Value));

                    if (dot > lastDot)
                    {
                        target.Value = t.Value;
                        lastDot = dot;
                    }

                }
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            EntityQuery teamA = GetEntityQuery(typeof(Team_A), ComponentType.ReadOnly<Translation>(), typeof(Target));
            EntityQuery teamB = GetEntityQuery(typeof(Team_B), ComponentType.ReadOnly<Translation>(), typeof(Target));

            NativeArray<Entity> teamAEntities = teamA.ToEntityArray(Allocator.TempJob);
            NativeArray<Translation> teamATranslationArray = teamA.ToComponentDataArray<Translation>(Allocator.TempJob);
            NativeArray<Target> teamATargetArray = teamA.ToComponentDataArray<Target>(Allocator.TempJob);
            NativeArray<Entity> teamBEntities = teamB.ToEntityArray(Allocator.TempJob);
            NativeArray<Translation> teamBTranslationArray = teamB.ToComponentDataArray<Translation>(Allocator.TempJob);
            NativeArray<Target> teamBTargetArray = teamB.ToComponentDataArray<Target>(Allocator.TempJob);

            NativeArray<Team> teamAArray = new NativeArray<Team>(teamAEntities.Length, Allocator.TempJob);
            NativeArray<Team> teamBArray = new NativeArray<Team>(teamBEntities.Length, Allocator.TempJob);

            for (int i = 0; i < teamAArray.Length; i++)
            {
                teamAArray[i] = new Team
                {
                    position = teamATranslationArray[i]
                    
                };
                Debug.DrawLine(teamATranslationArray[i].Value, teamATargetArray[i].Value, Color.blue);
            }

            for (int i = 0; i < teamBArray.Length; i++)
            {
                teamBArray[i] = new Team
                {
                    position = teamBTranslationArray[i]
                };
                Debug.DrawLine(teamBTranslationArray[i].Value, teamBTargetArray[i].Value, Color.green);
            }

            teamAEntities.Dispose();
            teamATranslationArray.Dispose();
            teamATargetArray.Dispose();
            teamBEntities.Dispose();
            teamBTranslationArray.Dispose();
            teamBTargetArray.Dispose();

            var teamAtoTeamB = new GetTargetsJobA
            {
                enemyArray = teamBArray
            };
            JobHandle jobHandle = teamAtoTeamB.Schedule(this, inputDeps);

            var teamBtoTeamA = new GetTargetsJobB
            {
                enemyArray = teamAArray
            }.Schedule(this, jobHandle);


            return teamBtoTeamA;

        }
    }
}
