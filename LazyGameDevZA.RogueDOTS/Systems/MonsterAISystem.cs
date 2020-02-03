using LazyGameDevZA.RogueDOTS.Components;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace LazyGameDevZA.RogueDOTS.Systems
{
    [UpdateInGroup(typeof(GameSystemsGroup))]
    [AlwaysSynchronizeSystem]
    public class MonsterAISystem: JobComponentSystem
    {
        protected override void OnCreate()
        {
            this.RequireSingletonForUpdate<PlayerPosition>();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var playerPosition = this.GetSingleton<PlayerPosition>();
            
            this.Entities.WithAll<Monster>()
                .ForEach((Entity entity, in DynamicBuffer<VisibleTilePosition> visibleTiles, in ViewshedData viewshedData, in Position position, in Name name) =>
                {
                    for(int i = 0; i < visibleTiles.Length; i++)
                    {
                        var visibleTilePos = visibleTiles[i].Value;
                        var playerPos = playerPosition.Value;
                        if(visibleTilePos.x == playerPos.x && visibleTilePos.y == playerPos.y)
                        {
                            Debug.LogFormat("{0} shouts insults", name.Value);
                        }
                    }
                })
                .WithoutBurst()
                .Run();

            return default;
        }
    }
}
