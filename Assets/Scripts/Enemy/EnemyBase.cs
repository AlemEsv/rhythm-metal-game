using DG.Tweening;
using System;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    public int maxHealth = 3;
    protected int currentHealth;
    public int damage = 1;

    [Header("Rhythm Settings")]
    public int actionInterval = 1; // Actúa cada X beats
    protected int beatCounter = 0;

    protected Transform playerTransform;
    public bool IsAlive => currentHealth > 0;

    protected virtual void Start()
    {
        currentHealth = maxHealth;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) playerTransform = player.transform;

        if (Conductor.Instance != null)
        {
            Conductor.Instance.OnBeat.AddListener(OnBeatHandler);
        }
    }

    protected virtual void OnDestroy()
    {
        transform.DOKill();
        if (Conductor.Instance != null)
        {
            Conductor.Instance.OnBeat.RemoveListener(OnBeatHandler);
        }

    }

    // Lógica interna para manejar intervalos (ej. el volador ataca cada 4 beats)
    private void OnBeatHandler()
    {
        if (!IsAlive) return;

        beatCounter++;
        if (beatCounter >= actionInterval)
        {
            PerformRhythmAction();
            beatCounter = 0;
        }
    }

    protected abstract void PerformRhythmAction();

    public virtual void TakeDamage(int amount)
    {
        currentHealth -= amount;
        // animación de Hit o Flash rojo
        Debug.Log($"{gameObject.name} recibió daño. Vida: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        // notificar al RoomManager que murió un enemigo
        Destroy(gameObject);
    }
}