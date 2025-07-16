using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("Pause Menu UI")]
    public GameObject pauseMenuPanel; // The panel containing the pause menu UI
    public CanvasGroup gameplayUICanvasGroup; // Canvas group for gameplay UI elements
    
    [Header("Buttons")]
    public Button resumeButton;
    public Button retryButton;
    public Button mainMenuButton;
    
    [Header("Settings")]
    public string mainMenuSceneName = "MainMenu"; // Name of your main menu scene
    public float gameplayUIAlphaDuringPause = 0.3f; // How visible the gameplay UI should be when paused (0-1)
    
    private bool isPaused = false;
    private float originalGameplayUIAlpha = 1f;
    
    void Start()
    {
        // Make sure the pause menu is hidden at start
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
        
        // Store original alpha if we have a gameplay UI canvas group
        if (gameplayUICanvasGroup != null)
        {
            originalGameplayUIAlpha = gameplayUICanvasGroup.alpha;
        }
        
        // Add button listeners if the buttons exist
        if (resumeButton != null) 
        {
            resumeButton.onClick.AddListener(ResumeGame);
        }
        if (retryButton != null) 
        {
            retryButton.onClick.AddListener(RetryLevel);
        }
        if (mainMenuButton != null) 
        {
            mainMenuButton.onClick.AddListener(QuitToMainMenu);
        }
    }

    void Update()
    {
        // Toggle pause with Escape key
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
    
    public void TogglePause()
    {
        isPaused = !isPaused;
        
        if (isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }
    
    public void PauseGame()
    {
        // Set the time scale to 0 (pauses the game)
        Time.timeScale = 0f;
        
        // Show the pause menu
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true);
        }
        
        // Dim the gameplay UI elements
        if (gameplayUICanvasGroup != null)
        {
            gameplayUICanvasGroup.alpha = gameplayUIAlphaDuringPause;
        }
        
        // Update the GameManager if it exists
        if (GameManager.instance != null)
        {
            GameManager.instance.isGamePaused = true;
        }
        
        // Disable player controls here if needed

        // Dim the gameplay UI if we have a canvas group assigned
        if (gameplayUICanvasGroup != null)
        {
            gameplayUICanvasGroup.alpha = gameplayUIAlphaDuringPause;
            gameplayUICanvasGroup.interactable = false;
            gameplayUICanvasGroup.blocksRaycasts = false;
        }
    }
    
    public void ResumeGame()
    {
        // Set the time scale back to 1 (resumes the game)
        Time.timeScale = 1f;
        
        // Hide the pause menu
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
        
        // Restore the gameplay UI elements to full visibility
        if (gameplayUICanvasGroup != null)
        {
            gameplayUICanvasGroup.alpha = originalGameplayUIAlpha;
        }
        
        // Update the GameManager if it exists
        if (GameManager.instance != null)
        {
            GameManager.instance.isGamePaused = false;
        }
        
        isPaused = false;
        
        // Re-enable player controls here if needed

        // Restore the original gameplay UI alpha
        if (gameplayUICanvasGroup != null)
        {
            gameplayUICanvasGroup.alpha = originalGameplayUIAlpha;
            gameplayUICanvasGroup.interactable = true;
            gameplayUICanvasGroup.blocksRaycasts = true;
        }
    }
    
    public void RetryLevel()
    {
        // Reset time scale to normal
        Time.timeScale = 1f;
        
        // Reload the current scene
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }
    
    public void QuitToMainMenu()
    {
        // Reset time scale to normal
        Time.timeScale = 1f;
        
        // Load the main menu scene
        SceneManager.LoadScene(mainMenuSceneName);
    }
}
