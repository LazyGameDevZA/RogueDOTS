using LazyGameDevZA.RogueDOTS.Components;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using static Unity.Mathematics.math;
using static LazyGameDevZA.RogueDOTS.Map;

namespace LazyGameDevZA.RogueDOTS.Systems
{
    [AlwaysSynchronizeSystem]
    public class PlayerInputSystem : JobComponentSystem
    {
        private struct Move
        {
            public int DeltaX;
            public int DeltaY;
        }

        private EntityQuery mapQuery;

        protected override void OnCreate()
        {
            this.mapQuery = this.EntityManager.CreateEntityQuery(typeof(Tile));
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var mapEntity = this.mapQuery.GetSingletonEntity();
            var map = this.EntityManager.GetBuffer<Tile>(mapEntity).AsNativeArray();
            
            var move = new Move();
            
            if(Input.GetKeyDown(KeyCode.LeftArrow) ||
               Input.GetKeyDown(KeyCode.Keypad4) ||
               Input.GetKeyDown(KeyCode.H))
            {
                move.DeltaX = -1;
            }

            if(Input.GetKeyDown(KeyCode.RightArrow) ||
               Input.GetKeyDown(KeyCode.Keypad6) ||
               Input.GetKeyDown(KeyCode.L))
            {
                move.DeltaX = 1;
            }

            if(Input.GetKeyDown(KeyCode.UpArrow) ||
               Input.GetKeyDown(KeyCode.Keypad8) ||
               Input.GetKeyDown(KeyCode.K))
            {
                move.DeltaY = 1;
            }

            if(Input.GetKeyDown(KeyCode.DownArrow) ||
               Input.GetKeyDown(KeyCode.Keypad2) ||
               Input.GetKeyDown(KeyCode.J))
            {
                move.DeltaY = -1;
            }

            this.Entities.WithAll<Player>()
                .ForEach((ref Position position) =>
                {
                    var moveX = position.X + move.DeltaX;
                    var moveY = position.Y + move.DeltaY;
                    var destinationIdx = xy_idx(moveX, moveY);
                    if(map[destinationIdx].Type != TileType.Wall)
                    {
                        position.X = clamp(moveX, 0, 79);
                        position.Y = clamp(moveY, 0, 49);
                    }
                })
                .Run();

            return inputDeps;
        }
    }
}
