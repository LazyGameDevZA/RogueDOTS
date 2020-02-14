using Unity.Entities;

namespace LazyGameDevZA.RogueDOTS.Components
{
    public struct RunState : IComponentData
    {
        public State Value;
        
        public static implicit operator RunState(State value) => new RunState { Value = value };

        public static implicit operator State(RunState runState) => runState.Value;
        
        public enum State : byte
        {
            AwaitingInput,
            PreRun,
            PlayerTurn,
            MonsterTurn
        }
    }
}
