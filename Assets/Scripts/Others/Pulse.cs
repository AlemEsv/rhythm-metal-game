using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BackgroundBeatPulse : MonoBehaviour
{
    [Header("Ritmo")]
    [SerializeField] private int beatsToWait = 4; // Cada cuántos beats ocurre el pulso

    [Header("Color")]
    [SerializeField] private Color pulseColor = Color.red;
    [SerializeField] private float delay = 0f;
    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private Ease pulseEase = Ease.OutQuad;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private int beatCounter = 0;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    void Start()
    {
        if (Conductor.Instance != null)
        {
            Conductor.Instance.OnBeat.AddListener(OnBeatDetected);
        }
    }

    void OnDestroy()
    {
        if (Conductor.Instance != null)
        {
            Conductor.Instance.OnBeat.RemoveListener(OnBeatDetected);
        }
        transform.DOKill();
    }

    void OnBeatDetected()
    {
        beatCounter++;
        if (beatCounter >= beatsToWait)
        {
            PulseEffect();
            beatCounter = 0; // Reiniciar contador
        }
    }

    void PulseEffect()
    {
        transform.DOKill();

        Sequence pulseSequence = DOTween.Sequence();

        // Esperar el tiempo de delay
        if (delay > 0) pulseSequence.AppendInterval(delay);

        // Cambiar el color de golpe
        pulseSequence.AppendCallback(() => spriteRenderer.color = pulseColor);

        // Volver suavemente al color original
        pulseSequence.Append(spriteRenderer.DOColor(originalColor, fadeDuration).SetEase(pulseEase));

        pulseSequence.SetLink(gameObject);
    }
}