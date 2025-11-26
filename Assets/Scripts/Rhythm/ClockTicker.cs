using UnityEngine;
using DG.Tweening;

public class ClockTicker : MonoBehaviour
{
    [Header("Referencia")]
    public RectTransform clockHand;

    [Header("Configuración")]
    public float rotationAngle = -90f;
    public float tickDuration = 0.2f;
    public Ease tickEase = Ease.OutElastic;

    private void Start()
    {
        if (Conductor.Instance != null)
        {
            Conductor.Instance.OnBeat.AddListener(OnBeatTick);
        }
    }

    private void OnDestroy()
    {
        if (Conductor.Instance != null)
        {
            Conductor.Instance.OnBeat.RemoveListener(OnBeatTick);
        }
    }

    void OnBeatTick()
    {
        if (clockHand == null) return;
        clockHand.DOKill(true);

        // Rotar
        clockHand.DORotate(new Vector3(0, 0, rotationAngle), tickDuration, RotateMode.LocalAxisAdd)
            .SetEase(tickEase);
    }
}