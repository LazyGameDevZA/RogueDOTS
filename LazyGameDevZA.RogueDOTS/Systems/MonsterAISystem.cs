using LazyGameDevZA.RogueDOTS.Components;
using LazyGameDevZA.RogueDOTS.Toolkit.Collections;
using LazyGameDevZA.RogueDOTS.Toolkit.Geometry;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using static LazyGameDevZA.RogueDOTS.Toolkit.Pathfinding.AStar;
using static Unity.Mathematics.math;

namespace LazyGameDevZA.RogueDOTS.Systems
{
    [AlwaysSynchronizeSystem]
    [UpdateInGroup(typeof(GameSystemsGroup))]
    [UpdateAfter(typeof(VisibilitySystem))]
    public class MonsterAISystem: JobComponentSystem
    {
        private EntityQuery mapQuery;
        protected override void OnCreate()
        {
            this.RequireSingletonForUpdate<PlayerPosition>();
            
            this.mapQuery = this.EntityManager.CreateMapEntityQuery();
            this.RequireForUpdate(this.mapQuery);
            
            this.RequireSingletonForUpdate<RunState>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var runState = this.GetSingleton<RunState>();

            if(runState != RunState.State.MonsterTurn)
            {
                return default;
            }
            
            var playerPosition = this.GetSingleton<PlayerPosition>();

            var mapEntity = this.mapQuery.GetSingletonEntity();
            var map = this.EntityManager.GetMap(mapEntity);
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            
            this.Entities.WithAll<Monster>()
                .ForEach((Entity entity, ref Position position, in DynamicBuffer<VisibleTilePosition> visibleTiles, in Name name) =>
                {
                    var distance = DistanceAlg.Pythagoras.Distance2D(position, playerPosition);
                    if(distance < 1.5f)
                    {
                        ecb.AddComponent(entity, new WantsToMelee { Target = playerPosition.Entity });
                    }
                    else
                    {
                        for(int i = 0; i < visibleTiles.Length; i++)
                        {
                            if(all(playerPosition.Value == visibleTiles[i].Value))
                            {
                                var path = AStarSearch(map.xy_idx(position), map.xy_idx(playerPosition), map);
                                if(path.Success && path.Steps.Length > 1)
                                {
                                    position = map.idx_xy(path.Steps[1]);
                                }
                                break;
                            }
                        }
                    }
                })
                .WithoutBurst()
                .Run();

            ecb.Playback(this.EntityManager);
            ecb.Dispose();

            return default;
        }
    }
}
