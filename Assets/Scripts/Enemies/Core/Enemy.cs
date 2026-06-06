using UnityEngine;

namespace Enemies.Core
{
    public class Enemy : MonoBehaviour
    {
        [Header("Enemy Stats")]
        [SerializeField] private int maxHealth = 50;
        private int currentHealth;

        private void Start()
        {
            currentHealth = maxHealth;
        }

        public void TakeDamage(int damage)
        {
            currentHealth -= damage;
            Debug.Log($"{gameObject.name} took {damage} damage. Current HP: {currentHealth}");

            // Basic visual cue or reaction can be added here
            
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        protected virtual void Die()
        {
            Debug.Log($"{gameObject.name} died.");
            Destroy(gameObject);
        }
    }
}
