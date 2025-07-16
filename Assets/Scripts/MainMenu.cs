using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject instructionPanel;
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip backgroundMusic;

    [SerializeField]
    private Animator titleAnimator;

    [SerializeField]
    private float musicVolume = 0.5f;

    [SerializeField]
    private AudioClip buttonClickSound;

    [SerializeField]
    private bool isMuted = false;

    [SerializeField]
    private Sprite audioOnSprite;

    [SerializeField]
    private Sprite audioOffSprite;

    [SerializeField]
    private Image muteButtonImage;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Configure audio settings
        if (backgroundMusic != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.loop = true;
            audioSource.playOnAwake = true;
            audioSource.volume = musicVolume; // Set default volume here
            audioSource.Play();
        }

        if (titleAnimator != null)
        {
            titleAnimator.SetTrigger("Start");
        }

        // Load mute preference
        isMuted = PlayerPrefs.GetInt("MusicMuted", 0) == 1;

        // Apply mute state
        if (audioSource != null)
        {
            audioSource.mute = isMuted;
        }

        // Update button appearance
        if (muteButtonImage != null)
        {
            muteButtonImage.sprite = isMuted ? audioOffSprite : audioOnSprite;
        }
    }

    private void ButtonClickSound()
    {
        if (buttonClickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(buttonClickSound);
        }
    }

    public void StartGame()
    {
        ButtonClickSound();
        SceneManager.LoadScene("Level1");
    }

    public void ShowInstruction()
    {
        ButtonClickSound();
        instructionPanel.SetActive(true);
    }

    public void HideInstruction()
    {
        ButtonClickSound();
        instructionPanel.SetActive(false);
    }

    public void SetMusicVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = volume;
        }
    }

    public void QuitGame()
    {
        ButtonClickSound();
        Application.Quit();
        Debug.Log("Game is quitting...");
    }

    public void ToggleMute()
    {
        ButtonClickSound();
        isMuted = !isMuted;

        // Update audio state
        if (audioSource != null)
        {
            audioSource.mute = isMuted;
        }

        // Update button appearance
        if (muteButtonImage != null)
        {
            muteButtonImage.sprite = isMuted ? audioOffSprite : audioOnSprite;
        }

        // Optionally save mute preference
        PlayerPrefs.SetInt("MusicMuted", isMuted ? 1 : 0);
        PlayerPrefs.Save();
    }
}
