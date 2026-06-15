using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource backgroundMusic;
    [SerializeField] private AudioSource effectAudioSource;
    [SerializeField] private AudioClip backGroundClip;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip doubleJumbClip;
    [SerializeField] private AudioClip dashClip;
    [SerializeField] private AudioClip attackClip;
    [SerializeField] private AudioClip damageClip;
    [SerializeField] private AudioClip runClip;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayBackgroundMusic();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PlayBackgroundMusic()
    {
        if (backgroundMusic.clip != backGroundClip)
        {
            backgroundMusic.clip = backGroundClip;
            backgroundMusic.Play();
        }
    }
    public void PlayJumpSound()
    {
        effectAudioSource.PlayOneShot(jumpClip);
    }
    public void PlayDoubleJumpSound()
    {
        effectAudioSource.PlayOneShot(doubleJumbClip);
    }
    public void PlayDashSound()
    {
        effectAudioSource.PlayOneShot(dashClip);
    }
    public void PlayAttackSound()
    {
        effectAudioSource.PlayOneShot(attackClip);
    }
    public void PlayDamageSound()
    {
        effectAudioSource.PlayOneShot(damageClip);
    }
    public void PlayRunSound()
    {
        effectAudioSource.PlayOneShot(runClip);
    } 
}
