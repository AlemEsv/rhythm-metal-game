using UnityEngine;

public class RhythmProjectile : MonoBehaviour
{
    public int damage = 1;
    public float speed = 5f;
    public Vector2 direction;

    void Update()
    {
        // Movimiento simple (puedes hacerlo por casillas si prefieres grid)
        transform.Translate(direction * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Si choca con el jugador
        if (other.CompareTag("Player"))
        {
            IDamageable target = other.GetComponent<IDamageable>();
            if (target != null)
            {
                target.TakeDamage(damage); // Esto activará la lógica de Parry en PlayerCombat
            }
            Destroy(gameObject); // El proyectil desaparece
        }
        // Si choca con paredes
        else if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}