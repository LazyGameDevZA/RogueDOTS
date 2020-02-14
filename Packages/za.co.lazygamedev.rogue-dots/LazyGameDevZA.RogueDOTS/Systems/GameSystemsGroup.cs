using System;
using LazyGameDevZA.RogueDOTS.Components;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace LazyGameDevZA.RogueDOTS.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public class GameSystemsGroup : ComponentSystemGroup
    {
        private Entity runStateEntity;

        protected override void OnStartRunning()
        {
            this.runStateEntity = this.EntityManager.CreateEntity(typeof(RunState));
            
            this.SetSingleton<RunState>(RunState.State.PreRun);
            
            this.EntityManager.CreateEntity(typeof(Move));
        }

        protected override void OnUpdate()
        {
            var state = this.GetSingleton<RunState>();

            switch(state.Value)
            {
                case RunState.State.PreRun:
                    base.OnUpdate();
                    this.SetSingleton<RunState>(RunState.State.AwaitingInput);
                    break;
                case RunState.State.AwaitingInput:
                    this.SetSingleton(this.TryMove());
                    break;
                case RunState.State.PlayerTurn:
                    base.OnUpdate();
                    this.SetSingleton<RunState>(RunState.State.MonsterTurn);
                    break;
                case RunState.State.MonsterTurn:
                    base.OnUpdate();
                    this.SetSingleton<RunState>(RunState.State.AwaitingInput);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void OnStopRunning()
        {
            this.EntityManager.DestroyEntity(this.runStateEntity);
        }

        private RunState TryMove()
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
                    move.x = +1;
                }

                if(Input.GetKeyDown(KeyCode.UpArrow) ||
                   Input.GetKeyDown(KeyCode.Keypad8) ||
                   Input.GetKeyDown(KeyCode.K))
                {
                    move.y = +1;
                }

                if(Input.GetKeyDown(KeyCode.DownArrow) ||
                   Input.GetKeyDown(KeyCode.Keypad2) ||
                   Input.GetKeyDown(KeyCode.J))
                {
                    move.y = -1;
                }
                
                // Diagonals
                if(Input.GetKeyDown(KeyCode.Keypad9) ||
                   Input.GetKeyDown(KeyCode.Y))
                {
                    move.x = +1;
                    move.y = +1;
                }
                
                if(Input.GetKeyDown(KeyCode.Keypad7) ||
                   Input.GetKeyDown(KeyCode.U))
                {
                    move.x = -1;
                    move.y = +1;
                }
                
                if(Input.GetKeyDown(KeyCode.Keypad3) ||
                   Input.GetKeyDown(KeyCode.N))
                {
                    move.x = +1;
                    move.y = -1;
                }
                
                if(Input.GetKeyDown(KeyCode.Keypad1) ||
                   Input.GetKeyDown(KeyCode.B))
                {
                    move.x = -1;
                    move.y = -1;
                }
                
                this.SetSingleton<Move>(move);

                if(move.x != 0 || move.y != 0)
                {
                    return RunState.State.PlayerTurn;
                }

                return RunState.State.AwaitingInput;
        }
    }
}
