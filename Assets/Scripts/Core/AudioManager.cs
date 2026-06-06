using UnityEngine;

namespace Core
{
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager instance;
        public static AudioManager Instance => instance;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void PlaySFX(string soundName)
        {
            Debug.Log($"Playing sound effect: {soundName}");
        }

        public void PlayBGM(string musicName)
        {
            Debug.Log($"Playing background music: {musicName}");
        }
    }
}
