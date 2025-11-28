using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SideScrollingVisualizer : MonoBehaviour
{
    [Header("UI")]
    public RectTransform target;
    public RectTransform spawnPoint;
    public GameObject noteLine; 
    public Image Image;

    [Header("Ritmo")]
    public float beatsInAdvance = 4f;

    [Header("Colores")]
    public Color defaultColor = Color.white;
    public Color perfectColor = Color.green;
    public Color goodColor = Color.yellow;
    public Color badColor = Color.red;
    public float flashDuration = 0.15f;


    private int lastSpawnedBeat = 0;

    void Start()
    {
        if (Image != null) Image.color = defaultColor;

        if (Conductor.Instance != null)
        {
            lastSpawnedBeat = (int)Conductor.Instance.SongPositionInBeats;
        }
    }

    void OnEnable()
    {
        // MOVIMIENTO
        RhythmInput.OnInputSuccess += HandleHit;
        RhythmInput.OnInputFail += HandleMiss;

        // PARRY
        PlayerCombat.OnParrySuccess += HandleHit;
        PlayerCombat.OnParryFail += HandleMiss;

        // AGARRE
        PlayerController.OnClingSuccess += HandleHit;
        PlayerController.OnClingFail += HandleMiss;
    }

    void OnDisable()
    {
        RhythmInput.OnInputSuccess -= HandleHit;
        RhythmInput.OnInputFail -= HandleMiss;

        PlayerCombat.OnParrySuccess -= HandleHit;
        PlayerCombat.OnParryFail -= HandleMiss;

        PlayerController.OnClingSuccess -= HandleHit;
        PlayerController.OnClingFail -= HandleMiss;
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
        GameObject newNote = Instantiate(noteLine, spawnPoint.transform.parent);

        // Configuración Inicial
        newNote.transform.position = spawnPoint.transform.position;

        // Resetear escala y rotación por seguridad
        newNote.transform.localScale = Vector3.one;
        newNote.transform.rotation = Quaternion.identity;

        // Calcular duración
        float travelDuration = beatsInAdvance * Conductor.Instance.SecPerBeat;

        // movimiento
        newNote.transform.DOMove(target.transform.position, travelDuration)
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
        Color colorToUse = accuracy > 0.6f ? perfectColor : goodColor;

        FlashDiamond(colorToUse, true);
    }

    void HandleHit(float accuracy)
    {
        Color colorToUse = accuracy > 0.6f ? perfectColor : goodColor;
        FlashDiamond(colorToUse, true);
    }

    void HandleMiss()
    {
        FlashDiamond(badColor, false);
    }

    void FlashDiamond(Color color, bool punchEffect)
    {
        if (Image == null) return;

        // Matar animaciones anteriores
        Image.DOKill();
        target.DOKill();

        // Cambio de color instantaneo y fade out
        Image.color = color;
        Image.DOColor(defaultColor, flashDuration);

        // Efecto de golpe (Punch)
        target.localScale = Vector3.one; // Reset
        if (punchEffect)
        {
            target.DOPunchScale(Vector3.one * 0.3f, flashDuration, 10, 1);
        }
        else
        {
            target.DOShakeAnchorPos(flashDuration, 10, 20);
        }
    }

    float GetAccuracy()
    {
        if (Conductor.Instance == null) return 0;
        float diff = Mathf.Abs(Conductor.Instance.GetDistanceToNearestBeat() * Conductor.Instance.SecPerBeat);
        float tolerance = 0.2f;
        return 1f - (diff / tolerance);
    }
}