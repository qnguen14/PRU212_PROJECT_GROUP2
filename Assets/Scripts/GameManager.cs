using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool isGameActive = true; // Track if the game is active
    public bool isGamePaused = false; // Track if the game is paused

    // Sound management (optional - if you want centralized sound control)
    [UnitHeaderInspectable("Sound Settings")]
    public float masterVolume = 0.02f;
    public bool soundEnabled = true;
    
    [Header("Background Music")]
    public AudioClip backgroundMusic;
    public float musicVolume = 0.02f;
    private AudioSource musicSource;

    // Singleton pattern to access GameManager from anywhere
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Setup background music
            SetupBackgroundMusic();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void SetupBackgroundMusic()
    {
        // Add AudioSource component if it doesn't exist
        musicSource = GetComponent<AudioSource>();
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Configure the audio source for background music
        musicSource.clip = backgroundMusic;
        musicSource.volume = musicVolume * masterVolume;
        musicSource.loop = true;
        musicSource.playOnAwake = true;
        
        if (soundEnabled && backgroundMusic != null)
        {
            musicSource.Play();
        }
    }

    // Method to play sound at a position
    public void PlaySoundAtPosition(AudioClip clip, Vector3 position, float volume = 0.2f)
    {
        if (soundEnabled && clip != null)
        {
            // Create a temporary game object
            GameObject tempGO = new GameObject("TempAudio");
            tempGO.transform.position = position;
            
            // Add an audio source to it
            AudioSource audioSource = tempGO.AddComponent<AudioSource>();
            audioSource.clip = clip;
            audioSource.volume = volume * masterVolume;
            audioSource.spatialBlend = 1f; // Make it 3D sound
            audioSource.Play();
            
            // Destroy the game object after the clip has finished playing
            Destroy(tempGO, clip.length);
        }
    }
    
    // Methods to control background music
    public void PlayBackgroundMusic()
    {
        if (soundEnabled && !musicSource.isPlaying && backgroundMusic != null)
        {
            musicSource.Play();
        }
    }
    
    public void StopBackgroundMusic()
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (musicSource != null)
        {
            musicSource.volume = musicVolume * masterVolume;
        }
    }
}
