using LazyGameDevZA.RogueDOTS.Components;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace LazyGameDevZA.RogueDOTS
{
    public class GameBootstrap : JobComponentSystem
    {
        protected override void OnCreate()
        {
            var (map, rooms) = Map.NewMapRoomsAndCorridors();

            this.EntityManager.Insert(map, "Map");

            var (playerX, playerY) = rooms[0].Center();
            this.CreatePlayer(playerX, playerY);
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps) => inputDeps;

        private void CreatePlayer(int x, int y)
        {
            var entity = this.EntityManager.CreateEntity(typeof(Player), typeof(Position), typeof(Renderable));
            this.EntityManager.SetName(entity, "Player");
            this.EntityManager.SetComponentData(entity, new Position { X = x, Y = y });
            this.EntityManager.SetComponentData(entity, new Renderable { Glyph = (byte)'@', Foreground = Color.yellow, Background = Color.black });
        }

        private void CreateMonster(int x, int y, int i)
        {
            var entity = this.EntityManager.CreateEntity(typeof(Position), typeof(Renderable));
            this.EntityManager.SetName(entity, $"Monster {i}");
            this.EntityManager.SetComponentData(entity, new Position { X = x, Y = y });
            this.EntityManager.SetComponentData(entity, new Renderable { Glyph = 1, Foreground = Color.red, Background = Color.black });
        }
    }
}
