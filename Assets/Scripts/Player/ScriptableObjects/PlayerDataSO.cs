using UnityEngine;

namespace Player
{
    [CreateAssetMenu(fileName = "NewPlayerData", menuName = "Player/Player Data")]
    public class PlayerDataSO : ScriptableObject
    {
        [Header("Player Health")]
        public int maxHealth = 100;

        [Header("Movement Stats")]
        public float moveSpeed = 8f;
        public float jumpForce = 15f;
        public float groundCheckRadius = 0.2f;
        public LayerMask groundLayer;

        [Header("Double Jump")]
        public int maxJumps = 2;

        [Header("Dash Stats")]
        public float dashSpeed = 20f;
        public float dashDuration = 0.2f;
        public float dashCooldown = 1f;

        [Header("Attack Stats")]
        public int[] damagePerComboStep = { 20, 25, 40 };
        public float attackRadius = 0.8f;
        public Vector2 attackOffset = new Vector2(0.5f, 0f);
        public float comboWindow = 0.8f;
        public float hitstopDuration = 0.05f;
        public float screenShakeIntensity = 0.1f;
    }
}
