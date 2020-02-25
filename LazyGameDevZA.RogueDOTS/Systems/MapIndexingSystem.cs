using LazyGameDevZA.RogueDOTS.Components;
using Unity.Entities;

namespace LazyGameDevZA.RogueDOTS.Systems
{
    [AlwaysSynchronizeSystem]
    [UpdateInGroup(typeof(GameSystemsGroup))]
    [UpdateAfter(typeof(MonsterAISystem))]
    public class MapIndexingSystem: SystemBase
    {
        private EntityQuery mapQuery;

        protected override void OnCreate()
        {
            this.mapQuery = this.EntityManager.CreateMapEntityQuery(
                blockedTilesWriteAccess: true,
                visibleTilesWriteAccess: true,
                tileContentWriteAccess: true);
            this.RequireForUpdate(this.mapQuery);
        }

        protected override void OnUpdate()
        {
            var mapEntity = this.mapQuery.GetSingletonEntity();
            var map = this.EntityManager.GetMap(mapEntity);
            var mapBlockedTiles = map.BlockedTiles;
            var mapTileContents = map.TileContents;
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

                    if(blockers.HasComponent(entity))
                    {
                        mapBlockedTiles[idx] = true;
                    }

                    var contents = mapTileContents[idx];
                    contents.Add(entity);
                    mapTileContents[idx] = contents;
                })
                .WithoutBurst()
                .Run();
        }
    }
}
