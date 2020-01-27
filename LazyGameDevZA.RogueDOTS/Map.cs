using System.Linq;
using LazyGameDevZA.RogueDOTS.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace LazyGameDevZA.RogueDOTS
{
    public partial struct Map
    {
        private DynamicBuffer<Tile> tiles;
        private DynamicBuffer<Rect> rooms;

        public readonly int Width;
        public readonly int Height;

        private DynamicBuffer<RevealedTile> revealedTiles;
        private DynamicBuffer<VisibleTile> visibleTiles;

        public NativeArray<Tile> Tiles => this.tiles.AsNativeArray();
        public NativeArray<Rect> Rooms => this.rooms.AsNativeArray();
        public NativeArray<RevealedTile> RevealedTiles => this.revealedTiles.AsNativeArray();
        public NativeArray<VisibleTile> VisibleTiles => this.visibleTiles.AsNativeArray();

        public Map(
            DynamicBuffer<Tile> tiles,
            DynamicBuffer<Rect> rooms,
            int width,
            int height,
            DynamicBuffer<RevealedTile> revealedTiles,
            DynamicBuffer<VisibleTile> visibleTiles)
        {
            this.tiles = tiles;
            this.rooms = rooms;
            this.Width = width;
            this.Height = height;
            this.revealedTiles = revealedTiles;
            this.visibleTiles = visibleTiles;
        }
    }

    // Impl
    public partial struct Map
    {
        public int xy_idx(int x, int y)
        {
            return y * this.Width + x;
        }

        public static Map NewMapRoomsAndCorridors(EntityManager entityManager)
        {
            const int width = 80;
            const int height = 50;

            const int maxRooms = 30;
            const int minSize = 6;
            const int maxSize = 10;
            var map = entityManager.CreateMap(TileType.Wall, width, height, maxRooms);

            var random = new Random(32345);

            foreach(var _ in Enumerable.Range(0, maxRooms))
            {
                var w = random.Range(minSize, maxSize);
                var h = random.Range(minSize, maxSize);
                var x = random.RollDice(1, width - w - 1) - 1;
                var y = random.RollDice(1, height - h - 1) - 1;
                var newRoom = new Rect(x, y, w, h);

                var ok = true;

                foreach(var otherRoom in map.rooms)
                {
                    if(newRoom.Intersects(otherRoom))
                    {
                        ok = false;
                    }
                }

                if(ok)
                {
                    map.ApplyRoomToMap(newRoom);

                    if(map.rooms.Length != 0)
                    {
                        var (newX, newY) = newRoom.Center();
                        var (prevX, prevY) = map.rooms[map.rooms.Length - 1].Center();

                        if(random.Range(0, 2) == 1)
                        {
                            map.ApplyHorizontalTunnel(prevX, newX, prevY);
                            map.ApplyVerticalTunnel(prevY, newY, newX);
                        }
                        else
                        {
                            map.ApplyVerticalTunnel(prevY, newY, prevX);
                            map.ApplyHorizontalTunnel(prevX, newX, newY);
                        }
                    }

                    map.rooms.Add(newRoom);
                }
            }

            return map;
        }

        private static NativeArray<Tile> CreateNewWithFill(TileType fill, int length, Allocator allocator)
        {
            var map = new NativeArray<Tile>(length, allocator, NativeArrayOptions.UninitializedMemory);

            for(int i = 0; i < map.Length; i++)
            {
                map[i] = fill;
            }

            return map;
        }

        private void ApplyRoomToMap(Rect room)
        {
            for(int y = room.Y1 + 1; y <= room.Y2; y++)
            {
                for(int x = room.X1 + 1; x <= room.X2; x++)
                {
                    this.tiles[xy_idx(x, y)] = TileType.Floor;
                }
            }
        }

        private void ApplyHorizontalTunnel(int x1, int x2, int y)
        {
            var mapSize = this.Width * this.Height;

            for(int x = min(x1, x2); x <= max(x1, x2); x++)
            {
                var idx = xy_idx(x, y);

                if(idx > 0 && idx < mapSize)
                {
                    this.tiles[idx] = TileType.Floor;
                }
            }
        }

        private void ApplyVerticalTunnel(int y1, int y2, int x)
        {
            var mapSize = Width * Height;

            for(int y = min(y1, y2); y <= max(y1, y2); y++)
            {
                var idx = xy_idx(x, y);

                if(idx > 0 && idx < mapSize)
                {
                    tiles[idx] = TileType.Floor;
                }
            }
        }
    }

    public partial struct Map: IBaseMap
    {
        public bool IsOpaque(int idx)
        {
            return this.tiles[idx].Type == TileType.Wall;
        }
    }

    // Algorithm2D
    public partial struct Map: IAlgorithm2D
    {
        public int point2DToIndex(int2 pt)
        {
            var bounds = this.Dimensions;

            return pt.y * bounds.x + pt.x;
        }

        public int2 Dimensions => new int2(this.Width, this.Height);

        public bool Inbounds(int2 pos)
        {
            var bounds = this.Dimensions;
            return pos.x > 0 && pos.x < bounds.x && pos.y > 0 && pos.y < bounds.y;
        }
    }
}
