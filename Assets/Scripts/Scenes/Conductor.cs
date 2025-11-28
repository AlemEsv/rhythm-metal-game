using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class Conductor : MonoBehaviour
{
    public static Conductor Instance;

    [Header("Configuracion")]
    public SongData currentSongData; // Archivo de datos
    public AudioSource musicSource;  // El componente de audio

    [Header("Ajustes de sonido")]
    public bool loopSong = true;
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
    }

    void Start()
    {
        if (musicSource.clip != null && !musicSource.isPlaying)
        {
            PlaySong();
        }
    }

    public void PlaySong()
    {
        if (musicSource.clip == null) return;

        songDuration = musicSource.clip.length;
        musicSource.loop = loopSong;
        lastReportedBeat = -1;
        dspSongTime = (float)AudioSettings.dspTime;
        musicSource.volume = 1f; // Asegurar volumen máximo
        musicSource.Play();
    }

    public void PlaySongWithFade(float fadeDuration)
    {
        if (musicSource.clip == null) return;

        songDuration = musicSource.clip.length;
        musicSource.loop = loopSong;
        lastReportedBeat = -1;
        dspSongTime = (float)AudioSettings.dspTime;

        musicSource.volume = 0f; // Empezar en silencio
        musicSource.Play();

        musicSource.DOFade(1f, fadeDuration).SetEase(Ease.Linear);
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

    // utilizada por RhythmInput y otros sistemas
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