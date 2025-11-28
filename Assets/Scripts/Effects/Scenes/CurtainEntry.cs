using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CurtainEntry : MonoBehaviour
{
    [Header("Reference")]
    public Image curtainImage;
    public Canvas transitionCanvas;

    [Header("Settings")]
    public float animationDuration = 3.0f;
    public float startDelay = 0.5f;
    public Ease easeType = Ease.InOutCubic;

    private Material curtainMat;

    void Start()
    {
        if (curtainImage != null)
        {
            curtainImage.gameObject.SetActive(true);

            // Instancia del material para no modificar el archivo original
            curtainMat = new Material(curtainImage.material);
            curtainImage.material = curtainMat;

            curtainMat.SetFloat("_OpenAmount", 0f);
        }

        // Iniciar Música
        if (Conductor.Instance != null)
        {
            Conductor.Instance.PlaySongWithFade(animationDuration);
        }

        // Iniciar Animación
        AnimateCurtain();
    }

    void AnimateCurtain()
    {
        float progress = 0f;

        DOTween.To(() => progress, x => progress = x, 1.2f, animationDuration)
            .SetDelay(startDelay)
            .SetEase(easeType)
            .OnUpdate(() =>
            {
                if (curtainMat != null)
                    curtainMat.SetFloat("_OpenAmount", progress);
            })
            .OnComplete(() =>
            {
                if (curtainImage != null)
                    curtainImage.gameObject.SetActive(false);
            });
    }
}