using LazyGameDevZA.RogueDOTS.Components;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Profiling;
using static LazyGameDevZA.RogueDOTS.Toolkit.FOV;

namespace LazyGameDevZA.RogueDOTS.Systems
{
    [AlwaysSynchronizeSystem]
    [UpdateInGroup(typeof(GameSystemsGroup))]
    public class VisibilitySystem: JobComponentSystem
    {
        private EntityQuery mapQuery;
        
        
        protected override void OnCreate()
        {
            this.mapQuery = this.EntityManager.CreateMapEntityQuery(revealedTilesWriteAccess: true, true);
            this.RequireForUpdate(this.mapQuery);
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var mapEntity = this.mapQuery.GetSingletonEntity();
            var map = this.EntityManager.GetMap(mapEntity);
            var players = this.GetComponentDataFromEntity<Player>();

            var mapTiles = map.Tiles;
            var revealedTiles = map.RevealedTiles;
            var mapVisibleTiles = map.VisibleTiles;
            
            this.Entities.WithName($"{nameof(VisibilitySystem)}_CalculateEntitiesVisibility")
                .ForEach((ref DynamicBuffer<VisibleTilePosition> visibleTiles, ref ViewshedData viewshedData, in Entity entity, in Position position) =>
                {
                    visibleTiles.Clear();
                    visibleTiles.Reinterpret<int2>().CopyFrom(FieldOfView(position.Value, viewshedData.Range, map));
                    var _ = mapTiles;

                    for(int i = 0; i < visibleTiles.Length; i++)
                    {
                        var p = visibleTiles[i].Value;
                        if(p.x >= 0 && p.x <= map.Width - 1 && p.y >= 0 && p.y <= map.Width - 1)
                        {
                            continue;
                        }

                        visibleTiles.RemoveAt(i);
                        i--;
                    }

                    if(players.HasComponent(entity))
                    {
                        int2 pos;
                        
                        for(int i = 0; i < mapVisibleTiles.Length; i++)
                        {
                            mapVisibleTiles[i] = false;
                        }
                        
                        for(int i = 0; i < visibleTiles.Length; i++)
                        {
                            pos = visibleTiles[i].Value;
                            var idx = map.xy_idx(pos.x, pos.y);
                            revealedTiles[idx] = true;
                            mapVisibleTiles[idx] = true;
                        }
                    }
                })
                .WithChangeFilter<Position>()
                .WithReadOnly(mapTiles)
                .WithNativeDisableParallelForRestriction(revealedTiles)
                .WithNativeDisableParallelForRestriction(mapVisibleTiles)
                .Run();

            return default;
        }
    }
}
