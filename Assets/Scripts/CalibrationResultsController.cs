using UnityEngine;
using TMPro;

public class CalibrationResultsController : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI resultText; // Arrastra aquí el texto que dirá "Resultado: X"

    void Start()
    {
        // Recuperar el valor guardado (si no hay, devuelve 0)
        float latencia = PlayerPrefs.GetFloat("AudioLatency", 0f);
        
        // Convertir a milisegundos para que sea legible (ej: 0.05s -> 50ms)
        float latenciaMs = latencia * 1000f;

        // Mostrar en el texto. "F0" significa sin decimales (número entero)
        if (resultText != null)
        {
            resultText.text = $"RESULTADO DE LATENCIA:\n{latenciaMs.ToString("F0")} ms";
        }
    }

    void Update()
    {
        // Al presionar Espacio, ir al Menú Principal
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (SceneLoader.Instance != null)
                SceneLoader.Instance.LoadScene("MainMenuScene");
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuScene");
        }
    }
}