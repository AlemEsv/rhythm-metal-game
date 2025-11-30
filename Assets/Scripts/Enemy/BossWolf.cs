using UnityEngine;
using System.Collections;
using DG.Tweening;

public class BossWolf : EnemyBase
{
    [Header("Movement Settings")]
    public float moveDistancePerBeat = 3f;
    public LayerMask obstacleLayer;
    public float wallCheckDistance = 1.5f;

    [Header("Teleport Settings")]
    public float fallThresholdY = -6f;
    public float teleportDelay = 0.5f;

    // Puntos de respawn
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

    // Estados 
    private bool isTeleporting = false;
    private int movingDirection = 1; // 1 = Derecha, -1 = Izquierda

    protected override void Start()
    {
        base.Start();
        if (rb == null) rb = GetComponent<Rigidbody2D>();

        // Elegir direccion inicial aleatoria
        movingDirection = (Random.value > 0.5f) ? 1 : -1;
        FaceDirection();
    }

    void Update()
    {
        if (IsAlive && !isTeleporting && transform.position.y <= fallThresholdY)
        {
            Debug.Log($"Caída detectada en Y={transform.position.y}. Iniciando rescate...");
            StartCoroutine(TeleportSequence());
        }
    }

    protected override void PerformRhythmAction()
    {
        if (isTeleporting || !IsAlive) return;
        CheckWallAndMove();
    }

    void CheckWallAndMove()
    {
        // Detectar pared
        Vector2 rayOrigin = transform.position;
        Vector2 rayDir = Vector2.right * movingDirection;

        // Lanzar Raycast
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDir, wallCheckDistance, obstacleLayer);

        if (hit.collider != null)
        {
            // Si mira pared, ir direcci�n contraria
            movingDirection *= -1;
            FaceDirection();
            Debug.Log("Pared detectada");
        }

        // 2. Moverse
        MoveHorizontal();
    }

    void MoveHorizontal()
    {
        // Calcular destino
        float targetX = rb.position.x + (movingDirection * moveDistancePerBeat);

        // Animaci�n
        if (animator != null) animator.SetTrigger("Run");

        // Movimiento ffsico
        rb.DOMoveX(targetX, 0.2f).SetEase(Ease.OutQuad);
    }

    void PerformAttack()
    {
        if (animator != null) animator.SetTrigger("Attack");
        Debug.Log("El Lobo lanza un mordisco");

        Vector2 hitOrigin = (Vector2)transform.position + (Vector2.right * movingDirection * 1.0f);

        // Buscamos colisionadores
        Collider2D hit = Physics2D.OverlapCircle(hitOrigin, 1.0f, LayerMask.GetMask("Player"));

        if (hit != null)
        {
            IDamageable playerDamage = hit.GetComponent<IDamageable>();
            if (playerDamage != null && playerDamage.IsAlive)
            {
                playerDamage.TakeDamage(1);
                Debug.Log("El jugador fue mordido");
            }
        }
    }
    IEnumerator TeleportSequence()
    {
        isTeleporting = true;
        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = true;

        // Desaparecer
        if (animator != null) animator.SetTrigger("TeleportOut");
        spriteRenderer.DOFade(0f, 0.3f);

        yield return new WaitForSeconds(teleportDelay);

        // Elegir punto aleatorio
        int randomIndex = Random.Range(0, teleportPoints.Length);
        Vector3 randomPoint = teleportPoints[randomIndex];

        transform.position = randomPoint;
        Debug.Log($"Boss Teletransportado a punto {randomIndex}: {randomPoint}");

        // Reaparecer
        if (animator != null) animator.SetTrigger("TeleportIn");
        spriteRenderer.DOFade(1f, 0.2f);

        yield return new WaitForSeconds(0.2f);

        // Elegir nueva direccion aleatoria al reaparecer
        movingDirection = (Random.value > 0.5f) ? 1 : -1;
        FaceDirection();

        if (rb != null) rb.bodyType = RigidbodyType2D.Dynamic;
        isTeleporting = false;
    }

    void FaceDirection()
    {
        // Voltear sprite segun direccion
        transform.localScale = new Vector3(movingDirection, 1, 1);
    }

    // Dibujar linea
    void OnDrawGizmos()
    {
        // Linea de visión para paredes
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3.right * movingDirection * wallCheckDistance));

        // Área de Ataque (Esfera amarilla)
        Gizmos.color = Color.yellow;
        Vector3 attackCenter = transform.position + (Vector3.right * movingDirection * 1.0f);
        Gizmos.DrawWireSphere(attackCenter, 1.0f);
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
}