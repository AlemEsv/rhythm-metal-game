using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class LatencyCalibrator : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource metronomeAudio;
    public float bpm = 120f;

    private double beatInterval;
    private double nextBeatTime;
    private List<float> offsets = new List<float>();

    void Start()
    {
        beatInterval = 60.0 / bpm;
        nextBeatTime = AudioSettings.dspTime + beatInterval;

        if (metronomeAudio != null) metronomeAudio.Play();
    }

    void Update()
    {
        // Mantener el ritmo teórico
        if (AudioSettings.dspTime >= nextBeatTime)
        {
            nextBeatTime += beatInterval;
        }

        // REGISTRAR GOLPES (con W o Flecha Arriba)
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            CalculateOffset();
        }

        // PASAR A RESULTADOS
        if (Input.GetKeyDown(KeyCode.Space))
        {
            FinishCalibration();
        }
    }

    void CalculateOffset()
    {
        double time = AudioSettings.dspTime;
        double difference = time - (nextBeatTime - beatInterval);

        if (difference > beatInterval / 2)
        {
            difference -= beatInterval;
        }

        float offset = (float)difference;
        offsets.Add(offset);
        
        // Feedback visual rápido en consola para saber que funciona
        Debug.Log($"Golpe registrado. Offset: {offset * 1000} ms");
    }

    void FinishCalibration()
    {
        // Solo guardamos si el jugador hizo al menos un intento
        if (offsets.Count > 0)
        {
            float avgOffset = 0f;
            foreach (float o in offsets) avgOffset += o;
            avgOffset /= offsets.Count;

            // Guardar en PlayerPrefs
            PlayerPrefs.SetFloat("AudioLatency", avgOffset);
            PlayerPrefs.Save();
            Debug.Log("Latencia Final Guardada: " + avgOffset);
        }

        // Cambiar a la escena de resultados
        if (SceneLoader.Instance != null)
            SceneLoader.Instance.LoadScene("MainMenuScene");
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuScene");
    }
}