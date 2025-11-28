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
    public int maxParryCharges = 4;
    public float chargeRegenTime = 20f;
    public float parryRadius = 1.5f;
    public LayerMask projectileLayer;

    [Header("Attack layer")]
    public LayerMask enemyLayer;
    public float attackRange = 0.8f;

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
        CurrentParryCharges = maxParryCharges;
    }

    void OnEnable()
    {
        RhythmInput.OnCommandInput += HandleCombatCommand;
    }

    void OnDisable()
    {
        RhythmInput.OnCommandInput -= HandleCombatCommand;
    }

    void OnDestroy()
    {
        transform.DOKill();
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

        HandleChargeRegeneration();
    }

    private void HandleCombatCommand(RhythmActionType type, Vector2 direction)
    {
        if (!IsAlive) return;

        switch (type)
        {
            case RhythmActionType.AttackLeft:
                PerformAttack(Vector2.left);
                break;
            case RhythmActionType.AttackRight:
                PerformAttack(Vector2.right);
                break;
            case RhythmActionType.Parry:
                AttemptParry();
                break;
        }
    }

    void PerformAttack(Vector2 dir)
    {
        if (dir.x != 0)
            transform.localScale = new Vector3(Mathf.Sign(dir.x), 1, 1);

        if (animator != null) animator.SetTrigger("Attack");
        OnPlayerAttack?.Invoke(transform.position + (Vector3)dir);

        Vector2 attackOrigin = (Vector2)transform.position + (dir * 0.5f);
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackOrigin, attackRange, enemyLayer);

        if (hits.Length > 0)
        {
            foreach (var hit in hits)
            {
                IDamageable damageable = hit.GetComponent<IDamageable>();
                if (damageable != null && damageable.IsAlive)
                {
                    damageable.TakeDamage(attackDamage);
                    Debug.Log($"Golpe a {hit.name}");
                }
            }
        }
    }

    void AttemptParry()
    {
        if (animator != null) animator.SetTrigger("Parry");

        if (CurrentParryCharges <= 0)
        {
            Debug.Log("¡Sin cargas de Parry!");
            return;
        }

        Collider2D[] nearbyProjectiles = Physics2D.OverlapCircleAll(transform.position, parryRadius, projectileLayer);
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
            ConsumeParryCharge();
            OnParrySuccess?.Invoke();
            transform.DOPunchScale(Vector3.one * 0.2f, 0.2f, 5, 1f);
        }
        else
        {
            Debug.Log("Parry al aire (Gasta carga)");
            ConsumeParryCharge();
            OnParryFail?.Invoke();
        }
    }

    void HandleChargeRegeneration()
    {
        if (CurrentParryCharges < maxParryCharges)
        {
            regenTimer += Time.deltaTime;

            if (regenTimer >= chargeRegenTime)
            {
                CurrentParryCharges++;
                regenTimer = 0f;
                if (CurrentParryCharges > maxParryCharges) CurrentParryCharges = maxParryCharges;
            }
        }
        else
        {
            regenTimer = 0f;
        }
    }

    void ConsumeParryCharge()
    {
        CurrentParryCharges--;
        if (CurrentParryCharges == maxParryCharges - 1) regenTimer = 0f;
    }

    public float GetParryBarFillAmount()
    {
        if (CurrentParryCharges == maxParryCharges) return 1f;

        float singleChargeFraction = 1f / maxParryCharges;
        float currentBaseFill = (float)CurrentParryCharges / maxParryCharges;
        float nextChargeProgress = regenTimer / chargeRegenTime;

        return currentBaseFill + (nextChargeProgress * singleChargeFraction);
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
        transform.DOKill();
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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.right * 0.5f, attackRange);
    }
}