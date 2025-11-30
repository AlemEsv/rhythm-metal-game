using UnityEngine;
using DG.Tweening;

public class BossZoneTrigger : MonoBehaviour
{
    [Header("Settings")]
    public GameObject backgroundObject;
    public float fadeDuration = 2.0f;

    private bool hasTriggered = false;

    void Start()
    {
        if (backgroundObject == null && Camera.main != null)
        {
            Transform bgTransform = Camera.main.transform.Find("background");

            if (bgTransform != null)
            {
                backgroundObject = bgTransform.gameObject;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player"))
        {
            hasTriggered = true;

            // Activar efecto de oscuridad
            if (BossDarknessManager.Instance != null)
            {
                BossDarknessManager.Instance.ActivateDarkness();
            }

            // Fade Out y Eliminar el Fondo de la Cámara
            HandleBackgroundRemoval();

            Debug.Log("Entrada al Boss");
        }
    }

    void HandleBackgroundRemoval()
    {
        if (backgroundObject == null) return;

        SpriteRenderer sr = backgroundObject.GetComponent<SpriteRenderer>();

        if (sr != null)
        {
            // Hacer fade del alpha a 0
            sr.DOFade(0f, fadeDuration)
              .SetEase(Ease.Linear)
              .OnComplete(() => {
                  // Al terminar la animación, destruir el objeto para liberar memoria
                  Destroy(backgroundObject);
              });
        }
        else
        {
            Destroy(backgroundObject);
        }
    }
}