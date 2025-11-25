using UnityEngine;
using DG.Tweening; // Necesario para animaciones suaves

public class Metronomo : MonoBehaviour
{
    [SerializeField] private float beatScale = 1.3f;
    [SerializeField] private float duration = 0.2f;
    [SerializeField] private Color pressColor = Color.green;
    [SerializeField] private Color defaultColor = Color.white;

    // Referencias
    private SpriteRenderer spriteRenderer;
    private Vector3 initialScale;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            // Si es un cubo 3D y no un sprite, intenta buscar el MeshRenderer
            // Pero idealmente úsalo en un Sprite
            Debug.LogWarning("Metronomo: Falta SpriteRenderer para el cambio de color.");
        }
        else
        {
            spriteRenderer.color = defaultColor;
        }

        initialScale = transform.localScale;

        if (Conductor.Instance != null)
        {
            Conductor.Instance.OnBeat.AddListener(OnBeatPulse);
        }
    }

    void Update()
    {
        // Detectar Input
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            HandleInput();
        }
    }
    void OnBeatPulse()
    {
        transform.DOKill(true);

        transform.localScale = initialScale;
        // Argumentos: (Fuerza, Duración, Vibrato, Elasticidad)
        transform.DOPunchScale(Vector3.one * (beatScale - 1f), duration, 10, 1);
    }

    void HandleInput()
    {
        if (spriteRenderer == null) return;

        spriteRenderer.DOKill();
        spriteRenderer.color = pressColor;
        spriteRenderer.DOColor(defaultColor, 0.15f);

        // Un pequeño golpe visual extra para sentir el input
        transform.DOPunchScale(Vector3.one * 0.1f, 0.1f); 
    }

    void OnDestroy()
    {
        transform.DOKill();
        if (spriteRenderer != null)
            spriteRenderer.DOKill();

        if (Conductor.Instance != null)
        {
            Conductor.Instance.OnBeat.RemoveListener(OnBeatPulse);
        }
    }
}