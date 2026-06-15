using UnityEngine;

namespace Player
{
    public class PlayerCollision : MonoBehaviour
    {
        private PlayerController player;
        private Core.GameManager gameManager;
        private AudioManager audioManager;

        private void Awake()
        {
            player = GetComponent<PlayerController>();
            gameManager = FindAnyObjectByType<Core.GameManager>();
            audioManager = FindAnyObjectByType<AudioManager>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            HandleCollision(collision.gameObject);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            HandleCollision(collision.gameObject);
        }

        private void HandleCollision(GameObject other)
        {
            if (other.CompareTag("Deathzone"))
            {
                if (player != null && player.Respawn != null && !player.IsInvulnerable)
                {
                    player.Respawn.FallIntoPit(20);
                }
                return;
            }

            if (other.CompareTag("Checkpoint"))
            {
                if (player != null && player.Respawn != null)
                {
                    player.Respawn.UpdateCheckpoint(other.transform.position);
                    Destroy(other); // Phá huỷ checkpoint sau khi ăn để biến mất
                }
                return;
            }

            if (other.CompareTag("Trap"))
            {
                // Deal damage to player, but only if they are not invulnerable (e.g. dashing)
                if (player != null && !player.IsInvulnerable)
                {
                    int trapDamage = 20; // Default trap damage, customizable
                    player.TakeDamage(trapDamage);
                    audioManager.PlayDamageSound();
                }
            }
        }
    }
}
