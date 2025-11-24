using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneController : MonoBehaviour
{
    public string optionsSceneName = "OptionsScene";
    public string gameSceneName = "SampleScene";

    // Método para botón Options
    public void GoToOptions()
    {
        if (SceneLoader.Instance != null)
            SceneLoader.Instance.LoadScene(optionsSceneName);
        else
            SceneManager.LoadScene(optionsSceneName);
    }

    // Método para botón Play
    public void GoToPlay()
    {
        if (SceneLoader.Instance != null)
            SceneLoader.Instance.LoadScene(gameSceneName);
        else
            SceneManager.LoadScene(gameSceneName);
    }

    // Método para volver al menú principal
    public void GoToMainMenu()
    {
        if (SceneLoader.Instance != null)
            SceneLoader.Instance.LoadScene("MainMenuScene");
        else
            SceneManager.LoadScene("MainMenuScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
