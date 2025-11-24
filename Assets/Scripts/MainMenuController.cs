using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public string gameSceneName = "SampleScene";
    public string calibrationSceneName = "CalibrationScene";
    public string optionsSceneName = "OptionsScene";
    public string mainSceneName = "MainScene";

    void Update()
    {
        // Navegar al MainScene con ENTER o ESPACIO
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            GoToMainScene();
        }
    }

    public void GoToMainScene()
    {
        if (SceneLoader.Instance != null)
            SceneLoader.Instance.LoadScene(mainSceneName);
        else
            SceneManager.LoadScene(mainSceneName);
    }

    public void GoToOptions()
    {
        if (SceneLoader.Instance != null)
            SceneLoader.Instance.LoadScene(optionsSceneName);
        else
            SceneManager.LoadScene(optionsSceneName);
    }

    // recibe la DATA de la canción
    public void PlayLevel(SongData songToPlay)
    {
        // Guardar la elección
        GameManager.selectedSongData = songToPlay;

        // Cargar la escena
        if (SceneLoader.Instance != null)
            SceneLoader.Instance.LoadScene(gameSceneName);
        else
            SceneManager.LoadScene(gameSceneName);
    }

    public void GoToCalibration()
    {
        if (SceneLoader.Instance != null)
            SceneLoader.Instance.LoadScene(calibrationSceneName);
        else
            SceneManager.LoadScene(calibrationSceneName);
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
}