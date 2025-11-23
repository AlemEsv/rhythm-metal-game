using UnityEngine;
using System.Collections;

public class PixelMetronome : MonoBehaviour
{
[Header("Configuración")]
    public float bpm = 120f;
    public float minScale = 1.0f;
    public float maxScale = 1.3f;

    void Update()
    {
        double time = AudioSettings.dspTime;

        double phase = time * (bpm / 60.0) * 2.0 * System.Math.PI;

        // Mathf.Sin va de -1 a 1. Lo ajustamos para que vaya de 0 a 1
        float wave = (Mathf.Sin((float)phase) + 1f) / 2f;

        // Interpolamos suavemente entre el tamaño mínimo y máximo
        float currentScale = Mathf.Lerp(minScale, maxScale, wave);

        transform.localScale = new Vector3(currentScale, currentScale, 1f);
    }
}