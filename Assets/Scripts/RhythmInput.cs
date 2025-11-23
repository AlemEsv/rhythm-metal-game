using UnityEngine;
using System;

public class RhythmInput : MonoBehaviour
{
    [Header("Tolerancia y Buffer")]
    [Tooltip("Ventana de tiempo en segundos para aceptar el input")]
    public float toleranceSeconds = 0.10f; // Reducido de 0.15f para mayor precisión
    
    [Tooltip("Tiempo máximo en segundos antes del beat para bufferear el input")]
    public float bufferTimeBeforeBeat = 0.1f;

    [Tooltip("Porcentaje mínimo de precisión para que el movimiento cuente (0-1)")]
    [Range(0f, 1f)]
    public float minAccuracyThreshold = 0.4f;

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
        // Detectar input del jugador
        DetectInput();
        
        // Procesar input bufferado
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
        if (Conductor.Instance == null) 
        {
            Debug.LogWarning("RhythmInput: No hay instancia de Conductor");
            return;
        }

        // Calculamos la distancia al beat más cercano
        float beatDiff = Conductor.Instance.GetDistanceToNearestBeat();
        float timeDiff = beatDiff * Conductor.Instance.secPerBeat;
        
        // Calculamos cuál es el siguiente beat
        float currentBeat = Conductor.Instance.songPositionInBeats;
        float targetBeat = Mathf.Round(currentBeat);
        
        // Si estamos antes del beat (timeDiff negativo) y dentro del buffer
        if (timeDiff < 0 && Mathf.Abs(timeDiff) <= bufferTimeBeforeBeat)
        {
            // BUFFER: Guardamos el input para ejecutarlo en el beat exacto
            bufferedInput = direction;
            hasBufferedInput = true;
            nextBeatToExecute = targetBeat;
            
            Debug.Log($"<color=yellow>Input Bufferado ({Mathf.Abs(timeDiff):F3}s antes del beat)</color>");
            return;
        }
        
        // Si estamos dentro de la ventana de tolerancia
        if (Mathf.Abs(timeDiff) <= toleranceSeconds)
        {
            // ÉXITO: Ejecutar inmediatamente
            ExecuteInput(direction, timeDiff);
        }
        else
        {
            // FALLO: Fuera de la ventana de tiempo
            Debug.Log($"<color=red> Fallaste por {Mathf.Abs(timeDiff):F3}s (Tolerancia: {toleranceSeconds}s)</color>");
            OnInputFail?.Invoke();
        }
    }

    void ProcessBufferedInput()
    {
        if (!hasBufferedInput) return;
        if (Conductor.Instance == null) return;

        float currentBeat = Conductor.Instance.songPositionInBeats;
        
        // Si hemos alcanzado o pasado el beat objetivo
        if (currentBeat >= nextBeatToExecute)
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
            Debug.Log($"<color=orange>Input ignorado. Precisión: {accuracy*100:F0}% (Requerido: {minAccuracyThreshold*100}%)</color>");
            OnInputFail?.Invoke();
            return;
        }
        
        string feedbackColor = normalizedError < 0.3f ? "lime" : "green";
        string feedbackMsg = normalizedError < 0.3f ? "¡PERFECTO!" : "¡Bien!";
        
        Debug.Log($"<color={feedbackColor}>✓ {feedbackMsg} (Precisión: {accuracy*100:F0}%)</color>");
        
        // Disparar eventos
        OnMovementInput?.Invoke(direction);
        OnInputSuccess?.Invoke();
    }

    // Método público para obtener la precisión actual (útil para UI)
    public float GetCurrentBeatProximity()
    {
        if (Conductor.Instance == null) return 1f;
        
        float beatDiff = Conductor.Instance.GetDistanceToNearestBeat();
        float timeDiff = Mathf.Abs(beatDiff * Conductor.Instance.secPerBeat);
        
        return Mathf.Clamp01(timeDiff / toleranceSeconds);
    }
}