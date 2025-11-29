using DG.Tweening;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.Audio;

public class PressButton : MonoBehaviour
{
    [Header("Settings")]
    public DoorManager manager;
    public float pushDistance = 0.5f;
    public float duration = 0.15f;
    public AudioClip clickSound;

    private bool pressed, animating;
    private Vector3 startPos;
    private AudioSource audioSource;

    void Start()
    {
        startPos = transform.position;
        audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("Player") || animating || pressed) return;

        // Calcular dirección del golpe
        Vector2 hitPoint = col.GetContact(0).point;
        Vector2 dir = ((Vector2)transform.position - hitPoint).normalized;

        // Snap a ejes
        Vector3 pushDir = Mathf.Abs(dir.x) > Mathf.Abs(dir.y)
            ? new Vector3(Mathf.Sign(dir.x), 0, 0)
            : new Vector3(0, Mathf.Sign(dir.y), 0);

        // Marcar como presionado
        pressed = true;
        animating = true;

        // Animación
        transform.DOMove(startPos + (pushDir * pushDistance), duration)
            .SetLoops(2, LoopType.Yoyo)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => animating = false);

        // Sonido
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound);
        }

        // Cambiar color y avisar al manager
        GetComponent<SpriteRenderer>().color = Color.gray;
        if (manager != null) manager.ButtonPressed();
    }
}