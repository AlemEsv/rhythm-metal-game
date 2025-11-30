using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BossDarknessManager : MonoBehaviour
{
    public static BossDarknessManager Instance;

    [Header("References")]
    public Image darknessOverlay;
    public Transform player;
    public Camera mainCamera;

    [Header("Settings")]
    [Range(0f, 1f)] public float darknessRadius = 0.25f;
    public float transitionDuration = 1.0f;

    private Material darkMat;
    private bool isActive = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        if (darknessOverlay != null)
        {
            // Crear instancia del material
            darkMat = new Material(darknessOverlay.material);
            darknessOverlay.material = darkMat;
            darknessOverlay.gameObject.SetActive(false);
        }

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (mainCamera == null) mainCamera = Camera.main;
    }

    void Update()
    {
        if (!isActive || darkMat == null || player == null) return;

        // Convertir posición del Jugador
        Vector3 screenPos = mainCamera.WorldToViewportPoint(player.position);

        // Actualizar el shader
        darkMat.SetFloat("_CenterX", screenPos.x);
        darkMat.SetFloat("_CenterY", screenPos.y);

        // Mantener la proporción
        float ratio = (float)Screen.width / Screen.height;
        darkMat.SetFloat("_AspectRatio", ratio);
    }

    public void ActivateDarkness()
    {
        if (darknessOverlay == null) return;

        isActive = true;
        darknessOverlay.gameObject.SetActive(true);

        // Empezamos con el radio gigante
        darkMat.SetFloat("_Radius", 2.0f);

        DOTween.To(() => darkMat.GetFloat("_Radius"), x => darkMat.SetFloat("_Radius", x), darknessRadius, transitionDuration)
            .SetEase(Ease.OutQuad);
    }

    public void DeactivateDarkness()
    {
        if (darkMat == null) return;

        DOTween.To(() => darkMat.GetFloat("_Radius"), x => darkMat.SetFloat("_Radius", x), 2.0f, transitionDuration)
            .OnComplete(() => {
                isActive = false;
                darknessOverlay.gameObject.SetActive(false);
            });
    }
}