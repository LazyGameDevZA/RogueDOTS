using LazyGameDevZA.RogueDOTS.Components;
using LazyGameDevZA.RogueDOTS.Toolkit;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace LazyGameDevZA.RogueDOTS.Systems
{
    public enum RunState : byte
    {
        Paused,
        Running
    }
    
    public class GameSystemsGroup : ComponentSystemGroup
    {
        private RunState runState = RunState.Running;
        
        protected override void OnCreate()
        {
            var map = Map.NewMapRoomsAndCorridors(this.EntityManager);
            var rooms = map.Rooms;
            var roomCenters = new NativeArray<int2>(rooms.Length, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

            for(int i = 0; i < rooms.Length; i++)
            {
                var (x, y) = rooms[i].Center();
                roomCenters[i] = new int2(x, y);
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
                        typeof(Name),
                        typeof(Position),
                        typeof(Renderable),
                        typeof(VisibleTilePosition),
                        typeof(ViewshedData),
                        typeof(Monster));
                var formattedName = $"{name} #{i}";
                this.EntityManager.SetName(monster, formattedName);
                this.EntityManager.SetComponentData(monster, new Name { Value = formattedName });
                this.EntityManager.SetComponentData(monster, new Position { Value = roomCenters[i] });
                this.EntityManager.SetComponentData(
                    monster,
                    new Renderable { Glyph = glyph, Foreground = Color.red, Background = Color.black });
                this.EntityManager.SetComponentData(monster, new ViewshedData { Range = 8, Dirty = true });
            }

            this.EntityManager.CreateEntity(typeof(Move));
        }

        protected override void OnUpdate()
        {
            if(this.runState == RunState.Running)
            {
                base.OnUpdate();
                this.runState = RunState.Paused;
            }
            else
            {
            
                int2 move = default;
            
                if(Input.GetKeyDown(KeyCode.LeftArrow) ||
                   Input.GetKeyDown(KeyCode.Keypad4) ||
                   Input.GetKeyDown(KeyCode.H))
                {
                    move.x = -1;
                }

                if(Input.GetKeyDown(KeyCode.RightArrow) ||
                   Input.GetKeyDown(KeyCode.Keypad6) ||
                   Input.GetKeyDown(KeyCode.L))
                {
                    move.x = 1;
                }

                if(Input.GetKeyDown(KeyCode.UpArrow) ||
                   Input.GetKeyDown(KeyCode.Keypad8) ||
                   Input.GetKeyDown(KeyCode.K))
                {
                    move.y = 1;
                }

                if(Input.GetKeyDown(KeyCode.DownArrow) ||
                   Input.GetKeyDown(KeyCode.Keypad2) ||
                   Input.GetKeyDown(KeyCode.J))
                {
                    move.y = -1;
                }
                
                this.SetSingleton<Move>(move);

                if(move.x != 0 || move.y != 0)
                {
                    this.runState = RunState.Running;
                }
            }
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
