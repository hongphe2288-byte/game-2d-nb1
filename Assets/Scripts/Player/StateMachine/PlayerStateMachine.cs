using UnityEngine;

namespace Player
{
    public class PlayerStateMachine
    {
        public PlayerState CurrentState { get; private set; }

        public void Initialize(PlayerState startingState)
        {
            CurrentState = startingState;
            CurrentState.Enter();
            Debug.Log($"[FSM] Initialized state: {startingState.GetType().Name}");
        }

        public void ChangeState(PlayerState newState)
        {
            CurrentState.Exit();
            CurrentState = newState;
            CurrentState.Enter();
            Debug.Log($"[FSM] State changed to: {newState.GetType().Name}");
        }
    }
}
