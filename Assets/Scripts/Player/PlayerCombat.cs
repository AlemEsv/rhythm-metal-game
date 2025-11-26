using UnityEngine;
using System;
using DG.Tweening;

public class PlayerCombat : MonoBehaviour, IDamageable
{
    [Header("Stats")]
    public int maxHealth = 3;
    public int currentHealth = 3;
    public int attackDamage = 1;
    
    [Header("Combat Settings")]
    public float invulnerabilityDuration = 0.5f;
    
    [Header("Parry System")]
    public float parryWindow = 0.2f;
    public float parryCooldown = 0.5f;
    public LayerMask projectileLayer;
    public float parryRadius = 1.5f;
    
    // Estado
    private bool isInvulnerable = false;
    private float invulnerabilityTimer = 0f;
    private bool canParry = true;
    private float lastParryTime;
    
    [Header("Referencias Visuales")]
    public Animator animator;

    // Eventos
    public static event Action<int> OnHealthChanged;
    public static event Action OnPlayerDeath;
    public static event Action<Vector3> OnPlayerAttack;
    public static event Action OnParrySuccess;
    public static event Action OnParryFail;

    public bool IsAlive => currentHealth > 0;

    void Update()
    {
        if (isInvulnerable)
        {
            invulnerabilityTimer -= Time.deltaTime;
            if (invulnerabilityTimer <= 0f)
            {
                isInvulnerable = false;
            }
        }

        // Input de Parry
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AttemptParry();
        }
    }

    public void Attack()
    {
        if (!IsAlive) return;

        Debug.Log("Player lanza un ataque al aire!");
        
        // Disparar animación si existe
        if (animator != null) animator.SetTrigger("Attack");
        OnPlayerAttack?.Invoke(transform.position + transform.up);
    }

    public void Attack(Vector3 targetPosition, IDamageable target)
    {
        if (!IsAlive) return;

        Attack();
        
        // Aplicar daño específico
        if (target != null)
        {
            target.TakeDamage(attackDamage);
            Debug.Log($"Impacto confirmado en {targetPosition}");
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvulnerable || !IsAlive) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        
        Debug.Log($"<color=red>Player recibió {damage} de daño. Vida: {currentHealth}</color>");
        
        OnHealthChanged?.Invoke(currentHealth);
        
        if (animator != null) animator.SetTrigger("Hit");

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            isInvulnerable = true;
            invulnerabilityTimer = invulnerabilityDuration;
        }
    }

    void AttemptParry()
    {
        if (!canParry)
        {
            Debug.Log("⏳ Parry en cooldown");
            return;
        }

        if (!CheckParryTiming())
        {
            Debug.Log("❌ Parry fuera de ritmo");
            OnParryFail?.Invoke();
            return;
        }

        Collider2D[] nearbyProjectiles = Physics2D.OverlapCircleAll(transform.position, parryRadius, projectileLayer);
        
        if (nearbyProjectiles.Length > 0)
        {
            bool parrySuccess = false;
            
            foreach (Collider2D proj in nearbyProjectiles)
            {
                RhythmicProjectile projectile = proj.GetComponent<RhythmicProjectile>();
                if (projectile != null && projectile.CanBeParried())
                {
                    projectile.Parry(transform.position);
                    parrySuccess = true;
                }
            }

            if (parrySuccess)
            {
                Debug.Log("✓ ¡PARRY EXITOSO!");
                OnParrySuccess?.Invoke();
                StartParryCooldown();
                transform.DOPunchScale(Vector3.one * 0.2f, 0.2f, 5, 1f);
            }
        }
        else
        {
            Debug.Log("⚠️ No hay proyectiles cerca");
            OnParryFail?.Invoke();
        }
    }

    bool CheckParryTiming()
    {
        if (Conductor.Instance == null) return true;

        float distanceToBeat = Conductor.Instance.GetDistanceToNearestBeat();
        float tolerance = Conductor.Instance.SecPerBeat * parryWindow;
        
        return Mathf.Abs(distanceToBeat) <= tolerance;
    }

    void StartParryCooldown()
    {
        canParry = false;
        lastParryTime = Time.time;
        Invoke(nameof(ResetParry), parryCooldown);
    }

    void ResetParry()
    {
        canParry = true;
    }

    public void Die()
    {
        if (!IsAlive) return;

        Debug.Log("<color=red>¡GAME OVER! El jugador ha muerto.</color>");
        OnPlayerDeath?.Invoke();
        
        if (animator != null) animator.SetTrigger("Die");
        
        // Opcional: Desactivar el objeto o el control
        // gameObject.SetActive(false); 
    }

    public void Heal(int amount)
    {
        if (!IsAlive) return;
        
        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        
        OnHealthChanged?.Invoke(currentHealth);
    }

    void OnDrawGizmosSelected()
    {
        // Visualizar radio de parry en el editor
        Gizmos.color = canParry ? Color.cyan : Color.gray;
        Gizmos.DrawWireSphere(transform.position, parryRadius);
    }
}