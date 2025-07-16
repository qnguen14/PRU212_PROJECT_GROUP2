using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject instructionPanel;
    public void StartGame()
    {
        SceneManager.LoadScene("SampleScene"); 
    }

    public void ShowInstruction()
    {
        instructionPanel.SetActive(true);
    }

    public void HideInstruction()
    {
        instructionPanel.SetActive(false);
    }
}
