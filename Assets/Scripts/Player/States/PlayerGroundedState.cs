using UnityEngine;

namespace Player
{
    public class PlayerGroundedState : PlayerState
    {
        protected float xInput;

        public PlayerGroundedState(PlayerController player, PlayerStateMachine stateMachine, string animBoolName) 
            : base(player, stateMachine, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();
            player.ResetJumps();
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            xInput = player.XInput;

            // Attack
            if (Input.GetMouseButtonDown(0))
            {
                stateMachine.ChangeState(player.AttackState);
                return;
            }

            // Dash
            if (Input.GetKeyDown(KeyCode.LeftShift) && player.CanDash())
            {
                stateMachine.ChangeState(player.DashState);
                return;
            }

            // Jump
            if (Input.GetButtonDown("Jump"))
            {
                player.Jump();
                stateMachine.ChangeState(player.InAirState);
                return;
            }

            // Fall off edge
            if (!player.CheckGrounded())
            {
                stateMachine.ChangeState(player.InAirState);
                return;
            }
        }
    }
}
