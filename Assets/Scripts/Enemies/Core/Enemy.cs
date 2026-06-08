using UnityEngine;
using System.Collections;
using Pathfinding;

namespace Enemies.Core
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Animator))]
    public class Enemy : MonoBehaviour
    {
        [Header("Base Enemy Stats")]
        [SerializeField] protected int maxHealth = 50;
        [SerializeField] protected int damage = 15;
        [SerializeField] protected float attackCooldown = 1.5f;
        [SerializeField] protected float attackRadius = 1.2f;
        [SerializeField] protected float hitStunDuration = 0.4f; // Thời gian choáng khi bị chém trúng

        [Header("Animator Parameter Names")]
        [SerializeField] protected string idleAnimParam = "isIdle";
        [SerializeField] protected string attackAnimParam = "isAttacking";
        [SerializeField] protected string hitAnimParam = "isHit";
        [SerializeField] protected string deadAnimParam = "isDead";

        // Runtime state variables
        protected int currentHealth;
        protected float attackTimer;
        protected float hitStunTimer;
        protected bool isHit;
        protected bool isAttacking;
        protected bool isDead;

        // Components references
        protected Animator anim;
        protected Rigidbody2D rb;
        protected AIPath aiPath;
        protected AIDestinationSetter aiDestinationSetter;
        
        // Target reference
        protected Player.PlayerController player;

        // Cache animator parameters to prevent runtime warnings/errors
        private System.Collections.Generic.HashSet<string> animatorParameters;

        protected virtual void Awake()
        {
            anim = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
            aiPath = GetComponent<AIPath>();
            aiDestinationSetter = GetComponent<AIDestinationSetter>();

            // Cache animator parameters
            animatorParameters = new System.Collections.Generic.HashSet<string>();
            if (anim != null)
            {
                foreach (AnimatorControllerParameter param in anim.parameters)
                {
                    animatorParameters.Add(param.name);
                }
            }
        }

        protected virtual void Start()
        {
            currentHealth = maxHealth;
            player = FindAnyObjectByType<Player.PlayerController>();

            if (player != null && aiDestinationSetter != null)
            {
                aiDestinationSetter.target = player.transform;
            }
        }

        protected virtual void Update()
        {
            if (isDead) return;

            // Xử lý khi đang bị choáng (Hit Stun)
            if (isHit)
            {
                hitStunTimer -= Time.deltaTime;
                if (hitStunTimer <= 0)
                {
                    ExitHitStun();
                }
                return; // Đang bị choáng thì không thực hiện di chuyển hay tấn công
            }

            // Cập nhật bộ đếm thời gian cooldown tấn công
            if (attackTimer > 0)
            {
                attackTimer -= Time.deltaTime;
            }

            if (player == null) return;

            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

            // Kiểm tra điều kiện để thực hiện tấn công
            if (distanceToPlayer <= attackRadius)
            {
                if (attackTimer <= 0 && !isAttacking)
                {
                    StartAttack();
                }
            }
            else
            {
                // Nếu người chơi ở xa và quái không ở trạng thái tấn công/bị đánh thì tiếp tục đuổi theo
                if (!isAttacking && !isHit)
                {
                    MoveBehavior();
                }
            }
        }

        // Logic di chuyển đuổi theo người chơi
        protected virtual void MoveBehavior()
        {
            if (aiPath != null)
            {
                aiPath.canMove = true;
            }

            // Khi đuổi theo bình thường thì bật hoạt ảnh Idle (bay/di chuyển)
            SetAnimBool(idleAnimParam, true);
            SetAnimBool(attackAnimParam, false);
            SetAnimBool(hitAnimParam, false);
        }

        // Bắt đầu thực hiện tấn công
        protected virtual void StartAttack()
        {
            isAttacking = true;
            attackTimer = attackCooldown;

            // Dừng di chuyển khi tấn công
            if (aiPath != null)
            {
                aiPath.canMove = false;
            }
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero; // Dừng lập tức lực quán tính vật lý
            }

            // Cập nhật Animator
            SetAnimBool(idleAnimParam, false);
            SetAnimBool(attackAnimParam, true);

            // Thực hiện gây sát thương cho nhân vật
            ApplyDamageToPlayer();

            // Cơ chế Failsafe: Tự động chuyển về trạng thái đuổi theo sau 0.5s 
            // nếu không dùng Animation Event để gọi kết thúc đòn đánh
            StartCoroutine(AutoResetAttack(0.5f));
        }

        // Thực hiện gây sát thương lên nhân vật
        protected virtual void ApplyDamageToPlayer()
        {
            if (player != null)
            {
                player.TakeDamage(damage);
                Debug.Log($"{gameObject.name} đã tấn công trúng Player! Gây {damage} sát thương.");
            }
        }

        // Reset trạng thái tấn công (có thể gọi từ Animation Event để khớp 100% với animation kết thúc)
        public virtual void AnimationFinishTrigger()
        {
            if (isAttacking && !isHit && !isDead)
            {
                isAttacking = false;
                SetAnimBool(attackAnimParam, false);
                if (aiPath != null)
                {
                    aiPath.canMove = true;
                }
            }
        }

        protected virtual IEnumerator AutoResetAttack(float duration)
        {
            yield return new WaitForSeconds(duration);
            if (isAttacking)
            {
                AnimationFinishTrigger();
            }
        }

        // Nhận sát thương từ Player (gọi từ PlayerAttackState)
        public virtual void TakeDamage(int damageTaken)
        {
            if (isDead) return;

            currentHealth -= damageTaken;
            Debug.Log($"{gameObject.name} nhận {damageTaken} sát thương. Máu còn: {currentHealth}/{maxHealth}");

            // Chuyển sang trạng thái isHit
            isHit = true;
            isAttacking = false;
            hitStunTimer = hitStunDuration;

            // Dừng di chuyển ngay lập tức
            if (aiPath != null)
            {
                aiPath.canMove = false;
            }
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }

            // Cập nhật Animator
            SetAnimBool(idleAnimParam, false);
            SetAnimBool(attackAnimParam, false);
            SetAnimBool(hitAnimParam, true);

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        // Thoát trạng thái bị choáng
        protected virtual void ExitHitStun()
        {
            isHit = false;
            SetAnimBool(hitAnimParam, false);
            
            if (aiPath != null)
            {
                aiPath.canMove = true;
            }
        }

        // Xử lý khi hết HP
        protected virtual void Die()
        {
            isDead = true;
            
            if (aiPath != null)
            {
                aiPath.canMove = false;
            }
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }

            SetAnimBool(idleAnimParam, false);
            SetAnimBool(attackAnimParam, false);
            SetAnimBool(hitAnimParam, false);
            SetAnimBool(deadAnimParam, true);

            Debug.Log($"{gameObject.name} đã bị tiêu diệt!");
            
            // Hủy đối tượng sau 1.5s để có thời gian chạy animation chết
            Destroy(gameObject, 1.5f);
        }

        // Helper check parameter trước khi set để tránh lỗi cảnh báo của Unity Console
        protected void SetAnimBool(string paramName, bool value)
        {
            if (anim != null && animatorParameters.Contains(paramName))
            {
                anim.SetBool(paramName, value);
            }
        }
    }
}
