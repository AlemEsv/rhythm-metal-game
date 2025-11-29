using UnityEngine;
using DG.Tweening;

public class RhythmicProjectile : MonoBehaviour
{
    [Header("Settings")]
    private int damage = 1;
    private bool isParried = false;
    private bool canBeParried = true;
    public void Initialize(Vector3 targetPos, float durationSeconds, int dmg)
    {
        this.damage = dmg;

        transform.DOMove(targetPos, durationSeconds)
            .SetEase(Ease.Linear)
            .SetLink(gameObject)
            .OnComplete(() =>
        {
            // Si llega al destino sin chocar, se destruye (o explota)
            if (!isParried) Destroy(gameObject);
        });

        // Rotar hacia el objetivo
        RotateTowards(targetPos);
    }

    void RotateTowards(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Golpeamos al Jugador (si no ha sido reflejado)
        if (!isParried && other.CompareTag("Player"))
        {
            IDamageable player = other.GetComponent<IDamageable>();
            if (player != null && player.IsAlive)
            {
                player.TakeDamage(damage);
                DestroyProjectile();
            }
        }
        // Golpeamos a un Enemigo (si FUE reflejado con Parry)
        else if (isParried && (other.gameObject.layer == LayerMask.NameToLayer("Enemy") || other.CompareTag("Enemy")))
        {
            IDamageable enemy = other.GetComponent<IDamageable>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage * 2); // Doble daño al devolverlo
                DestroyProjectile();
            }
        }
        // CASO 3: Chocamos con una pared
        else if (other.gameObject.layer == LayerMask.NameToLayer("Wall"))
        {
            // Efecto visual opcional
            DestroyProjectile();
        }
    }
    public bool CanBeParried() => canBeParried && !isParried;

    public void Parry(Vector3 playerPos)
    {
        if (isParried) return;

        transform.DOKill();

        isParried = true;
        canBeParried = false; // No se puede hacer parry dos veces

        // Cambiar visualmente
        GetComponent<SpriteRenderer>().color = Color.cyan;
        transform.localScale *= 1.2f;

        // Devolver el proyectil
        Vector3 dirToPlayer = (transform.position - playerPos).normalized;
        Vector3 newTarget = transform.position + (dirToPlayer * 15f); // Mandarlo lejos

        // Se mueve muy rápido al ser devuelto
        transform.DOMove(newTarget, 0.5f)
            .SetEase(Ease.OutCubic)
            .SetLink(gameObject)
            .OnComplete(() => Destroy(gameObject));
        RotateTowards(newTarget);
    }
    private void DestroyProjectile()
    {
        transform.DOKill();
        Destroy(gameObject);
    }
    void OnDestroy()
    {
        transform.DOKill();
    }
}