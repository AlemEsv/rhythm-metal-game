using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SideScrollingVisualizer : MonoBehaviour
{
    [Header("Referencias UI")]
    public RectTransform targetDiamond;
    public RectTransform spawnPoint;
    public GameObject noteLinePrefab; 
    public Image diamondImage;

    [Header("Configuracion de Ritmo")]
    public float beatsInAdvance = 4f;

    [Header("Colores de Feedback")]
    public Color defaultColor = Color.white;
    public Color perfectColor = Color.green;
    public Color goodColor = Color.yellow;
    public Color badColor = Color.red;
    public float flashDuration = 0.15f;

    // Estado interno
    private int lastSpawnedBeat = 0;

    void Start()
    {
        if (diamondImage != null) diamondImage.color = defaultColor;

        // Inicializar contador de beats para evitar spawn masivo al inicio
        if (Conductor.Instance != null)
        {
            lastSpawnedBeat = (int)Conductor.Instance.SongPositionInBeats;
        }
    }

    void OnEnable()
    {
        RhythmInput.OnInputSuccess += HandleHit;
        RhythmInput.OnInputFail += HandleMiss;
    }

    void OnDisable()
    {
        RhythmInput.OnInputSuccess -= HandleHit;
        RhythmInput.OnInputFail -= HandleMiss;
    }

    void Update()
    {
        SpawnNotes();
    }
    void SpawnNotes()
    {
        if (Conductor.Instance == null || !Conductor.Instance.musicSource.isPlaying) return;

        float futureBeat = Conductor.Instance.SongPositionInBeats + beatsInAdvance;
        int nextBeatIndex = Mathf.FloorToInt(futureBeat);

        if (nextBeatIndex > lastSpawnedBeat)
        {
            CreateNote(nextBeatIndex);
            lastSpawnedBeat = nextBeatIndex;
        }
    }

    void CreateNote(int beatIndex)
    {
        // Instanciar
        GameObject newNote = Instantiate(noteLinePrefab, spawnPoint.transform.parent);

        // Configuración Inicial
        newNote.transform.position = spawnPoint.transform.position;

        // Resetear escala y rotación por seguridad
        newNote.transform.localScale = Vector3.one;
        newNote.transform.rotation = Quaternion.identity;

        // Calcular duración del viaje
        float travelDuration = beatsInAdvance * Conductor.Instance.SecPerBeat;

        // movimiento
        newNote.transform.DOMove(targetDiamond.transform.position, travelDuration)
            .SetEase(Ease.Linear) // Ritmo constante
            .OnComplete(() =>
            {
                // Limpieza al llegar
                Destroy(newNote);
            });
    }

    void HandleHit()
    {
        float accuracy = GetAccuracy();
        Color colorToUse = accuracy > 0.8f ? perfectColor : goodColor;

        FlashDiamond(colorToUse, true);
    }

    void HandleMiss()
    {
        FlashDiamond(badColor, false);
    }

    void FlashDiamond(Color color, bool punchEffect)
    {
        if (diamondImage == null) return;

        // Matar animaciones anteriores
        diamondImage.DOKill();
        targetDiamond.DOKill();

        // Cambio de color instantaneo y fade out
        diamondImage.color = color;
        diamondImage.DOColor(defaultColor, flashDuration);

        // Efecto de golpe (Punch)
        targetDiamond.localScale = Vector3.one; // Reset
        if (punchEffect)
        {
            targetDiamond.DOPunchScale(Vector3.one * 0.3f, flashDuration, 10, 1);
        }
        else
        {
            targetDiamond.DOShakeAnchorPos(flashDuration, 10, 20);
        }
    }

    float GetAccuracy()
    {
        if (Conductor.Instance == null) return 0;
        float diff = Mathf.Abs(Conductor.Instance.GetDistanceToNearestBeat() * Conductor.Instance.SecPerBeat);
        RhythmInput input = FindFirstObjectByType<RhythmInput>();
        float tolerance = input != null ? input.toleranceSeconds : 0.2f;
        return 1f - (diff / tolerance);
    }
}