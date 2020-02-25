using Unity.Entities;

namespace LazyGameDevZA.RogueDOTS.Toolkit
{
    public struct RltkBuilder
    {
        public int Width;
        public int Height;
        public int TileWidth;
        public int TileHeight;
    }

    public static class RltkBuilderImpl
    {
        public static void Build(ref this RltkBuilder builder, EntityManager entityManager)
        {
        }
    }
}
