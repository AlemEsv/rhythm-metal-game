using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameOverFade : MonoBehaviour
{
    [Header("Settings")]
    public Image blackOverlay;
    public float fadeDuration = 2.0f;
    public float delayBeforeFade = 0.5f;

    void Start()
    {
        if (blackOverlay != null)
        {
            blackOverlay.color = new Color(0, 0, 0, 0);
            blackOverlay.gameObject.SetActive(false);
        }
    }
    public void FadeOut()
    {
        if (blackOverlay == null) return;
        blackOverlay.gameObject.SetActive(true);

        blackOverlay.DOFade(1f, fadeDuration)
                    .SetDelay(delayBeforeFade)
                    .SetEase(Ease.InOutQuad)
                    .SetUpdate(true);
    }
}