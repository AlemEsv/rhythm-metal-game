using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameOverFade : MonoBehaviour
{
    [Header("Configuracion")]
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

    void OnEnable()
    {
        PlayerCombat.OnPlayerDeath += TriggerFade;
    }

    void OnDisable()
    {
        PlayerCombat.OnPlayerDeath -= TriggerFade;
    }

    void TriggerFade()
    {
        if (blackOverlay == null) return;
        blackOverlay.gameObject.SetActive(true);

        blackOverlay.DOFade(1f, fadeDuration)
            .SetDelay(delayBeforeFade)
            .SetEase(Ease.InOutQuad)
            .SetUpdate(true)
            .OnComplete(() =>
            {
            });
    }
}