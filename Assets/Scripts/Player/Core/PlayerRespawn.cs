using UnityEngine;

namespace Player
{
    public class PlayerRespawn : MonoBehaviour
    {
        private PlayerController player;
        // XÓA BIẾN activeCheckpoint CŨ ĐI, ta sẽ dùng GameManager.instance.lastCheckpointPos

        private void Awake()
        {
            player = GetComponent<PlayerController>();
        }

        private void Start()
        {
            // Nếu đây là lần đầu vào game (GameManager chưa lưu checkpoint nào)
            // thì mới lấy vị trí mặc định đầu map của Player làm checkpoint đầu tiên
            if (GameManager.instance.lastCheckpointPos == Vector2.zero)
            {
                GameManager.instance.lastCheckpointPos = transform.position;
            }
            else
            {
                // Nếu trước đó đã ăn checkpoint rồi rớt vực chết/load lại scene,
                // Tự động đưa Player về đúng vị trí checkpoint đã lưu trong GameManager ngay khi map load xong
                transform.position = GameManager.instance.lastCheckpointPos;
            }
        }

        public void UpdateCheckpoint(Vector2 newPosition)
        {
            // LƯU VÀO GAMEMANAGER: Không sợ bị mất khi chết hẳn hoặc load lại màn
            GameManager.instance.lastCheckpointPos = newPosition;
            Debug.Log($"[Checkpoint] GameManager đã lưu vị trí mới: {newPosition}");
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

                // DỊCH CHUYỂN VỀ CHECKPOINT CỦA GAMEMANAGER
                transform.position = GameManager.instance.lastCheckpointPos;
                Debug.Log($"[Respawn] Hồi sinh tại checkpoint: {GameManager.instance.lastCheckpointPos}. HP hiện tại: {player.CurrentHealth}");
            }
            else
            {
                // Xử lý khi người chơi chết hẳn ở đây (ví dụ: Gọi hàm hiện màn hình GameOver)
                Debug.Log("[Respawn] Người chơi đã hết máu và chết hẳn!");
            }
        }
    }
}