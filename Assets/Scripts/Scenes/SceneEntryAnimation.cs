using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SceneEntryAnimation : MonoBehaviour
{
    [Header("Referencias UI")]
    public Image transitionOverlay;
    public Canvas transitionCanvas;

    [Header("Configuración")]
    public float animationDuration = 2.5f;
    public float startDelay = 0.2f;
    [Range(0.5f, 2f)] public float maxRadius = 1.2f;

    private Material transitionMaterial;

    void Start()
    {
        if (transitionOverlay != null)
        {
            // Creamos una instancia del material para no modificar el original
            transitionMaterial = new Material(transitionOverlay.material);
            transitionOverlay.material = transitionMaterial;

            // Empezamos con el círculo cerrado (Radio 0)
            transitionMaterial.SetFloat("_Radius", 0f);

            // Ajustamos el Aspect Ratio para que el círculo sea perfecto
            float ratio = (float)Screen.width / Screen.height;
            transitionMaterial.SetFloat("_AspectRatio", ratio);

            transitionOverlay.gameObject.SetActive(true);
        }

        // Iniciar Música con Fade
        if (Conductor.Instance != null)
        {
            Conductor.Instance.PlaySongWithFade(animationDuration);
        }

        AnimateEntrance();
    }

    void AnimateEntrance()
    {
        float currentRadius = 0f;

        DOTween.To(() => currentRadius, x => currentRadius = x, maxRadius, animationDuration)
            .SetDelay(startDelay)
            .SetEase(Ease.OutCubic) // Efecto suave al final
            .OnUpdate(() =>
            {
                if (transitionMaterial != null)
                    transitionMaterial.SetFloat("_Radius", currentRadius);
            })
            .OnComplete(() =>
            {
                if (transitionOverlay != null)
                    transitionOverlay.gameObject.SetActive(false);
            });
    }
}