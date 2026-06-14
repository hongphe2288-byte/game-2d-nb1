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
            //if (other.CompareTag("Coin"))
            //{
            //    Destroy(other);
            //    if (gameManager != null)
            //    {
            //        gameManager.AddScore(1);
            //    }
            //}

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
            //else if (other.GetComponent<Enemies.Core.Enemy>() != null)
            //{
            //    // Deal damage to player when colliding with an enemy, if not invulnerable
            //    if (player != null && !player.IsInvulnerable)
            //    {
            //        int enemyDamage = 15; // Default enemy contact damage, customizable
            //        player.TakeDamage(enemyDamage);
            //    }
            //}
        }
    }
}
