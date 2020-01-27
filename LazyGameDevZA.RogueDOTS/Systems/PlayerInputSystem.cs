using LazyGameDevZA.RogueDOTS.Components;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using static Unity.Mathematics.math;
using int2 = Unity.Mathematics.int2;

namespace LazyGameDevZA.RogueDOTS.Systems
{
    [AlwaysSynchronizeSystem]
    public class PlayerInputSystem : JobComponentSystem
    {
        private EntityQuery mapQuery;

        protected override void OnCreate()
        {
            this.mapQuery = this.EntityManager.CreateMapEntityQuery();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var mapEntity = this.mapQuery.GetSingletonEntity();
            var map = this.EntityManager.GetMap(mapEntity);
            
            int2 move = default;
            
            if(Input.GetKeyDown(KeyCode.LeftArrow) ||
               Input.GetKeyDown(KeyCode.Keypad4) ||
               Input.GetKeyDown(KeyCode.H))
            {
                move.x = -1;
            }

            if(Input.GetKeyDown(KeyCode.RightArrow) ||
               Input.GetKeyDown(KeyCode.Keypad6) ||
               Input.GetKeyDown(KeyCode.L))
            {
                move.x = 1;
            }

            if(Input.GetKeyDown(KeyCode.UpArrow) ||
               Input.GetKeyDown(KeyCode.Keypad8) ||
               Input.GetKeyDown(KeyCode.K))
            {
                move.y = 1;
            }

            if(Input.GetKeyDown(KeyCode.DownArrow) ||
               Input.GetKeyDown(KeyCode.Keypad2) ||
               Input.GetKeyDown(KeyCode.J))
            {
                move.y = -1;
            }
            
            var max = new int2(map.Width - 1, map.Height - 1);
            var min = int2.zero;

            this.Entities.WithAll<Player>()
                .ForEach((ref Position position, ref ViewshedData viewshedData) =>
                {
                    var destination = position.Value + move;
                    var destinationIdx = map.xy_idx(destination.x, destination.y);
                    if(map.Tiles[destinationIdx] != TileType.Wall)
                    {
                        position.Value = clamp(destination, min, max);
                        viewshedData.Dirty = true;
                    }
                })
                .Run();

            return default;
        }
    }
}
