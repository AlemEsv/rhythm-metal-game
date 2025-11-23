using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public string gameSceneName = "SampleScene";
    public string calibrationSceneName = "CalibrationScene";

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