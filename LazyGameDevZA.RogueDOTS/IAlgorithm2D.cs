using Unity.Mathematics;

namespace LazyGameDevZA.RogueDOTS
{
    public interface IAlgorithm2D
    {
        int point2DToIndex(int2 pt);
        int2 Dimensions { get; }

        bool Inbounds(int2 pos);
    }
}
