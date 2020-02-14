using Unity.Entities;

namespace LazyGameDevZA.RogueDOTS.Components
{
    public struct CombatStats: IComponentData
    {
        public int MaxHP;
        public int HP;
        public int Defense;
        public int Power;
    }
}
