using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameOverFade : MonoBehaviour
{
    [Header("Settings")]
    public Image blackOverlay;
    public float fadeDuration = 2.0f;
    public float delayBeforeFade = 0.5f;

    [Header("UI")]
    public GameObject gameOverContainer;
    public Button retryButton;
    public Button exitButton;

    void Start()
    {
        if (blackOverlay != null)
        {
            blackOverlay.color = new Color(0, 0, 0, 0);
            blackOverlay.gameObject.SetActive(false);
        }

        if (gameOverContainer != null)
        {
            gameOverContainer.SetActive(false);

            CanvasGroup cg = gameOverContainer.GetComponent<CanvasGroup>();
            if (cg != null) cg.alpha = 0;
        }

        // Asignar funciones a los botones
        if (retryButton != null) retryButton.onClick.AddListener(RetryGame);
        if (exitButton != null) exitButton.onClick.AddListener(ExitGame);
    }
    public void FadeOut()
    {
        if (blackOverlay == null) return;
        blackOverlay.gameObject.SetActive(true);

        // fade a negro
        blackOverlay.DOFade(1f, fadeDuration)
                    .SetDelay(delayBeforeFade)
                    .SetEase(Ease.InOutQuad)
                    .SetUpdate(true) 
                    .OnComplete(ShowGameOverUI);
    }

    void ShowGameOverUI()
    {
        if (gameOverContainer != null)
        {
            gameOverContainer.SetActive(true);

            // Animar la entrada del Game Over
            CanvasGroup cg = gameOverContainer.GetComponent<CanvasGroup>();
            if (cg != null)
            {
                cg.DOFade(1f, 1f).SetUpdate(true);
            }
        }

        // Hacemos visible el cursor para poder clicar
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void RetryGame()
    {
        Time.timeScale = 1f; // Asegurar que el tiempo vuelve a la normalidad
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitGame()
    {
        Time.timeScale = 1f;
        // Carga el menú principal
        SceneManager.LoadScene("MainMenuScene");
        // Application.Quit();
    }
}