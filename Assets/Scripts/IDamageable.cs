using UnityEngine;

/// <summary>
/// Interfaz para cualquier entidad que pueda recibir daño
/// </summary>
public interface IDamageable
{
    void TakeDamage(int damage);
    void Die();
    bool IsAlive { get; }
}
