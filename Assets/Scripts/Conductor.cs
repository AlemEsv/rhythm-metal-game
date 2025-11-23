using UnityEngine;
using UnityEngine.Events;

public class Conductor : MonoBehaviour
{
    public static Conductor Instance;

    [Header("Configuración Básica")]
    public AudioSource musicSource; 
    public float bpm = 120f;
    public float firstBeatOffset = 0f; // Por si la canción tiene silencio al inicio
    public float inputOffset = 0f;     // Latencia de input (se ajusta en calibración)
    [Tooltip("Marca esto si quieres que el sistema maneje el loop infinitamente")]
    public bool loopSong = true;

    [Header("Debug (Solo lectura)")]
    public float songPosition;       // Posición actual en segundos (dentro del loop)
    public float songPositionInBeats; // Posición actual en beats
    public float secPerBeat;         // Cuánto dura un beat

    public UnityEvent OnBeat;        // Evento para que los enemigos/UI reaccionen

    // Variables internas para el cálculo preciso
    private float dspSongTime;       // El momento exacto en que empezó (o reinició) la canción
    private int lastReportedBeat = 0;
    private float songDuration;      // Duración total del clip de audio

    void Awake()
    {
        // Singleton Pattern
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;

        // 1. Calculamos la duración de una negra (crotchet)
        secPerBeat = 60f / bpm;

        // Cargar offset guardado
        inputOffset = PlayerPrefs.GetFloat("InputOffset", 0f);
    }

    void Start()
    {
        // Configuración inicial del audio
        if(musicSource.clip != null)
        {
            songDuration = musicSource.clip.length;
        }
        else
        {
            Debug.LogError("¡Falta el AudioClip en el AudioSource!");
            return;
        }

        musicSource.loop = loopSong; // Le decimos a Unity que loopee el audio físicamente
        
        // Guardamos el tiempo de inicio DSP
        dspSongTime = (float)AudioSettings.dspTime;
        
        musicSource.Play();
    }

    void Update()
    {
        if (!musicSource.isPlaying) return;
        
        // Calculamos dónde estamos basándonos en el reloj absoluto
        songPosition = (float)(AudioSettings.dspTime - dspSongTime) - firstBeatOffset;

        // Si hemos configurado loop y la posición supera la duración de la canción...
        if (loopSong && songPosition >= songDuration)
        {
            // Ajustamos el "tiempo de inicio" sumándole la duración de la canción.
            // Esto engaña a la matemática para que crea que acabamos de empezar de nuevo,
            // manteniendo la precisión perfecta del dspTime.
            dspSongTime += songDuration;
            
            // Recalculamos la posición para este frame
            songPosition -= songDuration;
            
            // Reseteamos el contador de beats para que empiece desde 0 otra vez
            lastReportedBeat = 0;
        }

        // --- DISPARO DE EVENTOS ---
        
        songPositionInBeats = songPosition / secPerBeat;

        // Si hemos pasado al siguiente número entero de beat...
        if (songPositionInBeats > lastReportedBeat + 1)
        {
            lastReportedBeat++;
            OnBeat.Invoke(); // ¡PUM! Disparar evento
            // Debug.Log($"Beat: {lastReportedBeat}"); 
        }
    }

    // Función auxiliar para el Input (Devuelve distancia al beat más cercano en beats)
    public float GetDistanceToNearestBeat()
    {
        // Ajustamos la posición con el offset de input (latencia)
        float adjustedSongPosition = songPosition - inputOffset;
        float adjustedPositionInBeats = adjustedSongPosition / secPerBeat;

        float nearestBeat = Mathf.Round(adjustedPositionInBeats);
        return adjustedPositionInBeats - nearestBeat;
    }

    public void SetInputOffset(float newOffset)
    {
        inputOffset = newOffset;
        PlayerPrefs.SetFloat("InputOffset", inputOffset);
        PlayerPrefs.Save();
        Debug.Log($"Nuevo Input Offset guardado: {inputOffset}s");
    }
}