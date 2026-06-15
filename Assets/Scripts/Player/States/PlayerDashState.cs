using UnityEngine;

namespace Player
{
    public class PlayerDashState : PlayerState
    {
        private float dashTimer;
        private float originalGravity;
        private AudioManager audioManager;

        public PlayerDashState(PlayerController player, PlayerStateMachine stateMachine, string animBoolName)
            : base(player, stateMachine, animBoolName)
        {
            audioManager = Object.FindObjectOfType<AudioManager>();
        }

        public override void Enter()
        {
            base.Enter();

            // Play dash sound
            if (audioManager != null)
            {
                audioManager.PlayDashSound();
            }

            dashTimer = player.PlayerData.dashDuration;

            // Save original gravity and freeze gravity
            originalGravity = player.Rb.gravityScale;
            player.Rb.gravityScale = 0f;

            // Set invulnerability
            player.SetInvulnerable(true);

            // Record dash cooldown
            player.StartDashCooldown();

            // Set velocity
            float direction = player.FacingDir;
            player.SetVelocity(direction * player.PlayerData.dashSpeed, 0f);
        }

        public override void Exit()
        {
            base.Exit();

            // Restore gravity
            player.Rb.gravityScale = originalGravity;

            // Remove invulnerability
            player.SetInvulnerable(false);

            // Reset velocity slightly so they don't carry too much momentum
            player.SetVelocity(0f, player.Rb.linearVelocity.y);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            dashTimer -= Time.deltaTime;

            // While dashing, keep velocity constant
            float direction = player.FacingDir;
            player.SetVelocity(direction * player.PlayerData.dashSpeed, 0f);

            if (dashTimer <= 0f)
            {
                if (player.CheckGrounded())
                {
                    stateMachine.ChangeState(player.IdleState);
                }
                else
                {
                    stateMachine.ChangeState(player.InAirState);
                }
            }
        }
    }
}