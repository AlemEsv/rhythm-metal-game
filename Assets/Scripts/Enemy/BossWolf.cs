using UnityEngine;
using System.Collections;
using DG.Tweening;

public class BossWolf : EnemyBase
{
    [Header("Combat Settings")]
    public float attackRange = 1.5f;
    public float attackDamageRadius = 1.0f;
    public LayerMask targetLayer;

    [Header("Movement Settings")]
    public float moveDistancePerBeat = 3f;
    public LayerMask obstacleLayer;
    public float wallCheckDistance = 1.5f;

    [Header("Teleport Settings")]
    public float fallThresholdY = -6.0f;
    public float teleportDelay = 0.5f;

    private readonly Vector3[] teleportPoints = new Vector3[]
    {
        new Vector3(5.9f, 8.3f, 0),
        new Vector3(-5f, 3f, 0),
        new Vector3(23f, 3f, 0)
    };

    [Header("Visuals")]
    public SpriteRenderer spriteRenderer;
    public Animator animator;
    public Rigidbody2D rb;

    private bool isTeleporting = false;
    private int movingDirection = 1;
    private Transform playerTarget;

    protected override void Start()
    {
        base.Start();
        if (rb == null) rb = GetComponent<Rigidbody2D>();

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) playerTarget = p.transform;

        movingDirection = (Random.value > 0.5f) ? 1 : -1;
        FaceDirection();
    }

    void OnDisable()
    {
        isTeleporting = false;
    }

    void Update()
    {
        if (IsAlive && !isTeleporting)
        {
            if (transform.position.y <= fallThresholdY)
            {
                StartCoroutine(TeleportSequence());
            }
        }
    }

    protected override void PerformRhythmAction()
    {
        if (isTeleporting || !IsAlive || playerTarget == null) return;

        float distToPlayer = Vector2.Distance(transform.position, playerTarget.position);

        if (distToPlayer <= attackRange)
        {
            PerformAttack();
        }
        else
        {
            CheckWallAndMove();
        }
    }

    void PerformAttack()
    {
        if (animator != null) animator.SetTrigger("Attack");

        Vector2 hitOrigin = (Vector2)transform.position + (Vector2.right * movingDirection * 1.0f);
        Collider2D hit = Physics2D.OverlapCircle(hitOrigin, attackDamageRadius, targetLayer);

        if (hit != null)
        {
            IDamageable damageable = hit.GetComponent<IDamageable>();
            if (damageable == null) damageable = hit.GetComponentInParent<IDamageable>();

            if (damageable != null && damageable.IsAlive)
            {
                damageable.TakeDamage(1);
            }
        }
    }

    void CheckWallAndMove()
    {
        Vector2 rayOrigin = transform.position;
        Vector2 rayDir = Vector2.right * movingDirection;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDir, wallCheckDistance, obstacleLayer);

        if (hit.collider != null)
        {
            movingDirection *= -1;
            FaceDirection();
        }
        MoveHorizontal();
    }

    void MoveHorizontal()
    {
        float targetX = rb.position.x + (movingDirection * moveDistancePerBeat);
        if (animator != null) animator.SetTrigger("Run");
        rb.DOMoveX(targetX, 0.2f).SetEase(Ease.OutQuad);
    }

    IEnumerator TeleportSequence()
    {
        isTeleporting = true;

        // Congelar Físicas
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        // Desaparecer
        if (spriteRenderer != null) spriteRenderer.DOFade(0f, 0.3f);

        yield return new WaitForSeconds(teleportDelay);

        // Mover
        if (teleportPoints.Length > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, teleportPoints.Length);
            Vector3 randomPoint = teleportPoints[randomIndex];

            transform.localPosition = randomPoint;
            if (rb != null) rb.position = transform.position;
        }

        // Reaparecer
        if (spriteRenderer != null) spriteRenderer.DOFade(1f, 0.2f);

        yield return new WaitForSeconds(0.2f);

        movingDirection = (UnityEngine.Random.value > 0.5f) ? 1 : -1;
        FaceDirection();

        if (rb != null) rb.bodyType = RigidbodyType2D.Dynamic;

        isTeleporting = false;
    }

    void FaceDirection()
    {
        transform.localScale = new Vector3(movingDirection, 1, 1);
    }

    public override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        if (IsAlive && animator != null) animator.SetTrigger("Hit");
    }

    public override void Die()
    {
        base.Die();
        StopAllCoroutines();
        transform.DOKill();
        if (animator != null) animator.SetTrigger("Die");
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Static;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector3 attackCenter = transform.position + (Vector3.right * movingDirection * 1.0f);
        Gizmos.DrawWireSphere(attackCenter, attackDamageRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3.right * movingDirection * wallCheckDistance));
    }
}