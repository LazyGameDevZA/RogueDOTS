using LazyGameDevZA.RogueDOTS.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using static Unity.Mathematics.math;
using int2 = Unity.Mathematics.int2;

namespace LazyGameDevZA.RogueDOTS.Systems
{
    [AlwaysSynchronizeSystem]
    [UpdateInGroup(typeof(GameSystemsGroup))]
    [UpdateBefore(typeof(VisibilitySystem))]
    public class PlayerMoveSystem : SystemBase
    {
        private EntityQuery mapQuery;

        protected override void OnCreate()
        {
            this.mapQuery = this.EntityManager.CreateMapEntityQuery();

            this.RequireForUpdate(this.mapQuery);
            this.RequireSingletonForUpdate<Move>();
            
            this.EntityManager.CreateEntity(typeof(PlayerPosition));
            
            this.RequireSingletonForUpdate<RunState>();
        }

        protected override void OnUpdate()
        {
            var runState = this.GetSingleton<RunState>();

            if(runState != RunState.State.PlayerTurn)
            {
                return;
            }
            
            var mapEntity = this.mapQuery.GetSingletonEntity();
            var map = this.EntityManager.GetMap(mapEntity);

            int2 move = this.GetSingleton<Move>();
            
            var max = int2(map.Width - 1, map.Height - 1);
            var min = int2.zero;

            var combatStats = this.GetComponentDataFromEntity<CombatStats>();
            var ecb = new EntityCommandBuffer(Allocator.TempJob);

            this.Entities.WithAll<Player>()
                .ForEach((Entity entity, ref Position position, ref ViewshedData viewshedData) =>
                {
                    var destination = position + move;
                    var destinationIdx = map.xy_idx(destination.x, destination.y);
                    
                    var potentialTargets = map.TileContents[destinationIdx];

                    for(int i = 0; i < potentialTargets.Length; i++)
                    {
                        var potentialTarget = potentialTargets[i];

                        if(combatStats.HasComponent(potentialTarget))
                        {
                            ecb.AddComponent(entity, new WantsToMelee { Target = potentialTarget });
                            return;
                        }
                    }
                    
                    if(map.Tiles[destinationIdx] != TileType.Wall)
                    {
                        position = clamp(destination, min, max);
                        
                        this.SetSingleton(new PlayerPosition { Entity = entity, Value = position });
                    }
                })
                .WithoutBurst()
                .Run();
            
            ecb.Playback(this.EntityManager);
            ecb.Dispose();
        }
    }
}
