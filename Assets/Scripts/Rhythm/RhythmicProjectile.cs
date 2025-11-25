using UnityEngine;
using DG.Tweening;

/// <summary>
/// Proyectil que se mueve al ritmo de la música y puede ser reflejado con parry
/// </summary>
public class RhythmicProjectile : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public Vector2 direction = Vector2.right;
    
    [Header("Rhythm Settings")]
    public int beatsPerMove = 1; // Cuántos beats espera antes de moverse
    public bool moveOnBeat = true; // Si se mueve solo en beats o continuo
    
    [Header("Combat")]
    public int damage = 1;
    public LayerMask playerLayer;
    
    [Header("Parry Settings")]
    public bool canBeParried = true;
    public float parryReflectSpeed = 8f;
    public LayerMask enemyLayer;
    
    private bool isParried = false;
    private int beatCounter = 0;
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        }

        // Suscribirse a los beats
        if (Conductor.Instance != null && moveOnBeat)
        {
            Conductor.Instance.OnBeat.AddListener(OnBeatOccurred);
        }
        else if (!moveOnBeat)
        {
            rb.linearVelocity = direction.normalized * moveSpeed;
        }

        UpdateRotation();
    }

    void OnBeatOccurred()
    {
        if (!moveOnBeat || isParried) return;

        beatCounter++;
        
        if (beatCounter >= beatsPerMove)
        {
            beatCounter = 0;
            MoveStep();
        }
    }

    void MoveStep()
    {
        if (Conductor.Instance == null) return;
        
        Vector2 targetPos = (Vector2)transform.position + direction.normalized;
        transform.DOMove(targetPos, Conductor.Instance.SecPerBeat * 0.8f).SetEase(Ease.OutQuad);
        transform.DOPunchScale(Vector3.one * 0.1f, 0.1f, 1, 0);
    }

    void UpdateRotation()
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public bool CanBeParried()
    {
        return canBeParried && !isParried;
    }

    public void Parry(Vector3 playerPosition)
    {
        if (!canBeParried || isParried) return;

        isParried = true;
        direction = -direction;
        
        if (moveOnBeat && Conductor.Instance != null)
        {
            Conductor.Instance.OnBeat.RemoveListener(OnBeatOccurred);
            moveOnBeat = false;
        }
        
        rb.linearVelocity = direction.normalized * parryReflectSpeed;
        
        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.cyan;
        }
        
        transform.DOPunchScale(Vector3.one * 0.3f, 0.2f, 5, 1f);
        UpdateRotation();
        
        Debug.Log("🔄 Proyectil reflejado!");
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isParried && ((1 << collision.gameObject.layer) & playerLayer) != 0)
        {
            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null && damageable.IsAlive)
            {
                damageable.TakeDamage(damage);
                DestroyProjectile();
            }
        }
        else if (isParried && ((1 << collision.gameObject.layer) & enemyLayer) != 0)
        {
            IDamageable damageable = collision.GetComponent<IDamageable>();
            if (damageable != null && damageable.IsAlive)
            {
                damageable.TakeDamage(damage);
                DestroyProjectile();
            }
        }
        else if (collision.CompareTag("Wall"))
        {
            DestroyProjectile();
        }
    }

    void DestroyProjectile()
    {
        transform.DOScale(Vector3.zero, 0.2f).OnComplete(() => Destroy(gameObject));
    }

    void OnDestroy()
    {
        if (Conductor.Instance != null && moveOnBeat)
        {
            Conductor.Instance.OnBeat.RemoveListener(OnBeatOccurred);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = isParried ? Color.cyan : Color.red;
        Gizmos.DrawRay(transform.position, direction.normalized * 0.5f);
    }
}
