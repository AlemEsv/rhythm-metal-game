using UnityEngine;

public class MusicChangeZone : MonoBehaviour
{
    [Header("Settings")]
    public AudioClip zoneMusic;
    public float zoneBpm = 120f;

    [Header("Transition")]
    public float fadeDuration = 1.5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Solo el jugador activa el cambio
        if (other.CompareTag("Player"))
        {
            if (Conductor.Instance != null)
            {
                Conductor.Instance.SwitchMusic(zoneMusic, zoneBpm, fadeDuration);
            }
        }
    }
}