using System.Linq;
using LazyGameDevZA.RogueDOTS.Components;
using LazyGameDevZA.RogueDOTS.Toolkit;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

namespace LazyGameDevZA.RogueDOTS
{
    public class Bootstrap: ICustomBootstrap
    {
        // ReSharper disable once InconsistentNaming
        private EntityManager EntityManager;
        
        public bool Initialize(string defaultWorldName)
        {
            var world = new World(defaultWorldName);
            this.EntityManager = world.EntityManager;

            var map = Map.NewMapRoomsAndCorridors(this.EntityManager);
            var rooms = map.Rooms;
            var roomCenters = new NativeArray<int2>(rooms.Length, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

            for(int i = 0; i < rooms.Length; i++)
            {
                var (x, y) = rooms[i].Center();
                roomCenters[i] = int2(x, y);
            }

            this.CreatePlayer(roomCenters[0]);

            var rng = RandomNumberGenerator.New();
            for(int i = 1; i < roomCenters.Length; i++)
            {
                byte glyph;
                string name;

                var roll = rng.RollDice(1, 2);

                switch(roll)
                {
                    case 1:
                        glyph = (byte)'g';
                        name = "Goblin";
                        break;
                    default:
                        glyph = (byte)'o';
                        name = "Orc";
                        break;
                }

                var monster =
                    this.EntityManager.CreateEntity(
                        typeof(Position),
                        typeof(Renderable),
                        typeof(VisibleTilePosition),
                        typeof(ViewshedData),
                        typeof(Monster),
                        typeof(Name),
                        typeof(BlocksTile));
                var formattedName = $"{name} #{i}";
                this.EntityManager.SetName(monster, formattedName);
                this.EntityManager.SetComponentData(monster, new Position { Value = roomCenters[i] });
                this.EntityManager.SetComponentData(
                    monster,
                    new Renderable { Glyph = glyph, Foreground = Color.red, Background = Color.black });
                this.EntityManager.SetComponentData(monster, new ViewshedData { Range = 8, Dirty = true });
                this.EntityManager.SetComponentData(monster, new Name { Value = formattedName });
            }

            this.EntityManager.CreateEntity(typeof(Move));

            World.DefaultGameObjectInjectionWorld = world;
            var systems = DefaultWorldInitialization.GetAllSystems(WorldSystemFilterFlags.Default);

            DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(world, systems);
            ScriptBehaviourUpdateOrder.UpdatePlayerLoop(world);

            return true;
        }
        
        private void CreatePlayer(int2 position)
        {
            var entity = this.EntityManager.CreateEntity(
                typeof(Player),
                typeof(Position),
                typeof(Renderable),
                typeof(VisibleTilePosition),
                typeof(ViewshedData));
            this.EntityManager.SetName(entity, "Player");
            this.EntityManager.SetComponentData(entity, new Position { Value = position });
            this.EntityManager.SetComponentData(
                entity,
                new Renderable { Glyph = (byte)'@', Foreground = Color.yellow, Background = Color.black });
            this.EntityManager.SetComponentData(entity, new ViewshedData { Range = 8, Dirty = true });
        }
    }
}
