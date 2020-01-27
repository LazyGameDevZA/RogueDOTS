using LazyGameDevZA.RogueDOTS.Components;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace LazyGameDevZA.RogueDOTS
{
    public class GameBootstrap : JobComponentSystem
    {
        protected override void OnCreate()
        {
            var map = Map.NewMapRoomsAndCorridors(this.EntityManager);

            var (playerX, playerY) = map.Rooms[0].Center();
            this.CreatePlayer(playerX, playerY);
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps) => inputDeps;

        private void CreatePlayer(int x, int y)
        {
            var entity = this.EntityManager.CreateEntity(
                typeof(Player),
                typeof(Position),
                typeof(Renderable),
                typeof(VisibleTilePosition),
                typeof(ViewshedData));
            this.EntityManager.SetName(entity, "Player");
            this.EntityManager.SetComponentData(entity, new Position { Value = new int2(x, y) });
            this.EntityManager.SetComponentData(
                entity,
                new Renderable { Glyph = (byte)'@', Foreground = Color.yellow, Background = Color.black });
            this.EntityManager.SetComponentData(entity, new ViewshedData { Range = 8, Dirty = true });
        }

        private void CreateMonster(int x, int y, int i)
        {
            var entity = this.EntityManager.CreateEntity(typeof(Position), typeof(Renderable));
            this.EntityManager.SetName(entity, $"Monster {i}");
            this.EntityManager.SetComponentData(entity, new Position { Value = new int2(x, y)});
            this.EntityManager.SetComponentData(entity, new Renderable { Glyph = 1, Foreground = Color.red, Background = Color.black });
        }
    }
}
