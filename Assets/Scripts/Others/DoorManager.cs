using System;
using UnityEngine;
using UnityEngine.Audio;

public class DoorManager : MonoBehaviour
{
    [Header("Settings")]
    public GameObject door;
    public int buttonsNeeded = 4;

    [Header("Audio")]
    public AudioClip successSound;

    // evento
    public event Action<int, int> OnProgressChanged;

    private AudioSource audioSource;
    private int count = 0;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        UpdateProgressUI();
    }
    public void ButtonPressed()
    {
        count++;

        UpdateProgressUI();
        // Verificamos si alcanzamos la meta
        if (count >= buttonsNeeded)
        {
            OpenDoor();
        }
    }
    private void OpenDoor()
    {
        if (successSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(successSound);
        }

        // Abrir la puerta
        if (door != null)
        {
            door.SetActive(false); // Ocultar puerta
        }
    }

    private void UpdateProgressUI()
    {
        OnProgressChanged?.Invoke(count, buttonsNeeded);
    }
}