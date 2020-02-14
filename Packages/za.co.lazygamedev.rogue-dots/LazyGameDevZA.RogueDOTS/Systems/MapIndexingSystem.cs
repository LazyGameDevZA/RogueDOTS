using LazyGameDevZA.RogueDOTS.Components;
using Unity.Entities;
using Unity.Jobs;

namespace LazyGameDevZA.RogueDOTS.Systems
{
    [AlwaysSynchronizeSystem]
    [UpdateInGroup(typeof(GameSystemsGroup))]
    [UpdateAfter(typeof(MonsterAISystem))]
    public class MapIndexingSystem: JobComponentSystem
    {
        private EntityQuery mapQuery;

        protected override void OnCreate()
        {
            this.mapQuery = this.EntityManager.CreateMapEntityQuery(blockedTilesWriteAccess: true, visibleTilesWriteAccess: true);
            this.RequireForUpdate(this.mapQuery);
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var mapEntity = this.mapQuery.GetSingletonEntity();
            var map = this.EntityManager.GetMap(mapEntity);
            var mapBlockedTiles = map.BlockedTiles;
            var blockers = this.GetComponentDataFromEntity<BlocksTile>();

            this.Job.WithName($"{nameof(MapIndexingSystem)}_PopulateBlockedFromMapJob")
                .WithCode(() =>
                {
                    map.PopulateBlocked();
                    map.ClearContentIndex();
                })
                .Run();

            this.Entities.WithName($"{nameof(MapIndexingSystem)}_PopulateBlockedFromEntitiesJob")
                .ForEach((Entity entity, in Position position) =>
                {
                    var idx = map.xy_idx(position);
                    mapBlockedTiles[idx] = true;

                    if(blockers.HasComponent(entity))
                    {
                        mapBlockedTiles[idx] = true;
                    }

                    map.TileContents[idx].Add(entity);
                })
                .Run();
            
            return default;
        }
    }
}
