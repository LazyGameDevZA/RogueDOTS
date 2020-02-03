using Unity.Collections;
using Unity.Entities;

namespace LazyGameDevZA.RogueDOTS.Components
{
    public struct Name: IComponentData
    {
        public FixedString32 Value;
    }
}
