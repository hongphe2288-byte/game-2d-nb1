using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        private int score = 0;
        // [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private GameObject gameOverUi;
        private bool isGameOver = false;

        void Start()
        {
            if (gameOverUi != null)
            {
                gameOverUi.SetActive(false);
            }
            // UpdateScore();
        }

        public void AddScore(int points)
        {
            if (!isGameOver)
            {
                score += points;
                // UpdateScore();
            }
        }

        // private void UpdateScore()
        // {
        //     if (scoreText != null)
        //     {
        //         scoreText.text = score.ToString();
        //     }
        // }

        public void GameOver()
        {
            isGameOver = true;
            Time.timeScale = 0;
            if (gameOverUi != null)
            {
                gameOverUi.SetActive(true);
            }
        }

        public void RestartGame()
        {
            isGameOver = false;
            score = 0;
            // UpdateScore();
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public bool IsGameOver()
        {
            return isGameOver;
        }

        // Hitstop effect (time freeze on hit)
        public void TriggerHitstop(float duration)
        {
            StartCoroutine(HitstopCoroutine(duration));
        }

        private IEnumerator HitstopCoroutine(float duration)
        {
            float originalTimeScale = Time.timeScale;
            Time.timeScale = 0f; // Freeze time

            // Use WaitForSecondsRealtime because Time.timeScale is 0
            yield return new WaitForSecondsRealtime(duration);

            if (!isGameOver)
            {
                Time.timeScale = 1f;
            }
        }

        // Screen Shake trigger
        public void TriggerScreenShake(float intensity)
        {
            Debug.Log($"[Screen Shake] Shake triggered with intensity: {intensity}");
            // In a full implementation, you would do:
            // CinemachineImpulseSource.GenerateImpulse(intensity);
            // Or custom camera shaking logic
        }
    }
}
