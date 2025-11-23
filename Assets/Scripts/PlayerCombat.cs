using UnityEngine;
using System;

public class PlayerCombat : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    public int maxHealth = 3;
    public int currentHealth = 3;
    public int attackDamage = 1;
    
    [Header("Combat Settings")]
    public float invulnerabilityDuration = 0.5f;
    public float bumpDistance = 0.5f;
    public float bumpDuration = 0.1f;
    
    // Estado
    private bool isInvulnerable = false;
    private float invulnerabilityTimer = 0f;
    
    // Eventos
    public static event Action<int> OnHealthChanged;
    public static event Action OnPlayerDeath;
    public static event Action<Vector3> OnPlayerAttack;

    public bool IsAlive => currentHealth > 0;

    void Update()
    {
        // Contador de invulnerabilidad
        if (isInvulnerable)
        {
            invulnerabilityTimer -= Time.deltaTime;
            if (invulnerabilityTimer <= 0f)
            {
                isInvulnerable = false;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvulnerable || !IsAlive) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        
        Debug.Log($"<color=red>Player recibió {damage} de daño. Vida restante: {currentHealth}/{maxHealth}</color>");
        
        OnHealthChanged?.Invoke(currentHealth);
        
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Activar invulnerabilidad temporal (iFrames)
            isInvulnerable = true;
            invulnerabilityTimer = invulnerabilityDuration;
            
            // TODO: Aquí añadir efecto visual de daño (flash blanco, shake, etc.)
        }
    }

    public void Attack(Vector3 targetPosition, IDamageable target)
    {
        if (!IsAlive) return;

        Debug.Log($"<color=yellow>Player ataca!</color>");
        
        // Aplicar daño al objetivo
        target?.TakeDamage(attackDamage);
        
        // Evento para efectos visuales
        OnPlayerAttack?.Invoke(targetPosition);
        
        // TODO: Bump attack animation (mover hacia el enemigo y regresar)
        // transform.DOMove hacia targetPosition 50% y luego regresar
    }

    void Die()
    {
        Debug.Log("<color=red>¡GAME OVER! El jugador ha muerto.</color>");
        OnPlayerDeath?.Invoke();
        
        // TODO: Mostrar pantalla de Game Over, reiniciar nivel, etc.
    }

    public void Heal(int amount)
    {
        if (!IsAlive) return;
        
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        
        OnHealthChanged?.Invoke(currentHealth);
        Debug.Log($"<color=green>Player curado. Vida: {currentHealth}/{maxHealth}</color>");
    }
}
