using System.Linq;
using System.Runtime.CompilerServices;
using LazyGameDevZA.RogueDOTS.Components;
using Unity.Collections;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace LazyGameDevZA.RogueDOTS
{
    public struct Rect
    {
        public readonly int X1, X2, Y1, Y2;

        public Rect(int x, int y, int w, int h)
        {
            this.X1 = x;
            this.Y1 = y;
            this.X2 = x + w;
            this.Y2 = y + h;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Intersects(Rect other) => this.X1 <= other.X2 && this.X2 >= other.X1 && this.Y1 <= other.Y2 && this.Y2 >= other.Y1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public (int, int) Center() => ((this.X1 + this.X2) / 2, (this.Y1 + this.Y2) / 2);
    }

    public static class Map
    {
        public const int Width = 80;
        public const int Height = 50;
        
    
        public static int xy_idx(int x, int y)
        {
            return y * 80 + x;
        }

        public static NativeArray<Tile> NewMapTest()
        {
            var map = new NativeArray<Tile>(Width * Height, Allocator.Temp);

            for(int x = 0; x < Width; x++)
            {
                map[xy_idx(x, 0)] = TileType.Wall;
                map[xy_idx(x, 49)] = TileType.Wall;
            }

            for(int y = 0; y < Height; y++)
            {
                map[xy_idx(0, y)] = TileType.Wall;
                map[xy_idx(79, y)] = TileType.Wall;
            }

            var random = new Random(32345);

            for(int i = 0; i < 400; i++)
            {
                var x = random.RollDice(1, 79);
                var y = random.RollDice(1, 49);
                var idx = xy_idx(x, y);

                if(idx != xy_idx(40, 25))
                {
                    map[idx] = TileType.Wall;
                }
            }

            return map;
        }

        public static (NativeArray<Tile> map, NativeList<Rect> rooms) NewMapRoomsAndCorridors()
        {
            var map = CreateNewWithFill(TileType.Wall, Width * Height, Allocator.Temp);

            const int maxRooms = 30;
            const int minSize = 6;
            const int maxSize = 10;
            var rooms = new NativeList<Rect>(maxRooms, Allocator.Temp);
            
            var random = new Random(32345);

            foreach(var _ in Enumerable.Range(0, maxRooms))
            {
                var w = random.Range(minSize, maxSize);
                var h = random.Range(minSize, maxSize);
                var x = random.RollDice(1, Width - w - 1) - 1;
                var y = random.RollDice(1, Height - h - 1) - 1;
                var newRoom = new Rect(x, y, w, h);

                var ok = true;

                foreach(var otherRoom in rooms)
                {
                    if(newRoom.Intersects(otherRoom))
                    {
                        ok = false;
                    }
                }

                if(ok)
                {
                    ApplyRoomToMap(newRoom, map);

                    if(rooms.Length != 0)
                    {
                        var (newX, newY) = newRoom.Center();
                        var (prevX, prevY) = rooms[rooms.Length - 1].Center();

                        if(random.Range(0, 2) == 1)
                        {
                            ApplyHorizontalTunnel(map, prevX, newX, prevY);
                            ApplyVerticalTunnel(map, prevY, newY, newX);
                        }
                        else
                        {
                            ApplyVerticalTunnel(map, prevY, newY, prevX);
                            ApplyHorizontalTunnel(map, prevX, newX, newY);
                        }
                    }
                    
                    rooms.Add(newRoom);
                }
            }

            return (map, rooms);
        }

        private static NativeArray<Tile> CreateNewWithFill(TileType fill, int length, Allocator allocator)
        {
            var map = new NativeArray<Tile>(length, allocator);

            for(int i = 0; i < map.Length; i++)
            {
                map[i] = fill;
            }

            return map;
        }

        private static void ApplyRoomToMap(Rect room, NativeArray<Tile> map)
        {
            for(int y = room.Y1 + 1; y <= room.Y2; y++)
            {
                for(int x = room.X1 + 1; x <= room.X2; x++)
                {
                    map[xy_idx(x, y)] = TileType.Floor;
                }
            }
        }

        private static void ApplyHorizontalTunnel(NativeArray<Tile> map, int x1, int x2, int y)
        {
            var mapSize = Width * Height;
            
            for(int x = min(x1, x2); x <= max(x1, x2); x++)
            {
                var idx = xy_idx(x, y);

                if(idx > 0 && idx < mapSize)
                {
                    map[idx] = TileType.Floor;
                }
            }
        }

        private static void ApplyVerticalTunnel(NativeArray<Tile> map, int y1, int y2, int x)
        {
            var mapSize = Width * Height;
            
            for(int y = min(y1, y2); y <= max(y1, y2); y++)
            {
                var idx = xy_idx(x, y);

                if(idx > 0 && idx < mapSize)
                {
                    map[idx] = TileType.Floor;
                }
            }
        }
    }
}
