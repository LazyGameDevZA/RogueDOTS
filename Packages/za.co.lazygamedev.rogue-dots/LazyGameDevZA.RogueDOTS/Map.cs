using System.Linq;
using LazyGameDevZA.RogueDOTS.Components;
using LazyGameDevZA.RogueDOTS.Toolkit;
using LazyGameDevZA.RogueDOTS.Toolkit.Geometry;
using Unity.Collections;
using Unity.Entities;
using static Unity.Mathematics.math;
using int2 = Unity.Mathematics.int2;

namespace LazyGameDevZA.RogueDOTS
{
    public partial struct Map
    {
        private DynamicBuffer<Tile> tiles;
        private DynamicBuffer<Room> rooms;

        public readonly int Width;
        public readonly int Height;

        private DynamicBuffer<RevealedTile> revealedTiles;
        private DynamicBuffer<VisibleTile> visibleTiles;
        private DynamicBuffer<BlockedTile> blockedTiles;

        private int Length => this.Width * this.Height;

        public NativeArray<Tile> Tiles => this.tiles.AsNativeArray();
        public NativeArray<Rect> Rooms => this.rooms.Reinterpret<Rect>().AsNativeArray();
        public NativeArray<RevealedTile> RevealedTiles => this.revealedTiles.AsNativeArray();
        public NativeArray<VisibleTile> VisibleTiles => this.visibleTiles.AsNativeArray();
        public NativeArray<BlockedTile> BlockedTiles => this.blockedTiles.AsNativeArray();

        public Map(
            DynamicBuffer<Tile> tiles,
            DynamicBuffer<Room> rooms,
            int width,
            int height,
            DynamicBuffer<RevealedTile> revealedTiles,
            DynamicBuffer<VisibleTile> visibleTiles,
            DynamicBuffer<BlockedTile> blockedTiles)
        {
            this.tiles = tiles;
            this.rooms = rooms;
            this.Width = width;
            this.Height = height;
            this.revealedTiles = revealedTiles;
            this.visibleTiles = visibleTiles;
            this.blockedTiles = blockedTiles;
        }
    }

    // Impl
    public partial struct Map
    {
        public int xy_idx(int x, int y)
        {
            return y * this.Width + x;
        }

        public int xy_idx(int2 position)
        {
            return xy_idx(position.x, position.y);
        }

        public int2 idx_xy(int idx)
        {
            var x = idx % this.Width;
            var y = idx / this.Width;
            
            return int2(x, y);
        }

        public static Map NewMapRoomsAndCorridors(EntityManager entityManager)
        {
            const int width = 80;
            const int height = 50;

            const int maxRooms = 30;
            const int minSize = 6;
            const int maxSize = 10;
            var map = entityManager.CreateMap(TileType.Wall, width, height, maxRooms);

            var rooms = map.rooms.Reinterpret<Rect>();

            var rng = RandomNumberGenerator.New();
            
            foreach(var _ in Enumerable.Range(0, maxRooms))
            {
                var w = rng.Range(minSize, maxSize);
                var h = rng.Range(minSize, maxSize);
                var x = rng.RollDice(1, width - w - 1) - 1;
                var y = rng.RollDice(1, height - h - 1) - 1;
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

                    if(rooms.Length != 0)
                    {
                        var (newX, newY) = newRoom.Center();
                        var (prevX, prevY) = rooms[map.rooms.Length - 1].Center();

                        if(rng.Range(0, 2) == 1)
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

                    rooms.Add(newRoom);
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

        private bool IsExitValid(int2 position)
        {
            var (x, y) = position;
            if(x < 1 || x > this.Width - 1 || y < 1 || y > this.Height)
            {
                return false;
            }

            var idx = this.xy_idx(position);

            return !this.blockedTiles[idx];
        }

        public void PopulateBlocked()
        {
            for(int i = 0; i < this.Length; i++)
            {
                this.blockedTiles[i] = this.tiles[i] == TileType.Wall;
            }
        }
    }

    public partial struct Map: IBaseMap
    {
        public bool IsOpaque(int idx)
        {
            return this.tiles[idx].Type == TileType.Wall;
        }

        public NativeList<Exit> GetAvailableExits(int idx, NativeList<Exit> exits = default)
        {
            if(exits.IsCreated)
            {
                exits = new NativeList<Exit>(8, Allocator.Temp);
            }
            else
            {
                exits.Clear();
            }

            var position = this.idx_xy(idx);

            // Cardinal directions
            var west = position + int2(-1, 0);
            var east = position + int2(+1, 0);
            var south = position + int2(0, -1);
            var north = position + int2(0, +1);
            
            if(this.IsExitValid(west)) { exits.Add((xy_idx(west), 1f)); }
            if(this.IsExitValid(east)) { exits.Add((xy_idx(east), 1f)); }
            if(this.IsExitValid(south)) { exits.Add((xy_idx(south), 1f)); }
            if(this.IsExitValid(north)) { exits.Add((xy_idx(north), 1f)); }
            
            // Diagonals
            var southWest = position + int2(-1, -1);
            var southEast = position + int2(+1, -1);
            var northWest = position + int2(-1, +1);
            var northEast = position + int2(+1, +1);
            
            if(this.IsExitValid(southWest)) { exits.Add((xy_idx(southWest), 1.45f)); }
            if(this.IsExitValid(southEast)) { exits.Add((xy_idx(southEast), 1.45f)); }
            if(this.IsExitValid(northWest)) { exits.Add((xy_idx(northWest), 1.45f)); }
            if(this.IsExitValid(northEast)) { exits.Add((xy_idx(northEast), 1.45f)); }

            return exits;
        }

        public float GetPathingDistance(int idx1, int idx2)
        {
            var p1 = idx_xy(idx1);
            var p2 = idx_xy(idx2);

            return DistanceAlg.Pythagoras.Distance2D(p1, p2);
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

        public int2 Dimensions => int2(this.Width, this.Height);

        public bool Inbounds(int2 pos)
        {
            var bounds = this.Dimensions;
            return pos.x >= 0 && pos.x < bounds.x && pos.y >= 0 && pos.y < bounds.y;
        }
    }

    public static class Int2Extensions
    {
        public static void Deconstruct(this int2 value, out int x, out int y)
        {
            x = value.x;
            y = value.y;
        }
    }
}
