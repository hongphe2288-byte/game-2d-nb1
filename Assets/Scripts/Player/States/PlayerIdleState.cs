using UnityEngine;

namespace Player
{
    public class PlayerIdleState : PlayerGroundedState
    {
        public PlayerIdleState(PlayerController player, PlayerStateMachine stateMachine, string animBoolName) 
            : base(player, stateMachine, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();
            player.SetVelocity(0f, player.Rb.linearVelocity.y);
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            if (xInput != 0f)
            {
                stateMachine.ChangeState(player.MoveState);
            }
        }
    }
}
