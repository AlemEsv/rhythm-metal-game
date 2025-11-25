using UnityEngine;
using UnityEngine.Events;

public class Conductor : MonoBehaviour
{
    public static Conductor Instance;

    [Header("Configuración Principal")]
    public SongData currentSongData; // Archivo de datos
    public AudioSource musicSource;  // El componente de audio

    [Header("Ajustes de Reproducción")]
    public bool loopSong = true;     // ¿Se repite la canción?
    public float inputOffset = 0f;   // Calibración de latencia

    [Space(10)]
    public UnityEvent OnBeat;        // Evento del Beat

    public float SongPosition { get; private set; }
    public float SongPositionInBeats { get; private set; }
    public float SecPerBeat { get; private set; }

    private float bpm;
    private float firstBeatOffset;
    private float dspSongTime;
    private int lastReportedBeat = 0;
    private float songDuration;

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;

        // Cargar latencia guardada
        inputOffset = PlayerPrefs.GetFloat("InputOffset", 0f);

        // cargar datos
        if (currentSongData != null)
        {
            musicSource.clip = currentSongData.audioClip;
            bpm = currentSongData.bpm;
            firstBeatOffset = currentSongData.firstBeatOffset;

            // Calcular duración del beat
            SecPerBeat = 60f / bpm;
            musicSource.playOnAwake = false;
        }
        else
        {
            Debug.LogError("No hay archivos de música.");
        }
    }

    void Start()
    {
        if (musicSource.clip == null) return;

        songDuration = musicSource.clip.length;
        musicSource.loop = loopSong;

        // Iniciar conteo de tiempo
        lastReportedBeat = -1;
        dspSongTime = (float)AudioSettings.dspTime;
        musicSource.Play();
    }

    void Update()
    {
        if (!musicSource.isPlaying) return;

        // Calcular posición en segundos
        SongPosition = (float)(AudioSettings.dspTime - dspSongTime) - firstBeatOffset;

        // Manejar Loop
        if (loopSong && SongPosition >= songDuration)
        {
            dspSongTime += songDuration;
            SongPosition -= songDuration;
            lastReportedBeat = -1;
        }

        // Calcular posición en Beats
        SongPositionInBeats = SongPosition / SecPerBeat;

        // Disparar Evento OnBeat
        if (SongPositionInBeats > lastReportedBeat + 1)
        {
            lastReportedBeat++;
            OnBeat.Invoke();
        }
    }

    // Función auxiliar utilizada por RhythmInput y otros sistemas
    public float GetDistanceToNearestBeat()
    {
        float adjustedSongPosition = SongPosition - inputOffset;
        float adjustedPositionInBeats = adjustedSongPosition / SecPerBeat;

        float nearestBeat = Mathf.Round(adjustedPositionInBeats);
        return adjustedPositionInBeats - nearestBeat;
    }

    public void SetInputOffset(float newOffset)
    {
        inputOffset = newOffset;
        PlayerPrefs.SetFloat("InputOffset", inputOffset);
        PlayerPrefs.Save();
    }
}