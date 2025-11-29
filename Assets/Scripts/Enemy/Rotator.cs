using UnityEngine;
using DG.Tweening;

public class SimpleRotator : MonoBehaviour
{
    [Header("Rotation")]
    public float durationOneTurn = 1f;
    public bool clockwise = true;

    void Start()
    {
        float angle = clockwise ? -360f : 360f;

        // Rotación infinita y lineal
        transform.DORotate(new Vector3(0, 0, angle), durationOneTurn, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetLoops(-1)
            .SetLink(gameObject);
    }
}