using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    public void LoadLevel()
    {
        SceneManager.LoadScene(1); // Load the specified scene
    }
}
