using UnityEngine;
using TMPro;
using DG.Tweening;

public class DoomsdayClock : MonoBehaviour
{
    [Header("Time")]
    public float totalTime = 60f;
    public float panicThreshold = 15f;

    [Header("References")]
    public TextMeshProUGUI timerText;

    [Header("Player")]
    public PlayerCombat playerCombat;

    private float currentTime;
    private bool isRunning = true;
    private int beatCounter = 0;

    void Start()
    {
        currentTime = totalTime;
        if (Conductor.Instance != null)
        {
            Conductor.Instance.OnBeat.AddListener(OnBeatHandler);
        }
        UpdateTimerDisplay();
    }

    void OnDestroy()
    {
        if (Conductor.Instance != null)
        {
            Conductor.Instance.OnBeat.RemoveListener(OnBeatHandler);
        }
        transform.DOKill();
    }

    void Update()
    {
        if (!isRunning) return;
        currentTime -= Time.deltaTime;

        UpdateTimerDisplay();

        if (currentTime <= 0)
        {
            currentTime = 0;
            UpdateTimerDisplay(); // Para que muestre 00:00 exacto
            TriggerDeath();
        }

        UpdateVisualState();
    }

    // da formato al timer
    void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            if (currentTime > 0)
            {
                int minutes = Mathf.FloorToInt(currentTime / 60F);
                int seconds = Mathf.FloorToInt(currentTime % 60F);

                timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
            }
            else
            {
                timerText.text = "00:00";
            }
        }
    }

    private void OnBeatHandler()
    {
        if (!isRunning) return;

        beatCounter++;

        bool isPanicMode = currentTime <= panicThreshold;

        // Fase 1: Cada 4 beats. Fase 2: Cada 2 beats.
        int beatInterval = isPanicMode ? 1 : 2;

        if (beatCounter % beatInterval == 0)
        {
            PerformBeatPulse(isPanicMode);
        }
    }

    private void PerformBeatPulse(bool isPanic)
    {
        // Fuerza del golpe
        float punchStrength = isPanic ? 0.3f : 0.2f;

        // Vibración visual
        if (timerText != null)
        {
            timerText.transform.DOKill(true);
            timerText.transform.DOPunchScale(Vector3.one * punchStrength, 0.2f, 1, 0.5f);
        }
    }

    private void UpdateVisualState()
    {
        if (timerText == null) return;

        // Si entramos en pánico
        if (currentTime <= panicThreshold)
        {
            timerText.color = Color.red;
        }
        else
        {
            timerText.color = Color.white;
        }
    }

    private void TriggerDeath()
    {
        if (!isRunning) return;
        isRunning = false;

        // Matar al jugador
        if (playerCombat != null)
        {
            playerCombat.Die();
        }
    }
}