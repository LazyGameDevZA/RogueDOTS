using System.Linq;
using LazyGameDevZA.RogueDOTS.Components;
using Unity.Entities;

namespace LazyGameDevZA.RogueDOTS
{
    public static class EntitiesExtensions
    {

        public delegate void ForEachMap(in Map map);

        public static Map CreateMap(this EntityManager entityManager, TileType fill, int width, int height, int maxRooms)
        {
            var entity = entityManager.CreateEntity(
                typeof(Tile),
                typeof(Room),
                typeof(MapDimensions),
                typeof(RevealedTile),
                typeof(VisibleTile));
            
            entityManager.SetName(entity, "Map");
            var tiles = entityManager.GetBuffer<Tile>(entity);
            var tileCount = height * width;
            tiles.Capacity = tileCount;

            var rooms = entityManager.GetBuffer<Room>(entity);
            rooms.Capacity = maxRooms;
            
            entityManager.SetComponentData(entity, new MapDimensions { Width = width, Height = height });

            var revealedTiles = entityManager.GetBuffer<RevealedTile>(entity);
            revealedTiles.Capacity = tileCount;

            var visibleTiles = entityManager.GetBuffer<VisibleTile>(entity);
            visibleTiles.Capacity = tileCount;

            foreach(var _ in Enumerable.Range(0, tileCount))
            {
                tiles.Add(fill);
                revealedTiles.Add(default);
                visibleTiles.Add(default);
            }
            
            return new Map(tiles, rooms, width, height, revealedTiles, visibleTiles);
        }

        public static Map GetMap(this EntityManager entityManager, Entity entity)
        {
            var tiles = entityManager.GetBuffer<Tile>(entity);
            var rooms = entityManager.GetBuffer<Room>(entity);
            var dimensions = entityManager.GetComponentData<MapDimensions>(entity);
            var revealedTiles = entityManager.GetBuffer<RevealedTile>(entity);
            var visibleTiles = entityManager.GetBuffer<VisibleTile>(entity);

            return new Map(tiles, rooms, dimensions.Width, dimensions.Height, revealedTiles, visibleTiles);
        }

        public static EntityQuery CreateMapEntityQuery(
            this EntityManager entityManager,
            bool revealedTilesWriteAccess = false,
            bool visibleTilesWriteAccess = false)
        {
            return entityManager.CreateEntityQuery(
                ComponentType.ReadOnly<Tile>(),
                ComponentType.ReadOnly<Room>(),
                ComponentType.ReadOnly<MapDimensions>(),
                revealedTilesWriteAccess ? ComponentType.ReadWrite<RevealedTile>() : ComponentType.ReadOnly<RevealedTile>(),
                visibleTilesWriteAccess ? ComponentType.ReadWrite<VisibleTile>() : ComponentType.ReadOnly<VisibleTile>());
        }
    }
}
