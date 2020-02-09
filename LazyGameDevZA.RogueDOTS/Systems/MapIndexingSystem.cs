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
            this.mapQuery = this.EntityManager.CreateMapEntityQuery(blockedTilesWriteAccess: true);
            this.RequireForUpdate(this.mapQuery);
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var mapEntity = this.mapQuery.GetSingletonEntity();
            var map = this.EntityManager.GetMap(mapEntity);
            var mapBlockedTiles = map.BlockedTiles;

            this.Job.WithName($"{nameof(MapIndexingSystem)}_PopulateBlockedFromMapJob")
                .WithCode(() => { map.PopulateBlocked(); })
                .Run();

            this.Entities.WithName($"{nameof(MapIndexingSystem)}_PopulateBlockedFromEntitiesJob")
                .WithAll<BlocksTile>()
                .ForEach((in Position position) =>
                {
                    var idx = map.xy_idx(position);
                    mapBlockedTiles[idx] = true;
                })
                .WithNativeDisableParallelForRestriction(mapBlockedTiles)
                .Run();
            
            return default;
        }
    }
}
