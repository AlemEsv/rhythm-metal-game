using UnityEngine;
using System;

public class RhythmInput : MonoBehaviour
{
    [Header("Tolerancia y Buffer")]
    public float toleranceSeconds = 0.20f;
    public float bufferTimeBeforeBeat = 0.15f;

    [Tooltip("Porcentaje mínimo de precisión")]
    [Range(0f, 1f)]
    public float minAccuracyThreshold = 0.3f;

    // Eventos
    public static event Action<Vector2> OnMovementInput;
    public static event Action OnInputSuccess;
    public static event Action OnInputFail;

    // Sistema de Buffer
    private Vector2 bufferedInput = Vector2.zero;
    private bool hasBufferedInput = false;
    private float nextBeatToExecute = -1f;

    void Update()
    {
        DetectInput();
        ProcessBufferedInput();
    }

    void DetectInput()
    {
        Vector2 input = Vector2.zero;

        // Detección de teclas WASD y Flechas
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) 
            input = Vector2.up;
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) 
            input = Vector2.down;
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) 
            input = Vector2.left;
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) 
            input = Vector2.right;

        if (input != Vector2.zero)
        {
            ValidateInput(input);
        }
    }

    void ValidateInput(Vector2 direction)
    {
        if (Conductor.Instance == null) return;

        // distancia al beat más cercano
        float beatDiff = Conductor.Instance.GetDistanceToNearestBeat();
        float timeDiff = beatDiff * Conductor.Instance.SecPerBeat;
        
        // Calculamos cuál es el siguiente beat
        float currentBeat = Conductor.Instance.SongPositionInBeats;
        float targetBeat = Mathf.Round(currentBeat);

        // ¿Es un golpe válido?
        if (Mathf.Abs(timeDiff) <= toleranceSeconds)
        {
            // Limpiamos cualquier buffer pendiente para no duplicar acciones
            hasBufferedInput = false;
            ExecuteInput(direction, timeDiff);
            return;
        }

        // ¿Es demasiado pronto, pero intención válida?
        if (timeDiff < 0 && Mathf.Abs(timeDiff) <= (toleranceSeconds + bufferTimeBeforeBeat))
        {
            bufferedInput = direction;
            hasBufferedInput = true;
            nextBeatToExecute = targetBeat;
            Debug.Log($"<color=yellow>Muy temprano</color>");
            return;
        }
        else
        {
            // Demasiado tarde o demasiado pronto fuera de rango
            Debug.Log($"<color=red>Fallaste por {Mathf.Abs(timeDiff):F3}s</color>");
            OnInputFail?.Invoke();
        }
    }

    void ProcessBufferedInput()
    {
        if (!hasBufferedInput) return;
        if (Conductor.Instance == null) return;

        float currentBeat = Conductor.Instance.SongPositionInBeats;
        // margen de seguridad 0.05
        if (currentBeat >= nextBeatToExecute - 0.05f)
        {
            ExecuteInput(bufferedInput, 0f); // Ejecutamos con timing perfecto
            
            // Limpiamos el buffer
            hasBufferedInput = false;
            bufferedInput = Vector2.zero;
            nextBeatToExecute = -1f;
        }
    }

    void ExecuteInput(Vector2 direction, float timeDifference)
    {
        // Calculamos el error normalizado (0 = perfecto, 1 = límite de tolerancia)
        float normalizedError = Mathf.Abs(timeDifference) / toleranceSeconds;
        float accuracy = 1.0f - normalizedError;

        // Filtro de precisión mínima
        if (accuracy < minAccuracyThreshold)
        {
            OnInputFail?.Invoke();
            return;
        }


        string rating = accuracy > 0.8f ? "PERFECT" : (accuracy > 0.5f ? "GOOD" : "OK");
        Debug.Log($"<color=green>{rating} - {accuracy * 100:F0}%</color>");

        OnMovementInput?.Invoke(direction);
        OnInputSuccess?.Invoke();
    }

    public float GetCurrentBeatProximity()
    {
        if (Conductor.Instance == null) return 1f;
        
        float beatDiff = Conductor.Instance.GetDistanceToNearestBeat();
        float timeDiff = Mathf.Abs(beatDiff * Conductor.Instance.SecPerBeat);
        
        return Mathf.Clamp01(timeDiff / toleranceSeconds);
    }
}