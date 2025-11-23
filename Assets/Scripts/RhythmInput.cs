using UnityEngine;
using System;

public class RhythmInput : MonoBehaviour
{
    [Header("Tolerancia")]
    public float toleranceSeconds = 0.15f; // Ventana de éxito

    // Eventos
    public static event Action<Vector2> OnMovementInput; 

    void Update()
    {
        Vector2 input = Vector2.zero;

        // Detección de teclas
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) input = Vector2.up;
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) input = Vector2.down;
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) input = Vector2.left;
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) input = Vector2.right;

        if (input != Vector2.zero)
        {
            ValidateInput(input);
        }
    }

    void ValidateInput(Vector2 direction)
    {
        if (Conductor.Instance == null) return;

        // Obtenemos la diferencia en beats y la pasamos a segundos
        float beatDiff = Conductor.Instance.GetDistanceToNearestBeat();
        float timeDiff = beatDiff * Conductor.Instance.secPerBeat;

        // Chequeo de ventana de tiempo
        if (Mathf.Abs(timeDiff) <= toleranceSeconds)
        {
            Debug.Log("<color=green>¡Input Correcto!</color>");
            OnMovementInput?.Invoke(direction);
        }
        else
        {
            Debug.Log($"<color=red>Fallaste por {timeDiff:F2}s</color>");
        }
    }
}