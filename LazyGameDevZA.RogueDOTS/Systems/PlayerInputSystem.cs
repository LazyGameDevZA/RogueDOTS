using LazyGameDevZA.RogueDOTS.Components;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using static Unity.Mathematics.math;

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
        
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var move = new Move();
            if(Input.GetKeyDown(KeyCode.LeftArrow))
            {
                move.DeltaX = -1;
            }

            if(Input.GetKeyDown(KeyCode.RightArrow))
            {
                move.DeltaX = 1;
            }

            if(Input.GetKeyDown(KeyCode.UpArrow))
            {
                move.DeltaY = 1;
            }

            if(Input.GetKeyDown(KeyCode.DownArrow))
            {
                move.DeltaY = -1;
            }

            this.Entities.WithAll<Player>()
                .ForEach((ref Position position) =>
                {
                    position.X = min(79, max(0, position.X + move.DeltaX));
                    position.Y = min(49, max(0, position.Y + move.DeltaY));
                })
                .Run();

            return inputDeps;
        }
    }
}
