using UnityEngine;

namespace Enemies
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class BossBullet : MonoBehaviour
    {
        [Header("Bullet Settings")]
        [SerializeField] private int damage = 15;
        [SerializeField] private float lifeTime = 5.0f;

        private Rigidbody2D rb;
        private bool isDestroyed = false;
        private Vector2 lastPosition;
        private GameObject owner;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.gravityScale = 0f;
                rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            }
        }

        private void Start()
        {
            // Ghi nhận vị trí ban đầu
            lastPosition = transform.position;

            // Bỏ qua va chạm vật lý với các quái vật và Boss
            IgnoreEnemyCollisions();

            // Tự động hủy sau khoảng thời gian lifeTime để tránh rò rỉ bộ nhớ
            Destroy(gameObject, lifeTime);
        }

        // Đăng ký chủ sở hữu để bỏ qua va chạm triệt để (Không gây lỗi biên dịch ở SquidBoss)
        public void SetOwner(GameObject ownerObj)
        {
            owner = ownerObj;
            IgnoreEnemyCollisions();
        }

        private void IgnoreEnemyCollisions()
        {
            Collider2D bulletCollider = GetComponent<Collider2D>();
            if (bulletCollider != null)
            {
                // Bỏ qua va chạm với tất cả Enemy trong Scene
                Enemies.Core.Enemy[] enemies = FindObjectsByType<Enemies.Core.Enemy>(FindObjectsSortMode.None);
                foreach (var enemy in enemies)
                {
                    if (enemy != null && enemy.gameObject != gameObject)
                    {
                        Collider2D[] enemyColliders = enemy.GetComponentsInChildren<Collider2D>();
                        foreach (var enemyCol in enemyColliders)
                        {
                            if (enemyCol != null)
                            {
                                Physics2D.IgnoreCollision(bulletCollider, enemyCol);
                            }
                        }
                    }
                }

                // Đặc biệt bỏ qua va chạm với chủ nhân bắn ra nó (Boss)
                if (owner != null)
                {
                    Collider2D[] ownerColliders = owner.GetComponentsInChildren<Collider2D>();
                    foreach (var ownerCol in ownerColliders)
                    {
                        if (ownerCol != null)
                        {
                            Physics2D.IgnoreCollision(bulletCollider, ownerCol);
                        }
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            if (isDestroyed) return;

            Vector2 currentPosition = transform.position;

            // Sử dụng Linecast để quét va chạm trên đường đạn bay từ frame trước đến frame này
            RaycastHit2D[] hits = Physics2D.LinecastAll(lastPosition, currentPosition);

            foreach (var hit in hits)
            {
                Collider2D col = hit.collider;
                if (col == null || col.gameObject == gameObject) continue;

                // Bỏ qua va chạm với Boss chủ sở hữu và Enemy đồng bọn
                if (IsEnemyOrOwner(col.gameObject))
                {
                    continue;
                }

                HandleCollision(col);
                if (isDestroyed) break;
            }

            // Cập nhật lại vị trí cuối cùng của đạn
            lastPosition = currentPosition;
        }

        // Hàm kiểm tra xem đối tượng có phải là Enemy hoặc chủ sở hữu đạn hay không (KHÔNG DÙNG CompareTag "Enemy" để tránh lỗi Tag)
        private bool IsEnemyOrOwner(GameObject obj)
        {
            if (obj == null) return false;

            // Kiểm tra nếu là Owner
            if (obj == owner || (owner != null && obj.transform.IsChildOf(owner.transform)))
            {
                return true;
            }

            // Kiểm tra Layer và Component
            if (obj.layer == LayerMask.NameToLayer("Enemy") ||
                obj.GetComponent<Enemies.Core.Enemy>() != null ||
                obj.GetComponentInParent<Enemies.Core.Enemy>() != null)
            {
                return true;
            }

            return false;
        }

        // Hàm cho phép Boss truyền sát thương cụ thể của Boss vào viên đạn
        public void SetDamage(int damageAmount)
        {
            damage = damageAmount;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision != null)
            {
                if (IsEnemyOrOwner(collision.gameObject)) return;
                HandleCollision(collision);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision != null && collision.collider != null)
            {
                if (IsEnemyOrOwner(collision.gameObject)) return;
                HandleCollision(collision.collider);
            }
        }

        private void HandleCollision(Collider2D col)
        {
            if (isDestroyed || col == null) return;

            GameObject other = col.gameObject;

            // 1. Kiểm tra nếu va chạm với Player
            Player.PlayerController player = other.GetComponent<Player.PlayerController>();
            if (player == null)
            {
                player = other.GetComponentInParent<Player.PlayerController>();
            }

            if (player != null)
            {
                isDestroyed = true;

                // Gây sát thương cho Player nếu Player không ở trạng thái bất tử (đang dash...)
                if (!player.IsInvulnerable)
                {
                    player.TakeDamage(damage);
                    Debug.Log($"[BossBullet] Đạn bắn trúng Player! Gây {damage} sát thương.");
                }

                // Hủy quả cầu đạn
                Destroy(gameObject);
                return;
            }

            // 2. Kiểm tra va chạm với môi trường (ví dụ: Tường, Mặt đất)
            // Nếu chạm phải vật cản vật lý cứng (không phải Trigger) thì tự hủy
            if (!col.isTrigger)
            {
                isDestroyed = true;
                Debug.Log($"[BossBullet] Đạn va chạm với vật cản {other.name} và tự hủy.");
                Destroy(gameObject);
            }
        }

        // Vẽ Gizmos hiển thị đường đi của đạn trong Scene để dễ debug
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(lastPosition, (Vector2)transform.position);
        }
    }
}
