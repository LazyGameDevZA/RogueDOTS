using Unity.Collections;
using Unity.Mathematics;

namespace LazyGameDevZA.RogueDOTS.Toolkit
{
    public interface IBaseMap
    {
        bool IsOpaque(int idx);

        NativeList<Exit> GetAvailableExits(int idx, NativeList<Exit> exits = default);

        // TODO default impl to return 1f
        float GetPathingDistance(int idx1, int idx2);
    }
    
    public readonly struct Exit
    {
        public readonly int Index;
        public readonly float Cost;

        public Exit(int index, float cost)
        {
            this.Index = index;
            this.Cost = cost;
        }

        public void Deconstruct(out int index, out float cost)
        {
            index = this.Index;
            cost = this.Cost;
        }
        
        public static implicit operator Exit((int index, float cost) value) => new Exit(value.index, value.cost); 
    }
}
