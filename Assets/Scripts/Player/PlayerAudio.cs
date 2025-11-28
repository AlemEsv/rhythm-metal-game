using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerAudio : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;

    [Header("Clips")]
    public AudioClip runClip;
    public AudioClip attackClip;
    public AudioClip jumpClip;
    public AudioClip parryClip;
    public AudioClip hitClip;
    public AudioClip dieClip;

    void Awake()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
    }

    public void PlayRun() => PlaySound(runClip, 0.4f);
    public void PlayAttack() => PlaySound(attackClip);
    public void PlayJump() => PlaySound(jumpClip);
    public void PlayParry() => PlaySound(parryClip);
    public void PlayHit() => PlaySound(hitClip);
    public void PlayDie() => PlaySound(dieClip);

    private void PlaySound(AudioClip clip, float volume = 1f)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }
}