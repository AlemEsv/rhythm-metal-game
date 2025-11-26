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

    [Header("Parry")]
    public int maxParryCharges = 4;
    public float chargeRegenTime = 20f;
    public float parryWindow = 0.2f;
    public LayerMask projectileLayer;
    public float parryRadius = 1.5f;

    // Estado de Parry
    public int CurrentParryCharges { get; private set; }
    private float regenTimer = 0f;

    // Estado de Combate
    private bool isInvulnerable = false;
    private float invulnerabilityTimer = 0f;

    [Header("Visual")]
    public Animator animator;

    // Eventos
    public static event Action<int> OnHealthChanged;
    public static event Action OnPlayerDeath;
    public static event Action<Vector3> OnPlayerAttack;
    public static event Action OnParrySuccess;
    public static event Action OnParryFail;

    public bool IsAlive => currentHealth > 0;

    void Awake()
    {
        // Iniciar con la barra llena
        CurrentParryCharges = maxParryCharges;
    }

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

        // Regeneración de Parry
        HandleChargeRegeneration();

        // Parry
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(1))
        {
            AttemptParry();
        }
    }

    // LÓGICA DE PARRY
    void HandleChargeRegeneration()
    {
        // Solo regeneramos si no estamos llenos
        if (CurrentParryCharges < maxParryCharges)
        {
            regenTimer += Time.deltaTime;

            // Si se completa el tiempo, ganamos una carga
            if (regenTimer >= chargeRegenTime)
            {
                CurrentParryCharges++;
                regenTimer = 0f; // Reseteamos timer para la siguiente carga

                // Si todavía falta para el máximo, mantenemos el remanente por si hubo lag
                if (CurrentParryCharges > maxParryCharges)
                    CurrentParryCharges = maxParryCharges;
            }
        }
        else
        {
            regenTimer = 0f;
        }
    }

    public float GetParryBarFillAmount()
    {
        if (CurrentParryCharges == maxParryCharges) return 1f;

        float singleChargeFraction = 1f / maxParryCharges;
        float currentBaseFill = (float)CurrentParryCharges / maxParryCharges;
        float nextChargeProgress = regenTimer / chargeRegenTime;

        return currentBaseFill + (nextChargeProgress * singleChargeFraction);
    }

    void AttemptParry()
    {
        // Verificar si tenemos cargas
        if (CurrentParryCharges <= 0)
        {
            // sonido de "Error"
            return;
        }

        // Verificar Ritmo
        if (!CheckParryTiming())
        {
            Debug.Log("Parry fuera de ritmo");
            ConsumeParryCharge(); // Castigo: Gastas carga por fallar el ritmo
            OnParryFail?.Invoke();
            return;
        }

        // Verificar Proyectiles Cercanos
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
                Debug.Log("PARRY EXITOSO");
                ConsumeParryCharge(); // Gastamos la carga al usarla

                OnParrySuccess?.Invoke();
                transform.DOPunchScale(Vector3.one * 0.2f, 0.2f, 5, 1f);
            }
        }
        else
        {
            Debug.Log("No hay proyectiles cerca");
            ConsumeParryCharge(); // Gastas carga si haces parry al aire
            OnParryFail?.Invoke();
        }
    }

    void ConsumeParryCharge()
    {
        CurrentParryCharges--;
        // No reseteamos el regenTimer para que siga cargando la siguiente barra en segundo plano
        // a menos que estuvieramos al máximo, entonces empieza de 0.
        if (CurrentParryCharges == maxParryCharges - 1) regenTimer = 0f;
    }

    bool CheckParryTiming()
    {
        if (Conductor.Instance == null) return true;

        float distanceToBeat = Conductor.Instance.GetDistanceToNearestBeat();
        float tolerance = Conductor.Instance.SecPerBeat * parryWindow;

        return Mathf.Abs(distanceToBeat) <= tolerance;
    }

    // LÓGICA DE COMBATE
    public void Attack()
    {
        if (!IsAlive) return;

        if (animator != null) animator.SetTrigger("Attack");
        OnPlayerAttack?.Invoke(transform.position + transform.up);
    }

    public void Attack(Vector3 targetPosition, IDamageable target)
    {
        if (!IsAlive) return;

        Attack();

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

    public void Die()
    {
        if (!IsAlive) return;

        Debug.Log("<color=red>GAME OVER</color>");
        OnPlayerDeath?.Invoke();

        if (animator != null) animator.SetTrigger("Die");
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
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, parryRadius);
    }
}