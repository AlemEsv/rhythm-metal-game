using UnityEngine;
using DG.Tweening;

public class PauseManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pausePanel;

    [Header("Blur Settings")]
    public UnityEngine.Rendering.Volume postProcessVolume;

    private bool isPaused = false;
    private PlayerCombat playerCombat;

    void Start()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        playerCombat = FindFirstObjectByType<PlayerCombat>();
    }

    void Update()
    {
        if (playerCombat != null && !playerCombat.IsAlive) return;

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

    void PauseGame()
    {
        Time.timeScale = 0f; // Detiene el tiempo del juego
        if (pausePanel != null) pausePanel.SetActive(true);

        // Habilitar cursor
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Si usas música, puedes pausarla aquí
        if (Conductor.Instance != null) Conductor.Instance.musicSource.Pause();
    }

    void ResumeGame()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        Time.timeScale = 1f; // Reanuda el tiempo

        // Ocultar cursor si es necesario para tu juego
        // Cursor.visible = false;
        // Cursor.lockState = CursorLockMode.Locked;

        // Reanudar música
        if (Conductor.Instance != null) Conductor.Instance.musicSource.UnPause();
    }
}