using LazyGameDevZA.RogueDOTS.Components;
using Unity.Entities;
using Unity.Jobs;
using static Unity.Mathematics.math;
using int2 = Unity.Mathematics.int2;

namespace LazyGameDevZA.RogueDOTS.Systems
{
    [AlwaysSynchronizeSystem]
    [UpdateInGroup(typeof(GameSystemsGroup))]
    public class MoveSystem : JobComponentSystem
    {
        private EntityQuery mapQuery;

        protected override void OnCreate()
        {
            this.mapQuery = this.EntityManager.CreateMapEntityQuery();

            this.RequireForUpdate(this.mapQuery);
            this.RequireSingletonForUpdate<Move>();
            
            this.EntityManager.CreateEntity(typeof(PlayerPosition));
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var mapEntity = this.mapQuery.GetSingletonEntity();
            var map = this.EntityManager.GetMap(mapEntity);

            int2 move = this.GetSingleton<Move>();
            
            var max = int2(map.Width - 1, map.Height - 1);
            var min = int2.zero;

            this.Entities.WithAll<Player>()
                .ForEach((ref Position position, ref ViewshedData viewshedData) =>
                {
                    var destination = position.Value + move;
                    var destinationIdx = map.xy_idx(destination.x, destination.y);
                    if(map.Tiles[destinationIdx] != TileType.Wall)
                    {
                        position.Value = clamp(destination, min, max);

                        this.SetSingleton<PlayerPosition>(position.Value);
                    }
                })
                .WithoutBurst()
                .Run();

            return default;
        }
    }
}
