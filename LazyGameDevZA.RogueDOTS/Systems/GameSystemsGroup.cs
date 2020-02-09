using LazyGameDevZA.RogueDOTS.Components;
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
    }
}
