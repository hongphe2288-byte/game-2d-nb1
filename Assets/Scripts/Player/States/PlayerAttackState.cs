using UnityEngine;

namespace Player
{
    public class PlayerAttackState : PlayerState
    {
        private bool attackInputReceived;
        private int currentComboIndex;
        private float lastAttackTime;

        public PlayerAttackState(PlayerController player, PlayerStateMachine stateMachine, string animBoolName) 
            : base(player, stateMachine, animBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();

            attackInputReceived = false;

            // Determine if combo should reset due to time window
            if (Time.time - lastAttackTime > player.PlayerData.comboWindow)
            {
                currentComboIndex = 0;
            }

            // Set Animator parameter
            if (player.HasParameter("comboIndex"))
            {
                player.Anim.SetInteger("comboIndex", currentComboIndex);
            }
            
            // Push player slightly forward for attack feedback
            float pushForce = 3f; // Small forward push
            player.SetVelocity(player.FacingDir * pushForce, player.Rb.linearVelocity.y);
        }

        public override void Exit()
        {
            base.Exit();
            lastAttackTime = Time.time;
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // Detect next attack input for combo chaining
            if (Input.GetMouseButtonDown(0))
            {
                attackInputReceived = true;
            }

            // Fail-safe timeout: transition back to Idle if the animation event doesn't fire
            if (Time.time - startTime > 0.8f)
            {
                Debug.LogWarning("[FSM Warning] Player Attack State timed out (0.8s). Force transitioning to IdleState. Please verify that Animation Events (AnimationFinishTrigger) are set up on your attack animations.");
                currentComboIndex = 0;
                stateMachine.ChangeState(player.IdleState);
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
            
            // Decelerate the forward push over time
            player.SetVelocity(player.Rb.linearVelocity.x * 0.9f, player.Rb.linearVelocity.y);
        }

        public override void AnimationTrigger()
        {
            base.AnimationTrigger();

            // Perform Hit Detection
            PerformAttackDetection();
        }

        public override void AnimationFinishTrigger()
        {
            base.AnimationFinishTrigger();

            if (attackInputReceived)
            {
                // Move to next combo step
                currentComboIndex++;
                if (currentComboIndex >= player.PlayerData.damagePerComboStep.Length)
                {
                    currentComboIndex = 0;
                }

                // Restart attack state for next combo
                stateMachine.ChangeState(this);
            }
            else
            {
                // Reset combo index and return to idle
                currentComboIndex = 0;
                stateMachine.ChangeState(player.IdleState);
            }
        }

        private void PerformAttackDetection()
        {
            // Calculate hitbox position relative to player facing direction
            Vector2 hitboxPos = (Vector2)player.transform.position + new Vector2(
                player.PlayerData.attackOffset.x * player.FacingDir,
                player.PlayerData.attackOffset.y
            );

            // Find all colliders in the attack range
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(hitboxPos, player.PlayerData.attackRadius);

            bool hitAnyEnemy = false;
            int damage = player.PlayerData.damagePerComboStep[currentComboIndex];

            foreach (Collider2D enemyCollider in hitEnemies)
            {
                // Check if collider has Enemy component (to be created)
                Enemies.Core.Enemy enemy = enemyCollider.GetComponent<Enemies.Core.Enemy>();
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                    hitAnyEnemy = true;
                }
            }

            if (hitAnyEnemy)
            {
                // Trigger hitstop and screen shake on the GameManager
                player.TriggerHitstopAndScreenShake(player.PlayerData.hitstopDuration, player.PlayerData.screenShakeIntensity);
            }
        }
    }
}
