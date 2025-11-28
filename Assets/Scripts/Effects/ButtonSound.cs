using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]
public class ButtonSounds : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [Header("Audio")]
    [SerializeField] private AudioClip sonidoHover;
    [SerializeField] private AudioClip sonidoClick;

    private AudioSource miAudioSource;

    private void Awake()
    {
        miAudioSource = GetComponent<AudioSource>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (sonidoHover != null)
        {
            miAudioSource.PlayOneShot(sonidoHover);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (sonidoClick != null)
        {
            miAudioSource.PlayOneShot(sonidoClick);
        }
    }
}