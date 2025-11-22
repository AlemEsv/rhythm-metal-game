using UnityEngine;
using UnityEngine.Events;

public class Conductor : MonoBehaviour
{
    // Esto permite que otros scripts encuentren al Conductor fácilmente
    public static Conductor Instance;

    [Header("Configuración")]
    public AudioSource musicSource; // Aquí arrastraremos la música
    public float bpm = 120f;        // Beats por minuto de tu canción
    public float firstBeatOffset = 0f; // Ajuste si la canción no empieza de inmediato

    // Variables que otros scripts leerán
    public float SongPosition { get; private set; }
    public float SongPositionInBeats { get; private set; }
    public float SecPerBeat { get; private set; }

    // Evento: Avisa cada vez que ocurre un beat
    public UnityEvent OnBeat; 

    private float dspSongTime; // Tiempo preciso del motor de audio

    void Awake()
    {
        // Configuración del "Singleton" (Solo puede haber un Conductor)
        if (Instance != null && Instance != this) 
        {
            Destroy(this);
        }
        else 
        {
            Instance = this;
        }

        // Calcular cuánto dura un beat en segundos
        SecPerBeat = 60f / bpm;
    }

    void Start()
    {
        // Guardamos el momento exacto en que inicia la música
        dspSongTime = (float)AudioSettings.dspTime;
        musicSource.Play();
    }

    void Update()
    {
        // Calcular posición actual de la canción
        SongPosition = (float)(AudioSettings.dspTime - dspSongTime) - firstBeatOffset;
        
        // Calcular en qué beat estamos (ej: 1.5, 2.0, 2.5)
        SongPositionInBeats = SongPosition / SecPerBeat;
    }

    // Esta función ayuda a saber si estamos cerca de un beat
    public float GetDistanceToNearestBeat()
    {
        float nearestBeat = Mathf.Round(SongPositionInBeats);
        return SongPositionInBeats - nearestBeat;
    }
}