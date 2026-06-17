using UnityEngine;

namespace Enemies
{
    /// <summary>
    /// Script điều khiển Boss Mực (Squid Boss).
    /// Kế thừa từ Enemies.Core.Enemy để sử dụng chung cơ chế HP, bị Hit, Choáng và chết.
    /// Có cơ chế kích hoạt khi Player đến gần, chịu ảnh hưởng của trọng lực bản đồ, và tấn công tầm xa bằng bắn đạn.
    /// </summary>
    public class SquidBoss : Core.Enemy
    {
        [Header("Squid Boss Settings")]
        [SerializeField] private GameObject bulletPrefab;      // Prefab của quả cầu năng lượng
        [SerializeField] private Transform firePos;            // Vị trí bắn đạn
        [SerializeField] private float bulletForce = 12f;      // Lực bay của đạn
        [SerializeField] private float activationRadius = 8f;  // Bán kính kích hoạt Boss hoạt động

        private bool isActivated = false;

        protected override void Awake()
        {
            base.Awake();

            // Tự động tắt kích hoạt đạn gốc trong scene ở Awake để tránh đạn tự rơi tự do và bị hủy ngay khi game start
            if (bulletPrefab != null && bulletPrefab.scene.IsValid())
            {
                bulletPrefab.SetActive(false);
                Debug.Log($"[SquidBoss] [Awake] Phát hiện bulletPrefab là đối tượng trong Scene. Đã tự động ẩn {bulletPrefab.name} để tránh bị rơi tự do.");
            }
        }

        protected override void Start()
        {
            base.Start();

            // Ban đầu chưa kích hoạt, vô hiệu hóa di chuyển của AIPath
            if (aiPath != null)
            {
                aiPath.canMove = false;
            }

            Debug.Log($"[SquidBoss] Boss Mực {gameObject.name} đã được khởi tạo. Đang chờ Player trong bán kính {activationRadius}m...");
        }

        protected override void Update()
        {
            if (isDead) return;

            // Xử lý khi bị choáng (Hit Stun)
            if (isHit)
            {
                hitStunTimer -= Time.deltaTime;
                if (hitStunTimer <= 0)
                {
                    ExitHitStun();
                }
                return;
            }

            // Cập nhật thời gian hồi chiêu tấn công
            if (attackTimer > 0)
            {
                attackTimer -= Time.deltaTime;
            }

            if (player == null) return;

            // Kiểm tra trạng thái kích hoạt dựa trên khoảng cách tới Player
            if (!isActivated)
            {
                float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
                if (distanceToPlayer <= activationRadius)
                {
                    ActivateBoss();
                }
                else
                {
                    // Đứng yên ở trạng thái Idle trước khi kích hoạt
                    SetAnimBool(idleAnimParam, true);
                    return;
                }
            }

            // LUÔN LUÔN XOAY MẶT VỀ PHÍA PLAYER KHI ĐÃ THỨC TỈNH
            RotateTowardsPlayer();

            // Sau khi đã kích hoạt, chạy logic kiểm tra tầm đánh / di chuyển bám đuổi của lớp cha
            float currentDistance = Vector2.Distance(transform.position, player.transform.position);

            if (currentDistance <= attackRadius)
            {
                if (attackTimer <= 0 && !isAttacking)
                {
                    StartAttack();
                }
            }
            else
            {
                if (!isAttacking && !isHit)
                {
                    MoveBehavior();
                }
            }
        }

        // Xoay mặt về phía Player (Sprite mặc định hướng sang bên Trái)
        private void RotateTowardsPlayer()
        {
            if (player == null || isHit || isDead) return;

            float directionX = player.transform.position.x - transform.position.x;
            if (directionX > 0.1f)
            {
                // Player ở bên phải -> giữ mặt hướng sang phải (scale X dương)
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
            else if (directionX < -0.1f)
            {
                // Player ở bên trái -> lật mặt sang trái (scale X âm)
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
            }
        }

        // Kích hoạt Boss
        private void ActivateBoss()
        {
            isActivated = true;
            if (aiPath != null)
            {
                aiPath.canMove = true;
            }
            Debug.Log($"[SquidBoss] Player đã đi vào phạm vi kích hoạt! Boss Mực {gameObject.name} thức tỉnh!");
        }

        // Ghi đè phương thức tấn công: Thay vì cận chiến trực tiếp, Boss sẽ bắn đạn
        protected override void StartAttack()
        {
            isAttacking = true;
            attackTimer = attackCooldown;

            // Dừng di chuyển khi bắn đạn
            if (aiPath != null)
            {
                aiPath.canMove = false;
            }
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero; // Triệt tiêu lực quán tính vật lý
            }

            // Hướng mặt về phía Player trước khi bắn (Sprite mặc định hướng sang bên Trái)
            RotateTowardsPlayer();

            // Cập nhật Animator
            SetAnimBool(idleAnimParam, false);
            SetAnimBool(attackAnimParam, true);

            // Không gọi ShootBullet() trực tiếp ở đây nữa để tránh bắn đạn ngay frame đầu tiên.
            // Việc bắn đạn sẽ hoàn toàn do Animation Event ở frame 40 kích hoạt.
            // Việc kết thúc trạng thái tấn công (isAttacking = false) cũng sẽ hoàn toàn phụ thuộc vào
            // Animation Event gọi hàm AnimationFinishTrigger() ở frame cuối cùng của animation tấn công.
            // Cơ chế Failsafe: Tự động reset sau 1.5s phòng khi Animation Event bị thiếu trong Editor
            StartCoroutine(AutoResetAttack(1.5f));
        }

        // Thực hiện khởi tạo và bắn đạn về phía Player (để public để Animation Event gọi dễ dàng)
        public void ShootBullet()
        {
            if (bulletPrefab == null)
            {
                Debug.LogWarning($"[SquidBoss] Chưa gán Bullet Prefab trên {gameObject.name}!");
                return;
            }

            if (firePos == null)
            {
                Debug.LogWarning($"[SquidBoss] Chưa gán Fire Pos trên {gameObject.name}! Sẽ bắn từ tâm của Boss.");
            }

            Vector3 spawnPos = firePos != null ? firePos.position : transform.position;

            // Tính toán hướng bắn từ firePos tới Player
            Vector2 shootDirection = ((Vector2)player.transform.position - (Vector2)spawnPos).normalized;

            // Dịch chuyển spawnPos ra xa một khoảng nhỏ (0.5f) theo hướng bắn để tránh spawn chìm trong Collider của đất hay Boss
            spawnPos += (Vector3)(shootDirection * 0.5f);

            // Tạo quả cầu đạn
            GameObject bulletObj = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);

            // Đảm bảo đạn được active (vì prefab gốc có thể đã bị deactivate ở Start)
            bulletObj.SetActive(true);

            // Bỏ qua va chạm vật lý giữa đạn và chính con Boss này để Boss không bị đẩy lệch đi khi bắn
            Collider2D bulletCol = bulletObj.GetComponent<Collider2D>();
            Collider2D[] bossColliders = GetComponentsInChildren<Collider2D>();
            if (bulletCol != null)
            {
                foreach (var bossCol in bossColliders)
                {
                    if (bossCol != null)
                    {
                        Physics2D.IgnoreCollision(bulletCol, bossCol);
                    }
                }
            }

            // Gán sát thương của Boss vào viên đạn
            BossBullet bulletScript = bulletObj.GetComponent<BossBullet>();
            if (bulletScript == null)
            {
                bulletScript = bulletObj.GetComponentInParent<BossBullet>();
            }

            // Tự động gán thêm script BossBullet nếu thiếu (Self-healing)
            if (bulletScript == null)
            {
                bulletScript = bulletObj.AddComponent<BossBullet>();
                Debug.Log($"[SquidBoss] Tự động gắn thêm component BossBullet cho đạn {bulletObj.name} do thiếu thiết lập.");
            }

            if (bulletScript != null)
            {
                bulletScript.SetOwner(gameObject); // Thiết lập Boss là chủ sở hữu đạn để bỏ qua va chạm tuyệt đối
                bulletScript.SetDamage(damage);    // damage kế thừa từ lớp cha Enemy
            }

            // Áp dụng lực vật lý đẩy viên đạn bay đi
            Rigidbody2D bulletRb = bulletObj.GetComponent<Rigidbody2D>();
            if (bulletRb != null)
            {
                bulletRb.linearVelocity = shootDirection * bulletForce;
            }

            Debug.Log($"[SquidBoss] Boss Mực bắn một quả cầu năng lượng hướng về Player! Lực: {bulletForce}");
        }

        // Nhận sát thương: Nếu đang ngủ mà bị chém trúng, Boss cũng tự động thức tỉnh
        public override void TakeDamage(int damageTaken)
        {
            if (!isActivated && !isDead)
            {
                ActivateBoss();
            }

            base.TakeDamage(damageTaken);
        }
    }
}
