using System;
using LazyGameDevZA.RogueDOTS.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using static LazyGameDevZA.RogueDOTS.Map;

namespace LazyGameDevZA.RogueDOTS
{
    public class GameBootstrap : JobComponentSystem
    {
        protected override void OnCreate()
        {
            this.NewMap();
            
            this.CreatePlayer(40, 25);

            for(int i = 0; i < 10; i++)
            {
                this.CreateMonster(i * 7, 20);
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps) => inputDeps;

        private void NewMap()
        {
            var map = new NativeArray<Tile>(80 * 50, Allocator.Temp);
            
            
            for(int x = 0; x < 80; x++)
            {
                map[xy_idx(x, 0)] = TileType.Wall;
                map[xy_idx(x, 49)] = TileType.Wall;
            }

            for(int y = 0; y < 50; y++)
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

            var entity = this.EntityManager.CreateEntity(typeof(Tile));
            var buffer = this.EntityManager.GetBuffer<Tile>(entity);
            buffer.Capacity = map.Length;

            for(int i = 0; i < map.Length; i++)
            {
                buffer.Add(map[i]);
            }
        }

        private void CreatePlayer(int x, int y)
        {
            var entity = this.EntityManager.CreateEntity(typeof(Player), typeof(Position), typeof(Renderable));
            this.EntityManager.SetComponentData(entity, new Position { X = x, Y = y });
            this.EntityManager.SetComponentData(entity, new Renderable { Glyph = (byte)'@', Foreground = Color.yellow, Background = Color.black });
        }

        private void CreateMonster(int x, int y)
        {
            var entity = this.EntityManager.CreateEntity(typeof(Position), typeof(Renderable));
            this.EntityManager.SetComponentData(entity, new Position { X = x, Y = y });
            this.EntityManager.SetComponentData(entity, new Renderable { Glyph = 1, Foreground = Color.red, Background = Color.black });
        }
    }
}
