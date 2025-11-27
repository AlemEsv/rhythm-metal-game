using UnityEngine;
using System;

public enum RhythmActionType
{
    Move,       // A / D (2 Bloques)
    Jump,       // W
    WallCling,  // SPACE
    AttackLeft, // Q
    AttackRight,// E
    Parry       // S
}

public class RhythmInput : MonoBehaviour
{
    [Header("Tolerancia")]
    public float toleranceSeconds = 0.25f;
    public float bufferTimeBeforeBeat = 0.15f;
    [Range(0f, 1f)] public float minAccuracyThreshold = 0.3f;

    // Eventos
    public static event Action<RhythmActionType, Vector2> OnCommandInput;
    public static event Action<float> OnInputSuccess;
    public static event Action OnInputFail;

    // Buffer simple
    private RhythmActionType bufferedAction;
    private Vector2 bufferedDirection;
    private bool hasBufferedInput = false;
    private float nextBeatToExecute = -1f;

    void Update()
    {
        if (Conductor.Instance == null) return;

        DetectInput();
        ProcessBufferedInput();
    }

    void DetectInput()
    {
        RhythmActionType action = RhythmActionType.Move; // Default
        Vector2 dir = Vector2.zero;
        bool hasInput = false;

        // --- MAPEO DE CONTROLES ---

        // MOVIMIENTO (A/D) -> 2 Bloques
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        { action = RhythmActionType.Move; dir = Vector2.left; hasInput = true; }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        { action = RhythmActionType.Move; dir = Vector2.right; hasInput = true; }

        // SALTO (W)
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        { action = RhythmActionType.Jump; dir = Vector2.up; hasInput = true; }

        // AGARRE (SPACE)
        else if (Input.GetKeyDown(KeyCode.Space))
        { action = RhythmActionType.WallCling; hasInput = true; }

        // COMBATE (Q, E, S)
        else if (Input.GetKeyDown(KeyCode.Q)) { action = RhythmActionType.AttackLeft; hasInput = true; }
        else if (Input.GetKeyDown(KeyCode.E)) { action = RhythmActionType.AttackRight; hasInput = true; }
        else if (Input.GetKeyDown(KeyCode.S)) { action = RhythmActionType.Parry; hasInput = true; }

        if (hasInput)
        {
            ValidateInput(action, dir);
        }
    }

    void ValidateInput(RhythmActionType action, Vector2 dir)
    {
        float beatDiff = Conductor.Instance.GetDistanceToNearestBeat();
        float timeDiff = beatDiff * Conductor.Instance.SecPerBeat;
        int targetBeat = Mathf.RoundToInt(Conductor.Instance.SongPositionInBeats);

        // 1. ÉXITO (En tiempo)
        if (Mathf.Abs(timeDiff) <= toleranceSeconds)
        {
            hasBufferedInput = false;
            ExecuteCommand(action, dir, timeDiff);
        }
        // 2. BUFFER (Temprano)
        else if (timeDiff < 0 && Mathf.Abs(timeDiff) <= (toleranceSeconds + bufferTimeBeforeBeat))
        {
            bufferedAction = action;
            bufferedDirection = dir;
            hasBufferedInput = true;
            nextBeatToExecute = targetBeat;
            Debug.Log($"<color=yellow>Buffer: {action}</color>");
        }
        // 3. FALLO
        else
        {
            OnInputFail?.Invoke();
        }
    }

    void ProcessBufferedInput()
    {
        if (!hasBufferedInput) return;

        float currentBeat = Conductor.Instance.SongPositionInBeats;
        if (currentBeat >= nextBeatToExecute - 0.05f)
        {
            ExecuteCommand(bufferedAction, bufferedDirection, 0f);
            hasBufferedInput = false;
            nextBeatToExecute = -1f;
        }
    }

    void ExecuteCommand(RhythmActionType action, Vector2 dir, float timeDifference)
    {
        float accuracy = 1.0f - (Mathf.Abs(timeDifference) / toleranceSeconds);
        if (accuracy < minAccuracyThreshold)
        {
            OnInputFail?.Invoke();
            return;
        }

        OnCommandInput?.Invoke(action, dir);
        OnInputSuccess?.Invoke(accuracy);
    }
}