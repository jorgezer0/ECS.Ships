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

        struct TeamA
        {
            public readonly int Length;
            [ReadOnly] public ComponentDataArray<Position> position;
            public ComponentDataArray<Target> target;
            [ReadOnly] public ComponentDataArray<Team_A> team;
        }

        [Inject] TeamA m_teamA;

        struct TeamB
        {
            public readonly int Length;
            [ReadOnly] public ComponentDataArray<Position> position;
            public ComponentDataArray<Target> target;
            [ReadOnly] public ComponentDataArray<Team_B> team;
        }

        [Inject] TeamB m_teamB;

        [BurstCompile]
        struct GetTargetsJob : IJobParallelFor
        {
            [ReadOnly] public ComponentDataArray<Position> position;

            [NativeDisableParallelForRestriction]
            public ComponentDataArray<Target> target;

            public void Execute (int i)
            {
                var t = target[i];
                t.Value = position[i].Value;
                target[i] = t;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var teamAtoTeamB = new GetTargetsJob
            {
                position = m_teamB.position,
                target = m_teamA.target
            }.Schedule(m_teamA.Length, 32, inputDeps);

            var teamBtoTeamA = new GetTargetsJob
            {
                position = m_teamA.position,
                target = m_teamB.target
            }.Schedule(m_teamB.Length, 32, teamAtoTeamB);

            return teamBtoTeamA;
        }
    }
}
