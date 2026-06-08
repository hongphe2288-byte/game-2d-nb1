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

        private float startY;
        private float randomOffset;

        protected override void Start()
        {
            base.Start();
            startY = transform.position.y;
            randomOffset = Random.Range(0f, 100f);
            
            Debug.Log($"[BatEnemy] {gameObject.name} (Dơi) đã sẵn sàng hoạt động!");
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
