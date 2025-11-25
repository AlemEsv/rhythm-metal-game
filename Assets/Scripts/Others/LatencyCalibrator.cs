using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class LatencyCalibrator : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI resultText;

    [Header("Configuración")]
    public int requiredTaps = 8;

    private string nextSceneName = "MainMenuScene";
    private List<float> samples = new List<float>();
    private bool isCalibrating = false;
    private bool calibrationFinished = false;

    void Start()
    {
        if (resultText != null) resultText.text = "";
    }

    void Update()
    {
        if (!calibrationFinished)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                if (!isCalibrating) StartCalibration();
                else RecordTap();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                GoToNextScene();
            }
        }
    }

    void StartCalibration()
    {
        isCalibrating = true;
        samples.Clear();

        // Iniciar música si no está sonando
        if (Conductor.Instance != null && !Conductor.Instance.musicSource.isPlaying)
            Conductor.Instance.musicSource.Play();
    }

    void RecordTap()
    {
        if (Conductor.Instance == null) return;

        float beatDist = Conductor.Instance.GetDistanceToNearestBeat();
        float errorInSeconds = beatDist * Conductor.Instance.SecPerBeat;

        samples.Add(errorInSeconds);

        if (samples.Count >= requiredTaps)
        {
            FinishCalibration();
        }
    }

    void FinishCalibration()
    {
        isCalibrating = false;
        calibrationFinished = true;

        float averageOffset = samples.Average();

        // Guardar datos
        if (Conductor.Instance != null)
            Conductor.Instance.SetInputOffset(averageOffset);

        // Mostrar texto final
        if (resultText != null)
        {
            resultText.text = $"{averageOffset * 1000:F0} ms\n\n<size=60%>Presiona Espacio</size>";
        }

        PlayerPrefs.SetInt("FirstTimeSetupDone", 1);
        PlayerPrefs.Save();
    }

    void GoToNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}