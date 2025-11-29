using UnityEngine;

public class HazardDamage : MonoBehaviour
{
    public int damage = 1;
    public bool instantKill = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            IDamageable player = other.GetComponent<IDamageable>();
            if (player != null)
            {
                if (instantKill) player.Die();
                else player.TakeDamage(damage);
            }
        }
    }
}