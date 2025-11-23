using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class LatencyCalibrator : MonoBehaviour
{
    [Header("Configuración")]
    public int requiredBeats = 8;
    
    [Header("Visual References")]
    public Transform leftLine;
    public Transform rightLine;
    public float lineMaxDistance = 5f; // Distancia máxima de las líneas desde el centro

    private List<float> offsets = new List<float>();
    private bool isCalibrating = false;

    void Start()
    {
        if (Conductor.Instance != null)
        {
            Conductor.Instance.inputOffset = 0f;
            isCalibrating = true;
            Debug.Log($"Presiona cualquier tecla al ritmo de la música... (0/{requiredBeats})");
        }
        else
        {
            Debug.LogError("Error: Falta el objeto Conductor en la escena.");
        }
    }

    void Update()
    {
        // Feedback visual: líneas moviéndose hacia el centro
        if (Conductor.Instance != null && leftLine != null && rightLine != null)
        {
            // Obtener qué tan cerca estamos del beat (0 = en el beat, 1 = lejos del beat)
            float distanceToBeat = Mathf.Abs(Conductor.Instance.GetDistanceToNearestBeat());
            
            // Interpolar posición de las líneas
            // En el beat: distanceToBeat = 0, líneas en el centro (distancia = 0)
            // Lejos del beat: distanceToBeat = 1, líneas separadas (distancia = lineMaxDistance)
            float currentDistance = distanceToBeat * lineMaxDistance;
            
            leftLine.position = new Vector3(-currentDistance, 0, 0);
            rightLine.position = new Vector3(currentDistance, 0, 0);
        }

        // Solo capturar inputs durante la calibración
        if (!isCalibrating) return;

        if (Input.anyKeyDown)
        {
            RecordOffset();
        }
    }

    void RecordOffset()
    {
        if (Conductor.Instance == null) return;

        // Obtenemos la diferencia cruda (sin offset aplicado, porque lo pusimos a 0)
        float beatDiffInBeats = Conductor.Instance.GetDistanceToNearestBeat();
        float diffInSeconds = beatDiffInBeats * Conductor.Instance.secPerBeat;

        offsets.Add(diffInSeconds);
        
        Debug.Log($"Muestras: {offsets.Count}/{requiredBeats} - Última: {diffInSeconds:F3}s");

        if (offsets.Count >= requiredBeats)
        {
            FinishCalibration();
        }
    }

    void FinishCalibration()
    {
        isCalibrating = false;

        float averageOffset = offsets.Average();
        Conductor.Instance.SetInputOffset(averageOffset);

        Debug.Log($"Offset detectado: {averageOffset:F3}s");
    }
}
