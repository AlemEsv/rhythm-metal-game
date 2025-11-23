using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour
{
    [Header("Video")]
    public VideoPlayer videoPlayer;

    [Header("Navegación")]
    public string nextSceneName = "CalibrationScene"; 

    void Start()
    {
        // Configuración de seguridad para el video
        if (videoPlayer != null)
        {
            videoPlayer.isLooping = true; 
            videoPlayer.Play();
        }
    }

    void Update()
    {
        // Solo detectamos el Input para cambiar de escena
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LoadNextScene();
        }
    }

    void LoadNextScene()
    {
        // Usamos tu SceneLoader inteligente
        if (SceneLoader.Instance != null)
        {
            SceneLoader.Instance.LoadScene(nextSceneName);
        }
        else
        {
            // Fallback por si pruebas la escena sola
            SceneManager.LoadScene(nextSceneName);
        }
    }
}