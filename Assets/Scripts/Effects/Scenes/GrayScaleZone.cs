using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(SpriteRenderer))]
public class GrayScaleZone : MonoBehaviour
{
    [Header("Manager")]
    public DoorManager linkedDoor;

    [Header("Duration")]
    public float duration = 2.0f;

    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        // Asegurar que el material sea el correcto y empiece visible
        sr.color = new Color(1, 1, 1, 1);

        if (linkedDoor != null)
        {
            linkedDoor.OnDoorOpened += RemoveCurse;
        }
    }

    void OnDestroy()
    {
        if (linkedDoor != null) linkedDoor.OnDoorOpened -= RemoveCurse;
    }

    void RemoveCurse()
    {
        Material mat = GetComponent<SpriteRenderer>().material;
        mat.DOFloat(0f, "_FadeAmount", duration)
           .SetEase(Ease.OutQuad)
           .OnComplete(() => gameObject.SetActive(false));
    }
}