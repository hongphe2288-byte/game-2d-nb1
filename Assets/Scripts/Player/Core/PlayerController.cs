using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerDataSO playerData;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private HealthBar healthBar;

        // State Machine properties
        public PlayerStateMachine StateMachine { get; private set; }
        public PlayerIdleState IdleState { get; private set; }
        public PlayerMoveState MoveState { get; private set; }
        public PlayerInAirState InAirState { get; private set; }
        public PlayerDashState DashState { get; private set; }
        public PlayerAttackState AttackState { get; private set; }

        // Components
        public Rigidbody2D Rb { get; private set; }
        public Animator Anim { get; private set; }
        public PlayerRespawn Respawn { get; private set; }
        private Core.GameManager gameManager;

        // Input
        public float XInput { get; private set; }
        public float YInput { get; private set; }

        // Settings / Status getters
        public PlayerDataSO PlayerData => playerData;
        public float FacingDir { get; private set; } = 1f;
        public bool IsInvulnerable { get; private set; }

        // State variables
        private int jumpsRemaining;
        private float dashCooldownTimer;
        public int CurrentHealth { get; private set; }

        // Animator parameters cache
        private System.Collections.Generic.HashSet<string> animatorParameters;

        private void Awake()
        {
            Rb = GetComponent<Rigidbody2D>();
            Anim = GetComponent<Animator>();
            Respawn = GetComponent<PlayerRespawn>();
            gameManager = FindAnyObjectByType<Core.GameManager>();

            // Cache animator parameters to prevent runtime warnings/errors
            animatorParameters = new System.Collections.Generic.HashSet<string>();
            if (Anim != null)
            {
                foreach (AnimatorControllerParameter param in Anim.parameters)
                {
                    animatorParameters.Add(param.name);
                }
            }

            // Initialize State Machine and States
            StateMachine = new PlayerStateMachine();
            
            // Map states to animator boolean parameters
            IdleState = new PlayerIdleState(this, StateMachine, "isIdle");
            MoveState = new PlayerMoveState(this, StateMachine, "isRunning");
            InAirState = new PlayerInAirState(this, StateMachine, "isJumping");
            DashState = new PlayerDashState(this, StateMachine, "isDashing");
            AttackState = new PlayerAttackState(this, StateMachine, "isAttacking");
        }

        public bool HasParameter(string paramName)
        {
            return animatorParameters != null && animatorParameters.Contains(paramName);
        }

        private void Start()
        {
            // Set initial health
            CurrentHealth = playerData.maxHealth;

            // Find HealthBar if not explicitly assigned
            if (healthBar == null)
            {
                healthBar = FindAnyObjectByType<HealthBar>();
            }

            // Initialize HealthBar UI
            if (healthBar != null)
            {
                healthBar.updateBar(CurrentHealth, playerData.maxHealth);
            }

            // Reset jump counter
            ResetJumps();

            // Safety Checks for setup
            if (groundCheck == null)
            {
                Debug.LogError("[PlayerController Error] Ground Check Transform is not assigned in the Inspector! Please assign a child Transform placed at the player's feet.");
            }
            if (playerData.groundLayer == 0)
            {
                Debug.LogWarning("[PlayerController Warning] Ground Layer in PlayerData is set to 'Nothing'! Using fallback detection (checking any solid collider except Player). Please select your Ground Layer in the Scriptable Object.");
            }

            // Set initial state
            StateMachine.Initialize(IdleState);
        }

        private void Update()
        {
            if (gameManager != null && gameManager.IsGameOver()) return;

            // Read Inputs
            XInput = Input.GetAxisRaw("Horizontal");
            YInput = Input.GetAxisRaw("Vertical");

            // Tick dash cooldown
            if (dashCooldownTimer > 0f)
            {
                dashCooldownTimer -= Time.deltaTime;
            }

            // Update State Logic
            StateMachine.CurrentState.LogicUpdate();
        }

        private void FixedUpdate()
        {
            if (gameManager != null && gameManager.IsGameOver()) return;

            // Update State Physics
            StateMachine.CurrentState.PhysicsUpdate();
        }

        public void SetVelocity(float xVelocity, float yVelocity)
        {
            Rb.linearVelocity = new Vector2(xVelocity, yVelocity);
        }

        public bool CheckGrounded()
        {
            // 1. Standard Check (using groundCheck and groundLayer)
            if (groundCheck != null && playerData.groundLayer != 0)
            {
                if (Physics2D.OverlapCircle(groundCheck.position, playerData.groundCheckRadius, playerData.groundLayer))
                {
                    return true;
                }
            }

            // 2. Fallback Overlap Check (using groundCheck and all solid colliders)
            if (groundCheck != null)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheck.position, playerData.groundCheckRadius);
                foreach (Collider2D collider in colliders)
                {
                    if (collider.gameObject != gameObject && !collider.isTrigger)
                    {
                        return true;
                    }
                }
            }

            // 3. Absolute Fail-safe Physics Contact Check (using Rigidbody2D contacts)
            // If the player's collider is touching any surface with an upward-facing normal, they are grounded!
            if (Rb != null)
            {
                ContactPoint2D[] contacts = new ContactPoint2D[10];
                int contactCount = Rb.GetContacts(contacts);
                for (int i = 0; i < contactCount; i++)
                {
                    // normal.y > 0.5f means the surface is below the player (floor or ramp)
                    if (contacts[i].normal.y > 0.5f)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public void CheckFlip(float xInput)
        {
            if (xInput > 0f && FacingDir < 0f)
            {
                Flip();
            }
            else if (xInput < 0f && FacingDir > 0f)
            {
                Flip();
            }
        }

        private void Flip()
        {
            FacingDir *= -1f;
            transform.localScale = new Vector3(FacingDir, 1f, 1f);
        }

        // Jump control helper
        public void ResetJumps()
        {
            jumpsRemaining = playerData.maxJumps;
        }

        public bool CanJump()
        {
            return jumpsRemaining > 0;
        }

        public void Jump()
        {
            jumpsRemaining--;
            SetVelocity(Rb.linearVelocity.x, playerData.jumpForce);
        }

        // Dash control helper
        public bool CanDash()
        {
            return dashCooldownTimer <= 0f;
        }

        public void StartDashCooldown()
        {
            dashCooldownTimer = playerData.dashCooldown;
        }

        // Invulnerability
        public void SetInvulnerable(bool value)
        {
            IsInvulnerable = value;
        }

        // HP logic
        public void TakeDamage(int damage)
        {
            if (IsInvulnerable) return;

            CurrentHealth -= damage;
            if (CurrentHealth < 0) CurrentHealth = 0;
            Debug.Log($"Player took {damage} damage. Current HP: {CurrentHealth}");

            if (healthBar != null)
            {
                healthBar.updateBar(CurrentHealth, playerData.maxHealth);
            }

            if (CurrentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            CurrentHealth = 0;
            if (healthBar != null)
            {
                healthBar.updateBar(CurrentHealth, playerData.maxHealth);
            }
            if (gameManager != null)
            {
                gameManager.GameOver();
            }
        }

        // Hitstop and Screen Shake coordinator
        public void TriggerHitstopAndScreenShake(float hitstopDuration, float shakeIntensity)
        {
            if (gameManager != null)
            {
                gameManager.TriggerHitstop(hitstopDuration);
                gameManager.TriggerScreenShake(shakeIntensity);
            }
        }

        // Animation Event Redirectors
        // These should be called by Animation Events on the player model
        public void AnimationTrigger()
        {
            StateMachine.CurrentState.AnimationTrigger();
        }

        public void AnimationFinishTrigger()
        {
            StateMachine.CurrentState.AnimationFinishTrigger();
        }
    }
}
