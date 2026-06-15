using UnityEngine;

namespace Player
{
    public class PlayerRespawn : MonoBehaviour
    {
        private PlayerController player;
        private Vector2 activeCheckpoint;

        private void Awake()
        {
            player = GetComponent<PlayerController>();
        }

        private void Start()
        {
            // Thiết lập checkpoint mặc định tại vị trí ban đầu của người chơi khi bắt đầu màn
            activeCheckpoint = transform.position;
        }

        public void UpdateCheckpoint(Vector2 newPosition)
        {
            activeCheckpoint = newPosition;
            Debug.Log($"[Checkpoint] Checkpoint được cập nhật thành: {activeCheckpoint}");
        }

        public void FallIntoPit(int damage)
        {
            if (player == null) return;

            // 1. Trừ máu
            player.TakeDamage(damage);

            // 2. Nếu người chơi chưa chết, dịch chuyển về checkpoint
            if (player.CurrentHealth > 0)
            {
                // Chuyển FSM về trạng thái Idle để reset các vận tốc và animation
                if (player.StateMachine != null && player.IdleState != null)
                {
                    player.StateMachine.ChangeState(player.IdleState);
                }

                // Reset lực vật lý để dừng hoàn toàn đà rơi
                if (player.Rb != null)
                {
                    player.SetVelocity(0f, 0f);
                    player.Rb.angularVelocity = 0f;
                }

                // Dịch chuyển nhân vật về checkpoint
                transform.position = activeCheckpoint;
                Debug.Log($"[Respawn] Hồi sinh tại checkpoint: {activeCheckpoint}. HP hiện tại: {player.CurrentHealth}");
            }
        }
    }
}
