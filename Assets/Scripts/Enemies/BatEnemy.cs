using UnityEngine;

namespace Enemies
{
    /// <summary>
    /// Script đại diện cho Enemy Dơi (Bat Enemy).
    /// Kế thừa toàn bộ hành vi cốt lõi từ lớp cơ sở Enemies.Core.Enemy
    /// bao gồm tự động di chuyển AIPath, nhận sát thương, trạng thái bị tấn công, và bị choáng.
    /// </summary>
    public class BatEnemy : Core.Enemy
    {
        [Header("Bat Specific Settings")]
        [SerializeField] private float hoverAmplitude = 0.5f; // Biên độ dao động bay lượn
        [SerializeField] private float hoverFrequency = 2f;   // Tần số dao động bay lượn
        [SerializeField] private float activationRadius = 8f;  // Bán kính kích hoạt Bat hoạt động giống như boss

        private float startY;
        private float randomOffset;
        private bool isActivated = false;

        protected override void Start()
        {
            base.Start();
            startY = transform.position.y;
            randomOffset = Random.Range(0f, 100f);
            
            // Ban đầu chưa kích hoạt, vô hiệu hóa di chuyển của AIPath
            if (aiPath != null)
            {
                aiPath.canMove = false;
            }
            
            Debug.Log($"[BatEnemy] {gameObject.name} (Dơi) đã sẵn sàng hoạt động! Đang chờ Player trong bán kính {activationRadius}m...");
        }

        protected override void Update()
        {
            if (isDead) return;

            if (player == null) return;

            // Kiểm tra trạng thái kích hoạt dựa trên khoảng cách tới Player
            if (!isActivated)
            {
                float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
                if (distanceToPlayer <= activationRadius)
                {
                    ActivateBat();
                }
                else
                {
                    // Đứng yên ở trạng thái Idle trước khi kích hoạt
                    SetAnimBool(idleAnimParam, true);
                    return;
                }
            }

            // Khi đã kích hoạt, chạy logic Update bình thường của base
            base.Update();
        }

        private void ActivateBat()
        {
            isActivated = true;
            if (aiPath != null)
            {
                aiPath.canMove = true;
            }
            Debug.Log($"[BatEnemy] Player đã đi vào phạm vi kích hoạt! Bat {gameObject.name} thức tỉnh!");
        }

        public override void TakeDamage(int damageTaken)
        {
            if (!isActivated && !isDead)
            {
                ActivateBat();
            }

            base.TakeDamage(damageTaken);
        }

        // Ví dụ ghi đè cách di chuyển: Thêm hiệu ứng bay nhấp nhô (hover) cho Dơi
        protected override void MoveBehavior()
        {
            base.MoveBehavior();

            // Nếu muốn thêm hiệu ứng bay nhấp nhô nhẹ theo hình sin
            if (!isHit && !isAttacking && !isDead)
            {
                // Nhấp nhô trục Y bằng cách thêm lực hoặc trực tiếp thay đổi vị trí đồ họa của Sprite
                // Ở đây chúng ta chỉ sử dụng AIPath mặc định để di chuyển, 
                // nhưng nếu muốn tùy biến thêm bạn có thể ghi đè logic tại đây.
            }
        }

        // Ví dụ về việc thay đổi điều kiện tấn công hoặc hành động tấn công đặc biệt của Dơi
        protected override void StartAttack()
        {
            // Có thể thêm hiệu ứng âm thanh đập cánh hoặc rít lên của Dơi tại đây trước khi đánh
            Debug.Log($"[BatEnemy] {gameObject.name} rít lên và lao vào tấn công!");
            
            base.StartAttack();
        }
    }
}
