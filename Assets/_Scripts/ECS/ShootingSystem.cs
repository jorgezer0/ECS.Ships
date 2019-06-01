using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Burst;
using Unity.Rendering;
using UnityEngine;

namespace Ships.ECS
{
    public class ShootingSystem : ComponentSystem
    {

        private bool canShoot = true;
        
        protected override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                Entities.WithAll<Team_A>().ForEach((Entity entity, ref Translation translation, ref Rotation rotation) =>
                {
                    var newEntity = PostUpdateCommands.CreateEntity(GameManager.GM.projectileArchetype);
                    PostUpdateCommands.AddComponent(newEntity, new Translation { Value = translation.Value });
                    PostUpdateCommands.AddComponent(newEntity, new Rotation { Value = rotation.Value });
                    PostUpdateCommands.AddComponent(newEntity, new MoveSpeed { Value = 200 });
                    PostUpdateCommands.AddComponent(newEntity, new ChunkWorldRenderBounds {  });
                    PostUpdateCommands.AddComponent(newEntity, new WorldRenderBounds { });
                    PostUpdateCommands.AddComponent(newEntity, new LocalToWorld { });

                    PostUpdateCommands.AddSharedComponent(newEntity, new RenderMesh { mesh = GameManager.GM.projectileMesh, material = GameManager.GM.projectileMaterial });
                });
            }
        }
    }
}
