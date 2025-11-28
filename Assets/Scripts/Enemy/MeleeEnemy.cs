using UnityEngine;
using DG.Tweening;

public class MeleeEnemy : EnemyBase
{
    [Header("Movement")]
    public float moveDistance = 1f;
    public float moveDuration = 0.2f;
    public float attackRange = 1.2f;
    public float verticalJumpForce = 5f;

    private Rigidbody2D rb;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
    }

    protected override void PerformRhythmAction()
    {
        if (playerTransform == null) return;

        float distanceX = Mathf.Abs(transform.position.x - playerTransform.position.x);
        float distanceY = playerTransform.position.y - transform.position.y;

        // Decidir si atacar
        if (Vector2.Distance(transform.position, playerTransform.position) <= attackRange)
        {
            Attack();
        }
        // Decidir si perseguir o escalar
        else
        {
            // Girar sprite
            float dirX = Mathf.Sign(playerTransform.position.x - transform.position.x);
            transform.localScale = new Vector3(dirX, 1, 1);

            // Si el jugador está muy alto, saltar
            if (distanceY > 1.5f && Mathf.Abs(distanceX) < 3f)
            {
                Jump();
            }
            else
            {
                Move(dirX);
            }
        }
    }

    void Move(float dirX)
    {
        // Moverse una casilla del grid hacia el jugador
        transform.DOMoveX(transform.position.x + (dirX * moveDistance), moveDuration).SetEase(Ease.OutQuad);
    }

    void Jump()
    {
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(Vector2.up * verticalJumpForce, ForceMode2D.Impulse);
    }

    void Attack()
    {
        // Animacion de ataque
        transform.DOPunchScale(Vector3.one * 0.2f, 0.1f); //feedback visual

        // Detectar si golpeamos al jugador
        Collider2D hit = Physics2D.OverlapCircle(transform.position, attackRange, LayerMask.GetMask("Player"));
        if (hit != null)
        {
            PlayerCombat player = hit.GetComponent<PlayerCombat>();
            if (player != null) player.TakeDamage(damage);
        }
    }
}