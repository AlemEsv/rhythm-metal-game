using DG.Tweening;
using System;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    public int maxHealth = 3;
    protected int currentHealth;

    [Header("Rhythm Settings")]
    public int actionInterval = 1;
    protected int beatCounter = 0;

    public bool IsAlive => currentHealth > 0;
    protected virtual void Start()
    {
        currentHealth = maxHealth;

        if (Conductor.Instance != null)
            Conductor.Instance.OnBeat.AddListener(OnBeatHandler);
    }

    protected virtual void OnDestroy()
    {
        transform.DOKill();
        if (Conductor.Instance != null)
            Conductor.Instance.OnBeat.RemoveListener(OnBeatHandler);
    }

    // Logica interna para manejar intervalos
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
        // animacion de Hit o Flash rojo
        transform.DOShakeScale(0.15f, 0.3f);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        // notificar al RoomManager
        Destroy(gameObject);
    }
}