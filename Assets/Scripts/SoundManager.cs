using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    public AudioSource backgroundMusic;
    public AudioClip winMusicClip; 

    private AudioSource audioSource;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>(); 
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayBackgroundMusic()
    {
        if (!backgroundMusic.isPlaying)
        {
            backgroundMusic.Play();
        }
    }

    public void StopBackgroundMusic()
    {
        backgroundMusic.Stop();
    }

    
    public void PlayWinMusic()
    {
        if (audioSource != null && winMusicClip != null)
        {
            audioSource.Stop(); // Stop current music or sounds
            audioSource.clip = winMusicClip; // Set the win music clip
            audioSource.Play(); // Play the win music
        }
    }
}
