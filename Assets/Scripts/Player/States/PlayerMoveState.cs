using UnityEngine;

namespace Player
{
    public class PlayerMoveState : PlayerGroundedState
    {
        public PlayerMoveState(PlayerController player, PlayerStateMachine stateMachine, string animBoolName) 
            : base(player, stateMachine, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            player.SetVelocity(xInput * player.PlayerData.moveSpeed, player.Rb.linearVelocity.y);
            player.CheckFlip(xInput);

            if (xInput == 0f)
            {
                stateMachine.ChangeState(player.IdleState);
            }
        }
    }
}
