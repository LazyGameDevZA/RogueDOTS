using LazyGameDevZA.RogueDOTS.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace LazyGameDevZA.RogueDOTS.Systems
{
    [AlwaysSynchronizeSystem]
    [UpdateInGroup(typeof(GameSystemsGroup))]
    [UpdateAfter(typeof(MeleeCombatSystem))]
    public class DamageSystem : JobComponentSystem
    {
        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var players = this.GetComponentDataFromEntity<Player>();
            var ecb = new EntityCommandBuffer(Allocator.TempJob);

            FixedString32 message = "You are dead";

            this.Entities
                .ForEach((Entity entity, ref CombatStats stats, in SufferDamage damage) =>
                {
                    stats.HP -= damage.Amount;

                    ecb.RemoveComponent<SufferDamage>(entity);

                    if(stats.HP < 1)
                    {
                        switch(players.HasComponent(entity))
                        {
                            case false:
                                ecb.DestroyEntity(entity);
                                break;
                            case true:
                                Debug.Log(message);
                                break;
                        }
                    }
                })
                .WithoutBurst()
                .Run();

            ecb.Playback(this.EntityManager);
            ecb.Dispose();

            return default;
        }
    }
}
