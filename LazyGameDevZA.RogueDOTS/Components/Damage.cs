using Unity.Entities;

namespace LazyGameDevZA.RogueDOTS.Components
{
    public struct WantsToMelee : IComponentData
    {
        public Entity Target;
    }

    public struct SufferDamage : IComponentData
    {
        public int Amount;
    }
}
