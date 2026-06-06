using UnityEngine;

namespace Player
{
    public class PlayerInAirState : PlayerState
    {
        private float xInput;

        public PlayerInAirState(PlayerController player, PlayerStateMachine stateMachine, string animBoolName) 
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

            xInput = player.XInput;

            // Apply horizontal velocity in the air (can be slightly modified for air control if desired, currently using moveSpeed)
            player.SetVelocity(xInput * player.PlayerData.moveSpeed, player.Rb.linearVelocity.y);
            player.CheckFlip(xInput);

            // Double Jump
            if (Input.GetButtonDown("Jump") && player.CanJump())
            {
                player.Jump();
                startTime = Time.time; // Reset startTime to prevent instant landing check success
            }

            // Dash in the air
            if (Input.GetKeyDown(KeyCode.LeftShift) && player.CanDash())
            {
                stateMachine.ChangeState(player.DashState);
                return;
            }

            // Attack in the air (if attack in air is allowed)
            if (Input.GetMouseButtonDown(0))
            {
                stateMachine.ChangeState(player.AttackState);
                return;
            }

            // Land check
            // We allow landing if the player is grounded and either moving downward/stable, 
            // OR if they have been in the air for a short time (to avoid landing on the frame they jump)
            if (player.CheckGrounded() && (player.Rb.linearVelocity.y <= 0.1f || Time.time - startTime > 0.1f))
            {
                stateMachine.ChangeState(player.IdleState);
                return;
            }
        }
    }
}
