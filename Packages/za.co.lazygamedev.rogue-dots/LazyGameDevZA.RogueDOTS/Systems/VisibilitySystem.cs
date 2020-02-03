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
        private static ProfilerMarker jobMarker = new ProfilerMarker($"{nameof(VisibilitySystem)}_CalculateEntitiesVisibility");
        
        private EntityQuery mapQuery;
        
        
        protected override void OnCreate()
        {
            this.mapQuery = this.EntityManager.CreateMapEntityQuery(revealedTilesWriteAccess: true, true);
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var mapEntity = this.mapQuery.GetSingletonEntity();
            var map = this.EntityManager.GetMap(mapEntity);
            var players = this.GetComponentDataFromEntity<Player>();
            var jobMarker = VisibilitySystem.jobMarker;

            this.Entities
                .ForEach((ref DynamicBuffer<VisibleTilePosition> visibleTiles, ref ViewshedData viewshedData, in Entity entity, in Position position) =>
                {
                    jobMarker.Begin();
                    if(viewshedData.Dirty)
                    {
                        viewshedData.Dirty = false;
                        visibleTiles.Clear();
                        visibleTiles.Reinterpret<int2>().CopyFrom(FieldOfView(position.Value, viewshedData.Range, map));

                        for(int i = 0; i < visibleTiles.Length; i++)
                        {
                            var p = visibleTiles[i].Value;
                            if(p.x > 0 && p.x < map.Width - 1 && p.y > 0 && p.y < map.Width - 1)
                            {
                                continue;
                            }

                            visibleTiles.RemoveAt(i);
                            i--;
                        }

                        if(players.HasComponent(entity))
                        {
                            int2 pos;
                            var revealedTiles = map.RevealedTiles;
                            var mapVisibleTiles = map.VisibleTiles;
                            
                            for(int i = 0; i < map.VisibleTiles.Length; i++)
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
                    }
                    jobMarker.End();
                })
                .Run();
            
            return default;
        }
    }
}
