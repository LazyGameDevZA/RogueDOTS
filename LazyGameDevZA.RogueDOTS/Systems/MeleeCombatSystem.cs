using LazyGameDevZA.RogueDOTS.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using static Unity.Mathematics.math;

namespace LazyGameDevZA.RogueDOTS.Systems
{
    [AlwaysSynchronizeSystem]
    [UpdateInGroup(typeof(GameSystemsGroup))]
    [UpdateAfter(typeof(MapIndexingSystem))]
    public class MeleeCombatSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var combatStats = this.GetComponentDataFromEntity<CombatStats>();
            var names = this.GetComponentDataFromEntity<Name>();
            var ecb = new EntityCommandBuffer(Allocator.TempJob);
            
            this.Entities.WithName("TODO")
                .ForEach((Entity entity, in Name name, in CombatStats stats, in WantsToMelee wantsToMelee) =>
                {
                    if(stats.HP > 0)
                    {
                        if(!combatStats.HasComponent(wantsToMelee.Target))
                        {
                            return;
                        }

                        var targetStats = combatStats[wantsToMelee.Target];
                        if(targetStats.HP > 0)
                        {
                            var targetName = names[wantsToMelee.Target];
                            var damage = max(0, stats.Power - targetStats.Defense);

                            if(damage == 0)
                            {
                                Debug.LogFormat("{0} is unable to hurt {1}", name.Value, targetName.Value);
                            }
                            else
                            {
                                Debug.LogFormat("{0} hits {1}, for {2} hp.", name.Value, targetName.Value, damage);
                                ecb.AddComponent(wantsToMelee.Target, new SufferDamage { Amount = damage });
                            }
                        }
                    }
                    
                    ecb.RemoveComponent<WantsToMelee>(entity);
                })
                .WithoutBurst()
                .Run();

            ecb.Playback(this.EntityManager);
            ecb.Dispose();
        }
    }
}
