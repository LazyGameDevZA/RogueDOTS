using LazyGameDevZA.RogueDOTS.Components;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
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
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var playerPosition = this.GetSingleton<PlayerPosition>();

            var mapEntity = this.mapQuery.GetSingletonEntity();
            var map = this.EntityManager.GetMap(mapEntity);
            
            this.Entities.WithAll<Monster>()
                .ForEach((Entity entity, ref Position position, ref ViewshedData viewshedData, in DynamicBuffer<VisibleTilePosition> visibleTiles, in Name name) =>
                {
                    for(int i = 0; i < visibleTiles.Length; i++)
                    {
                        if(all(playerPosition.Value == visibleTiles[i].Value))
                        {
                            Debug.LogFormat("{0} shouts insults", name.Value);
                            var path = AStarSearch(map.xy_idx(position), map.xy_idx(playerPosition), map);
                            if(path.Success && path.Steps.Length > 1)
                            {
                                position = map.idx_xy(path.Steps[1]);
                            }
                            break;
                        }
                    }
                })
                .WithoutBurst()
                .Run();

            return default;
        }
    }
}
